namespace TodoListApi.DTOs
{
    public class TaskRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? FinishDate { get; set; } = null;
        public int UserId { get; set; }
        public Enums.enStatus Status { get; set; } = Enums.enStatus.Pending;


    }
}
