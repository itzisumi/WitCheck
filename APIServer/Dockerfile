FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["APIServer/APIServer.csproj", "APIServer/"]
COPY ["Database/Database.csproj",   "Database/"]
COPY ["Shared/Shared.csproj",       "Shared/"]
RUN dotnet restore "APIServer/APIServer.csproj"
COPY . .
WORKDIR "/src/APIServer"
RUN dotnet publish "APIServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "APIServer.dll"]
