using Microsoft.Extensions.Options;

namespace TodoListApi.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public int ExpiryMinutes { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }

    }
}
