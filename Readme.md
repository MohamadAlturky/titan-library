# Titan Library Api

> in the following sections we will explain the system and how we build it.

## Common layer

you can see it as a generic layer we can ship it to any project
it contains the conventions and the abstractions for the architecture.

## Domain Layer

here is the important layer where we will explain the buisness rules.
it is splited by subdomains and contains

- the factories for enforce the buisness rules
- the abstractions for data access with the snapshot pattern to split the data concerns from the domain concerns.

for the users subdomain we have the users with inheritance behavior to abstract the users operations
from the concrete operations for the author and customer.

## Contracts Layer

contains some of the abstractions that is belongs to the domain like dto, might be service interfaces.

why i always use this layer
the dtos is a very complex thing in app, i always have problem handling the different types of dtos in the system
because some use cases need returns some data diferent from other use cases, and some times you need to create a hiraricy between these dtos to not repeat yourself and not multible models for the same thing.

## Application Layer

contains the use cases of the app on top of the domain layer
with the abstractions for the commands and the quries CQRS.
we have implemented an abstraction on top on the commands to handle the validation using the step design pattern.
the application layer uses the result pattern to identify success and failed senarios and return a proper message using predefined message key for each senario.

## Endpoints layer

contains the minimal apis implementation that depends of the application layer that defines the http endpoints for getting the right data from the user and call the right command or query.

## Infrastructure layer

contains the implementation for the external concerns of the app:

- db utils and repositoies implementation with ado.net the tabels structure defined in configuration classes
and there is some ADO extention methods to helps the development
- caching using redis

## Api Layer

this is the start up project for the backend here it contains no implementation just defines the required things to run the app.
uses scalar for documentation.

## Tools

### Db migration creator

one of the biggest problems in the development when using ado is how we can make migrations to the db to execute DDL queries.
in ef core it handles that internally with its own migration tools dotnet-ef so we copied the approach by
creating an abstraction for IMigration and the app on the start up will look for the migrations table and apply the new migrations that is not applied like efcore
the responsibily for the tool is just creating the file in a convention that runs without any error.
the responsibilty of the develper is defining the DDL inside the generated class.

## Web layer

implemented using react single page app, with different routs and layouts for customers authors and admins.
contains services uses axois to define the api calls and uses axios interceptors to handle auth headers and invalid token cases.
there is custom hooks for auth and theme.

## App Startup Seeders

when run the app there is three operations on db:

- apply migrations
- seed messages
- seed admin

## Backend messages handling

the application layer returns message keys to the api layer
all of the message keys are defined in a static class
it is seeded in the startup
the admin can edit this messages
and in the api layer it takes a message key and uses the api response resolver to return a proper response.

## Exception Handling

we use an exception middle ware to handle all of the unexpeted errors log them and return a message to the user

## Logging and Opentelemetry

we used serilog and seq as a sink to add logging to the system in a structured way and see the logs in a centralized ui.

## Docker setup

we used docker compose to define the services of the app.

- front end
- backend
- postegres
- redis
- promethuse for store opentelemetry data
- redis opentelemetry data exporter
- postegres opentelemetry data exporter
- grafana for view opentelemetry data exporter in dashboards
- seq service for store the logs and serve the ui

and we defined the volums for them to ensure persistence.
