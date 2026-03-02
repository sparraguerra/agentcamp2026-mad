# Agentes de Desarrollo - D&D Copilot

## 🤖 Descripción General de la Estrategia de Agentes

El proyecto **D&D Copilot** utiliza una arquitectura de agentes distribuida por equipos de desarrollo, todos coordinados por un **Agente Maestro Central (Matrix Brain)**. Este enfoque permite:

- ✅ **Paralelización**: Múltiples equipos trabajan simultáneamente
- ✅ **Especialización**: Cada agente tiene responsabilidades específicas
- ✅ **Coordinación**: El agente maestro orquesta interdependencias
- ✅ **Escalabilidad**: Fácil agregar nuevos agentes
- ✅ **Calidad**: Revisor de código y auditor de seguridad verifican cambios

---

## 🎯 Estructura de Agentes por Equipo

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│           MATRIX BRAIN (Agente Maestro)                   │
│        Orquestación, Planificación, Decisiones             │
│                                                             │
└────────┬────────────────────┬────────────────┬─────────────┘
         │                    │                │
    ┌────▼──────┐    ┌────────▼────────┐  ┌───▼──────────┐
    │   BACKEND │    │    FRONTEND     │  │ INFRAESTRUCTURA
    │   EQUIPO  │    │     EQUIPO      │  │    EQUIPO
    │           │    │                 │  │
    └────┬──────┘    └────────┬────────┘  └───┬──────────┘
         │                    │                │
    ┌────▼──────────────┐ ┌───▼──────────────┐│
    │  DEV JEDI (Impl)  │ │ FEATURE ANALYST  ││
    │  (Backend)        │ │ (Análisis)       ││
    └────┬──────────────┘ └──────────────────┘│
         │                                     │
    ┌────▼──────────────────┐                 │
    │ CODE QUALITY GUARDIAN │                 │
    │ (QA + Testing)        │                 │
    └────┬──────────────────┘                 │
         │                  ┌──────────────────┘
    ┌────▼──────────────────────────────┐
    │  SECURITY AUDITOR                 │
    │  (Auditoría Seguridad)            │
    └────────────────────────────────────┘
         │
    ┌────▼──────────────────────────────┐
    │  DOCUMENTATION AGENT              │
    │  (Documentación Sincronizada)     │
    └────────────────────────────────────┘
```

---

## 📋 Agentes Detallados

### 1. 🧠 MATRIX BRAIN (Agente Maestro)

**Rol**: Orquestador central, toma decisiones estratégicas

**Responsabilidades**:
- Recibir solicitudes de usuario
- Analizar complejidad y dependencias
- Diseñar plan de trabajo
- Asignar tareas a agentes especializados
- Coordinar parallelización
- Priorizar trabajos
- Tomar decisiones de arquitectura
- Escalar issues a nivel estratégico

**Interacciones**:
- ⬇️ **Inicia**: FEATURE ANALYST, DEV JEDI (backend/frontend)
- ⬇️ **Espera**: Reporte de SECURITY AUDITOR
- ⬇️ **Coordina**: DOCUMENTATION AGENT para actualizar docs

**Ejemplo de Tarea**:
```
Usuario: "Implementar nuevo sistema de quest"
│
├─ MATRIX BRAIN:
│  ├─ Analizar impacto (BD, API, Frontend)
│  ├─ Crear plan:
│  │  1. FEATURE ANALYST: Descomponer en subtareas
│  │  2. DEV JEDI (Backend): Entidades, Controllers
│  │  3. DEV JEDI (Frontend): UI componentes
│  │  4. CODE QUALITY GUARDIAN: Tests y calidad
│  │  5. SECURITY AUDITOR: Validar seguridad
│  │  6. DOCUMENTATION AGENT: Actualizar docs
│  └─ Orquestar ejecución paralela
```

---

### 2. 📊 FEATURE ANALYST

**Rol**: Analista de funcionalidades y descomposición de historias

**Responsabilidades**:
- Recibir feature requests o issues
- Analizar requisitos funcionales
- Descomponer en historias de usuario
- Identificar dependencias técnicas
- Estimar complejidad
- Priorizar tareas
- Crear especificaciones técnicas detalladas
- Identificar riesgos

**Interacciones**:
- ⬆️ **Recibe de**: MATRIX BRAIN
- ⬇️ **Entrega a**: DEV JEDI, CODE QUALITY GUARDIAN
- **Herramientas**: Análisis, descomposición, requisitos

**Ejemplo de Salida**:
```markdown
## Feature: Sistema de Quests Multi-etapa

### Historias de Usuario:
1. "Como jugador, quiero aceptar quests para ganar XP"
   - Complejidad: MEDIA
   - Dependencias: Sistema de experiencia
   - Tests necesarios: 5

2. "Como admin, quiero crear quests con múltiples etapas"
   - Complejidad: ALTA
   - Dependencias: Entidad Quest, API, UI
   - Tests necesarios: 8

### Tareas Backend:
- [ ] Crear entidad Quest.cs
- [ ] Crear entidad QuestStage.cs
- [ ] QuestController CRUD
- [ ] QuestService lógica
- [ ] Migrations BD

### Tareas Frontend:
- [ ] QuestList componente
- [ ] QuestDetail componente
- [ ] QuestAccept button
- [ ] QuestProgress display
```

---

### 3. 💻 DEV JEDI (Backend)

**Rol**: Implementador de lógica backend

**Responsabilidades**:
- Implementar controllers y endpoints API
- Crear servicios de negocio
- Implementar repositorios
- Escribir migrations BD
- Manejo de errores y validaciones
- Performance optimization
- Integración con servicios externos

**Especialización**: C#, .NET, Entity Framework, SQL

**Interacciones**:
- ⬆️ **Recibe de**: FEATURE ANALYST, CODE QUALITY GUARDIAN
- ⬇️ **Envía a**: CODE QUALITY GUARDIAN (para review)
- **Equipo**: Backend

**Archivo de Tareas Backend**
```yaml
Equipo: Backend DEV JEDI
Stack: C# / .NET 9.0 / EF Core / SQL

Projects:
  - DndCopilot.Api
  - DndCopilot.Core
  - DndCopilot.Infrastructure

Responsabilidades:
  - Controllers REST
  - Services (CombatService, DiceRoller, GameService)
  - Entidades del Dominio (Entity Framework)
  - Repositorios (Data Access)
  - Migrations BD
  - Unit Tests (.NET)
  - Validaciones y error handling
```

---

### 4. 💻 DEV JEDI (Frontend)

**Rol**: Implementador de UI/UX

**Responsabilidades**:
- Crear componentes React funcionales
- Implementar páginas y layouts
- Integrar llamadas API
- Manejo de estado con hooks
- Estilos CSS responsivos
- Accesibilidad (a11y)
- Performance de renderizado

**Especialización**: TypeScript, React, CSS, Axios

**Interacciones**:
- ⬆️ **Recibe de**: FEATURE ANALYST, CODE QUALITY GUARDIAN
- ⬇️ **Envía a**: CODE QUALITY GUARDIAN (para review)
- **Equipo**: Frontend

**Archivo de Tareas Frontend**
```yaml
Equipo: Frontend DEV JEDI
Stack: React 18 / TypeScript / CSS3 / Axios

Projects:
  - client/src/components
  - client/src/pages
  - client/src/services

Responsabilidades:
  - Componentes React reutilizables
  - Páginas (Login, Dashboard, Game)
  - Integración API (api.ts)
  - Estilos CSS (responsive, dark mode)
  - Tipos TypeScript
  - Tests React (.test.tsx)
  - Accesibilidad WCAG
```

---

### 5. 🛡️ CODE QUALITY GUARDIAN

**Rol**: Verificador de calidad y testing

**Responsabilidades**:
- Revisar código de DEV JEDI
- Ejecutar tests automáticos
- Verificar cobertura de tests
- Proponer mejoras de código
- Refactorización si es necesario
- Validar performance
- Linting y formateo
- Documentación de código

**Herramientas**: 
- Backend: NUnit, Moq, CodeQL
- Frontend: Jest, React Testing Library, ESLint

**Interacciones**:
- ⬆️ **Recibe de**: DEV JEDI (ambos equipos)
- ⬇️ **Aprueba/Rechaza**: A SECURITY AUDITOR
- **Bloquea merge**: Si tests fallan

**Checklist de Revisión**:
```
Code Review:
- [ ] Código sigue convenciones de proyecto
- [ ] No hay duplicación de código
- [ ] Nombres descriptivos de variables/métodos
- [ ] Complejidad ciclomática razonable (< 10)
- [ ] Manejo de excepciones correcto
- [ ] DTOs usados en APIs (no entidades raw)

Testing:
- [ ] Todos los tests pasan
- [ ] Cobertura mínima 80%
- [ ] Tests para lógica crítica (combate, dados)
- [ ] Tests edge cases
- [ ] Mocks para dependencias externas

Performance:
- [ ] Queries optimizadas (sin N+1)
- [ ] Caching implementado donde aplique
- [ ] Tamaño de bundle aceptable (< 500KB)
- [ ] Rendering performance OK

Security:
- [ ] Sin hardcoded secrets
- [ ] Validación de entrada
- [ ] SQL injection prevention
- [ ] XSS protection
```

---

### 6. 🔐 SECURITY AUDITOR

**Rol**: Audita seguridad del código y dependencias

**Responsabilidades**:
- Revisar security del código
- Auditar dependencias vulnerables
- Verificar secretos commiteados
- Validar autenticación/autorización
- Revisar CORS, HTTPS, enc datos
-_penetration testing conceptual
- Reportar vulnerabilidades
- Proponer fixes de seguridad

**Herramientas**:
- CodeQL, Snyk, OWASP Top 10
- Semantic analysis

**Interacciones**:
- ⬆️ **Recibe de**: CODE QUALITY GUARDIAN
- ⬇️ **Aprueba/Rechaza** cambios críticos
- ⬇️ **Informa a**: MATRIX BRAIN de vulnerabilidades

**Checklist de Seguridad**:
```
Código:
- [ ] No secrets en código fuente
- [ ] Validación de todas las entradas
- [ ] Sanitización de outputs
- [ ] Error handling sin info sensible
- [ ] JWT tokens con exp time
- [ ] Password hashing (bcrypt/PBKDF2)

Dependencias:
- [ ] npm audit clean (Frontend)
- [ ] No dependencias deprecated
- [ ] Versiones vulnerables reportadas
- [ ] Lockfile commiteado

Infraestructura:
- [ ] CORS configurado estrictamente
- [ ] HTTPS habilitado producción
- [ ] Rate limiting implementado
- [ ] Logging sin datos sensibles
- [ ] Key Vault para secrets Azure
```

---

### 7. 📚 DOCUMENTATION AGENT

**Rol**: Mantiene documentación sincronizada

**Responsabilidades**:
- Actualizar docs cuando hay cambios
- Mantener README actualizado
- Documentar APIs (Swagger)
- Crear guías de usuario
- Actualizar guías de desarrollo
- Mantener diagramas arquitectura
- Crear tutoriales
- Documentar decisiones (ADR)

**Formatos**: Markdown, OpenAPI/Swagger, Mermaid

**Interacciones**:
- ⬆️ **Recibe solicitudes de**: MATRIX BRAIN
- ⬆️ **Monitorea cambios**: Todos los agentes
- ⬇️ **Genera/Actualiza**: Docs en carpeta `/docs`

**Documentos a Mantener**:
```
/docs/
├── ANALISIS_FUNCIONAL.md  (Features, casos uso)
├── ANALISIS_TECNICO.md    (Arquitectura, APIs, BD)
├── AZURE_DEPLOYMENT.md    (Despliegue Azure)
├── LOCAL_EXECUTION.md     (Setup local)
├── AGENTES.md             (Este archivo)
└── API_REFERENCE.md       (Endpoints)

/client/
└── README.md & components/README.md

README.md (raíz)     (Overview general)
GAME_GUIDE.md        (Guía de juego)
```

---

## 🔄 Flujos de Trabajo

### Flujo 1: Nueva Característica

```
1. Usuario → MATRIX BRAIN
   "Implementar sistema de logros"

2. MATRIX BRAIN
   - Analiza complejidad
   - Crea plan
   - Estima tiempo
   - Status: "PLANNING"

3. MATRIX BRAIN → FEATURE ANALYST
   "Descomponer feature: Logros"

4. FEATURE ANALYST
   - Especificación detallada
   - Historias de usuario
   - Subtareas identificadas
   - Status: "SPECIFIED"
   → Devuelve a MATRIX BRAIN

5. MATRIX BRAIN → DEV JEDI (Backend + Frontend en paralelo)
   "Implementar logros"

6. DEV JEDI (Backend)
   - Crea entidades (Achievement, PlayerAchievement)
   - Controllers: GetAchievements, UnlockAchievement
   - Services: AchievementService
   - Tests unitarios
   → Lee: CODE QUALITY GUARDIAN

7. DEV JEDI (Frontend)
   - Componentes: AchievementBadge, AchievementList
   - Página: AchievementsPage
   - Integración API
   - Tests React
   → Lee: CODE QUALITY GUARDIAN

8. CODE QUALITY GUARDIAN (Backend)
   - Revisa código
   - Ejecuta tests
   - Verifica cobertura (>80%)
   - Si OK → SECURITY AUDITOR
   - Si NO → Feedback a DEV JEDI

9. CODE QUALITY GUARDIAN (Frontend)
   - Revisa componentes
   - Tests para todos los paths
   - Performance check
   - Accesibilidad verificada
   - Si OK → SECURITY AUDITOR
   - Si NO → Feedback a DEV JEDI

10. SECURITY AUDITOR
    - Revisa ambos cambios
    - Valida auth/permisos
    - Audita dependencias
    - Si OK → Aprueba merge
    - Si NO → Requiere fixes

11. DOCUMENTATION AGENT
    - Actualiza ANALISIS_FUNCIONAL.md
    - Documenta nuevos endpoints
    - Crea guía de logros
    - Actualiza README

12. MATRIX BRAIN
    - Coordina merge de todas las ramas
    - Verifica tests CI/CD
    - Status: "COMPLETED"
```

### Flujo 2: Bug Fix Crítico

```
1. Usuario → MATRIX BRAIN
   "API retorna 500 en endpoint /combat/attack"

2. MATRIX BRAIN (URGENT)
   - Máxima prioridad
   - Asigna DEV JEDI backend inmediatamente
   - Status: "CRITICAL_HOTFIX"

3. DEV JEDI (Backend)
   - Reproduce bug localmente
   - Identifica causa (ej: null exception)
   - Escribe fix minimal
   - Adds tests para prevenir regresión

4. CODE QUALITY GUARDIAN
   - Revisa fix urgente
   - Verifica tests pasan
   - Aprueba con fast-track

5. SECURITY AUDITOR
   - Quick security check
   - Aprueba o rechaza

6. MATRIX BRAIN
   - Merge a main inmediatamente
   - Redeploy a producción
   - Status: "HOTFIX_DEPLOYED"

7. DOCUMENTATION AGENT
   - Documenta qué fue el bug
   - Crea post mortem (si es severo)
```

### Flujo 3: Review de Código

```
DEV JEDI commits → GitHub PR
        ↓
CODE QUALITY GUARDIAN (Automático)
  - Revisa cambios
  - Ejecuta tests
  - Calcula cobertura
  - Lint check
  - Si alguno falla → Rechaza PR
        ↓
SECURITY AUDITOR (Automático)
  - CodeQL scan
  - Dependency audit
  - Si vulnerabilidad → Bloquea PR
        ↓
MATRIX BRAIN o REPOSITORY MASTER
  - Aprobación final
  - Merge a main
        ↓
CI/CD: GitHub Actions
  - Build
  - Tests en sandbox
  - Deploy automático
```

---

## ⚙️ Configuración por Agente

### MATRIX BRAIN Config
```yaml
name: "Matrix Brain"
role: "orchestra"
capabilities:
  - analyze_complexity
  - create_plans
  - prioritize_work
  - coordinate_agents
  - escalate_issues
  - architectural_decisions

kpis:
  - project_velocity
  - team_satisfaction
  - on_time_delivery
  - critical_issues_resolved
```

### FEATURE ANALYST Config
```yaml
name: "Feature Analyst"
role: "analyzer"
capabilities:
  - requirement_gathering
  - user_story_creation
  - dependency_mapping
  - complexity_estimation
  - risk_identification
  - specification_writing

outputs:
  - user_stories.md
  - technical_specs.md
  - task_breakdown.md
  - dependency_graph.md
```

### DEV JEDI Backend Config
```yaml
name: "Dev Jedi (Backend)"
role: "developer"
tech_stack:
  - ".NET 9.0"
  - "C#"
  - "Entity Framework Core"
  - "Azure SQL / SQLite"
  - "NUnit / Moq"

folders:
  - "src/DndCopilot.Api"
  - "src/DndCopilot.Core"
  - "src/DndCopilot.Infrastructure"
  - "tests/"

quality_gates:
  - tests_pass: true
  - coverage: ">80%"
  - static_analysis: "no critical"
  - code_review: "approved"
```

### DEV JEDI Frontend Config
```yaml
name: "Dev Jedi (Frontend)"
role: "developer"
tech_stack:
  - "React 18"
  - "TypeScript"
  - "CSS3"
  - "Axios"
  - "Jest / React Testing Library"

folders:
  - "client/src/components"
  - "client/src/pages"
  - "client/src/services"

quality_gates:
  - tests_pass: true
  - coverage: ">75%"
  - lighthouse_score: ">85"
  - accessibility: "WCAG AA"
  - linting: "no errors"
```

### CODE QUALITY GUARDIAN Config
```yaml
name: "Code Quality Guardian"
role: "qa"
capabilities:
  - code_review
  - test_execution
  - coverage_analysis
  - performance_testing
  - refactoring_suggestion
  - documentation_check

tools:
  - SonarQube / CodeQL
  - Coverage.py / Coverlet
  - Lighthouse
  - ESLint / StyleCop

standards:
  - "SonarQube A (or higher)"
  - "Test coverage: >80%"
  - "No code smells"
  - "Performance: <100ms for API"
```

### SECURITY AUDITOR Config
```yaml
name: "Security Auditor"
role: "security"
capabilities:
  - code_security_review
  - dependency_audit
  - secret_detection
  - threat_modeling
  - vulnerability_assessment
  - compliance_check

tools:
  - CodeQL
  - Snyk
  - OWASP Dependency Check
  - TruffleHog

standards:
  - "OWASP Top 10"
  - "Zero critical vulnerabilities"
  - "No hardcoded secrets"
  - "JWT properly implemented"
  - "CORS configured"
```

### DOCUMENTATION AGENT Config
```yaml
name: "Documentation Agent"
role: "documentation"
capabilities:
  - markdown_writing
  - api_documentation
  - user_guide_creation
  - architecture_diagramming
  - change_log_maintenance

formats:
  - Markdown (.md)
  - OpenAPI 3.0 (Swagger)
  - Mermaid diagrams
  - PlantUML

documents:
  - ANALISIS_FUNCIONAL.md
  - ANALISIS_TECNICO.md
  - AZURE_DEPLOYMENT.md
  - LOCAL_EXECUTION.md
  - README files
  - API_REFERENCE.md
```

---

## 📞 Canales de Comunicación

```
Dev -> Code Quality -> Security -> Matrix Brain
 ↓       ↓              ↓          ↓
Pull    Automated      Automated  Final
Request Review         Scan       Approval

     ↓ Aprobado ↓
     
    GitHub Branch
         ↓
    CI/CD Pipeline
         ↓
    Deploy
         ↓
    DOCUMENTATION AGENT
    Actualiza docs
```

---

## 📊 Dashboard de Estados

```
MATRIZ BRAIN
├─ Backend DEV JEDI: 8/12 tasks (67%) 🔄 EN PROGRESO
│  ├─ Crear entidades Quest: ✅ DONE
│  ├─ QuestController: 🔄 IN PROGRESS
│  ├─ QuestService: ⏳ BLOCKED (espera DB schema)
│  └─ ...
├─ Frontend DEV JEDI: 10/14 tasks (71%) 🔄 EN PROGRESO
│  ├─ QuestList component: ✅ DONE
│  ├─ QuestDetail component: 🔄 IN PROGRESS
│  └─ ...
├─ CODE QUALITY GUARDIAN: ✅ Reviewing Backend
├─ SECURITY AUDITOR: ⏳ Waiting for code review
└─ DOCUMENTATION AGENT: ✅ Docs updated
```

---

## 🎯 Métricas de Éxito

| Métrica | Target | Actual |
|---------|--------|--------|
| Tasks on time | 95% | - |
| Code coverage | > 80% | - |
| PR review time | < 2h | - |
| Security issues | 0 Critical | - |
| Documentation updated | 100% | - |
| Team velocity | baseline + 10% | - |

---

## 🚀 Cómo Usar Esta Estructura

### Para el Usuario (Tu):
```
Usuario: "Quiero agregar tarea [X]"
         ↓
         MATRIX BRAIN
         (Orquestador)
         ↓
    Revisa el plan del proyecto
    Coordina equipos
    Reporta progreso
```

### Para Cada Agente:
```
Agente recibe: Tarea de MATRIX BRAIN
Agente hace: Su trabajo especializado
Agente entrega: Código/Doc de calidad
Agente reporta: Status a MATRIX BRAIN
```

---

## 📚 Referencia Rápida

| Necesito | Contactar | Esperar |
|----------|-----------|---------|
| Nuevos Análisis | FEATURE ANALYST | Especificación |
| Implementar Backend | DEV JEDI Backend | Código + Tests |
| Implementar Frontend | DEV JEDI Frontend | Componentes + Tests |
| Revisar Calidad | CODE QUALITY GUARDIAN | Aprobación | |
| Revisar Seguridad | SECURITY AUDITOR | Aprobación |
| Actualizar Docs | DOCUMENTATION AGENT | Docs sincronizadas |
| Decisión Estratégica | MATRIX BRAIN | Dirección |

---

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Propietario**: MATRIX BRAIN  
**Última Actualización**: Febrero 2026
