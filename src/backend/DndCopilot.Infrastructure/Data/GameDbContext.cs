using Microsoft.EntityFrameworkCore;
using DndCopilot.Core.Entities;

namespace DndCopilot.Infrastructure.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Npc> Npcs { get; set; }
    public DbSet<LootTable> LootTables { get; set; }
    public DbSet<Quest> Quests { get; set; }
    public DbSet<QuestStage> QuestStages { get; set; }
    public DbSet<QuestReward> QuestRewards { get; set; }
    public DbSet<CharacterQuest> CharacterQuests { get; set; }
    public DbSet<CombatEncounter> CombatEncounters { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });

        // Character configuration
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Characters)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Item configuration
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // InventoryItem configuration
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                .WithMany(c => c.Inventory)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Npc configuration
        modelBuilder.Entity<Npc>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // LootTable configuration
        modelBuilder.Entity<LootTable>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Npc)
                .WithMany(n => n.LootTables)
                .HasForeignKey(e => e.NpcId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Quest configuration
        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // QuestStage configuration
        modelBuilder.Entity<QuestStage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Quest)
                .WithMany(q => q.Stages)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // QuestReward configuration
        modelBuilder.Entity<QuestReward>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Quest)
                .WithMany(q => q.Rewards)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CharacterQuest configuration
        modelBuilder.Entity<CharacterQuest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                .WithMany(c => c.Quests)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Quest)
                .WithMany()
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CombatEncounter configuration
        modelBuilder.Entity<CombatEncounter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Npc)
                .WithMany()
                .HasForeignKey(e => e.NpcId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GameSession configuration
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.CurrentLocation).HasMaxLength(200);
        });
    }
}
