namespace TaskManagementAPI.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public string? FullName { get; set; }
}

// Tạo thêm class DTO (Data Transfer Object) để nhận dữ liệu đăng nhập từ Client
// (Vì Client gửi pass thường, không gửi pass hash)
public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}