FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project files into the container
COPY ["SimpleTaskManager.WebApi/SimpleTaskManager.WebApi.csproj", "SimpleTaskManager.WebApi/"]
COPY ["SimpleTaskManager.BLL/SimpleTaskManager.BLL.csproj", "SimpleTaskManager.BLL/"]
COPY ["SimpleTaskManager.DAL/SimpleTaskManager.DAL.csproj", "SimpleTaskManager.DAL/"]
COPY ["SimpleTaskManager.xUnitTests/SimpleTaskManager.xUnitTests.csproj", "SimpleTaskManager.xUnitTests/"]

# Restore project dependencies inside the container
RUN dotnet restore "SimpleTaskManager.WebApi/SimpleTaskManager.WebApi.csproj"

# Copy the entire source code into the container
COPY . .

# Build the project
WORKDIR "/src/SimpleTaskManager.WebApi"
RUN dotnet build "SimpleTaskManager.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleTaskManager.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set the working directory for the runtime environment
WORKDIR /app

# Copy the published files from the build container to the runtime container
COPY --from=publish /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "SimpleTaskManager.WebApi.dll"]
