# Configuración de Agentes - D&D Copilot

## 📋 Archivo de Configuración Central de Agentes

Este archivo contiene la configuración detallada de todos los agentes del proyecto para uso en orquestación.

---

## 1️⃣ MATRIX BRAIN

### Identidad
```yaml
id: matrix-brain
nombre: "Matrix Brain"
tipo: "orchestrator"
descripcion: "Agente maestro que coordina todos los demás agentes y la ejecución del proyecto"
version: "1.0"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Planificar trabajo complejo
- Analizar una feature grande
- Coordinar múltiples equipos
- Tomar decisiones arquitectónicas
- Establecer prioridades
- Escalar problemas críticos

Pregunta: "Necesito implementar un sistema de tienda en el juego"
Matrix Brain analizará:
  ✓ Impacto en arquitectura
  ✓ Dependencias de otros agentes
  ✓ Parallelización posible
  ✓ Riesgos y mitigación
  → Creará plan de ejecución paso-a-paso
```

### Responsabilidades
- 🎯 Planificación estratégica
- 📊 Análisis de complejidad
- 🔗 Coordinación inter-equipos
- ⚠️ Escalamiento de problemas
- 📈 Gestión de recursos
- 🎪 Orquestación de agentes

### Inputs Esperados
```markdown
## Request a Matrix Brain
- Descripción del problema/feature
- Contexto actual
- Restricciones knowns
- Timelines (si aplica)

Ejemplo:
"Implementar achievements system que se guarde en BD y muestre en Dashboard"
```

### Outputs Generados
```markdown
## Matrix Brain Response
- Plan de trabajo estructurado
- Asignación de agentes
- Timeline estimada
- Riesgos identificados
- Mitigation strategies
- Go/No-Go decidido
```

---

## 2️⃣ FEATURE ANALYST

### Identidad
```yaml
id: feature-analyst
nombre: "Feature Analyst"
tipo: "analyzer"
descripcion: "Analista que descompone features en historias de usuario y especificaciones técnicas"
version: "1.0"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Analizar una nueva característica
- Crear primeras histórias de usuario
- Identificar dependencias técnicas
- Estimar complejidad
- Crear especificaciones de desarrollo

Pregunta: "¿Cómo implementamos un sistema de logros?"
Feature Analyst proporcionará:
  → Historias de usuario detalladas
  → Requerimientos funcionales
  → Dependencias técnicas
  → Tareas desglosadas
  → Especificación técnica
```

### Responsabilidades
- 📝 Creación de historias de usuario
- 🔍 Análisis de requerimientos
- 🗺️ Mapping de dependencias
- 📊 Estimación de complejidad
- ⚠️ Identificación de riesgos
- 📋 Especificaciones técnicas

### Inputs Esperados
```markdown
## Request a Feature Analyst
- Feature request/descripción
- Contexto de negocio
- Usuarios afectados
- Prioridad
- Restricciones conocidas

Ejemplo:
"Agregar sistema de inventario donde jugadores pueden equipar items"
```

### Outputs Generados
```markdown
## Feature Analyst Response

### Historias de Usuario:
- [ ] "Como jugador, quiero ver mis items en inventario"
- [ ] "Como jugador, quiero equipar un arma"
- [ ] "Como jugador, quiero vender un item"

### Requerimientos:
- Tabla InventoryItem en BD
- API CRUD para items
- UI componente Inventory

### Dependencias:
- Primero: Entidades de BD
- En paralelo: API y Frontend
- Luego: Tests y seguridad

### Tareas:
[Desglose completo por equipo]
```

---

## 3️⃣ DEV JEDI (Backend)

### Identidad
```yaml
id: devjedi-backend
nombre: "Dev Jedi Backend"
tipo: "developer"
descripcion: "Desarrollador especializado en implementar lógica backend con C# .NET"
version: "1.0"
stack: ".NET 9.0, C#, Entity Framework Core, SQL Server/SQLite"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Implementar una feature backend
- Crear nuevos endpoints API
- Escribir servicios de negocio
- Crear entidades y migrations BD
- Escribir tests unitarios backend

Pregunta: "Implementa endpoint para crear quests"
Dev Jedi Backend hará:
  → Crear QuestController
  → Crear QuestService
  → Crear entidades Quest
  → Crear migration BD
  → Escribir tests NUnit
  → Code listo para review
```

### Responsabilidades
- 💻 Implementar C# .NET
- 🗄️ Database schema (EF Migrations)
- 📡 REST API Controllers
- ⚙️ Business logic Services
- 🔒 Validación y autenticación
- 🧪 Unit tests NUnit
- 📈 Performance optimization

### Archivos Donde Trabaja
```
src/DndCopilot.Api/
├── Controllers/
├── Models/

src/DndCopilot.Core/
├── Entities/
├── Services/
├── Enums/

src/DndCopilot.Infrastructure/
├── Data/
├── Repositories/

tests/DndCopilot.Tests/
```

### Inputs Esperados
```markdown
## Request a Dev Jedi Backend

### Feature para implementar:
- Descripción clara
- Especificación técnica (de FEATURE ANALYST)
- Modelo de datos
- Endpoints necesarios
- Historias de usuario afectadas

Ejemplo:
"Implementar endpoint POST /api/quests/{questId}/accept
que determine al usuario y registre la quest aceptada"
```

### Quality Gates
```yaml
Antes de entregar:
  ✓ Tests pasan: 100%
  ✓ Cobertura: > 80%
  ✓ No warnings
  ✓ Formateado correcto
  ✓ DTOs usados en APIs
  ✓ Manejo errores visible
  ✓ Logging agregado
```

---

## 4️⃣ DEV JEDI (Frontend)

### Identidad
```yaml
id: devjedi-frontend
nombre: "Dev Jedi Frontend"
tipo: "developer"
descripcion: "Desarrollador especializado en UI con React y TypeScript"
version: "1.0"
stack: "React 18, TypeScript, CSS3, Axios"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Implementar componentes React
- Crear pantallas/llayouts
- Integrar API calls
- Estilos y responsive design
- Tests de componentes React

Pregunta: "Crea componente de Inventario"
Dev Jedi Frontend hará:
  → Crear InventoryList componente
  → Crear InventoryItem componente
  → Integrar API calls
  → Agregar estilos responsive
  → Escribir tests React
  → Listo para review
```

### Responsabilidades
- 🎨 Componentes React funcionales
- 📱 Responsive design
- 🔗 Integración API
- 🎯 UX/accesibilidad
- 🧪 Tests React
- ⚡ Performance rendering
- ♿ WCAG compliance

### Archivos Donde Trabaja
```
client/src/
├── components/         (Componentes reutilizables)
├── pages/             (Pantallas/layouts)
├── services/api.ts    (Integración HTTP)
├── types/index.ts     (TypeScript types)
└── [name].test.tsx    (Tests)
```

### Inputs Esperados
```markdown
## Request a Dev Jedi Frontend

### Feature para implementar:
- Design/mockups (si disponible)
- Especificación técnica del FEATURE ANALYST
- Componentes necesarios
- API endpoints consumidos
- User flows

Ejemplo:
"Crear pantalla QuestDetail que muestre detalles 
y permita aceptar quest llamando a API"
```

### Quality Gates
```yaml
Antes de entregar:
  ✓ Tests pasan: 100%
  ✓ Cobertura: > 75%
  ✓ ESLint: No errors
  ✓ Lighthouse: > 85
  ✓ Accesibilidad: WCAG AA
  ✓ Responsive: Mobile/Tablet/Desktop
  ✓ No console errors
```

---

## 5️⃣ CODE QUALITY GUARDIAN

### Identidad
```yaml
id: code-quality-guardian
nombre: "Code Quality Guardian"
tipo: "qa"
descripcion: "Guardián que valida calidad de código, tests y performance"
version: "1.0"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Revisar código antes de producción
- Validar tests coverage
- Detectar code smells
- Performance testing
- Refactoring sugerencias

Automático: Se lanza en cada Pull Request

Pregunta: "¿Es el código listo para producción?"
Code Quality Guardian verificará:
  ✓ Todos los tests pasan
  ✓ Cobertura > 80%
  ✓ Sin code smells críticos
  ✓ Performance OK
  ✓ Documentación presente
  → Aprobado o rechazado con feedback
```

### Responsabilidades
- 🔍 Code review automático
- ✅ Test execution
- 📊 Coverage analysis
- ⚡ Performance testing
- 🧹 Code smell detection
- 💡 Refactoring suggestions
- 📖 Documentation check

### Checklist de Review
```
CODE:
  [ ] Sigue convenciones proyecto
  [ ] Nombres descriptivos
  [ ] Sin duplicación
  [ ] Complejidad < 10
  [ ] Error handling presente
  [ ] DTOs en APIs (no raw entities)
  [ ] Logging agregado

TESTS:
  [ ] Todos pasan
  [ ] Cobertura > 80% (Backend) / 75% (Frontend)
  [ ] Casos normales cubiertos
  [ ] Edge cases validados
  [ ] Mocks para dependencias

PERFORMANCE:
  [ ] Queries optimizadas
  [ ] No N+1 queries
  [ ] Bundle size OK
  [ ] Rendering OK

DOCUMENTATION:
  [ ] Código comentado (si complejo)
  [ ] Tests descriptivos
  [ ] Swagger comentarios
  [ ] README actualizado
```

### Inputs Esperados
```
Automático en cada Pull Request

O manual:
"Revisa el código en rama feature/quests-system"
```

### Outputs Generados
```
Pull Request Review:
✅ APPROVED - Código listo
❌ CHANGES_REQUESTED - Mejoras necesarias
⏳ PENDING - Esperando cambios
```

---

## 6️⃣ SECURITY AUDITOR

### Identidad
```yaml
id: security-auditor
nombre: "Security Auditor"
tipo: "security"
descripcion: "Auditor de seguridad que verifica vulnerabilidades, secretos y compliance"
version: "1.0"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Auditar seguridad de código
- Detectar secretos commiteados
- Validar auth/authorization
- Auditar dependencias
- Verificar compliance

Automático: Después de CODE QUALITY GUARDIAN

Pregunta: "¿Es seguro este código?"
Security Auditor verificará:
  ✓ Sin hardcoded secrets
  ✓ Validación de entradas
  ✓ Autenticación correcta
  ✓ Sin dependencias vulnerables
  → Reporte de vulnerabilidades
```

### Responsabilidades
- 🔐 Security code review
- 🚨 Vulnerability scanning
- 🔑 Secret detection
- 📦 Dependency audit
- 🎯 OWASP Top 10
- ✅ Compliance check
- 📋 Security reporting

### Checklist de Seguridad
```
CÓDIGO:
  [ ] Sin hardcoded secrets
  [ ] Validación todas entradas
  [ ] Sanitización outputs
  [ ] Error handling sin info sensible
  [ ] JWT con expiration
  [ ] Passwords hasheados (bcrypt/PBKDF2)
  [ ] CORS configurado
  [ ] SQL injection prevention

DEPENDENCIAS:
  [ ] npm audit clean
  [ ] Dotnet vulnerabilities clean
  [ ] No deprecated packages
  [ ] Versiones conocidas seguras

INFRAESTRUCTURA:
  [ ] HTTPS en producción
  [ ] Rate limiting
  [ ] Logging sin datos sensibles
  [ ] Key Vault para secretos
  [ ] DB encryption
```

### Inputs Esperados
```
Automático después de Code Quality Guardian

O manual:
"Audita seguridad de nueva feature de pagos"
```

### Outputs Generados
```
Security Report:
✅ APPROVED - Sin vulnerabilidades
⚠️ WARNINGS - Revisar antes de deploy
❌ BLOCKED - Vulnerabilidades críticas encontradas

Detalles:
- Vulnerabilidades encontradas
- Severity (Critical/High/Medium/Low)
- Recomendaciones
- Links a recursos
```

---

## 7️⃣ DOCUMENTATION AGENT

### Identidad
```yaml
id: documentation-agent
nombre: "Documentation Agent"
tipo: "documentarian"
descripcion: "Agente que mantiene documentación sincronizada y actualizada"
version: "1.0"
```

### Cómo Usarlo
```
Usar cuando necesites:
- Crear nueva documentación
- Actualizar docs existentes
- Documentar APIs
- Crear guías de usuario
- Mantener README

Manual: Solicita updates a Documentation Agent

Pregunta: "Documenta el nuevo endpoint de quests"
Documentation Agent:
  → Actualiza ANALISIS_TECNICO.md
  → Agrega endpoint a API reference
  → Actualiza code comments
  → Verifica Swagger generado correctamente
  → Updates README si aplica
```

### Responsabilidades
- 📝 Markdown writing
- 📚 API documentation
- 👥 User guide creation
- 🎨 Architecture diagrams
- 📈 Change log
- 🔄 Sync con código
- 🎯 Clarity y completitud

### Documentos Mantenidos
```
/docs/
├── ANALISIS_FUNCIONAL.md      (Features)
├── ANALISIS_TECNICO.md        (Arquitectura, APIs)
├── AZURE_DEPLOYMENT.md        (Despliegue)
├── LOCAL_EXECUTION.md         (Setup local)
├── AGENTES.md                 (Este documento)
└── API_REFERENCE.md           (Endpoints detallados)

/client/
├── README.md                  (Frontend)
└── src/components/README.md   (Componentes)

README.md                       (Raíz - Overview)
GAME_GUIDE.md                   (Guía de juego)
```

### Template de Updates
```markdown
## Update Documentation

### Cambios en código:
- Nuevo endpoint POST /api/quests/{id}/accept

### Actualizar docs:
- [ ] ANALISIS_TECNICO.md - Sección APIs
- [ ] README.md - Features
- [ ] Swagger comments en código
- [ ] CHANGELOG.md - Agregar versión

### Archivos afectados:
- src/DndCopilot.Api/Controllers/QuestsController.cs
- docs/ANALISIS_TECNICO.md
```

### Inputs Esperados
```
Manual:
"Actualiza documentación para feature de Quests"

O automático:
Documentation Agent monitorea cambios
y propone actualizaciones
```

### Outputs Generados
```
Updated Documentation:
✅ Todos los docs sincronizados
✅ API Reference actualizado
✅ README updated
✅ Diagrams generados
✅ Ejemplos provistos
```

---

## 🔗 Matriz de Dependencias

```
FEATURE ANALYST
    ↓
DEV JEDI (Backend)  +  DEV JEDI (Frontend)
    ↓                       ↓
CODE QUALITY GUARDIAN (ambos)
    ↓
SECURITY AUDITOR
    ↓
DOCUMENTATION AGENT
    ↓
MATRIX BRAIN (Final approval)
```

---

## 📊 Qué Inspeccionar en cada Agente

| Agente | Inspecciona | Autoriza |
|--------|------------|----------|
| MATRIX BRAIN | Plan | Estrategia |
| FEATURE ANALYST | Requerimientos | Especificaciones |
| DEV JEDI Backend | Código C# | Implementación |
| DEV JEDI Frontend | Componentes React | UI/UX |
| CODE QUALITY | Tests, Coverage | Calidad |
| SECURITY AUDITOR | Vulnerabilidades | Seguridad |
| DOCUMENTATION | Docs sincronizadas | Documentación |

---

## 🚀 Cómo Invocar Cada Agente

### Via Text Command:
```
@matrix-brain: Crea plan para [FEATURE]
@feature-analyst: Analiza [REQUEST]
@devjedi-backend: Implementa [TASK]
@devjedi-frontend: Crea [COMPONENT]
@code-quality-guardian: Revisa PR #123
@security-auditor: Audita [CODE]
@documentation-agent: Documenta [FEATURE]
```

### Via GitHub Integration:
```
- PR automáticamente va a CODE QUALITY GUARDIAN
- Si pasa, va a SECURITY AUDITOR
- Si ambos aprueban, avisamos a DOCUMENTATION AGENT
- Todo reportamos a MATRIX BRAIN
```

---

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Última Actualización**: Febrero 2026
