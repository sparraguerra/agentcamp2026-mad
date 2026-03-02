namespace DnDAdventure;

public class Location
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Enemy> Enemies { get; set; }
    public bool Visited { get; set; }
    public bool IsFinalLocation { get; set; }
    public List<string> Items { get; set; }

    public Location(string name, string description, bool isFinalLocation = false)
    {
        Name = name;
        Description = description;
        Enemies = new List<Enemy>();
        Visited = false;
        IsFinalLocation = isFinalLocation;
        Items = new List<string>();
    }

    public void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void AddItem(string item)
    {
        Items.Add(item);
    }

    public bool HasEnemies()
    {
        return Enemies.Any(e => e.IsAlive());
    }

    public Enemy? GetNextEnemy()
    {
        return Enemies.FirstOrDefault(e => e.IsAlive());
    }
}
