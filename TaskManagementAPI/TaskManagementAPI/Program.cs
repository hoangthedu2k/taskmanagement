
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
namespace TaskManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. CẤU HÌNH DB CONTEXT ---
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            // ------------------------------

            // --- 2. CẤU HÌNH CORS (Để Angular gọi được) ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Địa chỉ Angular
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            // ------------------------------

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAngular"); // Kích hoạt CORS

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
