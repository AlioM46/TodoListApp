using Microsoft.AspNetCore.Mvc;
using TodoListApi.DTOs;
using TodoListApi.Interfaces;

namespace TodoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITask _taskService) : ControllerBase
    {


        [HttpPost("task")]
        public async Task<IActionResult> AddTask([FromBody] TaskRequestDto taskRequest)
        {
            var result = await _taskService.AddTask(taskRequest);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("task/{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.FindTask(id);
            if (task == null)
                return NotFound(new { Information = "Task not found." });
            return Ok(task);
        }

        [HttpGet("task")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasks();
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskRequestDto taskRequest)
        {
            var result = await _taskService.UpdateTask(id, taskRequest);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTask(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
