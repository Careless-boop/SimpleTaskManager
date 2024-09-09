using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.DAL.Models;

namespace SimpleTaskManager.WebApi.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDTO filter)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.GetTasksByUserAsync(user!.Id, filter);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody] TaskDTO taskData)
        {
            var user = HttpContext.Items["User"] as User;

            var task = await _taskService.CreateTaskAsync(user!, taskData);

            _logger.LogInformation($"User {user!.Email} created task with id - {task.Id}");

            return Ok(taskData);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> PutTask([FromRoute] string taskId, [FromBody] TaskDTO task)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.UpdateTaskAsync(task, taskId, user!.Id);

            if(result.Task == null)
            {
                _logger.LogWarning($"User {user.Email} failed to update task with id - {taskId}\n\t" +
                    $"Message: {result.Message}");
                return BadRequest(result.Message);
            }

            _logger.LogInformation($"User {user.Email} updated task with id - {taskId}");
            return Ok(result.Task);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] string taskId)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.DeleteTaskAsync(taskId, user!.Id);

            if (!result.IsDeleted)
            {
                _logger.LogWarning($"User {user.Email} failed to deleted task with id - {taskId}\n\t" +
                    $"Message: {result.Message}");
                return BadRequest(result.Message);
            }

            _logger.LogInformation($"User {user.Email} deleted task with id - {taskId}");
            return Ok(result.Message);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById([FromRoute] string taskId)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.GetTaskById(taskId, user!.Id);

            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
