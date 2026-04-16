using Microsoft.EntityFrameworkCore;
using ProfileApi.Data;
using ProfileApi.Services.Implimentation;

namespace ProfileApi
{
    public class Program
    {
        public static async Task Main(string[] args)  // Changed to async Task
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure for PXXL (port 8080) - MUST be before building app
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080);
            });

            // Add services to the container.
            builder.Services.AddScoped<ExternalApiService, ExternalApiService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // SQLite configuration for deployment on PXXL
            var dbPath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "/app/data", "profiles.db");
            var connectionString = $"Data Source={dbPath}";

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            builder.Services.AddHttpClient<ExternalApiService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            // Ensure database is created - FIXED async/await
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
            }

            app.Run();
        }
    }
}