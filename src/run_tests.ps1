# Comprehensive Game Actions Testing Script
# This script tests all possible game actions and interactions

`$API_URL = "http://localhost:5000/api"
`$TEST_RESULTS = @()

# Colors for output
`$Colors = @{
    Success = 'Green'
    Error = 'Red'
    Warning = 'Yellow'
    Info = 'Cyan'
}

function Log-Test {
    param(
        [string]`$Action,
        [string]`$Status,
        [string]`$Message
    )
    
    `$color = `$Colors[`$Status]
    Write-Host "[`$Status] `$Action" -ForegroundColor `$color
    if (`$Message) {
        Write-Host "  └─ `$Message" -ForegroundColor Gray
    }
    
    `$TEST_RESULTS += @{
        Action = `$Action
        Status = `$Status
        Message = `$Message
        Timestamp = Get-Date
    }
}

function Test-Endpoint {
    param(
        [string]`$Method,
        [string]`$Endpoint,
        [object]`$Body,
        [string]`$Token,
        [string]`$TestName
    )
    
    try {
        `$headers = @{
            'Content-Type' = 'application/json'
        }
        
        if (`$Token) {
            `$headers['Authorization'] = "Bearer `$Token"
        }
        
        `$params = @{
            Uri = "`$API_URL`$Endpoint"
            Method = `$Method
            Headers = `$headers
        }
        
        if (`$Body) {
            `$params['Body'] = `$Body | ConvertTo-Json -Depth 10
        }
        
        `$response = Invoke-RestMethod @params
        Log-Test -Action `$TestName -Status "Success" -Message "OK"
        return `$response
    }
    catch {
        Log-Test -Action `$TestName -Status "Error" -Message "`$(`$_.Exception.Message)"
        return `$null
    }
}

Write-Host "========================================"
Write-Host "   D AND D COPILOT - TEST SUITE"
Write-Host "========================================"
Write-Host ""

# Wait for API to be ready
Write-Host "Waiting for API..." -ForegroundColor Yellow
`$attempt = 0
`$maxAttempts = 30

while (`$attempt -lt `$maxAttempts) {
    try {
        `$health = Invoke-RestMethod -Uri "http://localhost:5000/api/ping" -Method Get -ErrorAction Stop
        Write-Host "API is ready!" -ForegroundColor Green
        break
    }
    catch {
        `$attempt++
        Write-Host "." -NoNewline -ForegroundColor Yellow
        Start-Sleep -Seconds 1
    }
}

if (`$attempt -eq `$maxAttempts) {
    Write-Host "API failed to start. Exiting." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================"
Write-Host "1. AUTHENTICATION TESTS"
Write-Host "========================================"

# Register user
`$testEmail = "testuser_$(Get-Random)@test.com"
`$testPassword = "TestPassword123!"

`$registerBody = @{
    email = `$testEmail
    password = `$testPassword
}

`$registerResponse = Test-Endpoint -Method "POST" -Endpoint "/auth/register" -Body `$registerBody -TestName "Register new user"

if (-not `$registerResponse) {
    Write-Host "Registration failed. Exiting." -ForegroundColor Red
    exit 1
}

# Login
`$loginBody = @{
    email = `$testEmail
    password = `$testPassword
}

`$loginResponse = Test-Endpoint -Method "POST" -Endpoint "/auth/login" -Body `$loginBody -TestName "Login user"

if (-not `$loginResponse -or -not `$loginResponse.token) {
    Write-Host "Login failed. Exiting." -ForegroundColor Red
    exit 1
}

`$token = `$loginResponse.token
Log-Test -Action "Extract JWT Token" -Status "Success" -Message "Token obtained"

Write-Host ""
Write-Host "========================================"
Write-Host "2. CHARACTER CREATION TESTS"
Write-Host "========================================"

# Create character
`$characterBody = @{
    name = "TestHero_$(Get-Random)"
    class = 1
}

`$characterResponse = Test-Endpoint -Method "POST" -Endpoint "/characters" -Body `$characterBody -Token `$token -TestName "Create new character"

if (-not `$characterResponse -or -not `$characterResponse.id) {
    Write-Host "Character creation failed. Exiting." -ForegroundColor Red
    exit 1
}

`$characterId = `$characterResponse.id
Log-Test -Action "Extract Character ID" -Status "Success" -Message "Character ID: `$characterId"

Write-Host ""
Write-Host "========================================"
Write-Host "3. GAME SESSION TESTS"
Write-Host "========================================"

# Start game
`$startGameBody = @{
    characterId = `$characterId
}

`$gameResponse = Test-Endpoint -Method "POST" -Endpoint "/game/start" -Body `$startGameBody -Token `$token -TestName "Start new game session"

if (-not `$gameResponse -or -not `$gameResponse.sessionId) {
    Write-Host "Game start failed. Exiting." -ForegroundColor Red
    exit 1
}

`$sessionId = `$gameResponse.sessionId
Log-Test -Action "Extract Session ID" -Status "Success" -Message "Session ID: `$sessionId"

Write-Host ""
Write-Host "========================================"
Write-Host "4. LOCATION AND NAVIGATION TESTS"
Write-Host "========================================"

# Test location actions
@(
    @{ Action = "look"; Description = "Look around" },
    @{ Action = "go to tavern"; Description = "Go to tavern" }
) | ForEach-Object {
    `$actionBody = @{
        sessionId = `$sessionId
        action = `$_.Action
    }
    Test-Endpoint -Method "POST" -Endpoint "/game/action" -Body `$actionBody -Token `$token -TestName "Action: `$(`$_.Description)"
}

Write-Host ""
Write-Host "========================================"
Write-Host "5. NPC INTERACTION TESTS"
Write-Host "========================================"

# Talk to NPCs
@(
    @{ Action = "talk to bartender"; Description = "Talk to Grundy" },
    @{ Action = "continue conversation"; Description = "Continue conversation" }
) | ForEach-Object {
    `$actionBody = @{
        sessionId = `$sessionId
        action = `$_.Action
    }
    Test-Endpoint -Method "POST" -Endpoint "/game/action" -Body `$actionBody -Token `$token -TestName "NPC: `$(`$_.Description)"
}

Write-Host ""
Write-Host "========================================"
Write-Host "6. INVENTORY TESTS"
Write-Host "========================================"

# Check inventory
@(
    @{ Action = "check inventory"; Description = "View inventory" },
    @{ Action = "items"; Description = "Check items" }
) | ForEach-Object {
    `$actionBody = @{
        sessionId = `$sessionId
        action = `$_.Action
    }
    Test-Endpoint -Method "POST" -Endpoint "/game/action" -Body `$actionBody -Token `$token -TestName "Inventory: `$(`$_.Description)"
}

Write-Host ""
Write-Host "========================================"
Write-Host "7. DICE ROLLER TESTS"
Write-Host "========================================"

# Test different dice notations
@(
    @{ Notation = "1d20"; Description = "Roll 1d20" },
    @{ Notation = "2d6"; Description = "Roll 2d6" },
    @{ Notation = "1d20+5"; Description = "Roll 1d20 with modifier" }
) | ForEach-Object {
    `$roleBody = @{
        notation = `$_.Notation
    }
    Test-Endpoint -Method "POST" -Endpoint "/dice/roll" -Body `$roleBody -Token `$token -TestName "Dice: `$(`$_.Description)"
}

Write-Host ""
Write-Host "========================================"
Write-Host "8. REST TESTS"
Write-Host "========================================"

# Rest
`$actionBody = @{
    sessionId = `$sessionId
    action = "rest"
}
Test-Endpoint -Method "POST" -Endpoint "/game/action" -Body `$actionBody -Token `$token -TestName "Action: Rest and heal"

Write-Host ""
Write-Host "========================================"
Write-Host "TEST RESULTS SUMMARY"
Write-Host "========================================"

`$successCount = (`$TEST_RESULTS | Where-Object { `$_.Status -eq "Success" }).Count
`$errorCount = (`$TEST_RESULTS | Where-Object { `$_.Status -eq "Error" }).Count

Write-Host "Total Tests: `$(`$TEST_RESULTS.Count)"
Write-Host "Successful: `$successCount" -ForegroundColor Green
Write-Host "Errors: `$errorCount" -ForegroundColor Red

if (`$errorCount -eq 0) {
    Write-Host ""
    Write-Host "ALL TESTS PASSED!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Some tests failed. Review errors above." -ForegroundColor Red
}
