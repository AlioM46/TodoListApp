using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.DTOs;
using TodoListApi.Interfaces;

namespace TodoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthentication _AuthService) : ControllerBase
    {



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestInfo)
        {
            var res = await _AuthService.Login(loginRequestInfo);

            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRequestDto registerRequestInfo)
        {
            var res = await _AuthService.Register(registerRequestInfo);

            if (res.IsSuccess)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
                return Ok("You've logged out successfully.");
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> Register(RefreshTokenDto refreshTokenRequest )
        {
            string res = await _AuthService.RefreshToken(refreshTokenRequest.RefreshToken);

            if (!string.IsNullOrEmpty(res))
            {
                return Ok(new {AccessToken = res });
            }
            else
            {
                return BadRequest("sorry, you have to login again.");
            }
        }



    }
}
