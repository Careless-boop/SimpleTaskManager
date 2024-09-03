using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces.Users;

namespace SimpleTaskManager.DAL.Repository.Realizations.Users
{
    public class UserRepository(SimpleTaskManagerDbContext dbContext) : Repository<User>(dbContext), IUserRepository
    {
    }
}
