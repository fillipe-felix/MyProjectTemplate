using System.Net;

using MyProjectTemplate.Core.Contracts;

namespace MyProjectTemplate.Core.Exceptions;

public class ForbiddenException : Exception, IException
{

    public ForbiddenException()
    {
    }

    public ForbiddenException(string message)
    {
        Message = message;
    }

    public int Code { get; set; } = (int)HttpStatusCode.Forbidden;
    public override string Message { get; }
}
