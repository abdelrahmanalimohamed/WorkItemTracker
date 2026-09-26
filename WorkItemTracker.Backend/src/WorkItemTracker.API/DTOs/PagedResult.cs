namespace WorkItemTracker.API.DTOs
{
	public sealed record PagedResult<T>(
	IReadOnlyList<T> Items,
	int Page,
	int PageSize,
	int TotalCount,
	int TotalPages);
}
