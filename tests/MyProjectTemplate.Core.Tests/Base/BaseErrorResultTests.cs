using FluentAssertions;

using MyProjectTemplate.Core.Base;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Base;

public class BaseErrorResultTests
{
    [Fact]
    public void Ctor_Default_ShouldInitializeWithSuccessFalseEmptyMessageAndEmptyErrors()
    {
        // Act
        var result = new BaseErrorResult();

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().BeEmpty();
        result.Errors.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Ctor_WithNullErrors_ShouldInitializeEmptyErrorsList()
    {
        // Act
        var result = new BaseErrorResult(message: "failed", errors: null);

        // Assert
        result.Message.Should().Be("failed");
        result.Errors.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Ctor_WithErrors_ShouldCopyErrorsIntoInternalList()
    {
        // Arrange
        var inputErrors = new[]
        {
            new BaseError { Field = "Name",  Error = "Required" },
            new BaseError { Field = "Email", Error = "Invalid"  }
        };

        // Act
        var result = new BaseErrorResult(success: false, message: "validation failed", errors: inputErrors);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("validation failed");
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().ContainSingle(e => e.Field == "Name"  && e.Error == "Required");
        result.Errors.Should().ContainSingle(e => e.Field == "Email" && e.Error == "Invalid");
    }

    [Fact]
    public void Errors_List_ShouldBeIndependentCopy()
    {
        // Arrange
        var source = new List<BaseError>
        {
            new BaseError { Field = "A", Error = "err" }
        };

        // Act
        var result = new BaseErrorResult(errors: source);
        source.Add(new BaseError { Field = "B", Error = "err2" });

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().NotContain(e => e.Field == "B");
    }

    [Fact]
    public void Properties_FromBase_ShouldBeSettable()
    {
        // Arrange
        var result = new BaseErrorResult();

        // Act
        result.Success = true;
        result.Message = "ok";
        result.Errors.Add(new BaseError { Field = "X", Error = "Y" });

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("ok");
        result.Errors.Should().HaveCount(1);
        var err = result.Errors.First();
        err.Field.Should().Be("X");
        err.Error.Should().Be("Y");
    }
}
