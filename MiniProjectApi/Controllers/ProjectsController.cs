using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjectApi.Data;
using MiniProjectApi.Dtos;
using MiniProjectApi.Models;
using Microsoft.AspNetCore.Authorization; // <-- Required for authentication
using System.Security.Claims; // <-- Required to read User ID from token

namespace MiniProjectApi.Controllers
{
    [Authorize] // <-- This entire controller is protected. User must be logged in.
    [ApiController]
    [Route("api/[controller]")] // This makes the route /api/projects
    public class ProjectsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProjectsController(DataContext context)
        {
            _context = context;
        }

        // Helper function to get the authenticated User's ID from the token
        private int GetUserIdFromToken()
        {
            // The User's ID is stored as "NameIdentifier" in the token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            return int.Parse(userIdString);
        }

        // GET /api/projects
        // Gets all projects for the logged-in user
        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetProjects()
        {
            var userId = GetUserIdFromToken();

            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.Tasks) // Include the tasks for each project
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    CreationDate = p.CreationDate,
                    Tasks = p.Tasks.Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        IsCompleted = t.IsCompleted,
                        DueDate = t.DueDate,
                        ProjectId = t.ProjectId
                    }).ToList()
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET /api/projects/{id}
        // Gets a single project by ID, checking ownership
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var userId = GetUserIdFromToken();

            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.Id == id && p.UserId == userId) // Ensure user owns this project
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    CreationDate = p.CreationDate,
                    Tasks = p.Tasks.Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        IsCompleted = t.IsCompleted,
                        DueDate = t.DueDate,
                        ProjectId = t.ProjectId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound("Project not found or you do not have access.");
            }

            return Ok(project);
        }

        // POST /api/projects
        // Creates a new project for the logged-in user
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto createDto)
        {
            var userId = GetUserIdFromToken();

            var project = new Project
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreationDate = DateTime.UtcNow,
                UserId = userId // Link the project to the logged-in user
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Return a DTO, not the raw model
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreationDate = project.CreationDate,
                Tasks = new List<TaskDto>() // New project has no tasks yet
            };

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
        }

        // DELETE /api/projects/{id}
        // Deletes a project, checking ownership
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = GetUserIdFromToken();

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound("Project not found or you do not have access.");
            }

            // EF Core (with Cascade delete) will handle deleting the associated tasks
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent(); // Success, no content to return
        }
    }
}