using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.DTOs;
using TodoListApi.Interfaces;
using TodoListApi.Models;

namespace TodoListApi.Services
{
    public class TaskServices(AppDbContext _dbContext) : ITask
    {

   

        public async Task<Message> AddTask(TaskRequestDto taskRequestInfo)
        {
            if (taskRequestInfo == null)
                return new Message { IsSuccess = false, Information = "Task data is empty." };

            if (string.IsNullOrWhiteSpace(taskRequestInfo.Title) || string.IsNullOrWhiteSpace(taskRequestInfo.Description))
                return new Message { IsSuccess = false, Information = "Title and Description are required." };

            bool userExists = await _dbContext.Users.AnyAsync(u => u.Id == taskRequestInfo.UserId);
            if (!userExists)
                return new Message { IsSuccess = false, Information = "User does not exist." };

            var newTask = new Models.Task
            {
                Title = taskRequestInfo.Title,
                Description = taskRequestInfo.Description,
                FinishDate = taskRequestInfo.FinishDate,
                UserId = taskRequestInfo.UserId
            };

            await _dbContext.Tasks.AddAsync(newTask);
            int rows = await _dbContext.SaveChangesAsync();

            if (rows > 0)
                return new Message { IsSuccess = true, Information = "Task created successfully." };

            return new Message { IsSuccess = false, Information = "Failed to create task." };
        }

        public async Task<Message> DeleteTask(int taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);

            if (task == null)
                return new Message { IsSuccess = false, Information = "Task not found." };

            _dbContext.Tasks.Remove(task);
            int rows = await _dbContext.SaveChangesAsync();

            if (rows > 0)
                return new Message { IsSuccess = true, Information = "Task deleted successfully." };

            return new Message { IsSuccess = false, Information = "Failed to delete task." };
        }

        public async Task<TaskResponseDto> FindTask(int taskId)
        {
            var task = await _dbContext.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId);

            // FirstOrDefault => Returns null or the object

            if (task == null)
                return null;

            return new TaskResponseDto
            {
                Title = task.Title,
                Description = task.Description,
                CreatedDate = task.CreatedDate,
                LastUpdateDate = task.LastUpdateDate,
                FinishDate = task.FinishDate,
                Status = task.Status,
                UserId = task.UserId
            };
        }

        public async Task<List<TaskResponseDto>> GetAllTasks()
        {
            var tasks = await _dbContext.Tasks
                .AsNoTracking()
                .Select(task => new TaskResponseDto
                {
                    Title = task.Title,
                    Description = task.Description,
                    CreatedDate = task.CreatedDate,
                    LastUpdateDate = task.LastUpdateDate,
                    FinishDate = task.FinishDate,
                    Status = task.Status,
                    UserId = task.UserId
                }).ToListAsync();

            return tasks;
        }

        public async Task<Message> UpdateTask(int taskId, TaskRequestDto taskRequestInfo)
        {
            if (taskRequestInfo == null)
                return new Message { IsSuccess = false, Information = "Task data is empty." };

            if (string.IsNullOrWhiteSpace(taskRequestInfo.Title) || string.IsNullOrWhiteSpace(taskRequestInfo.Description))
                return new Message { IsSuccess = false, Information = "Title and Description are required." };

            var task = await _dbContext.Tasks.FindAsync(taskId);

            if (task == null)
                return new Message { IsSuccess = false, Information = "Task not found." };

            bool userExists = await _dbContext.Users.AnyAsync(u => u.Id == taskRequestInfo.UserId);
            if (!userExists)
                return new Message { IsSuccess = false, Information = "User does not exist." };

            task.Title = taskRequestInfo.Title;
            task.Description = taskRequestInfo.Description;
            task.FinishDate = taskRequestInfo.FinishDate;
            task.UserId = taskRequestInfo.UserId;
            task.LastUpdateDate = DateTime.UtcNow;

            _dbContext.Tasks.Update(task);
            int rows = await _dbContext.SaveChangesAsync();

            if (rows > 0)
                return new Message { IsSuccess = true, Information = "Task updated successfully." };

            return new Message { IsSuccess = false, Information = "Failed to update task." };
        }
    }
}
