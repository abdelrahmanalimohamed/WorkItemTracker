using WorkItemTracker.API.Enums;
using WorkItemTracker.API.Models.Base;

namespace WorkItemTracker.API.Models
{
	public sealed class WorkItem : BaseModel
	{
		public required string Title { get; set; }
		public string? Description { get; set; }
		public WorkItemStatus Status { get; set; } = WorkItemStatus.Todo;

	}
}