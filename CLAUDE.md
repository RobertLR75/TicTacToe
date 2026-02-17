# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build TicTacToe.sln
dotnet build TicTacToe.sln -c Release

# Run backend
dotnet run --project src/Backend/GameService/GameService.csproj

# Test (once tests are added)
dotnet test TicTacToe.sln
```

## Architecture

This is a TicTacToe game built on .NET 10.0 (C#) with a planned frontend (currently empty).

**Solution structure:**
- `src/Backend/GameService/` — C# class library targeting .NET 10.0, nullable and implicit usings enabled
- `src/FrontEnd/` — not yet implemented
- `TicTacToe.sln` — Visual Studio solution file

## Agent-OS Framework

The project includes an Agent-OS framework at `agent-os/` with custom Claude commands in `.claude/commands/agent-os/`. These commands support a structured AI-assisted development workflow:

- `/agent-os:plan-product` — establish foundational product docs (`agent-os/product/`: mission, roadmap, tech-stack)
- `/agent-os:shape-spec` — structure planning for significant features; creates timestamped spec folders under `agent-os/specs/`
- `/agent-os:discover-standards` — extract patterns and conventions from the codebase into documented standards
- `/agent-os:index-standards` — maintain `agent-os/standards/index.yml`
- `/agent-os:inject-standards` — inject relevant standards into context before implementing features

When working on any significant feature, check `agent-os/standards/` for established conventions first.
