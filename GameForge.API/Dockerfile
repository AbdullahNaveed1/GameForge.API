# ==========================================
# STAGE 1: Base Runtime Environment
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

USER $APP_UID

# ==========================================
# STAGE 2: Build & Restore Dependencies
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["GameForge.API.csproj", "./"]
RUN dotnet restore "GameForge.API.csproj"

COPY . .
RUN dotnet build "GameForge.API.csproj" -c Release -o /app/build

# ==========================================
# STAGE 3: Publish Optimized Binaries
# ==========================================
FROM build AS publish
RUN dotnet publish "GameForge.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ==========================================
# STAGE 4: Final Lean Production Image
# ==========================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GameForge.API.dll"]
