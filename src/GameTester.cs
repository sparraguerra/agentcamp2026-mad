using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class GameTester
{
    private static readonly HttpClient client = new HttpClient();
    private const string API_BASE = "http://localhost:5000/api";
    private static string? token;
    private static int characterId;
    private static int sessionId;

    static async Task Main()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("   D&D COPILOT - COMPREHENSIVE TEST SUITE");
        Console.WriteLine("========================================\n");

        // Wait for API
        Console.Write("Waiting for API...");
        for (int i = 0; i < 30; i++)
        {
            try
            {
                var response = await client.GetAsync($"{API_BASE}/dice/validate?notation=1d20");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\nAPI is ready!\n");
                    break;
                }
            }
            catch { }
            await Task.Delay(1000);
            if (i == 29) throw new Exception("API failed to start");
        }

        try
        {
            // 1. Authentication Tests
            Console.WriteLine("========================================");
            Console.WriteLine("1. AUTHENTICATION TESTS");
            Console.WriteLine("========================================");
            
            string testEmail = $"testuser_{Guid.NewGuid().GetHashCode()}@test.com";
            string testPassword = "TestPassword123!";
            
            await RegisterUser(testEmail, testPassword);
            await LoginUser(testEmail, testPassword);
            
            // 2. Character Creation
            Console.WriteLine("\n========================================");
            Console.WriteLine("2. CHARACTER CREATION TESTS");
            Console.WriteLine("========================================");
            
            await CreateCharacter();
            
            // 3. Game Session
            Console.WriteLine("\n========================================");
            Console.WriteLine("3. GAME SESSION TESTS");
            Console.WriteLine("========================================");
            
            await StartGame();
            
            // 4. Location & Navigation
            Console.WriteLine("\n========================================");
            Console.WriteLine("4. LOCATION & NAVIGATION TESTS");
            Console.WriteLine("========================================");
            
            await TestLocationActions();
            
            // 5. NPC Interactions
            Console.WriteLine("\n========================================");
            Console.WriteLine("5. NPC INTERACTION TESTS");
            Console.WriteLine("========================================");
            
            await TestNpcInteractions();
            
            // 6. Inventory
            Console.WriteLine("\n========================================");
            Console.WriteLine("6. INVENTORY TESTS");
            Console.WriteLine("========================================");
            
            await TestInventory();
            
            // 7. Dice Roller
            Console.WriteLine("\n========================================");
            Console.WriteLine("7. DICE ROLLER TESTS");
            Console.WriteLine("========================================");
            
            await TestDiceRoller();
            
            // 8. Rest
            Console.WriteLine("\n========================================");
            Console.WriteLine("8. REST & HEALING TESTS");
            Console.WriteLine("========================================");
            
            await TestRest();
            
            Console.WriteLine("\n========================================");
            Console.WriteLine("ALL TESTS COMPLETED!");
            Console.WriteLine("========================================");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.ResetColor();
        }
    }

    static async Task RegisterUser(string email, string password)
    {
        var data = new { email, password };
        var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{API_BASE}/auth/register", content);
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Register user: SUCCESS");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Register user: FAILED - {response.StatusCode}");
            Console.ResetColor();
            throw new Exception("Registration failed");
        }
    }

    static async Task LoginUser(string email, string password)
    {
        var data = new { email, password };
        var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{API_BASE}/auth/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(responseText))
            {
                token = doc.RootElement.GetProperty("token").GetString();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Login user: SUCCESS");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Login user: FAILED - {response.StatusCode}");
            Console.ResetColor();
            throw new Exception("Login failed");
        }
    }

    static async Task CreateCharacter()
    {
        var data = new { 
            name = $"TestHero_{Guid.NewGuid().GetHashCode()}", 
            @class = 1 
        };
        var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE}/characters")
        {
            Content = content
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(responseText))
            {
                characterId = doc.RootElement.GetProperty("id").GetInt32();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ Create character: SUCCESS (ID: {characterId})");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Create character: FAILED - {response.StatusCode}");
            Console.ResetColor();
            throw new Exception("Character creation failed");
        }
    }

    static async Task StartGame()
    {
        var data = new { characterId };
        var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE}/game/start")
        {
            Content = content
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            using (var doc = JsonDocument.Parse(responseText))
            {
                sessionId = doc.RootElement.GetProperty("sessionId").GetInt32();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ Start game session: SUCCESS (Session ID: {sessionId})");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Start game session: FAILED - {response.StatusCode}");
            Console.ResetColor();
            throw new Exception("Game start failed");
        }
    }

    static async Task TestLocationActions()
    {
        var actions = new[] { "look", "examine", "go to tavern", "visit blacksmith" };
        
        foreach (var action in actions)
        {
            await PerformGameAction(action);
        }
    }

    static async Task TestNpcInteractions()
    {
        var actions = new[] { "talk to bartender", "talk to marcus", "continue conversation" };
        
        foreach (var action in actions)
        {
            await PerformGameAction(action);
        }
    }

    static async Task TestInventory()
    {
        var actions = new[] { "check inventory", "inventory", "items" };
        
        foreach (var action in actions)
        {
            await PerformGameAction(action);
        }
    }

    static async Task TestDiceRoller()
    {
        var notations = new[] { "1d20", "2d6", "1d20+5", "3d8-2" };
        
        foreach (var notation in notations)
        {
            await RollDice(notation);
        }
    }

    static async Task TestRest()
    {
        await PerformGameAction("rest");
    }

    static async Task PerformGameAction(string action)
    {
        var data = new { sessionId, action };
        var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE}/game/action")
        {
            Content = content
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Action '{action}': SUCCESS");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ Action '{action}': {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static async Task RollDice(string notation)
    {
        var data = new { notation };
        var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE}/dice/roll")
        {
            Content = content
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Dice roll '{notation}': SUCCESS");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ Dice roll '{notation}': {response.StatusCode}");
            Console.ResetColor();
        }
    }
}
