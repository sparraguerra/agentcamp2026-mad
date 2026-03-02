FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/DndCopilot.Core/DndCopilot.Core.csproj DndCopilot.Core/
COPY src/DndCopilot.Infrastructure/DndCopilot.Infrastructure.csproj DndCopilot.Infrastructure/
COPY src/DndCopilot.Api/DndCopilot.Api.csproj DndCopilot.Api/
RUN dotnet restore DndCopilot.Api/DndCopilot.Api.csproj

COPY src/DndCopilot.Core/ DndCopilot.Core/
COPY src/DndCopilot.Infrastructure/ DndCopilot.Infrastructure/
COPY src/DndCopilot.Api/ DndCopilot.Api/
RUN dotnet publish DndCopilot.Api/DndCopilot.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY docs/NPCS.md /app/docs/NPCS.md
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "DndCopilot.Api.dll"]
