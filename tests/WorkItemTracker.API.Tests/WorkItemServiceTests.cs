using Microsoft.EntityFrameworkCore;
using WorkItemTracker.API.Data;
using WorkItemTracker.API.DTOs;
using WorkItemTracker.API.Enums;
using WorkItemTracker.API.Exceptions;
using WorkItemTracker.API.Services;

namespace WorkItemTracker.API.Tests
{
	public class WorkItemServiceTests
	{
		private static (AppDbContext Db, WorkItemService Service) CreateSut()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			var db = new AppDbContext(options);
			return (db, new WorkItemService(db));
		}

		[Fact]
		public async Task Todo_Can_Move_To_InProgress()
		{
			await using var sut = CreateSut().Db;
			var service = new WorkItemService(sut);
			var item = await service.CreateAsync(new CreateWorkItemRequest { Title = "Test" }, default);

			var result = await service.ChangeStatusAsync(item.Id, WorkItemStatus.InProgress, default);

			Assert.Equal(WorkItemStatus.InProgress, result!.Status);
		}

		[Fact]
		public async Task InProgress_Can_Move_To_Done()
		{
			await using var db = CreateSut().Db;
			var service = new WorkItemService(db);
			var item = await service.CreateAsync(new CreateWorkItemRequest { Title = "Test" }, default);
			await service.ChangeStatusAsync(item.Id, WorkItemStatus.InProgress, default);

			var result = await service.ChangeStatusAsync(item.Id, WorkItemStatus.Done, default);

			Assert.Equal(WorkItemStatus.Done, result!.Status);
		}


		[Theory]
		[InlineData(WorkItemStatus.Todo, WorkItemStatus.Done)]
		[InlineData(WorkItemStatus.InProgress, WorkItemStatus.Todo)]
		[InlineData(WorkItemStatus.Done, WorkItemStatus.InProgress)]
		[InlineData(WorkItemStatus.Done, WorkItemStatus.Todo)]
		public async Task Invalid_Transition_Throws_Conflict_Exception(WorkItemStatus current, WorkItemStatus next)
		{
			await using var db = CreateSut().Db;
			var service = new WorkItemService(db);
			var item = await service.CreateAsync(new CreateWorkItemRequest { Title = "Test" }, default);
			if (current != WorkItemStatus.Todo)
				await service.ChangeStatusAsync(item.Id, WorkItemStatus.InProgress, default);
			if (current == WorkItemStatus.Done)
				await service.ChangeStatusAsync(item.Id, WorkItemStatus.Done, default);

			await Assert.ThrowsAsync<InvalidStatusTransitionException>(() =>
				service.ChangeStatusAsync(item.Id, next, default));
		}
		[Fact]
		public async Task Missing_item_returns_null()
		{
			await using var db = CreateSut().Db;
			var service = new WorkItemService(db);

			var result = await service.ChangeStatusAsync(999, WorkItemStatus.InProgress, default);

			Assert.Null(result);
		}

		[Fact]
		public async Task Created_item_is_persisted_and_can_be_loaded_after_new_context()
		{
			var databaseName = Guid.NewGuid().ToString();
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName)
				.Options;

			int id;
			await using (var db = new AppDbContext(options))
			{
				var service = new WorkItemService(db);
				var created = await service.CreateAsync(new CreateWorkItemRequest
				{
					Title = "Persistent item",
					Description = "Stored"
				}, default);
				id = created.Id;
			}

			await using (var db = new AppDbContext(options))
			{
				var service = new WorkItemService(db);
				var searchDTO = new WorkItemSearchDto
				{
					Title = "Persistent"
				};
				var result = await service.SearchAsync(searchDTO, default);
				Assert.Contains(result.Items, x => x.Id == id && x.Title == "Persistent item");
			}
		}
	}
}
