# Análisis Técnico - D&D Copilot

## 🏗️ Arquitectura General

D&D Copilot sigue una **arquitectura de capas (Layered Architecture)** con separación clara entre capas de presentación, dominio e infraestructura.

```
┌─────────────────────────────────────────────────────────┐
│              Frontend (React + TypeScript)              │
│        http://localhost:3000                            │
└──────────────────────────┬──────────────────────────────┘
                           │
                    HTTP REST + JWT
                           │
┌──────────────────────────▼──────────────────────────────┐
│           API Layer (ASP.NET Core)                      │
│    Controllers → Models/DTOs → Services                 │
│    http://localhost:5000                                │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│         Domain Layer (Core Services)                    │
│    CombatService, DiceRoller, NpcAgentService          │
│    GameService, AuthService                             │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│    Infrastructure Layer (Data + EF Core)               │
│    DbContext, Repositories, Database                   │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│              SQLite / SQL Server                        │
│         (Local / Azure SQL Database)                    │
└─────────────────────────────────────────────────────────┘
```

## 🗂️ Estructura de Directorios

```
src/
├── DndCopilot.Api/
│   ├── Program.cs                    # Bootstrap, DI, configuración
│   ├── appsettings.json              # Configuración (BD, JWT, CORS)
│   ├── appsettings.Development.json  # Configuración desarrollo
│   ├── DndCopilot.Api.csproj         # Referencia a Core e Infrastructure
│   ├── DndCopilot.Api.http           # Ejemplos HTTP para testing
│   ├── Controllers/
│   │   ├── AuthController.cs         # Endpoints de autenticación
│   │   ├── CharactersController.cs   # CRUD de personajes
│   │   ├── CombatController.cs       # Lógica de combate
│   │   ├── DiceController.cs         # Rodador de dados
│   │   ├── GameController.cs         # Lógica general del juego
│   │   └── NpcAgentController.cs     # Interacción con NPCs
│   ├── Models/
│   │   ├── AuthModels.cs             # LoginRequest, RegisterRequest, TokenResponse
│   │   ├── CharacterModels.cs        # CreateCharacterRequest, CharacterResponse
│   │   ├── CombatModels.cs           # AttackRequest, CombatTurnResponse
│   │   ├── DiceModels.cs             # DiceRollRequest, DiceRollResponse
│   │   ├── GameModels.cs             # GameStateResponse
│   │   └── NpcAgentModels.cs         # NpcInteractionRequest
│   └── Services/
│       └── AuthService.cs            # JWT generation, Password hashing
│
├── DndCopilot.Core/                  # Capa de dominio (sin dependencias de Framework)
│   ├── DndCopilot.Core.csproj
│   ├── Entities/
│   │   ├── Character.cs              # Agregado raíz (personaje con stats)
│   │   ├── CharacterQuest.cs         # Quest del personaje
│   │   ├── CombatEncounter.cs        # Encuentro de combate
│   │   ├── GameSession.cs            # Sesión de juego
│   │   ├── Npc.cs                    # Definición de NPC
│   │   ├── Quest.cs                  # Quest template
│   │   ├── Item.cs                   # Definición de item
│   │   ├── InventoryItem.cs          # Item en inventario del jugador
│   │   ├── LootTable.cs              # Tabla de loot drops
│   │   ├── Location.cs               # Localización del mapa
│   │   └── ... (otras entidades)
│   ├── Enums/
│   │   ├── CharacterClass.cs         # Warrior, Mage, Rogue, Cleric, Ranger
│   │   ├── ItemType.cs               # Consumible, Weapon, Armor
│   │   ├── NpcBehavior.cs            # Aggressive, Defensive, Flee
│   │   ├── ItemRarity.cs             # Common, Uncommon, Rare, Legendary
│   │   └── QuestStatus.cs            # Pending, InProgress, Completed
│   ├── Interfaces/
│   │   ├── ICharacterRepository.cs
│   │   ├── INpcRepository.cs
│   │   ├── IItemRepository.cs
│   │   ├── IGameSessionRepository.cs
│   │   └── ... (otros repositorios)
│   └── Services/
│       ├── CombatService.cs          # Lógica de combate (turnos, daño, acciones)
│       ├── DiceRoller.cs             # Rodador de dados (XdY+Z)
│       ├── GameService.cs            # Lógica general del juego
│       ├── NpcAgentService.cs        # Comportamientos IA de NPCs
│       └── PromptTemplates.cs        # Plantillas de prompts para IA
│
└── DndCopilot.Infrastructure/        # Capa de datos y servicios externos
    ├── DndCopilot.Infrastructure.csproj  # Referencia a Core
    ├── Data/
    │   └── DndCopilotContext.cs      # DbContext de EF Core
    ├── Repositories/
    │   ├── CharacterRepository.cs
    │   ├── NpcRepository.cs
    │   ├── ItemRepository.cs
    │   ├── GameSessionRepository.cs
    │   ├── BaseRepository.cs          # Repositorio genérico
    │   └── ... (otros repositorios)
    └── Services/
        ├── SeedData.cs               # Seed de datos iniciales
        ├── DatabaseInitializer.cs    # Setup inicial de BD
        └── ... (servicios de infraestructura)

client/
├── package.json                      # Dependencias npm
├── tsconfig.json                     # Configuración TypeScript
├── public/
│   ├── index.html                    # HTML raíz
│   └── manifest.json                 # Metadatos PWA
├── src/
│   ├── index.tsx                     # Punto de entrada React
│   ├── App.tsx                       # Componente raíz
│   ├── App.css                       # Estilos globales
│   ├── components/
│   │   ├── CharacterAvatar.tsx       # Avatar pixelado 16-bits
│   │   ├── CharacterAvatar.test.tsx
│   │   ├── CharacterCard.tsx         # Tarjeta de personaje
│   │   ├── CreateCharacter.tsx       # Form crear personaje
│   │   ├── DiceRoller.tsx            # Componente rodador de dados
│   │   ├── Game.tsx                  # Comp. principal del juego
│   │   └── README.md                 # Docs de componentes
│   ├── pages/
│   │   ├── Dashboard.tsx             # Panel principal (listado personajes)
│   │   └── Login.tsx                 # Página de autenticación
│   ├── services/
│   │   └── api.ts                    # Integración con API (axios)
│   └── types/
│       └── index.ts                  # Tipos TypeScript compartidos

tests/
└── DndCopilot.Tests/
    ├── DndCopilot.Tests.csproj       # Referencia a Core
    ├── DiceRollerTests.cs            # Tests unitarios DiceRoller
    ├── CombatServiceTests.cs         # Tests unitarios CombatService
    ├── NpcAgentServiceTests.cs       # Tests unitarios NpcAgentService
    ├── bin/                          # Output compilado
    └── obj/

infrastructure/
└── terraform/
    ├── main.tf                       # Configuración principal, providers
    ├── variables.tf                  # Variables de input
    ├── resources.tf                  # Definición de recursos Azure
    ├── outputs.tf                    # Outputs post-despliegue
    └── terraform.tfvars.example      # Ejemplo de valores variables

docs/
├── ANALISIS_FUNCIONAL.md             # Este archivo
├── ANALISIS_TECNICO.md               # Análisis técnico
├── AZURE_DEPLOYMENT.md               # Despliegue en Azure
├── LOCAL_EXECUTION.md                # Ejecución local
└── NpcAgentService.md                # Documentación específica de NPCs
```

## 🔌 Tecnologías Stack

### Backend
| Componente | Tecnología | Versión | Propósito |
|-----------|-----------|---------|----------|
| Framework | ASP.NET Core | 9.0 | Web API REST |
| ORM | Entity Framework Core | 9.0 | Acceso a datos |
| Base de Datos | SQLite / SQL Server | Latest | Persistencia |
| Autenticación | JWT | - | Tokens seguros |
| Testing | NUnit | 4.x | Tests unitarios |
| HTML API | Swagger / OpenAPI | 3.0 | Documentación API |

### Frontend
| Componente | Tecnología | Versión | Propósito |
|-----------|-----------|---------|----------|
| Framework | React | 18+ | UI library |
| Lenguaje | TypeScript | 5+ | Tipado en JS |
| HTTP Client | Axios | 1.x | Requests HTTP |
| Routing | React Router | 6+ | Navegación SPA |
| Estilos | CSS3 | - | Styling |
| Testing | Jest + React Testing Library | - | Tests unitarios |

### Infraestructura
| Componente | Tecnología | Versión | Propósito |
|-----------|-----------|---------|----------|
| IaC | Terraform | 1.0+ | Infrastructure as Code |
| Cloud | Microsoft Azure | - | Hosting cloud |
| CI/CD | GitHub Actions | - | Automatización (opcional) |
| Container | Docker | - | Containerización (opcional) |

## 🗄️ Modelo de Datos

### Entidades Principales

```
┌─────────────────────────────────────────────┐
│          User (Identity)                    │
│  - Id (PK)                                  │
│  - Username (unique)                        │
│  - Email (unique)                           │
│  - PasswordHash                             │
│  - CreatedAt                                │
└─────────┬───────────────────────────────────┘
          │ 1:N
          ▼
┌─────────────────────────────────────────────┐
│        Character                            │
│  - Id (PK)                                  │
│  - UserId (FK)                              │
│  - Name                                     │
│  - Class (Enum)                             │
│  - Level                                    │
│  - Experience                               │
│  - CurrentHp / MaxHp                        │
│  - Attack                                   │
│  - Defense                                  │
│  - LocationsVisited (JSON array)            │
│  - CreatedAt                                │
└─────────┬───────┬──────────┬────────────────┘
          │       │          │
        1:N     1:N        1:N
          │       │          │
    ┌─────▼───┐  │   ┌──────▼──┐
    │Inventory │  │   │GameSession
    │  Item    │  │   │  .cs
    └──────────┘  │   └─────────┘
          ┌───────▼────────┐
          │ CombatEncounter│
          │  .cs           │
          └────────────────┘
```

### Tablas de Base de Datos

#### Users
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY,
    Username VARCHAR(255) UNIQUE NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
```

#### Characters
```sql
CREATE TABLE Characters (
    Id INT PRIMARY KEY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    Name VARCHAR(255) NOT NULL,
    Class INT NOT NULL,  -- Enum (0=Warrior, 1=Mage, 2=Rogue, 3=Cleric, 4=Ranger)
    Level INT DEFAULT 1,
    Experience INT DEFAULT 0,
    CurrentHp INT NOT NULL,
    MaxHp INT NOT NULL,
    Attack INT NOT NULL,
    Defense INT NOT NULL,
    LocationsVisited VARCHAR(MAX),  -- JSON array
    CreatedAt DATETIME DEFAULT GETDATE()
);
```

#### Items
```sql
CREATE TABLE Items (
    Id INT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    Type INT NOT NULL,  -- Enum (0=Consumible, 1=Weapon, 2=Armor)
    Value INT,
    BaseDamage INT,
    BaseArmor INT
);
```

#### InventoryItems
```sql
CREATE TABLE InventoryItems (
    Id INT PRIMARY KEY,
    CharacterId INT FOREIGN KEY REFERENCES Characters(Id),
    ItemId INT FOREIGN KEY REFERENCES Items(Id),
    Quantity INT DEFAULT 1,
    IsEquipped BIT DEFAULT 0
);
```

## 🔐 Seguridad

### Autenticación
- **Protocolo**: JWT (JSON Web Tokens)
- **Algoritmo**: HS256
- **Expiración**: 24 horas (configurable)
- **Claims**: `sub` (usuario ID), `email`, `iat` (issued at)

### Hashing de Contraseñas
- **Actual**: SHA256 (desarrollo)
- **Recomendado**: bcrypt o PBKDF2 (producción)

### CORS
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Validación de Entrada
- DTOs con Data Annotations
- Validación en Controllers
- Excepciones controladas

## 📡 Endpoints API Principal

### Autenticación
```http
POST /api/auth/register
Body: { username, email, password }
Response: { token, userId, username }

POST /api/auth/login
Body: { username, password }
Response: { token, userId, username }
```

### Personajes
```http
GET /api/characters
Headers: Authorization: Bearer <token>
Response: Character[]

GET /api/characters/{id}
Headers: Authorization: Bearer <token>
Response: Character

POST /api/characters
Headers: Authorization: Bearer <token>
Body: { name, class }
Response: Character

PUT /api/characters/{id}
Headers: Authorization: Bearer <token>
Body: { name, class, ... }
Response: Character

DELETE /api/characters/{id}
Headers: Authorization: Bearer <token>
Response: 204 No Content
```

### Dados
```http
POST /api/dice/roll
Headers: Authorization: Bearer <token>
Body: { notation: "2d6+3" }
Response: { rolls: [3, 5], total: 11, notation: "2d6+3" }
```

### Combate
```http
POST /api/combat/attack
Headers: Authorization: Bearer <token>
Body: { characterId, enemyId }
Response: { playerDamage, enemyDamage, playerHp, enemyHp, combatOver }

POST /api/combat/defend
Headers: Authorization: Bearer <token>
Body: { characterId }
Response: { defenseBoost, enemyDamage, ... }

POST /api/combat/useItem
Headers: Authorization: Bearer <token>
Body: { characterId, itemId }
Response: { itemUsed, effect, enemyDamage, ... }

POST /api/combat/flee
Headers: Authorization: Bearer <token>
Body: { characterId, enemyId }
Response: { success, enemyDamage, ... }
```

### NPCs
```http
GET /api/npcs
Response: Npc[]

GET /api/npcs/{id}
Response: Npc

POST /api/npcs/{characterId}/interact
Headers: Authorization: Bearer <token>
Body: { action: "talk" | "trade" | "fight" }
Response: { npcResponse, combatInitiated }
```

## 🧮 Algoritmos Principales

### Cálculo de Daño en Combate
```csharp
public int CalculateDamage(int attackerAttack, int defenderDefense)
{
    int baseDamage = attackerAttack - defenderDefense;
    return Math.Max(1, baseDamage); // Mínimo 1 de daño
}
```

### Rodador de Dados (Parser XdY+Z)
```csharp
/*
Ejemplo: "2d6+3"
- 2 = número de dados
- 6 = caras por dado  
- 3 = modificador
- Resultado: suma de 2 tiradas d6 + 3
*/

// Validaciones
- Dados: 1-100
- Caras: 1-100
- Modificador: -100 a +100
```

### Sistema de Nivel
```csharp
public void LevelUp(Character character)
{
    character.Level++;
    character.MaxHp += 20;
    character.CurrentHp = character.MaxHp; // Restaura HP
    character.Attack += 5;
    character.Defense += 2;
    character.Experience = 0; // Reset XP
}
```

### IA del NPC (Comportamiento)
```csharp
public NpcAction DecideAction(Npc npc, Character player)
{
    if (npc.Behavior == NpcBehavior.Aggressive)
        return AttackWithFullPower();
    
    if (npc.Behavior == NpcBehavior.Defensive)
        return AttackWithReducedDamage();
    
    if (npc.Behavior == NpcBehavior.Flee && npc.CurrentHp < npc.MaxHp * 0.3)
        return FleeAttempt();
    
    return AttackNormal();
}
```

## 🧪 Testing

### Framework de Testing
- **Plataforma**: NUnit
- **Mocking**: Moq
- **Coverage**: Servicios críticos (Combate, Dados)

### Archivos de Test
```
tests/DndCopilot.Tests/
├── DiceRollerTests.cs       # 6+ tests
├── CombatServiceTests.cs    # 4+ tests  
└── NpcAgentServiceTests.cs  # 3+ tests
```

### Ejecutar Tests
```bash
dotnet test                          # Todos los tests
dotnet test --filter="DiceRoller"   # Tests específicos
dotnet test --verbosity=detailed    # Con output detallado
```

## 🚀 Deployment

### Desarrollo Local
- **API**: `dotnet run` desde `src/DndCopilot.Api`
- **Frontend**: `npm start` desde `client/`
- **BD**: SQLite automático (`dndcopilot.db`)

### Staging/Producción (Azure)
- **Web API**: Azure App Service
- **Frontend**: Azure Static Web Apps
- **BD**: Azure SQL Database o Azure Cosmos DB (opcional)
- **Storage**: Azure Blob Storage (bits para assets)
- **CI/CD**: GitHub Actions

### Terraform para Azure
```hcl
# Proveedores
provider "azurerm" {
  version = "~> 3.0"
}

# Recursos principales
- Resource Group
- App Service Plan
- App Service (API)
- Static Web App (Frontend)
- SQL Server + Database
- Key Vault (secretos)
```

## 🔄 Flujo de Datos

### Crear Personaje
```
Frontend (CreateCharacter.tsx)
    ↓ POST /api/characters
API (CharactersController)
    ↓ ValidateInput
Business Logic (CharacterService)
    ↓ Create new Character entity
Infrastructure (CharacterRepository)
    ↓ DbContext.SaveChanges()
Database (Characters table)
    ↓ Return new Character
API → Frontend display
```

### Combate Turno-a-Turno
```
Frontend (Game.tsx) [Acción Jugador]
    ↓ POST /api/combat/attack
API (CombatController)
    ↓ 
CombatService.ExecutePlayerAction()
    ↓ Calcular daño jugador
    ↓ Actualizar HP enemigo
    ↓ Determinar acción IA enemigo
    ↓ NpcAgentService.DecideAction()
    ↓ Calcular daño enemigo
    ↓ Actualizar HP jugador
    ↓ Verificar victoria/derrota
    ↓
API → CombatResult (daños, HP, turno)
    ↓
Frontend renderiza nueva UI
```

## 📊 Performance

### Optimizaciones Implementadas
- Índices en BD en FK (CharacterId, UserId)
- Query optimization en EF Core (Select, Include)
- Lazy loading vs Eager loading (configurable)
- Caching de datos estáticos (Items, NPCs)

### Métricas Esperadas
| Métrica | Target |
|---------|--------|
| Latencia Login | < 200ms |
| Latencia Combate | < 100ms |
| Latencia Dice Roll | < 50ms |
| FCP (First Contentful Paint) | < 2s |
| TTI (Time to Interactive) | < 3s |

## 🔧 Configuración

### appsettings.json (Backend)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=dndcopilot.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKey123",
    "Issuer": "DndCopilotApi",
    "Audience": "DndCopilotClient",
    "ExpirationHours": "24"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### .env (Frontend)
```
REACT_APP_API_URL=http://localhost:5000/api
REACT_APP_JWT_KEY=auth_token
```

## 🌐 Integración Frontend-Backend

### Flujo HTTP

1. **Request**: Frontend → API
   - Headers: `Authorization: Bearer <token>`
   - Body: JSON serializado
   
2. **Response**: API → Frontend
   - Status Code: 200, 400, 401, 500
   - Body: JSON con datos o error

### Manejo de Errores
```typescript
// Frontend (api.ts)
try {
    const response = await axios.post(endpoint, data);
    return response.data;
} catch (error) {
    if (error.response?.status === 401) {
        // Redirect to login
    } else if (error.response?.status === 400) {
        // Mostrar error validación
    }
}
```

## 🎯 Patrones de Código

### Inyección de Dependencias (DI)
```csharp
// Program.cs
builder.Services.AddScoped<ICombatService, CombatService>();
builder.Services.AddScoped<IDiceRoller, DiceRoller>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
```

### Repository Pattern
```csharp
public interface ICharacterRepository
{
    Task<Character> GetByIdAsync(int id);
    Task<IEnumerable<Character>> GetAllAsync();
    Task AddAsync(Character character);
    Task UpdateAsync(Character character);
    Task DeleteAsync(int id);
}
```

### Async/Await
```csharp
public async Task<CharacterResponse> CreateCharacterAsync(CreateCharacterRequest request, int userId)
{
    var character = new Character 
    { 
        UserId = userId, 
        Name = request.Name, 
        Class = request.Class 
    };
    
    await _characterRepository.AddAsync(character);
    return _mapper.Map<CharacterResponse>(character);
}
```

---

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Mantenedor**: D&D Copilot Team
