using MediatR;

using MyProjectTemplate.Core.Base;

namespace MyProjectTemplate.Application.Example.Commands.DeleteExample;

public record DeleteExampleCommand(string Id) : IRequest<BaseResult>;
