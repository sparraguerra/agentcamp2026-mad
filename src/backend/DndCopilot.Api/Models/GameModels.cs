namespace DndCopilot.Api.Models;

public class GameMessage
{
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class StartGameRequest
{
    public int CharacterId { get; set; }
}

public class StartGameResponse
{
    public int SessionId { get; set; }
    public string WelcomeMessage { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public string CharacterClass { get; set; } = string.Empty;
    public List<string> AvailableActions { get; set; } = new();
    public Dictionary<string, object> GameState { get; set; } = new();
}

public class GameActionRequest
{
    public int SessionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public int? PlayerRollTotal { get; set; }
    public string? PlayerRollNotation { get; set; }
}

public class GameActionResponse
{
    public string Response { get; set; } = string.Empty;
    public List<string> AvailableActions { get; set; } = new();
    public Dictionary<string, object> GameState { get; set; } = new();
    
    /// <summary>
    /// If an action has sub-actions (nested options), they appear here.
    /// The frontend should display these as a sub-menu for the parent action.
    /// </summary>
    public List<string> SubActions { get; set; } = new();
    
    /// <summary>
    /// The parent action name that generated the SubActions.
    /// Used by frontend to track context.
    /// </summary>
    public string? SelectedParentAction { get; set; }
}
