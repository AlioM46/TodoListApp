using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoListApi.Data;
using TodoListApi.DTOs;
using TodoListApi.Interfaces;
using TodoListApi.Models;
using TodoListApi.Options;

namespace TodoListApi.Services
{
    public class AuthenticationServices(AppDbContext _dbContext, JwtOptions _jwtSettings, IUser _userService) : IAuthentication
    {

        PasswordHasher<LoginRequestDto> passwordHasher = new();
        public async Task<Message<LoginResponseDto>> Login(LoginRequestDto loginRequestInfo)
        {
            if (loginRequestInfo == null || string.IsNullOrEmpty(loginRequestInfo.Username) || string.IsNullOrEmpty(loginRequestInfo.Password))
            {
                return new Message<LoginResponseDto> { IsSuccess = false, Information = "Invalid Credentials!" };
            }


            var NewHashedPassword = passwordHasher.HashPassword(loginRequestInfo, loginRequestInfo.Password);
            var userInfo = await _dbContext.Users.FirstOrDefaultAsync((e) => e.Username == loginRequestInfo.Username && e.HashedPassword == NewHashedPassword);

            if (userInfo == null)
            {
                return new Message<LoginResponseDto> { Information = "login has failed, try again later.", IsSuccess = false };

            }
            return new Message<LoginResponseDto> { Information = "you have logged in successfully.", Data = GenerateLoginResponse(userInfo) };
        }

        public async Task<bool> Logout()
        {
            return true;
        }

        public async Task<Message<LoginResponseDto>> Register(UserRequestDto registerRequestInfo)
        {
            var result = await _userService.AddUser(registerRequestInfo);
            if (!result.IsSuccess)
            {
                return new Message<LoginResponseDto> {IsSuccess = false, Information = result.Information};
            }

            var newUser = await _dbContext.Users.FindAsync(result.Data);

            if (newUser == null) {
                return new Message<LoginResponseDto> { IsSuccess = false, Information = "user is not found." };
            }

            return new Message<LoginResponseDto> { IsSuccess = true, Information= "you have registered successfully.", Data = GenerateLoginResponse(newUser)};
        }


        private async Task<User> ValidateRefreshToken(string RefreshToken, int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null || user.RefreshToken != RefreshToken || user.RefreshTokenExpiryTime >= DateTime.UtcNow)
            {
                return null;
            }

            return user;

        }
        // Refresh token is send by the front-end after store it in cookies.
        public async Task<string> RefreshToken(string RefreshToken, int userId)
        {
            User userInfo = await ValidateRefreshToken(RefreshToken, userId);

            if (userInfo == null)
            {
                return null;
            }

            return GenerateAccessToken(userInfo);

        }


        private LoginResponseDto GenerateLoginResponse(User userInfo)
        {
            string RefreshToken = GenerateRefreshToken();

            userInfo.RefreshToken = RefreshToken;
            userInfo.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            string AccessToken = GenerateAccessToken(userInfo);


            return new LoginResponseDto { AccessToken = AccessToken, RefreshToken = RefreshToken };

        }


        private string GenerateAccessToken(User userInfo)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
            new Claim(ClaimTypes.Name, userInfo.Username),
            new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
            // Add roles or custom claims if needed
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

    }
}
