using System.Text.RegularExpressions;
using DndCopilot.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DndCopilot.Infrastructure.Services;

/// <summary>
/// Mock implementation of IFoundryAiClient for development/testing
/// when Dapr Conversation service is not available.
/// This version processes conversation history to generate contextual responses.
/// </summary>
public class MockFoundryAiClient : IFoundryAiClient
{
    private readonly ILogger<MockFoundryAiClient> _logger;

    private readonly Dictionary<string, Dictionary<string, string[]>> _contextualResponses = new()
    {
        ["grundy"] = new()
        {
            ["greeting"] = new[]
            {
                "Que pasa, tronco, que te trae por la taberna? Vienes por jarra o por aventura?",
                "Bienvenido, majo. Si es tu primera vez, pilla sitio junto al fuego y no te me cortes.",
            },
            ["goblins"] = new[]
            {
                "Esos goblins la estan liando en las ruinas, chaval. El barrio anda mosqueado con el tema.",
                "Si tienes manos, hay buena pasta limpiando ese campamento. Cien de oro, ni tan mal.",
                "Te va la movida de los goblins, no? Pues hace falta gente fina, de barrio, que no se raje.",
            },
            ["gold"] = new[]
            {
                "La pasta anda justa, tronco, pero con lo de los goblins te puedes forrar un fisquito.",
                "Oro quieres? Si no te achantas, aqui hay forma de sacarlo, majo.",
            },
            ["tavern"] = new[]
            {
                "Llevo anos llevando esta taberna y he visto de todo, tronco: heroes, vendehumos y valientes de verdad.",
                "La mejor taberna de la zona, te lo digo yo. Buena jarra, buena gente y cero tonterias.",
            },
            ["bard"] = new[]
            {
                "La barda de la esquina se marca unos temas finos, majo. Entra la bebida sola.",
                "Hoy la musica esta de lujo, chaval. Igual te anima a meterte en alguna quest de las gordas.",
            },
            ["default"] = new[]
            {
                "Ya ves, tronco. Tomate una jarra y lo piensas con calma.",
                "Que mas te cuento, majo?",
                "Asi va la vida, chaval. Te pongo otra o que?",
            }
        },
        ["marcus"] = new()
        {
            ["greeting"] = new[]
            {
                "Ah, an adventurer! Looking for some quality gear, are ye?",
                "Welcome to me smithy! What brings ye here?",
            },
            ["sword"] = new[]
            {
                "That Iron Sword will serve ye well against goblins and bandits. Sharp as the day I forged it.",
                "Looking for a good blade, eh? I've got just the thing for ye.",
            },
            ["armor"] = new[]
            {
                "The Leather Armor I make is light but protective. Perfect for an adventurer who needs mobility.",
                "Good armor keeps ye alive, and me craft is the finest there is.",
            },
            ["axe"] = new[]
            {
                "The Steel Axe is me pride. Hefty weapon, but deals tremendous damage in the right hands.",
                "An axe, ye say? Aye, I've got a beauty that'll cleave through anything.",
            },
            ["custom"] = new[]
            {
                "I can make ye custom gear if ye bring me the right materials. Dragon scales, mithril ore, that sort of thing.",
                "With the proper materials, I can craft something truly special for ye.",
            },
            ["weapon"] = new[]
            {
                "Ye look like ye could handle yourself in a fight. What kind of weapon speaks to ye?",
                "Tell me what ye need, and I'll forge it true.",
            },
            ["default"] = new[]
            {
                "Every piece I craft has a bit of me soul in it.",
                "Quality work takes time, but it's worth every minute.",
                "Anything else ye need, friend?",
            }
        },
        ["elara"] = new()
        {
            ["greeting"] = new[]
            {
                "Ah, viajero de mirada inquieta... toma asiento y deja que el laúd te cuente lo que el camino aún calla.",
                "Bienvenido a mi rincón de melodías; cada héroe merece una canción antes de su próxima batalla.",
            },
            ["song"] = new[]
            {
                "Si lo deseas, puedo entonar una balada sobre tus pasos, nivel y fortuna, para que la taberna recuerde tu nombre.",
                "Las cuerdas ya susurran tu historia; dime y convertiré tu aventura en verso.",
            },
            ["tower"] = new[]
            {
                "La Torre de la IA aparece en viejas leyendas élficas; no todos los que la buscan regresan con la misma alma.",
                "Dicen que en la Torre la magia y la máquina hablan el mismo idioma, pero exigen un precio.",
            },
            ["secret"] = new[]
            {
                "Hay rutas menos transitadas entre ruinas y cavernas; a veces, la sombra protege más que la espada.",
                "Escucha: no toda victoria está en combatir; hay puertas que solo ceden ante quien observa primero.",
            },
            ["default"] = new[]
            {
                "Tu historia tiene ritmo, aventurero. Continúa, y yo pondré música a tus decisiones.",
                "Hablas como quien aún no conoce su mejor capítulo; sigamos escribiéndolo.",
            }
        },
        ["bencomo"] = new()
        {
            ["greeting"] = new[]
            {
                "Habla bajito, mi niño, que aquí las paredes oyen más de la cuenta, chacho.",
                "Acércate sin hacer ruido, pibe... hay asuntos que no conviene soltar en voz alta.",
            },
            ["tower"] = new[]
            {
                "Lo de la Torre de la IA no es cuento, mi niño: ahí arriba se mueve algo feo de verdad.",
                "En la Torre se están juntando fuerzas que ni los magos viejos quieren mentar, vale.",
            },
            ["goblins"] = new[]
            {
                "Los goblins son solo la cortina, ¿me entiendes, chacho? Detrás hay manos más peligrosas.",
                "No te despistes con los goblins; eso es un fisquito del problema, nada más.",
            },
            ["deal"] = new[]
            {
                "Si investigas las ruinas primero, yo te consigo una recompensa que no sale en ningún tablón, mi niño.",
                "Hazme ese trabajo y tendrás pago... y respuestas. Pero con discreción, pibe, que esto no es juego.",
            },
            ["default"] = new[]
            {
                "No preguntes de golpe, chacho. Aquí la información se gana poco a poco.",
                "Ojo avizor y boca cerrada, mi niño: esa es la única forma de salir vivo de esto.",
            }
        },
    };

    public MockFoundryAiClient(ILogger<MockFoundryAiClient> logger)
    {
        _logger = logger;
    }

    public async Task<FoundryAiResponse> GenerateCompletionAsync(string prompt, FoundryAiParameters? parameters = null)
    {
        // Simulate a slight delay (as a real API would have)
        await Task.Delay(Random.Shared.Next(100, 300));

        _logger.LogDebug("MockFoundryAiClient: Processing prompt with system prompt: {SystemPrompt}", 
            parameters?.SystemPrompt?.Substring(0, Math.Min(50, parameters.SystemPrompt?.Length ?? 0)) + "...");

        // Extract NPC name from system prompt
        var npcName = ExtractNpcName(parameters?.SystemPrompt ?? "");
        
        // Generate contextual response based on conversation history
        var response = GenerateContextualResponse(npcName, prompt);

        _logger.LogInformation("MockFoundryAiClient: Generated response for NPC {NpcName}", npcName);

        return new FoundryAiResponse
        {
            Success = true,
            Content = response,
            Metadata = new Dictionary<string, object>
            {
                ["source"] = "mock-ai-contextual",
                ["npc"] = npcName,
                ["response_type"] = "contextual"
            }
        };
    }

    private string GenerateContextualResponse(string npcName, string prompt)
    {
        // Normalize NPC name
        npcName = npcName?.ToLower() ?? "";
        
        if (!_contextualResponses.ContainsKey(npcName))
        {
            return GetGenericResponse(prompt);
        }

        var npcRegistry = _contextualResponses[npcName];
        var selectedCategory = DetermineCategory(prompt, npcRegistry.Keys);

        if (npcRegistry.TryGetValue(selectedCategory, out var responses))
        {
            return responses[Random.Shared.Next(responses.Length)];
        }

        return npcRegistry["default"][Random.Shared.Next(npcRegistry["default"].Length)];
    }

    private string DetermineCategory(string prompt, IEnumerable<string> availableCategories)
    {
        var promptLower = prompt.ToLower();

        // Check for topic keywords in the prompt
        foreach (var category in availableCategories)
        {
            if (category == "default") continue;

            // Match keywords for each category
            var keywords = category switch
            {
                "greeting" => new[] { "hello", "hi", "welcome", "greetings", "speak to", "talk to", "meet" },
                "goblins" => new[] { "goblin", "goblins", "ruins", "monsters", "quest", "trouble", "problem" },
                "gold" => new[] { "gold", "coin", "coins", "money", "pay", "reward", "payment" },
                "tavern" => new[] { "tavern", "inn", "bar", "drink", "ale" },
                "bard" => new[] { "bard", "music", "song", "play" },
                "song" => new[] { "song", "sing", "music", "ballad", "balada", "cancion", "cantar", "laud" },
                "tower" => new[] { "tower", "ia tower", "ai tower", "torre", "tecnologia", "technology" },
                "secret" => new[] { "secret", "hidden", "ruta", "route", "clue", "pista", "sombra", "shadow" },
                "deal" => new[] { "deal", "reward", "recompensa", "trato", "mission", "mision", "investigar", "ruins" },
                "sword" => new[] { "sword", "blade", "weapon", "sharp" },
                "armor" => new[] { "armor", "protection", "protect", "leather" },
                "axe" => new[] { "axe", "axes", "staff", "polearm" },
                "custom" => new[] { "custom", "special", "unique", "dragon", "mithril", "material", "craft" },
                "weapon" => new[] { "weapon", "gear", "equipment", "item" },
                _ => Array.Empty<string>()
            };

            foreach (var keyword in keywords)
            {
                if (promptLower.Contains(keyword))
                {
                    return category;
                }
            }
        }

        // Check conversation history for continuity
        var historyContext = ExtractConversationContext(prompt);
        foreach (var category in availableCategories)
        {
            if (category == "default") continue;
            
            if (historyContext.Contains(category))
            {
                return category;
            }
        }

        return "default";
    }

    private string ExtractConversationContext(string prompt)
    {
        // Extract the conversation history part (which comes before the "Respond to" line)
        var contextMatch = Regex.Match(prompt, @"(?:Player|You):\s*(.+?)(?=Respond to the|$)", 
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        if (contextMatch.Success)
        {
            return contextMatch.Groups[1].Value.ToLower();
        }

        return prompt.ToLower();
    }

    private static string ExtractNpcName(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        var textLower = text.ToLower();

        if (textLower.Contains("grundy") || textLower.Contains("bartender"))
            return "grundy";

        if (textLower.Contains("marcus") || textLower.Contains("blacksmith") || textLower.Contains("smith"))
            return "marcus";

        if (textLower.Contains("elara"))
            return "elara";

        if (textLower.Contains("bencomo"))
            return "bencomo";

        return string.Empty;
    }

    private static string GetGenericResponse(string prompt)
    {
        var responses = new[]
        {
            "The NPC looks at you kindly and says: 'That's an interesting thing to say. What else is on your mind?'",
            "The NPC nods thoughtfully: 'I see. I hadn't considered that before. Anything else you'd like to discuss?'",
            "The NPC chuckles: 'Ha! You've got a sense of humor. I like that in an adventurer.'",
            "The NPC strokes their chin: 'Hmm, that's a curious approach. What brings you to that conclusion?'",
            "The NPC smiles warmly: 'You seem like a person of substance. I respect that.'",
        };

        return responses[Random.Shared.Next(responses.Length)];
    }
}
