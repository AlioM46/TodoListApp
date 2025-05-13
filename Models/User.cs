using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApi.Models
{

    [Index(nameof(Email), IsUnique = true)]
    public class User
    {

        [Key]
        public int Id { get; set; }


        [EmailAddress]
        [Column(TypeName = "nvarchar(100)")]
        [Required]

        public string Email { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        [Required]

        public string Username { get; set; }

        [Range(4,20)]
        [Required]

        public string HashedPassword { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;


        public Enums.enRoles Role { get; set; } = Enums.enRoles.User;
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; } = null;
        public DateTime? RefreshTokenExpiryTime { get; set; } = null;


        public ICollection<Task> Tasks { get; set; }    

    }
}
