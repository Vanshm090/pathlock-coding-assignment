using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjectApi.Data;
using MiniProjectApi.Dtos;
using MiniProjectApi.Models;
using System.Security.Claims;

namespace MiniProjectApi.Controllers
{
    [Authorize]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly DataContext _context;

        public TasksController(DataContext context)
        {
            _context = context;
        }

        // Helper function to get the authenticated User's ID
        private int GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new InvalidOperationException("User ID not found in token.");
            }
            return int.Parse(userIdString);
        }

        // Helper function to check if a user owns the project
        // This is crucial for security
        private async Task<Project?> GetUserProject(int projectId, int userId)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        }

        // POST /api/projects/{projectId}/tasks
        
        [HttpPost("api/projects/{projectId}/tasks")]
        public async Task<ActionResult<TaskDto>> CreateTask(int projectId, CreateTaskDto createDto)
        {
            var userId = GetUserIdFromToken();
            var project = await GetUserProject(projectId, userId);

            if (project == null)
            {
                return Forbid("You do not have access to this project.");
            }

            var task = new Models.Task
            {
                Title = createDto.Title,
                DueDate = createDto.DueDate,
                IsCompleted = false,
                ProjectId = projectId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                ProjectId = task.ProjectId
            };

            return CreatedAtAction(nameof(GetTask), new { taskId = task.Id }, taskDto);
        }

        // This is a helper action for the CreatedAtAction above
        // It's not part of the required endpoints but is good practice
        [HttpGet("api/tasks/{taskId}", Name = "GetTask")]
        public async Task<ActionResult<TaskDto>> GetTask(int taskId)
        {
            var userId = GetUserIdFromToken();
            var task = await _context.Tasks
                .Include(t => t.Project)
                .Where(t => t.Id == taskId && t.Project.UserId == userId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    ProjectId = t.ProjectId
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }


        // PUT /api/tasks/{taskId
        
        [HttpPut("api/tasks/{taskId}")]
        public async Task<IActionResult> UpdateTask(int taskId, UpdateTaskDto updateDto)
        {
            var userId = GetUserIdFromToken();
            var task = await _context.Tasks
                .Include(t => t.Project) // Load the parent project
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            if (task.Project?.UserId != userId)
            {
                return Forbid("You do not have access to this task.");
            }

            // Update properties
            task.Title = updateDto.Title;
            task.DueDate = updateDto.DueDate;
            task.IsCompleted = updateDto.IsCompleted;

            await _context.SaveChangesAsync();

            return NoContent(); // Success, no content
        }

        // DELETE /api/tasks/{taskId}
        
        [HttpDelete("api/tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var userId = GetUserIdFromToken();
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            if (task.Project?.UserId != userId)
            {
                return Forbid("You do not have access to this task.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}