# Titan Library API

> The following sections explain the system and how it was built.

## Common Layer

Think of this as a generic layer that can be shipped to any project. It contains the conventions and abstractions for the overall architecture.

## Domain Layer

This is the most important layer, where the business rules live. It is split by subdomains and contains:

- Factories that enforce the business rules
- Abstractions for data access, using the snapshot pattern to separate data concerns from domain concerns

For the users subdomain, inheritance is used to abstract shared user operations away from the concrete implementations for authors and customers.

## Contracts Layer

Contains the DTOs used to transfer data between layers.

**Why this layer exists:**
DTOs are surprisingly complex to manage in a large application. Different use cases often need to return slightly different shapes of data, and without a dedicated place for them, you end up either duplicating models or creating an uncontrolled mess. This layer provides a hierarchy for DTOs so you can reuse and extend them cleanly.

## Application Layer

Contains the use cases of the application, built on top of the domain layer, with abstractions for commands and queries (CQRS).

A validation abstraction is implemented on top of commands using the step design pattern. The application layer uses the result pattern to distinguish between success and failure scenarios and returns a descriptive message using predefined message keys for each case.

## Endpoints Layer

Contains the Minimal API implementation. It depends on the application layer and defines the HTTP endpoints responsible for receiving data from the client and dispatching the appropriate command or query.

## Infrastructure Layer

Contains the implementation for all external concerns of the application:

- Database utilities and repository implementations using ADO.NET, with table structure defined in configuration classes and ADO extension methods to speed up development
- Caching using Redis

## API Layer

This is the startup project for the backend. It contains no business logic — it only wires up everything needed to run the application. Uses Scalar for API documentation.

## Tools

### DB Migration Creator

One of the bigger challenges when using ADO.NET is managing database migrations for DDL queries. Entity Framework Core handles this internally with its own `dotnet-ef` tooling, so we took the same approach: there is an `IMigration` abstraction, and on startup the app checks the migrations table and applies any pending migrations — just like EF Core.

The tool's responsibility is generating a migration file in the correct convention so it runs without errors. The developer's responsibility is filling in the DDL inside the generated class.

## Web Layer

Implemented as a React single-page application with separate routes and layouts for customers, authors, and admins. Contains service modules that use Axios to define API calls, along with Axios interceptors to handle auth headers and invalid token scenarios and 500 errors and general failed messages. Custom hooks are provided for authentication and theming.

## App Startup Seeders

When the application starts, three operations run against the database:

- Apply migrations
- Seed messages
- Seed admin

## Backend Message Handling

The application layer returns message keys to the API layer. All message keys are defined in a static class, seeded on startup, and can be edited by the admin. The API layer receives a message key and uses an API response resolver to return an appropriate response to the client.

## Exception Handling

A middleware component handles all unexpected exceptions — it logs the error and returns a user-friendly message.

## Logging and OpenTelemetry

Serilog is used for structured logging with Seq as the sink, giving a centralized UI for viewing logs.

## Docker Setup

Docker Compose is used to define all application services:

- Frontend
- Backend
- PostgreSQL
- Redis
- Prometheus for storing OpenTelemetry data
- Redis OpenTelemetry exporter
- PostgreSQL OpenTelemetry exporter
- Grafana for visualizing OpenTelemetry data in dashboards
- Seq for storing logs and serving the UI

Volumes are defined for all stateful services to ensure data persistence.
