#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=linux/arm mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 8081
EXPOSE 8008

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first
COPY ["HomeUtilities/HomeUtilities.csproj", "HomeUtilities/"]
COPY ["HomeUtilities.Common/HomeUtilities.Common.csproj", "HomeUtilities.Common/"]
COPY ["SpoonacularDAL/SpoonacularDAL.csproj", "SpoonacularDAL/"]

# Restore dependencies for all projects
RUN dotnet restore "HomeUtilities/HomeUtilities.csproj"
RUN dotnet restore "HomeUtilities.Common/HomeUtilities.Common.csproj"
RUN dotnet restore "SpoonacularDAL/SpoonacularDAL.csproj"

# Copy source code
COPY . .

# Explicitly clean obj and bin folders
RUN find /src -name "obj" -type d -exec rm -rf {} +
RUN find /src -name "bin" -type d -exec rm -rf {} +

# Build each project explicitly using full paths
RUN dotnet build "/src/HomeUtilities.Common/HomeUtilities.Common.csproj" -c $BUILD_CONFIGURATION -o /app/build/HomeUtilities.Common
RUN dotnet build "/src/SpoonacularDAL/SpoonacularDAL.csproj" -c $BUILD_CONFIGURATION -o /app/build/SpoonacularDAL
RUN dotnet build "/src/HomeUtilities/HomeUtilities.csproj" -c $BUILD_CONFIGURATION -o /app/build/HomeUtilities

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/HomeUtilities
RUN dotnet publish "HomeUtilities.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeUtilities.dll"]