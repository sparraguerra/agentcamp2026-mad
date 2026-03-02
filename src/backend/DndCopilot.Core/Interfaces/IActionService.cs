namespace DndCopilot.Core.Interfaces;

/// <summary>
/// Action profile defining an action available in a location or context.
/// Loaded from docs/ACTIONS.md at runtime.
/// </summary>
public class ActionProfile
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LocationOrContext { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public string Outcome { get; set; } = string.Empty;
    public List<string> Fallback { get; set; } = new();
    public ActionType Type { get; set; } = ActionType.Exploration;
    public List<ActionProfile> SubActions { get; set; } = new();
    
    /// <summary>
    /// Returns true if this action has sub-actions (nested options).
    /// </summary>
    public bool HasSubActions => SubActions.Count > 0;
}

/// <summary>
/// Types of actions in the game.
/// </summary>
public enum ActionType
{
    Exploration,
    Combat,
    Interaction,
    Quest,
    Inventory,
    Travel,
    Social,
    Magic,
    Utility
}

/// <summary>
/// Registry that provides action profiles loaded from external documentation.
/// </summary>
public interface IActionRegistry
{
    /// <summary>
    /// Gets all loaded action profiles keyed by location name.
    /// </summary>
    IReadOnlyDictionary<string, List<ActionProfile>> GetActionsByLocation(string location);

    /// <summary>
    /// Gets all available locations with defined actions.
    /// </summary>
    IReadOnlyList<string> GetAvailableLocations();

    /// <summary>
    /// Finds an action by keyword in a specific location.
    /// Returns null if no matching action is found.
    /// </summary>
    ActionProfile? FindActionByKeyword(string location, string keyword);

    /// <summary>
    /// Gets all global actions available in any location.
    /// </summary>
    IReadOnlyList<ActionProfile> GetGlobalActions();
}

/// <summary>
/// Service for managing game actions and handling action validation and execution.
/// </summary>
public interface IActionService
{
    /// <summary>
    /// Validates if an action is available in a specific location.
    /// </summary>
    bool IsValidAction(string location, string actionKeyword);

    /// <summary>
    /// Gets available actions for a specific location.
    /// </summary>
    List<string> GetAvailableActions(string location);

    /// <summary>
    /// Gets the matching action profile for a user's input.
    /// </summary>
    ActionProfile? GetActionForInput(string location, string userInput);

    /// <summary>
    /// Gets a formatted description of available actions.
    /// </summary>
    string GetActionsDescription(string location);
    
    /// <summary>
    /// Gets sub-actions for a parent action in a specific location.
    /// </summary>
    List<string> GetSubActions(string location, string parentActionName);
    
    /// <summary>
    /// Checks if an action has sub-actions (nested options).
    /// </summary>
    bool HasSubActions(string location, string actionName);
}
