using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Add Services ---

// Add services for Swagger/OpenAPI (for testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy to allow the React frontend to call the API
// Assumes React runs on localhost:3000 or 5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- 2. Configure HTTP Pipeline ---

// Use Swagger in development mode for easy testing


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseCors("AllowReactApp"); // Apply the CORS policy

// --- 3. Define In-Memory Storage ---

// In-memory data storage as required
// Using ConcurrentDictionary for thread-safety
var tasks = new ConcurrentDictionary<Guid, TaskItem>();


// --- 4. Define API Endpoints ---

var api = app.MapGroup("/api/tasks");

// GET /api/tasks
// Gets all tasks
api.MapGet("/", () =>
{
    return Results.Ok(tasks.Values.OrderBy(t => t.Description));
});

// POST /api/tasks
// Adds a new task
api.MapPost("/", ([FromBody] CreateTaskDto taskDto) =>
{
    if (string.IsNullOrWhiteSpace(taskDto.Description))
    {
        return Results.BadRequest("Description is required.");
    }
    
    var task = new TaskItem
    {
        Id = Guid.NewGuid(),
        Description = taskDto.Description,
        IsCompleted = false
    };

    tasks[task.Id] = task;

    return Results.Created($"/api/tasks/{task.Id}", task);
});

// PUT /api/tasks/{id}
// Updates a task (e.g., marks as completed)
api.MapPut("/{id}", (Guid id, [FromBody] UpdateTaskDto updatedTaskDto) =>
{
    if (!tasks.TryGetValue(id, out var task))
    {
        return Results.NotFound();
    }
    
    if (string.IsNullOrWhiteSpace(updatedTaskDto.Description))
    {
         return Results.BadRequest("Description is required.");
    }

    // Update the task properties
    task.Description = updatedTaskDto.Description;
    task.IsCompleted = updatedTaskDto.IsCompleted;
    
    tasks[id] = task; // Save the update
    
    return Results.Ok(task);
});

// DELETE /api/tasks/{id}
// Deletes a task
api.MapDelete("/{id}", (Guid id) =>
{
    if (!tasks.TryRemove(id, out _))
    {
        return Results.NotFound();
    }
    
    return Results.NoContent();
});


// --- 5. Run the Application ---

app.Run();


// --- 6. Define Models (MUST be at the end of the file) ---

// The TaskItem model
public class TaskItem
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}

// DTO (Data Transfer Object) for creating a task
// (Client shouldn't send an Id)
public class CreateTaskDto
{
    public string Description { get; set; }
}

// DTO for updating a task
public class UpdateTaskDto
{
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}