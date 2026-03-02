# Sistema de Gestión de Acciones - Resumen de Implementación

## 📋 Problema Original
Muchas acciones no estaban definidas y fallaban. Las acciones estaban hardcodeadas en el `GameService` con un montón de rutas y lógica compleja, dificultando el mantenimiento y la extensión.

## ✅ Solución Implementada

Se creó un **sistema especializado de gestión de acciones** similar al que ya existía para NPCs, permitiendo definir todas las acciones en un archivo de configuración markdown.

---

## 📁 Archivos Creados

### 1️⃣ **docs/ACTIONS.md** - Configuración Central de Acciones
- Define todas las acciones disponibles por **localización** o **contexto**
- Formato similar a `docs/NPCS.md`
- Estructura: 
  ```markdown
  ## Location: [Nombre]
  - **Description**: [Descripción de la localización]
  - **NPCs**: [NPCs presentes]
  - **Actions**:
    - **keyword**: [Descripción de la acción]
  ```
- Acciones globales aplicables en cualquier localización
- Fácil de editar sin tocar código

### 2️⃣ **src/DndCopilot.Core/Interfaces/IActionService.cs** - Definiciones
Nuevas clases e interfaces:
- **`ActionProfile`** - Perfil de una acción (nombre, descripción, tipo, etc.)
- **`ActionType` enum** - Clasificación de acciones:
  - Exploration (exploración)
  - Combat (combate)
  - Social (social)
  - Quest (misiones)
  - Inventory (inventario)
  - Travel (viaje)
  - Utility (utilidad)
  - Magic (magia)
- **`IActionRegistry`** - Interfaz para acceder al registro de acciones
- **`IActionService`** - Interfaz para el servicio de acciones

### 3️⃣ **src/DndCopilot.Core/Services/ActionProfileRegistry.cs** - Carga de Configuración
- Lee acciones desde `docs/ACTIONS.md`
- Parsea markdown con **regex** (igual que `NpcProfileRegistry`)
- Lookups rápidos por tipo
- Método `LoadFromFile()` para carga al iniciar
- Maneja:
  - Ubicaciones
  - Descripción de ubicaciones
  - NPCs y enemigos
  - Acciones por ubicación
  - Acciones globales

### 4️⃣ **src/DndCopilot.Core/Services/ActionService.cs** - Lógica de Servicio
Implementa `IActionService`:
- **`IsValidAction()`** - Valida si una acción existe en una ubicación
- **`GetAvailableActions()`** - Lista acciones disponibles (con soporte global)
- **`GetActionForInput()`** - Resuelve la acción desde entrada del usuario (matching flexible)
- **`GetActionsDescription()`** - Descripción formateada

### 5️⃣ **src/DndCopilot.Core/Services/GameService.cs** - Refactorización
Cambios principales:
- ✅ Inyecta `IActionService` en constructor
- ✅ `ProcessActionAsync()` completamente refactorizado para usar ActionService
- ✅ Elimina **55+ líneas de rutas hardcodeadas**
- ✅ Nuevo sistema de handlers por tipo de acción:
  - `HandleExplorationActionAsync()`
  - `HandleTravelAction()`
  - `HandleQuestAction()`
  - `HandleUtilityAction()`
  - `HandleMagicAction()`
- ✅ `GetLocationActions()` ahora usa ActionService

### 6️⃣ **src/DndCopilot.Api/Program.cs** - Registración DI
Agreguó:
```csharp
// Load Action profiles from docs/ACTIONS.md
var actionRegistry = ActionProfileRegistry.LoadFromFile(actionsDocPath);
builder.Services.AddSingleton<IActionRegistry>(actionRegistry);
builder.Services.AddScoped<IActionService, ActionService>();
```

---

## 🎯 Beneficios

| Aspecto | Mejora |
|--------|--------|
| **Mantenibilidad** | Acciones definidas en archivo, no en código |
| **Extensibilidad** | Agregar acciones sin recompilar |
| **Consistencia** | Same pattern as NPC system |
| **Reducción de código** | Eliminadas líneas de routeo hardcodeado |
| **Flexibilidad** | Matching inteligente de palabras clave |
| **Organización** | Acciones agrupadas por tipo |

---

## 🔄 Flujo de Ejecución

```
Usuario escribe acción
        ↓
ProcessActionAsync()
        ↓
ActionService.GetActionForInput(location, input)
        ↓
Encuentra ActionProfile
        ↓
Enruta por ActionType
        ↓
Handler específico (Exploration, Social, Quest, etc)
        ↓
Retorna (response, availableActions)
```

---

## 📖 Ejemplo de Configuración

En `docs/ACTIONS.md`:
```markdown
## Location: The Rusty Dragon Tavern

- **Description**: Una taberna cálida y acogedora llena de aventureros
- **NPCs**: Grundy, Elara, Theron
- **Actions**:
  - **look**: Observa a los clientes y el ambiente
  - **talk**: Habla con los NPCs presentes
  - **quest board**: Revisa las misiones disponibles
  - **back**: Regresa a la Plaza de la Aldea
```

---

## ✨ Próximos Pasos Posibles

- Agregar más acciones en `docs/ACTIONS.md` por ubicación
- Nuevas ubicaciones con sus propias acciones
- Sistema de consecuencias (acciones que cambian estado del juego)
- Validación en tiempo de ejecución de acciones no configuradas
- Logging de acciones ejecutadas

---

## 🛠️ Compilación Exitosa ✓

```
DndCopilot.Core net9.0 ✓
DndCopilot.Infrastructure net9.0 ✓
DndCopilot.Tests net9.0 ✓
DndCopilot.Api net9.0 ✓
```

**Status**: Compilación correcta sin errores críticos
