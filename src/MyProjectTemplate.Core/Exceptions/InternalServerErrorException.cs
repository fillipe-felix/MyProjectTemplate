using System.Net;

using MyProjectTemplate.Core.Contracts;

namespace MyProjectTemplate.Core.Exceptions;

public class InternalServerErrorException : Exception, IException
{
    public int Code { get; set; } = (int)HttpStatusCode.InternalServerError;
    public override string Message { get; }
}
