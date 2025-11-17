# Estrutura Reorganizada do Projeto

## Organização das Pastas

```
AnfMicroserviceSolution/
├── backend/                    # Todo o código backend (.NET)
│   ├── src/                   # Código-fonte dos projetos
│   │   ├── Email.Api/
│   │   ├── Email.Worker/
│   │   ├── Email.ApiGateway/
│   │   ├── Email.Application/
│   │   ├── Email.Domain/
│   │   └── Email.Infrastructure/
│   ├── tests/                 # Testes unitários
│   │   └── Email.Domain.Tests/
│   ├── deploy/                # Configurações de deploy
│   │   └── otel-collector-config.yaml
│   ├── docs/                 # Documentação técnica
│   └── EmailMicroserviceSolution.sln
├── frontend/                  # Frontend Angular
│   └── email-front/
├── scripts/                   # Scripts utilitários
│   ├── infra-up.bat
│   └── commit-angular.bat
├── docs/                      # Documentação geral
│   ├── COMO-RODAR.md
│   └── ESTRUTURA-REORGANIZADA.md
├── docker-compose.yml         # Orquestração Docker (raiz)
└── .github/                   # GitHub Actions workflows
    └── workflows/
```

## Correções Aplicadas

### 1. Docker Compose (`docker-compose.yml`)
- ✅ Atualizado `context` para `./backend` em todos os serviços
- ✅ Corrigido caminho do volume do otel-collector: `./backend/deploy/otel-collector-config.yaml`

### 2. Dockerfiles
- ✅ Atualizado `dotnet restore` para usar `EmailMicroserviceSolution.sln` diretamente
- ✅ Contexto de build agora é `./backend`

### 3. PreBuild Event (`Email.Worker.csproj`)
- ✅ Ajustado caminho do script: `$(SolutionDir)..` para acessar `scripts/` na raiz

### 4. GitHub Actions Workflows
- ✅ **ci.yml**: Atualizado caminhos para `backend/EmailMicroserviceSolution.sln`
- ✅ **ci.yml**: Atualizado context e dockerfile paths para `./backend`
- ✅ **cd.yml**: Adicionado `context` na matrix de serviços
- ✅ **docker-compose-test.yml**: Mantido (usa docker-compose.yml da raiz)

### 5. Organização de Arquivos
- ✅ Removido `.sln` duplicado da raiz
- ✅ Movido `tests/` para `backend/tests/`
- ✅ Consolidado documentação em `docs/`
- ✅ Criado `README.md` principal na raiz

## Como Usar

### Desenvolvimento Local

1. **Subir infraestrutura:**
   ```bash
   scripts\infra-up.bat
   ```

2. **Rodar API (Visual Studio):**
   - Abrir `backend/EmailMicroserviceSolution.sln`
   - Configurar `Email.Api` como startup project
   - Pressionar F5

3. **Rodar Worker (Visual Studio):**
   - Configurar `Email.Worker` como segundo projeto de inicialização
   - Ou rodar separadamente

4. **Rodar Frontend:**
   ```bash
   cd frontend/email-front
   ng serve
   ```

### Docker Compose

```bash
# Subir tudo
docker compose up -d

# Subir apenas infraestrutura
docker compose up rabbitmq smtp-service otel-collector -d
```

## Verificação

- ✅ Caminhos do Docker Compose corrigidos
- ✅ Caminhos dos Dockerfiles corrigidos
- ✅ Caminhos dos GitHub Actions corrigidos
- ✅ PreBuild event ajustado
- ✅ Estrutura organizada e consistente
- ✅ Documentação consolidada
- ✅ Testes movidos para backend

