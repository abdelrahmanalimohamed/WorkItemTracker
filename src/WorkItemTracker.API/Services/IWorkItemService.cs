using WorkItemTracker.API.DTOs;
using WorkItemTracker.API.Enums;

namespace WorkItemTracker.API.Services
{
	public interface IWorkItemService
	{
		Task<WorkItemResponse> CreateAsync
			(CreateWorkItemRequest request, 
			CancellationToken cancellationToken);
		Task<PagedResult<WorkItemResponse>> SearchAsync(
			WorkItemSearchDto workItemSearch,
			CancellationToken cancellationToken);
		Task<WorkItemResponse?> GetByIdAsync(
			int id, 
			CancellationToken cancellationToken);
		Task<WorkItemResponse?> ChangeStatusAsync(
			int id, 
			WorkItemStatus status, 
			CancellationToken cancellationToken);
	}
}
