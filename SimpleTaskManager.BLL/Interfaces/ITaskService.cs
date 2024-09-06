using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.DAL.Models;

namespace SimpleTaskManager.BLL.Interfaces
{
    public interface ITaskService
    {
        Task<DAL.Models.Task> CreateTaskAsync(User user, TaskDTO taskData);
        Task<IEnumerable<DAL.Models.Task>> GetTasksBuUserAsync(Guid userId, TaskFilterDTO filter);
        Task<IEnumerable<DAL.Models.Task>> GetTasksAsync(TaskFilterDTO filter, IQueryable<DAL.Models.Task> outerQuery);
        Task<(TaskDTO? Task, string Message)> UpdateTaskAsync(TaskDTO taskData, string taskId, Guid userId);
        Task<(bool IsDeleted, string Message)> DeleteTaskAsync(string taskId, Guid userId);
        Task<DAL.Models.Task?> GetTaskById(string taskId, Guid userId);
    }
}
