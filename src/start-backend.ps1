# Script para iniciar el backend con Dapr
# Uso: .\start-backend.ps1

Write-Host "🚀 Iniciando D&D Copilot Backend con Dapr..." -ForegroundColor Green

# Navegar al directorio de la API
Set-Location -Path "$PSScriptRoot\src\DndCopilot.Api"

# Verificar que existen los componentes de Dapr
$componentsPath = "$PSScriptRoot\components"
if (-not (Test-Path $componentsPath)) {
    Write-Host "❌ Error: No se encuentra la carpeta 'components' en $componentsPath" -ForegroundColor Red
    exit 1
}

# Iniciar con Dapr
Write-Host "📦 Iniciando backend en puerto 5101 con Dapr..." -ForegroundColor Cyan
Write-Host "🔧 Componentes Dapr: $componentsPath" -ForegroundColor Cyan
Write-Host "" 

dapr run `
    --app-id dnd-copilot `
    --app-port 5101 `
    --resources-path "$componentsPath" `
    --log-level info `
    -- dotnet run

# Si Dapr falla, intentar sin Dapr
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "⚠️  Dapr falló. Iniciando sin Dapr (modo mock)..." -ForegroundColor Yellow
    Write-Host ""
    dotnet run
}
