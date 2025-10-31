using System.ComponentModel.DataAnnotations;

namespace MiniProjectApi.Dtos
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
    
    public class UpdateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int ProjectId { get; set; }
    }
}