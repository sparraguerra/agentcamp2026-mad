# Testing Script for D&D Copilot API
# Tests all game actions and functionality

$API_BASE = "http://localhost:5101/api"
$TEST_RESULTS = @()
$PASSED = 0
$FAILED = 0

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "D AND D COPILOT - API TESTING SUITE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

function Test-Action {
    param(
        [string]$Name,
        [string]$Endpoint,
        [string]$Method,
        [object]$Body,
        [string]$Token
    )
    
    try {
        $headers = @{"Content-Type" = "application/json"}
        if ($Token) { $headers["Authorization"] = "Bearer $Token" }
        
        $params = @{
            Uri = "$API_BASE$Endpoint"
            Method = $Method
            Headers = $headers
            TimeoutSec = 10
        }
        
        if ($Body) { $params["Body"] = $Body | ConvertTo-Json -Depth 5 }
        
        $response = Invoke-RestMethod @params
        Write-Host "[PASS] $Name" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "[FAIL] $Name - $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# 1. AUTH
Write-Host ""
Write-Host "1. AUTHENTICATION" -ForegroundColor Yellow
$email = "tester_$(Get-Random)@test.com"
$password = "Test123!"

$reg = Test-Action -Name "Register" -Endpoint "/auth/register" -Method "POST" -Body @{email=$email; password=$password}
$login = Test-Action -Name "Login" -Endpoint "/auth/login" -Method "POST" -Body @{email=$email; password=$password}

if (-not $login.token) { Write-Host "No token received!" -ForegroundColor Red; exit 1 }
$token = $login.token
Write-Host "Token: $($token.Substring(0,20))..." -ForegroundColor Cyan

# 2. CHARACTER
Write-Host ""
Write-Host "2. CHARACTER CREATION" -ForegroundColor Yellow
$char = Test-Action -Name "Create Character" -Endpoint "/characters" -Method "POST" -Body @{name="TestHero_$(Get-Random)"; class=1} -Token $token

if (-not $char.id) { Write-Host "No character ID!" -ForegroundColor Red; exit 1 }
$characterId = $char.id
Write-Host "Character ID: $characterId" -ForegroundColor Cyan

# 3. GAME SESSION
Write-Host ""
Write-Host "3. GAME SESSION" -ForegroundColor Yellow
$game = Test-Action -Name "Start Game" -Endpoint "/game/start" -Method "POST" -Body @{characterId=$characterId} -Token $token

if (-not $game.sessionId) { Write-Host "No session ID!" -ForegroundColor Red; exit 1 }
$sessionId = $game.sessionId
Write-Host "Session ID: $sessionId" -ForegroundColor Cyan
Write-Host "Welcome: $($game.welcomeMessage.Substring(0,100))..." -ForegroundColor Cyan

# 4. GAME ACTIONS
Write-Host ""
Write-Host "4. TESTING GAME ACTIONS" -ForegroundColor Yellow

$actions = @(
    "look around",
    "examine",
    "go to tavern",
    "talk to bartender",
    "talk to Grundy",
    "continue conversation",
    "ask about quests",
    "visit blacksmith",
    "talk to marcus",
    "browse weapons",
    "check inventory",
    "items",
    "rest",
    "roll 1d20"
)

foreach ($action in $actions) {
    $result = Test-Action -Name "Action: $action" -Endpoint "/game/action" -Method "POST" -Body @{sessionId=$sessionId; action=$action} -Token $token
}

# 5. DICE
Write-Host ""
Write-Host "5. TESTING DICE ROLLER" -ForegroundColor Yellow

$diceTests = @("1d20", "2d6", "1d20+5", "3d8-2")

foreach ($dice in $diceTests) {
    Test-Action -Name "Roll $dice" -Endpoint "/dice/roll" -Method "POST" -Body @{notation=$dice} -Token $token
}

# 6. SUMMARY
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TESTING COMPLETE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
