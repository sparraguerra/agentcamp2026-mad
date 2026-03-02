# 🎮 Simulación de Flujo Completo de un Jugador - D&D Copilot

**Fecha:** 9 de febrero de 2026  
**Propósito:** Documentar el flujo completo de un jugador típico en el sistema D&D Copilot

---

## **FASE 1: Registro y Autenticación** 🔐

### **Paso 1.1 - Registro Inicial**
- El jugador ingresa a `http://localhost:3000`
- En la pantalla de [Login.tsx](../client/src/pages/Login.tsx), hace clic en "Register"
- Completa el formulario:
  - Username: `AragonElGrande`
  - Email: `aragon@gondor.com`
  - Password: `******`
- El sistema llama a [`POST /api/auth/register`](../src/DndCopilot.Api/Controllers/AuthController.cs#L18-L56)
- Se crea el usuario en la BD con hash SHA256 de la contraseña
- Recibe un JWT token automáticamente
- Es redirigido al Dashboard

### **Paso 1.2 - Login (visitas posteriores)**
- Ingresa credenciales en [`POST /api/auth/login`](../src/DndCopilot.Api/Controllers/AuthController.cs#L58-L70)
- El servidor verifica el hash de la contraseña
- Recibe JWT token válido por 24 horas

---

## **FASE 2: Creación de Personaje** ⚔️

### **Paso 2.1 - Dashboard Inicial**
- El jugador ve el [Dashboard](../client/src/pages/Dashboard.tsx) vacío
- Aparece mensaje: "No characters yet. Create your first character to start your adventure!"
- Hace clic en "+ Create Character"

### **Paso 2.2 - Configuración del Personaje**
- Se abre modal con [CreateCharacter.tsx](../client/src/components/CreateCharacter.tsx)
- Ingresa nombre: `Aragorn`
- Selecciona clase: `Warrior ⚔️`
- Hace clic en "Create Character"
- Llama a [`POST /api/characters`](../src/DndCopilot.Api/Controllers/CharactersController.cs#L84-L133)
- El sistema crea el [Character](../src/DndCopilot.Core/Entities/Character.cs#L5-L27) con stats iniciales:
  - **HP**: 100/100
  - **Nivel**: 1
  - **STR, DEX, CON, INT, WIS, CHA**: 10 cada uno
  - **Gold**: 100

---

## **FASE 3: Inicio del Juego** 🎲

### **Paso 3.1 - Selección de Personaje**
- En el Dashboard, ve su [CharacterCard](../client/src/components/CharacterCard.tsx) con stats
- Hace clic en el botón "PLAY"
- Se activa el componente [Game.tsx](../client/src/components/Game.tsx)

### **Paso 3.2 - Inicio de Sesión**
- Llama a [`POST /api/game/start`](../src/DndCopilot.Api/Controllers/GameController.cs#L39-L84)
- Se crea una GameSession en la BD:
  - `CharacterId`: ID del personaje
  - `IsActive`: true
  - `CurrentLocation`: "Village Square"
  - `ConversationHistory`: []
- El [GameService](../src/DndCopilot.Core/Services/GameService.cs#L15-L32) genera mensaje de bienvenida:

```
🎮 GAME START 🎮

Welcome, Aragorn the Warrior!

You stand at the entrance of the Village Square. 
The sun is setting, casting long shadows across 
the cobblestone streets...

⚔️ HP: 100/100
💰 Gold: 100
⭐ Level: 1

What would you like to do?
```

### **Paso 3.3 - Acciones Disponibles**
- Ve botones de Quick Actions:
  - Look around
  - Go to tavern
  - Visit blacksmith
  - Check inventory
  - Rest

---

## **FASE 4: Exploración del Mundo** 🗺️

### **Paso 4.1 - Investigar el Entorno**
- Usuario escribe: `look around` o hace clic en el botón
- Llama a [`POST /api/game/action`](../src/DndCopilot.Api/Controllers/GameController.cs#L86-L144)
- [GameService.ProcessAction](../src/DndCopilot.Core/Services/GameService.cs#L34-L75) detecta keyword "look"
- Respuesta del Game Master:

```
🏛️ VILLAGE SQUARE

You see:
- The Rusty Dragon Tavern to the north 
  (sounds of laughter inside)
- A blacksmith's forge to the east 
  (smoke rising from the chimney)
- A general store to the west 
  (merchant calling out deals)
- The town hall to the south 
  (guards standing at attention)
- A fountain in the center with clear, cool water
```

### **Paso 4.2 - Visitar la Taberna**
- Escribe: `go to tavern`
- La sesión actualiza `CurrentLocation = "The Rusty Dragon Tavern"`
- GenerateTavernScene genera:

```
🍺 You enter The Rusty Dragon Tavern...

The warmth and noise of the tavern envelop you. 
Grundy the bartender nods in your direction.

'Welcome, traveler! What'll it be?'

You notice:
- A bard playing in the corner
- Several adventurers at a table discussing rumors
- A mysterious hooded figure in the shadows
- A quest board with various notices
```

- Nuevas acciones disponibles:
  - Talk to bartender
  - Listen to bard
  - Check quest board
  - Order drink
  - Leave tavern

---

## **FASE 5: Sistema de Dados** 🎲

### **Paso 5.1 - Lanzar Dados para Convencer al Bartender**
- Escribe: `roll 1d20+5` (Charisma check)
- GameService.RollDice procesa
- Llama al [DiceRoller](../src/DndCopilot.Core/Services/DiceRoller.cs#L14-L45)
- Sistema calcula:
  - Tira 1d20: resultado = 17
  - Aplica modificador: +5
  - Total: 22
- Respuesta:

```
🎲 DICE ROLL

Rolled 1d20+5: [17] = 22

The dice have spoken!
```

### **Paso 5.2 - Uso del Dice Roller del Dashboard**
- En el [DiceRoller](../client/src/components/DiceRoller.tsx) del sidebar
- Puede usar botones rápidos: d4, d6, d8, d10, d12, d20, d100
- O escribir notación custom: `3d6+2`, `2d8-1`
- Llama a [`POST /api/dice/roll`](../src/DndCopilot.Api/Controllers/DiceController.cs#L20-L39)

---

## **FASE 6: Combate** ⚔️

### **Paso 6.1 - Encontrar Enemigo**
- Escribe: `fight` o `initiate combat`
- InitiateCombat genera encuentro:

```
⚔️ COMBAT INITIATED!

A goblin jumps out from the shadows!

🧌 Goblin Scout
HP: 25/25
ATK: 6

Prepare for battle!
```

- Acciones de combate disponibles:
  - Attack
  - Defend
  - Use item
  - Flee

### **Paso 6.2 - Atacar al Goblin**
- Hace clic en "Attack"
- Sistema llama a [`POST /api/combat/attack`](../src/DndCopilot.Api/Controllers/CombatController.cs#L30-L61)
- [CombatService.ExecuteTurn](../src/DndCopilot.Core/Services/CombatService.cs#L15-L25) calcula:
  
**Turno del Jugador:**
- Attack Roll: 1d20 + STR modifier (Warrior)
  - Roll: 15 + 0 = 15
- Damage Roll: 1d10 + STR modifier
  - Roll: 8 + 0 = 8
- Daño real: 8 - Defense del Goblin (2) = 6
- Goblin HP: 25 - 6 = **19 HP restantes**

**Turno del Goblin:**
- NpcAttack determina acción según NpcBehavior
- Si es Aggressive: Ataca
- Attack Roll: 1d20
- Damage Roll: 1d6
- Resultado: 4 de daño
- Aragorn HP: 100 - 4 = **96 HP**

### **Paso 6.3 - Victoria**
- Después de varios turnos, Goblin HP llega a 0
- Aragorn gana experiencia (20-40 XP)
- Posible loot drop según LootTable

---

## **FASE 7: Sistema de Inventario** 🎒

### **Paso 7.1 - Ver Inventario**
- Escribe: `inventory` o `check items`
- ShowInventory muestra:

```
🎒 INVENTORY

💰 Gold: 120

Items:
- Health Potion x2
- Iron Sword x1
- Leather Armor x1
```

### **Paso 7.2 - Usar Item**
- En el inventario las InventoryItem son:
  - Stackable (pociones)
  - Equippable (armas, armadura)
- Usa Health Potion: restaura 40 HP
- Equipa Iron Sword: +5 al ataque

---

## **FASE 8: Quests y Progresión** 📜

### **Paso 8.1 - Aceptar Quest**
- En la taberna, interactúa con quest board
- Ve quest disponible con Quest multi-stage
- Sistema crea CharacterQuest

### **Paso 8.2 - Ver Log de Quests**
- Escribe: `quest log`
- ShowQuests muestra:

```
📜 QUEST LOG

Active Quests:
1. The Goblin Menace
   - Clear out the goblin camp (1/5)
   - Reward: 100 gold, Iron Sword
   - Progress: In Progress
```

### **Paso 8.3 - Subir de Nivel**
- Al acumular 100 XP, Character sube de nivel
- Stats aumentados:
  - Nivel: 1 → 2
  - Max HP: 100 → 120
  - STR: 10 → 12
  - Otros stats mejoran

---

## **FASE 9: Interacción con NPCs** 🗣️

### **Paso 9.1 - Hablar con NPC**
- Escribe: `talk to bartender`
- Sistema usa [NpcAgentService](../src/DndCopilot.Core/Services/NpcAgentService.cs) con patrón observe-decide-act

**Observe Phase:**
- `ObserveAsync` analiza:
  - Ubicación del jugador
  - NPCs cercanos
  - Objetos en el área
- Clasifica entidades como threats/opportunities/neutral

**Decide Phase:**
- `DecideAsync` usa:
  - Personalidad del NPC (NpcBehavior)
  - Objetivos del NPC
  - Observaciones previas
- Elige acción y genera reasoning

**Act Phase:**
- `ActAsync` ejecuta:
  - Genera diálogo contextual
  - Publica eventos del juego
  - Actualiza estado del mundo

---

## **FASE 10: Descanso y Gestión** 💤

### **Paso 10.1 - Descansar**
- Escribe: `rest`
- Rest restaura HP completos
- Mensaje de confirmación

### **Paso 10.2 - Visitar Herrero**
- Va al blacksmith
- Ve items disponibles con precios
- Puede comprar/vender equipo
- Sistema actualiza gold y inventory

---

## **FASE 11: Ruta a AI Tower y Boss Final** 🗼

### **Paso 11.1 - Ruta completa hasta la AI Tower**
- Desde `Village Square`, el jugador sigue la ruta canónica:
  1. `go to dark forest`
  2. `go to ancient ruins`
  3. `go to crystal cavern`
  4. `go to poison swamp`
  5. `go to frozen mountain`
  6. `go to abandoned temple`
  7. `go to fire desert`
  8. `go to ruined fortress`
  9. `go to dark dungeon`
  10. `go to ai tower`
- En cada paso, `CurrentLocation` se actualiza y el sistema devuelve nuevas acciones contextuales.

### **Paso 11.2 - Encuentro con Alberto Díaz**
- En `AI Tower` aparece el NPC final: **Alberto Díaz**.
- El jugador puede interactuar con `talk to alberto díaz`.
- Alberto responde con tono malvado canario y presume del control de la torre.

### **Paso 11.3 - Truco para ganar (SCanaryNet)**
- Acción clave: `shut down scanarynet`
- Resultado esperado:
  - La IA de control de la torre queda desactivada.
  - Alberto pierde su ventaja táctica.
  - El sistema muestra explícitamente que es el mejor momento para derrotarlo.

---

## **FASE 12: Finalización de Sesión** 🚪

### **Paso 12.1 - Guardar Progreso**
- Todo se guarda automáticamente en GameSession
- Conversation history serializado en JSON
- Estado del personaje actualizado en BD

### **Paso 12.2 - Salir del Juego**
- Hace clic en "EXIT"
- Llama a [`POST /api/game/end/{sessionId}`](../src/DndCopilot.Api/Controllers/GameController.cs#L182-L204)
- Marca sesión como `IsActive = false`
- `EndedAt = DateTime.UtcNow`
- Regresa al Dashboard

### **Paso 12.3 - Continuar Después**
- En próxima sesión, puede:
  - Jugar con mismo personaje (nueva sesión)
  - Crear nuevo personaje
  - Ver historial de sesiones con `GET /api/game/session/{sessionId}`

---

## **Flujo de Datos Técnico** 📊

### **Frontend → Backend:**
1. React Game.tsx envía acciones
2. api.ts con axios + JWT auth
3. API Controllers validan JWT
4. Servicios procesan lógica de negocio
5. Repositories acceden a SQLite

### **Backend → Frontend:**
1. Respuestas JSON estructuradas
2. Estado del juego actualizado
3. Lista de acciones disponibles
4. UI se re-renderiza con nuevo estado

---

## **Arquitectura de Componentes**

### **Capa de Presentación (Frontend)**
- **Dashboard**: Gestión de personajes
- **Game**: Loop principal de juego
- **CharacterCard**: Visualización de stats
- **DiceRoller**: Sistema de tiradas
- **CreateCharacter**: Creación de personajes

### **Capa de API (Backend)**
- **AuthController**: Autenticación y autorización
- **CharactersController**: CRUD de personajes
- **GameController**: Gestión de sesiones
- **CombatController**: Sistema de combate
- **DiceController**: Lógica de dados
- **NpcAgentController**: Comportamiento de NPCs

### **Capa de Servicios (Core)**
- **GameService**: Lógica principal del juego
- **CombatService**: Mecánicas de combate
- **DiceRoller**: Sistema de dados
- **NpcAgentService**: IA de NPCs con patrón observe-decide-act
- **AuthService**: JWT y hashing

### **Capa de Datos (Infrastructure)**
- **GameDbContext**: Entity Framework Core
- **Repositories**: Acceso a datos
- **SQLite Database**: Persistencia

---

## **Flujos de Datos Clave**

### **1. Flujo de Autenticación**
```
Usuario → Login Form → POST /api/auth/login 
→ AuthService.VerifyPassword 
→ AuthService.GenerateJwtToken 
→ JWT Token → Local Storage → Axios Interceptor
```

### **2. Flujo de Creación de Personaje**
```
CreateCharacter Form → POST /api/characters 
→ CharacterRepository.AddAsync 
→ SQLite INSERT → Character Entity 
→ Response → Dashboard Update
```

### **3. Flujo de Acción de Juego**
```
Game Input → POST /api/game/action 
→ GameService.ProcessAction 
→ Keyword Detection → Location Update 
→ Response Generation → Conversation History 
→ GameSession Update → UI Render
```

### **4. Flujo de Combate**
```
Attack Button → POST /api/combat/attack 
→ CombatService.ExecuteTurn 
→ DiceRoller.Roll → Damage Calculation 
→ HP Update → Check Victory/Defeat 
→ XP Award → Loot Generation → Response
```

### **5. Flujo de NPC Interaction**
```
Talk Command → NpcAgentController.Observe 
→ NpcAgentService.ObserveAsync → AI Classification 
→ NpcAgentController.Decide → Decision AI 
→ NpcAgentController.Act → Dialogue Generation 
→ Event Publishing → Response
```

---

## **Puntos de Integración**

### **Seguridad**
- JWT tokens con expiración de 24h
- SHA256 password hashing
- Authorization middleware en todos los endpoints protegidos
- CORS configurado para frontend

### **Persistencia**
- SQLite con Entity Framework Core
- Automatic migrations
- Seed data para items, NPCs, loot tables, quests
- Conversation history serializado en JSON

### **Estado de Juego**
- GameSession activa por personaje
- Historial de conversaciones
- Current location tracking
- Character stats actualizados en tiempo real

### **Sistema de Eventos**
- Eventos de NPC publicados vía Dapr
- Event types: npc.observed, npc.decided, npc.action.*
- Permite extensibilidad futura

---

## **Posibles Extensiones Futuras**

### **Gameplay**
- Sistema de crafting
- Mapas más complejos con navegación
- Boss battles con mecánicas especiales
- Sistema de aliados/party
- Mazmorras procedurales

### **Social**
- Multiplayer sessions
- Trading entre jugadores
- Guild system
- Leaderboards

### **Técnica**
- Integración con AI Foundry para diálogos dinámicos
- WebSockets para actualizaciones en tiempo real
- Redis para sesiones
- Azure deployment con App Service
- CI/CD con GitHub Actions

---

## **Métricas de Éxito**

### **Tiempo Promedio de Sesión**
- Primera sesión: 15-30 minutos (exploración)
- Sesiones posteriores: 20-45 minutos (quests)

### **Engagement**
- Creación de múltiples personajes
- Completación de quests
- Frecuencia de combates
- Uso del dice roller

### **Retención**
- Sesiones por usuario
- Días activos
- Personajes creados
- Nivel máximo alcanzado

---

Esta simulación representa un flujo completo y realista que abarca todos los sistemas principales del juego: autenticación, creación de personajes, exploración, combate, inventario, quests, NPCs, y progresión del jugador.
