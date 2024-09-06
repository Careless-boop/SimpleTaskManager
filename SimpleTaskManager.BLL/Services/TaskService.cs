using Microsoft.EntityFrameworkCore;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.DAL.Enums;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;

namespace SimpleTaskManager.BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepositoryWrapper _repository;

        public TaskService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<DAL.Models.Task> CreateTaskAsync(User user, TaskDTO taskData)
        {
            DAL.Models.Task task = new()
            {
                Id = Guid.NewGuid(),
                Title = taskData.Title,
                Description = taskData.Description,
                DueDate = taskData.DueDate,
                Status = taskData.Status,
                Priority = taskData.Priority,
                User = user,
            };

            var result = await _repository.TaskRepository.CreateAsync(task);
            await _repository.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<DAL.Models.Task>> GetTasksBuUserAsync(Guid userId, TaskFilterDTO filter)
        {
            var query = _repository.TaskRepository.FindAll(t => t.UserId == userId);
            return await GetTasksAsync(filter, query);
        }

        public async Task<IEnumerable<DAL.Models.Task>> GetTasksAsync(TaskFilterDTO filter, IQueryable<DAL.Models.Task>? outerQuery = null)
        {
            var query = outerQuery ?? _repository.TaskRepository.FindAll();

            //filtering
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(t => t.Title.Contains(filter.Title));
            }
            if (filter.Status.HasValue)
            {
                query = query.Where(t => t.Status == filter.Status.Value);
            }
            if (filter.DueDate.HasValue)
            {
                query = query.Where(t => t.DueDate == filter.DueDate.Value);
            }
            if (filter.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.Priority.Value);
            }

            //sorting
            if (filter.SortBy.HasValue)
            {
                query = filter.SortBy.Value switch
                {
                    TaskSortBy.Title => filter.SortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                    TaskSortBy.DueDate => filter.SortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                    TaskSortBy.Priority => filter.SortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                    _ => query
                };
            }

            //pagination
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return await query.ToListAsync();
        }

        public async Task<(TaskDTO? Task, string Message)> UpdateTaskAsync(TaskDTO taskData, string taskId, Guid userId)
        {
            var task = await CheckTaskAsync(taskId);
            if (task == null)
            {
                return (null, "Task not found.");
            }

            if(task.UserId != userId)
            {
                return (null,"You don't have permission.");
            }

            //mapping dto and existing task properties

            task.Title = taskData.Title;
            task.Description = taskData.Description;
            task.DueDate = taskData.DueDate;
            task.Status = taskData.Status;
            task.Priority = taskData.Priority;
            task.UpdatedAt = DateTime.UtcNow;

            _repository.TaskRepository.Update(task);
            await _repository.SaveChangesAsync();
            
            return (taskData,"Updated successfully.");
        }

        public async Task<(bool IsDeleted, string Message)> DeleteTaskAsync(string taskId, Guid userId)
        {
            var task = await CheckTaskAsync(taskId);
            if (task == null)
            {
                return (false, "Task not found.");
            }

            if (task.UserId != userId)
            {
                return (false, "You don't have permission.");
            }

            _repository.TaskRepository.Delete(task);
            await _repository.SaveChangesAsync();

            return (true, "Deleted successfully.");
        }

        public async Task<DAL.Models.Task?> GetTaskById(string taskId, Guid userId)
        {
            var task = await CheckTaskAsync(taskId);
            if (task == null)
            {
                return null;
            }

            if (task.UserId != userId)
            {
                return null;
            }

            task.User = null!;

            return task;
        }

        private async Task<DAL.Models.Task?> CheckTaskAsync(string taskId)
        {
            var task = await _repository.TaskRepository.FirstOrDefaultAsync(t => t.Id == new Guid(taskId));

            return task;
        }
    }
}
