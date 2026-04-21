# Good Hamburger

Sistema de pedidos para a lanchonete **Good Hamburger** — desafio técnico para desenvolvedor C#.

Construído com **ASP.NET Core 8**, **Entity Framework Core + SQLite** e **Blazor WebAssembly**, seguindo arquitetura em camadas com separação clara entre domínio, aplicação, infraestrutura e apresentação.

---

## Funcionalidades

- Cardápio com sanduíches e acompanhamentos
- Montagem de pedido com cálculo de desconto em tempo real
- CRUD completo de pedidos via API REST
- Regras de desconto por combo aplicadas no domínio
- Validação de itens duplicados com resposta clara (HTTP 422)
- Painel administrativo com métricas, filtros, edição e remoção
- Testes unitários cobrindo todas as regras de negócio

---

## Como executar

### Opção 1 — Docker (recomendado)

Requer apenas Docker instalado. Sem necessidade de .NET SDK no host.

```bash
docker compose up -d --build
```

| Serviço | URL |
|---|---|
| Frontend (Blazor) | http://localhost:5200 |
| API REST | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| Health check | http://localhost:5000/health |

```bash
# Acompanhar logs
docker compose logs -f

# Parar
docker compose down

# Parar e remover o banco de dados
docker compose down -v
```

### Opção 2 — Local com .NET SDK

**Pré-requisitos:** .NET 8 SDK

```bash
# Terminal 1 — API
cd src/GoodHamburger.API
dotnet run
# API disponível em http://localhost:5000

# Terminal 2 — Frontend
cd src/GoodHamburger.Web
dotnet run
# Frontend disponível em https://localhost:7200
```

### Testes automatizados

```bash
dotnet test GoodHamburger.sln
```

### Script de validação

Executa build, sobe os containers e testa os endpoints principais:

```bash
# Apenas smoke tests (sem testes unitários)
./scripts/validate.sh --skip-tests

# Validação completa incluindo testes automatizados
./scripts/validate.sh
```

---

## Arquitetura

```
src/
├── GoodHamburger.Domain/          # Entidades, enums, regras de negócio puras
├── GoodHamburger.Application/     # Use cases, interfaces, DTOs, Result<T>
├── GoodHamburger.Infrastructure/  # EF Core + SQLite, repositórios, migrations
├── GoodHamburger.API/             # Controllers, middleware, configuração
└── GoodHamburger.Web/             # Blazor WebAssembly (frontend)

tests/
└── GoodHamburger.Tests/           # xUnit + FluentAssertions
```

### Fluxo de dependências

```
Domain  ←  Application  ←  Infrastructure
                        ←  API
Web  →  API (via HTTP)
```

Cada camada depende apenas das camadas à sua esquerda. O **Domain** não referencia nenhum framework — as regras de negócio são testáveis com `new` direto, sem container IoC.

### Decisões técnicas

**Arquitetura em camadas ao invés de Clean Architecture completa**
O desafio tem escopo definido. Introduzir CQRS, MediatR ou múltiplas interfaces por use case seria over-engineering. A separação em 4 camadas já demonstra conhecimento de separação de responsabilidades sem adicionar complexidade desnecessária.

**SQLite ao invés de SQL Server / PostgreSQL**
Zero configuração de infraestrutura. O banco é criado automaticamente na primeira execução via `db.Database.Migrate()`. Para um avaliador rodar o projeto, basta `docker compose up`.

**`Result<T>` com `ErrorCode` tipado ao invés de exceções no fluxo normal**
Exceções são para situações inesperadas. "Pedido não encontrado" é um resultado esperado — o `Result<T>` com `ErrorCode.NotFound` ou `ErrorCode.InvalidInput` permite que o controller mapeie para o HTTP status correto sem string matching.

**Blazor WebAssembly ao invés de Server**
O enunciado diz "frontend consumindo a API". WASM executa no browser e faz chamadas HTTP diretamente, que é o modelo mais alinhado com o que foi pedido.

**Dois formatos aceitos no request de pedido**
O frontend usa `hasFries`/`hasDrink` (booleanos, naturalmente sem duplicatas). Para demonstrar explicitamente a validação de duplicatas pedida pelo desafio, a API também aceita `items: ["Fries", "Fries"]` — nesse caso retorna HTTP 422 com mensagem clara via `DuplicateItemException`.

---

## Cardápio e regras de desconto

### Itens disponíveis

| Item | Categoria | Preço |
|---|---|---|
| X Burger | Sanduíche | R$ 5,00 |
| X Egg | Sanduíche | R$ 4,50 |
| X Bacon | Sanduíche | R$ 7,00 |
| Batata Frita | Acompanhamento | R$ 2,00 |
| Refrigerante | Bebida | R$ 2,50 |

### Descontos por combo

| Combinação | Desconto |
|---|---|
| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata | 10% |
| Apenas sanduíche | 0% |

Cada pedido contém exatamente **um sanduíche**. Batata e refrigerante são opcionais. Itens duplicados retornam HTTP 422 com mensagem de erro clara.

---

## Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/menu` | Lista todos os itens do cardápio |
| `POST` | `/api/orders` | Cria pedido e calcula desconto |
| `GET` | `/api/orders` | Lista todos os pedidos |
| `GET` | `/api/orders/{id}` | Busca pedido por ID |
| `PUT` | `/api/orders/{id}` | Atualiza pedido e recalcula total |
| `DELETE` | `/api/orders/{id}` | Remove pedido |
| `GET` | `/health` | Health check da API |

Documentação interativa disponível via Swagger em `/swagger`.

---

## O que ficou fora

- **Autenticação/autorização** — o painel admin não tem proteção de acesso. Fora do escopo do desafio.
- **Paginação** — a listagem retorna todos os pedidos. Adequado para a escala do desafio.
