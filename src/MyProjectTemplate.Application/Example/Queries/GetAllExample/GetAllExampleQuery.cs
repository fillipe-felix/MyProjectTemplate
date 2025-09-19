using MediatR;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Core.Base;

namespace MyProjectTemplate.Application.Example.Queries.GetAllExample;

public record GetAllExampleQuery(PaginationParams Pagination) : IRequest<PagedResult<ExampleDto>>;
