#!/bin/bash

echo "🧹 Cleaning TicTacToe projects..."

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Stop running processes
echo "Stopping running processes..."
pkill -f "dotnet.*TicTacToeUI" 2>/dev/null
pkill -f "dotnet.*GameService" 2>/dev/null

# Kill processes on specific ports
echo "Freeing up ports..."
lsof -ti :7080 | xargs kill -9 2>/dev/null
lsof -ti :7082 | xargs kill -9 2>/dev/null
lsof -ti :5080 | xargs kill -9 2>/dev/null
lsof -ti :5082 | xargs kill -9 2>/dev/null

sleep 2

# Clean Frontend
echo "Cleaning Frontend..."
cd "$SCRIPT_DIR/src/FrontEnd/TicTacToeUI"
rm -rf bin obj
dotnet clean > /dev/null 2>&1

# Clean Backend
echo "Cleaning Backend..."
cd "$SCRIPT_DIR/src/Backend/GameService"
rm -rf bin obj
dotnet clean > /dev/null 2>&1

cd "$SCRIPT_DIR"

echo "✅ Cleanup complete!"
echo ""
echo "Next steps:"
echo "1. Terminal 1: ./start-backend.sh"
echo "2. Terminal 2: ./start-frontend.sh"
echo "3. Browser: open https://localhost:7080 then Cmd+Shift+R"




