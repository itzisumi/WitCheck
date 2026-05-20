# Blazor WASM: build with dotnet, serve static output with nginx
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["WebGUI/WebGUI.csproj",     "WebGUI/"]
COPY ["APICaller/APICaller.csproj","APICaller/"]
COPY ["Database/Database.csproj", "Database/"]
COPY ["Shared/Shared.csproj",     "Shared/"]
RUN dotnet restore "WebGUI/WebGUI.csproj"
COPY . .
WORKDIR "/src/WebGUI"
RUN dotnet publish "WebGUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY WebGUI/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
