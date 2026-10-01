# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first (cached unless the .csproj changes)
COPY PersonalBudgetTracker.csproj ./
RUN dotnet restore PersonalBudgetTracker.csproj

# Copy the rest of the source and publish
COPY . ./
RUN dotnet publish PersonalBudgetTracker.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

# SQLite database lives in /app/data
RUN mkdir -p /app/data
ENV ASPNETCORE_ENVIRONMENT=Production \
    LANG=en_US.UTF-8 \
    LC_ALL=en_US.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ConnectionStrings__DefaultConnection="Data Source=/app/data/budgettracker.db"

# Render (and most hosts) pass the port in $PORT; Program.cs reads it. 8080 is the fallback.
EXPOSE 8080
ENTRYPOINT ["dotnet", "PersonalBudgetTracker.dll"]
