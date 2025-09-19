using MediatR;

using Microsoft.Extensions.Logging;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Interfaces;

namespace MyProjectTemplate.Application.Example.Queries.GetAllExample;

public class GetAllExampleQueryHandler : IRequestHandler<GetAllExampleQuery, PagedResult<ExampleDto>>
{
    private readonly IExampleRepository _exampleRepository;
    private readonly ILogger<GetAllExampleQueryHandler> _logger;

    public GetAllExampleQueryHandler(IExampleRepository exampleRepository, ILogger<GetAllExampleQueryHandler> logger)
    {
        _exampleRepository = exampleRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ExampleDto>> Handle(GetAllExampleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Handling GetAllExampleQuery");

            var (items, totalCount) = await _exampleRepository.GetAllAsync(
                request.Pagination,
                orderBy: q => q.OrderBy(p => p.Id),
                cancellationToken: cancellationToken
            );

            var dtos = items.Select(s => new ExampleDto().ToDTO(s)).ToList();

            _logger.LogInformation("GetAllExampleQuery handled successfully: {Count} items", dtos.Count);

            return new PagedResult<ExampleDto>
            {
                Data = dtos,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling GetAllExampleQuery");
            throw;
        }
    }
}
