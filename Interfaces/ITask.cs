using TodoListApi.DTOs;

namespace TodoListApi.Interfaces
{
    public interface ITask
    {
        Task<Message<object>> AddTask(TaskRequestDto taskRequestInfo);




        Task<Message<object>> DeleteTask(int taskId);

        Task<Message<object>> UpdateTask(int taskId, TaskRequestDto taskRequestInfo);
    
        Task<TaskResponseDto> FindTask(int taskId);

        Task<List<TaskResponseDto>> GetAllTasks();

    }
}
