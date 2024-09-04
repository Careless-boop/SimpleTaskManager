using SimpleTaskManager.DAL.Models;

namespace SimpleTaskManager.BLL.Interfaces
{
    public interface IJwtTokensService
    {
        string GenerateAccessToken(User user);
        Task<User?> GetUserFromAccessTokenAsync(string accessToken);
    }
}
