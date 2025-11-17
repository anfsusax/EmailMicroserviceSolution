@echo off
setlocal

REM Navega para a raiz da solução
cd /d "%~dp0.."

echo [commit-angular] Adicionando arquivos do frontend Angular e alterações do backend...

git add frontend/email-front/
git add src/Email.Api/Program.cs
git add frontend/email-front/COMO-RODAR.md

echo [commit-angular] Fazendo commit...
git commit -m "feat: Adiciona frontend Angular com autenticação e lista de e-mails

- Configura CORS na API para permitir requisições do Angular
- Desabilita redirecionamento HTTPS em desenvolvimento
- Cria estrutura base do frontend Angular
- Implementa AuthService para geração e armazenamento de tokens
- Cria componente de login com formulário reativo
- Cria componente de lista de e-mails (placeholder)
- Configura rotas da aplicação
- Adiciona configuração de ambiente para URL da API"

if %errorlevel% neq 0 (
    echo [commit-angular] Erro ao fazer commit. Verifique se há alterações para commitar.
    exit /b %errorlevel%
)

echo [commit-angular] Fazendo push para o repositório remoto...
git push origin main

if %errorlevel% neq 0 (
    echo [commit-angular] Erro ao fazer push. Verifique sua conexão e permissões.
    exit /b %errorlevel%
)

echo [commit-angular] Commit e push concluídos com sucesso!
exit /b 0

