using TodoListApi.DTOs;

namespace TodoListApi.Interfaces
{
    public interface IAuthentication
    {
        Task<Message<LoginResponseDto>> Login(LoginRequestDto loginRequestInfo);
        Task<Message<LoginResponseDto>> Register(UserRequestDto registerRequestInfo);

        Task<bool> Logout();
    }
}
