@echo off
setlocal

REM Navega para a raiz da solução
cd /d "%~dp0.."

echo [commit-organizacao] Adicionando todas as alterações de organização...

REM Adicionar backend (incluindo testes movidos)
git add backend/

REM Adicionar frontend
git add frontend/

REM Adicionar scripts
git add scripts/

REM Adicionar documentação
git add docs/
git add README.md

REM Adicionar docker-compose e GitHub Actions
git add docker-compose.yml
git add .github/

REM Remover pasta tests/ antiga se existir (será removida do Git)
if exist tests\ (
    echo [commit-organizacao] Removendo pasta tests/ antiga do Git...
    git rm -r tests\ 2>nul
)

echo [commit-organizacao] Fazendo commit...
git commit -m "refactor: Reorganiza estrutura do projeto

- Move código backend para pasta backend/
- Move testes para backend/tests/
- Remove .sln duplicado da raiz
- Consolida documentação em docs/
- Cria README.md principal na raiz
- Atualiza Docker Compose e GitHub Actions com novos caminhos
- Corrige caminhos em Dockerfiles e scripts
- Organiza estrutura para melhor manutenibilidade

Estrutura final:
- backend/ (código .NET + testes)
- frontend/ (Angular)
- docs/ (documentação consolidada)
- scripts/ (utilitários)"

if %errorlevel% neq 0 (
    echo [commit-organizacao] Erro ao fazer commit. Verifique se há alterações para commitar.
    exit /b %errorlevel%
)

echo [commit-organizacao] Fazendo push para o repositório remoto...
git push origin main

if %errorlevel% neq 0 (
    echo [commit-organizacao] Erro ao fazer push. Verifique sua conexão e permissões.
    exit /b %errorlevel%
)

echo [commit-organizacao] Commit e push concluídos com sucesso!
exit /b 0

