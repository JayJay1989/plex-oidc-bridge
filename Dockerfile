# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only project files first for better layer caching
COPY PlexSSO/*.csproj PlexSSO/
RUN dotnet restore PlexSSO/PlexSSO.csproj

# Copy the rest and build
COPY . .
RUN dotnet publish PlexSSO/PlexSSO.csproj -c Release -o /app /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
# If your app uses ASPNETCORE_URLS, set it here (Kestrel to 0.0.0.0:8080)
ENV ASPNETCORE_URLS=http://+:8080

# (Optional) drop privileges
# RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
# USER appuser

COPY --from=build /app .
ENTRYPOINT ["dotnet", "PlexSSO.dll"]
