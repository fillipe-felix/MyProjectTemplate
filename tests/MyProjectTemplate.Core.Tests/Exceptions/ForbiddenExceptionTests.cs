using System.Net;

using FluentAssertions;

using MyProjectTemplate.Core.Exceptions;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Exceptions;

public class ForbiddenExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultCode_And_NullMessage()
    {
        // Act
        var ex = new ForbiddenException();

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.Forbidden);
        ex.Message.Should().BeNull();
    }

    [Fact]
    public void MessageConstructor_ShouldSetMessage_And_DefaultCode()
    {
        // Act
        var ex = new ForbiddenException("forbidden");

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.Forbidden);
        ex.Message.Should().Be("forbidden");
    }

    [Fact]
    public void Code_ShouldBeSettable()
    {
        // Arrange
        var ex = new ForbiddenException();

        // Act
        ex.Code = 499;

        // Assert
        ex.Code.Should().Be(499);
    }
}
