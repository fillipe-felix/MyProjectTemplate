using FluentAssertions;

using MyProjectTemplate.Core.Base;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Base;

public class BaseErrorTests
{
    [Fact]
    public void BaseError_ShouldAllowSetProperties()
    {
        // Arrange
        var error = new BaseError();

        // Act
        error.Field = "Field1";
        error.Error = "Some error";

        // Assert
        error.Field.Should().Be("Field1");
        error.Error.Should().Be("Some error");
    }
}
