using TodoListApi.DTOs;

namespace TodoListApi.Interfaces
{
    public interface ITask
    {
        Task<Message> AddTask(TaskRequestDto taskRequestInfo);




        Task<Message> DeleteTask(int taskId);

        Task<Message> UpdateTask(int taskId, TaskRequestDto taskRequestInfo);
    
        Task<TaskResponseDto> FindTask(int taskId);

        Task<List<TaskResponseDto>> GetAllTasks();

    }
}
