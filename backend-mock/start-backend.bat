@echo off
REM Script Adaptativo para Iniciar Backend Mock no Windows
REM Detecta automaticamente qual comando Python usar

setlocal enabledelayedexpansion

echo 🚀 Iniciando Backend Mock do Acervo Educacional...
echo.

set "PYTHON_CMD="

REM Detectar comando Python
python3 --version >nul 2>&1
if %errorlevel% == 0 (
    set "PYTHON_CMD=python3"
    echo 🐍 Usando comando: python3
) else (
    python --version >nul 2>&1
    if !errorlevel! == 0 (
        REM Verificar se é Python 3
        for /f "tokens=*" %%i in ('python --version 2^>^&1') do set "version=%%i"
        echo !version! | findstr "Python 3" >nul
        if !errorlevel! == 0 (
            set "PYTHON_CMD=python"
            echo 🐍 Usando comando: python
        )
    ) else (
        py --version >nul 2>&1
        if !errorlevel! == 0 (
            set "PYTHON_CMD=py -3"
            echo 🐍 Usando comando: py -3
        )
    )
)

if "%PYTHON_CMD%"=="" (
    echo ❌ ERRO: Python 3 não encontrado!
    echo.
    echo 📥 Por favor, instale Python 3.8+ ou execute:
    echo    scripts\check-python.bat
    echo.
    pause
    exit /b 1
)

REM Verificar se requirements.txt existe
if not exist "requirements.txt" (
    echo ❌ ERRO: requirements.txt não encontrado!
    echo 📍 Execute este script do diretório backend-mock\
    pause
    exit /b 1
)

REM Instalar dependências
echo 📦 Instalando dependências...
pip3 install -r requirements.txt >nul 2>&1
if %errorlevel% neq 0 (
    pip install -r requirements.txt >nul 2>&1
    if !errorlevel! neq 0 (
        echo ⚠️  pip não encontrado, tentando continuar...
    )
)

echo.

REM Verificar se server.py existe
if not exist "server.py" (
    echo ❌ ERRO: server.py não encontrado!
    echo 📍 Execute este script do diretório backend-mock\
    pause
    exit /b 1
)

REM Iniciar servidor
echo 🎯 Iniciando servidor na porta 5005...
echo 📍 Swagger UI: http://localhost:5005/swagger
echo 🔐 Credenciais: admin@acervoeducacional.com / Admin@123
echo.
echo ⚠️  Para parar o servidor, pressione Ctrl+C
echo.

REM Executar servidor
%PYTHON_CMD% server.py

