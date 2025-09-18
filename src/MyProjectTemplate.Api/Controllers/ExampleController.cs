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

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExampleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExampleController> _logger;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ExampleController"/>.
    /// </summary>
    /// <param name="mediator">O mediator para enviar comandos e queries.</param>
    /// <param name="logger">O logger para registrar informações.</param>
    public ExampleController(IMediator mediator, ILogger<ExampleController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Retorna um item Teste pelo identificador.
    /// </summary>
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
    /// Retorna lista paginada/filtrada de Testes.
    /// </summary>
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
    /// Cria um novo Teste.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResult<CreateExampleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BaseErrorResult),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<CreateExampleResponse>>> CreateAsync([FromBody] CreateExampleCommand command, CancellationToken cancellationToken)
    {
        if (command is null) return BadRequest();

        var response = await _mediator.Send(command, cancellationToken);

        return CreatedAtRoute(nameof(GetByIdAsync), new { id = response.Data.id }, response);
    }

    /// <summary>
    /// Atualiza um Teste existente.
    /// </summary>
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
    /// Remove um Teste pelo identificador.
    /// </summary>
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
