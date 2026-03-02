# Quick Start Guide - D&D Copilot

This guide will help you get the D&D Copilot game up and running quickly.

## Prerequisites

- .NET 9.0 SDK
- Node.js 20.x or higher
- npm 10.x or higher

## Step 1: Start the Backend API

1. Open a terminal and navigate to the API directory:
```bash
cd src/DndCopilot.Api
```

2. Run the API:
```bash
dotnet run
```

The API will start and you should see:
```
Now listening on: http://localhost:5000
```

The API includes Swagger documentation available at: http://localhost:5000/swagger

## Step 2: Start the Frontend

1. Open a **new terminal** and navigate to the client directory:
```bash
cd client
```

2. Install dependencies (first time only):
```bash
npm install
```

3. Start the development server:
```bash
npm start
```

The frontend will automatically open in your browser at: http://localhost:3000

## Step 3: Create Your First Character

1. On the login screen, click "Register" to create a new account
2. Fill in:
   - Username: Choose any username
   - Email: Enter any email
   - Password: Choose a password
3. Click "Register"
4. You'll be automatically logged in
5. Click "+ Create Character"
6. Enter a character name (e.g., "Aragorn")
7. Select a class (Warrior, Mage, Rogue, Cleric, or Ranger)
8. Click "Create Character"

## Step 4: Roll Some Dice!

Use the Dice Roller on the right side of the dashboard:
- Type a dice notation like `2d6`, `1d20+5`, or `3d8`
- Click "Roll" or use one of the quick roll buttons
- See your results!

## API Testing with cURL

If you want to test the API directly, here are some example commands:

### Register a User
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"hero","email":"hero@example.com","password":"mypassword"}'
```

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"hero","password":"mypassword"}'
```

### Create a Character (replace TOKEN with your actual token)
```bash
curl -X POST http://localhost:5000/api/characters \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"name":"Gandalf","class":1}'
```

### Roll Dice (replace TOKEN with your actual token)
```bash
curl -X POST http://localhost:5000/api/dice/roll \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"notation":"2d6"}'
```

## Database

The application uses SQLite and automatically creates a database file named `dndcopilot.db` in the API directory. The database is automatically seeded with:
- 10 items (weapons, armor, potions)
- 5 NPCs (Goblin, Orc Warrior, Skeleton, Giant Rat, Dark Mage)
- 6 loot table entries
- 1 multi-stage quest

## Character Classes

- **Warrior (⚔️)**: Rolls 1d10 + STR modifier for damage
- **Mage (🔮)**: Rolls 1d6 + INT modifier for damage
- **Rogue (🗡️)**: Rolls 1d8 + DEX modifier for damage
- **Cleric (✨)**: Rolls 1d8 + WIS modifier for damage
- **Ranger (🏹)**: Rolls 1d8 + DEX modifier for damage

## NPC Behaviors

- **Aggressive**: Always attacks
- **Defensive**: Attacks but deals reduced damage
- **Flee**: Will attempt to flee when HP is low (< 30%)

## Troubleshooting

### Port Already in Use
If you get an error that the port is already in use:
- For API: Edit `src/DndCopilot.Api/Properties/launchSettings.json` and change the port numbers
- For Frontend: The React app will automatically try the next available port

### Database Issues
If you encounter database issues:
```bash
cd src/DndCopilot.Api
rm dndcopilot.db
dotnet run
```
This will recreate the database with fresh seed data.

### Frontend Build Errors
If you encounter build errors in the frontend:
```bash
cd client
rm -rf node_modules package-lock.json
npm install
npm start
```

## Running Tests

To run the backend unit tests:
```bash
cd /path/to/dnd-copilot
dotnet test
```

All 10 tests should pass:
- 6 dice roller tests
- 4 combat service tests

## Next Steps

- Explore the API documentation at http://localhost:5000/swagger
- Create multiple characters with different classes
- Try different dice notations in the dice roller
- Check out the seed data in the database to see available items and NPCs

## Support

For issues or questions, please refer to the main README.md file for more detailed documentation.

Happy adventuring! 🎲
