# Use the official ASP.NET runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build Stage: Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy only the project file and restore dependencies to optimize caching
COPY ["WeatherAPI/WeatherAPI.csproj", "WeatherAPI/"]
RUN dotnet restore "WeatherAPI/WeatherAPI.csproj"

# Copy the entire application and build it
COPY . .
WORKDIR "/src/WeatherAPI"
RUN dotnet build "WeatherAPI.csproj" -c Release -o /app/build

# Publish Stage: Publish the application using the previously built image
FROM build AS publish
RUN dotnet publish "WeatherAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage: Use the base image and copy the published files
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the container entry point
ENTRYPOINT ["dotnet", "WeatherAPI.dll"]
