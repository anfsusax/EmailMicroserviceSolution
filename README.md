# Email Microservice Solution

Solução completa de microserviços para envio de e-mails com arquitetura moderna, incluindo API REST, Worker assíncrono, API Gateway e frontend Angular.

## 📋 Estrutura do Projeto

```
AnfMicroserviceSolution/
├── backend/                    # Código backend (.NET 8)
│   ├── src/                   # Projetos da solução
│   │   ├── Email.Api/         # API REST principal
│   │   ├── Email.Worker/      # Worker para processamento assíncrono
│   │   ├── Email.ApiGateway/  # API Gateway (Ocelot)
│   │   ├── Email.Application/ # Camada de aplicação
│   │   ├── Email.Domain/      # Entidades e regras de negócio
│   │   └── Email.Infrastructure/ # Implementações (RabbitMQ, SMTP, etc)
│   ├── tests/                 # Testes unitários
│   ├── deploy/                # Configurações de deploy
│   ├── docs/                  # Documentação técnica
│   └── EmailMicroserviceSolution.sln
├── frontend/                   # Frontend Angular
│   └── email-front/           # Aplicação Angular
├── scripts/                    # Scripts utilitários
├── docs/                       # Documentação geral
├── docker-compose.yml          # Orquestração Docker
└── .github/                    # GitHub Actions (CI/CD)
```

## 🚀 Início Rápido

### Pré-requisitos
- .NET 8 SDK
- Node.js 18+ e npm
- Docker Desktop
- Visual Studio 2022 ou VS Code

### Passos

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd AnfMicroserviceSolution
   ```

2. **Suba a infraestrutura**
   ```bash
   scripts\infra-up.bat
   ```

3. **Execute a API**
   - Abra `backend/EmailMicroserviceSolution.sln` no Visual Studio
   - Configure `Email.Api` como projeto de inicialização
   - Pressione F5

4. **Execute o Worker** (opcional)
   - Configure `Email.Worker` como segundo projeto de inicialização

5. **Execute o Frontend**
   ```bash
   cd frontend/email-front
   npm install
   ng serve
   ```

## 📚 Documentação

- [Como Rodar o Projeto](docs/COMO-RODAR.md)
- [Estrutura Reorganizada](docs/ESTRUTURA-REORGANIZADA.md)
- [Guia do Desenvolvedor](backend/docs/dev-desperate-guide.md)

## 🏗️ Arquitetura

### Backend
- **Email.Api**: API REST com autenticação JWT
- **Email.Worker**: Processamento assíncrono de e-mails via RabbitMQ
- **Email.ApiGateway**: API Gateway usando Ocelot
- **Email.Application**: Casos de uso e handlers
- **Email.Domain**: Entidades e value objects
- **Email.Infrastructure**: Implementações de infraestrutura

### Frontend
- **Angular 18+**: Framework frontend
- **Standalone Components**: Arquitetura moderna
- **Reactive Forms**: Formulários reativos
- **HTTP Client**: Comunicação com API

### Infraestrutura
- **RabbitMQ**: Fila de mensagens
- **Mailhog**: Servidor SMTP para desenvolvimento
- **OpenTelemetry**: Observabilidade (métricas e traces)
- **Grafana**: Visualização de métricas
- **ElasticSearch**: Armazenamento de logs (opcional)

## 🔧 Tecnologias

### Backend
- .NET 8
- ASP.NET Core
- Entity Framework Core
- RabbitMQ.Client
- Serilog
- OpenTelemetry
- Ocelot

### Frontend
- Angular 18+
- TypeScript
- RxJS
- Angular Material (planejado)

### DevOps
- Docker & Docker Compose
- GitHub Actions
- Trivy (security scanning)

## 📝 Endpoints da API

### Autenticação
- `POST /api/Auth/token` - Gerar token JWT

### E-mails
- `POST /api/Emails` - Enviar e-mail (requer autenticação)
- `GET /api/Emails/{id}` - Consultar status do e-mail (requer autenticação)

## 🧪 Testes

```bash
cd backend
dotnet test
```

## 🐳 Docker

### Subir todos os serviços
```bash
docker compose up -d
```

### Subir apenas infraestrutura
```bash
docker compose up rabbitmq smtp-service otel-collector -d
```

## 🔐 Autenticação

A API usa JWT para autenticação. Para obter um token:

```bash
POST http://localhost:5041/api/Auth/token
Content-Type: application/json

{
  "userName": "usuario",
  "email": "usuario@example.com",
  "role": "admin"
}
```

Use o token retornado no header:
```
Authorization: Bearer <token>
```

## 📊 Observabilidade

- **Grafana**: http://localhost:3000
- **Mailhog UI**: http://localhost:8025
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

## 🤝 Contribuindo

1. Faça fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.

## 👥 Autores

- Desenvolvedor - [GitHub](https://github.com/anfsusax)

## 🙏 Agradecimentos

- Comunidade .NET
- Comunidade Angular
- Todos os mantenedores das bibliotecas open-source utilizadas

