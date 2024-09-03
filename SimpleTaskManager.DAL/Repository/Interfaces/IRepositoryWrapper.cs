using SimpleTaskManager.DAL.Repository.Interfaces.Tasks;
using SimpleTaskManager.DAL.Repository.Interfaces.Users;

namespace SimpleTaskManager.DAL.Repository.Interfaces
{
    public interface IRepositoryWrapper
    {
        ITaskRepository TaskRepository { get; }
        IUserRepository UserRepository { get; }

        public Task<int> SaveChangesAsync();
    }
}
