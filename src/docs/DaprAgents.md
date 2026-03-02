# Dapr Agents - Guion en formato Speaker Notes por slide

## Slide 1 - Titulo
**Contenido slide**
- Dapr Agents
- De prototipo IA a sistema distribuido
- Caso real: DnD Copilot

**Speaker notes (extendidas)**
- Apertura recomendada (15-20 segundos): "Hoy no vengo a vender un framework de moda; vengo a enseñar como llevar agentes de IA a un terreno donde no se rompan en cuanto hay carga, fallos o mas de un servicio."
- Objetivo de la charla: aterrizar Dapr Agents en una arquitectura que un equipo backend pueda mantener en el tiempo.
- Contexto para la audiencia: si ya conocen microservicios y Dapr, entenderan rapido por que Dapr Agents encaja bien.
- Mensaje central que debes repetir en varios momentos: "Agente sin plataforma es prototipo; agente con runtime distribuido es sistema." 
- Transicion: "Primero refrescamos Dapr y agentes en 2 bloques muy cortos, y luego lo bajamos al repo con una demo unica al final."

---

## Slide 2 - Agenda
**Contenido slide**
1. Intro Dapr (quick refresh)
2. Intro agentes (quick refresh)
3. Que son Dapr Agents
4. Endpoints, ciclo, tools, memoria, patrones
5. Integracion con pub/sub, workflows, bindings
6. Ventajas/desventajas
7. Lenguajes
8. Demo final (solo al final)

**Speaker notes (extendidas)**
- Explica la dinamica: primero marco conceptual, luego implementacion real en codigo.
- Justifica por que dejas demo al final: asi la audiencia entiende "que" y "por que" antes del "como".
- Marca expectativa de valor: "Al acabar, no solo sabremos que es Dapr Agents; sabremos exactamente donde vive cada pieza en nuestro codigo."
- Recomendacion de tiempo:
  - Slides 3-7: 8-10 min
  - Slides 8-15: 15-20 min
  - Slides 16-25 (demo): 10-12 min
  - Cierre: 2 min
- Transicion: "Empezamos por Dapr en 60 segundos."

---

## Slide 3 - Dapr en 60 segundos
**Contenido slide**
- Runtime con sidecar
- Building blocks: invocation, state, pub/sub, workflows, bindings
- Polyglot
- Componentes declarativos

**Speaker notes (extendidas)**
- Define Dapr en una frase corta: "Dapr es una capa de capacidades distribuidas estandarizadas, desacopladas de tu lenguaje y de tu proveedor cloud."
- Sidecar: insiste en que no contamina la logica de negocio con SDKs de 10 proveedores.
- Building blocks: no hace falta explicar todos en profundidad, solo remarcar que resuelven problemas repetitivos de cualquier sistema distribuido.
- Polyglot: conecta con el debate de lenguaje en agentes.
- Componentes declarativos: valioso para platform teams y operaciones.
- Frase ancla: "Con Dapr no codificas infraestructura cada vez: la configuras."
- Transicion: "Con eso en mente, recordemos que es un agente en terminos practicos."

---

## Slide 4 - Agentes en 60 segundos
**Contenido slide**
- LLM para razonar
- Tools para actuar
- Memoria para continuidad
- Loop de decision

**Speaker notes (extendidas)**
- Evita una definicion filosofica. Hazlo operativo:
  - Razonar: interpretar objetivo y decidir siguiente paso.
  - Actuar: ejecutar herramientas que cambian el mundo real (APIs, DB, colas).
  - Recordar: no repetir contexto en cada turno.
  - Iterar: planificar y ajustar segun resultados.
- Riesgo sin plataforma: cada equipo inventa su propio "mini runtime" (retries, estado, colas, trazas, seguridad).
- Mensaje para equipos enterprise: el problema no es hacer un agente que responda, el problema es operarlo con fiabilidad.
- Transicion: "Aqui entra Dapr Agents."

---

## Slide 5 - Que es Dapr Agents (mensaje equilibrado)
**Contenido slide**
- Framework oficial (foco actual Python)
- Arquitectura/patron reutilizable sobre Dapr
- Enfoque enterprise: durable + observable + escalable

**Speaker notes (extendidas)**
- Di explicitamente el matiz para evitar confusion:
  - "A dia de hoy, el framework oficial de Dapr Agents esta centrado en Python."
  - "Pero el patron arquitectonico no depende de Python porque descansa en building blocks de Dapr."
- Esto evita dos errores comunes:
  - Pensar que solo Python puede hacer agentes con Dapr.
  - Prometer que todo lo de docs Python existe tal cual en .NET hoy.
- Frase recomendada: "El runtime es comun; la ergonomia del framework varia por lenguaje."
- Transicion: "Veamos la estructura mental que propone Dapr Agents."

---

## Slide 6 - Estructura de Dapr Agents
**Contenido slide**
- Agent
- DurableAgent
- AgentRunner

**Speaker notes (extendidas)**
- Agent:
  - Conversacional, directo, ideal para flujos mas acotados.
  - Puede tener memoria y tools.
- DurableAgent:
  - Pensado para ejecuciones largas, recuperables, con estado persistido.
  - Se apoya en workflows para checkpoint y reanudacion.
- AgentRunner:
  - Es el puente entre el agente y el mundo: HTTP, pub/sub, ejecucion programatica.
- Punto didactico: esta triada separa bien "inteligencia", "durabilidad" y "forma de exposicion".
- Transicion: "Ahora, como se traduce esto a endpoints y flujo de ejecucion."

---

## Slide 7 - Endpoints en Dapr Agents
**Contenido slide**
- `POST /agent/run`
- `GET /agent/instances/{id}`
- Entrada por eventos (pub/sub)

**Speaker notes (extendidas)**
- Explica el patron oficial:
  - `POST /agent/run`: dispara trabajo (normalmente async en durable).
  - `GET /agent/instances/{id}`: consulta estado/resultado.
- Entra por eventos:
  - no todo tiene que venir por request/response HTTP.
  - sistemas event-driven encajan mejor con pub/sub.
- En tu repo:
  - la API de agente esta modelada como `observe/decide/act`, que es una variante valida y muy explicable.
- Transicion: "Con endpoint definido, veamos el loop interno del agente."

---

## Slide 8 - Ciclo de razonamiento
**Contenido slide**
1. input
2. decidir
3. usar tool
4. integrar resultado
5. responder
6. persistir contexto

**Speaker notes (extendidas)**
- Recomendacion al explicarlo: usa un ejemplo de 1 frase ("hablo con Elara y pregunto por una quest").
- Mapea el ejemplo al ciclo:
  - input: pregunta del jugador.
  - decidir: el modelo evalua intencion.
  - usar tool: consulta catalogo/estado/evento si aplica.
  - integrar: combina salida de tool + memoria.
  - responder: linea del NPC.
  - persistir: guarda turno para continuidad.
- Mensaje tecnico: este ciclo es donde se nota si tienes o no una base de plataforma.
- Transicion: "La pieza mas subestimada suele ser tools."

---

## Slide 9 - Tools
**Contenido slide**
- Tool local
- Tool remota
- Tool de datos/documentos

**Speaker notes (extendidas)**
- Define tool como contrato de accion, no como funcion suelta.
- Tool local:
  - logica encapsulada en el propio servicio.
- Tool remota:
  - service invocation, APIs internas, MCP.
- Tool de datos/documentos:
  - retrieval, consultas, operaciones sobre fuentes de conocimiento.
- Frase para enfatizar: "El LLM no deberia inventar acciones; deberia orquestar herramientas confiables."
- Transicion: "Para que no parezca magia, la memoria es clave."

---

## Slide 10 - Memoria / State
**Contenido slide**
- In-memory
- Vector memory
- State store (produccion)

**Speaker notes (extendidas)**
- In-memory:
  - buena para demo y prototipo local.
  - debilidad: reinicio = perdida de contexto.
- Vector memory:
  - util para semantica y RAG.
- State store:
  - enfoque pragmatico para continuidad transaccional/conversacional.
- En produccion, habla de dos cosas:
  - politicas de retencion (evitar crecimiento infinito).
  - estrategia de clave de sesion (multiusuario/multinpc).
- Transicion: "Con tools y memoria, pasamos de chat a sistema agentico."

---

## Slide 11 - Patrones
**Contenido slide**
- Augmented LLM
- Prompt chaining
- Routing
- Parallelization
- Orchestrator-workers
- Evaluator-optimizer
- Durable agent

**Speaker notes (extendidas)**
- Recomendacion de storytelling: presentalo como "espectro de autonomia".
- Desde simple (Augmented LLM) hasta autonomo (Durable agent).
- Consejo arquitectonico para audiencias enterprise:
  - empieza por patrones controlados.
  - sube autonomia cuando tengas observabilidad y guardrails.
- Frase util: "No se trata de poner mas autonomia; se trata de poner la autonomia necesaria."
- Transicion: "Ahora enlazamos con los bloques Dapr que habilitan esto en serio."

---

## Slide 12 - Integracion pub/sub, workflows, bindings
**Contenido slide**
- Pub/sub: coordinacion asincrona
- Workflows: durabilidad, retries, checkpoint
- Bindings: conectores a sistemas externos

**Speaker notes (extendidas)**
- Pub/sub:
  - desacopla productores/consumidores.
  - facilita multiagente y escalado independiente.
- Workflows:
  - evita perder progreso ante fallos.
  - aporta trazabilidad de pasos largos.
- Bindings:
  - salida/entrada estandarizada hacia sistemas externos.
- Mensaje de plataforma: "Dapr Agents no compite con Dapr; se apoya en Dapr."
- Transicion: "Con esta base, veamos trade-offs reales."

---

## Slide 13 - Ventajas
**Contenido slide**
- Fiabilidad operativa
- Escalabilidad real
- Menos pegamento de infraestructura
- Observabilidad y seguridad
- Menor lock-in

**Speaker notes (extendidas)**
- Fiabilidad: si hay fallo de red o nodo, no se pierde todo.
- Escalabilidad: separas agentes, memoria, mensajeria y exposicion.
- Menos pegamento: menos codigo accidental, mas foco en dominio.
- Observabilidad: trazas y logs consistentes para debugging real.
- Menor lock-in: componentes intercambiables por entorno.
- Consejo de presentacion: usa una historia de "prototipo que funcionaba hasta que hubo concurrencia".
- Transicion: "No hay bala de plata, asi que toca hablar de costes."

---

## Slide 14 - Desventajas
**Contenido slide**
- Curva de aprendizaje
- Mas complejidad inicial
- Puede ser too much para casos pequenos
- Ecosistema oficial Agents mas maduro en Python

**Speaker notes (extendidas)**
- Curva de aprendizaje:
  - hay conceptos nuevos para equipos solo app-level.
- Complejidad inicial:
  - sidecar, componentes, deployment model.
- Overkill en casos pequenos:
  - para una demo de 1 prompt, puede sobrar.
- Madurez por lenguaje:
  - experiencia de desarrollo hoy es mas completa en Python para Dapr Agents oficial.
- Frase honesta: "Si tu problema no es distribuido, no sobredimensiones."
- Transicion: "Esto conecta con la pregunta clasica: en que lenguaje lo hago."

---

## Slide 15 - Lenguajes
**Contenido slide**
- Estado actual: framework `dapr-agents` en Python
- Runtime Dapr: polyglot
- Patron aplicable en .NET, Java, Go, Node

**Speaker notes (extendidas)**
- Respuesta corta para preguntas de pasillo:
  - "Framework oficial, Python."
  - "Patron sobre Dapr, polyglot."
- En .NET:
  - puedes implementar ciclo de agente, memoria state, eventos y endpoints sin bloqueo tecnico.
- Recomendacion estrategica:
  - si equipo es .NET-first y ya opera Dapr, tiene sentido construir en .NET.
  - si quieres seguir quickstarts oficiales al pie de la letra, Python acelera onboarding.
- Transicion: "Con esto claro, pasamos a la demo unica final."

---

## Slide 16 - Demo final: objetivo
**Contenido slide**
- Una sola demo al final
- Mostrar `donde` y `como` se aplica cada concepto en vuestro codigo

**Speaker notes (extendidas)**
- Aclara que la demo es de "trazabilidad arquitectonica".
- Estructura verbal de cada parada:
  - concepto -> archivo -> evidencia -> estado actual.
- Define exito de la demo:
  - la audiencia sale sabiendo que ya existe y que falta por cerrar.
- Transicion: "Primero vemos el mapa general y luego entramos en cada pieza."

---

## Slide 17 - Demo final: mapa rapido concepto -> archivo
**Contenido slide**
- Endpoints -> `src/DndCopilot.Api/Controllers/NpcAgentController.cs`
- Patrones -> `src/DndCopilot.Core/Services/NpcAgentService.cs`
- Razonamiento -> `src/DndCopilot.Core/Services/NpcAgentService.cs`
- Tools (equivalente actual) -> `src/DndCopilot.Core/Services/ActionService.cs`
- Memoria/State -> `src/DndCopilot.Infrastructure/Services/DaprStateClient.cs`
- Pub/Sub -> `src/DndCopilot.Infrastructure/Services/DaprEventPublisher.cs`
- Workflows -> gap actual
- Bindings -> gap actual

**Speaker notes (extendidas)**
- Esta slide te da control narrativo: evitas navegar sin orden por el repo.
- Recalca que no escondes gaps: eso aporta credibilidad.
- Mensaje: "No partimos de cero: tenemos patron base operativo con algunos bloques pendientes de madurar."
- Transicion: "Empezamos por la puerta de entrada: endpoints."

---

## Slide 18 - Demo final detallada: Endpoints
**Contenido slide**
- Donde
  - `src/DndCopilot.Api/Controllers/NpcAgentController.cs:13`
  - `src/DndCopilot.Api/Controllers/NpcAgentController.cs:28`
  - `src/DndCopilot.Api/Controllers/NpcAgentController.cs:64`
  - `src/DndCopilot.Api/Controllers/NpcAgentController.cs:112`
- Como
  - API de agente separada en `observe`, `decide`, `act`.

**Speaker notes (extendidas)**
- Explicacion para audiencia tecnica:
  - controller dedicado, contrato limpio por fase.
  - validaciones de entrada por endpoint.
- Ventaja de ODA en API:
  - testabilidad por fases.
  - control y trazabilidad sobre cada decision.
- Relacion con Dapr Agents:
  - no replica `/agent/run`, pero implementa el mismo concepto de "agent endpoint".
- Transicion: "Detras de estos endpoints esta el patron de comportamiento."

---

## Slide 19 - Demo final detallada: Patrones
**Contenido slide**
- Donde
  - `src/DndCopilot.Api/Controllers/NpcAgentController.cs`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs`
  - `src/DndCopilot.Infrastructure/Services/FallbackFoundryAiClient.cs:7`
- Como
  - Patron Observe-Decide-Act.
  - Patron fallback (Dapr Conversation -> mock en fallo de conexion).

**Speaker notes (extendidas)**
- ODA:
  - observe: recolecta contexto.
  - decide: elige accion con reasoning.
  - act: ejecuta y genera resultado/eventos.
- Fallback:
  - comportamiento robusto en local/dev cuando Conversation API no esta disponible.
- Mensaje de ingenieria:
  - hay patron de IA y patron de resiliencia, ambos son necesarios.
- Transicion: "Vamos ahora al nucleo: el ciclo de razonamiento en codigo."

---

## Slide 20 - Demo final detallada: Ciclo de razonamiento
**Contenido slide**
- Donde
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:34`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:44`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:61`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:84`
- Como
  1. Lee memoria.
  2. Construye prompt/contexto.
  3. Llama al LLM.
  4. Persiste turno.
  5. Emite evento.

**Speaker notes (extendidas)**
- Recorre `RunAsync` de arriba a abajo, sin saltos.
- Puntos a subrayar durante la demo:
  - clave de memoria por npc+sesion.
  - truncado de historial (ultimos 20 mensajes).
  - emision de evento de interaccion.
- Si hay poco tiempo, esta es la slide mas importante de la demo.
- Transicion: "Ese loop necesita herramientas y contratos de accion."

---

## Slide 21 - Demo final detallada: Tools
**Contenido slide**
- Donde (equivalente actual)
  - `src/DndCopilot.Core/Services/ActionService.cs:14`
  - `src/DndCopilot.Core/Services/ActionService.cs:66`
  - `docs/ACTIONS.md`
- Como
  - No hay function-calling formal de tools Dapr Agents.
  - Hay un catalogo de acciones validables/ejecutables usado por el motor de juego.

**Speaker notes (extendidas)**
- Importante: no sobreprometer.
- Explica "equivalente actual":
  - las acciones funcionan como herramientas de dominio.
  - el agente decide dentro del espacio de acciones posibles.
- Diferencia con docs oficiales Python:
  - no hay decorador `@tool` ni MCP tools runtime en este flujo.
- Esto te permite abrir una linea de evolucion clara para siguientes iteraciones.
- Transicion: "Ahora vamos a la memoria persistente, que si esta totalmente aterrizada."

---

## Slide 22 - Demo final detallada: Memoria / State
**Contenido slide**
- Donde
  - `src/DndCopilot.Infrastructure/Services/DaprStateClient.cs:38`
  - `src/DndCopilot.Infrastructure/Services/DaprStateClient.cs:69`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:37`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:61`
  - `components/statestore.yaml:1`
- Como
  - Se usa Dapr HTTP API `v1.0/state`.
  - Clave por NPC + sesion (`agent-memory-{npc}-{sessionId}`).
  - Se limita a ultimos 20 mensajes.

**Speaker notes (extendidas)**
- Demuestra capa por capa:
  - servicio de dominio usa interfaz `IDaprStateClient`.
  - infraestructura implementa llamadas a `v1.0/state`.
  - componente define backend de estado.
- Beneficio arquitectonico:
  - puedes cambiar backend state sin reescribir el servicio de agente.
- Nota operativa:
  - hoy `npc-statestore` esta en `state.in-memory`; para produccion querrias backend persistente.
- Transicion: "Con estado listo, pasamos a eventos y pub/sub."

---

## Slide 23 - Demo final detallada: Integracion pub/sub
**Contenido slide**
- Donde
  - `src/DndCopilot.Infrastructure/Services/DaprEventPublisher.cs:31`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:84`
  - `src/DndCopilot.Core/Services/NpcAgentService.cs:240`
  - `src/DndCopilot.Api/Program.cs:98`
- Como
  - Existe publisher Dapr real (`v1.0/publish/{pubsub}/{topic}`).
  - En runtime actual se inyecta `StubDaprEventPublisher`, no publisher Dapr real.

**Speaker notes (extendidas)**
- Explicalo en dos capas:
  - capacidad implementada: `DaprEventPublisher` real.
  - configuracion actual: stub para desarrollo en DI.
- Mensaje claro para evitar malentendidos:
  - "El codigo para publicar a Dapr pub/sub existe, pero no esta activado por defecto en la composicion actual."
- Esto te permite hablar de "readiness parcial": arquitectura preparada, wiring pendiente.
- Transicion: "Y ahora los dos bloques que hoy son roadmap: workflows y bindings."

---

## Slide 24 - Demo final detallada: Workflows y Bindings
**Contenido slide**
- Workflows
  - Estado actual: no hay implementacion de Dapr Workflows en `src/**`.
  - Hay mention/vision en documentacion.
- Bindings
  - Estado actual: no hay componentes de tipo `bindings.*` en `components/`.

**Speaker notes (extendidas)**
- Workflows:
  - hoy no hay orchestration durable implementada en codigo.
  - impacto: menos resiliencia para procesos largos.
- Bindings:
  - hoy no hay conectores declarativos de input/output en componentes.
- Recomienda siguientes pasos concretos:
  1. activar pub/sub real en DI.
  2. introducir primer workflow para una accion larga.
  3. agregar un binding externo para un caso de integracion real.
- Transicion: "Cerramos con la receta de demo para repetirla sin improvisar."

---

## Slide 25 - Demo final: script de ejecucion (paso a paso)
**Contenido slide**
1. Abrir `NpcAgentController` y explicar ODA endpoints.
2. Abrir `NpcAgentService.RunAsync` y recorrer loop.
3. Abrir `DaprConversationAiClient` y mostrar llamada a Conversation API.
4. Abrir `DaprStateClient` y mostrar `v1.0/state`.
5. Abrir `DaprEventPublisher` y explicar `v1.0/publish`.
6. Abrir `Program.cs` y mostrar inyeccion actual (`StubDaprEventPublisher`).
7. Abrir `components/openai.yaml` y `components/statestore.yaml`.
8. Cerrar con gaps de workflows/bindings.

**Speaker notes (extendidas)**
- Script recomendado (8-12 min):
  - 1 min: controller (entrada).
  - 3 min: `RunAsync` (nucleo).
  - 1.5 min: conversation client.
  - 1.5 min: state.
  - 1.5 min: pub/sub + DI.
  - 1.5 min: componentes + roadmap.
- Consejo de delivery:
  - en cada paso di una frase fija: "esto implementa X y se conecta con Y".
- Evita abrir mas archivos de los necesarios para no romper ritmo.
- Transicion: "Con esto, cierro con una idea final."

---

## Slide 26 - Cierre
**Contenido slide**
- Dapr Agents = IA + disciplina de sistemas distribuidos
- Ya aplicado en el repo: endpoints, loop, memoria, conversation API
- Siguiente evolucion: pub/sub real en runtime, workflows, bindings

**Speaker notes (extendidas)**
- Cierre en 3 frases:
  1. "Ya tenemos base agentica funcional en el repo."
  2. "No estamos en fase conceptual: hay ciclo, memoria y endpoints reales."
  3. "El salto siguiente es completar durabilidad y eventos en runtime productivo."
- Llamada a la accion para el equipo:
  - elegir un flujo de negocio candidato a workflow.
  - activar publisher real y medir trazas de punta a punta.
- Frase final sugerida: "No partimos de cero; partimos de una base valida y con direccion clara."

---

## Referencias oficiales
- https://docs.dapr.io/developing-ai/dapr-agents
- https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-introduction/
- https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-core-concepts/
- https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-patterns/
- https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-integrations/
- https://docs.dapr.io/developing-ai/dapr-agents/dapr-agents-quickstarts/
