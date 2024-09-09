using Moq;
using SimpleTaskManager.BLL.Configurations;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Services;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;
using System.Linq.Expressions;

namespace SimpleTaskManager.xUnitTests.UserServiceTests
{
    public class RegisterUserAsyncTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly PasswordConfiguration _passwordConfiguration;
        private readonly UserService _userService;

        public RegisterUserAsyncTests()
        {
            _passwordConfiguration = new PasswordConfiguration()
            {
                RequiredLenght = 8,
                SpecialSymbols = "!@#$%^&*()_-+=<>?/{}~|"
            };
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _userService = new UserService(_repositoryWrapperMock.Object, _passwordConfiguration);
        }
        [Fact]
        public async System.Threading.Tasks.Task RegisterUserAsync_ShouldReturnFalse_WhenEmailRegistered()
        {
            //Arrange
            SetupMocks(true, true);
            var registerDto = new RegisterUserDTO()
            {
                Email = "existingemail",
                Username = "existingusername",
                Password = "invalid"
            };

            var expected = (false, "Email already registered.");

            //Act
            var result = await _userService.RegisterUserAsync(registerDto);
            
            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task RegisterUserAsync_ShouldReturnFalse_WhenUsernameRegistered()
        {
            //Arrange
            SetupMocks(true, false);
            var registerDto = new RegisterUserDTO()
            {
                Email = "existingemail",
                Username = "existingusername",
                Password = "invalid"
            };

            var expected = (false, "Username already taken.");

            //Act
            var result = await _userService.RegisterUserAsync(registerDto);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task RegisterUserAsync_ShouldReturnFalse_WhenPasswordInvalid()
        {
            //Arrange
            SetupMocks(false, false);
            var registerDto = new RegisterUserDTO()
            {
                Email = "email",
                Username = "username",
                Password = "invalid"
            };

            var expected = (false, $"Password must be at least {_passwordConfiguration.RequiredLenght} characters long!");

            //Act
            var result = await _userService.RegisterUserAsync(registerDto);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task RegisterUserAsync_ShouldReturnTrue_WhenCredentialsValid()
        {
            //Arrange
            SetupMocks(false, false);
            var registerDto = new RegisterUserDTO()
            {
                Email = "email",
                Username = "username",
                Password = "ValidPass#"
            };

            var expected = (true, "User successfully registered.");

            //Act
            var result = await _userService.RegisterUserAsync(registerDto);

            //Assert
            Assert.Equal(expected, result);
        }
        private void SetupMocks(bool result, bool isEmail)
        {
            if (isEmail)
            {
                _repositoryWrapperMock.Setup(r => r.UserRepository.AnyAsync(It.Is<Expression<Func<User, bool>>>(expr =>
                    expr.ToString().Contains("Email")), default))
                    .ReturnsAsync(result);
            }
            else
            {
                _repositoryWrapperMock.Setup(r => r.UserRepository.AnyAsync(It.Is<Expression<Func<User, bool>>>(expr =>
                    expr.ToString().Contains("Username")), default))
                    .ReturnsAsync(result);
            }
            _repositoryWrapperMock.Setup(r => r.TaskRepository.CreateAsync(It.IsAny<DAL.Models.Task>())).ReturnsAsync(new DAL.Models.Task());
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        }
    }
}
