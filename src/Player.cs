namespace DnDAdventure;

public class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    public List<string> Inventory { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }

    public Player(string name)
    {
        Name = name;
        MaxHealth = 100;
        Health = MaxHealth;
        AttackPower = 15;
        Defense = 5;
        Level = 1;
        Experience = 0;
        Inventory = new List<string> { "Poción de Salud", "Poción de Salud", "Poción de Fuerza" };
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public int TakeDamage(int damage)
    {
        int actualDamage = Math.Max(0, damage - Defense);
        Health -= actualDamage;
        if (Health < 0) Health = 0;
        return actualDamage;
    }

    public int Attack()
    {
        return AttackPower;
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth;
    }

    public void UseHealthPotion()
    {
        if (Inventory.Contains("Poción de Salud"))
        {
            Inventory.Remove("Poción de Salud");
            Heal(40);
            Console.WriteLine($"¡Has usado una Poción de Salud! Salud restaurada: {Health}/{MaxHealth}");
        }
        else
        {
            Console.WriteLine("No tienes pociones de salud en tu inventario.");
        }
    }

    public void UseStrengthPotion()
    {
        if (Inventory.Contains("Poción de Fuerza"))
        {
            Inventory.Remove("Poción de Fuerza");
            AttackPower += 10;
            Console.WriteLine($"¡Has usado una Poción de Fuerza! Tu poder de ataque aumenta a {AttackPower}");
        }
        else
        {
            Console.WriteLine("No tienes pociones de fuerza en tu inventario.");
        }
    }

    public void GainExperience(int exp)
    {
        Experience += exp;
        Console.WriteLine($"¡Has ganado {exp} puntos de experiencia!");
        
        // Level up every 100 XP
        while (Experience >= 100)
        {
            LevelUp();
            Experience -= 100;
        }
    }

    private void LevelUp()
    {
        Level++;
        MaxHealth += 20;
        Health = MaxHealth;
        AttackPower += 5;
        Defense += 2;
        Console.WriteLine($"¡Has subido al nivel {Level}!");
        Console.WriteLine($"Estadísticas: Salud: {MaxHealth}, Ataque: {AttackPower}, Defensa: {Defense}");
    }

    public void ShowInventory()
    {
        Console.WriteLine("\n=== Inventario ===");
        if (Inventory.Count == 0)
        {
            Console.WriteLine("Tu inventario está vacío.");
        }
        else
        {
            var itemGroups = Inventory.GroupBy(i => i);
            foreach (var group in itemGroups)
            {
                Console.WriteLine($"- {group.Key} x{group.Count()}");
            }
        }
        Console.WriteLine("==================\n");
    }

    public void ShowStats()
    {
        Console.WriteLine("\n=== Estadísticas del Jugador ===");
        Console.WriteLine($"Nombre: {Name}");
        Console.WriteLine($"Nivel: {Level}");
        Console.WriteLine($"Salud: {Health}/{MaxHealth}");
        Console.WriteLine($"Ataque: {AttackPower}");
        Console.WriteLine($"Defensa: {Defense}");
        Console.WriteLine($"Experiencia: {Experience}/100");
        Console.WriteLine("================================\n");
    }
}
