## Livro do Desenvolvedor Desesperado

> “Tal como Davi reuniu os valentes, cada microserviço aqui tem uma função clara na batalha.” (1 Samuel 22)

### Capítulo 1 – Cartografia do Reino (Ecossistema Atual)
- **Solução**: `EmailMicroserviceSolution.sln` orquestra seis projetos (`Email.Api`, `Email.Application`, `Email.Domain`, `Email.Infrastructure`, `Email.Worker`, `Email.ApiGateway`).
- **Fluxo principal**:
  1. `POST /api/auth/token` (controller `src/Email.Api/Controllers/AuthController.cs`) gera JWT.
  2. Cliente chama `POST /api/emails` com o token; `EmailsController` publica comando via `IEmailCommandHandler`.
  3. `Email.Infrastructure` empacota mensagem e publica no RabbitMQ (`RabbitMqEmailQueue`).
  4. `Email.Worker` (`EmailQueueWorker`) consome fila, envia pelo `SmtpEmailSender`, atualiza status (`InMemoryEmailStatusStore`).
  5. `GET /api/emails/{id}` devolve `EmailStatusResponse`.
- **Infra de suporte** (`docker-compose.yml`):
  - RabbitMQ (5672/15672) para filas.
  - Mailhog (8025) simula SMTP.
  - OpenTelemetry Collector + Grafana + ElasticSearch (4317/3000/9200) para observabilidade.
  - `email-api`, `email-worker`, `email-gateway` sob a mesma stack.
- **Arquitetura**:
  - `Email.Domain`: entidades (`EmailMessage`, `Attachment`, `Recipient`) e enum `EmailStatus`.
  - `Email.Application`: CQRS/Use Cases (`SendEmailCommand`, `EmailProcessingService`).
  - `Email.Infrastructure`: integrações e implementações concretas (RabbitMQ, SMTP, métricas, templates).
  - `Email.Api`: controllers, DI, JWT, Swagger.
  - `Email.Worker`: background service consumindo fila.
  - `Email.ApiGateway`: Ocelot roteando `POST /emails` e `GET /emails/{id}` (porta 9000).

Checklist rápido:
- `dotnet test` (qualidade).
- `dotnet run --project ...` (execução local).
- `docker compose up --build` (stack completa com observabilidade).

### Capítulo 2 – Manual de Campo (Guia Estruturado)
1. **Visão Geral** – contextualiza a arquitetura com a parábola de Neemias (já no `README.md`).
2. **Preparação da Missão** – instruções de setup (SDK .NET 8, Docker Desktop, variáveis JWT).
3. **Fluxo de Envio** – narrativa “pedido chega ao portão, é revisado e enviado”; destacar pontos de extensão (templates, schedule).
4. **Painel do Vigia** – como acessar RabbitMQ UI, Mailhog, Grafana, Elastic para troubleshooting.
5. **Perguntas Frequentes** – ponte com o capítulo 3 abaixo.
6. **Checklists** – antes de deploy, antes de incident response, antes de testes de carga.

### Capítulo 3 – Perguntas & Respostas por Guarda (FAQ)
- **Autenticação**
  - *Como gero token?* `POST /api/auth/token` com `GenerateTokenRequest`. Ver `AuthController`.
  - *Onde configuro segredo JWT?* `src/Email.Api/appsettings.*` (`JwtOptions`).
- **Envio de e-mails**
  - *Campos obrigatórios?* `SendEmailRequest` (to, subject, body ou template). Exemplo no `README`.
  - *Como acompanho status?* `GET /api/emails/{id}`.
  - *Templates?* `InMemoryTemplateRenderer` (trocar por provider real).
- **Fila e Worker**
  - *Onde configuro RabbitMQ?* `Email.Infrastructure.Configuration.RabbitMqOptions`.
  - *Retentativas?* `EmailProcessingOptions.MaxRetryAttempts` + `EmailProcessingService`.
  - *Como simulo falha?* Interromper `smtp-service` ou alterar credenciais.
- **Observabilidade**
  - *Logs estruturados?* Serilog já envia para console; configurar sink adicional.
  - *Traces e métricas?* `OpenTelemetry` em `Program.cs`; exporter OTLP → Grafana/Elastic.
  - *Mailhog captura onde?* http://localhost:8025.

### Capítulo 4 – Projeto Angular Educativo
Objetivo: aprender fundamentos Angular consumindo esta API.

- **Módulos/Páginas**
  - `AuthModule` (login → JWT).
  - `EmailComposerModule` (form de envio com attachments e template data).
  - `StatusModule` (lista e detalhamento `GET /api/emails/{id}` com polling).
  - `DashboardModule` (cards com métricas básicas chamando `/health` ou endpoints adicionais futuros).
- **Serviços Angular**
  - `AuthService` → `/api/auth/token`.
  - `EmailService` → `POST /api/emails`, `GET /api/emails/{id}`.
  - `GatewayService` (opcional) → consumir via `http://localhost:9000`.
  - Interceptor JWT + guard para rotas protegidas.
- **Fundamentos abordados**
  - Reactive Forms (validação de envio).
  - HttpClient + interceptors.
  - Routing + lazy loading.
  - State management simples (BehaviorSubject para status recentes).
- **Roadmap incremental**
  1. Configurar workspace Angular + `proxy.conf.json` apontando para API.
  2. Implementar login e armazenamento seguro do token.
  3. Criar formulário de envio com preview.
  4. Adicionar página de histórico (usar IDs retornados e persistir no localStorage).
  5. Introduzir dashboard com gráficos (ex.: Chart.js) usando métricas futuras.

### Capítulo 5 – Estratégia de Evolução (Discipulado Contínuo)
1. **Leitura guiada**: estudar Capítulos 1-3 enquanto executa `dotnet run` e testa via Swagger/Postman.
2. **Missões práticas**:
   - *Missão 1*: gerar token, enviar e-mail e confirmar no Mailhog.
   - *Missão 2*: desligar SMTP e observar retentativas/logs.
   - *Missão 3*: subir stack docker e validar dashboards.
3. **Construção Angular**: seguir roadmap do Capítulo 4, criando commits pequenos e documentados.
4. **Checkpoints de aprendizado**:
   - Consegue explicar o fluxo end-to-end sem consultar o código?
   - Consegue depurar uma falha de autenticação?
   - Consegue adicionar um novo campo ao `SendEmailRequest` e propagar até o front?
5. **Ganchos futuros**:
   - Persistência durável (Postgres/Mongo) substituindo `InMemoryEmailStatusStore`.
   - Novos canais (SMS/WhatsApp) reaproveitando domínio existente.
   - Testes de contrato para o Gateway.
   - Automação CI/CD + alertas (Grafana Alerting).

> “Quando a obra parece grande demais, lembre-se de Neemias 4: cada família cuidou de um trecho do muro. Pegue um capítulo por vez.”


