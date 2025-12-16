namespace TaskManagerApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; } // required: Bắt buộc phải có
    public string? Description { get; set; }   // ?: Có thể null
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}