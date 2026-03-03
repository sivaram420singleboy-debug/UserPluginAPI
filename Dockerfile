# ---------------- BUILD STAGE ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore
COPY UserPluginAPI.csproj ./
RUN dotnet restore

# Copy everything
COPY . .

# Publish ONLY main project
RUN dotnet publish UserPluginAPI.csproj -c Release -o /app/publish


# ---------------- RUNTIME STAGE ----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UserPluginAPI.dll"]