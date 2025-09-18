using MediatR;

using Microsoft.Extensions.Logging;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Interfaces;

namespace MyProjectTemplate.Application.Example.Commands.UpdateExample;

public class UpdateExampleCommandHandler : IRequestHandler<UpdateExampleCommand, BaseResult>
{
    private readonly IExampleRepository _exampleRepository;
    private readonly ILogger<UpdateExampleCommandHandler> _logger;

    public UpdateExampleCommandHandler(IExampleRepository exampleRepository, ILogger<UpdateExampleCommandHandler> logger)
    {
        _exampleRepository = exampleRepository;
        _logger = logger;
    }

    public async Task<BaseResult> Handle(UpdateExampleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateExampleCommand for Id={Id}", request?.Id);
        
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var example = await _exampleRepository.GetByIdAsync(request.Id);
            
            example.Update(
                request.Name,
                request.Description,
                request.Date,
                request.Location,
                request.Latitude,
                request.Longitude,
                request.Difficulty
            );
            
            await _exampleRepository.UpdateAsync(example);

            _logger.LogInformation("Successfully updated Example Id={Id}", request.Id);
            return new BaseResult(true, $"Successfully updated Example Id={example.Id}");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Update operation was canceled for Id={Id}", request.Id);
            return new BaseResult(false, "Operation canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating Example Id={Id}", request.Id);
            throw;
        }
    }
}
