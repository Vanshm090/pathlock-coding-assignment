using System.ComponentModel.DataAnnotations;

namespace MiniProjectApi.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Foreign key and navigation property for the Project
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}