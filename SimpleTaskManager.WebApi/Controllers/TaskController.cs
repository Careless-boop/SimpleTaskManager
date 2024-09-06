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

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDTO filter)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.GetTasksBuUserAsync(user!.Id, filter);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody] TaskDTO task)
        {
            var user = HttpContext.Items["User"] as User;

            await _taskService.CreateTaskAsync(user!, task);

            return Ok(task);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> PutTask([FromRoute] string taskId, [FromBody] TaskDTO task)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.UpdateTaskAsync(task, taskId, user!.Id);

            if(result.Task == null)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Task);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] string taskId)
        {
            var user = HttpContext.Items["User"] as User;

            var result = await _taskService.DeleteTaskAsync(taskId, user!.Id);

            if (!result.IsDeleted)
            {
                return BadRequest(result.Message);
            }

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
