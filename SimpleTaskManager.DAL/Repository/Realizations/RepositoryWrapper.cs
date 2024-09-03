using SimpleTaskManager.DAL.Repository.Interfaces.Tasks;
using SimpleTaskManager.DAL.Repository.Interfaces;
using SimpleTaskManager.DAL.Repository.Realizations.Tasks;
using SimpleTaskManager.DAL.Repository.Interfaces.Users;
using SimpleTaskManager.DAL.Repository.Realizations.Users;

namespace SimpleTaskManager.DAL.Repository.Realizations
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly SimpleTaskManagerDbContext _dbContext;

        private readonly Lazy<ITaskRepository> _taskRepository;
        private readonly Lazy<IUserRepository> _userRepository;

        public RepositoryWrapper(SimpleTaskManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            _taskRepository = new Lazy<ITaskRepository>(() => new TaskRepository(_dbContext));
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_dbContext));
        }

        public ITaskRepository TaskRepository => _taskRepository.Value;
        public IUserRepository UserRepository => _userRepository.Value;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
