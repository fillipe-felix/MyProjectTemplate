using MediatR;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Core.Base;

namespace MyProjectTemplate.Application.Example.Queries.GetByIdExample;

public record GetByIdExampleQuery(string Id) : IRequest<BaseResult<ExampleDto>>;
