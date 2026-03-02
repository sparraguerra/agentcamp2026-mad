# Documentación - D&D Copilot

## 📚 Índice de Documentación

Bienvenido a la documentación del proyecto **D&D Copilot**. Esta carpeta contiene toda la información técnica y funcional necesaria para entender, desarrollar y desplegar el proyecto.

---

## 🎯 Documentos por Rol

### 👤 Si Eres Usuario/Gamer
1. **[GAME_GUIDE.md](../GAME_GUIDE.md)** - Guía completa de cómo jugar
   - Mecánicas de combate
   - Descripción de localizaciones
   - Tips estratégicos
   - Condiciones de victoria

### 💻 Si Eres Desarrollador

#### 📖 Comenzar
1. **[LOCAL_EXECUTION.md](./LOCAL_EXECUTION.md)** - Cómo ejecutar el proyecto localmente
   - Requisitos previos
   - Instalación paso-a-paso
   - Configuración
   - Troubleshooting

2. **[.copilot-instructions.md](../.copilot-instructions.md)** - Guía de Copilot para el proyecto
   - Arquitectura
   - Convenciones de código
   - Patrones
   - Stack tecnológico

#### 🔍 Entender el Proyecto
3. **[ANALISIS_FUNCIONAL.md](./ANALISIS_FUNCIONAL.md)** - Qué hace el proyecto
   - Features principales
   - Funcionalidades por módulo
   - Flujos de negocio
   - Casos de uso
   - Métricas de éxito
   
   **Leer si**: Necesitas entender qué hace cada parte

4. **[ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md)** - Cómo está construido
   - Arquitectura de capas
   - Stack tecnológico
   - Estructura de carpetas
   - Modelo de datos
   - APIs principales
   - Algoritmos clave
   - Testing
   
   **Leer si**: Necesitas entender la arquitectura técnica

#### 🚀 Desplegar el Proyecto
5. **[AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)** - Despliegue a Azure
   - Requisitos previos
   - Configuración de Azure
   - Terraform setup
   - CI/CD con GitHub Actions
   - Monitoreo en producción
   - Troubleshooting
   
   **Leer si**: Necesitas desplegar a producción

### 🤖 Si Trabajas con Agentes de IA

6. **[AGENTES.md](./AGENTES.md)** - Orquestación de agentes de desarrollo
   - Descripción de cada agente
   - Responsabilidades
   - Flujos de trabajo
   - Cómo funcionan juntos
   
   **Leer si**: Usarás agentes de IA para desarrollo

7. **[AGENTES_CONFIG.md](./AGENTES_CONFIG.md)** - Configuración detallada de agentes
   - Identidad de cada agente
   - Cómo invocar cada uno
   - Quality gates
   - Checklists
   
   **Leer si**: Necesitas configurar o usar agentes específicos

### 🔒 Para Infraestructura/DevOps

8. **[AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)** - Setup de Azure
   - Creación de recursos
   - Base de datos en cloud
   - Key Vault para secretos
   - Monitoreo y alertas

---

## 📋 Quick Navigation

### Por Tarea

| Tarea | Documento |
|-------|-----------|
| Empezar a desarrollar localmente | [LOCAL_EXECUTION.md](./LOCAL_EXECUTION.md) |
| Entender cómo juega | [GAME_GUIDE.md](../GAME_GUIDE.md) |
| Aprender arquitectura | [ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md) |
| Entender features | [ANALISIS_FUNCIONAL.md](./ANALISIS_FUNCIONAL.md) |
| Ver cómo escribir código | [.copilot-instructions.md](../.copilot-instructions.md) |
| Desplegar a Azure | [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md) |
| Usar agentes de IA | [AGENTES.md](./AGENTES.md) |
| Configurar agentes específicos | [AGENTES_CONFIG.md](./AGENTES_CONFIG.md) |

### Por Nivel de Detalle

**Nivel 1: Overview Rápido** (5 min)
- [README.md](../README.md) - Overview del proyecto

**Nivel 2: Comprensión Funcional** (15 min)
- [GAME_GUIDE.md](../GAME_GUIDE.md) - Qué es el juego
- [ANALISIS_FUNCIONAL.md](./ANALISIS_FUNCIONAL.md) - Qué hace técnicamente

**Nivel 3: Implementación** (30 min)
- [.copilot-instructions.md](../.copilot-instructions.md) - Cómo desarrollar
- [LOCAL_EXECUTION.md](./LOCAL_EXECUTION.md) - Cómo correr localmente

**Nivel 4: Deep Dive** (1-2 horas)
- [ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md) - Arquitectura completa
- [AGENTES.md](./AGENTES.md) - Sistema de agentes

**Nivel 5: Production** (30 min)
- [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md) - Desplegar a Cloud

---

## 🗺️ Estructura de la Documentación

```
docs/
├── ANALISIS_FUNCIONAL.md      # QUÉ hace el proyecto
├── ANALISIS_TECNICO.md        # CÓMO está construido
├── AGENTES.md                 # Orquestación de agentes
├── AGENTES_CONFIG.md          # Config detallada agentes
├── AZURE_DEPLOYMENT.md        # Despliegue a cloud
├── LOCAL_EXECUTION.md         # Ejecución local
├── README.md                  # Este archivo
├── NpcAgentService.md         # Documentación específica NPCs
└── ... (otros archivos)

Raíz:
├── .copilot-instructions.md   # Convenciones y stack del projeto
├── README.md                  # Overview general
├── GAME_GUIDE.md              # Guía de juego
├── QUICKSTART.md              # Quick start
└── AVATAR_IMPLEMENTATION.md   # Sistema de avatares
```

---

## 🚀 Flujos de Trabajo Típicos

### Flujo 1: "Quiero jugar"
```
1. Lee: GAME_GUIDE.md
2. Lee: LOCAL_EXECUTION.md (Quick Start)
3. Ejecuta: dotnet run + npm start
4. Accede: http://localhost:3000
```

### Flujo 2: "Quiero desarrollar una feature"
```
1. Lee: .copilot-instructions.md
2. Lee: ANALISIS_TECNICO.md (Arquitectura)
3. Crea rama en Git
4. Implementa en LOCAL_EXECUTION.md (dev local)
5. Sigue convenciones en .copilot-instructions.md
6. Tests: CODE QUALITY GUARDIAN verifica
7. Seguridad: SECURITY AUDITOR verifica
8. Merge: A main
```

### Flujo 3: "Quiero desplegar a Producción"
```
1. Lee: AZURE_DEPLOYMENT.md
2. Configura Azure CLI + Terraform
3. Deploy backend + frontend
4. Deploy BD SQL
5. Configura Key Vault
6. Verifica en AZURE_DEPLOYMENT.md
7. Monitorea con Application Insights
```

### Flujo 4: "Quiero usar Agentes"
```
1. Lee: AGENTES.md (Overview)
2. Lee: AGENTES_CONFIG.md (Config específica)
3. Invoca agente (ej: @matrix-brain)
4. Proporciona contexto/tarea
5. Sigue output del agente
```

---

## 📞 Referencias Cruzadas

### Documentos Relacionados

- **ANALISIS_FUNCIONAL.md** menciona:
  - Cómo jugar → [GAME_GUIDE.md](../GAME_GUIDE.md)
  - Cómo desarrollar → [.copilot-instructions.md](../.copilot-instructions.md)

- **ANALISIS_TECNICO.md** menciona:
  - Configuración local → [LOCAL_EXECUTION.md](./LOCAL_EXECUTION.md)
  - Despliegue → [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)

- **LOCAL_EXECUTION.md** menciona:
  - Convenciones código → [.copilot-instructions.md](../.copilot-instructions.md)
  - Estructura proyecto → [ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md)

- **AGENTES.md** menciona:
  - Cómo se invoca CODE QUALITY → [AGENTES_CONFIG.md](./AGENTES_CONFIG.md)

---

## ✅ Checklist por Rol

### Checklist para Nuevo Desarrollador
```
[ ] Leer README.md
[ ] Leer .copilot-instructions.md
[ ] Leer ANALISIS_TECNICO.md
[ ] Ejecutar LOCAL_EXECUTION.md
[ ] Corroborar que todo funciona
[ ] Leer GAME_GUIDE.md para entender el juego
[ ] Leer .copilot-instructions.md nuevamente
```

### Checklist para Nuevo Agente
```
[ ] Leer AGENTES.md
[ ] Leer propia configuración en AGENTES_CONFIG.md
[ ] Entender responsabilidades
[ ] Revisar checklists de validación
[ ] Conocer canales de comunicación
```

### Checklist para Deploy a Producción
```
[ ] Leer AZURE_DEPLOYMENT.md completamente
[ ] Verificar todos los requisitos previos
[ ] Ejecutar Terraform plan
[ ] Revisar recursos a crear
[ ] Configurar Key Vault
[ ] Setup CI/CD
[ ] Verificar monitoreo
[ ] Test endpoints producción
```

---

## 🔗 URLs Importantes

### Documentación Online
- [GitHub Copilot](https://github.com/features/copilot)
- [.NET 9.0 Docs](https://learn.microsoft.com/dotnet/)
- [React Documentation](https://react.dev)
- [Azure Docs](https://learn.microsoft.com/azure/)

### Herramientas
- [Swagger UI](http://localhost:5000/swagger) - Documentación API local
- [Azure Portal](https://portal.azure.com) - Gestión Azure
- [GitHub](https://github.com) - Control de versión

### Proyecto
- **Frontend Local**: http://localhost:3000
- **Backend Local**: http://localhost:5000
- **API Docs**: http://localhost:5000/swagger

---

## 📞 Contacto & Soporte

### Problemas de Setup Locales
→ Ver [LOCAL_EXECUTION.md - Troubleshooting](./LOCAL_EXECUTION.md#troubleshooting)

### Problemas de Despliegue
→ Ver [AZURE_DEPLOYMENT.md - Troubleshooting](./AZURE_DEPLOYMENT.md#troubleshooting)

### Preguntas de Arquitectura
→ Leer [ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md)

### Preguntas de Agentes
→ Ver [AGENTES.md](./AGENTES.md) o [AGENTES_CONFIG.md](./AGENTES_CONFIG.md)

---

## 📈 Mantenimiento de Documentación

Todos los documentos deben mantenerse actualizados.

Cuándo actualizar:
- ✏️ Cambio en arquitectura → Actualizar [ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md)
- ✏️ Nuevas features → Actualizar [ANALISIS_FUNCIONAL.md](./ANALISIS_FUNCIONAL.md)
- ✏️ Nuevos endpoints → Actualizar [ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md)
- ✏️ Cambios en flujo agentes → Actualizar [AGENTES.md](./AGENTES.md)
- ✏️ Cambios de dependencies → Actualizar [LOCAL_EXECUTION.md](./LOCAL_EXECUTION.md)

**Responsable**: DOCUMENTATION AGENT

---

## 📊 Estadísticas de Documentación

| Documento | Líneas | Última Actualización |
|-----------|--------|---------------------|
| ANALISIS_FUNCIONAL.md | ~450 | Febrero 2026 |
| ANALISIS_TECNICO.md | ~550 | Febrero 2026 |
| AGENTES.md | ~650 | Febrero 2026 |
| AGENTES_CONFIG.md | ~480 | Febrero 2026 |
| AZURE_DEPLOYMENT.md | ~400 | Febrero 2026 |
| LOCAL_EXECUTION.md | ~500 | Febrero 2026 |

---

## 📝 Versión

**Versión de Documentación**: 1.0  
**Fecha**: Febrero 2026  
**Propietario**: DOCUMENTATION AGENT  
**Última Actualización**: Febrero 2026  
**Stack Documentado**: .NET 9.0, React 18, TypeScript 5, Azure

---

**¡Gracias por leer la documentación! Para cualquier duda, consulta el documento específico o invoca al agente apropiado.** 🚀
