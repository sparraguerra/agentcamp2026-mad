#!/usr/bin/env python3
"""
prompt_pipeline.py - Normalizes game context before calling Foundry AI

This script processes game context data and normalizes it into a format
suitable for Foundry AI prompts used by the NpcAgentService.

Usage:
    python prompt_pipeline.py --context <json_file> --prompt-type <observe|decide|act>
    
Example:
    python prompt_pipeline.py --context game_context.json --prompt-type observe
"""

import argparse
import json
import sys
from typing import Any
from dataclasses import dataclass, asdict


@dataclass
class NpcContext:
    """Normalized NPC context for AI prompts."""
    npc_id: int
    npc_name: str
    personality: str = ""
    goal: str = ""
    current_mood: str = "neutral"
    health_percentage: float = 1.0


@dataclass
class EnvironmentContext:
    """Normalized environment context for AI prompts."""
    location: str
    time_of_day: str = "day"
    weather: str = "clear"
    lighting: str = "normal"
    ambient_sounds: list = None

    def __post_init__(self):
        if self.ambient_sounds is None:
            self.ambient_sounds = []


@dataclass
class EntityContext:
    """Normalized entity context (characters, objects) for AI prompts."""
    name: str
    entity_type: str  # "character", "object", "creature"
    distance: str = "nearby"  # "close", "nearby", "far"
    threat_level: str = "neutral"  # "friendly", "neutral", "hostile"
    description: str = ""


@dataclass
class NormalizedGameContext:
    """Fully normalized game context ready for AI prompt generation."""
    npc: NpcContext
    environment: EnvironmentContext
    entities: list
    game_state: dict

    def __post_init__(self):
        if self.entities is None:
            self.entities = []
        if self.game_state is None:
            self.game_state = {}


def normalize_npc_context(raw_context: dict) -> NpcContext:
    """Normalize raw NPC data into structured context."""
    npc_data = raw_context.get("npc", {})
    
    # Calculate health percentage
    current_hp = npc_data.get("hit_points", npc_data.get("hitPoints", 100))
    max_hp = npc_data.get("max_hit_points", npc_data.get("maxHitPoints", 100))
    health_pct = current_hp / max_hp if max_hp > 0 else 0
    
    # Determine mood based on health and situation
    mood = determine_mood(health_pct, raw_context)
    
    return NpcContext(
        npc_id=npc_data.get("id", npc_data.get("npcId", 0)),
        npc_name=npc_data.get("name", npc_data.get("npcName", "Unknown NPC")),
        personality=npc_data.get("personality", ""),
        goal=npc_data.get("goal", ""),
        current_mood=mood,
        health_percentage=health_pct
    )


def determine_mood(health_pct: float, context: dict) -> str:
    """Determine NPC mood based on health and context."""
    if health_pct < 0.2:
        return "desperate"
    elif health_pct < 0.5:
        return "worried"
    
    # Check for threats
    threats = context.get("perceived_threats", context.get("perceivedThreats", []))
    if threats:
        return "alert"
    
    return "neutral"


def normalize_environment(raw_context: dict) -> EnvironmentContext:
    """Normalize raw environment data into structured context."""
    env_data = raw_context.get("environment", {})
    location = raw_context.get("current_location", 
                               raw_context.get("currentLocation",
                               env_data.get("location", "Unknown")))
    
    return EnvironmentContext(
        location=location,
        time_of_day=env_data.get("time_of_day", env_data.get("timeOfDay", "day")),
        weather=env_data.get("weather", "clear"),
        lighting=env_data.get("lighting", "normal"),
        ambient_sounds=env_data.get("ambient_sounds", env_data.get("ambientSounds", []))
    )


def normalize_entities(raw_context: dict) -> list:
    """Normalize raw entity lists into structured context."""
    entities = []
    
    # Process nearby characters
    characters = raw_context.get("nearby_characters", 
                                 raw_context.get("nearbyCharacters", []))
    for char in characters:
        if isinstance(char, str):
            entity = EntityContext(
                name=char,
                entity_type="character",
                threat_level=classify_threat(char)
            )
        else:
            entity = EntityContext(
                name=char.get("name", "Unknown"),
                entity_type="character",
                distance=char.get("distance", "nearby"),
                threat_level=char.get("threat_level", char.get("threatLevel", "neutral")),
                description=char.get("description", "")
            )
        entities.append(entity)
    
    # Process nearby objects
    objects = raw_context.get("nearby_objects", 
                              raw_context.get("nearbyObjects", []))
    for obj in objects:
        if isinstance(obj, str):
            entity = EntityContext(
                name=obj,
                entity_type="object"
            )
        else:
            entity = EntityContext(
                name=obj.get("name", "Unknown Object"),
                entity_type="object",
                distance=obj.get("distance", "nearby"),
                description=obj.get("description", "")
            )
        entities.append(entity)
    
    return entities


def classify_threat(entity_name: str) -> str:
    """Simple heuristic threat classification based on entity name."""
    name_lower = entity_name.lower()
    
    hostile_keywords = ["goblin", "orc", "troll", "dragon", "demon", "undead", 
                        "skeleton", "zombie", "enemy", "hostile", "bandit", "thief"]
    friendly_keywords = ["merchant", "ally", "friend", "guard", "healer", "priest",
                         "villager", "innkeeper", "bartender"]
    
    for keyword in hostile_keywords:
        if keyword in name_lower:
            return "hostile"
    
    for keyword in friendly_keywords:
        if keyword in name_lower:
            return "friendly"
    
    return "neutral"


def normalize_game_context(raw_context: dict) -> NormalizedGameContext:
    """Normalize raw game context into structured format for AI prompts."""
    return NormalizedGameContext(
        npc=normalize_npc_context(raw_context),
        environment=normalize_environment(raw_context),
        entities=normalize_entities(raw_context),
        game_state=raw_context.get("game_state", raw_context.get("gameState", {}))
    )


def generate_observe_prompt(context: NormalizedGameContext) -> str:
    """Generate observation prompt from normalized context."""
    entities_desc = []
    for entity in context.entities:
        threat_marker = ""
        if entity.threat_level == "hostile":
            threat_marker = " [THREAT]"
        elif entity.threat_level == "friendly":
            threat_marker = " [FRIENDLY]"
        entities_desc.append(f"- {entity.name} ({entity.entity_type}){threat_marker}")
    
    entities_text = "\n".join(entities_desc) if entities_desc else "- No entities nearby"
    
    return f"""NPC: {context.npc.npc_name}
Location: {context.environment.location}
Time: {context.environment.time_of_day}
Weather: {context.environment.weather}
Lighting: {context.environment.lighting}
NPC Mood: {context.npc.current_mood}
NPC Health: {context.npc.health_percentage:.0%}

Nearby Entities:
{entities_text}

Describe what this NPC observes in their environment in 2-3 sentences, considering their current mood and health state."""


def generate_decide_prompt(context: NormalizedGameContext, available_actions: list = None) -> str:
    """Generate decision prompt from normalized context."""
    if available_actions is None:
        available_actions = ["wait", "patrol", "interact", "flee", "attack", "defend"]
    
    threats = [e for e in context.entities if e.threat_level == "hostile"]
    opportunities = [e for e in context.entities if e.threat_level == "friendly"]
    
    threats_text = ", ".join([t.name for t in threats]) if threats else "None detected"
    opportunities_text = ", ".join([o.name for o in opportunities]) if opportunities else "None detected"
    
    return f"""NPC: {context.npc.npc_name}
Personality: {context.npc.personality or "Standard NPC behavior"}
Goal: {context.npc.goal or "Maintain current position"}
Current Mood: {context.npc.current_mood}
Health: {context.npc.health_percentage:.0%}

Observations:
- Location: {context.environment.location}
- Threats: {threats_text}
- Opportunities: {opportunities_text}
- Environment: {context.environment.time_of_day}, {context.environment.weather}

Available Actions: {", ".join(available_actions)}

Choose the best action for this NPC and explain the reasoning. Format:
ACTION: [chosen action]
TARGET: [target entity if any]
REASONING: [brief explanation]
CONFIDENCE: [0.0-1.0]"""


def generate_act_prompt(context: NormalizedGameContext, action: str, target: str = "") -> str:
    """Generate action execution prompt from normalized context."""
    return f"""NPC: {context.npc.npc_name}
Action: {action}
Target: {target or "None"}
Location: {context.environment.location}
NPC Mood: {context.npc.current_mood}

Describe the outcome of this action and provide a dialogue line for the NPC.
Consider the NPC's personality and current emotional state.

Format:
OUTCOME: [action result description]
DIALOGUE: [what the NPC says]"""


def process_context(raw_context: dict, prompt_type: str, 
                    available_actions: list = None, 
                    action: str = None, 
                    target: str = None) -> dict:
    """Process raw context and generate appropriate prompt."""
    normalized = normalize_game_context(raw_context)
    
    if prompt_type == "observe":
        prompt = generate_observe_prompt(normalized)
    elif prompt_type == "decide":
        prompt = generate_decide_prompt(normalized, available_actions)
    elif prompt_type == "act":
        if not action:
            raise ValueError("Action is required for 'act' prompt type")
        prompt = generate_act_prompt(normalized, action, target or "")
    else:
        raise ValueError(f"Unknown prompt type: {prompt_type}")
    
    return {
        "normalized_context": {
            "npc": asdict(normalized.npc),
            "environment": asdict(normalized.environment),
            "entities": [asdict(e) for e in normalized.entities],
            "game_state": normalized.game_state
        },
        "prompt": prompt,
        "prompt_type": prompt_type
    }


def main():
    """Main entry point for the prompt pipeline script."""
    parser = argparse.ArgumentParser(
        description="Normalize game context for Foundry AI prompts"
    )
    parser.add_argument(
        "--context", "-c",
        type=str,
        help="Path to JSON file containing raw game context"
    )
    parser.add_argument(
        "--context-json",
        type=str,
        help="Raw JSON string containing game context"
    )
    parser.add_argument(
        "--prompt-type", "-t",
        type=str,
        choices=["observe", "decide", "act"],
        required=True,
        help="Type of prompt to generate"
    )
    parser.add_argument(
        "--action", "-a",
        type=str,
        help="Action for 'act' prompt type"
    )
    parser.add_argument(
        "--target",
        type=str,
        help="Target entity for action"
    )
    parser.add_argument(
        "--available-actions",
        type=str,
        nargs="+",
        help="Available actions for 'decide' prompt type"
    )
    parser.add_argument(
        "--output", "-o",
        type=str,
        help="Output file path (default: stdout)"
    )
    
    args = parser.parse_args()
    
    # Load context
    if args.context:
        with open(args.context, "r") as f:
            raw_context = json.load(f)
    elif args.context_json:
        raw_context = json.loads(args.context_json)
    else:
        # Read from stdin
        raw_context = json.load(sys.stdin)
    
    # Process and generate prompt
    try:
        result = process_context(
            raw_context,
            args.prompt_type,
            args.available_actions,
            args.action,
            args.target
        )
    except ValueError as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)
    
    # Output result
    output_json = json.dumps(result, indent=2)
    
    if args.output:
        with open(args.output, "w") as f:
            f.write(output_json)
    else:
        print(output_json)


if __name__ == "__main__":
    main()
