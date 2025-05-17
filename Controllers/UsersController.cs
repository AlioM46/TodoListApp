using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.DTOs;
using TodoListApi.Interfaces;

namespace TodoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Enums.enRoles.Admin))]

    public class UsersController(IUser _userService) : ControllerBase
    {

        [HttpPost("add")]
        public async Task<IActionResult> AddUser( UserRequestDto userDto)
        {
            var result = await _userService.AddUser(userDto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUser(userId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("find/{userId}")]
        public async Task<IActionResult> FindUser(int userId)
        {
            var user = await _userService.FindUser(userId);
            if (user == null)
                return NotFound(new { Message = "User not found." });
            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId,  UserRequestDto userDto)
        {
            var result = await _userService.UpdateUser(userId, userDto);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
    }
}
