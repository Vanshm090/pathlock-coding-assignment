using System.ComponentModel.DataAnnotations;

namespace MiniProjectApi.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        // Foreign key and navigation property for the User
        public int UserId { get; set; }
        public User? User { get; set; }

        // Navigation property: A project can have many tasks
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}