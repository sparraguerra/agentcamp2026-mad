using DndCopilot.Core.Enums;

namespace DndCopilot.Core.Entities;

public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredLevel { get; set; } = 1;
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
    
    public List<QuestStage> Stages { get; set; } = new();
    public List<QuestReward> Rewards { get; set; } = new();
}
