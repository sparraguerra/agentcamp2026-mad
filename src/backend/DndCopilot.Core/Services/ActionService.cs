using DndCopilot.Core.Interfaces;

namespace DndCopilot.Core.Services;

/// <summary>
/// Service for managing game actions and handling action validation and execution.
/// Uses ActionProfileRegistry to load and manage available actions by location.
/// </summary>
public class ActionService : IActionService
{
    private readonly IActionRegistry _actionRegistry;

    public ActionService(IActionRegistry actionRegistry)
    {
        _actionRegistry = actionRegistry;
    }

    public bool IsValidAction(string location, string actionKeyword)
    {
        var normalizedKeyword = actionKeyword.ToLower().Trim();

        // Check if action exists in location
        var action = _actionRegistry.FindActionByKeyword(location, normalizedKeyword);
        return action != null;
    }

    public List<string> GetAvailableActions(string location)
    {
        var actions = new List<string>();

        // Add location-specific actions (root level only)
        var locationActions = _actionRegistry.GetActionsByLocation(location);
        if (locationActions.TryGetValue(location, out var locationList))
        {
            actions.AddRange(locationList.Select(a => string.IsNullOrWhiteSpace(a.DisplayName) ? a.Name : a.DisplayName));
        }

        // Add global actions (root level only)
        var globalActions = _actionRegistry.GetGlobalActions();
        actions.AddRange(globalActions.Select(a => string.IsNullOrWhiteSpace(a.DisplayName) ? a.Name : a.DisplayName));

        // Remove duplicates and sort
        return actions.Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(a => a)
                     .ToList();
    }

    public List<string> GetSubActions(string location, string parentActionName)
    {
        var subActions = new List<string>();

        // Find the parent action
        var locationActions = _actionRegistry.GetActionsByLocation(location);
        if (locationActions.TryGetValue(location, out var locationList))
        {
            var parentAction = locationList.FirstOrDefault(a =>
                a.DisplayName.Equals(parentActionName, StringComparison.OrdinalIgnoreCase) ||
                a.Name.Equals(parentActionName, StringComparison.OrdinalIgnoreCase));

            if (parentAction?.HasSubActions == true)
            {
                return parentAction.SubActions
                    .Select(s => string.IsNullOrWhiteSpace(s.DisplayName) ? s.Name : s.DisplayName)
                    .ToList();
            }
        }

        return subActions;
    }

    public ActionProfile? GetActionForInput(string location, string userInput)
    {
        var normalizedInput = userInput.ToLower().Trim();
        
        // Try exact match first
        var action = _actionRegistry.FindActionByKeyword(location, normalizedInput);
        if (action != null)
            return action;

        // Try partial match by checking if input contains keyword
        var locationActions = _actionRegistry.GetActionsByLocation(location);
        if (locationActions.TryGetValue(location, out var locationList))
        {
            var partialMatch = locationList.FirstOrDefault(a =>
                a.Keywords.Any(k => normalizedInput.Contains(k) || k.Contains(normalizedInput)));
            if (partialMatch != null)
                return partialMatch;
        }

        // Check global actions
        var globalActions = _actionRegistry.GetGlobalActions();
        var globalMatch = globalActions.FirstOrDefault(a =>
            a.Keywords.Any(k => normalizedInput.Contains(k) || k.Contains(normalizedInput)));
        
        return globalMatch;
    }

    public bool HasSubActions(string location, string actionName)
    {
        var normalizedName = actionName.ToLower().Trim();
        var action = GetActionForInput(location, normalizedName);
        return action?.HasSubActions ?? false;
    }

    public string GetActionsDescription(string location)
    {
        var actions = GetAvailableActions(location);
        
        if (actions.Count == 0)
            return "No actions are available in this location.";

        return $"Available actions: {string.Join(", ", actions)}";
    }
}
