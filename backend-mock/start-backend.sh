#!/bin/bash

# Script Adaptativo para Iniciar Backend Mock
# Detecta automaticamente qual comando Python usar

echo "🚀 Iniciando Backend Mock do Acervo Educacional..."
echo ""

# Função para detectar Python
detect_python() {
    if command -v python3 &> /dev/null; then
        echo "python3"
    elif command -v python &> /dev/null; then
        # Verificar se é Python 3
        local version=$(python --version 2>&1)
        if [[ "$version" == *"Python 3"* ]]; then
            echo "python"
        else
            echo ""
        fi
    elif command -v py &> /dev/null; then
        echo "py"
    else
        echo ""
    fi
}

# Detectar comando Python
PYTHON_CMD=$(detect_python)

if [[ -z "$PYTHON_CMD" ]]; then
    echo "❌ ERRO: Python 3 não encontrado!"
    echo ""
    echo "📥 Por favor, instale Python 3.8+ ou execute:"
    echo "   ./scripts/check-python.sh"
    echo ""
    exit 1
fi

echo "🐍 Usando comando: $PYTHON_CMD"

# Verificar se requirements.txt existe
if [[ ! -f "requirements.txt" ]]; then
    echo "❌ ERRO: requirements.txt não encontrado!"
    echo "📍 Execute este script do diretório backend-mock/"
    exit 1
fi

# Instalar dependências
echo "📦 Instalando dependências..."
if command -v pip3 &> /dev/null; then
    pip3 install -r requirements.txt
elif command -v pip &> /dev/null; then
    pip install -r requirements.txt
else
    echo "⚠️  pip não encontrado, tentando continuar..."
fi

echo ""

# Verificar se server.py existe
if [[ ! -f "server.py" ]]; then
    echo "❌ ERRO: server.py não encontrado!"
    echo "📍 Execute este script do diretório backend-mock/"
    exit 1
fi

# Iniciar servidor
echo "🎯 Iniciando servidor na porta 5005..."
echo "📍 Swagger UI: http://localhost:5005/swagger"
echo "🔐 Credenciais: admin@acervoeducacional.com / Admin@123"
echo ""
echo "⚠️  Para parar o servidor, pressione Ctrl+C"
echo ""

# Executar servidor
if [[ "$PYTHON_CMD" == "py" ]]; then
    py -3 server.py
else
    $PYTHON_CMD server.py
fi

