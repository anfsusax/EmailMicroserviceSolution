## EmailMicroserviceSolution

Este repositório traz um ecossistema de microserviços .NET 8 preparado para orquestrar o envio confiável de boletos e outras mensagens digitais. A arquitetura segue DDD, Clean Architecture e boas práticas de mensageria assíncrona, entregando API, Worker, Gateway, infraestrutura de observabilidade e orquestração Docker.

### Visão Geral (parábola inspirada em Neemias 4)

Assim como Neemias reconstruiu os muros de Jerusalém com equipes em turnos (Neemias 4), dividimos o sistema em frentes especializadas que trabalham em paralelo mas compartilham a mesma visão:

- **Email.Api**: o “portão principal” que valida e autentica cada pedido (JWT/OAuth2), expõe `POST /api/emails` e acompanha o status.
- **Email.Application / Email.Domain**: o “plano mestre” com entidades, regras e casos de uso (fila, retries, métricas).
- **Email.Infrastructure**: os “pedreiros” que falam com RabbitMQ, SMTP, telemetria e armazenamento de status.
- **Email.Worker**: o “vigia noturno” que consome a fila e garante o envio com política de retentativa.
- **Email.ApiGateway**: o “muro externo” baseado em Ocelot para centralizar chamadas e aplicar políticas de borda.
- **Observabilidade & Operações**: OpenTelemetry, ElasticSearch, Grafana, RabbitMQ e Mailhog sob orquestração Docker.

### Estrutura

- `src/Email.Domain`: entidades (`EmailMessage`, `Attachment`, `Recipient`), value objects e enum `EmailStatus`.
- `src/Email.Application`: contratos, validações com FluentValidation, `SendEmailCommandHandler`, `EmailProcessingService`, opções de processamento.
- `src/Email.Infrastructure`: integrações (RabbitMQ, SMTP via MailKit), métricas (`Meter`), armazenamento em memória e DI.
- `src/Email.Api`: ASP.NET Core com controllers, autenticação JWT, Swagger, health checks e OpenTelemetry.
- `src/Email.Worker`: serviço hospedado que utiliza `EmailProcessingService` e expõe telemetria.
- `src/Email.ApiGateway`: gateway Ocelot com Dockerfile próprio.
- `docker-compose.yml` + `deploy/otel-collector-config.yaml`: infraestrutura observável (RabbitMQ, Mailhog, Elastic, Grafana, OTel Collector).

### Endpoints principais

- `POST /api/Auth/token`: gera JWT temporário (use em Swagger/Postman).
- `POST /api/emails`: enfileira envios com templates, anexos (base64 ou link seguro), `cc`, `bcc`, `metadata`, `scheduleAt`.
- `GET /api/emails/{id}`: consulta status (`Queued`, `Processing`, `Sent`, `Failed`, `DeadLettered`).
- `GET /health`: liveness/readiness.
- `Gateway`: expõe `/emails` e `/emails/{id}` via porta `9000`.

Exemplo de carga pronta para produção:

```json
{
  "subject": "Boleto Novembro/2025",
  "body": "<p>Fallback caso o template não seja encontrado.</p>",
  "isHtml": true,
  "to": [{ "name": "Carla Souza", "address": "carla@empresa.com" }],
  "cc": [{ "name": "Financeiro", "address": "fin@empresa.com" }],
  "bcc": [{ "name": "Auditoria", "address": "audit@empresa.com" }],
  "attachments": [
    {
      "fileName": "boleto.pdf",
      "contentType": "application/pdf",
      "base64Content": "JVBERi0xLjQKJcTl8uXr..."
    },
    {
      "fileName": "relatorio.csv",
      "contentType": "text/csv",
      "externalUrl": "https://storage.empresa.com/relatorios/123.csv"
    }
  ],
  "scheduleAt": "2025-11-14T21:28:04.834Z",
  "templateId": "boleto-padrao",
  "templateData": {
    "nome_cliente": "Carla Souza",
    "vencimento": "14/11/2025",
    "valor": "R$ 850,00"
  },
  "metadata": {
    "prioridade": "alta",
    "categoria": "boletos"
  }
}
```

### Como executar

1. **Executar testes**
   ```bash
   dotnet test
   ```
2. **Executar localmente**
   ```bash
   dotnet run --project src/Email.Api/Email.Api.csproj
   dotnet run --project src/Email.Worker/Email.Worker.csproj
   dotnet run --project src/Email.ApiGateway/Email.ApiGateway.csproj
   ```
3. **Subir stack completa**
   ```bash
   docker compose up --build
   ```
   - API em `http://localhost:8080`
   - Gateway em `http://localhost:9000`
   - RabbitMQ UI em `http://localhost:15672`
   - Mailhog em `http://localhost:8025`
   - Grafana em `http://localhost:3000`
   - ElasticSearch em `http://localhost:9200`

### Próximos passos sugeridos

- Adicionar persistência durável (PostgreSQL, Mongo ou Elastic) para histórico.
- Implementar integrações WhatsApp/SMS e suporte a anexos volumosos (ZIP, exames, laudos).
- Criar dashboard customizado no Grafana + alertas baseados em métricas expostas.
- Implementar circuit breaker (Polly) para SMTP e mensageria, reforçando resiliência.

### Estilo e boas práticas

- Código orientado a Clean Architecture (camadas desacopladas por DI).
- SOLID aplicado em abstrações e serviços especializados.
- Observabilidade nativa (OpenTelemetry + OTLP) pronta para ELK/Grafana.
- Dockerfiles individuais e compose para facilitar CI/CD.

Que este sistema seja como os muros de Jerusalém: cada tijolo foi colocado com propósito e vigilância, garantindo proteção para que as “mensagens” (boletos, exames, notificações) alcancem seus destinatários com fidelidade.

