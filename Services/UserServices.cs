using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.DTOs;
using TodoListApi.Interfaces;
using TodoListApi.Models;

namespace TodoListApi.Services
{
    public class UserServices(AppDbContext _dbContext) : IUser
    {
        private PasswordHasher<User> _Hasher = new PasswordHasher<User> ();
        public async Task<Message<int>> AddUser(UserRequestDto userRequestInfo)
        {
            if (userRequestInfo == null)
                return new Message<int> { IsSuccess = false, Information = "Object is empty." };

            if (string.IsNullOrEmpty(userRequestInfo.Email) || string.IsNullOrEmpty(userRequestInfo.Username) || string.IsNullOrEmpty(userRequestInfo.Password))
                return new Message<int> { IsSuccess = false, Information = "One or more fields are empty." };

            bool emailExists = await _dbContext.Users.AnyAsync(e => e.Email == userRequestInfo.Email);
            bool usernameExists = await _dbContext.Users.AnyAsync(e => e.Username == userRequestInfo.Username);

            if (emailExists || usernameExists)
                return new Message<int> { IsSuccess = false, Information = "Email or Username already exists." };

            var newUser = new User
            {
                Email = userRequestInfo.Email,
                Username = userRequestInfo.Username,
            };

            newUser.HashedPassword = _Hasher.HashPassword(newUser, userRequestInfo.Password);

            await _dbContext.Users.AddAsync(newUser);
            int rows = await _dbContext.SaveChangesAsync();

            return rows > 0
                ? new Message<int> { IsSuccess = true, Information = "User created successfully, go to login page.", Data = newUser.Id }
                : new Message<int> { IsSuccess = false, Information = "User creation failed, try again." };
        }

        public async Task<Message<object>> DeleteUser(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return new Message<object> { IsSuccess = false, Information = "User not found." };

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return new Message<object> { IsSuccess = true, Information = "User deleted successfully." };
        }

        public async Task<UserResponseDto> FindUser(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserResponseDto
            {
                Email = user.Email,
                Username = user.Username,
                JoinDate = user.JoinDate,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            return await _dbContext.Users
                .Select(user => new UserResponseDto
                {
                    Email = user.Email,
                    Username = user.Username,
                    JoinDate = user.JoinDate,
                    Role = user.Role,
                    IsActive = user.IsActive
                }).ToListAsync();
        }

        public async Task<Message<UserResponseDto>> UpdateUser(int userId, UserRequestDto userRequestInfo)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return new Message<UserResponseDto> { IsSuccess = false, Information = "User not found." };

            if (!string.IsNullOrEmpty(userRequestInfo.Email))

                user.Email = userRequestInfo.Email;

            if (!string.IsNullOrEmpty(userRequestInfo.Username))
                user.Username = userRequestInfo.Username;

            if (!string.IsNullOrEmpty(userRequestInfo.Password))
                user.HashedPassword = _Hasher.HashPassword(user, userRequestInfo.Password);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();


            var userResponseInfo = new UserResponseDto
            {
                Email = user.Email,
                Username = user.Username,
                JoinDate = user.JoinDate,
                Role = user.Role,
                IsActive = user.IsActive
            };


            return new Message<UserResponseDto> { IsSuccess = true, Information = "User updated successfully." , Data = userResponseInfo};
        }
    }
}
