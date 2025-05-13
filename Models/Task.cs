using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApi.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName ="nvarchar(50)")]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public  string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

        public DateTime? FinishDate { get; set; } = null;

        [Required]
        public Enums.enStatus Status { get; set; } = Enums.enStatus.Pending;

        [Required]
        public int UserId { get; set; }  

        [ForeignKey(nameof(UserId))]     
        public User User { get; set; }   
    }
}
