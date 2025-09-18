using System.Net;

using MyProjectTemplate.Core.Contracts;

namespace MyProjectTemplate.Core.Exceptions;

public class ConflictException : Exception, IException
{

    public ConflictException()
    {
    }

    public ConflictException(string message)
    {
        Message = message;
    }

    public int Code { get; set; } = (int)HttpStatusCode.Conflict;
    public override string Message { get; }
}
