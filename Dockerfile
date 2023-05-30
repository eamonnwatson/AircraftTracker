#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AircraftTracker.csproj", "."]
RUN dotnet restore "./AircraftTracker.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AircraftTracker.csproj" -c Release -o /app/build -r linux-arm64 --no-self-contained

FROM build AS publish
RUN dotnet publish "AircraftTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false -r linux-arm64 --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AircraftTracker.dll"]