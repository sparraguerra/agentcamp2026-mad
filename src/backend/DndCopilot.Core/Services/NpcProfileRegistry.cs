using System.Text.RegularExpressions;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Core.Services;

/// <summary>
/// Loads NPC agent profiles from the docs/NPCS.md markdown file.
/// Expected format per NPC:
///   ## NPC: Name
///   - **Role**: ...
///   - **Location**: ...
///   - **Goal**: ...
///   ### Instructions
///   - instruction line
///   ### Fallback
///   > fallback text
/// </summary>
public class NpcProfileRegistry : INpcProfileRegistry
{
    private readonly Dictionary<string, NpcAgentProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _locationToNpc = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, NpcAgentProfile> Profiles => _profiles;

    public NpcProfileRegistry(string markdownContent)
    {
        Parse(markdownContent);
    }

    public string? GetNpcNameByLocation(string location)
    {
        return _locationToNpc.TryGetValue(location, out var name) ? name : null;
    }

    private void Parse(string content)
    {
        // Split by NPC header: ## NPC: Name
        var npcSections = Regex.Split(content, @"(?=^## NPC: )", RegexOptions.Multiline);

        foreach (var section in npcSections)
        {
            if (!section.StartsWith("## NPC:"))
                continue;

            var profile = ParseNpcSection(section);
            if (!string.IsNullOrEmpty(profile.Name))
            {
                _profiles[profile.Name] = profile;
                if (!string.IsNullOrEmpty(profile.Location))
                {
                    // Multiple NPCs can share a location — first one registered per location
                    // is the "primary" for location-based lookup.
                    _locationToNpc.TryAdd(profile.Location, profile.Name);
                }
            }
        }
    }

    private static NpcAgentProfile ParseNpcSection(string section)
    {
        var lines = section.Split('\n');

        var profile = new NpcAgentProfile();

        // First line: ## NPC: Name
        var headerMatch = Regex.Match(lines[0], @"^## NPC:\s*(.+)$");
        if (headerMatch.Success)
            profile.Name = headerMatch.Groups[1].Value.Trim();

        var currentBlock = "fields"; // fields | instructions | fallback

        foreach (var rawLine in lines.Skip(1))
        {
            var line = rawLine.TrimEnd();

            // Detect section changes
            if (Regex.IsMatch(line, @"^### Instructions\s*$", RegexOptions.IgnoreCase))
            {
                currentBlock = "instructions";
                continue;
            }
            if (Regex.IsMatch(line, @"^### Fallback\s*$", RegexOptions.IgnoreCase))
            {
                currentBlock = "fallback";
                continue;
            }
            // Another H2/H3 header means end of this NPC
            if (Regex.IsMatch(line, @"^##[^#]"))
                break;

            switch (currentBlock)
            {
                case "fields":
                    ParseField(line, profile);
                    break;
                case "instructions":
                    if (line.StartsWith("- "))
                        profile.Instructions.Add(line[2..].Trim());
                    break;
            }
        }

        return profile;
    }

    private static void ParseField(string line, NpcAgentProfile profile)
    {
        // Format: - **Key**: Value
        var match = Regex.Match(line, @"^- \*\*(\w+)\*\*:\s*(.+)$");
        if (!match.Success)
            return;

        var key = match.Groups[1].Value.Trim();
        var value = match.Groups[2].Value.Trim();

        switch (key.ToLowerInvariant())
        {
            case "role":
                profile.Role = value;
                break;
            case "location":
                profile.Location = value;
                break;
            case "goal":
                profile.Goal = value;
                break;
            // Race, Personality, Tone, Accent are appended into Role for the LLM prompt
            case "race":
                profile.Role = $"{value} — {profile.Role}";
                break;
            case "personality":
            case "tone":
            case "accent":
                profile.Instructions.Insert(0, $"{key}: {value}");
                break;
        }
    }

    /// <summary>
    /// Factory helper: loads the registry from a file path.
    /// </summary>
    public static NpcProfileRegistry LoadFromFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return new NpcProfileRegistry(content);
    }
}
