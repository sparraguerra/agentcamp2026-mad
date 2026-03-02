using System.Text.RegularExpressions;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Core.Services;

/// <summary>
/// Loads action profiles from the docs/ACTIONS.md markdown file.
/// Expected format per location:
///   ## Location: Name
///   - **Description**: ...
///   - **NPCs**: ... (optional)
///   - **Enemies**: ... (optional)
///   - **Actions**:
///     - **keyword**: Description
///     - **keyword**: Description
/// </summary>
public class ActionProfileRegistry : IActionRegistry
{
    private readonly Dictionary<string, List<ActionProfile>> _actionsByLocation = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<ActionProfile> _globalActions = new();
    private readonly List<string> _availableLocations = new();

    public IReadOnlyDictionary<string, List<ActionProfile>> AllActionsByLocation => _actionsByLocation;
    public IReadOnlyList<string> AvailableLocations => _availableLocations.AsReadOnly();

    public ActionProfileRegistry(string markdownContent)
    {
        Parse(markdownContent);
    }

    public IReadOnlyDictionary<string, List<ActionProfile>> GetActionsByLocation(string location)
    {
        if (_actionsByLocation.TryGetValue(location, out var actions))
        {
            return new Dictionary<string, List<ActionProfile>> { { location, actions } };
        }
        return new Dictionary<string, List<ActionProfile>>();
    }

    public IReadOnlyList<string> GetAvailableLocations()
    {
        return _availableLocations.AsReadOnly();
    }

    public ActionProfile? FindActionByKeyword(string location, string keyword)
    {
        var normalizedKeyword = keyword.ToLower().Trim();

        // Check location-specific actions first (including in sub-actions)
        if (_actionsByLocation.TryGetValue(location, out var locationActions))
        {
            var action = locationActions.FirstOrDefault(a => 
                a.Keywords.Any(k => k.Equals(normalizedKeyword, StringComparison.OrdinalIgnoreCase)));
            if (action != null)
                return action;
            
            // Search recursively in sub-actions
            foreach (var rootAction in locationActions)
            {
                var subAction = FindActionRecursively(rootAction, normalizedKeyword);
                if (subAction != null)
                    return subAction;
            }
        }

        // Fallback to global actions
        return _globalActions.FirstOrDefault(a => 
            a.Keywords.Any(k => k.Equals(normalizedKeyword, StringComparison.OrdinalIgnoreCase)));
    }

    private ActionProfile? FindActionRecursively(ActionProfile action, string normalizedKeyword)
    {
        if (action.SubActions == null || action.SubActions.Count == 0)
            return null;

        var subAction = action.SubActions.FirstOrDefault(a =>
            a.Keywords.Any(k => k.Equals(normalizedKeyword, StringComparison.OrdinalIgnoreCase)));
        
        if (subAction != null)
            return subAction;

        // Recurse deeper if needed
        foreach (var sub in action.SubActions)
        {
            var deeperAction = FindActionRecursively(sub, normalizedKeyword);
            if (deeperAction != null)
                return deeperAction;
        }

        return null;
    }

    public IReadOnlyList<ActionProfile> GetGlobalActions()
    {
        return _globalActions.AsReadOnly();
    }

    private void Parse(string content)
    {
        // Split by location header: ## Location: Name
        var locationSections = Regex.Split(content, @"(?=^## Location: )", RegexOptions.Multiline);

        foreach (var section in locationSections)
        {
            if (!section.StartsWith("## Location:"))
                continue;

            ParseLocationSection(section);
        }

        // Parse global actions section
        var globalMatch = Regex.Match(content, @"^## Global Actions\s*$([\s\S]*?)(?=^##|$)", RegexOptions.Multiline);
        if (globalMatch.Success)
        {
            ParseGlobalActionsSection(globalMatch.Groups[1].Value);
        }
    }

    private void ParseLocationSection(string section)
    {
        var lines = section.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        
        if (lines.Length == 0)
            return;

        // Parse header: ## Location: Name
        var headerMatch = Regex.Match(lines[0], @"^## Location:\s*(.+)$");
        if (!headerMatch.Success)
            return;

        var locationName = headerMatch.Groups[1].Value.Trim();
        _availableLocations.Add(locationName);

        var location = new List<ActionProfile>();
        var currentBlock = "metadata"; // metadata | actions
        var description = string.Empty;
        var npcs = string.Empty;
        var enemies = string.Empty;

        var i = 1;
        while (i < lines.Length)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            // Detect section changes
            if (Regex.IsMatch(trimmed, @"^- \*\*Actions\*\*:", RegexOptions.IgnoreCase))
            {
                currentBlock = "actions";
                i++;
                continue;
            }

            // Stop if we hit another location
            if (trimmed.StartsWith("## Location:"))
                break;

            // Stop if we hit another section (like ## Global Actions)
            if (trimmed.StartsWith("##"))
                break;

            switch (currentBlock)
            {
                case "metadata":
                    if (trimmed.StartsWith("- **Description**:"))
                    {
                        description = ExtractValue(trimmed, "Description");
                    }
                    else if (trimmed.StartsWith("- **NPCs**:"))
                    {
                        npcs = ExtractValue(trimmed, "NPCs");
                    }
                    else if (trimmed.StartsWith("- **Enemies**:"))
                    {
                        enemies = ExtractValue(trimmed, "Enemies");
                    }
                    i++;
                    break;

                case "actions":
                    if (trimmed.StartsWith("- **") && trimmed.Contains("**:"))
                    {
                        // Get the indentation level of the current action
                        var rootIndentation = GetIndentation(line);
                        
                        var action = ParseActionLine(trimmed, locationName);
                        if (action != null)
                        {
                            i++;
                            // Check for sub-actions: lines with MORE indentation than the root
                            while (i < lines.Length)
                            {
                                var nextLine = lines[i];
                                var nextTrimmed = nextLine.Trim();
                                var nextIndentation = GetIndentation(nextLine);
                                
                                // Check if next line is indented MORE than current action
                                if (nextIndentation > rootIndentation && nextTrimmed.StartsWith("- **") && nextTrimmed.Contains("**:"))
                                {
                                    var subAction = ParseActionLine(nextTrimmed, locationName);
                                    if (subAction != null)
                                    {
                                        action.SubActions.Add(subAction);
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (nextIndentation <= rootIndentation)
                                {
                                    // Found same or lower indentation, stop looking for sub-actions
                                    break;
                                }
                                else if (!nextTrimmed.StartsWith("- **"))
                                {
                                    // Empty line or non-action line, skip it
                                    i++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            location.Add(action);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                    break;
            }
        }

        if (location.Count > 0)
        {
            _actionsByLocation[locationName] = location;
        }
    }

    private static int GetIndentation(string line)
    {
        var count = 0;
        foreach (var c in line)
        {
            if (c == ' ')
                count++;
            else
                break;
        }
        return count;
    }

    private void ParseGlobalActionsSection(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip empty lines and headers
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                continue;

            if (trimmed.StartsWith("- **") && trimmed.Contains("**:"))
            {
                var action = ParseActionLine(trimmed, "Global");
                if (action != null)
                {
                    action.LocationOrContext = "Global";
                    _globalActions.Add(action);
                }
            }
        }
    }

    private ActionProfile? ParseActionLine(string line, string location)
    {
        // Format: - **keyword**: Description
        var match = Regex.Match(line, @"^- \*\*([^\*]+)\*\*:\s*(.+)$");
        if (!match.Success)
            return null;

        var rawKeyword = match.Groups[1].Value.Trim();
        var keywords = BuildKeywords(rawKeyword);
        var keyword = keywords.FirstOrDefault() ?? rawKeyword.Trim().ToLowerInvariant();
        var description = match.Groups[2].Value.Trim();

        return new ActionProfile
        {
            Name = keyword,
            DisplayName = rawKeyword,
            Keywords = keywords,
            Description = description,
            LocationOrContext = location,
            Outcome = description,
            Type = DetermineActionType(keywords)
        };
    }

    private static List<string> BuildKeywords(string rawKeyword)
    {
        var normalized = rawKeyword.Trim().ToLowerInvariant();
        var keywords = new List<string>();
        var baseKeyword = normalized;

        if (normalized.StartsWith("go to "))
        {
            baseKeyword = normalized[6..].Trim();
        }
        else if (normalized.StartsWith("go "))
        {
            baseKeyword = normalized[3..].Trim();
        }
        else if (normalized.StartsWith("visit "))
        {
            baseKeyword = normalized[6..].Trim();
        }
        else if (normalized.StartsWith("leave "))
        {
            baseKeyword = "leave";
        }
        else if (normalized == "back" || normalized == "go back")
        {
            baseKeyword = "back";
        }
        else if (normalized.StartsWith("look around"))
        {
            baseKeyword = "look";
        }
        else if (normalized.StartsWith("talk to "))
        {
            baseKeyword = "talk";
        }

        if (!string.IsNullOrWhiteSpace(baseKeyword))
            keywords.Add(baseKeyword);
        if (!keywords.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            keywords.Add(normalized);

        return keywords;
    }

    private static string ExtractValue(string line, string key)
    {
        var match = Regex.Match(line, $@"^- \*\*{key}\*\*:\s*(.+)$");
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    private static ActionType DetermineActionType(IEnumerable<string> keywords)
    {
        var normalized = keywords.Select(k => k.ToLowerInvariant()).ToList();

        if (normalized.Any(k => k is "fight" or "attack" or "combat"))
            return ActionType.Combat;

        if (normalized.Any(k => k is "inventory" or "bag" or "items"))
            return ActionType.Inventory;

        if (normalized.Any(k => k.StartsWith("go to ") || k.StartsWith("visit ") || k.StartsWith("go ")))
            return ActionType.Travel;

        if (normalized.Any(k => k is "go" or "travel" or "move" or "navigate" or "back" or "leave" or "ascend" or "store" or "town hall" or "tavern" or "blacksmith" or "dark forest" or "ancient ruins" or "crystal cavern" or "poison swamp" or "frozen mountain" or "abandoned temple" or "fire desert" or "ruined fortress" or "dark dungeon" or "ai tower"))
            return ActionType.Travel;

        if (normalized.Any(k => k is "talk" or "speak" or "chat" or "listen" or "approach" or "investigate"))
            return ActionType.Social;

        if (normalized.Any(k => k is "quest" or "mission" or "quest board" or "quest log"))
            return ActionType.Quest;

        if (normalized.Any(k => k is "cast" or "spell" or "magic" or "hack" or "roll" or "dice"))
            return ActionType.Magic;

        if (normalized.Any(k => k is "rest" or "save" or "help" or "status" or "quit"))
            return ActionType.Utility;

        return ActionType.Exploration;
    }

    /// <summary>
    /// Factory helper: loads the registry from a file path.
    /// </summary>
    public static ActionProfileRegistry LoadFromFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return new ActionProfileRegistry(content);
    }
}
