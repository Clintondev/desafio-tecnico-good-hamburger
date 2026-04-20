# Good Hamburger

Sistema de pedidos para a lanchonete Good Hamburger.

## Como executar (recomendado: Docker)

### Pre-requisitos
- Docker
- Docker Compose

### Subir API + Frontend

```bash
docker compose up -d --build
```

### URLs
- Frontend: http://localhost:5200
- API: http://localhost:5000
- Swagger JSON: http://localhost:5000/swagger/v1/swagger.json

### Logs

```bash
docker compose logs -f goodhamburger-api
docker compose logs -f goodhamburger-web
```

### Parar

```bash
docker compose down
```

### Parar e limpar banco

```bash
docker compose down -v
```

## Validacao rapida

Executa build/subida dos containers e smoke tests dos endpoints principais:

```bash
./scripts/validate.sh --skip-tests
```

Executa a validacao completa, incluindo testes automatizados:

```bash
./scripts/validate.sh
```

## Como executar localmente (alternativo)

### Pre-requisitos
- .NET 8 SDK
- dotnet-ef

### API

```bash
cd src/GoodHamburger.API
dotnet run
```

### Frontend

```bash
cd src/GoodHamburger.Web
dotnet run
```

### Testes

```bash
cd ..
dotnet test GoodHamburger.sln
```

## Decisoes de arquitetura

- Arquitetura em camadas: Domain -> Application -> Infrastructure -> API.
- Domain sem dependencia de framework para regras de negocio puras.
- Infrastructure com EF Core + SQLite para persistencia simples.
- API REST com controllers e middleware global de excecao.
- Blazor WebAssembly para o frontend consumindo a API via HttpClient.
- Docker Compose para execucao padronizada sem instalar SDK no host.

## Regras de negocio

- Pedido sempre possui 1 sanduiche.
- Descontos:
  - 20%: sanduiche + batata + refrigerante
  - 15%: sanduiche + refrigerante
  - 10%: sanduiche + batata
  - 0%: apenas sanduiche

## O que ficou fora

- Autenticacao/autorizacao para area administrativa.
- Paginacao na listagem de pedidos.
- Upload de imagens reais dos itens.
