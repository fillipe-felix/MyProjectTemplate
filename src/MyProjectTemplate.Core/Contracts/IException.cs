namespace MyProjectTemplate.Core.Contracts;

public interface IException
{
    public int Code { get; set; }
    public string Message { get; }
}