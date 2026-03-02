namespace DndCopilot.Core.Services;

public class DiceRoller
{
    private readonly Random _random;

    public DiceRoller()
    {
        _random = new Random();
    }

    /// <summary>
    /// Roll dice using standard notation (e.g., "2d6", "1d20", "3d8+5")
    /// </summary>
    public DiceResult Roll(string notation)
    {
        var parts = notation.ToLower().Split('d');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid dice notation. Use format like '2d6' or '1d20+3'");
        }

        if (!int.TryParse(parts[0], out int numberOfDice) || numberOfDice <= 0)
        {
            throw new ArgumentException("Invalid number of dice");
        }

        var diceAndModifier = parts[1].Split(new[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (!int.TryParse(diceAndModifier[0], out int diceSides) || diceSides <= 0)
        {
            throw new ArgumentException("Invalid dice sides");
        }

        int modifier = 0;
        if (diceAndModifier.Length > 1)
        {
            var sign = parts[1].Contains('-') ? -1 : 1;
            if (int.TryParse(diceAndModifier[1], out int mod))
            {
                modifier = mod * sign;
            }
        }

        return Roll(numberOfDice, diceSides, modifier);
    }

    /// <summary>
    /// Roll a specific number of dice with specific sides
    /// </summary>
    public DiceResult Roll(int numberOfDice, int diceSides, int modifier = 0)
    {
        if (numberOfDice <= 0 || diceSides <= 0)
        {
            throw new ArgumentException("Number of dice and dice sides must be positive");
        }

        var rolls = new List<int>();
        int total = 0;

        for (int i = 0; i < numberOfDice; i++)
        {
            int roll = _random.Next(1, diceSides + 1);
            rolls.Add(roll);
            total += roll;
        }

        total += modifier;

        return new DiceResult
        {
            Rolls = rolls,
            Modifier = modifier,
            Total = total,
            NumberOfDice = numberOfDice,
            DiceSides = diceSides
        };
    }
}

public class DiceResult
{
    public List<int> Rolls { get; set; } = new();
    public int Modifier { get; set; }
    public int Total { get; set; }
    public int NumberOfDice { get; set; }
    public int DiceSides { get; set; }

    public override string ToString()
    {
        var rollsStr = string.Join(", ", Rolls);
        var modifierStr = Modifier != 0 ? $" {(Modifier > 0 ? "+" : "")}{Modifier}" : "";
        return $"Rolled {NumberOfDice}d{DiceSides}{modifierStr}: [{rollsStr}] = {Total}";
    }
}
