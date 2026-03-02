using Moq;
using Xunit;
using DndCopilot.Core.Interfaces;
using DndCopilot.Core.Services;

namespace DndCopilot.Tests;

public class NpcAgentServiceTests
{
    private readonly Mock<IFoundryAiClient> _mockFoundryAiClient;
    private readonly Mock<IDaprEventPublisher> _mockEventPublisher;
    private readonly Mock<IDaprStateClient> _mockStateClient;
    private readonly NpcAgentService _npcAgentService;

    public NpcAgentServiceTests()
    {
        _mockFoundryAiClient = new Mock<IFoundryAiClient>();
        _mockEventPublisher = new Mock<IDaprEventPublisher>();
        _mockStateClient = new Mock<IDaprStateClient>();
        _npcAgentService = new NpcAgentService(_mockFoundryAiClient.Object, _mockEventPublisher.Object, _mockStateClient.Object);
    }

    [Fact]
    public async Task ObserveAsync_WithValidRequest_ReturnsObservationResult()
    {
        // Arrange
        var request = new NpcObserveRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            CurrentLocation = "Village Square",
            NearbyCharacters = new List<string> { "Hero", "Merchant" },
            NearbyObjects = new List<string> { "Fountain", "Bench" }
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = true,
                Content = "The guard surveys the village square, watching the fountain and nearby travelers."
            });

        _mockEventPublisher.Setup(x => x.PublishEventAsync(It.IsAny<string>(), It.IsAny<GameEvent>()))
            .ReturnsAsync(true);

        // Act
        var result = await _npcAgentService.ObserveAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("1", result.NpcId);
        Assert.Contains("Village Square", result.ContextualData["location"]?.ToString());
        Assert.NotEmpty(result.EnvironmentSummary);
    }

    [Fact]
    public async Task ObserveAsync_WithHostileEntities_IdentifiesThreats()
    {
        // Arrange
        var request = new NpcObserveRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            CurrentLocation = "Forest Path",
            NearbyCharacters = new List<string> { "Goblin Scout", "Friendly Traveler" },
            NearbyObjects = new List<string>()
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = true,
                Content = "The guard spots movement in the shadows."
            });

        _mockEventPublisher.Setup(x => x.PublishEventAsync(It.IsAny<string>(), It.IsAny<GameEvent>()))
            .ReturnsAsync(true);

        // Act
        var result = await _npcAgentService.ObserveAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Goblin Scout", result.PerceivedThreats);
    }

    [Fact]
    public async Task DecideAsync_WithThreats_ChoosesDefensiveAction()
    {
        // Arrange
        var observation = new NpcObserveResult
        {
            Success = true,
            NpcId = "1",
            PerceivedThreats = new List<string> { "Goblin" },
            PerceivedOpportunities = new List<string>(),
            EnvironmentSummary = "A dangerous encounter"
        };

        var request = new NpcDecideRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            NpcPersonality = "Brave and protective",
            NpcGoal = "Protect the village",
            Observation = observation,
            AvailableActions = new List<string> { "attack", "defend", "flee", "wait" }
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = true,
                Content = @"ACTION: defend
TARGET: Goblin
REASONING: Threats detected, taking defensive stance to protect the area.
CONFIDENCE: 0.85"
            });

        _mockEventPublisher.Setup(x => x.PublishEventAsync(It.IsAny<string>(), It.IsAny<GameEvent>()))
            .ReturnsAsync(true);

        // Act
        var result = await _npcAgentService.DecideAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("defend", result.ChosenAction);
        Assert.Equal("Goblin", result.TargetEntity);
        Assert.True(result.Confidence > 0);
    }

    [Fact]
    public async Task DecideAsync_WhenAiFails_UsesFallbackDecision()
    {
        // Arrange
        var observation = new NpcObserveResult
        {
            Success = true,
            NpcId = "1",
            PerceivedThreats = new List<string> { "Enemy" },
            PerceivedOpportunities = new List<string>(),
            EnvironmentSummary = "Danger nearby"
        };

        var request = new NpcDecideRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            NpcPersonality = "Cautious",
            NpcGoal = "Stay alive",
            Observation = observation,
            AvailableActions = new List<string> { "flee", "defend", "attack" }
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = false,
                ErrorMessage = "AI service unavailable"
            });

        _mockEventPublisher.Setup(x => x.PublishEventAsync(It.IsAny<string>(), It.IsAny<GameEvent>()))
            .ReturnsAsync(true);

        // Act
        var result = await _npcAgentService.DecideAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.ChosenAction);
        Assert.NotEmpty(result.Reasoning);
    }

    [Fact]
    public async Task ActAsync_WithValidAction_ReturnsActionResult()
    {
        // Arrange
        var request = new NpcActRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            Action = "attack",
            TargetEntity = "Goblin",
            ActionParameters = new Dictionary<string, object>()
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = true,
                Content = @"OUTCOME: The guard swings their sword at the goblin with precision.
DIALOGUE: For the village!"
            });

        _mockEventPublisher.Setup(x => x.PublishEventsAsync(It.IsAny<string>(), It.IsAny<IEnumerable<GameEvent>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _npcAgentService.ActAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("attack", result.ActionPerformed);
        Assert.NotEmpty(result.Outcome);
        Assert.NotEmpty(result.DialogueLine);
        Assert.NotEmpty(result.GeneratedEvents);
    }

    [Fact]
    public async Task ActAsync_WhenAiFails_UsesFallbackResponse()
    {
        // Arrange
        var request = new NpcActRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            Action = "attack",
            TargetEntity = "Goblin",
            ActionParameters = new Dictionary<string, object>()
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = false,
                ErrorMessage = "AI service unavailable"
            });

        _mockEventPublisher.Setup(x => x.PublishEventsAsync(It.IsAny<string>(), It.IsAny<IEnumerable<GameEvent>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _npcAgentService.ActAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Guard", result.Outcome);
        Assert.Contains("attack", result.Outcome);
    }

    [Fact]
    public async Task ObserveAsync_PublishesEvent()
    {
        // Arrange
        var request = new NpcObserveRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            CurrentLocation = "Village Square",
            NearbyCharacters = new List<string>(),
            NearbyObjects = new List<string>()
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse { Success = true, Content = "Test observation" });

        _mockEventPublisher.Setup(x => x.PublishEventAsync(It.IsAny<string>(), It.IsAny<GameEvent>()))
            .ReturnsAsync(true);

        // Act
        await _npcAgentService.ObserveAsync(request);

        // Assert
        _mockEventPublisher.Verify(
            x => x.PublishEventAsync("npc-events", It.Is<GameEvent>(e => e.EventType == "npc.observed")),
            Times.Once);
    }

    [Fact]
    public async Task DecideAsync_PublishesEvent()
    {
        // Arrange
        var observation = new NpcObserveResult
        {
            Success = true,
            NpcId = "1",
            EnvironmentSummary = "Test environment"
        };

        var request = new NpcDecideRequest
        {
            NpcId = 1,
            NpcName = "Guard",
            Observation = observation,
            AvailableActions = new List<string> { "wait" }
        };

        _mockFoundryAiClient.Setup(x => x.GenerateCompletionAsync(It.IsAny<string>(), It.IsAny<FoundryAiParameters>()))
            .ReturnsAsync(new FoundryAiResponse
            {
                Success = true,
                Content = "ACTION: wait\nTARGET: none\nREASONING: Waiting\nCONFIDENCE: 0.5"
            });

        _mockEventPublisher.Setup(x => x.PublishEventAsync(It.IsAny<string>(), It.IsAny<GameEvent>()))
            .ReturnsAsync(true);

        // Act
        await _npcAgentService.DecideAsync(request);

        // Assert
        _mockEventPublisher.Verify(
            x => x.PublishEventAsync("npc-events", It.Is<GameEvent>(e => e.EventType == "npc.decided")),
            Times.Once);
    }
}
