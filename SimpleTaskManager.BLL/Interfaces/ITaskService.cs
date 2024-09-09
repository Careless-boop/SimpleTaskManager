using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.DAL.Models;

namespace SimpleTaskManager.BLL.Interfaces
{
    public interface ITaskService
    {
        Task<DAL.Models.Task> CreateTaskAsync(User user, TaskDTO taskData);
        Task<IEnumerable<DAL.Models.Task>> GetTasksByUserAsync(Guid userId, TaskFilterDTO filter);
        Task<(TaskDTO? Task, string Message)> UpdateTaskAsync(TaskDTO taskData, string taskId, Guid userId);
        Task<(bool IsDeleted, string Message)> DeleteTaskAsync(string taskId, Guid userId);
        Task<DAL.Models.Task?> GetTaskById(string taskId, Guid userId);
    }
}
