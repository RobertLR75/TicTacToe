#!/bin/bash

echo "🚀 Starting TicTacToe with .NET Aspire"
echo "======================================"

cd "$(dirname "$0")"

# Cleanup first
echo ""
echo "Step 1: Cleanup..."
./cleanup.sh

# Build Backend
echo ""
echo "Step 2: Building Backend..."
cd src/Backend/GameService
dotnet build -v q

if [ $? -ne 0 ]; then
    echo "❌ Backend build failed!"
    exit 1
fi

# Build Frontend
echo ""
echo "Step 3: Building Frontend..."
cd ../../FrontEnd/TicTacToeUI
dotnet build -v q

if [ $? -ne 0 ]; then
    echo "❌ Frontend build failed!"
    exit 1
fi

# Build AppHost
echo ""
echo "Step 4: Building AppHost..."
cd ../../AppHost
dotnet build -v q

if [ $? -ne 0 ]; then
    echo "❌ AppHost build failed!"
    exit 1
fi

echo ""
echo "✅ Build successful!"
echo ""
echo "======================================"
echo "Starting Aspire AppHost..."
echo ""
echo "This will start both services:"
echo "  - Backend API: https://localhost:7082"
echo "  - Frontend UI: https://localhost:7080"
echo ""
echo "Press Ctrl+C to stop all services"
echo "======================================"
echo ""

# Start AppHost
dotnet run --no-build

