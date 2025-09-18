using MediatR;

using Microsoft.Extensions.Logging;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Interfaces;

namespace MyProjectTemplate.Application.Example.Queries.GetAllExample;

public class GetAllExampleQueryHandler : IRequestHandler<GetAllExampleQuery, BaseResult<IEnumerable<ExampleDto>>>
{
    private readonly IExampleRepository _exampleRepository;
    private readonly ILogger<GetAllExampleQueryHandler> _logger;

    public GetAllExampleQueryHandler(IExampleRepository exampleRepository, ILogger<GetAllExampleQueryHandler> logger)
    {
        _exampleRepository = exampleRepository;
        _logger = logger;
    }

    public async Task<BaseResult<IEnumerable<ExampleDto>>> Handle(GetAllExampleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Handling GetAllExampleQuery");

            var entities = await _exampleRepository.GetAllAsync();

            var dtos = entities.Select(s => new ExampleDto().ToDTO(s)).ToList();

            _logger.LogInformation("GetAllExampleQuery handled successfully: {Count} items", dtos.Count);

            return new BaseResult<IEnumerable<ExampleDto>>(dtos);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllExampleQuery handling was canceled.");
            return new BaseResult<IEnumerable<ExampleDto>>(Enumerable.Empty<ExampleDto>(), false, "Operation canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling GetAllExampleQuery");
            throw;
        }
    }
}
