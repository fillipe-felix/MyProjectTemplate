using System.Net;

using MyProjectTemplate.Core.Contracts;

namespace MyProjectTemplate.Core.Exceptions;

public class BadRequestException : Exception, IException
{

    public BadRequestException()
    {
    }

    public BadRequestException(string message)
    {
        Message = message;
    }

    public int Code { get; set; } = (int)HttpStatusCode.BadRequest;
    public override string Message { get; }
}
