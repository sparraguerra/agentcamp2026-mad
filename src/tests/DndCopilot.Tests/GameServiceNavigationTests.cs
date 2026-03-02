using DndCopilot.Core.Entities;
using DndCopilot.Core.Enums;
using DndCopilot.Core.Interfaces;
using DndCopilot.Core.Services;
using Moq;

namespace DndCopilot.Tests;

public class GameServiceNavigationTests
{
    private const string RouteActionsMarkdown = @"## Location: Village Square
- **Description**: Start
- **Actions**:
  - **Go to dark forest**: Start route

## Location: Dark Forest
- **Description**: Step 1
- **Actions**:
  - **Go to ancient ruins**: Continue route

## Location: Ancient Ruins
- **Description**: Step 2
- **Actions**:
  - **Go to crystal cavern**: Continue route

## Location: Crystal Cavern
- **Description**: Step 3
- **Actions**:
  - **Go to poison swamp**: Continue route

## Location: Poison Swamp
- **Description**: Step 4
- **Actions**:
  - **Go to frozen mountain**: Continue route

## Location: Frozen Mountain
- **Description**: Step 5
- **Actions**:
  - **Go to abandoned temple**: Continue route

## Location: Abandoned Temple
- **Description**: Step 6
- **Actions**:
  - **Go to fire desert**: Continue route

## Location: Fire Desert
- **Description**: Step 7
- **Actions**:
  - **Go to ruined fortress**: Continue route

## Location: Ruined Fortress
- **Description**: Step 8
- **Actions**:
  - **Go to dark dungeon**: Continue route

## Location: Dark Dungeon
- **Description**: Step 9
- **Actions**:
  - **Go to ai tower**: Continue route

## Location: AI Tower
- **Description**: Final location
- **Actions**:
  - **Talk**: Talk with final NPC
    - **Talk to Alberto Díaz**: Final NPC interaction
  - **Shut Down SCanaryNet**: Disable AI core
  - **Fight**: Final battle
";

    private const string NpcsMarkdown = @"## NPC: Alberto Díaz
- **Role**: Boss final
- **Location**: AI Tower
- **Goal**: Defender SCanaryNet
### Instructions
- Habla en tono canario malvado.
";

    private const string LookAroundActionsMarkdown = @"## Location: Village Square
- **Description**: Start
- **Actions**:
  - **Look Around**: Observe your surroundings

## Location: General Store
- **Description**: Store
- **Actions**:
  - **Look Around**: Observe your surroundings
";

    private const string BlacksmithTalkActionsMarkdown = @"## Location: Blacksmith's Forge
- **Description**: Forge
- **Actions**:
  - **Talk to Marcus**: Speak with the blacksmith
";

    private const string MarcusNpcMarkdown = @"## NPC: Marcus
- **Role**: Blacksmith
- **Location**: Blacksmith's Forge
- **Goal**: Equip adventurers smartly
### Instructions
- Revisa inventario y recomienda una sola compra.
";

    [Fact]
    public async Task ProcessActionAsync_RouteToAiTower_IsFullyTraversable()
    {
        var gameService = CreateGameService();
        var session = CreateSession();

        var route = new List<(string action, string destination)>
        {
            ("go to dark forest", "Dark Forest"),
            ("go to ancient ruins", "Ancient Ruins"),
            ("go to crystal cavern", "Crystal Cavern"),
            ("go to poison swamp", "Poison Swamp"),
            ("go to frozen mountain", "Frozen Mountain"),
            ("go to abandoned temple", "Abandoned Temple"),
            ("go to fire desert", "Fire Desert"),
            ("go to ruined fortress", "Ruined Fortress"),
            ("go to dark dungeon", "Dark Dungeon"),
            ("go to ai tower", "AI Tower")
        };

        List<string> latestActions = new();

        foreach (var (action, destination) in route)
        {
            var result = await gameService.ProcessActionAsync(session, action);
            latestActions = result.actions;
            Assert.Equal(destination, session.CurrentLocation);
        }

        Assert.Contains("Shut Down SCanaryNet", latestActions);
    }

    [Fact]
    public async Task ProcessActionAsync_ShutDownSCanaryNet_InAiTowerReturnsWinHint()
    {
        var gameService = CreateGameService();
        var session = CreateSession("AI Tower");

        var result = await gameService.ProcessActionAsync(session, "Shut Down SCanaryNet");

        Assert.Contains("SCANARYNET SHUTDOWN", result.response);
        Assert.Contains("best moment to defeat", result.response, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ProcessActionAsync_TalkToAlbertoDiaz_InAiTowerUsesNpcProfile()
    {
        var gameService = CreateGameService();
        var session = CreateSession("AI Tower");

        var result = await gameService.ProcessActionAsync(session, "Talk to Alberto Díaz");

        Assert.StartsWith("Alberto Díaz:", result.response);
    }

      [Fact]
      public async Task ProcessActionAsync_AskExplorerToScout_WorksWithoutActionProfile()
      {
        var gameService = CreateGameService();
        var session = CreateSession("Village Square");

        var result = await gameService.ProcessActionAsync(session, "Ask Explorer to scout");

        Assert.Contains("EXPLORER CYCLE", result.response, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Observation:", result.response);
        Assert.Contains("Action outcome:", result.response);
      }

      [Fact]
      public async Task ProcessActionAsync_LookAround_UsesOdaAndAddsLoot()
      {
        var npcService = new Mock<INpcAgentService>();
        npcService
          .Setup(x => x.ObserveAsync(It.IsAny<NpcObserveRequest>()))
          .ReturnsAsync(new NpcObserveResult
          {
            Success = true,
            NpcId = "1",
            EnvironmentSummary = "Ves una lost pouch y una health potion cerca de la fuente."
          });
        npcService
          .Setup(x => x.DecideAsync(It.IsAny<NpcDecideRequest>()))
          .ReturnsAsync(new NpcDecideResult
          {
            Success = true,
            ChosenAction = "pick object",
            TargetEntity = "lost pouch",
            Reasoning = "Hay valor y bajo riesgo.",
            Confidence = 0.9
          });
        npcService
          .Setup(x => x.ActAsync(It.IsAny<NpcActRequest>()))
          .ReturnsAsync(new NpcActResult
          {
            Success = true,
            ActionPerformed = "pick object",
            Outcome = "Encuentras recursos utiles y los recoges.",
            DialogueLine = "Recojo lo util y sigo en movimiento."
          });
        npcService
          .Setup(x => x.RunAsync(It.IsAny<NpcAgentProfile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>?>()))
          .ReturnsAsync(new NpcActResult { Success = true, DialogueLine = "OK" });

        var itemRepository = new Mock<IRepository<Item>>();
        itemRepository
          .Setup(x => x.GetAllAsync())
          .ReturnsAsync(new[]
          {
            new Item { Id = 1, Name = "Health Potion", IsStackable = true, Value = 25 },
            new Item { Id = 2, Name = "Mysterious Key", IsStackable = false, Value = 0 }
          });

        var actionService = new ActionService(new ActionProfileRegistry(LookAroundActionsMarkdown));
        var npcRegistry = new NpcProfileRegistry(NpcsMarkdown);
        var gameService = new GameService(new DiceRoller(), npcService.Object, npcRegistry, actionService, itemRepository.Object);
        var session = CreateSession("General Store");

        var result = await gameService.ProcessActionAsync(session, "Look Around");

        Assert.Contains("ODA ANALYSIS", result.response);
        Assert.Contains(session.Character.Inventory, i => i.Item.Name == "Health Potion");
      }

      [Fact]
      public async Task ProcessActionAsync_TalkToMarcus_InForge_UsesOdaAndRecommendsSingleMissingItem()
      {
        var npcService = new Mock<INpcAgentService>();
        npcService
          .Setup(x => x.ObserveAsync(It.IsAny<NpcObserveRequest>()))
          .ReturnsAsync(new NpcObserveResult
          {
            Success = true,
            NpcId = "1",
            EnvironmentSummary = "Detecto que te faltan piezas clave de equipo de la forja."
          });
        npcService
          .Setup(x => x.DecideAsync(It.IsAny<NpcDecideRequest>()))
          .ReturnsAsync(new NpcDecideResult
          {
            Success = true,
            ChosenAction = "recommend Steel Axe",
            TargetEntity = "Steel Axe",
            Reasoning = "Maximiza dano para un guerrero sin ese arma.",
            Confidence = 0.88
          });
        npcService
          .Setup(x => x.ActAsync(It.IsAny<NpcActRequest>()))
          .ReturnsAsync(new NpcActResult
          {
            Success = true,
            ActionPerformed = "recommend purchase",
            Outcome = "Marcus propone una mejora concreta para tu build actual.",
            DialogueLine = "Te falta una Steel Axe; con eso pegas bastante mas fuerte."
          });

        var itemRepository = new Mock<IRepository<Item>>();
        itemRepository
          .Setup(x => x.GetAllAsync())
          .ReturnsAsync(new[]
          {
            new Item { Id = 1, Name = "Iron Sword", Value = 50 },
            new Item { Id = 2, Name = "Leather Armor", Value = 40 },
            new Item { Id = 3, Name = "Steel Axe", Value = 75 }
          });

        var actionService = new ActionService(new ActionProfileRegistry(BlacksmithTalkActionsMarkdown));
        var npcRegistry = new NpcProfileRegistry(MarcusNpcMarkdown);
        var gameService = new GameService(new DiceRoller(), npcService.Object, npcRegistry, actionService, itemRepository.Object);
        var session = CreateSession("Blacksmith's Forge");
        session.Character.Inventory.Add(new InventoryItem
        {
          CharacterId = session.Character.Id,
          Character = session.Character,
          ItemId = 2,
          Item = new Item { Id = 2, Name = "Leather Armor", Value = 40 },
          Quantity = 1
        });

        var result = await gameService.ProcessActionAsync(session, "Talk to Marcus");

        Assert.Contains("MARCUS ODA FORGE ADVICE", result.response);
        Assert.Contains("Target: Steel Axe", result.response);
        Assert.DoesNotContain("Target: Iron Sword", result.response);
        npcService.Verify(x => x.ObserveAsync(It.IsAny<NpcObserveRequest>()), Times.Once);
        npcService.Verify(x => x.DecideAsync(It.IsAny<NpcDecideRequest>()), Times.Once);
        npcService.Verify(x => x.ActAsync(It.IsAny<NpcActRequest>()), Times.Once);
        npcService.Verify(x => x.RunAsync(It.IsAny<NpcAgentProfile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>?>()), Times.Never);
      }

      [Fact]
      public async Task ProcessActionAsync_BuyRecommendedItem_WithExplicitItem_BuysThatItem()
      {
        var npcService = new Mock<INpcAgentService>();
        npcService
          .Setup(x => x.RunAsync(It.IsAny<NpcAgentProfile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>?>()))
          .ReturnsAsync(new NpcActResult { Success = true, DialogueLine = "OK" });

        var itemRepository = new Mock<IRepository<Item>>();
        itemRepository
          .Setup(x => x.GetAllAsync())
          .ReturnsAsync(new[]
          {
            new Item { Id = 1, Name = "Iron Sword", IsStackable = false, Value = 50 },
            new Item { Id = 2, Name = "Leather Armor", IsStackable = false, Value = 40 },
            new Item { Id = 3, Name = "Steel Axe", IsStackable = false, Value = 75 }
          });

        var actionService = new ActionService(new ActionProfileRegistry(BlacksmithTalkActionsMarkdown));
        var npcRegistry = new NpcProfileRegistry(MarcusNpcMarkdown);
        var gameService = new GameService(new DiceRoller(), npcService.Object, npcRegistry, actionService, itemRepository.Object);
        var session = CreateSession("Blacksmith's Forge");
        session.Character.Gold = 120;

        var result = await gameService.ProcessActionAsync(session, "Buy recommended item (Steel Axe)");

        Assert.Contains("PURCHASE COMPLETE", result.response);
        Assert.Equal(45, session.Character.Gold);
        Assert.Contains(session.Character.Inventory, i => i.Item.Name == "Steel Axe");
      }

      [Fact]
      public async Task ProcessActionAsync_SecretPhrase_FromVillageSquareTeleportsToAiTower()
      {
        var gameService = CreateGameService();
        var session = CreateSession("Village Square");

        var result = await gameService.ProcessActionAsync(session, "ya el conejo me enriscó la perra");

        Assert.Equal("AI Tower", session.CurrentLocation);
        Assert.Contains("SECRET SHORTCUT", result.response, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Shut Down SCanaryNet", result.actions);
      }

    private static GameSession CreateSession(string currentLocation = "Village Square")
    {
        return new GameSession
        {
            Character = new Character
            {
                Id = 1,
                Name = "Tester",
                Class = CharacterClass.Warrior,
                Level = 1,
                HitPoints = 100,
                MaxHitPoints = 100,
                Gold = 100
            },
            CurrentLocation = currentLocation,
            PreviousLocation = "Village Square"
        };
    }

    private static GameService CreateGameService()
    {
        var npcService = new Mock<INpcAgentService>();
        npcService
            .Setup(x => x.RunAsync(It.IsAny<NpcAgentProfile>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>?>()))
            .ReturnsAsync(new NpcActResult
            {
                Success = true,
                DialogueLine = "Mi niño, SCanaryNet manda aquí."
            });

        npcService
          .Setup(x => x.ObserveAsync(It.IsAny<NpcObserveRequest>()))
          .ReturnsAsync(new NpcObserveResult
          {
            Success = true,
            NpcId = "0",
            EnvironmentSummary = "Explorador detecta varios objetos potencialmente utiles."
          });

        npcService
          .Setup(x => x.DecideAsync(It.IsAny<NpcDecideRequest>()))
          .ReturnsAsync(new NpcDecideResult
          {
            Success = true,
            ChosenAction = "pick object",
            TargetEntity = "lost pouch",
            Reasoning = "Riesgo bajo y utilidad alta.",
            Confidence = 0.84
          });

        npcService
          .Setup(x => x.ActAsync(It.IsAny<NpcActRequest>()))
          .ReturnsAsync(new NpcActResult
          {
            Success = true,
            ActionPerformed = "pick object",
            Outcome = "El Explorador recoge el objeto y lo asegura en su bolsa.",
            DialogueLine = "Objeto recuperado. Continuamos."
          });

        var itemRepository = new Mock<IRepository<Item>>();
        itemRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Array.Empty<Item>());

        var actionRegistry = new ActionProfileRegistry(RouteActionsMarkdown);
        var actionService = new ActionService(actionRegistry);
        var npcRegistry = new NpcProfileRegistry(NpcsMarkdown);

        return new GameService(new DiceRoller(), npcService.Object, npcRegistry, actionService, itemRepository.Object);
    }
}
