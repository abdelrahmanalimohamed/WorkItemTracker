using WorkItemTracker.API.Enums;

namespace WorkItemTracker.API.Exceptions
{
	public sealed class InvalidStatusTransitionException(WorkItemStatus current, WorkItemStatus requested)
	: Exception($"Invalid status transition from {current} to {requested}.")
	{
		public WorkItemStatus Current { get; } = current;
		public WorkItemStatus Requested { get; } = requested;
	}
}