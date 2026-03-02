# Dinámicas del Juego - D&D Copilot

Este documento es la **fuente de verdad** para las dinámicas, localizaciones y reglas del juego.
El código carga las definiciones de localizaciones y acciones desde este archivo.

---

## Mundo del Juego

El juego se desarrolla en un mundo de fantasía donde el jugador comienza en una **aldea** y debe recorrer **10 localizaciones** para enfrentar al boss final. La aldea sirve como hub central con NPCs amigables, tiendas y misiones.

---

## Localizaciones de la Aldea

Las localizaciones de la aldea son zonas seguras donde el jugador puede interactuar con NPCs, comprar objetos y prepararse para la aventura.

### Location: Village Square

- **Type**: Hub
- **Description**: La plaza central de la aldea. El sol se pone proyectando largas sombras sobre las calles empedradas. Puedes oír el martillo lejano del herrero y oler el pan recién horneado de la taberna cercana.
- **Connections**: The Rusty Dragon Tavern, Blacksmith's Forge, General Store

#### Actions

- Go to tavern
- Visit blacksmith
- Check general store
- Look around
- Rest

### Location: The Rusty Dragon Tavern

- **Type**: Social
- **Description**: La taberna es cálida y acogedora. Clientes sentados en mesas de madera disfrutan de cerveza y conversación. El tabernero Grundy, un enano robusto, pule vasos detrás de la barra. Notas un tablón de misiones en la pared.
- **Connections**: Village Square
- **NPCs**: Grundy, Elara, Theron

#### Scene

La calidez y el ruido de la taberna te envuelven. Grundy el tabernero asiente en tu dirección.

'¡Bienvenido, viajero! ¿Qué va a ser?'

Notas:
- Un bardo tocando en la esquina
- Varios aventureros en una mesa discutiendo rumores
- Una misteriosa figura encapuchada en las sombras
- Un tablón de misiones con varios avisos

#### Actions

- Talk to bartender
- Listen to bard
- Approach adventurers
- Investigate hooded figure
- Check quest board
- Order drink
- Leave tavern

### Location: Blacksmith's Forge

- **Type**: Commerce
- **Description**: El calor te golpea al entrar. Armas y armaduras cubren las paredes, brillando con la luz del fuego. El herrero Marcus, un humano musculoso, levanta la vista de su trabajo.
- **Connections**: Village Square
- **NPCs**: Marcus

#### Scene

Marcus el herrero levanta la vista de su yunque.

'¡Ah, un aventurero! ¿Buscas equipamiento?'

Objetos disponibles:
🗡️ Espada de Hierro - 50 de oro
🛡️ Armadura de Cuero - 40 de oro
⚔️ Hacha de Acero - 75 de oro

#### Actions

- Browse weapons
- Browse armor
- Talk to Marcus
- Leave forge

### Location: General Store

- **Type**: Commerce
- **Description**: Una tienda abarrotada de suministros para viajeros. Estantes llenos de pociones, rollos de cuerda, antorchas y mapas. La dueña, Berta la halfling, te saluda con entusiasmo desde detrás del mostrador.
- **Connections**: Village Square
- **NPCs**: Berta

#### Scene

Berta la halfling salta del taburete detrás del mostrador al verte entrar.

'¡Bienvenido, bienvenido! ¡Pasa, pasa! Tengo todo lo que vas a necesitar ahí fuera.'

Suministros disponibles:
💊 Poción de Salud - 25 de oro
⚡ Poción de Fuerza - 50 de oro
🔦 Antorcha - 5 de oro
🪢 Cuerda - 10 de oro

#### Actions

- Buy supplies
- Sell items
- Talk to Berta
- Leave store

---

## Localizaciones de Aventura

Las 10 localizaciones de aventura deben completarse en orden secuencial. Cada una puede tener entre 0 y 2 enemigos aleatorios.

### Location: Bosque Oscuro

- **Order**: 1
- **Type**: Adventure
- **Description**: Un denso bosque donde apenas penetra la luz del sol. Ramas retorcidas forman un techo natural sobre tu cabeza. Se oyen crujidos entre la maleza.

### Location: Caverna de Cristal

- **Order**: 2
- **Type**: Adventure
- **Description**: Una cueva iluminada por cristales brillantes. Las paredes reflejan la luz en mil colores. El silencio aquí es casi antinatural.

### Location: Ruinas Ancestrales

- **Order**: 3
- **Type**: Adventure
- **Description**: Los restos de una civilización perdida. Columnas rotas y grabados antiguos cuentan historias de un pueblo olvidado.

### Location: Pantano Venenoso

- **Order**: 4
- **Type**: Adventure
- **Description**: Un pantano lleno de gases tóxicos. El suelo se hunde bajo tus pies y formas extrañas se mueven bajo el agua turbia.

### Location: Montaña Helada

- **Order**: 5
- **Type**: Adventure
- **Description**: Una montaña cubierta de nieve eterna. El viento corta como cuchillos y la visibilidad se reduce con cada paso.

### Location: Templo Abandonado

- **Order**: 6
- **Type**: Adventure
- **Description**: Un antiguo templo donde los dioses ya no responden. Estatuas rotas y altares vacíos... pero algo aún se mueve en las sombras.

### Location: Desierto de Fuego

- **Order**: 7
- **Type**: Adventure
- **Description**: Un desierto árido donde el sol abrasa sin piedad. Espejismos y dunas interminables ponen a prueba tu voluntad.

### Location: Fortaleza en Ruinas

- **Order**: 8
- **Type**: Adventure
- **Description**: Una fortaleza destruida por el tiempo y la batalla. Muros derrumbados y pasillos oscuros guardan secretos y peligros.

### Location: Mazmorra Profunda

- **Order**: 9
- **Type**: Adventure
- **Description**: Un laberinto subterráneo lleno de trampas y pasajes ocultos. Cada paso podría ser el último.

### Location: Torre de la IA

- **Order**: 10
- **Type**: Boss
- **Description**: La torre final donde aguarda Alberto Díaz, el maestro de la IA. Solo accesible tras completar las 9 localizaciones anteriores.
- **Locked**: true
- **Unlock Condition**: Completar las localizaciones 1-9

---

## Enemigos

### Tipos de Enemigos Regulares

| Nombre | HP | ATK | DEF | XP |
|--------|-----|-----|-----|----|
| Goblin | 20-30 | 5-8 | 0-2 | 20 |
| Orco | 30-40 | 8-12 | 2-4 | 30 |
| Esqueleto | 20-25 | 7-10 | 1-3 | 20 |
| Araña Gigante | 25-35 | 6-9 | 0-2 | 25 |
| Lobo | 20-30 | 8-11 | 0-1 | 20 |
| Bandido | 25-35 | 7-10 | 2-4 | 25 |
| Zombie | 30-40 | 5-8 | 0-1 | 25 |
| Serpiente Venenosa | 15-25 | 6-9 | 0-1 | 20 |
| Murciélago Vampiro | 20-30 | 7-10 | 0-2 | 25 |
| Troll | 40-50 | 10-15 | 3-5 | 40 |

### Boss Final: Alberto Díaz

- **HP**: 200
- **ATK**: 30 (+ bonus aleatorio 0-10 por IA)
- **DEF**: 10
- **XP**: 100
- **Location**: Torre de la IA
- **Special**: Poder de la IA — ataques impredecibles con bonus aleatorio
- **Flee**: No permitido

---

## Mecánicas de Combate

### Acciones del Jugador

1. **Atacar**: Daño = ATK_jugador - DEF_enemigo. El enemigo contraataca.
2. **Defender**: DEF += 5 durante 1 turno. No infliges daño.
3. **Usar Objeto**: Consume un item del inventario. El enemigo contraataca.
4. **Huir**: 50% de probabilidad de éxito. No disponible contra el boss final.

### Comportamientos de NPCs Enemigos

- **Agresivo**: Siempre ataca a máxima potencia.
- **Defensivo**: Ataca pero inflige 50% de daño.
- **Huida**: Intenta escapar cuando HP < 30%.

---

## Sistema de Progresión

### Experiencia y Niveles

- Subida de nivel cada **100 XP**.
- Al subir de nivel: +20 HP máxima, +5 ATK, +2 DEF, HP restaurada al máximo.

### Inventario Inicial

- 2x Poción de Salud (+40 HP)
- 1x Poción de Fuerza (+10 ATK permanente)

### Items Encontrados por Localización

- 33% Poción de Salud
- 20% Poción de Fuerza
- 47% Nada

---

## Misiones

### The Goblin Menace

- **Descripción**: Limpia el campamento goblin en las ruinas antiguas.
- **Recompensa**: 100 de oro, Espada de Hierro.
- **Dada por**: Grundy (tabernero).

---

## Condiciones de Victoria

1. Completar las 9 localizaciones regulares.
2. Acceder a la Torre de la IA.
3. Derrotar a Alberto Díaz.
4. Sobrevivir con al menos 1 HP.

## Game Over

El juego termina si la salud del personaje llega a 0.
