using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Models;
using Microsoft.AspNetCore.SignalR; // Import
using TaskManagerApi.Hubs;          // Import
namespace TaskManagerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<TaskHub> _hubContext; // Thêm HubContext
    public TasksController(AppDbContext context, IHubContext<TaskHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // 1. GET: api/tasks (Lấy danh sách)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        return await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    // 2. GET: api/tasks/5 (Lấy chi tiết 1 cái)
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        return task;
    }

    // 3. POST: api/tasks (Tạo mới)
    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow; // Set ngày tạo
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("TaskCreated", task); // Gửi thông báo tới tất cả client
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    // 4. PUT: api/tasks/5 (Cập nhật)
    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItem>> UpdateTask(int id, TaskItem task)
    {
        if (id != task.Id) return BadRequest();

        _context.Entry(task).State = EntityState.Modified;

        try
        {
          
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("TaskUpdated", task); // Gửi thông báo tới tất cả client
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tasks.Any(e => e.Id == id)) return NotFound();
            else throw;
        }

        return task;
    }

    // 5. DELETE: api/tasks/5 (Xóa)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("TaskDeleted", id); // Gửi thông báo tới tất cả client

        return NoContent();
    }
}