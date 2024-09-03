using SimpleTaskManager.DAL.Repository.Interfaces.Tasks;

namespace SimpleTaskManager.DAL.Repository.Realizations.Tasks
{
    public class TaskRepository(SimpleTaskManagerDbContext dbContext) : Repository<Models.Task>(dbContext), ITaskRepository
    {
    }
}
