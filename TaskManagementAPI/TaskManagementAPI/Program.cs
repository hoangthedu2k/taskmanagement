using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagerApi.Data;
using TaskManagerApi.Hubs;

namespace TaskManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            // Cấu hình JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // --- 1. CẤU HÌNH DB CONTEXT ---
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // --- 2. CẤU HÌNH CORS (SỬA LẠI ĐOẠN NÀY) ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Chỉ định rõ domain Angular
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // <--- QUAN TRỌNG: THÊM DÒNG NÀY ĐỂ CHẠY SIGNALR
                });
            });
            // ---------------------------------------------

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Thứ tự Middleware cực kỳ quan trọng:
            app.UseCors("AllowAngular"); // 1. CORS trước
            app.UseAuthentication();     // 2. Xác thực
            app.UseAuthorization();      // 3. Phân quyền

            app.MapControllers();
            app.MapHub<TaskHub>("/taskhub"); // Map Hub

            app.Run();
        }
    }
}