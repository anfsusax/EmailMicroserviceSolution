@echo off
setlocal

REM Navega para a raiz da solução (pasta pai de \scripts)
cd /d "%~dp0.."

echo [infra-up] Subindo serviços de infraestrutura (RabbitMQ, SMTP, OTEL, Grafana, Elastic)...
docker compose up rabbitmq smtp-service otel-collector grafana elasticsearch -d

if %errorlevel% neq 0 (
    echo [infra-up] Falha ao executar docker compose. Verifique se o Docker Desktop está em execução.
)

exit /b %errorlevel%

