namespace TodoListApi.DTOs
{
    public class TaskResponseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public DateTime? FinishDate { get; set; } = null;
        public Enums.enStatus Status { get; set; } = Enums.enStatus.Pending;
        public int UserId { get; set; }

    }
}
