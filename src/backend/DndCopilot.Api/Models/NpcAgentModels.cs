namespace DndCopilot.Api.Models;

/// <summary>
/// Request model for NPC observation endpoint.
/// </summary>
public class NpcAgentObserveRequest
{
    public int NpcId { get; set; }
    public string NpcName { get; set; } = string.Empty;
    public string CurrentLocation { get; set; } = string.Empty;
    public List<string> NearbyCharacters { get; set; } = new();
    public List<string> NearbyObjects { get; set; } = new();
    public Dictionary<string, object>? GameState { get; set; }
}

/// <summary>
/// Response model for NPC observation endpoint.
/// </summary>
public class NpcAgentObserveResponse
{
    public bool Success { get; set; }
    public string NpcId { get; set; } = string.Empty;
    public List<string> PerceivedThreats { get; set; } = new();
    public List<string> PerceivedOpportunities { get; set; } = new();
    public List<string> PerceivedNeutral { get; set; } = new();
    public string EnvironmentSummary { get; set; } = string.Empty;
}

/// <summary>
/// Request model for NPC decision endpoint.
/// </summary>
public class NpcAgentDecideRequest
{
    public int NpcId { get; set; }
    public string NpcName { get; set; } = string.Empty;
    public string NpcPersonality { get; set; } = string.Empty;
    public string NpcGoal { get; set; } = string.Empty;
    public NpcAgentObserveResponse? Observation { get; set; }
    public List<string> AvailableActions { get; set; } = new();
}

/// <summary>
/// Response model for NPC decision endpoint.
/// </summary>
public class NpcAgentDecideResponse
{
    public bool Success { get; set; }
    public string ChosenAction { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

/// <summary>
/// Request model for NPC action endpoint.
/// </summary>
public class NpcAgentActRequest
{
    public int NpcId { get; set; }
    public string NpcName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public Dictionary<string, object>? ActionParameters { get; set; }
    public Dictionary<string, object>? GameState { get; set; }
}

/// <summary>
/// Response model for NPC action endpoint.
/// </summary>
public class NpcAgentActResponse
{
    public bool Success { get; set; }
    public string ActionPerformed { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public string DialogueLine { get; set; } = string.Empty;
    public List<NpcAgentGameEvent> GeneratedEvents { get; set; } = new();
}

/// <summary>
/// Event model for API responses.
/// </summary>
public class NpcAgentGameEvent
{
    public string EventType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
