using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;
using TaskManagerApi.Models;

namespace TaskManagerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Khai báo bảng Tasks
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<User> Users { get; set; }

}