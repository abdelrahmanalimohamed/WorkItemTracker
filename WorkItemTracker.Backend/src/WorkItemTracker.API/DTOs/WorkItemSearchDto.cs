using WorkItemTracker.API.Enums;

namespace WorkItemTracker.API.DTOs
{
	public record WorkItemSearchDto(
	string? Title = null,
	WorkItemStatus? Status = null,
	int Page = 1,
	int PageSize = 10);
}
