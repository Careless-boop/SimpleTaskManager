using Moq;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.BLL.Services;
using SimpleTaskManager.DAL.Enums;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;

namespace SimpleTaskManager.xUnitTests.TaskServiceTests
{
    public class CreateTaskAsyncTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly ITaskService _taskService;

        public CreateTaskAsyncTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

            _taskService = new TaskService(_repositoryWrapperMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTask_Should_CreateTask_WithCorrectData()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser"
            };

            var taskData = new TaskDTO
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Pending,
                Priority = Priority.Medium
            };

            _repositoryWrapperMock.Setup(r => r.TaskRepository.CreateAsync(It.IsAny<DAL.Models.Task>()))
                .ReturnsAsync((DAL.Models.Task t) => t);

            // Act
            var result = await _taskService.CreateTaskAsync(user, taskData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskData.Title, result.Title);
            Assert.Equal(taskData.Description, result.Description);
            Assert.Equal(taskData.DueDate, result.DueDate);
            Assert.Equal(taskData.Status, result.Status);
            Assert.Equal(taskData.Priority, result.Priority);
            Assert.Equal(user, result.User);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTask_Should_SaveTask_And_CallSaveChanges()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Username = "testuser" };
            var taskData = new TaskDTO
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Pending,
                Priority = Priority.Medium
            };

            _repositoryWrapperMock.Setup(r => r.TaskRepository.CreateAsync(It.IsAny<DAL.Models.Task>()))
                .ReturnsAsync(new DAL.Models.Task());
            // Act
            await _taskService.CreateTaskAsync(user, taskData);

            _repositoryWrapperMock.Verify(r => r.TaskRepository.CreateAsync(It.IsAny<DAL.Models.Task>()), Times.Once);

            _repositoryWrapperMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
