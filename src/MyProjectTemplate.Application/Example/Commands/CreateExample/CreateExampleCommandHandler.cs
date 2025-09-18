using MediatR;

using Microsoft.Extensions.Logging;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Interfaces;

namespace MyProjectTemplate.Application.Example.Commands.CreateExample;

public class CreateExampleCommandHandler : IRequestHandler<CreateExampleCommand, BaseResult<CreateExampleResponse>>
{
    private readonly IExampleRepository _exampleRepository;
    private readonly ILogger<CreateExampleCommandHandler> _logger;

    public CreateExampleCommandHandler(IExampleRepository exampleRepository, ILogger<CreateExampleCommandHandler> logger)
    {
        _exampleRepository = exampleRepository;
        _logger = logger;
    }

    public async Task<BaseResult<CreateExampleResponse>> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateExampleCommand for Name={Name}", request?.Name);
        cancellationToken.ThrowIfCancellationRequested();

        var example = await _exampleRepository.AddAsync(request.ToEntity());
        var response = new CreateExampleResponse(example.Id);

        _logger.LogInformation("Example created successfully with Id={Id}", example.Id);
        return new BaseResult<CreateExampleResponse>(response, true, "Created successfully");
    }
}
