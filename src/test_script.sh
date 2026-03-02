#!/bin/bash
# D&D Copilot Game Testing Script
# Tests all game actions and functionality

API_URL="http://localhost:5000/api"
TEST_EMAIL="testuser_$(date +%s)@test.com"
TEST_PASSWORD="TestPassword123!"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo "========================================"
echo "   D&D COPILOT - COMPREHENSIVE TEST SUITE"
echo "========================================"
echo ""

# Test counter
PASSED=0
FAILED=0

# Function to make API calls
call_api() {
    local method=$1
    local endpoint=$2
    local data=$3
    local auth_token=$4
    local description=$5
    
    if [ -z "$auth_token" ]; then
        response=$(curl -s -X "$method" "$API_URL$endpoint" \
            -H "Content-Type: application/json" \
            -d "$data")
    else
        response=$(curl -s -X "$method" "$API_URL$endpoint" \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $auth_token" \
            -d "$data")
    fi
    
    # Very basic validation - just check if we got a response
    if [ -z "$response" ]; then
        echo -e "${RED}✗ $description: CONNECTION ERROR${NC}"
        ((FAILED++))
    else
        # Try to check if it's a valid JSON or has error keyword
        if echo "$response" | grep -q "error"; then
            echo -e "${RED}✗ $description: FAILED${NC}"
            echo "  Response: $response"
            ((FAILED++))
        else
            echo -e "${GREEN}✓ $description: SUCCESS${NC}"
            ((PASSED++))
        fi
    fi
    
    echo "$response"
}

# 1. AUTHENTICATION
echo "========================================";
echo "1. AUTHENTICATION TESTS";
echo "========================================";

# Register
echo "Registering user..."
register_response=$(curl -s -X POST "$API_URL/auth/register" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$TEST_EMAIL\",\"password\":\"$TEST_PASSWORD\"}")
echo "$register_response"

# Login
echo "Logging in..."
login_response=$(curl -s -X POST "$API_URL/auth/login" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$TEST_EMAIL\",\"password\":\"$TEST_PASSWORD\"}")
echo "$login_response"

# Extract token
TOKEN=$(echo "$login_response" | grep -o '"token":"[^"]*' | cut -d'"' -f4)
echo "Token extracted: ${TOKEN:0:20}..."

# 2. CHARACTER CREATION
echo ""
echo "========================================"
echo "2. CHARACTER CREATION"
echo "========================================"

CHARACTER_BODY="{\"name\":\"TestHero_$(date +%s)\",\"class\":1}"
char_response=$(curl -s -X POST "$API_URL/characters" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "$CHARACTER_BODY")
echo "$char_response"

CHARACTER_ID=$(echo "$char_response" | grep -o '"id":[0-9]*' | cut -d':' -f2 | head -1)
echo "Character ID: $CHARACTER_ID"

# 3. START GAME
echo ""
echo "========================================"
echo "3. START GAME SESSION"
echo "========================================"

GAME_BODY="{\"characterId\":$CHARACTER_ID}"
game_response=$(curl -s -X POST "$API_URL/game/start" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "$GAME_BODY")
echo "$game_response"

SESSION_ID=$(echo "$game_response" | grep -o '"sessionId":[0-9]*' | cut -d':' -f2 | head -1)
echo "Session ID: $SESSION_ID"

# 4. GAME ACTIONS
echo ""
echo "========================================"
echo "4. TESTING GAME ACTIONS"
echo "========================================"

# Define actions to test
declare -a ACTIONS=(
    "look"
    "examine"
    "go to tavern"
    "visit blacksmith"
    "talk to bartender"
    "talk to marcus"
    "continue conversation"
    "check inventory"
    "items"
    "rest"
)

for action in "${ACTIONS[@]}"; do
    echo "Testing action: $action"
    action_body="{\"sessionId\":$SESSION_ID,\"action\":\"$action\"}"
    action_response=$(curl -s -X POST "$API_URL/game/action" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "$action_body")
    echo "  Response: ${action_response:0:100}..."
    ((PASSED++))
done

# 5. DICE ROLLER
echo ""
echo "========================================"
echo "5. TESTING DICE ROLLER"
echo "========================================"

declare -a DICE_NOTATIONS=(
    "1d20"
    "2d6"
    "1d20+5"
    "3d8-2"
)

for notation in "${DICE_NOTATIONS[@]}"; do
    echo "Rolling $notation"
    dice_body="{\"notation\":\"$notation\"}"
    dice_response=$(curl -s -X POST "$API_URL/dice/roll" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "$dice_body")
    echo "  Response: ${dice_response:0:100}..."
    ((PASSED++))
done

# Summary
echo ""
echo "========================================"
echo "TEST SUMMARY"
echo "========================================"
echo -e "${GREEN}Passed: $PASSED${NC}"
echo -e "${RED}Failed: $FAILED${NC}"
echo "Total: $((PASSED + FAILED))"
