using System.Net;

using FluentAssertions;

using MyProjectTemplate.Core.Exceptions;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Exceptions;

public class ConflictExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultCode_And_NullMessage()
    {
        // Act
        var ex = new ConflictException();

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.Conflict);
        ex.Message.Should().BeNull();
    }

    [Fact]
    public void MessageConstructor_ShouldSetMessage_And_DefaultCode()
    {
        // Act
        var ex = new ConflictException("resource conflict");

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.Conflict);
        ex.Message.Should().Be("resource conflict");
    }

    [Fact]
    public void Code_ShouldBeSettable()
    {
        // Arrange
        var ex = new ConflictException();

        // Act
        ex.Code = 499;

        // Assert
        ex.Code.Should().Be(499);
    }
}
