using System.Net;

using MyProjectTemplate.Core.Contracts;

namespace MyProjectTemplate.Core.Exceptions;

public class ErrorException : Exception, IException
{
    public int Code { get; set; } = (int)HttpStatusCode.InternalServerError;
    public override string Message { get; }
}
