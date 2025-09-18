using MediatR;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Core.Base;

namespace MyProjectTemplate.Application.Example.Queries.GetAllExample;

public class GetAllExampleQuery : IRequest<BaseResult<IEnumerable<ExampleDto>>>;
