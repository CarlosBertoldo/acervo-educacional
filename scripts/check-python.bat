@echo off
REM Script de Verificação de Ambiente Python para Windows
REM Detecta qual comando Python está disponível e fornece instruções

echo 🐍 === VERIFICAÇÃO DE AMBIENTE PYTHON ===
echo.

set "PYTHON_CMD="
set "PYTHON_VERSION="
set "PIP_CMD="

echo 🖥️  Sistema Operacional: Windows
echo.

echo 🔍 Testando comandos Python disponíveis:

REM Testar python3
python3 --version >nul 2>&1
if %errorlevel% == 0 (
    for /f "tokens=*" %%i in ('python3 --version 2^>^&1') do set "PYTHON_VERSION=%%i"
    echo ✅ python3: !PYTHON_VERSION!
    set "PYTHON_CMD=python3"
) else (
    echo ❌ python3: não encontrado
)

REM Testar python
if "%PYTHON_CMD%"=="" (
    python --version >nul 2>&1
    if !errorlevel! == 0 (
        for /f "tokens=*" %%i in ('python --version 2^>^&1') do set "PYTHON_VERSION=%%i"
        echo ✅ python: !PYTHON_VERSION!
        set "PYTHON_CMD=python"
    ) else (
        echo ❌ python: não encontrado
    )
)

REM Testar py
if "%PYTHON_CMD%"=="" (
    py --version >nul 2>&1
    if !errorlevel! == 0 (
        for /f "tokens=*" %%i in ('py --version 2^>^&1') do set "PYTHON_VERSION=%%i"
        echo ✅ py: !PYTHON_VERSION!
        set "PYTHON_CMD=py"
    ) else (
        echo ❌ py: não encontrado
    )
)

echo.

REM Verificar se algum Python foi encontrado
if "%PYTHON_CMD%"=="" (
    echo 🚨 ERRO: Nenhuma versão do Python encontrada!
    echo.
    echo 📥 SOLUÇÕES PARA WINDOWS:
    echo.
    echo 1. Baixar Python do site oficial: https://python.org/downloads
    echo 2. Durante instalação, marcar 'Add Python to PATH'
    echo 3. Ou instalar via Microsoft Store
    echo 4. Reiniciar terminal após instalação
    echo.
    echo ❌ Não é possível continuar sem Python instalado.
    pause
    exit /b 1
)

echo 🎉 Python encontrado!
echo 📍 Comando recomendado: %PYTHON_CMD%
echo 📊 Versão: %PYTHON_VERSION%
echo.

REM Verificar se é Python 3
echo %PYTHON_VERSION% | findstr "Python 2" >nul
if %errorlevel% == 0 (
    echo ⚠️  AVISO: Python 2 detectado!
    echo 🚨 Este projeto requer Python 3.8 ou superior.
    echo.
    echo 💡 Tente usar: py -3 server.py
    echo.
)

REM Testar pip
echo 🔍 Verificando pip:

pip3 --version >nul 2>&1
if %errorlevel% == 0 (
    for /f "tokens=*" %%i in ('pip3 --version 2^>^&1') do echo ✅ pip3: %%i
    set "PIP_CMD=pip3"
) else (
    pip --version >nul 2>&1
    if !errorlevel! == 0 (
        for /f "tokens=*" %%i in ('pip --version 2^>^&1') do echo ✅ pip: %%i
        set "PIP_CMD=pip"
    ) else (
        echo ❌ pip: não encontrado
        echo ⚠️  pip é necessário para instalar dependências Python
    )
)

echo.

REM Gerar instruções específicas
echo 📋 === INSTRUÇÕES PARA ESTE SISTEMA ===
echo.

echo 🚀 Para executar o backend:
echo    cd backend-mock
if not "%PIP_CMD%"=="" (
    echo    %PIP_CMD% install -r requirements.txt
)
echo    %PYTHON_CMD% server.py
echo.

REM Verificar se python3 não existe mas python sim
python3 --version >nul 2>&1
if %errorlevel% neq 0 (
    python --version >nul 2>&1
    if !errorlevel! == 0 (
        echo 💡 === SOLUÇÃO PARA python3 NÃO ENCONTRADO ===
        echo.
        echo 🪟 Windows - Opções:
        echo 1. Criar cópia do executável:
        echo    where python
        echo    copy "[caminho_do_python]\python.exe" "[caminho_do_python]\python3.exe"
        echo.
        echo 2. Usar py launcher:
        echo    py -3 server.py
        echo.
        echo 3. Criar alias no PowerShell:
        echo    Set-Alias python3 python
        echo.
    )
)

echo ✅ Verificação concluída!
echo 📖 Para mais ajuda, consulte SETUP-LOCAL-CORRIGIDO.md
echo.
pause

