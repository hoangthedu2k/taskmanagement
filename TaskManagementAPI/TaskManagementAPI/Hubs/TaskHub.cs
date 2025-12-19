using Microsoft.AspNetCore.SignalR;

namespace TaskManagerApi.Hubs;

// Class này kế thừa từ Hub của SignalR
public class TaskHub : Hub
{
    // Hàm này để client gọi lên nếu muốn test (nhưng ta sẽ dùng Controller gọi là chính)
    public async Task SendMessage(string message)
    {
        // Gửi tin nhắn cho TẤT CẢ mọi người (Clients.All)
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}