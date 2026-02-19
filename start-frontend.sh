#!/bin/bash

echo "🎮 Starting TicTacToe Frontend..."
cd "$(dirname "$0")/src/FrontEnd/TicTacToeUI"

echo "Cleaning previous build..."
rm -rf bin obj

echo "Building..."
dotnet build -v m

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "Starting UI on https://localhost:7080..."
    echo "Make sure Backend is running on https://localhost:7082"
    dotnet run
else
    echo "❌ Build failed!"
    exit 1
fi

