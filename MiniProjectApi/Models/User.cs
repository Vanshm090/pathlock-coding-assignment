using System.ComponentModel.DataAnnotations;

namespace MiniProjectApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation property: A user can have many projects
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
