using System.Net;

using FluentAssertions;

using MyProjectTemplate.Core.Exceptions;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Exceptions;

public class ErrorExceptionTests
{
    [Fact]
    public void DefaultState_ShouldHaveInternalServerErrorCode_And_NullMessage()
    {
        // Act
        var ex = new ErrorException();

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.InternalServerError);
        ex.Message.Should().BeNull();
    }

    [Fact]
    public void Code_ShouldBeSettable()
    {
        // Arrange
        var ex = new ErrorException();

        // Act
        ex.Code = 550;

        // Assert
        ex.Code.Should().Be(550);
    }
}
