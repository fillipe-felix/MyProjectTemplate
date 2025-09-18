using Dodo.Primitives;

using MediatR;

using Microsoft.Extensions.Logging;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Interfaces;

namespace MyProjectTemplate.Application.Example.Queries.GetByIdExample;

public class GetByIdExampleQueryHandler : IRequestHandler<GetByIdExampleQuery, BaseResult<ExampleDto>>
{
    private readonly IExampleRepository _exampleRepository;
    private readonly ILogger<GetByIdExampleQueryHandler> _logger;

    public GetByIdExampleQueryHandler(IExampleRepository exampleRepository, ILogger<GetByIdExampleQueryHandler> logger)
    {
        _exampleRepository = exampleRepository;
        _logger = logger;
    }

    public async Task<BaseResult<ExampleDto>> Handle(GetByIdExampleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Handling GetByIdExampleQuery for Id: {Id}", request.Id);

            var id = Uuid.Parse(request.Id);
            var entity = await _exampleRepository.GetByIdAsync(id);

            if (entity is null)
            {
                _logger.LogWarning("Example not found. Id: {Id}", request.Id);
                throw new NotFoundException($"Example not found. Id={request.Id}");
            }

            var dto = new ExampleDto().ToDTO(entity);
            _logger.LogInformation("GetByIdExampleQuery handled successfully for Id: {Id}", request.Id);

            return new BaseResult<ExampleDto>(dto);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetByIdExampleQuery handling was canceled. Id: {Id}", request.Id);
            return new BaseResult<ExampleDto>(data: null!, success: false, message: "Operation canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling GetByIdExampleQuery for Id: {Id}", request.Id);
            throw;
        }
    }
}
