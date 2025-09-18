using System.Net;

using FluentAssertions;

using MyProjectTemplate.Core.Exceptions;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Exceptions;

public class BadRequestExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultCode_And_NullMessage()
    {
        // Act
        var ex = new BadRequestException();

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.BadRequest);
        ex.Message.Should().BeNull();
    }

    [Fact]
    public void MessageConstructor_ShouldSetMessage_And_DefaultCode()
    {
        // Act
        var ex = new BadRequestException("invalid payload");

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.BadRequest);
        ex.Message.Should().Be("invalid payload");
    }

    [Fact]
    public void Code_ShouldBeSettable()
    {
        // Arrange
        var ex = new BadRequestException();

        // Act
        ex.Code = 499;

        // Assert
        ex.Code.Should().Be(499);
    }
}
