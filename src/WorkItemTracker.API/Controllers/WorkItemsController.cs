using Microsoft.AspNetCore.Mvc;
using WorkItemTracker.API.DTOs;
using WorkItemTracker.API.Exceptions;
using WorkItemTracker.API.Services;

namespace WorkItemTracker.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkItemsController(IWorkItemService service) : ControllerBase
	{
		[HttpPost]
		[ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<WorkItemResponse>> Create(
			CreateWorkItemRequest request,
			CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.Title))
				ModelState.AddModelError(nameof(request.Title), "Title is required.");

			if (!ModelState.IsValid)
				return ValidationProblem(ModelState);

			var result = await service.CreateAsync(request, cancellationToken);

			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}
		[HttpGet]
		[ProducesResponseType(typeof(PagedResult<WorkItemResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<PagedResult<WorkItemResponse>>> Get(
			[FromQuery] WorkItemSearchDto search,
			CancellationToken cancellationToken = default)
		{
			if (search.Page < 1)
				ModelState.AddModelError(nameof(search.Page), "Page must be greater than 0.");

			if (search.PageSize is < 1 or > 100)
				ModelState.AddModelError(nameof(search.PageSize), "PageSize must be between 1 and 100.");

			if (!ModelState.IsValid)
				return ValidationProblem(ModelState);

			return Ok(await service.SearchAsync(search, cancellationToken));
		}

		[HttpGet("{id:int:min(1)}")]
		[ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<WorkItemResponse>> GetById(int id, CancellationToken cancellationToken)
		{
			var item = await service.GetByIdAsync(id, cancellationToken);
			return item is null ? NotFound() : Ok(item);
		}

		[HttpPatch("{id:int}/status")]
		[ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<WorkItemResponse>> ChangeStatus(
		  int id,
		  UpdateStatusRequest request,
		  CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
				return ValidationProblem(ModelState);

			try
			{
				var result = await service.ChangeStatusAsync(id, request.Status!.Value, cancellationToken);
				return result is null ? NotFound() : Ok(result);
			}
			catch (InvalidStatusTransitionException ex)
			{
				return Conflict(new ProblemDetails
				{
					Title = "Invalid status transition",
					Detail = ex.Message,
					Status = StatusCodes.Status409Conflict
				});
			}
		}

	}
}