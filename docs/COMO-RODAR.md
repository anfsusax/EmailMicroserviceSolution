# Como Rodar o Projeto

## Pré-requisitos
- .NET 8 SDK instalado
- Node.js e npm instalados
- Docker Desktop rodando (para infraestrutura)

## Passo a Passo

### 1. Subir a Infraestrutura (Docker)
```bash
# Na raiz do projeto
scripts\infra-up.bat
```

Ou manualmente:
```bash
docker compose up rabbitmq smtp-service otel-collector -d
```

### 2. Rodar a API (.NET)
```bash
# No Visual Studio:
# 1. Abra backend/EmailMicroserviceSolution.sln
# 2. Configure Email.Api como projeto de inicialização
# 3. Pressione F5

# Ou via terminal:
cd backend/src/Email.Api
dotnet run
```

A API deve iniciar em: `http://localhost:5041`
Verifique no navegador: `http://localhost:5041/swagger`

### 3. Rodar o Worker (opcional, mas recomendado)
```bash
# No Visual Studio:
# Configure Email.Worker como segundo projeto de inicialização

# Ou via terminal:
cd backend/src/Email.Worker
dotnet run
```

### 4. Rodar o Frontend Angular
```bash
cd frontend/email-front
npm install  # Apenas na primeira vez
ng serve
```

O frontend deve iniciar em: `http://localhost:4200`

## Verificação Rápida

1. **API rodando?**
   - Acesse: http://localhost:5041/swagger
   - Deve abrir a documentação Swagger

2. **Frontend rodando?**
   - Acesse: http://localhost:4200
   - Deve abrir a tela de login

3. **Infraestrutura rodando?**
   ```bash
   docker compose ps
   ```
   - Deve mostrar RabbitMQ, Mailhog, etc. como "Up"

## Troubleshooting

### Erro: "Não foi possível conectar à API"
- Verifique se a API está rodando (passo 2)
- Verifique se a porta 5041 está livre
- Verifique o console do navegador (F12) para mais detalhes

### Erro de CORS
- Certifique-se de que o CORS está configurado no `Program.cs`
- Reinicie a API após configurar CORS

### Erro ao enviar e-mail
- Verifique se o Worker está rodando
- Verifique se RabbitMQ e Mailhog estão rodando
- Verifique os logs do Worker no console

