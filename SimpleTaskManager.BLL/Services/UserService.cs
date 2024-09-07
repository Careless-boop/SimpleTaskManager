using SimpleTaskManager.BLL.Configurations;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;

namespace SimpleTaskManager.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly PasswordConfiguration _passwordConfiguration;

        public UserService(IRepositoryWrapper repositoryWrapper, PasswordConfiguration passwordConfiguration)
        {
            _repositoryWrapper = repositoryWrapper;
            _passwordConfiguration = passwordConfiguration;
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDTO registerUserDto)
        {
            if (await _repositoryWrapper.UserRepository.AnyAsync(u => u.Email == registerUserDto.Email))
            {
                return (false, "Email already registered.");
            }
            else if (await _repositoryWrapper.UserRepository.AnyAsync(u => u.Username == registerUserDto.Username))
            {
                return (false, "Username already taken.");
            }

            var passwordValidation = ValidatePassword(registerUserDto.Password);

            if (!passwordValidation.isValid)
            {
                return passwordValidation;  
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

            return (true, "User successfully registered.");
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

        private (bool isValid, string ErrorMessage) ValidatePassword(string password)
        {
            if(password.Length < _passwordConfiguration.RequiredLenght)
            {
                return (false, $"Password must be at least {_passwordConfiguration.RequiredLenght} characters long!");
            }
            else if(!password.Any(c => _passwordConfiguration.SpecialSymbols.Contains(c)))
            {
                return (false, "Password must contain at least one special symbol!");
            }
            else
            {
                return (true, "Password is valid.");
            }
        }
    }
}
