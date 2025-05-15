namespace TodoListApi.DTOs
{
    public class UserResponseDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public Enums.enRoles Role { get; set; } = Enums.enRoles.User;
        public bool IsActive { get; set; } = true;
    }
}
