# Implementação: Autenticação Real e Health Checks

## 🎯 Resumo

### Problema Atual
- ❌ Login aceita qualquer coisa (fictício)
- ❌ Não há cadastro de usuários
- ❌ Não há validação de credenciais
- ⚠️ Health checks básicos (precisam melhorar)

### Solução Proposta

#### 1. Autenticação Real
```
1. Criar entidade User (Domain)
2. Implementar repositório (Infrastructure) 
3. Adicionar BCrypt para hash de senhas
4. Criar serviço de autenticação (Application)
5. Atualizar endpoints de Auth
6. Adicionar endpoint de cadastro
```

#### 2. Health Checks Robustos
```
1. Health checks customizados (RabbitMQ, SMTP)
2. Endpoints separados (liveness/readiness)
3. Respostas estruturadas com detalhes
4. Status codes apropriados
```

## 📦 Dependências Necessárias

```xml
<PackageReference Include="BCrypt.Net-Next" Version="0.1.0" />
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions" Version="8.0.0" />
```

## 🚀 Ordem de Implementação

1. **Fase 1**: Entidade User e Repositório
2. **Fase 2**: BCrypt e Hash de Senhas
3. **Fase 3**: Serviço de Autenticação
4. **Fase 4**: Endpoints de Auth
5. **Fase 5**: Health Checks Customizados
6. **Fase 6**: Testes

## ✅ Critérios de Sucesso

- [ ] Usuário pode se cadastrar
- [ ] Login valida credenciais reais
- [ ] Senhas são hasheadas (nunca em texto plano)
- [ ] Health checks verificam dependências
- [ ] Orquestradores podem usar health checks
- [ ] Frontend funciona com nova autenticação

