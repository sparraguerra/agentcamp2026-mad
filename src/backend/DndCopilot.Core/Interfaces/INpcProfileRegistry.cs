namespace DndCopilot.Core.Interfaces;

/// <summary>
/// Registry that provides NPC agent profiles loaded from external documentation.
/// </summary>
public interface INpcProfileRegistry
{
    /// <summary>
    /// Gets all loaded NPC profiles keyed by NPC name.
    /// </summary>
    IReadOnlyDictionary<string, NpcAgentProfile> Profiles { get; }

    /// <summary>
    /// Resolves the NPC name present at a given location, if any.
    /// Returns null when no NPC is mapped to that location.
    /// </summary>
    string? GetNpcNameByLocation(string location);
}
