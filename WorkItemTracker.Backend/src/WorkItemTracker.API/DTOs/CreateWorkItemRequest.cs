using System.ComponentModel.DataAnnotations;

namespace WorkItemTracker.API.DTOs
{
	public sealed class CreateWorkItemRequest
	{
		[Required]
		[MaxLength(120)]
		public required string Title { get; set; }
		public string? Description { get; set; }
	}
}