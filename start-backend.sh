#!/bin/bash

echo "🚀 Starting TicTacToe Backend API..."
cd "$(dirname "$0")/src/Backend/GameService"

echo "Cleaning previous build..."
rm -rf bin obj

echo "Building..."
dotnet build -v m

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "Starting API on https://localhost:7082..."
    echo "Swagger UI will be available at: https://localhost:7082/swagger"
    dotnet run
else
    echo "❌ Build failed!"
    exit 1
fi

