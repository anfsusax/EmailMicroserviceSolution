# Plano: Autenticação Real e Health Checks Robustos

## 🎯 Objetivos

### 1. Autenticação Real
- ✅ Cadastro de usuários com validação
- ✅ Login com validação de credenciais
- ✅ Hash de senhas (BCrypt)
- ✅ Gerenciamento de usuários
- ✅ Roles e permissões

### 2. Health Checks Robustos
- ✅ Liveness probe (serviço está vivo?)
- ✅ Readiness probe (serviço está pronto?)
- ✅ Verificação de dependências (RabbitMQ, SMTP, etc.)
- ✅ Métricas detalhadas
- ✅ Status codes apropriados para orquestradores

## 📋 Implementação

### Fase 1: Autenticação Real

#### 1.1 Entidade User (Domain)
- `User.cs` com propriedades básicas
- Validações de domínio

#### 1.2 Repositório de Usuários (Infrastructure)
- Interface `IUserRepository`
- Implementação com armazenamento em memória (depois pode migrar para DB)

#### 1.3 Serviço de Autenticação (Application)
- `IAuthService` com métodos:
  - `RegisterAsync(RegisterRequest)`
  - `LoginAsync(LoginRequest)`
  - `ValidateCredentialsAsync(username, password)`

#### 1.4 Hash de Senhas
- Usar `BCrypt.Net-Next` para hash de senhas
- Nunca armazenar senhas em texto plano

#### 1.5 Endpoints
- `POST /api/Auth/register` - Cadastro
- `POST /api/Auth/login` - Login (substitui o token atual)
- `GET /api/Auth/me` - Dados do usuário logado

### Fase 2: Health Checks Robustos

#### 2.1 Health Checks Customizados
- `RabbitMqHealthCheck` - Verifica conexão com RabbitMQ
- `SmtpHealthCheck` - Verifica conexão com SMTP
- `MemoryHealthCheck` - Verifica uso de memória

#### 2.2 Endpoints de Health
- `GET /health` - Health check básico (liveness)
- `GET /health/ready` - Readiness check (dependências)
- `GET /health/live` - Liveness check (serviço vivo)

#### 2.3 Respostas Estruturadas
```json
{
  "status": "Healthy|Degraded|Unhealthy",
  "checks": {
    "rabbitmq": { "status": "Healthy", "responseTime": "5ms" },
    "smtp": { "status": "Healthy", "responseTime": "2ms" }
  },
  "timestamp": "2025-11-17T19:00:00Z"
}
```

## 🔧 Tecnologias

- **BCrypt.Net-Next**: Hash de senhas
- **ASP.NET Core Health Checks**: Health checks nativos
- **Microsoft.Extensions.Diagnostics.HealthChecks**: Diagnósticos

## 📝 Próximos Passos

1. Criar entidade User
2. Implementar repositório
3. Adicionar BCrypt
4. Criar serviço de autenticação
5. Atualizar endpoints
6. Implementar health checks customizados
7. Configurar endpoints de health

