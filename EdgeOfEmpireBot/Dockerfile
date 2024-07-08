# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory
WORKDIR /EdgeOfEmpireBot

# Copy the project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /EdgeOfEmpireBot

# Copy the build output from the previous stage
COPY --from=build-env /EdgeOfEmpireBot/out .

# Expose the port the application will run on
EXPOSE 80

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "EdgeOfEmpireBot.dll"]
