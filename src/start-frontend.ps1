# Script para iniciar el frontend
# Uso: .\start-frontend.ps1

Write-Host "🎨 Iniciando D&D Copilot Frontend..." -ForegroundColor Green

# Navegar al directorio del cliente
Set-Location -Path "$PSScriptRoot\client"

# Verificar si node_modules existe
if (-not (Test-Path "node_modules")) {
    Write-Host "📦 Instalando dependencias..." -ForegroundColor Cyan
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Error: npm install falló" -ForegroundColor Red
        exit 1
    }
}

# Iniciar el frontend
Write-Host "🚀 Iniciando frontend en http://localhost:3000..." -ForegroundColor Cyan
Write-Host ""

npm start
