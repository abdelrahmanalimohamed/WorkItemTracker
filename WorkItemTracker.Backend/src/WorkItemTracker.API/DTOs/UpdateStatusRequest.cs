using System.ComponentModel.DataAnnotations;
using WorkItemTracker.API.Enums;

namespace WorkItemTracker.API.DTOs
{
	public sealed class UpdateStatusRequest
	{
		[Required]
		public WorkItemStatus? Status { get; set; }
	}
}