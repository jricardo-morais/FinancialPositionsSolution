FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore FinancialPositionsSolution.sln

FROM build AS publish
WORKDIR "/src/FinancialPositions.ConsoleApp"
RUN dotnet publish "FinancialPositions.ConsoleApp.csproj" -c Release -o /app/publish
COPY FinancialPositions.ConsoleApp/appsettings.json /app/publish/appsettings.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialPositions.ConsoleApp.dll"]
