using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkItemTracker.API.DTOs;
using WorkItemTracker.API.Enums;

namespace WorkItemTracker.API.Tests
{
	public class ApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
	{
		[Fact]
		public async Task Post_Creates_Item_And_Item_Is_Available_From_Api()
		{
			using var client = factory.CreateClient();

			var response = await client.PostAsJsonAsync("/api/work-items", new
			{
				title = "API persistence test",
				description = "Created through HTTP"
			});

			response.EnsureSuccessStatusCode();

			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			jsonOptions.Converters.Add(new JsonStringEnumConverter());

			var created = await response.Content.ReadFromJsonAsync<WorkItemResponse>(jsonOptions);
			Assert.NotNull(created);

			var getResponse = await client.GetAsync($"/api/work-items/{created!.Id}");
			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
			var loaded = await getResponse.Content.ReadFromJsonAsync<WorkItemResponse>(jsonOptions);
			Assert.Equal("API persistence test", loaded!.Title);
		}

		[Fact]
		public async Task Invalid_Status_Transition_Returns_409()
		{
			using var client = factory.CreateClient();

			var create = await client.PostAsJsonAsync("/api/work-items", new { title = "Conflict test" });

			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			jsonOptions.Converters.Add(new JsonStringEnumConverter());

			var item = await create.Content.ReadFromJsonAsync<WorkItemResponse>(jsonOptions);

			var response = await client.PatchAsJsonAsync($"/api/work-items/{item!.Id}/status", new
			{
				status = WorkItemStatus.Done
			});

			Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
		}
	}
}