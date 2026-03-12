# OpenCode Bootstrap Prompt

This file is automatically loaded at the beginning of AI sessions to
ensure that all agents understand the **project architecture and
development standards**.

Agents must read:

-   `.opencode/project-context.md`
-   `.opencode/policies/dotnet-architecture.md`
-   `.opencode/teams/dotnet-team.yaml`

before generating code or architectural designs.

------------------------------------------------------------------------

# Objective

Ensure every AI session starts with full knowledge of the platform
architecture, technology stack, and development rules.

This prevents:

-   incorrect architecture decisions
-   inconsistent code generation
-   missing infrastructure integrations
-   violations of system policies

------------------------------------------------------------------------

# Platform Summary

This project is a **distributed .NET platform** built with:

-   FastEndpoints APIs
-   Vertical Slice Architecture
-   Blazor + MudBlazor frontend
-   Redis caching
-   PostgreSQL / SQL Server
-   MongoDB
-   RabbitMQ or Azure Service Bus
-   .NET Aspire orchestration

The system is **event-driven and cloud-native**.

------------------------------------------------------------------------

# Mandatory Architecture Rules

All generated services must follow:

Vertical Slice Architecture\
FastEndpoints APIs\
CQRS when appropriate\
Polyglot persistence\
Event-driven messaging

Agents must never generate:

-   layered monolith patterns
-   shared databases across services
-   direct HttpClient calls in UI components
-   business logic inside controllers or UI

------------------------------------------------------------------------

# Persistence Rules

Data access must go through:

`IStorageService`

Database usage:

PostgreSQL / SQL Server → transactional data\
MongoDB → document storage / read models\
Redis → caching and ephemeral state

------------------------------------------------------------------------

# Messaging Rules

Service communication must be asynchronous.

Supported brokers:

RabbitMQ\
Azure Service Bus

Required reliability patterns:

Outbox Pattern\
Inbox Pattern\
Idempotent consumers\
Dead letter queues

------------------------------------------------------------------------

# Frontend Rules

Blazor UI must:

-   use MudBlazor components
-   use typed HttpClient API services
-   map DTOs → ViewModels
-   use scoped state containers

Dialogs must use:

IDialogService

Notifications must use:

MudSnackbar

------------------------------------------------------------------------

# Testing Requirements

Every feature must include:

Unit tests\
Integration tests\
Infrastructure tests

Testing tools:

xUnit\
bUnit\
Testcontainers

Infrastructure containers during tests:

PostgreSQL\
Redis\
MongoDB\
RabbitMQ

------------------------------------------------------------------------

# DevOps Rules

All services must integrate with:

.NET Aspire AppHost

Observability must use:

OpenTelemetry\
Prometheus\
Grafana\
Aspire Dashboard

------------------------------------------------------------------------

# Agent Collaboration

If a task involves multiple domains, agents must coordinate through the
**Architecture Orchestrator Agent**.

Example workflow:

System Architect → system design\
Backend Architect → API implementation\
Messaging Architect → events and queues\
Frontend Architect → UI components\
Testing Agent → automated tests\
Platform Agent → Aspire integration

------------------------------------------------------------------------

# Expected Output Quality

All generated solutions must be:

-   production-ready
-   scalable
-   testable
-   aligned with platform architecture

------------------------------------------------------------------------

# Instruction

Before generating any code:

1.  Load the project context
2.  Apply architecture policies
3.  Use the correct specialized agent
4.  Ensure compliance with platform standards
