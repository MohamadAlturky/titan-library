FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Titan.Library.sln .
COPY Titan.Library.Api/Titan.Library.Api.csproj Titan.Library.Api/
COPY Titan.Library.Application/Titan.Library.Application.csproj Titan.Library.Application/
COPY Titan.Library.Common/Titan.Library.Common.csproj Titan.Library.Common/
COPY Titan.Library.Contracts/Titan.Library.Contracts.csproj Titan.Library.Contracts/
COPY Titan.Library.Domain/Titan.Library.Domain.csproj Titan.Library.Domain/
COPY Titan.Library.Endpoints/Titan.Library.Endpoints.csproj Titan.Library.Endpoints/
COPY Titan.Library.Infrastructure/Titan.Library.Infrastructure.csproj Titan.Library.Infrastructure/
COPY tests/Titan.Library.Application.UnitTests/Titan.Library.Application.UnitTests.csproj tests/Titan.Library.Application.UnitTests/
COPY tests/Titan.Library.Domain.UnitTests/Titan.Library.Domain.UnitTests.csproj tests/Titan.Library.Domain.UnitTests/

RUN dotnet restore

COPY . .

RUN dotnet test tests/Titan.Library.Application.UnitTests/Titan.Library.Application.UnitTests.csproj \
    --no-restore \
    --configuration Release && \
    dotnet test tests/Titan.Library.Domain.UnitTests/Titan.Library.Domain.UnitTests.csproj \
    --no-restore \
    --configuration Release

RUN dotnet publish Titan.Library.Api/Titan.Library.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Titan.Library.Api.dll"]
