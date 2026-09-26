using WorkItemTracker.API.Enums;

namespace WorkItemTracker.API.DTOs
{
	public sealed record WorkItemResponse(
	int Id,
	string Title,
	string? Description,
	WorkItemStatus Status,
	DateTime CreatedAt);
}
