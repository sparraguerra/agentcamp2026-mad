namespace DnDAdventure;

public class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    public string Description { get; set; }
    public bool IsBoss { get; set; }

    public Enemy(string name, int health, int attackPower, int defense, string description, bool isBoss = false)
    {
        Name = name;
        Health = health;
        AttackPower = attackPower;
        Defense = defense;
        Description = description;
        IsBoss = isBoss;
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public int TakeDamage(int damage)
    {
        int actualDamage = Math.Max(0, damage - Defense);
        Health -= actualDamage;
        return actualDamage;
    }

    public int Attack()
    {
        if (IsBoss)
        {
            // Alberto Díaz uses AI power - more unpredictable attacks
            Random rand = new Random();
            int aiBonus = rand.Next(0, 10);
            return AttackPower + aiBonus;
        }
        return AttackPower;
    }
}
