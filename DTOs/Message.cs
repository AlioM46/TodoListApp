namespace TodoListApi.DTOs
{
    public class Message<T>
    {
        public bool IsSuccess { get; set; } = true;
        public string Information { get; set; } = "";
        public T? Data { get; set; } = default;
    }
}
