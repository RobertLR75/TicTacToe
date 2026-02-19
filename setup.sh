#!/bin/bash

echo "🚀 Starting TicTacToe - Complete Setup"
echo "======================================"

# Cleanup first
echo ""
echo "Step 1: Cleanup..."
./cleanup.sh

# Build backend
echo ""
echo "Step 2: Building Backend..."
cd src/Backend/GameService
dotnet build -v q
if [ $? -ne 0 ]; then
    echo "❌ Backend build failed!"
    exit 1
fi
cd ../../..

# Build frontend
echo ""
echo "Step 3: Building Frontend..."
cd src/FrontEnd/TicTacToeUI
dotnet build -v q
if [ $? -ne 0 ]; then
    echo "❌ Frontend build failed!"
    exit 1
fi
cd ../../..

echo ""
echo "✅ Build successful!"
echo ""
echo "======================================"
echo "Now start the applications:"
echo ""
echo "Terminal 1: ./start-backend.sh"
echo "Terminal 2: ./start-frontend.sh"
echo "Browser:    open https://localhost:7080"
echo "======================================"

