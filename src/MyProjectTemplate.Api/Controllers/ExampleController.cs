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
/// Controlador responsável por gerenciar a entidade Example (CRUD).
/// </summary>
/// <remarks>
/// Endpoints expostos:
/// - GET api/Example/{id}: Obtém um Example pelo identificador.
/// - GET api/Example: Lista Examples (possível expansão futura para filtros/paginação).
/// - POST api/Example: Cria um novo Example.
/// - PUT api/Example/{id}: Atualiza um Example existente.
/// - DELETE api/Example/{id}: Remove um Example pelo identificador.
///
/// Convenções:
/// - Todas as respostas são envelopadas por BaseResult ou BaseErrorResult.
/// - Logs são registrados para eventos relevantes (p. ex., não encontrado, ID inválido).
/// </remarks>
/// <response code="400">Requisição inválida. Verifique o corpo e parâmetros.</response>
/// <response code="404">Recurso não encontrado.</response>
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
    /// Retorna um Example pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do Example (UUID em string).</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Um envelope <see cref="BaseResult{T}"/> contendo <see cref="ExampleDto"/> quando encontrado.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// GET api/Example/8f9e6f1a-2f6c-4f7a-b6f8-2f7c6a9d1e2b
    ///
    /// Possíveis resultados:
    /// - 200 OK: Example encontrado e retornado no corpo.
    /// - 404 NotFound: Example não encontrado para o ID informado.
    /// - 400 BadRequest: ID inválido ou erro de validação.
    /// </remarks>
    /// <response code="200">Retorna o Example solicitado.</response>
    /// <response code="404">Quando o Example não é encontrado.</response>
    /// <response code="400">Quando o ID é inválido ou a requisição é malformada.</response>
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
    /// Retorna uma lista de Examples.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Um envelope <see cref="BaseResult{T}"/> contendo uma coleção de <see cref="ExampleDto"/>.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// GET api/Example
    ///
    /// Possíveis resultados:
    /// - 200 OK: Lista retornada com sucesso (pode estar vazia).
    /// - 404 NotFound: Nenhum registro encontrado (quando aplicável).
    /// - 400 BadRequest: Erro de validação.
    ///
    /// Observações:
    /// - Este endpoint pode ser estendido futuramente para oferecer paginação e filtros.
    /// </remarks>
    /// <response code="200">Retorna a lista de Examples.</response>
    /// <response code="404">Quando não há registros conforme o critério.</response>
    /// <response code="400">Quando há erro de validação da requisição.</response>
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
    /// Cria um novo Example.
    /// </summary>
    /// <param name="command">Dados para criação do Example.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Um envelope <see cref="BaseResult{T}"/> com <see cref="CreateExampleResponse"/> contendo o ID gerado.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST api/Example
    ///     Content-Type: application/json
    ///     {
    ///         "nome": "Meu Example",
    ///         "descricao": "Descrição do Example"
    ///     }
    ///
    /// Possíveis resultados:
    /// - 201 Created: Example criado com sucesso. O cabeçalho Location aponta para GET api/Example/{id}.
    /// - 400 BadRequest: Corpo inválido ou erro de validação.
    /// </remarks>
    /// <response code="201">Criado com sucesso. Retorna o identificador do recurso.</response>
    /// <response code="400">Quando o corpo é nulo ou inválido.</response>
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
    /// Atualiza um Example existente.
    /// </summary>
    /// <param name="id">Identificador do Example (UUID em string).</param>
    /// <param name="command">Dados para atualização do Example.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>204 No Content em caso de sucesso.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     PUT api/Example/{id}
    ///     Content-Type: application/json
    ///     {
    ///         "nome": "Example Atualizado",
    ///         "descricao": "Descrição atualizada"
    ///     }
    ///
    /// Regras:
    /// - O ID do caminho deve ser um UUID válido. Caso contrário, retorna 400 BadRequest.
    /// - O ID do comando é definido internamente a partir do parâmetro de rota.
    ///
    /// Possíveis resultados:
    /// - 204 No Content: Atualizado com sucesso.
    /// - 400 BadRequest: ID inválido ou erro de validação dos dados.
    /// - 404 NotFound: Recurso não existe (quando aplicável no handler).
    /// </remarks>
    /// <response code="204">Atualizado com sucesso.</response>
    /// <response code="400">Quando o ID é inválido ou os dados são inconsistentes.</response>
    /// <response code="404">Quando o recurso não existe.</response>
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
    /// Remove um Example pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do Example (UUID em string).</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>204 No Content em caso de sucesso.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// DELETE api/Example/{id}
    ///
    /// Possíveis resultados:
    /// - 204 No Content: Removido com sucesso.
    /// - 404 NotFound: Recurso não encontrado.
    /// - 400 BadRequest: Quando ocorrer falha de validação ou erro de negócio reportado pelo handler.
    /// </remarks>
    /// <response code="204">Removido com sucesso.</response>
    /// <response code="404">Quando o recurso não é encontrado.</response>
    /// <response code="400">Quando há falha de validação/negócio.</response>
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
