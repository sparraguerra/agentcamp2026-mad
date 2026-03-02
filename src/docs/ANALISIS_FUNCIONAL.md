# Análisis Funcional - D&D Copilot

## 📋 Descripción General

**D&D Copilot** es un juego de aventura conversacional basado en Dungeons & Dragons (D&D) que permite a los usuarios crear personajes, explorar localizaciones, participar en combates por turnos contra enemigos IA y enfrentarse a un boss final. El juego combina mecánicas clásicas de rol con una interfaz moderna web.

## 🎮 Funcionalidades Principales

### 1. Sistema de Autenticación

#### Registración
- **Descripción**: Permite a nuevos usuarios crear cuentas
- **Entrada**: Username, Email, Password
- **Validaciones**:
  - Username único
  - Email válido
  - Password mínimo 6 caracteres
- **Salida**: Token JWT para sesión

#### Login
- **Descripción**: Permite a usuarios existentes iniciar sesión
- **Entrada**: Username/Email, Password
- **Salida**: Token JWT + Datos de usuario

### 2. Gestión de Personajes

#### Crear Personaje
- **Input**:
  - Nombre del personaje
  - Clase (Warrior, Mage, Rogue, Cleric, Ranger)
  - Usuario asociado
- **Estadísticas Iniciales**:
  - Salud: 100 HP
  - Ataque: 15
  - Defensa: 5
  - Nivel: 1
  - Experiencia: 0 XP
- **Inventario Inicial**:
  - 2x Poción de Salud (+40 HP)
  - 1x Poción de Fuerza (+10 ATK permanente)

#### Listar Personajes
- Mostrar todos los personajes del usuario autenticado
- Incluir estadísticas actuales y progreso

#### Ver Detalles del Personaje
- Estadísticas completas
- Inventario
- Historial de combates
- Progreso en el juego

### 3. Sistema de Juego (10 Localizaciones)

#### Estructura Básica
El juego se divide en **10 localizaciones** secuenciales que deben completarse en orden:

```
Localizaciones Regulares (1-9):
1. Bosque Oscuro
2. Caverna de Cristal
3. Ruinas Ancestrales
4. Pantano Venenoso
5. Montaña Helada
6. Templo Abandonado
7. Desierto de Fuego
8. Fortaleza en Ruinas
9. Mazmorra Profunda

Localización Final (10):
10. Torre de la IA 🔒 (Bloqueada hasta completar 1-9)
```

#### Mecánicas por Localización
- **Descripción Inmersiva**: Cada localización tiene una narrativa única
- **Enemigos Aleatorios**: 0-2 enemigos por localización seleccionados al azar
- **Items Encontrados**: 33% poción salud, 20% poción fuerza, 47% nada
- **Progreso**: Contador de localizaciones visitadas (X/10)

#### Tipos de Enemigos
- Goblin, Orco, Esqueleto, Araña Gigante, Lobo
- Bandido, Zombie, Serpiente Venenosa, Murciélago Vampiro, Troll

**Estadísticas Enemigas**:
- Salud: 20-50 HP
- Ataque: 5-15
- Defensa: 0-5

### 4. Sistema de Combate (Basado en Turnos)

#### Flujo de Combate
1. Se inicia combate al encontrar enemigo
2. Muestra: Salud del personaje, Salud del enemigo, Turno actual
3. Jugador elige acción
4. Se ejecuta acción del jugador
5. Se ejecuta turno del enemigo (si sigue vivo)
6. Repite hasta: Victoria, Derrota o Huida exitosa

#### Acciones Disponibles

**1. Atacar** 🗡️
```
Daño = Tu_Ataque - Defensa_Enemigo
Ejemplo: 15 - 5 = 10 daño
```
- El enemigo contraataca después (automático)

**2. Defender** 🛡️
```
Tu_Defensa += 5 (para el próximo ataque enemigo)
Duración: 1 turno solamente
```
- No infliges daño este turno
- Útil para recuperar salud cuando está baja

**3. Usar Objeto** 💊
```
Tipos:
- Poción de Salud: +40 HP
- Poción de Fuerza: +10 ATK permanente
```
- El enemigo contraataca después
- Si inventario vacío: no disponible

**4. Huir** 🏃
```
Probabilidad de éxito: 50%
```
- Escapa del combate sin recibir daño final
- NO disponible contra Alberto Díaz (boss final)
- Si fallas: enemigo ataca como turno normal

#### Comportamientos de NPCs
- **Agresivo**: Siempre ataca a máxima potencia
- **Defensivo**: Ataca pero inflige 50% de daño
- **Huida**: Intenta escapar cuando HP < 30%

### 5. Sistema de Dados 🎲

#### Notación D&D
Soporta notación estándar de dados D&D:
```
XdY         = X dados de Y caras (ej: 2d6)
XdY+Z       = Con modificador positivo (ej: 1d20+5)
XdY-Z       = Con modificador negativo (ej: 3d8-2)
```

#### Validaciones
- Número de dados: 1-100
- Caras de dado: 1-100
- Modificador: -100 a +100

#### Casos de Uso
- Ataques en combate
- Daño de habilidades
- Tiradas de suerte/evento

### 6. Sistema de Progresión

#### Experiencia y Niveles
```
Ganancias de XP:
- Enemigos normales: 20-40 XP
- Boss final (Alberto Díaz): 100 XP

Subida de Nivel:
- Cada 100 XP ganas 1 nivel
- Se notifica al jugador
- Salud se restaura al máximo
```

#### Bonificadores al Subir Nivel
| Estadística | Incremento |
|-------------|-----------|
| Salud Máxima | +20 HP |
| Ataque | +5 ATK |
| Defensa | +2 DEF |

### 7. Sistema de Inventario

#### Tipos de Items
1. **Pociones de Salud**
   - Efecto: +40 HP
   - Apilables: Sí
   - Cantidad inicial: 2

2. **Pociones de Fuerza**
   - Efecto: +10 ATK permanente
   - Apilables: Sí
   - Cantidad inicial: 1

3. **Items Futuros** (Extensibles)
   - Armas (no apilables)
   - Armaduras (no apilables)
   - Consumibles diversos

#### Gestión de Inventario
- Ver items disponibles
- Usar item en combate
- Descartar items
- Límite: 100 items por personaje (configurable)

### 8. Boss Final: Alberto Díaz 👑

#### Localización
- **Torre de la IA** (Localización 10)
- Solo accesible después de completar las 9 localizaciones anteriores

#### Estadísticas
```
Salud:     200 HP (2x más que enemigo normal)
Ataque:    30 (2x más)
Defensa:   10 (2x más)
```

#### Habilidad Especial: Poder de la IA
```
Cada ataque de Alberto incluye un bonus aleatorio de 0-10 daño
Ejemplo: 30 (ataque base) + 5 (bonus IA) = 35 daño
Mensajes especiales durante batalla
```

#### Condiciones Especiales
- ❌ No se puede huir
- No spawn de items
- Gran cantidad de XP (100 XP)
- Determina victoria final del juego

### 9. Interfaz de Usuario

#### Login/Registro
- Pantalla dedicada para autenticación
- Validación de credenciales
- Redirección post-login

#### Dashboard
- Listado de personajes del usuario
- Botón para crear nuevo personaje
- Selector de clase con preview de avatar pixelado
- Herramienta rodador de dados

#### Pantalla de Juego
- Descripción de localización actual (X/10)
- Status del personaje (HP, ATK, DEF, Nivel)
- Interfaz de combate (si hay enemigo)
- Botones de acciones
- Chat/Narrativa conversacional

#### Avatar Pixelado 16-bits
- **Warrior** (Rojo): Armadura y espada
- **Mage** (Azul): Túnica mágica con vara
- **Rogue** (Púrpura): Capa con daga
- **Cleric** (Dorado): Robes sagradas con cruz
- **Ranger** (Verde): Capa de naturaleza con arco

### 10. Persistencia de Datos

#### Datos Guardados
- Información del usuario
- Personajes creados
- Estadísticas de combate
- Inventario actual
- Progreso en localizaciones (visitadas = true/false)
- Nivel y experiencia
- Score/Ranking (opcional)

#### Base de Datos
- SQLite (desarrollo)
- SQL Server/PostgreSQL (producción en Azure)

## 🎯 Flujos de Negocio

### Flujo 1: Nueva Partida Completa

```mermaid
1. Registro/Login
   ↓
2. Crear Personaje (clase)
   ↓
3. Comenzar en Bosque Oscuro
   ↓
4. Exploración de 9 Localizaciones:
   - Encontrar 0-2 enemigos
   - Sistema combate por turnos
   - Ganar XP y items
   - Cambiar de localización
   ↓
5. Torre de la IA Desbloqueada
   ↓
6. Enfrentar Boss: Alberto Díaz
   ↓
7. Victoria/Derrota
```

### Flujo 2: Combate Individual

```
Inicio Combate
   ↓
Mostrar Status
   ↓
Esperar Acción Jugador
   ├─ Atacar      → Calcular daño → Daño enemigo
   ├─ Defender    → +5 DEF → Próximo turno enemigo
   ├─ Usar Objeto → Consumir ítem → Daño enemigo
   └─ Huir        → 50% éxito → Escapar o recibir daño
   ↓
Sistema IA Enemigo actúa
   ├─ Agresivo: Ataque normal
   ├─ Defensivo: Ataque al 50%
   └─ Huida: Intenta escapar si HP bajo
   ↓
¿Alguien muere?
   ├─ Jugador muere (0 HP)  → Game Over
   ├─ Enemigo muere         → Victoria, XP, Items
   ├─ Escape éxito          → Fin combate
   └─ Cursor para volver al combate
```

## 📊 Casos de Uso Avanzados

### Caso 1: Subida de Nivel Medio Juego
- Jugador 45 XP restantes, gana 80 XP
- Total: 45 + 80 = 125 XP
- Sube a nivel 2
- Se restaura salud máxima
- Se incrementan estadísticas
- Continúa combate si está activo

### Caso 2: Huida Fallida en Combate
- Jugador elige "Huir"
- 50% falla
- Enemigo ejecuta turno de venganza
- Si jugador muere → Game Over
- Si sobrevive → Vuelve a intentar

### Caso 3: Items Encontrados
- Localización random genera item
- 33% → Poción Salud
- 20% → Poción Fuerza
- 47% → Nada
- Se añade al inventario

## 🔄 Integraciones Externas

### Futuras Integraciones
- **IA Conversacional**: Descripciones generadas por IA
- **Multiplayer**: Combat en tiempo real
- **Webhooks**: Notificaciones de logros
- **Streaming**: OBS integration para content creators

## 📱 Canales de Acceso

- **Web**: http://localhost:3000 (desarrollo)
- **API REST**: http://localhost:5000/api
- **Swagger**: http://localhost:5000/swagger
- **Base de Datos**: SQLite (local) o Azure SQL Server (producción)

## 🎁 Futuras Mejoras

- [ ] Sistema de Quests multi-etapa
- [ ] Equipo de personajes (armas, armadura)
- [ ] Comercio entre jugadores
- [ ] Logros y estatísticas
- [ ] Modo cooperativo
- [ ] Más clases de personajes (Paladin, Druid, Bard)
- [ ] Sistema de magia/habilidades especiales
- [ ] Mapa del mundo explorable
- [ ] Sistema de guilds/clanes
- [ ] Leaderboard de jugadores

## 📈 Métricas de Éxito

| Métrica | Objetivo |
|---------|----------|
| Tiempo promedio de partida | 15-30 minutos |
| Tasa de completude | >60% jugadores finalizan |
| Combates promedio por juego | 5-8 combates |
| Items usados por partida | 2-4 items |
| Niveles alcanzados | 4-7 niveles |

---

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Autor**: D&D Copilot Team
