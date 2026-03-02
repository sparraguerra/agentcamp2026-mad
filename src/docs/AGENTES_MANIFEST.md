# Manifest de Agentes - D&D Copilot

## 📋 Resumen Ejecutivo

Este documento lista todos los agentes disponibles del proyecto D&D Copilot, sus responsabilidades, cómo invocarlos y qué esperar de cada uno.

---

## 🤖 Agentes Disponibles

### 1. 🧠 MATRIX BRAIN
**Type**: Orchestrator / Master Agent  
**Status**: ✅ Activo  
**Versión**: 1.0

#### Quick Info
- **Cuándo usarlo**: Planificación compleja, decisiones estratégicas
- **No usarlo para**: Tareas específicas de código
- **Tiempo de respuesta**: 5-10 min
- **Output**: Plan detallado, asignaciones, timeline

#### Cómo Invocarlo
```
@matrix-brain: [Tu solicitud compleja aquí]

Ejemplo:
"Necesito implementar un sistema de tienda donde 
los jugadores puedan comprar/vender items"
```

#### Qué Esperar
- ✅ Análisis de complejidad
- ✅ Plan paso-a-paso
- ✅ Asignaciones de agentes
- ✅ Timeline estimada
- ✅ Riesgos identificados
- ✅ Mitigation strategies

---

### 2. 📊 FEATURE ANALYST
**Type**: Analyzer  
**Status**: ✅ Activo  
**Versión**: 1.0

#### Quick Info
- **Cuándo usarlo**: Descomponer features, crear user stories
- **No usarlo para**: Implementación de código
- **Tiempo de respuesta**: 3-5 min
- **Output**: User stories, especificaciones técnicas

#### Cómo Invocarlo
```
@feature-analyst: Analiza [feature description]

Ejemplo:
"Sistema de achievement badges que se ganan 
al cumplir ciertos objetivos"
```

#### Qué Esperar
- ✅ Historias de usuario detalladas
- ✅ Especificación técnica
- ✅ Tareas desglosadas
- ✅ Dependencias mapeadas
- ✅ Estimación de complejidad

---

### 3. 💻 DEV JEDI (Backend)
**Type**: Developer / Backend  
**Status**: ✅ Activo  
**Versión**: 1.0  
**Stack**: C#, .NET 9.0, EF Core, SQL

#### Quick Info
- **Cuándo usarlo**: Implementar lógica backend, APIs, BD
- **No usarlo para**: Frontend, decisiones de arquitectura
- **Tiempo de respuesta**: 10-20 min
- **Output**: Código testeable, listo para review

#### Cómo Invocarlo
```
@devjedi-backend: Implementa [task description]

Ejemplo:
"Crea endpoint POST /api/achievements/unlock 
que se ejecuta cuando jugador cumple objetivo"
```

#### Qué Esperar
- ✅ Código C# limpio
- ✅ Unit tests NUnit
- ✅ Migrations BD
- ✅ Error handling
- ✅ Logging
- ✅ Código listo para CODE QUALITY GUARDIAN

---

### 4. 💻 DEV JEDI (Frontend)
**Type**: Developer / Frontend  
**Status**: ✅ Activo  
**Versión**: 1.0  
**Stack**: React 18, TypeScript, CSS3, Axios

#### Quick Info
- **Cuándo usarlo**: Crear componentes, UI, integración API
- **No usarlo para**: Lógica backend, decisiones DB
- **Tiempo de respuesta**: 10-20 min
- **Output**: Componentes React con tests

#### Cómo Invocarlo
```
@devjedi-frontend: Implementa [component description]

Ejemplo:
"Componente AchievementBadge que muestre 
icono, nombre y descripción del achievement"
```

#### Qué Esperar
- ✅ Componente React funcional
- ✅ TypeScript types
- ✅ Tests React
- ✅ Estilos CSS responsive
- ✅ Accesibilidad WCAG
- ✅ Componente listo para CODE QUALITY GUARDIAN

---

### 5. 🛡️ CODE QUALITY GUARDIAN
**Type**: QA / Reviewer  
**Status**: ✅ Activo  
**Versión**: 1.0

#### Quick Info
- **Cuándo usarlo**: Revisar código antes de merge
- **Automático**: En cada Pull Request
- **Tiempo de respuesta**: Automático (< 5 min)
- **Output**: Aprobación o feedback detallado

#### Cómo Invocarlo
```
Automático en GitHub Pull Request

O manual:
@code-quality-guardian: Revisa PR #[number]
```

#### Qué Verifica
- ✅ Todos los tests pasan
- ✅ Coverage > 80% (Backend) / 75% (Frontend)
- ✅ Sin code smells
- ✅ Performance OK
- ✅ Documentación presente
- ✅ Convenciones de código

#### Estados Posibles
- ✅ **APPROVED** - Código listo para merge
- ❌ **CHANGES_REQUESTED** - Mejoras necesarias
- ⏳ **PENDING** - Esperando cambios del developer

---

### 6. 🔐 SECURITY AUDITOR
**Type**: Security Inspector  
**Status**: ✅ Activo  
**Versión**: 1.0

#### Quick Info
- **Cuándo usarlo**: Auditar seguridad de código/dependencias
- **Automático**: Después de CODE QUALITY GUARDIAN
- **Tiempo de respuesta**: Automático (< 5 min)
- **Output**: Reporte de vulnerabilidades

#### Cómo Invocarlo
```
Automático en Pull Request (post CODE QUALITY)

O manual:
@security-auditor: Audita [code/branch]
```

#### Qué Verifica
- ✅ Sin hardcoded secrets
- ✅ Validación de entradas
- ✅ Sin dependencias vulnerables
- ✅ JWT correctamente implementado
- ✅ CORS configurado
- ✅ Password hashing
- ✅ OWASP Top 10

#### Estados Posibles
- ✅ **APPROVED** - Sin vulnerabilidades
- ⚠️ **WARNINGS** - Revisar antes de deploy
- ❌ **BLOCKED** - Vulnerabilidades críticas

---

### 7. 📚 DOCUMENTATION AGENT
**Type**: Documentarian  
**Status**: ✅ Activo  
**Versión**: 1.0

#### Quick Info
- **Cuándo usarlo**: Crear/actualizar documentación
- **Manual**: Solicita updates directamente
- **Tiempo de respuesta**: 3-5 min
- **Output**: Documentación actualizada

#### Cómo Invocarlo
```
@documentation-agent: Documenta [feature/change]

Ejemplo:
"Documenta nuevo endpoint POST /api/achievements/unlock 
en ANALISIS_TECNICO.md"
```

#### Qué Proporciona
- ✅ Documentación Markdown
- ✅ Diagramas Mermaid
- ✅ API Reference actualizada
- ✅ README actualizado
- ✅ Examples en código
- ✅ Swagger comments

#### Documentos Mantenidos
```
- ANALISIS_FUNCIONAL.md
- ANALISIS_TECNICO.md
- AGENTES.md
- AZURE_DEPLOYMENT.md
- LOCAL_EXECUTION.md
- API_REFERENCE.md
- README files
```

---

## 🔄 Flujo de Agentes

### Para Features Nuevas
```
MATRIX BRAIN (Planificación)
    ↓
FEATURE ANALYST (Especificación)
    ↓
DEV JEDI Backend + DEV JEDI Frontend (en paralelo)
    ↓
CODE QUALITY GUARDIAN (Revisión)
    ↓
SECURITY AUDITOR (Auditoría)
    ↓
DOCUMENTATION AGENT (Docs)
    ↓
MATRIX BRAIN (Aprobación Final)
```

### Para Bug Fixes
```
MATRIX BRAIN (Priorización)
    ↓
DEV JEDI (Backend o Frontend según necesite)
    ↓
CODE QUALITY GUARDIAN (Rápido)
    ↓
SECURITY AUDITOR (Rápido)
    ↓
Deploy a Producción
    ↓
DOCUMENTATION AGENT (Post-mortem si es grave)
```

---

## 📊 Matriz de Responsabilidades

| Responsabilidad | Agente | Backup |
|-----------------|--------|--------|
| Planificación | MATRIX BRAIN | FEATURE ANALYST |
| Análisis | FEATURE ANALYST | - |
| Impl. Backend | DEV JEDI Backend | - |
| Impl. Frontend | DEV JEDI Frontend | - |
| QA/Testing | CODE QUALITY GUARDIAN | - |
| Seguridad | SECURITY AUDITOR | CODE QUALITY |
| Documentación | DOCUMENTATION AGENT | - |
| Escalamiento | MATRIX BRAIN | - |

---

## 🎯 Cuándo Usar Cada Agente

### Necesito Planificar una Feature Grande
→ **MATRIX BRAIN** → **FEATURE ANALYST**

### Necesito Crear un Endpoint API
→ **FEATURE ANALYST** → **DEV JEDI Backend** → **CODE QUALITY**

### Necesito Crear un Componente
→ **FEATURE ANALYST** → **DEV JEDI Frontend** → **CODE QUALITY**

### Quiero Revisar Código Antes de Merge
→ **CODE QUALITY GUARDIAN** → **SECURITY AUDITOR**

### Necesito Auditar Seguridad
→ **SECURITY AUDITOR**

### Debo Actualizar Documentación
→ **DOCUMENTATION AGENT**

### Todo Está Listo, Necesito Aprobación Final
→ **MATRIX BRAIN**

---

## 📞 Cómo Contactar Agentes

### Vía Chat/Prompt
```
@nombre-agente: [Tu solicitud/pregunta]

Ejemplo:
@devjedi-backend: Implementa nuevo endpoint 
para obtener estadísticas del jugador
```

### Vía Git/PR
```
# El flujo automático invoca agentes en orden:
1. Abres Pull Request
2. CODE QUALITY GUARDIAN revisa automático
3. Si pasa, SECURITY AUDITOR revisa
4. Si todo OK, avisamos a DOCUMENTATION AGENT
5. MATRIX BRAIN da aprobación final
```

### Vía Comando Directo
```bash
# Backend implementation
@devjedi-backend: Implementa [task]

# Frontend implementation
@devjedi-frontend: Crea componente [name]

# análisis
@feature-analyst: Analiza [feature]

# Review
@code-quality-guardian: Revisa código [branch/pr]

# Security check
@security-auditor: Audita [code]

# Documentación
@documentation-agent: Documenta [change]
```

---

## ⏱️ SLA (Service Level Agreement)

| Agente | Respuesta Rápida | Respuesta Normal | Respuesta Lenta |
|--------|-----------------|-----------------|-----------------|
| MATRIX BRAIN | < 10 min | 10-20 min | 20-30 min |
| FEATURE ANALYST | < 5 min | 5-10 min | 10-15 min |
| DEV JEDI Backend | < 20 min | 20-40 min | 40-60 min |
| DEV JEDI Frontend | < 20 min | 20-40 min | 40-60 min |
| CODE QUALITY | < 5 min (auto) | 5-10 min | 10-15 min |
| SECURITY AUDITOR | < 5 min (auto) | 5-10 min | 10-15 min |
| DOCUMENTATION | < 5 min | 5-10 min | 10-15 min |

---

## ✅ Quality Gates de Cada Agente

### FEATURE ANALYST
```
✅ OUT: User stories claras
✅ OUT: Especificación técnica detallada
✅ OUT: Tareas desglosadas
✅ OUT: Dependencias mapeadas
❌ NO: Implementación de código
```

### DEV JEDI Backend
```
✅ OUT: Código C# compilable
✅ OUT: Tests unitarios > 80%
✅ OUT: Migrations BD
✅ OUT: Error handling
❌ FALLA: Si tests no pasan
❌ FALLA: Si coverage < 80%
```

### DEV JEDI Frontend
```
✅ OUT: Componente React funcional
✅ OUT: Tests React > 75%
✅ OUT: CSS responsive
✅ OUT: Accesibilidad WCAG AA
❌ FALLA: Si ESLint tiene errores
❌ FALLA: Si tests no pasan
```

### CODE QUALITY GUARDIAN
```
✅ APROBADO: Todos tests pasan + Coverage OK
✅ APROBADO: Sin code smells
✅ APROBADO: Performance OK
❌ RECHAZADO: Tests fallando
❌ RECHAZADO: Coverage bajo
❌ RECHAZADO: Code smells detectados
```

### SECURITY AUDITOR
```
✅ APROBADO: Sin vulnerabilidades
⚠️ WARNINGS: Issues no-críticos
❌ BLOQUEADO: Vulnerabilidades críticas
❌ BLOQUEADO: Hardcoded secrets
❌ BLOQUEADO: Dependencias maliciosas
```

### DOCUMENTATION AGENT
```
✅ COMPLETADO: Docs sincronizadas
✅ COMPLETADO: README actualizado
✅ COMPLETADO: API docs actalizadas
✅ COMPLETADO: Diagramas generados
❌ INCOMPLETO: Si falta documentación crítica
```

---

## 🎓 Training & Onboarding

### Para Nuevo Miembro del Equipo
```
1. Lee: docs/README.md (Índice)
2. Lee: .copilot-instructions.md (Convenciones)
3. Lee: docs/ANALISIS_TECNICO.md (Arquitectura)
4. Ejecuta: docs/LOCAL_EXECUTION.md (Setup)
5. Lee: docs/AGENTES.md (Cómo funcionan agentes)
6. Practica: Invoca un agente para tarea simple
```

### Para Integración de Nuevo Agente
```
1. Define identidad (nombre, tipo, stack)
2. Documenta en AGENTES_CONFIG.md
3. Crea quality gates específicos
4. Integra en flujo de CI/CD
5. Añade a documentación
6. Entrena al equipo
```

---

## 📊 Estadísticas de Agentes

| Agente | Tipo | Tareas/Semana (Est.) | Success Rate |
|--------|------|---------------------|--------------|
| MATRIX BRAIN | Orchestrator | 3-5 | 99% |
| FEATURE ANALYST | Analyzer | 5-10 | 98% |
| DEV JEDI Backend | Developer | 20-30 | 95% |
| DEV JEDI Frontend | Developer | 20-30 | 95% |
| CODE QUALITY GUARDIAN | QA | 50-100 | 100% (auto) |
| SECURITY AUDITOR | Security | 50-100 | 100% (auto) |
| DOCUMENTATION AGENT | Documentarian | 10-20 | 98% |

---

## 🔗 Links Relacionados

- 📖 [Documentación Agentes Detallada](./AGENTES.md)
- ⚙️ [Configuración de Agentes](./AGENTES_CONFIG.md)
- 📚 [Índice de Documentación](./README.md)
- 🏗️ [Arquitectura Técnica](./ANALISIS_TECNICO.md)
- 🎮 [Análisis Funcional](./ANALISIS_FUNCIONAL.md)

---

## 📞 Contacto

Para preguntas sobre agentes:
- Consulta [AGENTES.md](./AGENTES.md) para descripción detallada
- Consulta [AGENTES_CONFIG.md](./AGENTES_CONFIG.md) para configuración
- Invoca a MATRIX BRAIN para orquestación compleja

---

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Última Actualización**: Febrero 2026  
**Próxima Revisión**: Abril 2026
