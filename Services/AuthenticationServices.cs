using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    public class AuthenticationServices(AppDbContext _dbContext, IOptions<JwtOptions> _jwtSettings, IUser _userService) : IAuthentication
    {

        PasswordHasher<LoginRequestDto> passwordHasher = new();
        public async Task<Message<LoginResponseDto>> Login(LoginRequestDto loginRequestInfo)
        {
            if (loginRequestInfo == null || string.IsNullOrWhiteSpace(loginRequestInfo.Username) || string.IsNullOrWhiteSpace(loginRequestInfo.Password))
            {
                return new Message<LoginResponseDto> { IsSuccess = false, Information = "Invalid credentials!" };
            }

            string normalizedUsername = loginRequestInfo.Username.Trim().ToLower();

            var userInfo = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername);

            if (userInfo == null)
            {
                return new Message<LoginResponseDto> { IsSuccess = false, Information = "Login failed: user not found." };
            }

            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(
                loginRequestInfo, userInfo.HashedPassword, loginRequestInfo.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return new Message<LoginResponseDto> { IsSuccess = false, Information = "Login failed: incorrect password." };
            }

            return new Message<LoginResponseDto>
            {
                IsSuccess = true,
                Information = "You have logged in successfully.",
                Data = await GenerateLoginResponse(userInfo)
            };
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
                return new Message<LoginResponseDto> { IsSuccess = false, Information = result.Information };
            }

            var newUser = await _dbContext.Users.FindAsync(result.Data);

            if (newUser == null)
            {
                return new Message<LoginResponseDto> { IsSuccess = false, Information = "user is not found." };
            }

            return new Message<LoginResponseDto> { IsSuccess = true, Information = "you have registered successfully.", Data = await GenerateLoginResponse(newUser) };
        }


        private async Task<User> ValidateRefreshToken(string RefreshToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync((u) => u.RefreshToken == RefreshToken);

            if (user == null || user.RefreshToken != RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow || !user.IsActive)
            {
                return null;
            }

            return user;

        }
        // Refresh token is send by the front-end after store it in cookies.
        public async Task<string> RefreshToken(string RefreshToken)
        {
            User userInfo = await ValidateRefreshToken(RefreshToken);

            if (userInfo == null)
            {
                return null;
            }

            return GenerateAccessToken(userInfo, RefreshToken);

        }


        private async Task<LoginResponseDto >GenerateLoginResponse(User userInfo)
        {
            string RefreshToken = GenerateRefreshToken();

            userInfo.RefreshToken = RefreshToken;
            userInfo.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _dbContext.SaveChangesAsync();

            string AccessToken = GenerateAccessToken(userInfo, RefreshToken);
            


            return new LoginResponseDto { AccessToken = AccessToken, RefreshToken = RefreshToken };

        }


        private string GenerateAccessToken(User userInfo, string RefreshToken)
        {

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
            new Claim(ClaimTypes.Name, userInfo.Username),
            new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
            new Claim("IsActive", userInfo.IsActive.ToString()),
            new Claim("RefreshToken", RefreshToken),
            // Add roles or custom claims if needed
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpiryMinutes),
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
