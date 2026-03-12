# Project Context: .NET Distributed Platform

This file provides **global architectural context** for all OpenCode
agents working in this repository. Agents must read this file before
generating code or making architectural decisions.

------------------------------------------------------------------------

# Platform Overview

This repository contains a **cloud‑native distributed system** built
using modern .NET technologies.

Core stack:

-   .NET 8 / .NET 9
-   FastEndpoints
-   Blazor (Server or WebAssembly)
-   MudBlazor UI
-   .NET Aspire for orchestration
-   Redis
-   PostgreSQL / SQL Server
-   MongoDB
-   RabbitMQ or Azure Service Bus

Architecture goals:

-   high scalability
-   strong modularity
-   event‑driven systems
-   cloud‑native deployment
-   maintainable codebase

------------------------------------------------------------------------

# Backend Architecture

Backend services follow:

Vertical Slice Architecture\
CQRS (when appropriate)\
FastEndpoints APIs

Each feature slice contains:

Endpoint\
Request\
Response\
Validator\
Handler\
Mapper

Example:

Features/ Orders/ CreateOrder/ Endpoint.cs Request.cs Response.cs
Validator.cs Handler.cs

Rules:

-   endpoints must remain thin
-   handlers contain business logic
-   validation uses FluentValidation
-   mapping uses FastEndpoints Mapper

------------------------------------------------------------------------

# Persistence Strategy

The platform uses **polyglot persistence**.

Relational databases:

PostgreSQL or SQL Server

Use cases:

-   transactional systems
-   relational queries

Access strategy:

EF Core → write operations\
Dapper → read operations

------------------------------------------------------------------------

Document storage:

MongoDB

Use cases:

-   projections
-   read models
-   event storage
-   flexible schema data

------------------------------------------------------------------------

Caching:

Redis

Use cases:

-   distributed caching
-   session storage
-   rate limiting
-   distributed locks

------------------------------------------------------------------------

# Messaging Architecture

Services communicate using **event‑driven messaging**.

Supported brokers:

RabbitMQ\
Azure Service Bus

Messaging abstraction:

IBusService

Patterns used:

Domain Events\
Integration Events\
Async Commands\
Background Workers

Reliability patterns:

Outbox Pattern\
Inbox Pattern\
Idempotent Consumers\
Dead Letter Queues

------------------------------------------------------------------------

# Frontend Architecture

Frontend applications use:

Blazor Server or Blazor WebAssembly\
MudBlazor UI components

Rules:

-   UI must use MudBlazor components
-   avoid custom CSS when possible
-   API calls must use typed HttpClient services
-   backend DTOs must map to ViewModels

State management:

Scoped StateContainer services or observable state.

Dialogs:

IDialogService

Notifications:

MudSnackbar

------------------------------------------------------------------------

# Testing Strategy

Testing is mandatory.

Unit tests:

xUnit\
Moq or NSubstitute

Integration tests:

Microsoft.AspNetCore.TestHost

Infrastructure testing:

DotNet.Testcontainers

Containers used during tests:

PostgreSQL\
MongoDB\
Redis\
RabbitMQ

Frontend component testing:

bUnit

------------------------------------------------------------------------

# DevOps & Platform

The platform uses **.NET Aspire** for orchestration.

AppHost is responsible for:

-   service registration
-   container configuration
-   dependency wiring
-   observability setup

Infrastructure stack:

Docker\
Kubernetes (optional)\
GitHub Actions or Azure DevOps

Observability:

OpenTelemetry\
Prometheus\
Grafana\
Aspire Dashboard

------------------------------------------------------------------------

# Coding Standards

All code must follow:

-   File‑scoped namespaces
-   Primary constructors
-   Record DTOs where appropriate
-   Async/await for IO operations

Avoid:

-   synchronous database calls
-   tightly coupled modules
-   cross‑service database access

------------------------------------------------------------------------

# Architecture Guardrails

Agents must always enforce:

Vertical Slice Architecture\
FastEndpoints APIs\
Polyglot persistence\
Event‑driven integration\
Redis caching where appropriate

Never generate:

-   layered monolithic architectures
-   shared databases across microservices
-   direct HttpClient usage inside UI components

------------------------------------------------------------------------

# Agent Collaboration

Agents working in this repository:

System Solution Architect\
C# Backend Architect\
Blazor Frontend Architect\
Distributed Messaging Architect\
Testing & Quality Agent\
Aspire DevOps Platform Agent\
Architecture Orchestrator

The **Orchestrator Agent** coordinates the others when tasks span
multiple domains.

------------------------------------------------------------------------

# Goal

Ensure all generated solutions remain:

consistent\
scalable\
maintainable\
aligned with the platform architecture.
