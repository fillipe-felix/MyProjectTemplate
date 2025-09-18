using Dodo.Primitives;

using MediatR;

using Microsoft.Extensions.Logging;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Interfaces;

namespace MyProjectTemplate.Application.Example.Commands.DeleteExample;

public class DeleteExampleCommandHandler : IRequestHandler<DeleteExampleCommand, BaseResult>
{
    private readonly IExampleRepository _exampleRepository;
    private readonly ILogger<DeleteExampleCommandHandler> _logger;

    public DeleteExampleCommandHandler(IExampleRepository exampleRepository, ILogger<DeleteExampleCommandHandler> logger)
    {
        _exampleRepository = exampleRepository;
        _logger = logger;
    }

    public async Task<BaseResult> Handle(DeleteExampleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteExampleCommand for Id={Id}", request?.Id);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Uuid.TryParse(request.Id, out var uuid))
            {
                _logger.LogWarning("Invalid Example Id format: {Id}", request.Id);
                return new BaseResult(false, "The example ID is invalid.");
            }

            var exists = await _exampleRepository.ExistsAsync(uuid);
            if (!exists)
            {
                _logger.LogInformation("Example not found. Id={Id}", request.Id);
                throw new NotFoundException($"Example not found. Id={request.Id}");
            }

            await _exampleRepository.DeleteAsync(uuid);

            _logger.LogInformation("Successfully deleted Example Id={Id}", request.Id);
            return new BaseResult(true, $"Successfully deleted Example Id={request.Id}");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Delete operation was canceled for Id={Id}", request?.Id);
            return new BaseResult(false, "Operation canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting Example Id={Id}", request?.Id);
            throw;
        }
    }
}
