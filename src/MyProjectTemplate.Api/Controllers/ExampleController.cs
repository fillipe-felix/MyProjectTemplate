using Dodo.Primitives;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MyProjectTemplate.Application.Example.Commands.CreateExample;
using MyProjectTemplate.Application.Example.Commands.DeleteExample;
using MyProjectTemplate.Application.Example.Commands.UpdateExample;
using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Application.Example.Queries.GetAllExample;
using MyProjectTemplate.Application.Example.Queries.GetByIdExample;
using MyProjectTemplate.Core.Base;

namespace MyProjectTemplate.Api.Controllers;

/// <summary>
/// Controller responsible for managing the Example entity (CRUD).
/// </summary>
/// <remarks>
/// Exposed endpoints:
/// - GET api/Example/{id}: Retrieves an Example by its identifier.
/// - GET api/Example: Lists Examples (future expansion for filters/pagination).
/// - POST api/Example: Creates a new Example.
/// - PUT api/Example/{id}: Updates an existing Example.
/// - DELETE api/Example/{id}: Removes an Example by its identifier.
///
/// Conventions:
/// - All responses are wrapped by BaseResult or BaseErrorResult.
/// - Logs are recorded for relevant events (e.g., not found, invalid ID).
/// </remarks>
/// <response code="400">Invalid request. Check the body and parameters.</response>
/// <response code="404">Resource not found.</response>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExampleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExampleController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator to send commands and queries.</param>
    /// <param name="logger">The logger to record information.</param>
    public ExampleController(IMediator mediator, ILogger<ExampleController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Returns an Example by its identifier.
    /// </summary>
    /// <param name="id">Example identifier (UUID as string).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BaseResult{T}"/> envelope containing <see cref="ExampleDto"/> when found.</returns>
    /// <remarks>
    /// Request example:
    /// GET api/Example/8f9e6f1a-2f6c-4f7a-b6f8-2f7c6a9d1e2b
    ///
    /// Possible results:
    /// - 200 OK: Example found and returned in the body.
    /// - 404 NotFound: Example not found for the provided ID.
    /// - 400 BadRequest: Invalid ID or validation error.
    /// </remarks>
    /// <response code="200">Returns the requested Example.</response>
    /// <response code="404">When the Example is not found.</response>
    /// <response code="400">When the ID is invalid or the request is malformed.</response>
    [HttpGet("{id}", Name = nameof(GetByIdAsync))]
    [ProducesResponseType(typeof(BaseResult<ExampleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResult<ExampleDto>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseErrorResult),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ExampleDto>>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetByIdExampleQuery(id), cancellationToken);
        if (result is null)
        {
            _logger.LogInformation("Example not found. Id: {Id}", id);
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Returns a list of Examples.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BaseResult{T}"/> envelope containing a collection of <see cref="ExampleDto"/>.</returns>
    /// <remarks>
    /// Request example:
    /// GET api/Example
    ///
    /// Possible results:
    /// - 200 OK: List returned successfully (may be empty).
    /// - 404 NotFound: No records found (when applicable).
    /// - 400 BadRequest: Validation error.
    ///
    /// Notes:
    /// - This endpoint may be extended in the future to provide pagination and filters.
    /// </remarks>
    /// <response code="200">Returns the list of Examples.</response>
    /// <response code="404">When no records match the criteria.</response>
    /// <response code="400">When there is a validation error in the request.</response>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResult<IEnumerable<ExampleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResult<IEnumerable<ExampleDto>>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseErrorResult),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<IEnumerable<ExampleDto>>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExampleQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new Example.
    /// </summary>
    /// <param name="command">Data to create the Example.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BaseResult{T}"/> with <see cref="CreateExampleResponse"/> containing the generated ID.</returns>
    /// <remarks>
    /// Request example:
    /// 
    ///     POST api/Example
    ///     Content-Type: application/json
    ///     {
    ///         "name": "My Example",
    ///         "description": "Example description"
    ///     }
    ///
    /// Possible results:
    /// - 201 Created: Example created successfully. Location header points to GET api/Example/{id}.
    /// - 400 BadRequest: Invalid body or validation error.
    /// </remarks>
    /// <response code="201">Successfully created. Returns the resource identifier.</response>
    /// <response code="400">When the body is null or invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResult<CreateExampleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BaseErrorResult),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<CreateExampleResponse>>> CreateAsync([FromBody] CreateExampleCommand command, CancellationToken cancellationToken)
    {
        if (command is null) return BadRequest();

        var response = await _mediator.Send(command, cancellationToken);

        return CreatedAtRoute(nameof(GetByIdAsync), new { id = response.Data.Id }, response);
    }

    /// <summary>
    /// Updates an existing Example.
    /// </summary>
    /// <param name="id">Example identifier (UUID as string).</param>
    /// <param name="command">Data to update the Example.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content on success.</returns>
    /// <remarks>
    /// Request example:
    /// 
    ///     PUT api/Example/{id}
    ///     Content-Type: application/json
    ///     {
    ///         "name": "Updated Example",
    ///         "description": "Updated description"
    ///     }
    ///
    /// Rules:
    /// - The route ID must be a valid UUID. Otherwise, returns 400 BadRequest.
    /// - The command ID is set internally from the route parameter.
    ///
    /// Possible results:
    /// - 204 No Content: Successfully updated.
    /// - 400 BadRequest: Invalid ID or data validation error.
    /// - 404 NotFound: Resource does not exist (when applicable in the handler).
    /// </remarks>
    /// <response code="204">Successfully updated.</response>
    /// <response code="400">When the ID is invalid or the data is inconsistent.</response>
    /// <response code="404">When the resource does not exist.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BaseResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseResult),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResult),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseErrorResult),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateExampleCommand command, CancellationToken cancellationToken)
    {
        if (command is null) return BadRequest();

        if (!Uuid.TryParse(id, out var uuid))
        {
            _logger.LogWarning("The example ID provided for update is invalid: {Id}", id);
            return BadRequest(new BaseResult(false, "The example ID is invalid."));
        }
        
        command.Id = uuid;
        var updated = await _mediator.Send(command, cancellationToken);
        
        if (!updated.Success) return BadRequest(updated);

        return NoContent();
    }

    /// <summary>
    /// Removes an Example by its identifier.
    /// </summary>
    /// <param name="id">Example identifier (UUID as string).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content on success.</returns>
    /// <remarks>
    /// Request example:
    /// DELETE api/Example/{id}
    ///
    /// Possible results:
    /// - 204 No Content: Successfully removed.
    /// - 404 NotFound: Resource not found.
    /// - 400 BadRequest: When a validation or business error occurs as reported by the handler.
    /// </remarks>
    /// <response code="204">Successfully removed.</response>
    /// <response code="404">When the resource is not found.</response>
    /// <response code="400">When there is a validation/business failure.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(BaseResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseResult),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteExampleCommand(id), cancellationToken);
        if (!deleted.Success) return BadRequest(deleted);

        return NoContent();
    }

}