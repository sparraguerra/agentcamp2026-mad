using System.Text.RegularExpressions;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Enums;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Core.Services;

public class GameService
{
    private readonly DiceRoller _diceRoller;
    private readonly INpcAgentService _npcAgentService;
    private readonly INpcProfileRegistry _npcRegistry;
    private readonly IActionService _actionService;
    private readonly IRepository<Item> _itemRepository;

    public GameService(
        DiceRoller diceRoller,
        INpcAgentService npcAgentService,
        INpcProfileRegistry npcRegistry,
        IActionService actionService,
        IRepository<Item> itemRepository)
    {
        _diceRoller = diceRoller;
        _npcAgentService = npcAgentService;
        _npcRegistry = npcRegistry;
        _actionService = actionService;
        _itemRepository = itemRepository;
    }

    public string GenerateWelcomeMessage(Character character)
    {
        return $@"🎮 GAME START 🎮

Welcome, {character.Name} the {GetClassName(character.Class)}!

You stand at the entrance of the Village Square. The sun is setting, casting long shadows across the cobblestone streets. You can hear the distant sound of a blacksmith's hammer and smell bread baking from the nearby tavern.

Your adventure begins here...

⚔️ HP: {character.HitPoints}/{character.MaxHitPoints}
💰 Gold: {character.Gold}
⭐ Level: {character.Level}

What would you like to do?";
    }

    public List<string> GetAvailableActions(string location)
    {
        return _actionService.GetAvailableActions(location);
    }

    public bool ActionHasSubActions(string location, string actionName)
    {
        return _actionService.HasSubActions(location, actionName);
    }

    public List<string> GetActionSubActions(string location, string actionName)
    {
        return _actionService.GetSubActions(location, actionName);
    }

    public async Task<(string response, List<string> actions)> ProcessActionAsync(
        GameSession session,
        string action,
        int? playerRollTotal = null,
        string? playerRollNotation = null)
    {
        var character = session.Character;
        var location = session.CurrentLocation;
        var normalizedAction = action.ToLower().Trim();
        var rollCheck = BuildRollCheck(normalizedAction, playerRollTotal, playerRollNotation);

        if (IsBackCommand(normalizedAction) || IsLeaveCommand(normalizedAction))
        {
            return ReturnToMappedLocation(session);
        }

        if (IsInventoryCommand(normalizedAction))
        {
            return HandleInventoryCommand(character, normalizedAction, location);
        }

        if (normalizedAction.StartsWith("roll"))
        {
            return RollDice(normalizedAction);
        }

        if (normalizedAction == "continue adventure")
        {
            return GenerateLocationDescription(location);
        }

        if (TryExtractRecommendedPurchaseAction(normalizedAction, out var recommendedItemName))
        {
            if (!string.Equals(location, "Blacksmith's Forge", StringComparison.OrdinalIgnoreCase))
            {
                return ("You can only buy Marcus recommendations at the forge.", GetLocationActions(location));
            }

            var itemToBuy = string.IsNullOrWhiteSpace(recommendedItemName)
                ? await ResolveCurrentMarcusRecommendationAsync(character)
                : recommendedItemName;

            if (string.IsNullOrWhiteSpace(itemToBuy))
            {
                return ("Marcus has no recommendation right now. Talk to Marcus first.", GetLocationActions(location));
            }

            return await BuyItemAsync(character, itemToBuy, location);
        }

        // Allow direct scout command even if ACTIONS.md parsing misses the global action.
        if (IsExplorerScoutAction(string.Empty, normalizedAction))
        {
            return await HandleExplorerScoutActionAsync(session, character);
        }

        if (normalizedAction == "say goodbye")
        {
            session.CurrentConversationNpc = null; // Clear conversation tracking
            return ("You end the conversation.", GetLocationActions(location));
        }

        // Keep conversational context even when these actions are not listed in ACTIONS.md
        if ((normalizedAction == "continue conversation" || normalizedAction == "ask about quests")
            && !string.IsNullOrWhiteSpace(session.CurrentConversationNpc))
        {
            return await GenerateConversationAsync(character, session, normalizedAction);
        }

        // Hidden phrase that unlocks a direct shortcut from Village Square to AI Tower.
        if (IsSecretTowerShortcut(normalizedAction)
            && string.Equals(location, "Village Square", StringComparison.OrdinalIgnoreCase))
        {
            SetLocation(session, "AI Tower");
            var scene = GenerateTravelScene(character, "AI Tower");
            var response = $@"🕵️ SECRET SHORTCUT UNLOCKED

You whisper the forbidden phrase and reality bends around you.
In a blink, you are standing inside the AI Tower.

{scene.Item1}";

            return (response, scene.Item2);
        }

        // First, try to resolve the action using the ActionService
        var actionProfile = _actionService.GetActionForInput(location, normalizedAction);
        
        // Route to specific handlers based on action type and name
        if (actionProfile == null)
        {
            return GenerateGenericResponse(normalizedAction, location);
        }

        if (rollCheck is { IsSuccess: false })
        {
            var failResponse = $@"🎲 CHECK FAILED

Action: {action}
Roll: {rollCheck.Notation} = {rollCheck.RollTotal} vs DC {rollCheck.DifficultyClass}

You fail the attempt. Try again with a better roll.";

            return (failResponse, GetLocationActions(location));
        }

        var keyword = actionProfile.Name.ToLowerInvariant();

        if (IsExplorerScoutAction(keyword, normalizedAction))
        {
            return await HandleExplorerScoutActionAsync(session, character);
        }

        if (IsShopAction(keyword))
        {
            return await HandleShopActionAsync(actionProfile, session, character, normalizedAction);
        }

        if (keyword == "escape")
        {
            return EscapeToPreviousLocation(session);
        }

        var actionResult = actionProfile.Type switch
        {
            ActionType.Exploration => await HandleExplorationActionAsync(actionProfile, character, location, normalizedAction),
            ActionType.Social => await GenerateConversationAsync(character, session, normalizedAction),
            ActionType.Inventory => ShowInventory(character),
            ActionType.Quest => HandleQuestAction(actionProfile, session),
            ActionType.Combat => InitiateCombat(location),
            ActionType.Travel => HandleTravelAction(actionProfile, session, character),
            ActionType.Utility => HandleUtilityAction(actionProfile, session, character),
            ActionType.Magic => HandleMagicAction(actionProfile, character, location),
            _ => await HandleExplorationActionAsync(actionProfile, character, location, normalizedAction)
        };

        if (rollCheck is { IsSuccess: true })
        {
            var successNote = $@"🎲 CHECK SUCCESS
Roll: {rollCheck.Notation} = {rollCheck.RollTotal} vs DC {rollCheck.DifficultyClass}
Your roll helps this action.";

            return ($"{actionResult.Item1}\n\n{successNote}", actionResult.Item2);
        }

        return actionResult;
    }

    private static RollCheckResult? BuildRollCheck(string normalizedAction, int? playerRollTotal, string? playerRollNotation)
    {
        if (!playerRollTotal.HasValue)
        {
            return null;
        }

        var (requiresCheck, dc) = GetDifficultyCheck(normalizedAction);
        if (!requiresCheck)
        {
            return null;
        }

        return new RollCheckResult
        {
            RollTotal = playerRollTotal.Value,
            DifficultyClass = dc,
            Notation = string.IsNullOrWhiteSpace(playerRollNotation) ? "1d20" : playerRollNotation,
            IsSuccess = playerRollTotal.Value >= dc
        };
    }

    private static (bool requiresCheck, int difficultyClass) GetDifficultyCheck(string action)
    {
        if (action.Contains("climb") || action.Contains("disarm") || action.Contains("hack") || action.Contains("solve puzzle"))
        {
            return (true, 14);
        }

        if (action.Contains("search") || action.Contains("investigate") || action.Contains("examine") || action.Contains("decipher") || action.Contains("find"))
        {
            return (true, 12);
        }

        if (action.Contains("wade") || action.Contains("navigate") || action.Contains("pray") || action.Contains("warm up"))
        {
            return (true, 10);
        }

        return (false, 0);
    }

    private sealed class RollCheckResult
    {
        public int RollTotal { get; set; }
        public int DifficultyClass { get; set; }
        public string Notation { get; set; } = "1d20";
        public bool IsSuccess { get; set; }
    }

    private async Task<(string, List<string>)> HandleExplorationActionAsync(ActionProfile? profile, Character character, string location, string action)
    {
        var keyword = profile?.Name.ToLowerInvariant() ?? string.Empty;

        return keyword switch
        {
            "look" or "examine" or "observe" => await HandleLookAroundWithOdaAsync(character, location),
            "browse weapons" => ShowWeapons(location),
            "browse armor" => ShowArmor(location),
            "browse supplies" => ShowSupplies(location),
            "order drink" => OrderDrink(location),
            "listen to bard" => ListenToBard(location, character),
            "approach adventurers" => ApproachAdventurers(location),
            "investigate hooded figure" => InvestigateHoodedFigure(location),
            "check notices" => CheckNotices(location),
            "search" => SearchArea(location),
            "examine artifacts" => ExamineArtifacts(location),
            "search for treasure" => SearchForTreasure(location),
            "decipher inscriptions" => DecipherInscriptions(location),
            "mine crystals" => MineCrystals(location),
            "wade" => WadeSwamp(location),
            "find safe path" => FindSafePath(location),
            "warm up" => WarmUp(location),
            "search for shelter" => SearchForShelter(location),
            "pray" => PrayAtTemple(location),
            "examine altar" => ExamineAltar(location),
            "find treasure" => FindTempleTreasure(location),
            "break curse" => BreakCurse(location),
            "drink water" => DrinkWater(location),
            "find oasis" => FindOasis(location),
            "climb walls" => ClimbWalls(location),
            "search barracks" => SearchBarracks(location),
            "examine armory" => ExamineArmory(location),
            "find command room" => FindCommandRoom(location),
            "disarm trap" => DisarmTrap(location),
            "navigate" => NavigateArea(location),
            "ascend" => AscendTower(location),
            "hack console" => HackConsole(location),
            "examine technology" => ExamineTechnology(location),
            "solve puzzle" => SolvePuzzle(location),
            "shut down scanarynet" => ShutDownSCanaryNet(location),
            "cast spell" => CastSpell(character, location),
            "use antidote" => UseAntidote(character, location),
            _ => GenerateGenericResponse(action, location)
        };
    }

    private async Task<(string, List<string>)> HandleLookAroundWithOdaAsync(Character character, string location)
    {
        var nearbyObjects = GetNearbyObjectsForLocation(location);
        var nearbyCharacters = GetNearbyCharactersForLocation(location);

        var observe = await _npcAgentService.ObserveAsync(new NpcObserveRequest
        {
            NpcId = character.Id,
            NpcName = character.Name,
            CurrentLocation = location,
            NearbyCharacters = nearbyCharacters,
            NearbyObjects = nearbyObjects,
            GameState = new Dictionary<string, object>
            {
                ["playerLevel"] = character.Level,
                ["playerClass"] = character.Class.ToString(),
                ["playerGold"] = character.Gold
            }
        });

        var decide = await _npcAgentService.DecideAsync(new NpcDecideRequest
        {
            NpcId = character.Id,
            NpcName = character.Name,
            NpcPersonality = "Aventurero analitico y orientado a supervivencia",
            NpcGoal = "Analizar el entorno y aprovechar recursos sin asumir riesgos innecesarios",
            Observation = observe,
            AvailableActions = new List<string> { "search object", "pick object", "leave object", "report findings" }
        });

        var chosenAction = string.IsNullOrWhiteSpace(decide.ChosenAction) ? "report findings" : decide.ChosenAction;
        var chosenTarget = string.IsNullOrWhiteSpace(decide.TargetEntity) ? nearbyObjects.FirstOrDefault() ?? location : decide.TargetEntity;

        var act = await _npcAgentService.ActAsync(new NpcActRequest
        {
            NpcId = character.Id,
            NpcName = character.Name,
            Action = chosenAction,
            TargetEntity = chosenTarget,
            ActionParameters = new Dictionary<string, object>
            {
                ["location"] = location,
                ["nearbyObjects"] = nearbyObjects,
                ["nearbyCharacters"] = nearbyCharacters
            },
            GameState = new Dictionary<string, object>
            {
                ["playerLevel"] = character.Level,
                ["playerClass"] = character.Class.ToString(),
                ["playerGold"] = character.Gold
            }
        });

        var shouldCollectLoot = chosenAction.Contains("pick", StringComparison.OrdinalIgnoreCase)
            || chosenAction.Contains("search", StringComparison.OrdinalIgnoreCase)
            || act.Outcome.Contains("find", StringComparison.OrdinalIgnoreCase)
            || act.Outcome.Contains("collect", StringComparison.OrdinalIgnoreCase)
            || act.Outcome.Contains("pick", StringComparison.OrdinalIgnoreCase);

        var foundGold = shouldCollectLoot ? CalculateFoundGold(nearbyObjects, character.Level) : 0;
        if (foundGold > 0)
        {
            character.Gold += foundGold;
        }

        var foundItems = shouldCollectLoot
            ? await CollectFoundItemsAsync(character, nearbyObjects)
            : new List<string>();

        var scene = GenerateLocationDescription(location).Item1;
        var lootSummary = BuildLootSummary(foundGold, foundItems);

        var response = $@"{scene}

🧠 ODA ANALYSIS

Observe:
{observe.EnvironmentSummary}

Decide:
Action: {chosenAction}
Target: {chosenTarget}
Reasoning: {decide.Reasoning}

Act:
{act.Outcome}

{lootSummary}";

        return (response, GetLocationActions(location));
    }

    private static int CalculateFoundGold(List<string> nearbyObjects, int playerLevel)
    {
        var hasValuables = nearbyObjects.Any(o =>
            o.Contains("coin", StringComparison.OrdinalIgnoreCase)
            || o.Contains("pouch", StringComparison.OrdinalIgnoreCase)
            || o.Contains("chest", StringComparison.OrdinalIgnoreCase)
            || o.Contains("cache", StringComparison.OrdinalIgnoreCase)
            || o.Contains("token", StringComparison.OrdinalIgnoreCase));

        if (!hasValuables)
        {
            return 0;
        }

        return Math.Clamp(6 + playerLevel * 2 + nearbyObjects.Count, 8, 40);
    }

    private async Task<List<string>> CollectFoundItemsAsync(Character character, List<string> nearbyObjects)
    {
        var foundItems = new List<string>();
        var allItems = await _itemRepository.GetAllAsync();

        var candidates = new List<string>();
        if (nearbyObjects.Any(o => o.Contains("potion", StringComparison.OrdinalIgnoreCase)))
            candidates.Add("Health Potion");
        if (nearbyObjects.Any(o => o.Contains("key", StringComparison.OrdinalIgnoreCase)))
            candidates.Add("Mysterious Key");
        if (nearbyObjects.Any(o => o.Contains("dagger", StringComparison.OrdinalIgnoreCase)))
            candidates.Add("Dagger");

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase).Take(2))
        {
            var item = allItems.FirstOrDefault(i => i.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                continue;
            }

            AddItemToInventory(character, item);
            foundItems.Add(item.Name);
        }

        return foundItems;
    }

    private static string BuildLootSummary(int foundGold, List<string> foundItems)
    {
        if (foundGold <= 0 && foundItems.Count == 0)
        {
            return "Loot: No encuentras recursos utiles en esta inspeccion.";
        }

        var lines = new List<string> { "Loot:" };
        if (foundGold > 0)
        {
            lines.Add($"- Gold found: {foundGold}");
        }
        foreach (var itemName in foundItems)
        {
            lines.Add($"- Item found: {itemName}");
        }

        return string.Join("\n", lines);
    }

    private (string, List<string>) HandleTravelAction(ActionProfile profile, GameSession session, Character character)
    {
        var keyword = profile.Name.ToLowerInvariant();

        if (keyword == "back" || keyword == "leave")
        {
            return ReturnToMappedLocation(session);
        }

        var destination = ResolveDestination(keyword);
        if (destination == null && !string.IsNullOrWhiteSpace(profile.DisplayName))
        {
            destination = ResolveDestination(profile.DisplayName.ToLowerInvariant());
        }

        if (destination == null)
        {
            return ("You travel onward...", GetLocationActions(session.CurrentLocation));
        }

        SetLocation(session, destination);
        return GenerateTravelScene(character, destination);
    }

    private static string? ResolveDestination(string keyword)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["tavern"] = "The Rusty Dragon Tavern",
            ["the rusty dragon tavern"] = "The Rusty Dragon Tavern",
            ["blacksmith"] = "Blacksmith's Forge",
            ["blacksmith's forge"] = "Blacksmith's Forge",
            ["forge"] = "Blacksmith's Forge",
            ["store"] = "General Store",
            ["general store"] = "General Store",
            ["town hall"] = "Town Hall",
            ["dark forest"] = "Dark Forest",
            ["ancient ruins"] = "Ancient Ruins",
            ["crystal cavern"] = "Crystal Cavern",
            ["poison swamp"] = "Poison Swamp",
            ["frozen mountain"] = "Frozen Mountain",
            ["abandoned temple"] = "Abandoned Temple",
            ["fire desert"] = "Fire Desert",
            ["ruined fortress"] = "Ruined Fortress",
            ["dark dungeon"] = "Dark Dungeon",
            ["ai tower"] = "AI Tower"
        };

        return map.TryGetValue(keyword.Trim(), out var destination) ? destination : null;
    }

    private (string, List<string>) GenerateTravelScene(Character character, string destination)
    {
        return destination switch
        {
            "The Rusty Dragon Tavern" => GenerateTavernScene(character, destination),
            "Blacksmith's Forge" => GenerateBlacksmithScene(character, destination),
            "General Store" => GenerateGeneralStoreScene(destination),
            "Town Hall" => GenerateTownHallScene(destination),
            _ => GenerateLocationDescription(destination)
        };
    }

    private (string, List<string>) HandleQuestAction(ActionProfile profile, GameSession session)
    {
        var keyword = profile.Name.ToLower();
        
        return keyword switch
        {
            "quest board" or "quest" or "mission" => ShowQuests(session),
            "quest details" => ViewQuestDetails(session),
            "track quest" or "track" => TrackQuest(session),
            "stop tracking" or "untrack" => ClearTrackedQuest(session),
            _ => ShowQuests(session)
        };
    }

    private (string, List<string>) HandleUtilityAction(ActionProfile profile, GameSession session, Character character)
    {
        var keyword = profile.Name.ToLower();
        
        return keyword switch
        {
            "rest" or "sleep" => Rest(character),
            "status" => ShowCharacterStatus(character),
            _ => ($"You attempt {profile.Name}...", GetLocationActions(session.CurrentLocation))
        };
    }

    private (string, List<string>) HandleMagicAction(ActionProfile profile, Character character, string location)
    {
        var keyword = profile.Name.ToLower();
        
        return keyword switch
        {
            "roll" or "dice" => RollDice(profile.Name),
            "cast" or "spell" or "magic" => CastSpell(character, location),
            _ => ($"You attempt {profile.Name}...", GetLocationActions(location))
        };
    }

    private (string, List<string>) ShowCharacterStatus(Character character)
    {
        var response = $@"📊 CHARACTER STATUS

Name: {character.Name}
Class: {GetClassName(character.Class)}
Level: {character.Level}
HP: {character.HitPoints}/{character.MaxHitPoints}
Gold: {character.Gold}

Status: Ready for adventure!";

        return (response, GetLocationActions("Village Square"));
    }

    private (string, List<string>) CastSpell(Character character, string location)
    {
        var response = @"✨ You attempt to cast a spell...

*A shimmering magical energy surrounds you*

The spell takes effect!";

        return (response, GetLocationActions(location));
    }

    private static bool IsShopAction(string keyword)
    {
        return keyword.StartsWith("buy ") || keyword.StartsWith("browse ") || keyword == "order drink";
    }

    private async Task<(string, List<string>)> HandleShopActionAsync(ActionProfile profile, GameSession session, Character character, string action)
    {
        var keyword = profile.Name.ToLowerInvariant();

        return keyword switch
        {
            "browse weapons" => ShowWeapons(session.CurrentLocation),
            "browse armor" => ShowArmor(session.CurrentLocation),
            "browse supplies" => ShowSupplies(session.CurrentLocation),
            "buy weapon" => await BuyWeaponAsync(character, session.CurrentLocation),
            "buy armor" => await BuyArmorAsync(character, session.CurrentLocation),
            "buy supplies" => await BuySuppliesAsync(character, session.CurrentLocation),
            "order drink" => OrderDrink(session.CurrentLocation),
            _ => GenerateGenericResponse(action, session.CurrentLocation)
        };
    }

    private Task<(string, List<string>)> BuyWeaponAsync(Character character, string location)
    {
        if (!string.Equals(location, "Blacksmith's Forge", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(("You need to be at the blacksmith to buy weapons.", GetLocationActions(location)));
        }

        return BuyItemAsync(character, "Iron Sword", location);
    }

    private Task<(string, List<string>)> BuyArmorAsync(Character character, string location)
    {
        if (!string.Equals(location, "Blacksmith's Forge", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(("You need to be at the blacksmith to buy armor.", GetLocationActions(location)));
        }

        return BuyItemAsync(character, "Leather Armor", location);
    }

    private Task<(string, List<string>)> BuySuppliesAsync(Character character, string location)
    {
        if (!string.Equals(location, "General Store", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(("You need to be at the general store to buy supplies.", GetLocationActions(location)));
        }

        return BuyItemAsync(character, "Health Potion", location);
    }

    private (string, List<string>) EscapeToPreviousLocation(GameSession session)
    {
        var current = session.CurrentLocation;
        var previous = string.IsNullOrWhiteSpace(session.PreviousLocation)
            ? "Village Square"
            : session.PreviousLocation;

        if (string.Equals(current, previous, StringComparison.OrdinalIgnoreCase))
        {
            return ("You search for a way out but end up back where you started.", GetLocationActions(current));
        }

        session.PreviousLocation = current;
        session.CurrentLocation = previous;

        return ($"You escape from {current} and return to {previous}.", GetLocationActions(previous));
    }

    private (string, List<string>) ReturnToMappedLocation(GameSession session)
    {
        var current = session.CurrentLocation;
        var mapped = GetMappedReturnLocation(current);

        if (mapped == null)
        {
            return ("You are already at the start.", GetLocationActions(current));
        }

        if (string.Equals(current, mapped, StringComparison.OrdinalIgnoreCase))
        {
            return ($"You are already at {mapped}.", GetLocationActions(current));
        }

        session.PreviousLocation = current;
        session.CurrentLocation = mapped;
        return ($"You return to {mapped}.", GetLocationActions(mapped));
    }

    private static string? GetMappedReturnLocation(string currentLocation)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["The Rusty Dragon Tavern"] = "Village Square",
            ["Blacksmith's Forge"] = "Village Square",
            ["General Store"] = "Village Square",
            ["Town Hall"] = "Village Square",
            ["Dark Forest"] = "Village Square",
            ["Ancient Ruins"] = "Village Square",
            ["Crystal Cavern"] = "Village Square",
            ["Poison Swamp"] = "Village Square",
            ["Frozen Mountain"] = "Village Square",
            ["Abandoned Temple"] = "Village Square",
            ["Fire Desert"] = "Village Square",
            ["Ruined Fortress"] = "Village Square",
            ["Dark Dungeon"] = "Village Square",
            ["AI Tower"] = "Village Square"
        };

        return map.TryGetValue(currentLocation, out var mapped) ? mapped : null;
    }

    private void SetLocation(GameSession session, string newLocation)
    {
        if (string.Equals(session.CurrentLocation, newLocation, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        session.PreviousLocation = session.CurrentLocation;
        session.CurrentLocation = newLocation;
    }

    private (string, List<string>) OrderDrink(string location)
    {
        if (!string.Equals(location, "The Rusty Dragon Tavern", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no tavern here to order a drink.", GetLocationActions(location));
        }

        var response = @"🍺 Grundy slides a foaming mug across the bar.

You take a sip and feel your spirits lift.";

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ListenToBard(string location, Character character)
    {
        if (!string.Equals(location, "The Rusty Dragon Tavern", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no bard here to listen to.", GetLocationActions(location));
        }

        var response = $@"🎶 Elara's song fills the tavern.

""A tale of {character.Name} the {GetClassName(character.Class)}, bold and bright...""

The patrons applaud as the melody fades.";

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ApproachAdventurers(string location)
    {
        if (!string.Equals(location, "The Rusty Dragon Tavern", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no adventurers to approach here.", GetLocationActions(location));
        }

        var response = @"A group of adventurers share rumors of danger in the ruins to the east.
They warn you to bring healing supplies.";

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) InvestigateHoodedFigure(string location)
    {
        if (!string.Equals(location, "The Rusty Dragon Tavern", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no hooded figure here.", GetLocationActions(location));
        }

        var response = @"The hooded figure whispers about a shadow moving within the AI Tower.
""Seek the ancient inscriptions,"" they say, then fall silent.";

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) CheckNotices(string location)
    {
        if (!string.Equals(location, "Town Hall", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no official notices here.", GetLocationActions(location));
        }

        var response = @"📜 NOTICES

- Reward posted for goblin scouts near the old ruins.
- Curfew lifted for adventurers on official business.";

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) SearchArea(string location)
    {
        var response = location switch
        {
            "Dark Forest" => "You find fresh tracks and signs of recent goblin movement.",
            "Ancient Ruins" => "You uncover a cracked stone tablet with faded runes.",
            "Crystal Cavern" => "You notice glittering shards tucked between rocks.",
            "Poison Swamp" => "You locate a narrow ridge of dry ground through the mire.",
            "Ruined Fortress" => "You spot a collapsed passage leading deeper inside.",
            "Dark Dungeon" => "You detect a draft hinting at a hidden corridor.",
            _ => "You search carefully but find nothing unusual."
        };

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ExamineArtifacts(string location)
    {
        if (!string.Equals(location, "Ancient Ruins", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no ancient artifacts to examine here.", GetLocationActions(location));
        }

        var response = "The artifacts hum with faint magic, hinting at lost rituals.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) SearchForTreasure(string location)
    {
        if (!string.Equals(location, "Ancient Ruins", StringComparison.OrdinalIgnoreCase))
        {
            return ("You search for treasure but find only dust and stone.", GetLocationActions(location));
        }

        var response = "You uncover a small cache of coins hidden beneath a loose tile.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) DecipherInscriptions(string location)
    {
        if (!string.Equals(location, "Ancient Ruins", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no inscriptions to decipher here.", GetLocationActions(location));
        }

        var response = "The inscriptions warn of guardians that awaken at sunset.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) MineCrystals(string location)
    {
        if (!string.Equals(location, "Crystal Cavern", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no crystals here to mine.", GetLocationActions(location));
        }

        var response = "You chip off a few glittering crystals. They pulse with faint energy.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) WadeSwamp(string location)
    {
        if (!string.Equals(location, "Poison Swamp", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no swamp to wade through here.", GetLocationActions(location));
        }

        var response = "You wade carefully through the murky waters, avoiding the thickest fumes.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) FindSafePath(string location)
    {
        if (!string.Equals(location, "Poison Swamp", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no safe path to find here.", GetLocationActions(location));
        }

        var response = "You find a narrow path of solid ground through the swamp.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) WarmUp(string location)
    {
        if (!string.Equals(location, "Frozen Mountain", StringComparison.OrdinalIgnoreCase))
        {
            return ("You don't need to warm up here.", GetLocationActions(location));
        }

        var response = "You warm your hands by a small fire, staving off the bitter cold.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) SearchForShelter(string location)
    {
        if (!string.Equals(location, "Frozen Mountain", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no shelter to find here.", GetLocationActions(location));
        }

        var response = "You find a shallow cave that offers protection from the wind.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) PrayAtTemple(string location)
    {
        if (!string.Equals(location, "Abandoned Temple", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no altar here to pray at.", GetLocationActions(location));
        }

        var response = "A faint warmth answers your prayer, easing the tension in the air.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ExamineAltar(string location)
    {
        if (!string.Equals(location, "Abandoned Temple", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no altar to examine here.", GetLocationActions(location));
        }

        var response = "You discover a hidden compartment beneath the altar stone.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) FindTempleTreasure(string location)
    {
        if (!string.Equals(location, "Abandoned Temple", StringComparison.OrdinalIgnoreCase))
        {
            return ("You search for treasure but find only dust.", GetLocationActions(location));
        }

        var response = "You uncover a small relic wrapped in faded cloth.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) BreakCurse(string location)
    {
        if (!string.Equals(location, "Abandoned Temple", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no curse to break here.", GetLocationActions(location));
        }

        var response = "A surge of light dispels the lingering curse from the chamber.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) DrinkWater(string location)
    {
        if (!string.Equals(location, "Fire Desert", StringComparison.OrdinalIgnoreCase))
        {
            return ("You are not in a desert. You save your water.", GetLocationActions(location));
        }

        var response = "You sip your water carefully, easing the heat.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) FindOasis(string location)
    {
        if (!string.Equals(location, "Fire Desert", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no oasis to find here.", GetLocationActions(location));
        }

        var response = "In the distance, you glimpse a small oasis shimmering in the heat.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ClimbWalls(string location)
    {
        if (!string.Equals(location, "Ruined Fortress", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no walls to climb here.", GetLocationActions(location));
        }

        var response = "You climb the broken walls and scout the fortress layout.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) SearchBarracks(string location)
    {
        if (!string.Equals(location, "Ruined Fortress", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no barracks to search here.", GetLocationActions(location));
        }

        var response = "You rummage through the barracks and find scattered gear.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ExamineArmory(string location)
    {
        if (!string.Equals(location, "Ruined Fortress", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no armory to examine here.", GetLocationActions(location));
        }

        var response = "The armory is mostly empty, but a few racks remain intact.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) FindCommandRoom(string location)
    {
        if (!string.Equals(location, "Ruined Fortress", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no command room to find here.", GetLocationActions(location));
        }

        var response = "You locate the command room, its maps torn but still readable.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) DisarmTrap(string location)
    {
        if (!string.Equals(location, "Dark Dungeon", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no traps to disarm here.", GetLocationActions(location));
        }

        var response = "You disable a hidden trap with careful precision.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) NavigateArea(string location)
    {
        var response = location switch
        {
            "Crystal Cavern" => "You mark your path through the crystal tunnels to avoid getting lost.",
            "Dark Dungeon" => "You study the maze and choose a path deeper within.",
            _ => "You navigate onward, keeping your bearings."
        };

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) AscendTower(string location)
    {
        if (!string.Equals(location, "AI Tower", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no tower to ascend here.", GetLocationActions(location));
        }

        var response = "You climb to the next level as the hum of arcane tech grows louder.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) HackConsole(string location)
    {
        if (!string.Equals(location, "AI Tower", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no console to hack here.", GetLocationActions(location));
        }

        var response = "You bypass a glowing console and reveal hidden schematics.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ExamineTechnology(string location)
    {
        if (!string.Equals(location, "AI Tower", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no advanced technology to study here.", GetLocationActions(location));
        }

        var response = "You study the arcane circuits, learning how the tower sustains itself.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) SolvePuzzle(string location)
    {
        if (!string.Equals(location, "AI Tower", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no puzzle to solve here.", GetLocationActions(location));
        }

        var response = "You align the runes in the correct order, unlocking a sealed doorway.";
        return (response, GetLocationActions(location));
    }

    private (string, List<string>) ShutDownSCanaryNet(string location)
    {
        if (!string.Equals(location, "AI Tower", StringComparison.OrdinalIgnoreCase))
        {
            return ("SCanaryNet is out of reach from here.", GetLocationActions(location));
        }

        var response = @"⚡ SCANARYNET SHUTDOWN

You pull the emergency arcane breaker and the core goes dark.
The tower falls silent as Alberto Diaz loses his AI advantage.

Tip: with SCanaryNet offline, this is the best moment to defeat him.";

        return (response, GetLocationActions(location));
    }

    private (string, List<string>) UseAntidote(Character character, string location)
    {
        var response = "You check your pack for an antidote, but find none ready to use.";
        return (response, GetLocationActions(location));
    }

    private static bool IsBackCommand(string normalizedAction)
    {
        return normalizedAction == "back" || normalizedAction == "go back" || normalizedAction == "return";
    }

    private static bool IsLeaveCommand(string normalizedAction)
    {
        return normalizedAction == "leave" || normalizedAction.StartsWith("leave ");
    }

    private static bool IsInventoryCommand(string normalizedAction)
    {
        return normalizedAction.StartsWith("use ") || normalizedAction.StartsWith("equip ") || normalizedAction.StartsWith("unequip ");
    }

    private static bool IsSecretTowerShortcut(string normalizedAction)
    {
        return normalizedAction == "ya el conejo me enriscó la perra"
            || normalizedAction == "ya el conejo me enrisco la perra";
    }

    private (string, List<string>) HandleInventoryCommand(Character character, string normalizedAction, string location)
    {
        if (normalizedAction == "use item")
        {
            return ("Specify the item to use, e.g. 'use health potion'.", GetLocationActions(location));
        }

        if (normalizedAction.StartsWith("equip "))
        {
            var itemName = normalizedAction[6..].Trim();
            return EquipItem(character, itemName, location);
        }

        if (normalizedAction.StartsWith("unequip "))
        {
            var itemName = normalizedAction[8..].Trim();
            return UnequipItem(character, itemName, location);
        }

        if (normalizedAction.StartsWith("use "))
        {
            var itemName = normalizedAction[4..].Trim();
            return UseItem(character, itemName, location);
        }

        return ("That action is not available.", GetLocationActions(location));
    }

    private (string, List<string>) UseItem(Character character, string itemName, string location)
    {
        var inventoryItem = FindInventoryItem(character, itemName);
        if (inventoryItem == null)
        {
            return ($"You do not have '{itemName}'.", GetLocationActions(location));
        }

        var item = inventoryItem.Item;
        if (item.HealthRestore.HasValue && inventoryItem.Quantity > 0)
        {
            character.HitPoints = Math.Min(character.MaxHitPoints, character.HitPoints + item.HealthRestore.Value);
            inventoryItem.Quantity -= 1;
            if (inventoryItem.Quantity <= 0)
            {
                character.Inventory.Remove(inventoryItem);
            }

            var response = $@"✅ {item.Name} used.

HP: {character.HitPoints}/{character.MaxHitPoints}";
            return (response, GetLocationActions(location));
        }

        if (item.IsEquippable)
        {
            inventoryItem.IsEquipped = true;
            return ($"You equip {item.Name}.", GetLocationActions(location));
        }

        return ($"You cannot use {item.Name} right now.", GetLocationActions(location));
    }

    private (string, List<string>) EquipItem(Character character, string itemName, string location)
    {
        var inventoryItem = FindInventoryItem(character, itemName);
        if (inventoryItem == null)
        {
            return ($"You do not have '{itemName}'.", GetLocationActions(location));
        }

        if (!inventoryItem.Item.IsEquippable)
        {
            return ($"{inventoryItem.Item.Name} cannot be equipped.", GetLocationActions(location));
        }

        inventoryItem.IsEquipped = true;
        return ($"You equip {inventoryItem.Item.Name}.", GetLocationActions(location));
    }

    private (string, List<string>) UnequipItem(Character character, string itemName, string location)
    {
        var inventoryItem = FindInventoryItem(character, itemName);
        if (inventoryItem == null)
        {
            return ($"You do not have '{itemName}'.", GetLocationActions(location));
        }

        inventoryItem.IsEquipped = false;
        return ($"You unequip {inventoryItem.Item.Name}.", GetLocationActions(location));
    }

    private static InventoryItem? FindInventoryItem(Character character, string itemName)
    {
        return character.Inventory.FirstOrDefault(i =>
            i.Item.Name.Contains(itemName, StringComparison.OrdinalIgnoreCase));
    }

    private (string, List<string>) GenerateLocationDescription(string location)
    {
        var descriptions = new Dictionary<string, string>
        {
            ["Village Square"] = @"🏛️ VILLAGE SQUARE

You see:
- The Rusty Dragon Tavern to the north (sounds of laughter inside)
- A blacksmith's forge to the east (smoke rising from the chimney)
- A general store to the west (merchant calling out deals)
- The town hall to the south (guards standing at attention)
- A fountain in the center with clear, cool water",

            ["The Rusty Dragon Tavern"] = @"🍺 THE RUSTY DRAGON TAVERN

The tavern is warm and inviting. Patrons sit at wooden tables, enjoying ale and conversation. The bartender, a stout dwarf named Grundy, polishes glasses behind the bar. You notice a quest board on the wall.",

            ["Blacksmith's Forge"] = @"⚒️ BLACKSMITH'S FORGE

The heat hits you as you enter. Weapons and armor line the walls, glinting in the firelight. The blacksmith, a muscular human named Marcus, looks up from his work."
        ,

                ["General Store"] = @"🏪 GENERAL STORE

        Shelves are packed with potions, rations, and adventuring gear. The merchant greets you with a practiced smile.",

                ["Town Hall"] = @"🏛️ TOWN HALL

        The hall is quiet and orderly. Notices and decrees are pinned along the walls.",

                ["Dark Forest"] = @"🌲 DARK FOREST

        The canopy blocks the light. Every rustle sounds like something watching you.",

                ["Ancient Ruins"] = @"🏚️ ANCIENT RUINS

        Cracked stone pillars and weathered carvings hint at a lost civilization.",

                ["Crystal Cavern"] = @"💎 CRYSTAL CAVERN

        Blue light refracts through crystal walls, casting shifting patterns across the ground.",

                ["Poison Swamp"] = @"🧪 POISON SWAMP

        The air is thick with fumes. The mud bubbles with unsettling movement.",

                ["Frozen Mountain"] = @"❄️ FROZEN MOUNTAIN

        Icy winds cut across the slopes. Snow crunches underfoot with every step.",

                ["Abandoned Temple"] = @"⛪ ABANDONED TEMPLE

        Dusty altars and fading murals speak of forgotten gods.",

                ["Fire Desert"] = @"🏜️ FIRE DESERT

        Heat shimmers across the dunes. Every breath feels dry and heavy.",

                ["Ruined Fortress"] = @"🏰 RUINED FORTRESS

        Broken walls and shattered battlements hint at battles long past.",

                ["Dark Dungeon"] = @"🕳️ DARK DUNGEON

        The tunnels are cold and damp. You hear distant, echoing footsteps.",

                ["AI Tower"] = @"🗼 AI TOWER

        Arcane circuitry and glowing runes pulse with strange energy."
        };

        var description = descriptions.ContainsKey(location) 
            ? descriptions[location] 
            : "You are in an unfamiliar location.";

        var actions = GetLocationActions(location);
        return (description, actions);
    }

    private (string, List<string>) GenerateTavernScene(Character character, string location)
    {
        var response = @"🍺 You enter The Rusty Dragon Tavern...

The warmth and noise of the tavern envelop you. Grundy the bartender nods in your direction.

'Welcome, traveler! What'll it be?'

You notice:
- A bard playing in the corner
- Several adventurers at a table discussing rumors
- A mysterious hooded figure in the shadows
- A quest board with various notices";

        var actions = GetAvailableActions(location);
        return (response, actions);
    }

    private (string, List<string>) GenerateBlacksmithScene(Character character, string location)
    {
        var response = @"⚒️ You enter the Blacksmith's Forge...

Marcus the blacksmith looks up from his anvil.

'Ah, an adventurer! Looking for equipment?'

Available items:
🗡️ Iron Sword - 50 gold
🛡️ Leather Armor - 40 gold
⚔️ Steel Axe - 75 gold";

        var actions = GetAvailableActions(location);
        return (response, actions);
    }

    private (string, List<string>) GenerateGeneralStoreScene(string location)
    {
        var response = @"🏪 You step into the General Store...

Shelves are lined with potions, rations, and adventuring supplies.

""Welcome! Stock up before you head out,"" the merchant says.";

        var actions = new List<string>
        {
            "Browse supplies",
            "Talk to merchant",
            "Talk to Explorador",
            "Buy supplies",
            "Back"
        };

        return (response, actions);
    }

    private (string, List<string>) GenerateTownHallScene(string location)
    {
        var response = @"🏛️ You enter the Town Hall...

Notices line the walls, and the mayor reviews a stack of reports.";

        var actions = GetAvailableActions(location);
        return (response, actions);
    }

    private (string, List<string>) ShowWeapons(string location)
    {
        if (!string.Equals(location, "Blacksmith's Forge", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no weapons on display here.", GetLocationActions(location));
        }

        var response = @"🗡️ WEAPONS FOR SALE

- Iron Sword (50 gold)
- Steel Axe (75 gold)

Marcus says: 'Fine steel, balanced and sharp.'";

        var actions = new List<string>
        {
            "Browse armor",
            "Talk to Marcus",
            "Talk to Explorador",
            "Leave forge"
        };

        return (response, actions);
    }

    private (string, List<string>) ShowArmor(string location)
    {
        if (!string.Equals(location, "Blacksmith's Forge", StringComparison.OrdinalIgnoreCase))
        {
            return ("There is no armor here to inspect.", GetLocationActions(location));
        }

        var response = @"🛡️ ARMOR FOR SALE

- Leather Armor (40 gold)

Marcus says: 'Light, reliable, and easy to maintain.'";

        var actions = new List<string>
        {
            "Browse weapons",
            "Talk to Marcus",
            "Talk to Explorador",
            "Leave forge"
        };

        return (response, actions);
    }

    private (string, List<string>) ShowSupplies(string location)
    {
        if (!string.Equals(location, "General Store", StringComparison.OrdinalIgnoreCase))
        {
            return ("There are no supplies on display here.", GetLocationActions(location));
        }

        var response = @"🧪 SUPPLIES FOR SALE

- Health Potion (25 gold)
- Greater Health Potion (50 gold)

The merchant says: 'Stock up before you head out.'";

        var actions = new List<string>
        {
            "Buy supplies",
            "Talk to merchant",
            "Talk to Explorador",
            "Back"
        };

        return (response, actions);
    }

    private async Task<(string, List<string>)> BuyItemAsync(Character character, string itemName, string location)
    {
        var items = await _itemRepository.GetAllAsync();
        var item = items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        if (item == null)
        {
            return ($"The shop does not carry {itemName}.", GetLocationActions(location));
        }

        if (character.Gold < item.Value)
        {
            return ($"You do not have enough gold to buy {item.Name}.", GetLocationActions(location));
        }

        character.Gold -= item.Value;
        AddItemToInventory(character, item);

        var response = $@"✅ PURCHASE COMPLETE

Item: {item.Name}
Cost: {item.Value} gold
Gold remaining: {character.Gold}";

        return (response, GetLocationActions(location));
    }

    private static void AddItemToInventory(Character character, Item item)
    {
        var existing = character.Inventory.FirstOrDefault(i =>
            i.Item.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));

        if (existing != null && item.IsStackable)
        {
            existing.Quantity += 1;
            return;
        }

        character.Inventory.Add(new InventoryItem
        {
            CharacterId = character.Id,
            Character = character,
            ItemId = item.Id,
            Item = item,
            Quantity = 1,
            IsEquipped = false
        });
    }

    private async Task<(string, List<string>)> GenerateConversationAsync(Character character, GameSession session, string action)
    {
        var location = session.CurrentLocation;
        string? npcName = null;

        // Check if this is a "continue conversation" action
        var normalizedAction = action.ToLower().Trim();
        if (normalizedAction.Contains("continue") || normalizedAction == "ask about quests")
        {
            // Use the NPC from the current conversation session
            npcName = session.CurrentConversationNpc;
        }
        else
        {
            // Try to extract NPC name from action (e.g., "Talk to Elara" -> "Elara")
            npcName = ExtractNpcNameFromAction(action);
            
            // If we successfully extracted an NPC name, save it to the session
            if (!string.IsNullOrEmpty(npcName))
            {
                session.CurrentConversationNpc = npcName;
            }
        }

        // If no NPC name extracted or stored, get the first NPC for this location
        if (string.IsNullOrEmpty(npcName))
        {
            npcName = _npcRegistry.GetNpcNameByLocation(location);
            if (!string.IsNullOrEmpty(npcName))
            {
                session.CurrentConversationNpc = npcName;
            }
        }

        if (npcName == null || !_npcRegistry.Profiles.TryGetValue(npcName, out var agentProfile))
        {
            return ("There's no one nearby to talk to.", new List<string> { "Look around", "Go back" });
        }

        // Marcus in the forge uses full ODA to inspect inventory and recommend one missing item.
        if (string.Equals(npcName, "Marcus", StringComparison.OrdinalIgnoreCase)
            && string.Equals(location, "Blacksmith's Forge", StringComparison.OrdinalIgnoreCase))
        {
            return await HandleMarcusForgeConversationWithOdaAsync(character, session);
        }

        var sessionId = character.Id.ToString();
        var result = await _npcAgentService.RunAsync(
            agentProfile,
            action,
            sessionId,
            new Dictionary<string, object>
            {
                ["playerLevel"] = character.Level,
                ["playerClass"] = character.Class.ToString(),
                ["playerGold"] = character.Gold
            });

        var response = result.Success && !string.IsNullOrEmpty(result.DialogueLine)
            ? $"{npcName}: '{result.DialogueLine}'"
            : GetFallbackConversation(npcName, location);

        var actions = new List<string>
        {
            "Continue conversation",
            "Ask about quests",
            "Say goodbye"
        };

        return (response, actions);
    }

    private async Task<(string, List<string>)> HandleMarcusForgeConversationWithOdaAsync(Character character, GameSession session)
    {
        var forgeItems = await GetMarcusForgeItemsAsync();
        var ownedNames = character.Inventory
            .Where(i => i.Item != null)
            .Select(i => i.Item.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var missingItems = forgeItems
            .Where(i => !ownedNames.Contains(i.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var observe = await _npcAgentService.ObserveAsync(new NpcObserveRequest
        {
            NpcId = character.Id,
            NpcName = "Marcus",
            CurrentLocation = session.CurrentLocation,
            NearbyCharacters = GetNearbyCharactersForLocation(session.CurrentLocation),
            NearbyObjects = missingItems.Select(i => i.Name).DefaultIfEmpty("none").ToList(),
            GameState = new Dictionary<string, object>
            {
                ["playerClass"] = character.Class.ToString(),
                ["playerLevel"] = character.Level,
                ["playerGold"] = character.Gold,
                ["ownedItems"] = ownedNames,
                ["forgeCatalog"] = forgeItems.Select(i => i.Name).ToList(),
                ["missingItems"] = missingItems.Select(i => i.Name).ToList()
            }
        });

        var availableRecommendationActions = missingItems.Count > 0
            ? missingItems.Select(i => $"recommend {i.Name}").Append("no recommendation").ToList()
            : new List<string> { "no recommendation" };

        var decide = await _npcAgentService.DecideAsync(new NpcDecideRequest
        {
            NpcId = character.Id,
            NpcName = "Marcus",
            NpcPersonality = "Pragmatic master blacksmith, direct and practical",
            NpcGoal = "Recommend exactly one useful forge item the adventurer still lacks",
            Observation = observe,
            AvailableActions = availableRecommendationActions
        });

        var recommendedItem = ResolveSingleMarcusRecommendation(missingItems, decide.ChosenAction, decide.TargetEntity);

        var act = await _npcAgentService.ActAsync(new NpcActRequest
        {
            NpcId = character.Id,
            NpcName = "Marcus",
            Action = recommendedItem == null ? "no recommendation" : "recommend purchase",
            TargetEntity = recommendedItem?.Name,
            ActionParameters = new Dictionary<string, object>
            {
                ["gold"] = character.Gold,
                ["ownedItems"] = ownedNames,
                ["missingItems"] = missingItems.Select(i => i.Name).ToList(),
                ["recommendedItem"] = recommendedItem?.Name ?? "none"
            },
            GameState = new Dictionary<string, object>
            {
                ["location"] = session.CurrentLocation,
                ["playerClass"] = character.Class.ToString(),
                ["playerLevel"] = character.Level
            }
        });

        var recommendationLine = recommendedItem == null
            ? "Marcus says your loadout already covers his forge essentials."
            : $"Marcus recommends: {recommendedItem.Name} ({recommendedItem.Value} gold).";

        var response = $@"Marcus: '{act.DialogueLine}'

🧠 MARCUS ODA FORGE ADVICE

Observe:
{observe.EnvironmentSummary}

Decide:
Action: {decide.ChosenAction}
Target: {recommendedItem?.Name ?? "none"}
Reasoning: {decide.Reasoning}

Act:
{act.Outcome}

{recommendationLine}";

        var actions = new List<string>
        {
            "Buy weapon",
            "Buy armor",
            "Browse weapons",
            "Browse armor",
            recommendedItem == null ? "Buy recommended item" : $"Buy recommended item ({recommendedItem.Name})",
            "Continue conversation",
            "Say goodbye"
        };

        return (response, actions);
    }

    private async Task<List<Item>> GetMarcusForgeItemsAsync()
    {
        var allItems = await _itemRepository.GetAllAsync();
        var wanted = new[] { "Iron Sword", "Leather Armor", "Steel Axe" };

        return allItems
            .Where(i => wanted.Contains(i.Name, StringComparer.OrdinalIgnoreCase))
            .OrderBy(i => i.Value)
            .ToList();
    }

    private static Item? ResolveSingleMarcusRecommendation(List<Item> missingItems, string chosenAction, string targetEntity)
    {
        if (missingItems.Count == 0)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(targetEntity))
        {
            var targetMatch = missingItems.FirstOrDefault(i =>
                targetEntity.Contains(i.Name, StringComparison.OrdinalIgnoreCase)
                || i.Name.Contains(targetEntity, StringComparison.OrdinalIgnoreCase));
            if (targetMatch != null)
            {
                return targetMatch;
            }
        }

        if (!string.IsNullOrWhiteSpace(chosenAction))
        {
            var actionMatch = missingItems.FirstOrDefault(i =>
                chosenAction.Contains(i.Name, StringComparison.OrdinalIgnoreCase));
            if (actionMatch != null)
            {
                return actionMatch;
            }
        }

        return missingItems[0];
    }

    private async Task<string?> ResolveCurrentMarcusRecommendationAsync(Character character)
    {
        var forgeItems = await GetMarcusForgeItemsAsync();
        var ownedNames = character.Inventory
            .Where(i => i.Item != null)
            .Select(i => i.Item.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var missingItems = forgeItems
            .Where(i => !ownedNames.Contains(i.Name, StringComparer.OrdinalIgnoreCase))
            .OrderBy(i => i.Value)
            .ToList();

        return missingItems.FirstOrDefault()?.Name;
    }

    private static bool TryExtractRecommendedPurchaseAction(string normalizedAction, out string itemName)
    {
        itemName = string.Empty;
        if (!normalizedAction.StartsWith("buy recommended item", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var withParenthesis = Regex.Match(normalizedAction, @"buy\s+recommended\s+item\s*\(([^\)]+)\)", RegexOptions.IgnoreCase);
        if (withParenthesis.Success)
        {
            itemName = NormalizeItemName(withParenthesis.Groups[1].Value);
            return true;
        }

        var freeText = Regex.Match(normalizedAction, @"buy\s+recommended\s+item\s+(.+)$", RegexOptions.IgnoreCase);
        if (freeText.Success)
        {
            itemName = NormalizeItemName(freeText.Groups[1].Value);
        }

        return true;
    }

    private static string NormalizeItemName(string raw)
    {
        var cleaned = raw.Trim().Trim('.', '!', '?', ':', ';');
        if (string.IsNullOrWhiteSpace(cleaned))
        {
            return string.Empty;
        }

        return string.Join(" ", cleaned
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Length == 1
                ? char.ToUpperInvariant(word[0]).ToString()
                : char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    private async Task<(string, List<string>)> HandleExplorerScoutActionAsync(GameSession session, Character character)
    {
        var location = session.CurrentLocation;
        const string explorerName = "Explorador";
        var nearbyObjects = GetNearbyObjectsForLocation(location);
        var nearbyCharacters = GetNearbyCharactersForLocation(location);

        var observe = await _npcAgentService.ObserveAsync(new NpcObserveRequest
        {
            NpcId = 0,
            NpcName = explorerName,
            CurrentLocation = location,
            NearbyCharacters = nearbyCharacters,
            NearbyObjects = nearbyObjects,
            GameState = new Dictionary<string, object>
            {
                ["playerLevel"] = character.Level,
                ["playerClass"] = character.Class.ToString(),
                ["playerGold"] = character.Gold
            }
        });

        var availableActions = new List<string>
        {
            "search object",
            "pick object",
            "leave object",
            "report findings"
        };

        var decide = await _npcAgentService.DecideAsync(new NpcDecideRequest
        {
            NpcId = 0,
            NpcName = explorerName,
            NpcPersonality = "Prudente, observador y tactico",
            NpcGoal = "Evaluar objetos y decidir si conviene recogerlos o dejarlos",
            Observation = observe,
            AvailableActions = availableActions
        });

        var chosenTarget = string.IsNullOrWhiteSpace(decide.TargetEntity)
            ? nearbyObjects.FirstOrDefault() ?? location
            : decide.TargetEntity;

        var act = await _npcAgentService.ActAsync(new NpcActRequest
        {
            NpcId = 0,
            NpcName = explorerName,
            Action = string.IsNullOrWhiteSpace(decide.ChosenAction) ? "report findings" : decide.ChosenAction,
            TargetEntity = chosenTarget,
            ActionParameters = new Dictionary<string, object>
            {
                ["location"] = location,
                ["nearbyObjects"] = nearbyObjects,
                ["nearbyCharacters"] = nearbyCharacters
            },
            GameState = new Dictionary<string, object>
            {
                ["playerLevel"] = character.Level,
                ["playerClass"] = character.Class.ToString()
            }
        });

        var response = $@"🧭 EXPLORER CYCLE

Observation:
{observe.EnvironmentSummary}

Decision:
Action: {decide.ChosenAction}
Target: {chosenTarget}
Reasoning: {decide.Reasoning}

Action outcome:
{act.Outcome}

Explorador: '{act.DialogueLine}'";

        return (response, GetLocationActions(location));
    }

    private static bool IsExplorerScoutAction(string keyword, string normalizedAction)
    {
        return keyword == "ask explorador to scout"
               || normalizedAction == "ask explorador to scout"
               || normalizedAction == "ask explorer to scout"
               || normalizedAction == "ask explorador scout"
               || normalizedAction == "ask explorer scout"
               || normalizedAction == "explorador scout";
    }

    private static List<string> GetNearbyCharactersForLocation(string location)
    {
        return location switch
        {
            "The Rusty Dragon Tavern" => new List<string> { "Grundy", "Elara", "Bencomo", "adventurer" },
            "Blacksmith's Forge" => new List<string> { "Marcus", "merchant", "apprentice" },
            "General Store" => new List<string> { "Merchant", "customer" },
            "Town Hall" => new List<string> { "Mayor", "guard", "scribe" },
            "AI Tower" => new List<string> { "Alberto Díaz", "AI construct" },
            _ => new List<string> { "traveler", "wanderer" }
        };
    }

    private static List<string> GetNearbyObjectsForLocation(string location)
    {
        return location switch
        {
            "Village Square" => new List<string> { "old crate", "lost pouch", "weathered sign" },
            "The Rusty Dragon Tavern" => new List<string> { "coin purse", "sealed letter", "silver mug" },
            "Blacksmith's Forge" => new List<string> { "steel ingot", "broken dagger", "tool belt" },
            "General Store" => new List<string> { "health potion", "rope", "torch" },
            "Town Hall" => new List<string> { "notice roll", "archived map", "wax-sealed file" },
            "Dark Forest" => new List<string> { "hunter trap", "mossy chest", "arrow bundle" },
            "Ancient Ruins" => new List<string> { "runic tablet", "ancient key", "dusty reliquary" },
            "Crystal Cavern" => new List<string> { "raw crystal", "miner satchel", "glowing shard" },
            "Poison Swamp" => new List<string> { "antidote herb", "rusted lockbox", "bone charm" },
            "Frozen Mountain" => new List<string> { "frozen cache", "climber hook", "worn amulet" },
            "Abandoned Temple" => new List<string> { "cursed idol", "ritual bowl", "temple key" },
            "Fire Desert" => new List<string> { "water flask", "sand-worn compass", "charred relic" },
            "Ruined Fortress" => new List<string> { "captain insignia", "armory token", "sealed chest" },
            "Dark Dungeon" => new List<string> { "trap gear", "prison key", "obsidian fragment" },
            "AI Tower" => new List<string> { "override chip", "encrypted tablet", "energy core" },
            _ => new List<string> { "unidentified object" }
        };
    }

    private static string? ExtractNpcNameFromAction(string action)
    {
        // Extract NPC name from patterns like "Talk to Elara" or "Talk to Alberto Diaz".
        var match = Regex.Match(action, @"talk\s+to\s+(.+)$", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var npcName = match.Groups[1].Value.Trim().Trim('.', '!', '?', ':', ';');
            if (!string.IsNullOrWhiteSpace(npcName))
            {
                var normalizedWords = npcName
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.Length == 1
                        ? char.ToUpperInvariant(word[0]).ToString()
                        : char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant());

                return string.Join(" ", normalizedWords);
            }
        }

        return null;
    }

    private static string GetFallbackConversation(string npcName, string location)
    {
        return npcName switch
        {
            "Grundy" => "Grundy: 'Mira, tronco: en las ruinas hay movida con goblins. Si te manejas bien, te llevas buena pasta, majo.'",
            "Marcus" => "Marcus: 'I've been crafting weapons for 30 years. Need something special? I can make it, given the right materials.'",
            "Elara" => "Elara: 'The road here isn't kind to wanderers. But those with heart and skill... they always find their way. What brings you to our humble tavern?'",
            "Bencomo" => "Bencomo: *se inclina desde las sombras* 'Habla flojito, mi nino... he visto venir y marcharse a muchos. Si te metes en esto, hazlo con cabeza, chacho.'",
            "Explorador" => "Explorador: 'Zona revisada. Si el objeto aporta valor tactico y el riesgo es bajo, lo recojo; si no, lo marco y seguimos.'",
            "Alberto Díaz" => "Alberto Díaz: 'Tu llegaste lejos, mi nino... pero chacho, sin apagar SCanaryNet no tienes chance, pibe.'",
            "Mayor" => "Mayor: 'Welcome to our town. We need capable individuals for various matters. Speak to me again if you're interested in hearing about opportunities.'",
            "Merchant" => "Merchant: 'Welcome, welcome! I have finest supplies from across the realm. Potions, provisions, everything an adventurer needs.'",
            _ => $"{npcName}: 'Hmm, yes? What can I do for you?'"
        };
    }

    private (string, List<string>) ShowInventory(Character character)
    {
        var response = $@"🎒 INVENTORY

💰 Gold: {character.Gold}

Items:
{(character.Inventory?.Any() == true 
    ? string.Join("\n", character.Inventory.Select(i => $"- {i.Item.Name} x{i.Quantity}"))
    : "- Empty")}";

        var actions = new List<string>
        {
            "Use item",
            "Drop item",
            "Close inventory"
        };

        return (response, actions);
    }

    private (string, List<string>) Rest(Character character)
    {
        var healAmount = character.MaxHitPoints - character.HitPoints;
        character.HitPoints = character.MaxHitPoints;

        var response = $@"💤 You rest and recover...

HP restored: +{healAmount}
Current HP: {character.HitPoints}/{character.MaxHitPoints}

You feel refreshed and ready for adventure!";

        var actions = new List<string>
        {
            "Continue adventure",
            "Check inventory",
            "Look around"
        };

        return (response, actions);
    }

    private (string, List<string>) ShowQuests(GameSession session)
    {
        var tracked = string.IsNullOrWhiteSpace(session.CurrentScenario)
            ? "None"
            : session.CurrentScenario.Replace("Tracking: ", string.Empty, StringComparison.OrdinalIgnoreCase);

        var response = @"📜 QUEST LOG

Active Quests:
1. The Goblin Menace
   - Clear out the goblin camp
   - Reward: 100 gold, Iron Sword

Tracked Quest:
- " + tracked + @"

Available Quests:
- Talk to NPCs to discover new quests";

        var actions = new List<string>
        {
            "View quest details",
            "Track quest",
            "Close quest log"
        };

        return (response, actions);
    }

    private (string, List<string>) ViewQuestDetails(GameSession session)
    {
        var tracked = string.IsNullOrWhiteSpace(session.CurrentScenario)
            ? "The Goblin Menace"
            : session.CurrentScenario.Replace("Tracking: ", string.Empty, StringComparison.OrdinalIgnoreCase);

        var response = $@"📘 QUEST DETAILS

Name: {tracked}
Objective: Clear out the goblin camp
Location: Old Ruins, east of the village (past the stone bridge)
Reward: 100 gold, Iron Sword

Tip: Bring healing supplies and stay alert for ambushes.";

        var actions = new List<string>
        {
            "Track quest",
            "View quest log",
            "Close quest log"
        };

        return (response, actions);
    }

    private (string, List<string>) TrackQuest(GameSession session)
    {
        session.CurrentScenario = "Tracking: The Goblin Menace";

        var response = @"🧭 QUEST TRACKING ENABLED

Tracking: The Goblin Menace
Objective: Clear out the goblin camp
Hint: The ruins lie east of the village, past the old stone bridge.

You can now follow leads toward the goblin camp.";

        var actions = new List<string>
        {
            "Go to tavern",
            "Look around",
            "View quest log",
            "Stop tracking"
        };

        return (response, actions);
    }

    private (string, List<string>) ClearTrackedQuest(GameSession session)
    {
        session.CurrentScenario = string.Empty;

        var response = @"🧭 QUEST TRACKING DISABLED

You stop tracking your current quest.";

        var actions = new List<string>
        {
            "View quest log",
            "Look around"
        };

        return (response, actions);
    }

    private (string, List<string>) InitiateCombat(string location)
    {
        var response = @"⚔️ COMBAT INITIATED!

A goblin jumps out from the shadows!

🧌 Goblin Scout
HP: 25/25
ATK: 6

Prepare for battle!";

        var actions = new List<string>
        {
            "Attack",
            "Defend",
            "Use item",
            "Flee"
        };

        return (response, actions);
    }

    private (string, List<string>) RollDice(string action)
    {
        try
        {
            var notation = "1d20";
            if (action.Contains("d"))
            {
                var parts = action.Split(' ');
                foreach (var part in parts)
                {
                    if (part.Contains("d"))
                    {
                        notation = part;
                        break;
                    }
                }
            }

            var result = _diceRoller.Roll(notation);
            var response = $@"🎲 DICE ROLL

{result}

The dice have spoken!";

            var actions = new List<string>
            {
                "Roll again",
                "Continue adventure"
            };

            return (response, actions);
        }
        catch
        {
            return ("Invalid dice notation. Try something like '1d20' or '2d6'.", new List<string> { "Continue" });
        }
    }

    private (string, List<string>) GenerateGenericResponse(string action, string location)
    {
        var response = $@"You attempt to {action}...

*The dungeon master considers your action*

That's an interesting approach! In {location}, you find that your action has some effect, though perhaps not what you expected.";

        var actions = GetLocationActions(location);
        return (response, actions);
    }

    private List<string> GetLocationActions(string location)
    {
        return _actionService.GetAvailableActions(location);
    }

    private string GetClassName(CharacterClass characterClass)
    {
        return characterClass.ToString();
    }
}
