#!/bin/bash

# Script de Verificação de Ambiente Python
# Detecta qual comando Python está disponível e fornece instruções

echo "🐍 === VERIFICAÇÃO DE AMBIENTE PYTHON ==="
echo ""

# Variáveis para controle
PYTHON_CMD=""
PYTHON_VERSION=""
SYSTEM_OS=""

# Detectar sistema operacional
if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]] || [[ "$OS" == "Windows_NT" ]]; then
    SYSTEM_OS="Windows"
elif [[ "$OSTYPE" == "darwin"* ]]; then
    SYSTEM_OS="macOS"
else
    SYSTEM_OS="Linux"
fi

echo "🖥️  Sistema Operacional: $SYSTEM_OS"
echo ""

# Função para testar comando Python
test_python_command() {
    local cmd=$1
    if command -v "$cmd" &> /dev/null; then
        local version=$($cmd --version 2>&1)
        echo "✅ $cmd: $version"
        if [[ -z "$PYTHON_CMD" ]]; then
            PYTHON_CMD=$cmd
            PYTHON_VERSION=$version
        fi
        return 0
    else
        echo "❌ $cmd: não encontrado"
        return 1
    fi
}

# Testar diferentes comandos Python
echo "🔍 Testando comandos Python disponíveis:"
test_python_command "python3"
test_python_command "python"
test_python_command "py"

echo ""

# Verificar se algum Python foi encontrado
if [[ -z "$PYTHON_CMD" ]]; then
    echo "🚨 ERRO: Nenhuma versão do Python encontrada!"
    echo ""
    echo "📥 SOLUÇÕES POR SISTEMA:"
    echo ""
    
    if [[ "$SYSTEM_OS" == "Windows" ]]; then
        echo "🪟 Windows:"
        echo "1. Baixar Python do site oficial: https://python.org/downloads"
        echo "2. Durante instalação, marcar 'Add Python to PATH'"
        echo "3. Ou instalar via Microsoft Store"
        echo "4. Reiniciar terminal após instalação"
    
    elif [[ "$SYSTEM_OS" == "macOS" ]]; then
        echo "🍎 macOS:"
        echo "1. Instalar Homebrew: /bin/bash -c \"\$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\""
        echo "2. Instalar Python: brew install python3"
        echo "3. Ou baixar do site oficial: https://python.org/downloads"
    
    else
        echo "🐧 Linux:"
        echo "Ubuntu/Debian: sudo apt update && sudo apt install python3 python3-pip"
        echo "CentOS/RHEL: sudo yum install python3 python3-pip"
        echo "Arch Linux: sudo pacman -S python python-pip"
        echo "Fedora: sudo dnf install python3 python3-pip"
    fi
    
    echo ""
    echo "❌ Não é possível continuar sem Python instalado."
    exit 1
fi

# Python encontrado - mostrar informações
echo "🎉 Python encontrado!"
echo "📍 Comando recomendado: $PYTHON_CMD"
echo "📊 Versão: $PYTHON_VERSION"
echo ""

# Verificar se é Python 3
if [[ "$PYTHON_VERSION" == *"Python 2"* ]]; then
    echo "⚠️  AVISO: Python 2 detectado!"
    echo "🚨 Este projeto requer Python 3.8 ou superior."
    echo ""
    
    if [[ "$SYSTEM_OS" == "Windows" ]]; then
        echo "💡 Tente usar: py -3 server.py"
    else
        echo "💡 Instale Python 3 e tente novamente."
    fi
    echo ""
fi

# Testar pip
echo "🔍 Verificando pip:"
PIP_CMD=""

if command -v pip3 &> /dev/null; then
    echo "✅ pip3: $(pip3 --version)"
    PIP_CMD="pip3"
elif command -v pip &> /dev/null; then
    echo "✅ pip: $(pip --version)"
    PIP_CMD="pip"
else
    echo "❌ pip: não encontrado"
    echo "⚠️  pip é necessário para instalar dependências Python"
fi

echo ""

# Gerar instruções específicas
echo "📋 === INSTRUÇÕES PARA ESTE SISTEMA ==="
echo ""

echo "🚀 Para executar o backend:"
if [[ "$PYTHON_CMD" == "python3" ]]; then
    echo "   cd backend-mock"
    echo "   $PIP_CMD install -r requirements.txt"
    echo "   $PYTHON_CMD server.py"
elif [[ "$PYTHON_CMD" == "python" ]]; then
    echo "   cd backend-mock"
    echo "   $PIP_CMD install -r requirements.txt"
    echo "   $PYTHON_CMD server.py"
elif [[ "$PYTHON_CMD" == "py" ]]; then
    echo "   cd backend-mock"
    echo "   $PIP_CMD install -r requirements.txt"
    echo "   $PYTHON_CMD server.py"
fi

echo ""

# Verificar se python3 não existe mas python sim
if ! command -v python3 &> /dev/null && command -v python &> /dev/null; then
    echo "💡 === SOLUÇÃO PARA python3 NÃO ENCONTRADO ==="
    echo ""
    
    if [[ "$SYSTEM_OS" == "Windows" ]]; then
        echo "🪟 Windows - Opções:"
        echo "1. Criar cópia do executável:"
        echo "   where python"
        echo "   copy \"[caminho_do_python]\\python.exe\" \"[caminho_do_python]\\python3.exe\""
        echo ""
        echo "2. Usar py launcher:"
        echo "   py -3 server.py"
        echo ""
        echo "3. Criar alias no PowerShell:"
        echo "   Set-Alias python3 python"
    
    elif [[ "$SYSTEM_OS" == "macOS" ]]; then
        echo "🍎 macOS - Criar alias:"
        echo "   echo 'alias python3=\"python\"' >> ~/.zshrc"
        echo "   source ~/.zshrc"
        echo ""
        echo "Ou instalar Python 3 via Homebrew:"
        echo "   brew install python3"
    
    else
        echo "🐧 Linux - Criar symlink:"
        echo "   sudo ln -sf \$(which python) /usr/local/bin/python3"
        echo ""
        echo "Ou instalar python3:"
        echo "   sudo apt install python3  # Ubuntu/Debian"
        echo "   sudo yum install python3  # CentOS/RHEL"
    fi
    
    echo ""
fi

echo "✅ Verificação concluída!"
echo "📖 Para mais ajuda, consulte SETUP-LOCAL-CORRIGIDO.md"

