# dnd-copilot

D&D Conversational Adventure Game - A turn-based Dungeons & Dragons inspired game with API and Web Frontend.

## 🎮 Features

### Backend (ASP.NET Core API)
- **Authentication System**: JWT-based authentication with login/register
- **Character Management**: Create and manage D&D characters with stats
- **Dice System**: Support for dice notation (e.g., 2d6, 1d20+5, 3d8-2)
- **Turn-Based Combat**: Combat system with NPCs that have AI behaviors
- **NPC AI States**: Aggressive, Defensive, and Flee behaviors
- **Inventory System**: Stackable and equippable items
- **Loot Tables**: Random item drops from defeated enemies
- **Quest System**: Multi-stage quests with world state persistence
- **Database**: SQLite with Entity Framework Core

### Frontend (React + TypeScript)
- **User Authentication**: Login and registration screens
- **Character Dashboard**: View and manage your characters
- **Character Creation**: Interactive character creation with class selection
- **Dice Roller**: Visual dice rolling tool with quick roll buttons
- **Responsive Design**: Modern, gradient-based UI

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- Node.js 20.x or higher
- npm 10.x or higher

### Backend Setup

1. Navigate to the API directory:
```bash
cd src/DndCopilot.Api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the API:
```bash
dotnet run
```

The API will start on `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS).

### Frontend Setup

1. Navigate to the client directory:
```bash
cd client
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm start
```

The frontend will start on `http://localhost:3000` and automatically open in your browser.

## 🧪 Testing

Run the unit tests:
```bash
dotnet test
```

## 📚 API Documentation

Once the API is running, you can access the Swagger documentation at:
- `http://localhost:5000/swagger`

### Main Endpoints

#### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

#### Characters
- `GET /api/characters` - Get all characters for authenticated user
- `GET /api/characters/{id}` - Get specific character
- `POST /api/characters` - Create new character

#### Dice
- `POST /api/dice/roll` - Roll dice with notation (e.g., "2d6", "1d20+5")

#### Combat
- `POST /api/combat/attack` - Execute combat turn

## 🏗️ Project Structure

```
dnd-copilot/
├── src/
│   ├── DndCopilot.Api/          # Web API project
│   │   ├── Controllers/         # API controllers
│   │   ├── Models/             # Request/Response DTOs
│   │   └── Services/           # API services (Auth)
│   ├── DndCopilot.Core/         # Domain layer
│   │   ├── Entities/           # Domain entities
│   │   ├── Enums/              # Enumerations
│   │   ├── Interfaces/         # Repository interfaces
│   │   └── Services/           # Business logic (Combat, Dice)
│   └── DndCopilot.Infrastructure/ # Data layer
│       ├── Data/               # DbContext
│       └── Repositories/       # Repository implementations
├── tests/
│   └── DndCopilot.Tests/       # Unit tests
├── client/                      # React frontend
│   ├── src/
│   │   ├── components/         # React components
│   │   ├── pages/              # Page components
│   │   ├── services/           # API service layer
│   │   └── types/              # TypeScript types
│   └── public/
└── README.md
```

## 🎲 Character Classes

- **Warrior** (⚔️): Strong melee fighter
- **Mage** (🔮): Powerful spellcaster
- **Rogue** (🗡️): Stealthy and agile
- **Cleric** (✨): Healer and support
- **Ranger** (🏹): Skilled archer

## 🔒 Security

- JWT token-based authentication
- Password hashing with SHA256
- Authorization on protected endpoints
- CORS configured for frontend integration

## 📝 Configuration

### Backend Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=dndcopilot.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKeyForDevelopmentOnlyChangeInProduction123!",
    "Issuer": "DndCopilotApi",
    "Audience": "DndCopilotClient",
    "ExpirationHours": "24"
  }
}
```

### Frontend Configuration (client/.env)
```
REACT_APP_API_URL=http://localhost:5000/api
```

## 🎯 Future Enhancements

- [ ] Real-time combat encounters
- [ ] Quest progression tracking
- [ ] Inventory management UI
- [ ] Multiplayer support
- [ ] World map and locations
- [ ] Character equipment system
- [ ] Achievement system
- [ ] Character leveling and progression

## 📄 License

This project is part of the GitHub Copilot adoption course.

## 👥 Contributors

Created as part of Challenge 2 of the GitHub Copilot Adoption Course.
