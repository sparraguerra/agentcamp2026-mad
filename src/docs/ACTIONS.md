# Acciones del Juego - D&D Copilot

Este documento es la **fuente de verdad** para todas las acciones del juego.
El código carga los perfiles de acciones desde este archivo en tiempo de ejecución.

> **Formato**: Cada localización o contexto se define con un bloque `## Location: <Nombre>`
> seguido de una lista de acciones disponibles. El parser del juego consume este formato.

---

## Location: Village Square

- **Description**: El centro de la aldea donde comienza tu aventura
- **NPC**: (Ninguno)
- **Actions**:
  - **Go to dark forest**: Inicia el camino de aventura hacia la AI Tower
  - **Go to tavern**: Dirígete a The Rusty Dragon Tavern (norte)
  - **Go to blacksmith**: Dirígete a la Fragua del Herrero (este)
  - **Go to store**: Dirígete a la Tienda General (oeste)
  - **Go to town hall**: Dirígete al Ayuntamiento (sur)
  - **Look around**: Ejecuta Observe/Decide/Act para analizar entorno y encontrar botin
  - **Inventory**: Revisa tu mochila y objetos
  - **Rest**: Descansa y recupera energía

---

## Location: The Rusty Dragon Tavern

- **Description**: Una taberna cálida y acogedora llena de aventureros
- **NPCs**: Grundy, Elara, Bencomo
- **Actions**:
  - **Look Around**: Observa a los clientes y el ambiente
  - **Talk**: Habla con los NPCs presentes
    - **Talk to Grundy**: Habla con el bartender Grundy
    - **Talk to Elara**: Habla con la barda Elara
    - **Talk to Bencomo**: Habla con el misterioso Bencomo
  - **Quest Board**: Revisa las misiones disponibles en el tablero
  - **Order Drink**: Pide una bebida al bartender
  - **Listen to Bard**: Escucha la música de la barda
  - **Approach Adventurers**: Acércate a otros aventureros para escuchar rumores
  - **Investigate Hooded Figure**: Investiga a la figura encapuchada misteriosa
  - **Leave tavern**: Sal de la taberna

---

## Location: Blacksmith's Forge

- **Description**: Una fragua muy caliente con armas y armaduras
- **NPCs**: Marcus
- **Actions**:
  - **Look**: Observa las armas y armaduras disponibles
  - **Talk to Marcus**: Ejecuta ODA en la forja para recomendarte exactamente 1 equipo que te falte
  - **Browse Weapons**: Examina las armas disponibles (Espada de Hierro 50g, Hacha de Acero 75g)
  - **Browse Armor**: Examina las armaduras (Armadura de Cuero 40g)
  - **Buy Weapon**: Compra una arma
  - **Buy Armor**: Compra armadura
  - **Leave Forge**: Sal de la fragua

---

## Location: General Store

- **Description**: Una tienda general con suministros para aventureros
- **NPCs**: Merchant
- **Actions**:
  - **Look**: Observa los productos en venta
  - **Talk to Merchant**: Habla con el mercader
  - **Browse Supplies**: Examina pociones y suministros
  - **Buy Supplies**: Compra pociones y objetos útiles
  - **Leave Store**: Sal de la tienda

---

## Location: Town Hall

- **Description**: El ayuntamiento de la aldea, centro administrativo
- **NPCs**: Mayor
- **Actions**:
  - **Look**: Observa el edificio y los documentos
  - **Talk to Mayor**: Habla con el Alcalde sobre noticias de la aldea
  - **Check Notices**: Lee los edictos y anuncios oficiales
  - **Leave Town Hall**: Sal del ayuntamiento

---

## Location: Dark Forest

- **Description**: Un bosque denso donde apenas penetra la luz
- **Enemies**: Goblins, Wolves, Bandits
- **Actions**:
  - **Go to ancient ruins**: Avanza por el sendero del bosque hacia las ruinas
  - **Look**: Observa los alrededores peligrosos
  - **Fight**: Inicia combate con enemigos cercanos
  - **Search**: Busca objetos o enemigos ocultos
  - **Rest**: Descansa (peligroso - puede atraer enemigos)
  - **Escape**: Intenta huir del bosque
  - **Inventory**: Revisa tu mochila
  - **Cast Spell**: Lanza un hechizo si lo tienes disponible

---

## Location: Ancient Ruins

- **Description**: Ruinas de una civilización perdida
- **Enemies**: Skeletons, Goblins, Ancient Guardians
- **Actions**:
  - **Go to crystal cavern**: Sigue las runas antiguas hasta la caverna cristalina
  - **Look**: Examina las ruinas y busca indicios
  - **Fight**: Enfrenta a los enemigos
  - **Examine Artifacts**: Inspecciona artefactos antiguos
  - **Search for Treasure**: Busca tesoro escondido
  - **Decipher Inscriptions**: Intenta descifrar inscripciones antiguas
  - **Rest**: Descansa entre las ruinas
  - **Escape**: Huye de las ruinas

---

## Location: Crystal Cavern

- **Description**: Una cueva iluminada por cristales brillantes
- **Enemies**: Giant Spiders, Bats, Crystal Elementals
- **Actions**:
  - **Go to poison swamp**: Cruza la grieta y baja hacia el pantano venenoso
  - **Look**: Observa los cristales brillantes
  - **Mine Crystals**: Extrae cristales valiosos
  - **Fight**: Combate contra las criaturas
  - **Navigate**: Busca tu camino a través de la caverna
  - **Search**: Busca rutas alternativas
  - **Rest**: Descansa bajo la luz de los cristales
  - **Escape**: Huye hacia la salida

---

## Location: Poison Swamp

- **Description**: Un pantano lleno de gases tóxicos y criaturas peligrosas
- **Enemies**: Toxic Creatures, Serpents, Bog Monsters
- **Actions**:
  - **Go to frozen mountain**: Encuentra la salida norte hacia la montaña helada
  - **Look**: Observa el pantano peligroso
  - **Wade**: Intenta atravesar el pantano
  - **Fight**: Combate contra las criaturas del pantano
  - **Find Safe Path**: Busca un camino seguro
  - **Rest**: Descansa en una zona segura
  - **Use Antidote**: Usa antídoto contra veneno
  - **Escape**: Huye del pantano

---

## Location: Frozen Mountain

- **Description**: Una montaña cubierta de nieve eterna
- **Enemies**: Frost Giants, Dire Wolves, Ice Elementals
- **Actions**:
  - **Go to abandoned temple**: Sigue la ruta del risco hasta el templo abandonado
  - **Look**: Observa la montaña helada
  - **Climb**: Intenta escalar la montaña
  - **Fight**: Combate contra los enemigos helados
  - **Warm Up**: Caliéntate para evitar congelación
  - **Search for Shelter**: Busca un lugar cálido
  - **Rest**: Descansa (requiere calor)
  - **Escape**: Desciende de la montaña

---

## Location: Abandoned Temple

- **Description**: Un templo antiguo donde los dioses ya no responden
- **Enemies**: Undead, Temple Guardians, Cursed Spirits
- **Actions**:
  - **Go to fire desert**: Desciende por el pasadizo roto hasta el desierto ardiente
  - **Look**: Observa los símbolos y altares
  - **Pray**: Reza en los altares
  - **Fight**: Combate contra los no-muertos
  - **Examine Altar**: Inspecciona los altares para pistas
  - **Find Treasure**: Busca tesoros sagrados
  - **Break Curse**: Intenta romper maldiciones
  - **Escape**: Huye del templo

---

## Location: Fire Desert

- **Description**: Un desierto árido donde el sol abrasa sin piedad
- **Enemies**: Sand Elementals, Scorpions, Desert Bandits
- **Actions**:
  - **Go to ruined fortress**: Avanza entre dunas hacia la fortaleza en ruinas
  - **Look**: Observa el desierto desolado
  - **Drink Water**: Bebe agua para evitar deshidratación
  - **Fight**: Combate contra las criaturas del desierto
  - **Find Oasis**: Busca un oasis
  - **Travel**: Continúa a través del desierto
  - **Rest**: Descansa en la sombra
  - **Escape**: Abandona el desierto

---

## Location: Ruined Fortress

- **Description**: Una fortaleza que fue imponente, ahora en ruinas
- **Enemies**: Armored Skeletons, War Golems, Fortress Guardians
- **Actions**:
  - **Go to dark dungeon**: Accede al calabozo desde la sala de mando derruida
  - **Look**: Observa la fortaleza demolida
  - **Fight**: Combate contra los guardianes
  - **Climb Walls**: Escala los muros de la fortaleza
  - **Search Barracks**: Busca en los cuarteles
  - **Examine Armory**: Inspecciona el arsenal
  - **Find Command Room**: Busca la sala de comandos
  - **Escape**: Abandona la fortaleza

---

## Location: Dark Dungeon

- **Description**: Un laberinto subterráneo lleno de trampas mortales
- **Enemies**: Demons, Dungeon Horrors, Trap Guardians
- **Actions**:
  - **Go to ai tower**: Abre la puerta final y asciende hacia la AI Tower
  - **Look**: Examina cuidadosamente las trampas
  - **Fight**: Combate contra las criaturas infernales
  - **Disarm Trap**: Intenta desactivar una trampa
  - **Navigate**: Find the correct path through the maze
  - **Search**: Busca pistas y tesoro
  - **Rest**: Descansa en una cámara segura
  - **Escape**: Busca la salida del calabozo

---

## Location: AI Tower

- **Description**: Una torre futurista donde la magia y la tecnología se fusionan
- **NPCs**: Alberto Díaz (Boss Final)
- **Enemies**: AI Constructs, Magical Protections
- **Actions**:
  - **Talk**: Habla con el NPC final presente
    - **Talk to Alberto Díaz**: Enfréntate verbalmente al maestro de SCanaryNet
  - **Look**: Observa la tecnología mágica
  - **Ascend**: Sube por los pisos de la torre
  - **Hack Console**: Intenta hackear consolas (si tienes habilidades)
  - **Fight**: Enfrenta al jefe final
  - **Shut Down SCanaryNet**: Apaga la IA canaria para debilitar y vencer a Alberto Díaz
  - **Examine Technology**: Estudia la tecnología avanzada
  - **Solve Puzzle**: Resuelve los acertijos de protección mágica
  - **Escape**: Intenta huir de la torre

---

## Global Actions

Estas acciones funcionan en cualquier localización:

- **Inventory**: Revisa tu inventario
- **Talk to Explorador**: Habla con el explorador itinerante para pedir consejo tactico en cualquier localizacion
- **Status**: Consulta tu estado (vida, maná, estado de buffs/debuffs)
- **Rest**: Descansa para recuperar vida
- **Save Game**: Guarda tu progreso
- **Help**: Muestra lista de comandos disponibles
- **Quit**: Abandona el juego
