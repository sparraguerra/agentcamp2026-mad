using Xunit;
using DndCopilot.Core.Services;

namespace DndCopilot.Tests;

public class DiceRollerTests
{
    private readonly DiceRoller _diceRoller;

    public DiceRollerTests()
    {
        _diceRoller = new DiceRoller();
    }

    [Fact]
    public void Roll_WithValidNotation_ReturnsValidResult()
    {
        // Arrange
        var notation = "2d6";

        // Act
        var result = _diceRoller.Roll(notation);

        // Assert
        Assert.Equal(2, result.NumberOfDice);
        Assert.Equal(6, result.DiceSides);
        Assert.Equal(2, result.Rolls.Count);
        Assert.All(result.Rolls, roll => Assert.InRange(roll, 1, 6));
        Assert.InRange(result.Total, 2, 12);
    }

    [Fact]
    public void Roll_WithModifier_AppliesModifier()
    {
        // Arrange
        var notation = "1d20+5";

        // Act
        var result = _diceRoller.Roll(notation);

        // Assert
        Assert.Equal(1, result.NumberOfDice);
        Assert.Equal(20, result.DiceSides);
        Assert.Equal(5, result.Modifier);
        Assert.InRange(result.Total, 6, 25); // 1-20 + 5
    }

    [Fact]
    public void Roll_WithNegativeModifier_AppliesNegativeModifier()
    {
        // Arrange
        var notation = "1d6-2";

        // Act
        var result = _diceRoller.Roll(notation);

        // Assert
        Assert.Equal(-2, result.Modifier);
        Assert.InRange(result.Total, -1, 4); // 1-6 - 2
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("2x6")]
    [InlineData("d6")]
    public void Roll_WithInvalidNotation_ThrowsException(string notation)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _diceRoller.Roll(notation));
    }

    [Fact]
    public void Roll_WithNumbers_ReturnsExpectedResult()
    {
        // Arrange
        int numberOfDice = 3;
        int diceSides = 8;
        int modifier = 2;

        // Act
        var result = _diceRoller.Roll(numberOfDice, diceSides, modifier);

        // Assert
        Assert.Equal(3, result.NumberOfDice);
        Assert.Equal(8, result.DiceSides);
        Assert.Equal(2, result.Modifier);
        Assert.Equal(3, result.Rolls.Count);
        Assert.InRange(result.Total, 5, 26); // 3-24 + 2
    }
}
