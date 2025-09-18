using System.Net;

using MyProjectTemplate.Core.Contracts;

namespace MyProjectTemplate.Core.Exceptions;

public class NotFoundException : Exception, IException
{
    public int Code { get; set; } = (int)HttpStatusCode.NotFound;
    public override string Message { get; }
}
