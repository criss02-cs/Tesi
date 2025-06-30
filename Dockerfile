# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution file and project files
COPY Tesi.sln ./
COPY Tesi.Blazor/Tesi.Blazor.Server/Tesi.Blazor.Server.csproj ./Tesi.Blazor/Tesi.Blazor.Server/
COPY Tesi.Blazor/Tesi.Blazor.Client/Tesi.Blazor.Client.csproj ./Tesi.Blazor/Tesi.Blazor.Client/
COPY Tesi.Blazor/Tesi.Blazor.Shared/Tesi.Blazor.Shared.csproj ./Tesi.Blazor/Tesi.Blazor.Shared/
COPY Tesi.Solvers/Tesi.Solvers.csproj ./Tesi.Solvers/
COPY Tesi.JSPSolver/Tesi.JSPSolver.csproj ./Tesi.JSPSolver/

# Copy external DLL references
COPY Tesi.Dll/ ./Tesi.Dll/

# Restore dependencies
RUN dotnet restore Tesi.Blazor/Tesi.Blazor.Server/Tesi.Blazor.Server.csproj

# Copy the entire source code
COPY . ./

# Ensure wwwroot files are copied
COPY Tesi.Blazor/Tesi.Blazor.Server/wwwroot/ ./Tesi.Blazor/Tesi.Blazor.Server/wwwroot/
COPY Tesi.Blazor/Tesi.Blazor.Client/wwwroot/ ./Tesi.Blazor/Tesi.Blazor.Client/wwwroot/

# Build and publish the application
RUN dotnet publish Tesi.Blazor/Tesi.Blazor.Server/Tesi.Blazor.Server.csproj -c Release -o out --no-restore

# Use the official .NET 8.0 runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install required dependencies for native libraries
RUN apt-get update && apt-get install -y \
    libc6-dev \
    libgcc-s1 \
    libstdc++6 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the published application
COPY --from=build-env /app/out .

# Ensure wwwroot static files are available
COPY --from=build-env /app/Tesi.Blazor/Tesi.Blazor.Server/wwwroot ./wwwroot

# Copy Gurobi license file if it exists
COPY --from=build-env /app/Tesi.Blazor/Tesi.Blazor.Server/gurobi.prod.lic ./gurobi.prod.lic

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Tesi.Blazor.Server.dll"]
