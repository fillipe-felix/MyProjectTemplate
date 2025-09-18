using System.Net;

using FluentAssertions;

using MyProjectTemplate.Core.Exceptions;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Exceptions;

public class InternalServerErrorExceptionTests
{
    [Fact]
    public void MessageConstructor_ShouldSetMessage_And_DefaultCode()
    {
        // Act
        var ex = new InternalServerErrorException("unexpected error");

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.InternalServerError);
        ex.Message.Should().Be("unexpected error");
    }

    [Fact]
    public void Code_ShouldBeSettable()
    {
        // Arrange
        var ex = new InternalServerErrorException("msg");

        // Act
        ex.Code = 550;

        // Assert
        ex.Code.Should().Be(550);
    }
}
