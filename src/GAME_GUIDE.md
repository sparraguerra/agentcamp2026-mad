# Guía del Juego - Aventura D&D

## 🎮 Cómo Jugar

### Inicio Rápido
```bash
dotnet run
```

### Objetivo del Juego
Explora 10 peligrosas localizaciones, derrota enemigos, y finalmente enfrenta a **Alberto Díaz**, el maestro de la IA, en la Torre de la IA.

## 🗺️ Las 10 Localizaciones

### Localizaciones Regulares (1-9)
Debes completar estas 9 localizaciones antes de acceder a la final:

1. **Bosque Oscuro** - Un denso bosque donde apenas penetra la luz del sol
2. **Caverna de Cristal** - Una cueva iluminada por cristales brillantes
3. **Ruinas Ancestrales** - Los restos de una civilización perdida
4. **Pantano Venenoso** - Un pantano lleno de gases tóxicos
5. **Montaña Helada** - Una montaña cubierta de nieve eterna
6. **Templo Abandonado** - Un antiguo templo donde los dioses ya no responden
7. **Desierto de Fuego** - Un desierto árido donde el sol abrasa sin piedad
8. **Fortaleza en Ruinas** - Una fortaleza destruida por el tiempo
9. **Mazmorra Profunda** - Un laberinto subterráneo lleno de trampas

### Localización Final (10)
10. **Torre de la IA** - 🔒 Bloqueada hasta completar las 9 anteriores
    - Aquí te espera Alberto Díaz, el boss final

## ⚔️ Sistema de Combate

Durante cada batalla tienes 4 acciones disponibles:

### 1. Atacar 🗡️
- Inflige daño al enemigo
- Daño = Tu Ataque - Defensa del Enemigo
- El enemigo contraataca después

### 2. Defender 🛡️
- Aumenta tu defensa en +5 para el próximo ataque
- No infliges daño este turno
- Útil cuando tienes poca salud

### 3. Usar Objeto 💊
- **Poción de Salud**: Restaura 40 HP
- **Poción de Fuerza**: Aumenta tu ataque en +10 (permanente)
- El enemigo contraataca después

### 4. Huir 🏃
- 50% de probabilidad de escapar del combate
- ⚠️ NO disponible contra el boss final
- Si fallas, el enemigo te atacará

## 👤 Sistema de Personaje

### Estadísticas Iniciales
- **Salud**: 100/100
- **Ataque**: 15
- **Defensa**: 5
- **Nivel**: 1

### Inventario Inicial
- 2x Poción de Salud
- 1x Poción de Fuerza

### Sistema de Niveles
- Gana experiencia al derrotar enemigos:
  - Enemigos normales: 20-40 XP
  - Boss final: 100 XP
- Sube de nivel cada 100 XP
- Al subir de nivel obtienes:
  - +20 Salud máxima
  - +5 Ataque
  - +2 Defensa
  - Salud restaurada al máximo

## 👹 Enemigos

### Tipos de Enemigos Regulares
- Goblin
- Orco
- Esqueleto
- Araña Gigante
- Lobo
- Bandido
- Zombie
- Serpiente Venenosa
- Murciélago Vampiro
- Troll

### Características
- Cada localización tiene entre 0 y 2 enemigos aleatorios
- Estadísticas variables:
  - Salud: 20-50
  - Ataque: 5-15
  - Defensa: 0-5

### Boss Final: Alberto Díaz 👑
- **Salud**: 200
- **Ataque**: 30 (+ bonus aleatorio de 0-10 por IA)
- **Defensa**: 10
- **Habilidad Especial**: Poder de la IA
  - Sus ataques son impredecibles
  - Cada golpe puede tener un bonus de hasta +10 puntos
- **Ubicación**: Torre de la IA
- **Requisito**: Haber visitado las 9 localizaciones anteriores

## 💡 Consejos Estratégicos

1. **Gestiona tu Salud**: Usa pociones de salud cuando tu HP baje del 50%
2. **Usa la Defensa**: Cuando tengas poca salud, defender puede salvarte
3. **Guarda Pociones**: Reserva algunas pociones para el boss final
4. **Sube de Nivel**: Intenta subir de nivel antes de enfrentarte al boss
5. **No Huyas Innecesariamente**: Pierdes la experiencia del enemigo
6. **Explora Todo**: Algunas localizaciones tienen objetos útiles

## 🏆 Condiciones de Victoria

Para ganar el juego debes:
1. ✅ Completar las 9 localizaciones regulares
2. ✅ Acceder a la Torre de la IA
3. ✅ Derrotar a Alberto Díaz
4. ✅ Sobrevivir con al menos 1 HP

## 💀 Game Over

El juego termina si:
- Tu salud llega a 0
- Puedes intentarlo de nuevo ejecutando el juego otra vez

## 🎨 Interfaz

El juego utiliza una interfaz de texto con:
- Diseños ASCII art para pantallas importantes
- Descripciones inmersivas de localizaciones
- Sistema de menús intuitivo con opciones numeradas
- Seguimiento de progreso (X/10 localizaciones)
- Pantallas de estadísticas del jugador

## 🛠️ Requisitos Técnicos

- **.NET 9.0** o superior
- Sistema operativo: Windows, macOS, o Linux
- Terminal con soporte para caracteres Unicode

## 📝 Comandos Útiles

```bash
# Compilar el juego
dotnet build

# Ejecutar el juego
dotnet run

# Limpiar compilación
dotnet clean
```

---

¡Buena suerte en tu aventura, héroe! ⚔️🛡️
