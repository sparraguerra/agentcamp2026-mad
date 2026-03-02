# Instrucciones de Ejecución Local - D&D Copilot

## 🚀 Quick Start (5 minutos)

### Terminal 1: Backend
```bash
cd src/DndCopilot.Api
dotnet run
# Esperar: "Now listening on: http://localhost:5000"
```

### Terminal 2: Frontend
```bash
cd client
npm install  # Primera vez solamente
npm start
# Navegador abrirá automáticamente http://localhost:3000
```

✅ **¡Listo!** Ahora puedes registrarte y jugar.

---

## 📋 Requisitos Previos

### Software Requerido

#### .NET SDK 9.0 o Superior
**Verificar instalación:**
```bash
dotnet --version
```

**Descargar desde:** https://dotnet.microsoft.com/download

**Instalación por SO:**

**Windows (PowerShell)**
```powershell
# Con Chocolatey
choco install dotnet-sdk

# O descargar: https://dotnet.microsoft.com/download/dotnet/9.0
```

**macOS (Homebrew)**
```bash
brew install dotnet
```

**Linux (Ubuntu/Debian)**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest
```

#### Node.js 20.x y npm 10.x
**Verificar instalación:**
```bash
node --version  # v20.x+
npm --version   # 10.x+
```

**Descargar desde:** https://nodejs.org/

**Instalación por SO:**

**Windows (PowerShell)**
```powershell
# Con Chocolatey
choco install nodejs

# O descargar: https://nodejs.org/en/download/
```

**macOS (Homebrew)**
```bash
brew install node@20
```

**Linux (Ubuntu/Debian)**
```bash
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs
```

#### Git (Opcional pero recomendado)
```bash
git --version
```

Descargar desde: https://git-scm.com/

---

## 🛠️ Instalación Paso a Paso

### Paso 1: Clonar o Descargar Repositorio

**Opción A: Con Git**
```bash
git clone https://github.com/YOUR_ORG/dnd-copilot.git
cd dnd-copilot
```

**Opción B: Descargar ZIP**
```bash
# Descargar archivo ZIP desde GitHub
# Extraer la carpeta
cd dnd-copilot
```

### Paso 2: Restaurar Dependencias Backend

```bash
cd src/DndCopilot.Api
dotnet restore
```

**Salida esperada:**
```
Determining projects to restore...
Restored [ruta]/DndCopilot.Api.csproj...
Restored [ruta]/DndCopilot.Core.csproj...
Restored [ruta]/DndCopilot.Infrastructure.csproj...
```

### Paso 3: Instalar Dependencias Frontend

```bash
cd client
npm install
```

**Salida esperada:**
```
up to date, audited X packages
```

### Paso 4: Verificar Compilación

**Backend:**
```bash
cd src/DndCopilot.Api
dotnet build
# Salida: "Build succeeded" ✓
```

**Frontend:**
```bash
cd client
npm run build
# Salida sin errores ✓
```

---

## 🎮 Ejecutar la Aplicación

### Opción A: Dos Terminales (Recomendado)

#### Terminal 1: Iniciar Backend
```bash
cd src/DndCopilot.Api
dotnet run
```

**Salida esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

✅ **API escuchando en puerto 5000**

#### Terminal 2: Iniciar Frontend
```bash
cd client
npm start
```

**Salida esperada:**
```
> react-scripts start

Compiled successfully!

You can now view dnd-copilot in the browser.

  Local:            http://localhost:3000
  On Your Network:  http://192.168.x.x:3000

Note that the development build is not optimized.
To create a production build, use npm run build.
```

**El navegador se abrirá automáticamente en http://localhost:3000**

### Opción B: Una Terminal con npm Scripts

**Modificar package.json si deseas:**
```json
{
  "scripts": {
    "start": "npm start",
    "start-full": "concurrently \"dotnet run --project src/DndCopilot.Api\" \"npm start\""
  }
}
```

Instalar concurrently:
```bash
npm install --save-dev concurrently
```

Luego:
```bash
npm run start-full
```

### Opción C: Docker (Si tienes Docker instalado)

**Build imagen:**
```bash
docker build -f Dockerfile -t dnd-copilot-api .
docker run -p 5000:5000 dnd-copilot-api
```

---

## 🔐 Estructura Base de Datos

### Inicialización Automática

La aplicación **crea automáticamente** la base de datos SQLite en:
```
src/DndCopilot.Api/dndcopilot.db
```

Al iniciar por primera vez:
1. ✅ Se ejecutan migrations
2. ✅ Se crea `dndcopilot.db`
3. ✅ Se rellena con datos seed (Items, NPCs, etc.)

### Acceder a la Base de Datos

**Con SQLite Browser** (GUI)
```bash
# Descargar desde: https://sqlitebrowser.org/
# Abrir archivo: src/DndCopilot.Api/dndcopilot.db
```

**Con CLI:**
```bash
cd src/DndCopilot.Api
sqlite3 dndcopilot.db

# Dentro del CLI SQLite:
.tables                    # Ver todas las tablas
SELECT * FROM Users;       # Ver usuarios
SELECT * FROM Characters;  # Ver personajes
.quit                      # Salir
```

### Limpiar Base de Datos

Para empezar de cero:
```bash
cd src/DndCopilot.Api
rm dndcopilot.db           # Eliminar archivo
dotnet run                 # Se crea nuevo automáticamente
```

---

## 🌐 Acceder a la Aplicación

### Frontend Web
- **URL**: http://localhost:3000
- **Interfaz**: Login/Registro + Dashboard

### Backend API
- **Base URL**: http://localhost:5000/api
- **Swagger/OpenAPI**: http://localhost:5000/swagger

### Prueba Rápida de API
```bash
# Registrar usuario
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"hero","email":"hero@example.com","password":"password123"}'

# Respuesta esperada:
# {"token":"eyJhbGc...","userId":1,"username":"hero"}
```

---

## 🧪 Ejecutar Tests

### Tests .NET Backend

```bash
# Todos los tests
cd tests/DndCopilot.Tests
dotnet test

# Output esperado:
# Test Run Successful.
# Total tests: 13
# Passed: 13
# Duration: 0.25 seconds
```

### Tests Específicos

```bash
# Solo tests de Dice Roller
dotnet test --filter="DiceRoller"

# Solo tests de Combat
dotnet test --filter="CombatService"

# Con output detallado
dotnet test --verbosity=normal
```

### Tests React Frontend (Opcional)

```bash
cd client
npm test

# Presionar 'a' para ejecutar todos
# Presionar 'q' para salir
```

---

## 🔧 Configuración Local

### Backend: appsettings.Development.json

El archivo `src/DndCopilot.Api/appsettings.Development.json` ya está configurado para desarrollo local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=dndcopilot.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKeyForDevelopmentOnlyChangeInProduction123!",
    "Issuer": "DndCopilotApi",
    "Audience": "DndCopilotClient",
    "ExpirationHours": "24"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

**¡Sin cambios necesarios para desarrollo local!**

### Frontend: .env (Opcional)

Si necesitas cambiar la URL de la API:

**client/.env**
```
REACT_APP_API_URL=http://localhost:5000/api
```

Luego reiniciar `npm start`

---

## 🚦 Workflow Típico de Desarrollo

### 1. Clonar y Preparar
```bash
git clone <repo>
cd dnd-copilot
cd src/DndCopilot.Api && dotnet restore && cd ../../
cd client && npm install && cd ../..
```

### 2. Iniciar Servicios
```bash
# Terminal 1
cd src/DndCopilot.Api && dotnet run

# Terminal 2
cd client && npm start
```

### 3. Realizar Cambios

**Backend:**
- Los cambios en `.cs` se recompilan automáticamente
- Reinicia la aplicación manualmente si es necesario

**Frontend:**
- Los cambios en `.tsx` se hot reload automáticamente
- Gárdalo y verás cambios en tiempo real

### 4. Ejecutar Tests
```bash
cd tests/DndCopilot.Tests
dotnet test
```

### 5. Compilar para Producción

**Backend:**
```bash
cd src/DndCopilot.Api
dotnet publish -c Release
```

**Frontend:**
```bash
cd client
npm run build
# Genera carpeta `build/` lista para desplegar
```

---

## 🐞 Debugging

### Visual Studio Code

**Backend (.NET):**
1. Instalar extensión: "C#" (Microsoft)
2. Pressionar `F5` para iniciar con debugger
3. Establecer breakpoints en código

**Frontend (React):**
1. Abrir DevTools (F12)
2. Tab "Sources" para debugger
3. Usare console (F12 → Console)

### Visual Studio (Full)

Si instalaste Visual Studio:
```bash
# Abrir solución
start DndCopilot.sln

# F5 para iniciar con debugger
```

### Logs en Consola

**Backend:**
Los logs aparecen en la terminal donde ejecutaste `dotnet run`

```
info: DndCopilot.Api.Controllers.CharactersController[0]
      Getting all characters
warn: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (150ms) [Parameters=[], CommandType='Text', CommandText='SELECT...'
```

**Frontend:**
Abre la consola del navegador (F12 → Console):
```javascript
// Logs de React aparecem aquí
// Errores en rojo
// Warnings en amarillo
```

---

## 📁 Estructura de Carpetas Local

Después de ejecutar los comandos anteriores:

```
dnd-copilot/
├── node_modules/          # ← npm install crea esto
├── src/
│   ├── DndCopilot.Api/
│   │   ├── bin/
│   │   │   └── Debug/     # ← dotnet build crea esto
│   │   ├── dndcopilot.db  # ← Se crea auto al ejecutar
│   │   └── ...
│   ├── DndCopilot.Core/
│   └── DndCopilot.Infrastructure/
├── client/
│   ├── node_modules/      # ← npm install en client/
│   ├── build/             # ← npm run build crea esto
│   ├── public/
│   ├── src/
│   └── ...
└── ...
```

---

## 🎮 Primeros Pasos en el Juego

1. **Abre** http://localhost:3000
2. **Click** "Register"
3. **Ingresa**:
   - Username: `hero`
   - Email: `hero@game.com`
   - Password: `password123`
4. **Click** "Register"
5. **Click** "+ Create Character"
6. **Ingresa nombre**: `Aragorn`
7. **Selecciona clase**: Warrior (⚔️)
8. **Click** "Create Character"
9. **¡Comienza la aventura!** Explora el mapa y combate enemigos

---

## ⚡ Tips & Tricks

### Desarrollar Más Rápido
```bash
# Hot reload automático para TypeScript
cd client
npm start

# Rechargue automático para .NET (necesita instalación)
dotnet tool install -g dotnet-watch
cd src/DndCopilot.Api
dotnet watch run
```

### Limpiar Proyectos
```bash
# Backend
cd src/DndCopilot.Api
dotnet clean
rm -rf bin obj

# Frontend
cd client
rm -rf node_modules build
npm install
```

### Cambiar Puerto

**Backend (puerto 6000 en lugar de 5000):**
Editar `src/DndCopilot.Api/Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:6000;https://localhost:6001"
```

**Frontend (puerto 3001 en lugar de 3000):**
```bash
PORT=3001 npm start
```

### Borrar Token de Sesión
Si tienes problemas de autenticación:
```bash
# En consola del navegador (F12):
localStorage.removeItem('auth_token');
window.location.reload();
```

---

## 📚 Documentación Adicional

- **Guía del Juego**: [GAME_GUIDE.md](../GAME_GUIDE.md)
- **API REST**: http://localhost:5000/swagger
- **Avatar Pixelado**: [AVATAR_IMPLEMENTATION.md](../AVATAR_IMPLEMENTATION.md)
- **Análisis Funcional**: [docs/ANALISIS_FUNCIONAL.md](./ANALISIS_FUNCIONAL.md)
- **Análisis Técnico**: [docs/ANALISIS_TECNICO.md](./ANALISIS_TECNICO.md)

---

## ❓ Troubleshooting

### "Port already in use" (5000 o 3000)

**Opción 1: Cambiar puerto**
```bash
# Backend
cd src/DndCopilot.Api/Properties
# Editar launchSettings.json

# Frontend
PORT=3001 npm start
```

**Opción 2: Matar proceso en puerto**

**Windows (PowerShell):**
```powershell
netstat -ano | findstr :5000
taskkill /PID <proceso_id> /F

# O
Get-Process -Id $(Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process
```

**macOS/Linux:**
```bash
lsof -i :5000
kill -9 <proceso_id>
```

### "CORS error" (Frontend no puede conectar a API)

Verificar que el backend está corriendo:
```bash
curl http://localhost:5000/api/dice/roll
```

Si no responde, reiniciar con `dotnet run`

### "Module not found" (npm error)

```bash
cd client
rm -rf node_modules package-lock.json
npm install
```

### "Build failed" (.NET error)

```bash
cd src/DndCopilot.Api
dotnet clean
dotnet restore
dotnet build
```

### "Database locked" (SQLite error)

```bash
# Backend está usando BD, ciérralo primero
# O elimina el archivo y deja que se recree
rm src/DndCopilot.Api/dndcopilot.db
dotnet run
```

---

## 🎯 Próximos Pasos

1. ✅ Backend funcionando en http://localhost:5000
2. ✅ Frontend funcionando en http://localhost:3000
3. ✅ Base de datos SQLite creada
4. 📖 Lee [GAME_GUIDE.md](../GAME_GUIDE.md) para aprender a jugar
5. 🧪 Ejecuta tests: `dotnet test`
6. 🚀 Cuando termines, despliega en Azure: [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)

---

**¡Que disfrutes desarrollando D&D Copilot!** 🎲⚔️🔮

**Versión**: 1.0  
**Fecha**: Febrero 2026  
**Última Actualización**: Febrero 2026
