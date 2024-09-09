using Moq;
using SimpleTaskManager.BLL.Configurations;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Services;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;
using System.Linq.Expressions;

namespace SimpleTaskManager.xUnitTests.UserServiceTests
{
    public class AuthenticateUserAsyncTests
    {
        private readonly PasswordConfiguration _passwordConfiguration;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly UserService _userService;

        public AuthenticateUserAsyncTests()
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
        public async System.Threading.Tasks.Task AuthenticateUserAsync_ShouldReturnUser_WhenCredentialsValid()
        {
            //Arrange
            var loginUserDto = new LoginUserDTO
            {
                Login = "testuser",
                Password = "password123"
            };

            var user = new User
            {
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            };

            SetupMock(user);

            //Act
            var result = await _userService.AuthenticateUserAsync(loginUserDto);

            //Assert
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async System.Threading.Tasks.Task AuthenticateUserAsync_ShouldReturnNull_WhenNoUserFound()
        {
            // Arrange
            var loginUserDto = new LoginUserDTO
            {
                Login = "nonexistentuser",
                Password = "password123"
            };

            SetupMock((User)null!);

            // Act
            var result = await _userService.AuthenticateUserAsync(loginUserDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task AuthenticateUserAsync_Should_ReturnNull_When_PasswordIsInvalid()
        {
            // Arrange
            var loginUserDto = new LoginUserDTO
            {
                Login = "testuser",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            };

            SetupMock(user);

            // Act
            var result = await _userService.AuthenticateUserAsync(loginUserDto);

            // Assert
            Assert.Null(result);
        }

        private void SetupMock(User? user)
        {
            _repositoryWrapperMock.Setup(r => r.UserRepository.FirstOrDefaultAsync(It.Is<Expression<Func<User, bool>>>(expr =>
                expr.ToString().Contains("Username"))))
                .ReturnsAsync(user);
        }
    }
}
