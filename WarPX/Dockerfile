FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/WarPX.Domain/WarPX.Domain.csproj", "src/WarPX.Domain/"]
COPY ["src/WarPX.Application/WarPX.Application.csproj", "src/WarPX.Application/"]
COPY ["src/WarPX.Infrastructure/WarPX.Infrastructure.csproj", "src/WarPX.Infrastructure/"]
COPY ["src/WarPX.Api/WarPX.Api.csproj", "src/WarPX.Api/"]

RUN dotnet restore "src/WarPX.Api/WarPX.Api.csproj"

COPY . .
RUN dotnet publish "src/WarPX.Api/WarPX.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WarPX.Api.dll"]