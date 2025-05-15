using TodoListApi.DTOs;

namespace TodoListApi.Interfaces
{
    public interface IUser
    {
        // [Post] Create User

        Task<Message<int>> AddUser(UserRequestDto userRequestInfo);

        // [Delete] Delete User
        Task<Message<object>> DeleteUser(int userId);


        // [Put] Update User
        Task<Message<UserResponseDto>> UpdateUser(int userId, UserRequestDto userRequestInfo);


        // [GET] Find User
        Task<UserResponseDto> FindUser(int userId);

        // [Get] Get All Users
        Task<List<UserResponseDto>> GetAllUsers();
    }
}
