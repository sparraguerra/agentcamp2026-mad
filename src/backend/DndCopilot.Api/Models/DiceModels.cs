namespace DndCopilot.Api.Models;

public class DiceRollRequest
{
    public string Notation { get; set; } = string.Empty;
}

public class DiceRollResponse
{
    public List<int> Rolls { get; set; } = new();
    public int Modifier { get; set; }
    public int Total { get; set; }
    public string Description { get; set; } = string.Empty;
}
