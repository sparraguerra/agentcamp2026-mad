using DndCopilot.Core.Entities;
using DndCopilot.Core.Enums;

namespace DndCopilot.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(GameDbContext context)
    {
        // Ensure at least one user exists for development/testing
        if (!context.Users.Any())
        {
            context.Users.Add(new User
            {
                Username = "demo",
                Email = "demo@example.com",
                PasswordHash = "dev-placeholder",
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }

        // Check if items already seeded
        if (context.Items.Any())
        {
            return; // Database has been seeded
        }

        // Seed Items
        var items = new[]
        {
            // Weapons
            new Item
            {
                Name = "Iron Sword",
                Description = "A basic iron sword",
                Type = ItemType.Weapon,
                IsStackable = false,
                IsEquippable = true,
                Value = 50,
                AttackBonus = 5
            },
            new Item
            {
                Name = "Steel Axe",
                Description = "A heavy steel axe",
                Type = ItemType.Weapon,
                IsStackable = false,
                IsEquippable = true,
                Value = 75,
                AttackBonus = 7
            },
            new Item
            {
                Name = "Wooden Staff",
                Description = "A magical wooden staff",
                Type = ItemType.Weapon,
                IsStackable = false,
                IsEquippable = true,
                Value = 60,
                AttackBonus = 4
            },
            new Item
            {
                Name = "Dagger",
                Description = "A sharp dagger",
                Type = ItemType.Weapon,
                IsStackable = false,
                IsEquippable = true,
                Value = 30,
                AttackBonus = 3
            },
            // Armor
            new Item
            {
                Name = "Leather Armor",
                Description = "Basic leather protection",
                Type = ItemType.Armor,
                IsStackable = false,
                IsEquippable = true,
                Value = 40,
                DefenseBonus = 3
            },
            new Item
            {
                Name = "Chain Mail",
                Description = "Heavy chain mail armor",
                Type = ItemType.Armor,
                IsStackable = false,
                IsEquippable = true,
                Value = 100,
                DefenseBonus = 6
            },
            // Potions
            new Item
            {
                Name = "Health Potion",
                Description = "Restores 50 HP",
                Type = ItemType.Potion,
                IsStackable = true,
                IsEquippable = false,
                Value = 25,
                HealthRestore = 50
            },
            new Item
            {
                Name = "Greater Health Potion",
                Description = "Restores 100 HP",
                Type = ItemType.Potion,
                IsStackable = true,
                IsEquippable = false,
                Value = 50,
                HealthRestore = 100
            },
            // Misc
            new Item
            {
                Name = "Gold Coin",
                Description = "Currency of the realm",
                Type = ItemType.Misc,
                IsStackable = true,
                IsEquippable = false,
                Value = 1
            },
            new Item
            {
                Name = "Mysterious Key",
                Description = "A key to an unknown door",
                Type = ItemType.QuestItem,
                IsStackable = false,
                IsEquippable = false,
                Value = 0
            }
        };

        context.Items.AddRange(items);
        context.SaveChanges();

        // Seed NPCs
        var npcs = new[]
        {
            new Npc
            {
                Name = "Goblin",
                Description = "A small, green creature",
                HitPoints = 25,
                MaxHitPoints = 25,
                AttackPower = 6,
                Defense = 2,
                Behavior = NpcBehavior.Aggressive,
                IsHostile = true,
                ExperienceReward = 50,
                GoldReward = 10
            },
            new Npc
            {
                Name = "Orc Warrior",
                Description = "A fierce orc warrior",
                HitPoints = 50,
                MaxHitPoints = 50,
                AttackPower = 10,
                Defense = 4,
                Behavior = NpcBehavior.Aggressive,
                IsHostile = true,
                ExperienceReward = 100,
                GoldReward = 25
            },
            new Npc
            {
                Name = "Skeleton",
                Description = "An undead skeleton",
                HitPoints = 30,
                MaxHitPoints = 30,
                AttackPower = 7,
                Defense = 3,
                Behavior = NpcBehavior.Defensive,
                IsHostile = true,
                ExperienceReward = 75,
                GoldReward = 15
            },
            new Npc
            {
                Name = "Giant Rat",
                Description = "A large, aggressive rat",
                HitPoints = 15,
                MaxHitPoints = 15,
                AttackPower = 4,
                Defense = 1,
                Behavior = NpcBehavior.Flee,
                IsHostile = true,
                ExperienceReward = 25,
                GoldReward = 5
            },
            new Npc
            {
                Name = "Dark Mage",
                Description = "A powerful dark sorcerer",
                HitPoints = 40,
                MaxHitPoints = 40,
                AttackPower = 12,
                Defense = 2,
                Behavior = NpcBehavior.Defensive,
                IsHostile = true,
                ExperienceReward = 150,
                GoldReward = 50
            }
        };

        context.Npcs.AddRange(npcs);
        context.SaveChanges();

        // Seed Loot Tables
        var lootTables = new[]
        {
            // Goblin loot
            new LootTable
            {
                NpcId = npcs[0].Id,
                ItemId = items[3].Id, // Dagger
                DropChance = 0.15,
                MinQuantity = 1,
                MaxQuantity = 1
            },
            new LootTable
            {
                NpcId = npcs[0].Id,
                ItemId = items[6].Id, // Health Potion
                DropChance = 0.3,
                MinQuantity = 1,
                MaxQuantity = 2
            },
            // Orc Warrior loot
            new LootTable
            {
                NpcId = npcs[1].Id,
                ItemId = items[1].Id, // Steel Axe
                DropChance = 0.2,
                MinQuantity = 1,
                MaxQuantity = 1
            },
            new LootTable
            {
                NpcId = npcs[1].Id,
                ItemId = items[5].Id, // Chain Mail
                DropChance = 0.15,
                MinQuantity = 1,
                MaxQuantity = 1
            },
            // Dark Mage loot
            new LootTable
            {
                NpcId = npcs[4].Id,
                ItemId = items[2].Id, // Wooden Staff
                DropChance = 0.25,
                MinQuantity = 1,
                MaxQuantity = 1
            },
            new LootTable
            {
                NpcId = npcs[4].Id,
                ItemId = items[7].Id, // Greater Health Potion
                DropChance = 0.5,
                MinQuantity = 1,
                MaxQuantity = 3
            }
        };

        context.LootTables.AddRange(lootTables);
        context.SaveChanges();

        // Seed Quests
        var quest = new Quest
        {
            Name = "The Goblin Menace",
            Description = "Clear out the goblin camp threatening the village",
            RequiredLevel = 1,
            ExperienceReward = 200,
            GoldReward = 100
        };

        context.Quests.Add(quest);
        context.SaveChanges();

        // Seed Quest Stages
        var questStages = new[]
        {
            new QuestStage
            {
                QuestId = quest.Id,
                Order = 1,
                Description = "Speak with the village elder",
                CompletionCriteria = "Talk to Elder Marcus"
            },
            new QuestStage
            {
                QuestId = quest.Id,
                Order = 2,
                Description = "Find the goblin camp",
                CompletionCriteria = "Locate camp in Dark Woods"
            },
            new QuestStage
            {
                QuestId = quest.Id,
                Order = 3,
                Description = "Defeat 5 goblins",
                CompletionCriteria = "Kill 5 goblins"
            },
            new QuestStage
            {
                QuestId = quest.Id,
                Order = 4,
                Description = "Return to the village",
                CompletionCriteria = "Report to Elder Marcus"
            }
        };

        context.QuestStages.AddRange(questStages);
        context.SaveChanges();

        // Seed Quest Rewards
        var questRewards = new[]
        {
            new QuestReward
            {
                QuestId = quest.Id,
                ItemId = items[0].Id, // Iron Sword
                Quantity = 1
            },
            new QuestReward
            {
                QuestId = quest.Id,
                ItemId = items[4].Id, // Leather Armor
                Quantity = 1
            }
        };

        context.QuestRewards.AddRange(questRewards);
        context.SaveChanges();
    }
}
