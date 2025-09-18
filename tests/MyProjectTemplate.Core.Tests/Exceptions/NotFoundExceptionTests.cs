using System.Net;

using FluentAssertions;

using MyProjectTemplate.Core.Exceptions;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void MessageConstructor_ShouldSetMessage_And_DefaultCode()
    {
        // Act
        var ex = new NotFoundException("entity not found");

        // Assert
        ex.Code.Should().Be((int)HttpStatusCode.NotFound);
        ex.Message.Should().Be("entity not found");
    }

    [Fact]
    public void Code_ShouldBeSettable()
    {
        // Arrange
        var ex = new NotFoundException("msg");

        // Act
        ex.Code = 499;

        // Assert
        ex.Code.Should().Be(499);
    }
}
