using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;

namespace SimpleTaskManager.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public UserService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDTO registerUserDto)
        {
            if (await _repositoryWrapper.UserRepository.AnyAsync(u => u.Email == registerUserDto.Email))
            {
                return false;
            }
            if (await _repositoryWrapper.UserRepository.AnyAsync(u => u.Username == registerUserDto.Username))
            {
                return false;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
            };

            await _repositoryWrapper.UserRepository.CreateAsync(user);
            await _repositoryWrapper.SaveChangesAsync();
            return true;
        }

        public async Task<User?> AuthenticateUserAsync(LoginUserDTO loginUserDto)
        {
            var user = await _repositoryWrapper.UserRepository.FirstOrDefaultAsync(u => u.Username == loginUserDto.Login) ??
                await _repositoryWrapper.UserRepository.FirstOrDefaultAsync(u => u.Email == loginUserDto.Login) ??
                null;

            if (user == null)
            {
                return null;
            }

            bool verified = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash);
            if (!verified)
            {
                return null;
            }

            return user;
        }
    }
}
