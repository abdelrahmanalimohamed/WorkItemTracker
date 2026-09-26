using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorkItemTracker.API.Data;
using WorkItemTracker.API.Services;

namespace WorkItemTracker.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
			builder.Services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<AppDbContext>(options =>
	                options.UseSqlite
                    (builder.Configuration.GetConnectionString("DefaultConnection") ??
                    "Data Source=workitems.db"));
			
            builder.Services.AddScoped<IWorkItemService, WorkItemService>();

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				await db.Database.EnsureCreatedAsync();
			}

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors(policy => policy.WithOrigins("http://localhost:4200")
							.AllowAnyHeader()
							.AllowAnyMethod()
							.AllowCredentials());
			app.MapControllers();

            app.Run();
        }
    }
}
