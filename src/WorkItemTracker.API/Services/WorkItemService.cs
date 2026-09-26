using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WorkItemTracker.API.Data;
using WorkItemTracker.API.DTOs;
using WorkItemTracker.API.Enums;
using WorkItemTracker.API.Exceptions;
using WorkItemTracker.API.Models;

namespace WorkItemTracker.API.Services
{
	public sealed class WorkItemService : IWorkItemService
	{
		private readonly AppDbContext _appDbContext;
		public WorkItemService(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}
		public async Task<WorkItemResponse> CreateAsync(
			CreateWorkItemRequest request,
			CancellationToken cancellationToken)
		{
			var item = new WorkItem
			{
				Title = request.Title!.Trim(),
				Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
			};

			await _appDbContext.WorkItems.AddAsync(item , cancellationToken);
			await _appDbContext.SaveChangesAsync(cancellationToken);

			return Map(item);
		}

		public async Task<WorkItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
		{
			return await _appDbContext.WorkItems
					   .AsNoTracking()
					   .Where(x => x.Id == id)
					   .Select(
							x => new WorkItemResponse
							(x.Id, 
							x.Title, 
							x.Description, 
							x.Status, 
							x.CreatedAt))
					   .FirstOrDefaultAsync(cancellationToken);
		}
		public async Task<PagedResult<WorkItemResponse>> SearchAsync(
			WorkItemSearchDto workItemSearch,
			CancellationToken cancellationToken)
		{
			var query = _appDbContext.WorkItems
				.AsNoTracking()
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(workItemSearch.Title)) 
			{ 
				var term = workItemSearch.Title.Trim(); 
				query = query.Where(x => EF.Functions.Like(x.Title, $"%{term}%")); 
			}
			if (workItemSearch.Status.HasValue) 
			{
				query = query.Where(x => x.Status == workItemSearch.Status.Value); 
			}
			var totalCount = await query.CountAsync(cancellationToken);

			var items = await query
				.OrderByDescending(x => x.CreatedAt)
				.ThenByDescending(x => x.Id)
				.Skip((workItemSearch.Page - 1) * workItemSearch.PageSize)
				.Take(workItemSearch.PageSize)
				.Select(
						x => new WorkItemResponse
						(x.Id, x.Title, x.Description, x.Status, x.CreatedAt))
				.ToListAsync(cancellationToken); 
			
			var totalPages = (int)Math.Ceiling(totalCount / (double)workItemSearch.PageSize); 
			
			return new PagedResult<WorkItemResponse>(items, workItemSearch.Page, workItemSearch.PageSize, totalCount, totalPages);
		}
		public async Task<WorkItemResponse?> ChangeStatusAsync(
			int id, 
			WorkItemStatus status, 
			CancellationToken cancellationToken)
		{
			var item = await _appDbContext.WorkItems
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			if (item is null)
				return null;

			if (!IsAllowedTransition(item.Status, status))
				throw new InvalidStatusTransitionException(item.Status, status);

			item.Status = status;
			await _appDbContext.SaveChangesAsync(cancellationToken);
			return Map(item);
		}

		private static WorkItemResponse Map(WorkItem item) =>
			new(item.Id, item.Title, item.Description, item.Status, item.CreatedAt);

		private static bool IsAllowedTransition(
			WorkItemStatus current, 
			WorkItemStatus next) => (current, next) 
			switch
			  {
				  (WorkItemStatus.Todo, WorkItemStatus.InProgress) => true,
				  (WorkItemStatus.InProgress, WorkItemStatus.Done) => true,
				  _ => false
			  };

	}
}