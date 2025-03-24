
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServerSentEvents_POC_API.Services;

namespace ServerSentEvents_POC_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()   // Allows requests from any origin
                           .AllowAnyMethod()   // Allows all HTTP methods (GET, POST, etc.)
                           .AllowAnyHeader();  // Allows any headers
                });
            });

            // Add services to the container.            
            builder.Services.AddDbContext<NotificationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<NotificationService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            //app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
