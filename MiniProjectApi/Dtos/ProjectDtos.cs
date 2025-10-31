using System.ComponentModel.DataAnnotations;

namespace MiniProjectApi.Dtos
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public List<TaskDto> Tasks { get; set; } = new(); // We'll create TaskDto next
    }
}