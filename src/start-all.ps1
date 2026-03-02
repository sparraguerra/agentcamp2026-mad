# Script para iniciar D&D Copilot completo (Backend + Frontend)
# Uso: .\start-all.ps1

Write-Host "🎮 Iniciando D&D Copilot - Backend + Frontend" -ForegroundColor Green
Write-Host "============================================`n" -ForegroundColor Green

# Guardar el directorio raíz
$rootPath = $PSScriptRoot

# Verificar requisitos
Write-Host "🔍 Verificando requisitos..." -ForegroundColor Cyan

# Verificar .NET
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: .NET SDK no está instalado" -ForegroundColor Red
    Write-Host "   Descarga desde: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Verificar Node.js
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: Node.js no está instalado" -ForegroundColor Red
    Write-Host "   Descarga desde: https://nodejs.org/" -ForegroundColor Yellow
    exit 1
}

# Verificar Dapr CLI
try {
    $daprVersion = dapr --version
    Write-Host "✅ Dapr CLI: $daprVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: Dapr CLI no está instalado" -ForegroundColor Red
    Write-Host "   Descarga desde: https://docs.dapr.io/getting-started/install-dapr-cli/" -ForegroundColor Yellow
    exit 1
}

# Verificar componentes de Dapr
$componentsPath = Join-Path $rootPath "components"
if (-not (Test-Path $componentsPath)) {
    Write-Host "⚠️  Advertencia: carpeta 'components' no encontrada. Dapr se ejecutará en modo mock." -ForegroundColor Yellow
}

Write-Host "`n📦 Preparando servicios...`n" -ForegroundColor Cyan

# Instalar dependencias del frontend si es necesario
$clientPath = Join-Path $rootPath "client"
$nodeModulesPath = Join-Path $clientPath "node_modules"
if (-not (Test-Path $nodeModulesPath)) {
    Write-Host "📦 Instalando dependencias del frontend..." -ForegroundColor Yellow
    Push-Location $clientPath
    npm install
    Pop-Location
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Error: npm install falló" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Dependencias instaladas`n" -ForegroundColor Green
}

# Iniciar Backend en nueva ventana
Write-Host "🚀 Iniciando Backend (puerto 5101)..." -ForegroundColor Cyan
$backendPath = Join-Path $rootPath "src\DndCopilot.Api"
$daprResourcesPath = Join-Path $rootPath "components"

# Crear comando para el backend
$backendCommand = @"
Write-Host '🔧 Backend - D&D Copilot API' -ForegroundColor Magenta
Write-Host '=============================' -ForegroundColor Magenta
Write-Host 'Puerto: http://localhost:5101' -ForegroundColor Cyan
Write-Host 'Swagger: http://localhost:5101/swagger' -ForegroundColor Cyan
Write-Host ''
Set-Location '$backendPath'
dapr run --app-id dnd-copilot-api --app-port 5101 --dapr-http-port 3500 --resources-path '$daprResourcesPath' -- dotnet run
"@

Start-Process pwsh -ArgumentList "-NoExit", "-Command", $backendCommand

# Esperar a que el backend esté listo
Write-Host "⏳ Esperando a que el backend inicie..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Iniciar Frontend en nueva ventana
Write-Host "🎨 Iniciando Frontend (puerto 3000)..." -ForegroundColor Cyan

$frontendCommand = @"
Write-Host '🎨 Frontend - D&D Copilot Client' -ForegroundColor Magenta
Write-Host '=================================' -ForegroundColor Magenta
Write-Host 'URL: http://localhost:3000' -ForegroundColor Cyan
Write-Host ''
Set-Location '$clientPath'
npm start
"@

Start-Process pwsh -ArgumentList "-NoExit", "-Command", $frontendCommand

# Esperar un poco más
Start-Sleep -Seconds 3

Write-Host "`n✨ ¡Servicios iniciados!" -ForegroundColor Green
Write-Host "================================`n" -ForegroundColor Green
Write-Host "📱 Frontend:  http://localhost:3000" -ForegroundColor Cyan
Write-Host "🔧 Backend:   http://localhost:5101" -ForegroundColor Cyan
Write-Host "📚 Swagger:   http://localhost:5101/swagger" -ForegroundColor Cyan
Write-Host "`n💡 Tip: Las ventanas de backend y frontend se abrieron en terminales separadas" -ForegroundColor Yellow
Write-Host "    Cierra esas ventanas para detener los servicios`n" -ForegroundColor Yellow
Write-Host "Presiona Enter para salir..." -ForegroundColor Gray
Read-Host
