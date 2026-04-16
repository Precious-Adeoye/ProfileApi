
using Microsoft.EntityFrameworkCore;
using ProfileApi.Data;
using ProfileApi.Services.Implimentation;

namespace ProfileApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<ExternalApiService, ExternalApiService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Configure for PXXL (port 8080)
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080);
            });

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

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
                app.MapOpenApi();
            }

            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                 dbContext.Database.EnsureCreatedAsync();

                // Verify database is writable
                var testQuery =  dbContext.Profiles.AnyAsync();
            }




            app.Run();
        }
    }
}
