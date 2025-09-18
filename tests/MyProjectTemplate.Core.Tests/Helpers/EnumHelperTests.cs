using System.ComponentModel;

using FluentAssertions;

using MyProjectTemplate.Core.Helpers;

using Xunit;

namespace MyProjectTemplate.Core.Tests.Helpers;

public class EnumHelperTests
{
    private enum SampleEnum
    {
        [Description("First option")]
        First = 1,
        Second = 2
    }

    [Fact]
    public void GetDescription_ShouldReturnDescriptionAttribute_WhenPresent()
    {
        // Act
        var description = SampleEnum.First.GetDescription();

        // Assert
        description.Should().Be("First option");
    }

    [Fact]
    public void GetDescription_ShouldFallbackToName_WhenDescriptionNotPresent()
    {
        // Act
        var description = SampleEnum.Second.GetDescription();

        // Assert
        description.Should().Be(nameof(SampleEnum.Second));
    }

    [Fact]
    public void GetDescription_ShouldThrowArgumentNullException_WhenEnumIsNull()
    {
        // Arrange
        SampleEnum? value = null;

        // Act
        var act = () => EnumHelper.GetDescription((Enum)(object?)value!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("value");
    }
}
