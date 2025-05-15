using System.ComponentModel.DataAnnotations;

namespace TodoListApi.DTOs
{
    public class UserRequestDto
    {
        public string Email {  get; set; }
        public string Username {  get; set; }
        public string Password { get; set; }

        
    }
}
