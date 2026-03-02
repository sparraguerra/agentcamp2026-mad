# NPC Agent Service

The NPC Agent Service provides AI-powered NPC behavior using an observe-decide-act pattern, integrated with Foundry AI for response generation and Dapr pub/sub for event distribution.

## Architecture

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  NpcAgentAPI    │────▶│ NpcAgentService  │────▶│  Foundry AI     │
│  (Controller)   │     │  (Core Service)  │     │  (AI Client)    │
└─────────────────┘     └──────────────────┘     └─────────────────┘
                               │
                               ▼
                        ┌──────────────────┐
                        │  Dapr Pub/Sub    │
                        │  (Events)        │
                        └──────────────────┘
```

## Endpoints

### POST /api/npcagent/observe

Observes the game context and returns NPC perceptions.

**Request:**
```json
{
  "npcId": 1,
  "npcName": "Guard Captain",
  "currentLocation": "Village Square",
  "nearbyCharacters": ["Hero", "Merchant", "Goblin Scout"],
  "nearbyObjects": ["Fountain", "Market Stall"],
  "gameState": {
    "timeOfDay": "morning",
    "weather": "clear"
  }
}
```

**Response:**
```json
{
  "success": true,
  "npcId": "1",
  "perceivedThreats": ["Goblin Scout"],
  "perceivedOpportunities": ["Merchant"],
  "perceivedNeutral": ["Hero"],
  "environmentSummary": "The guard surveys the bustling village square, noting the goblin scout lurking near the fountain."
}
```

### POST /api/npcagent/decide

Decides the next action for an NPC based on observations.

**Request:**
```json
{
  "npcId": 1,
  "npcName": "Guard Captain",
  "npcPersonality": "Brave and protective",
  "npcGoal": "Protect the village",
  "observation": {
    "success": true,
    "npcId": "1",
    "perceivedThreats": ["Goblin Scout"],
    "perceivedOpportunities": [],
    "perceivedNeutral": ["Hero"],
    "environmentSummary": "Danger nearby"
  },
  "availableActions": ["attack", "defend", "patrol", "talk", "flee"]
}
```

**Response:**
```json
{
  "success": true,
  "chosenAction": "attack",
  "reasoning": "Threat detected - a goblin scout threatens village safety. Immediate action required.",
  "targetEntity": "Goblin Scout",
  "confidence": 0.85
}
```

### POST /api/npcagent/act

Executes the decided action and returns the result.

**Request:**
```json
{
  "npcId": 1,
  "npcName": "Guard Captain",
  "action": "attack",
  "targetEntity": "Goblin Scout",
  "actionParameters": {
    "weapon": "sword",
    "stance": "aggressive"
  },
  "gameState": {}
}
```

**Response:**
```json
{
  "success": true,
  "actionPerformed": "attack",
  "outcome": "The Guard Captain charges at the Goblin Scout with sword drawn.",
  "dialogueLine": "For the village! You shall not threaten our people!",
  "generatedEvents": [
    {
      "eventType": "npc.action.attack",
      "source": "Guard Captain",
      "target": "Goblin Scout",
      "timestamp": "2024-01-15T10:30:00Z"
    }
  ]
}
```

## Prompt Examples

### Observation Prompt

```
NPC Name: Guard Captain
Location: Village Square
Nearby Characters: Hero, Merchant, Goblin Scout
Nearby Objects: Fountain, Market Stall

Describe what this NPC observes in their environment in 2-3 sentences.
```

### Decision Prompt

```
NPC: Guard Captain
Personality: Brave and protective
Goal: Protect the village

Observations:
- Threats: Goblin Scout
- Opportunities: Merchant
- Environment: The guard surveys the bustling village square.

Available Actions: attack, defend, patrol, talk, flee

Choose the best action for this NPC and explain the reasoning. Format:
ACTION: [chosen action]
TARGET: [target entity if any]
REASONING: [brief explanation]
CONFIDENCE: [0.0-1.0]
```

### Action Execution Prompt

```
NPC: Guard Captain
Action: attack
Target: Goblin Scout
Parameters: {"weapon": "sword", "stance": "aggressive"}

Describe the outcome of this action and provide a dialogue line for the NPC. Format:
OUTCOME: [action result description]
DIALOGUE: [what the NPC says]
```

## Python Prompt Pipeline

The `scripts/prompt_pipeline.py` script normalizes game context before calling Foundry AI.

### Usage

```bash
# From a JSON file
python scripts/prompt_pipeline.py --context game_context.json --prompt-type observe

# From JSON string
python scripts/prompt_pipeline.py --context-json '{"npcId": 1, "npcName": "Guard"}' --prompt-type observe

# From stdin
cat game_context.json | python scripts/prompt_pipeline.py --prompt-type decide

# With specific actions
python scripts/prompt_pipeline.py --context game_context.json --prompt-type act --action "attack" --target "Goblin"
```

### Example Input (game_context.json)

```json
{
  "npc": {
    "id": 1,
    "name": "Guard Captain",
    "personality": "Brave and protective",
    "goal": "Protect the village",
    "hitPoints": 80,
    "maxHitPoints": 100
  },
  "environment": {
    "location": "Village Square",
    "timeOfDay": "morning",
    "weather": "clear",
    "lighting": "bright"
  },
  "nearbyCharacters": [
    {"name": "Hero", "distance": "nearby", "threatLevel": "friendly"},
    {"name": "Goblin Scout", "distance": "close", "threatLevel": "hostile"}
  ],
  "nearbyObjects": ["Fountain", "Market Stall"],
  "gameState": {
    "questActive": true,
    "alertLevel": "high"
  }
}
```

### Example Output

```json
{
  "normalized_context": {
    "npc": {
      "npc_id": 1,
      "npc_name": "Guard Captain",
      "personality": "Brave and protective",
      "goal": "Protect the village",
      "current_mood": "alert",
      "health_percentage": 0.8
    },
    "environment": {
      "location": "Village Square",
      "time_of_day": "morning",
      "weather": "clear",
      "lighting": "bright",
      "ambient_sounds": []
    },
    "entities": [
      {
        "name": "Hero",
        "entity_type": "character",
        "distance": "nearby",
        "threat_level": "friendly",
        "description": ""
      },
      {
        "name": "Goblin Scout",
        "entity_type": "character", 
        "distance": "close",
        "threat_level": "hostile",
        "description": ""
      }
    ],
    "game_state": {
      "questActive": true,
      "alertLevel": "high"
    }
  },
  "prompt": "NPC: Guard Captain\nLocation: Village Square\n...",
  "prompt_type": "observe"
}
```

## Dapr Pub/Sub Events

The service publishes events to the `npc-events` topic:

| Event Type | Description |
|------------|-------------|
| `npc.observed` | NPC completed observation phase |
| `npc.decided` | NPC made a decision |
| `npc.action.*` | NPC performed an action (e.g., `npc.action.attack`) |

### Event Schema

```json
{
  "eventType": "npc.action.attack",
  "source": "Guard Captain",
  "target": "Goblin Scout",
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "outcome": "The attack was successful",
    "parameters": {}
  }
}
```

## Configuration

### Foundry AI Settings

Configure in `appsettings.json`:

```json
{
  "FoundryAi": {
    "Endpoint": "https://your-foundry-endpoint/v1/completions",
    "ApiKey": "your-api-key"
  }
}
```

### Dapr Pub/Sub Settings

```json
{
  "Dapr": {
    "HttpPort": "3500",
    "PubSubName": "pubsub"
  }
}
```

## Testing

Run the unit tests:

```bash
dotnet test tests/DndCopilot.Tests/DndCopilot.Tests.csproj --filter "FullyQualifiedName~NpcAgentServiceTests"
```

## Development Mode

In development mode, stub implementations are used:
- `StubFoundryAiClient`: Returns predefined responses without calling the AI
- `StubDaprEventPublisher`: Logs events locally without Dapr

To use real implementations, update `Program.cs` to register:
- `FoundryAiClient` instead of `StubFoundryAiClient`
- `DaprEventPublisher` instead of `StubDaprEventPublisher`
