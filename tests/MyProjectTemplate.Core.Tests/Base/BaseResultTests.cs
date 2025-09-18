using MyProjectTemplate.Core.Base;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Base;

public class BaseResultTests
{
    [Fact]
    public void Ctor_Default_ShouldInitializeWithSuccessTrueAndEmptyMessage()
    {
        // Act
        var result = new BaseResult();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Message);
    }

    [Fact]
    public void Ctor_WithParameters_ShouldAssignProperties()
    {
        // Act
        var result = new BaseResult(success: false, message: "error");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("error", result.Message);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var result = new BaseResult();

        // Act
        result.Success = false;
        result.Message = "updated";

        // Assert
        Assert.False(result.Success);
        Assert.Equal("updated", result.Message);
    }
    
    [Fact]
    public void Ctor_Generic_Defaults_ShouldSetDataAndDefaults()
    {
        // Act
        var result = new BaseResult<int>(42);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Message);
        Assert.Equal(42, result.Data);
    }

    [Fact]
    public void Ctor_Generic_WithParameters_ShouldAssignAllProperties()
    {
        // Act
        var data = new { Name = "Test" };
        var result = new BaseResult<object>(data, success: false, message: "fail");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("fail", result.Message);
        Assert.Same(data, result.Data);
    }

    [Fact]
    public void Data_Property_ShouldBeSettable()
    {
        // Arrange
        var result = new BaseResult<string>("A");

        // Act
        result.Data = "B";

        // Assert
        Assert.Equal("B", result.Data);
    }

    [Fact]
    public void Inheritance_ShouldExposeBaseMembers()
    {
        // Arrange
        var result = new BaseResult<string>("data", success: true, message: "ok");

        // Act
        result.Success = false;
        result.Message = "changed";

        // Assert
        Assert.False(result.Success);
        Assert.Equal("changed", result.Message);
        Assert.Equal("data", result.Data);
    }
}
