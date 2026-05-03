# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the .csproj file and restore dependencies (this layers the cache)
COPY *.csproj ./
RUN dotnet restore

# 
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy only the compiled output from the build-env stage
COPY --from=build-env /app/out .

# Expose the port the app runs on
EXPOSE 5000

# Start the application
ENTRYPOINT ["dotnet", "WebApi.dll"]

