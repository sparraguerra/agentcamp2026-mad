namespace DnDAdventure;

public class Game
{
    private Player player;
    private List<Location> locations;
    private int currentLocationIndex;
    private Random random;

    public Game()
    {
        random = new Random();
        locations = new List<Location>();
        currentLocationIndex = 0;
        player = null!;
    }

    public void Start()
    {
        ShowWelcomeMessage();
        InitializePlayer();
        InitializeLocations();
        GameLoop();
    }

    private void ShowWelcomeMessage()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                       ║");
        Console.WriteLine("║          ⚔️  AVENTURA DE DUNGEONS & DRAGONS ⚔️         ║");
        Console.WriteLine("║                                                       ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("Bienvenido a una épica aventura donde deberás explorar");
        Console.WriteLine("10 peligrosas localizaciones, enfrentarte a enemigos,");
        Console.WriteLine("y finalmente desafiar al temible boss final:");
        Console.WriteLine("¡ALBERTO DÍAZ, el maestro de la IA!");
        Console.WriteLine();
    }

    private void InitializePlayer()
    {
        Console.Write("Introduce el nombre de tu héroe: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Aventurero";
        }
        player = new Player(name);
        Console.WriteLine($"\n¡Bienvenido, {player.Name}!");
        Console.WriteLine("Presiona cualquier tecla para comenzar tu aventura...");
        Console.ReadKey();
    }

    private void InitializeLocations()
    {
        // Create 9 regular locations
        locations.Add(new Location(
            "Bosque Oscuro",
            "Un denso bosque donde apenas penetra la luz del sol. Los árboles antiguos susurran secretos olvidados."
        ));

        locations.Add(new Location(
            "Caverna de Cristal",
            "Una cueva iluminada por cristales brillantes que emiten una luz azulada. El eco de tus pasos resuena constantemente."
        ));

        locations.Add(new Location(
            "Ruinas Ancestrales",
            "Los restos de una civilización perdida. Las piedras talladas cuentan historias de glorias pasadas."
        ));

        locations.Add(new Location(
            "Pantano Venenoso",
            "Un pantano lleno de gases tóxicos y criaturas peligrosas. Cada paso debe ser cuidadoso."
        ));

        locations.Add(new Location(
            "Montaña Helada",
            "Una montaña cubierta de nieve eterna. El viento gélido corta como cuchillas."
        ));

        locations.Add(new Location(
            "Templo Abandonado",
            "Un antiguo templo donde los dioses ya no responden. El silencio es ensordecedor."
        ));

        locations.Add(new Location(
            "Desierto de Fuego",
            "Un desierto árido donde el sol abrasa sin piedad. La arena quema bajo tus pies."
        ));

        locations.Add(new Location(
            "Fortaleza en Ruinas",
            "Una fortaleza que una vez fue imponente, ahora destruida por el tiempo y las batallas."
        ));

        locations.Add(new Location(
            "Mazmorra Profunda",
            "Un laberinto subterráneo lleno de trampas mortales y criaturas sedientas de sangre."
        ));

        // Create the final location (10th)
        locations.Add(new Location(
            "Torre de la IA",
            "Una torre futurista donde la magia y la tecnología se fusionan. En su cima aguarda Alberto Díaz, maestro absoluto de la Inteligencia Artificial.",
            true
        ));

        // Add random enemies to each regular location (0-2 enemies)
        for (int i = 0; i < 9; i++)
        {
            int enemyCount = random.Next(0, 3); // 0, 1, or 2 enemies
            for (int j = 0; j < enemyCount; j++)
            {
                locations[i].AddEnemy(GenerateRandomEnemy());
            }

            // Add some items to locations
            if (random.Next(0, 3) == 0) // 33% chance
            {
                locations[i].AddItem("Poción de Salud");
            }
            if (random.Next(0, 5) == 0) // 20% chance
            {
                locations[i].AddItem("Poción de Fuerza");
            }
        }

        // Add the final boss to the last location
        Enemy finalBoss = new Enemy(
            "Alberto Díaz",
            200,
            30,
            10,
            "El maestro supremo de la Inteligencia Artificial. Sus ojos brillan con el poder de mil algoritmos. Cada movimiento es calculado con precisión milimétrica.",
            true
        );
        locations[9].AddEnemy(finalBoss);
    }

    private Enemy GenerateRandomEnemy()
    {
        string[] enemyTypes = {
            "Goblin", "Orco", "Esqueleto", "Araña Gigante", "Lobo", 
            "Bandido", "Zombie", "Serpiente Venenosa", "Murciélago Vampiro", "Troll"
        };

        string[] descriptions = {
            "Una criatura malévola que acecha en las sombras.",
            "Un enemigo feroz con sed de batalla.",
            "Una bestia hambrienta que no conoce la piedad.",
            "Un monstruo peligroso que defiende su territorio.",
            "Una criatura salvaje con instintos asesinos."
        };

        string type = enemyTypes[random.Next(enemyTypes.Length)];
        int health = random.Next(20, 50);
        int attack = random.Next(5, 15);
        int defense = random.Next(0, 5);
        string description = descriptions[random.Next(descriptions.Length)];

        return new Enemy(type, health, attack, defense, description);
    }

    private void GameLoop()
    {
        while (player.IsAlive() && currentLocationIndex < locations.Count)
        {
            Location currentLocation = locations[currentLocationIndex];

            // Check if this is the final location and player hasn't visited all others
            if (currentLocation.IsFinalLocation)
            {
                int visitedCount = locations.Take(9).Count(l => l.Visited);
                if (visitedCount < 9)
                {
                    Console.Clear();
                    Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
                    Console.WriteLine("║             ⚠️  ACCESO DENEGADO ⚠️                    ║");
                    Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
                    Console.WriteLine();
                    Console.WriteLine("Una barrera mágica te impide el paso a la Torre de la IA.");
                    Console.WriteLine($"Has visitado {visitedCount} de 9 localizaciones requeridas.");
                    Console.WriteLine("Debes explorar todas las localizaciones antes de enfrentarte al boss final.");
                    Console.WriteLine();
                    Console.WriteLine("Presiona cualquier tecla para regresar...");
                    Console.ReadKey();
                    
                    // Go back to a previous location
                    currentLocationIndex = random.Next(0, 9);
                    continue;
                }
            }

            if (!currentLocation.Visited)
            {
                ShowLocationIntro(currentLocation);
                currentLocation.Visited = true;

                // Collect items if any
                if (currentLocation.Items.Count > 0)
                {
                    Console.WriteLine("\n¡Has encontrado algunos objetos!");
                    foreach (var item in currentLocation.Items)
                    {
                        player.Inventory.Add(item);
                        Console.WriteLine($"- {item}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }

            // Combat loop
            while (currentLocation.HasEnemies() && player.IsAlive())
            {
                Enemy? enemy = currentLocation.GetNextEnemy();
                if (enemy != null)
                {
                    Combat(enemy);
                }
            }

            // Check if player is still alive
            if (!player.IsAlive())
            {
                ShowGameOver();
                return;
            }

            // Check if this was the final location
            if (currentLocation.IsFinalLocation)
            {
                ShowVictory();
                return;
            }

            // Move to next location
            ShowLocationComplete();
            currentLocationIndex++;
        }
    }

    private void ShowLocationIntro(Location location)
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  📍 {location.Name.PadRight(48)} ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine(location.Description);
        Console.WriteLine();

        int enemyCount = location.Enemies.Count;
        if (enemyCount > 0)
        {
            if (location.IsFinalLocation)
            {
                Console.WriteLine("⚠️  ¡El boss final te espera en esta localización!");
            }
            else
            {
                Console.WriteLine($"⚔️  Detectas {enemyCount} {(enemyCount == 1 ? "enemigo" : "enemigos")} en esta área.");
            }
        }
        else
        {
            Console.WriteLine("✓ Esta localización parece segura, no hay enemigos a la vista.");
        }
        
        Console.WriteLine();
        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void Combat(Enemy enemy)
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   ⚔️  COMBATE ⚔️                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        if (enemy.IsBoss)
        {
            Console.WriteLine("🔥🔥🔥 ¡BATALLA CONTRA EL BOSS FINAL! 🔥🔥🔥");
            Console.WriteLine();
        }

        Console.WriteLine($"¡Un {enemy.Name} aparece ante ti!");
        Console.WriteLine(enemy.Description);
        Console.WriteLine();
        Console.WriteLine($"Enemigo: {enemy.Name}");
        Console.WriteLine($"Salud: {enemy.Health}");
        Console.WriteLine($"Ataque: {enemy.AttackPower}");
        Console.WriteLine($"Defensa: {enemy.Defense}");
        Console.WriteLine();
        Console.WriteLine("Presiona cualquier tecla para prepararte para el combate...");
        Console.ReadKey();

        bool defending = false;

        while (enemy.IsAlive() && player.IsAlive())
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   ⚔️  COMBATE ⚔️                      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Enemigo: {enemy.Name} - Salud: {enemy.Health}");
            Console.WriteLine($"Jugador: {player.Name} - Salud: {player.Health}/{player.MaxHealth}");
            Console.WriteLine();
            Console.WriteLine("¿Qué deseas hacer?");
            Console.WriteLine("1. Atacar");
            Console.WriteLine("2. Defender");
            Console.WriteLine("3. Usar objeto");
            Console.WriteLine("4. Huir");
            Console.WriteLine();
            Console.Write("Elige una opción (1-4): ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": // Attack
                    defending = false;
                    int playerDamage = player.Attack();
                    int damageDealt = enemy.TakeDamage(playerDamage);
                    Console.WriteLine();
                    Console.WriteLine($"¡Atacas a {enemy.Name} e infliges {damageDealt} puntos de daño!");
                    
                    if (!enemy.IsAlive())
                    {
                        Console.WriteLine($"¡Has derrotado a {enemy.Name}!");
                        int expReward = enemy.IsBoss ? 100 : random.Next(20, 40);
                        player.GainExperience(expReward);
                        Console.WriteLine();
                        Console.WriteLine("Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        return;
                    }
                    break;

                case "2": // Defend
                    Console.WriteLine();
                    Console.WriteLine("¡Te preparas para defender el próximo ataque!");
                    Console.WriteLine("Tu defensa aumenta temporalmente.");
                    defending = true;
                    break;

                case "3": // Use item
                    Console.WriteLine();
                    player.ShowInventory();
                    Console.WriteLine("1. Usar Poción de Salud");
                    Console.WriteLine("2. Usar Poción de Fuerza");
                    Console.WriteLine("3. Cancelar");
                    Console.Write("Elige una opción: ");
                    string? itemChoice = Console.ReadLine();

                    if (itemChoice == "1")
                    {
                        player.UseHealthPotion();
                    }
                    else if (itemChoice == "2")
                    {
                        player.UseStrengthPotion();
                    }
                    else
                    {
                        Console.WriteLine("Acción cancelada.");
                        continue; // Don't let enemy attack if cancelled
                    }
                    break;

                case "4": // Flee
                    if (enemy.IsBoss)
                    {
                        Console.WriteLine();
                        Console.WriteLine("¡No puedes huir de la batalla final!");
                        Console.WriteLine("Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        continue;
                    }

                    int fleeChance = random.Next(0, 100);
                    if (fleeChance < 50)
                    {
                        Console.WriteLine();
                        Console.WriteLine("¡Has huido con éxito del combate!");
                        Console.WriteLine("Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        enemy.Health = 0; // Mark enemy as defeated for simplification
                        return;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("¡No has conseguido huir!");
                    }
                    break;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Opción no válida.");
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    continue; // Don't let enemy attack on invalid input
            }

            // Enemy's turn
            if (enemy.IsAlive())
            {
                int enemyDamage = enemy.Attack();
                
                if (defending)
                {
                    int originalDefense = player.Defense;
                    player.Defense += 5;
                    int damageReceived = player.TakeDamage(enemyDamage);
                    player.Defense = originalDefense;
                    Console.WriteLine($"{enemy.Name} contraataca, pero tu defensa reduce el daño a {damageReceived} puntos.");
                    defending = false;
                }
                else
                {
                    int damageReceived = player.TakeDamage(enemyDamage);
                    Console.WriteLine($"{enemy.Name} contraataca e inflige {damageReceived} puntos de daño.");
                    
                    if (enemy.IsBoss)
                    {
                        Console.WriteLine("¡Alberto Díaz utiliza el poder de la IA para optimizar su ataque!");
                    }
                }

                if (!player.IsAlive())
                {
                    Console.WriteLine();
                    Console.WriteLine("¡Has sido derrotado!");
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    private void ShowLocationComplete()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║              ✓ LOCALIZACIÓN COMPLETADA ✓              ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"Has completado {locations[currentLocationIndex].Name}.");
        
        int visitedCount = locations.Take(currentLocationIndex + 1).Count(l => l.Visited);
        Console.WriteLine($"Localizaciones visitadas: {visitedCount}/{locations.Count}");
        
        player.ShowStats();
        
        if (currentLocationIndex < 8) // Not yet at the final location
        {
            Console.WriteLine("Presiona cualquier tecla para continuar a la siguiente localización...");
        }
        else if (currentLocationIndex == 8) // Just completed 9th location
        {
            Console.WriteLine("¡Has visitado todas las localizaciones regulares!");
            Console.WriteLine("La barrera mágica de la Torre de la IA se ha disipado.");
            Console.WriteLine("Estás listo para enfrentarte a Alberto Díaz...");
            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla cuando estés preparado...");
        }
        
        Console.ReadKey();
    }

    private void ShowGameOver()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  💀 GAME OVER 💀                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"{player.Name}, has caído en tu aventura.");
        Console.WriteLine($"Alcanzaste el nivel {player.Level}.");
        
        int visitedCount = locations.Count(l => l.Visited);
        Console.WriteLine($"Visitaste {visitedCount} de {locations.Count} localizaciones.");
        Console.WriteLine();
        Console.WriteLine("Pero los héroes nunca mueren... ¡su leyenda vive para siempre!");
    }

    private void ShowVictory()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║             🏆 ¡VICTORIA ÉPICA! 🏆                    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("🎉🎉🎉 ¡FELICIDADES! 🎉🎉🎉");
        Console.WriteLine();
        Console.WriteLine("Has derrotado a Alberto Díaz, el maestro de la IA,");
        Console.WriteLine("¡y completado todas las localizaciones!");
        Console.WriteLine();
        Console.WriteLine($"Héroe: {player.Name}");
        Console.WriteLine($"Nivel Final: {player.Level}");
        Console.WriteLine($"Salud Restante: {player.Health}/{player.MaxHealth}");
        Console.WriteLine();
        Console.WriteLine("Tu nombre será recordado en las leyendas por siempre.");
        Console.WriteLine("¡Eres el verdadero campeón de esta aventura!");
    }
}
