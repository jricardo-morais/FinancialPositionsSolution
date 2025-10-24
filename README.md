# Solu��o FinancialPositions

Este reposit�rio cont�m uma solu��o em .NET 8 que implementa um sistema simples para buscar, processar e expor posi��es financeiras de clientes do Andbank.

## Projetos
- `FinancialPositions.Api` - Web API ASP.NET Core que exp�e endpoints para consulta de posi��es financeiras.
- `FinancialPositions.ConsoleApp` - Worker em background que busca posi��es de uma API externa e as persiste no banco de dados.
- `FinancialPositions.Infrastructure` - DbContext do EF Core e implementa��es de reposit�rio.
- `FinancialPositions.Domain` - Entidades de dom�nio.
- `FinancialPositions.Application` - Servi�os de aplica��o (casos de uso) que orquestram opera��es de reposit�rio.

## Tecnologias
- .NET 8
- C# 12
- ASP.NET Core Web API
- Entity Framework Core (provedor Npgsql para PostgreSQL)
- Npgsql
- Polly (pol�ticas de resili�ncia para HTTP)
- OpenAPI / Swagger

## Pr�-requisitos
- SDK .NET 8 instalado: https://dotnet.microsoft.com/pt-br/download/dotnet/8.0
- Banco de dados PostgreSQL (ou Docker) acess�vel a partir da sua m�quina
- Opcional: Docker (para rodar PostgreSQL localmente ou containerizar a API)

## Configura��o do ambiente
- Atualize `appsettings.json` nos projetos `ConsoleApp` e `Api` ajustando a connection string e as configura��es da API externa.
- Certifique-se de que as chaves abaixo estejam configuradas no `appsettings.json`:
  - `ConnectionStrings:DefaultConnection` - connection string do PostgreSQL
  - `ApiSettings:Url` - URL base da API externa para buscar posi��es
  - `ApiSettings:ApiKey` - valor da chave de API usada ao chamar a API externa

## Executando com container docker  (opcional)
# a partir do diret�rio raiz da Solution executar os seguintes comando no docker

# Subir containers em background
docker-compose up -d --build


## Banco de dados
- Execute migra��es do EF Core no PostgreSQL.
- Exemplo usando `dotnet ef`:

```bash
# a partir do diret�rio do projeto Infrastructure
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -p FinancialPositions.Infrastructure -s FinancialPositions.Api
dotnet ef database update -p FinancialPositions.Infrastructure -s FinancialPositions.Api
```

## Executando localmente
1. Inicie a API (no projeto `FinancialPositions.Api`):
   - Pelo Visual Studio: execute o projeto (a UI do Swagger abrir� automaticamente).
   - Pela linha de comando:
     ```bash
     cd FinancialPositions.Api
     dotnet run
     ```
2. Inicie o worker Console (no projeto `FinancialPositions.ConsoleApp`):
   - Pela linha de comando:
     ```bash
     cd FinancialPositions.ConsoleApp
     dotnet run
     ```

## Uso da API
- A UI do Swagger fica dispon�vel em `/swagger` quando a API estiver em execu��o.
- Endpoints de exemplo:
  - `GET /api/positions/top10`
  - `GET /api/positions/client/{clientId}`
  - `GET /api/positions/client/{clientId}/summary`
