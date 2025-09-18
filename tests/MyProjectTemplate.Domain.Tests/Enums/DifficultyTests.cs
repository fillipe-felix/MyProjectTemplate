using FluentAssertions;

using MyProjectTemplate.Domain.Enums;

using Xunit;

namespace MyProjectTemplate.Domain.Tests.Enums;

public class DifficultyTests
{
    [Fact]
    public void Enum_ShouldContainExpectedValues_InOrder()
    {
        // Assert
        ((int)Difficulty.Easy).Should().Be(0);
        ((int)Difficulty.Medium).Should().Be(1);
        ((int)Difficulty.Hard).Should().Be(2);
    }

    [Fact]
    public void Parsing_FromString_ShouldReturnCorrectEnum()
    {
        Enum.Parse<Difficulty>("Easy").Should().Be(Difficulty.Easy);
        Enum.Parse<Difficulty>("Medium").Should().Be(Difficulty.Medium);
        Enum.Parse<Difficulty>("Hard").Should().Be(Difficulty.Hard);
    }

    [Fact]
    public void Values_ShouldBeUnique()
    {
        var values = Enum.GetValues(typeof(Difficulty)).Cast<int>().ToArray();
        values.Should().OnlyHaveUniqueItems();
    }
}
