using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkItemTracker.API.Data;

namespace WorkItemTracker.API.Tests
{
	public class ApiFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");
			builder.ConfigureServices(services =>
			{
				var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
				if (descriptor is not null)
					services.Remove(descriptor);

				services.AddDbContext<AppDbContext>(options =>
					options.UseInMemoryDatabase("api-tests"));
			});
		}
	}
}
