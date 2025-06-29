# 🔧 Setup Local Corrigido - Acervo Educacional

> **IMPORTANTE:** Este guia foi criado após identificar e corrigir inconsistências entre o ambiente sandbox e local.

## ⚠️ **PROBLEMAS IDENTIFICADOS E CORRIGIDOS:**

### **1. Inconsistência de Portas** ✅ CORRIGIDO
- **Problema:** Backend configurado para porta 5006, mas rodando na 5005
- **Solução:** Padronizado tudo para porta **5005**
- **Arquivos corrigidos:**
  - `frontend/acervo-educacional-frontend/.env`
  - `frontend/acervo-educacional-frontend/.env.example`
  - `backend-mock/server.py`
  - `frontend/acervo-educacional-frontend/src/setupTests.js`

### **2. Dependências Python Não Documentadas** ✅ CORRIGIDO
- **Problema:** Dependências Python não listadas
- **Solução:** Criado `backend-mock/requirements.txt`
- **Dependências:** flask, flask-cors, pyjwt

### **3. Arquivo .env Não Commitado** ✅ CORRIGIDO
- **Problema:** .env criado no sandbox mas não no repositório
- **Solução:** .env.example atualizado com porta correta

### **4. Problema PATH do Python** ✅ CORRIGIDO
- **Problema:** `python` funciona mas `python3` não está no PATH
- **Solução:** Scripts adaptativos e guia de correção
- **Arquivos criados:**
  - `scripts/check-python.sh` (Linux/macOS)
  - `scripts/check-python.bat` (Windows)
  - `backend-mock/start-backend.sh` (script adaptativo)
  - `backend-mock/start-backend.bat` (script adaptativo Windows)

## 🚀 **INSTRUÇÕES CORRIGIDAS PARA SETUP LOCAL:**

### **Pré-requisitos Obrigatórios:**

**🔍 PRIMEIRO: Verificar Python**
```bash
# Linux/macOS
./scripts/check-python.sh

# Windows
scripts\check-python.bat
```

**📋 Versões mínimas:**
```bash
# Verificar versões mínimas
node --version    # Recomendado: v18+
npm --version     # Recomendado: v9+
python --version  # OU python3 --version - Recomendado: v3.8+
git --version     # Qualquer versão recente
```

### **Passo 1: Clonar e Configurar**
```bash
# Clonar repositório
git clone https://github.com/CarlosBertoldo/acervo-educacional.git
cd acervo-educacional

# Verificar se está na branch main
git branch
```

### **Passo 2: Configurar Backend Mock**

**🚀 MÉTODO FÁCIL (Recomendado):**
```bash
# Linux/macOS
cd backend-mock
./start-backend.sh

# Windows
cd backend-mock
start-backend.bat
```

**🔧 MÉTODO MANUAL:**
```bash
# Ir para diretório do backend mock
cd backend-mock

# Instalar dependências Python
pip install -r requirements.txt
# OU se tiver pip3:
pip3 install -r requirements.txt

# Executar servidor (usar comando que funciona no seu sistema)
python server.py
# OU
python3 server.py
# OU (Windows)
py server.py
```

### **Passo 3: Configurar Frontend**
```bash
# Ir para diretório do frontend
cd ../frontend/acervo-educacional-frontend

# Criar arquivo .env (OBRIGATÓRIO)
cp .env.example .env

# Verificar conteúdo do .env
cat .env
# Deve mostrar: VITE_API_URL=http://localhost:5005/api

# Instalar dependências
npm install

# Verificar se dependências críticas foram instaladas
npm list dompurify jest typescript
```

### **Passo 4: Executar o Projeto**

**Terminal 1 - Backend:**
```bash
cd backend-mock
python3 server.py

# Deve mostrar:
# * Running on all addresses (0.0.0.0)
# * Running on http://127.0.0.1:5005
# * Running on http://[::1]:5005
# 📍 Swagger UI: http://localhost:5005/swagger
```

**Terminal 2 - Frontend:**
```bash
cd frontend/acervo-educacional-frontend
npm run dev

# Deve mostrar:
# Local:   http://localhost:5175/
# Network: use --host to expose
```

### **Passo 5: Verificar Funcionamento**

**1. Testar Backend:**
```bash
# Em um terceiro terminal
curl http://localhost:5005/api/health

# Deve retornar JSON com status
```

**2. Testar Frontend:**
- Abrir: http://localhost:5175
- Login: admin@acervoeducacional.com
- Senha: Admin@123

**3. Testar Swagger:**
- Abrir: http://localhost:5005/swagger
- Verificar documentação da API

## 🔍 **TROUBLESHOOTING ESPECÍFICO:**

### **Erro: "python3: command not found" (MAS python funciona)**

**🐍 PROBLEMA COMUM:** `python` está no PATH mas `python3` não.

**🪟 Windows - Soluções:**
```cmd
# Opção 1: Criar cópia do executável
where python
copy "C:\Users\%USERNAME%\AppData\Local\Programs\Python\Python311\python.exe" "C:\Users\%USERNAME%\AppData\Local\Programs\Python\Python311\python3.exe"

# Opção 2: Usar py launcher
py -3 server.py

# Opção 3: Usar python diretamente
python server.py
```

**🍎 macOS - Soluções:**
```bash
# Opção 1: Criar alias permanente
echo 'alias python3="python"' >> ~/.zshrc
source ~/.zshrc

# Opção 2: Instalar Python 3 via Homebrew
brew install python3

# Opção 3: Criar symlink
sudo ln -sf $(which python) /usr/local/bin/python3
```

**🐧 Linux - Soluções:**
```bash
# Opção 1: Instalar python3
sudo apt install python3 python3-pip  # Ubuntu/Debian
sudo yum install python3 python3-pip  # CentOS/RHEL

# Opção 2: Criar symlink
sudo ln -sf $(which python) /usr/local/bin/python3

# Opção 3: Usar python diretamente
python server.py
```

### **Erro: "EADDRINUSE: address already in use :::5005"**
```bash
# Verificar o que está usando a porta
lsof -i :5005

# Matar processo se necessário
kill -9 <PID>

# Ou usar porta alternativa
# Editar backend-mock/server.py linha 690: port=5006
# Editar frontend/.env: VITE_API_URL=http://localhost:5006/api
```

### **Erro: "Module not found: Can't resolve 'dompurify'"**
```bash
cd frontend/acervo-educacional-frontend
npm install dompurify @types/dompurify --save
```

### **Erro: "python3: command not found"**
```bash
# Windows
python server.py

# Mac com Homebrew
brew install python3

# Ubuntu/Debian
sudo apt update && sudo apt install python3 python3-pip
```

### **Erro: "npm: command not found"**
```bash
# Instalar Node.js do site oficial: https://nodejs.org
# Ou usar gerenciador de versão como nvm
```

### **Frontend não conecta com Backend**
```bash
# 1. Verificar se backend está rodando
curl http://localhost:5005/api/health

# 2. Verificar arquivo .env
cat frontend/acervo-educacional-frontend/.env

# 3. Verificar console do navegador (F12)
# Procurar por erros de CORS ou conexão

# 4. Verificar se porta está correta
netstat -tlnp | grep 5005
```

## ✅ **VALIDAÇÃO FINAL:**

Execute estes comandos para validar que tudo está funcionando:

```bash
# 1. Backend respondendo
curl -s http://localhost:5005/api/health && echo " ✅ Backend OK"

# 2. Frontend carregando
curl -s http://localhost:5175 | grep -q "Acervo" && echo " ✅ Frontend OK"

# 3. Dependências instaladas
cd frontend/acervo-educacional-frontend
npm list dompurify jest typescript | grep -E "(dompurify|jest|typescript)" && echo " ✅ Dependências OK"

# 4. Testes funcionando
npm test -- --passWithNoTests && echo " ✅ Testes OK"
```

## 📋 **CHECKLIST DE VERIFICAÇÃO:**

- [ ] Node.js v18+ instalado
- [ ] Python 3.8+ instalado
- [ ] Repositório clonado
- [ ] Backend dependencies instaladas (`pip3 install -r requirements.txt`)
- [ ] Frontend dependencies instaladas (`npm install`)
- [ ] Arquivo `.env` criado com porta 5005
- [ ] Backend rodando na porta 5005
- [ ] Frontend rodando na porta 5175
- [ ] Login funcionando
- [ ] Dashboard carregando
- [ ] Swagger acessível

## 🆘 **Se Ainda Não Funcionar:**

1. **Limpar tudo e recomeçar:**
```bash
# Remover node_modules
rm -rf frontend/acervo-educacional-frontend/node_modules

# Limpar cache npm
npm cache clean --force

# Reinstalar
cd frontend/acervo-educacional-frontend
npm install
```

2. **Verificar versões:**
```bash
node --version  # Deve ser v18+
npm --version   # Deve ser v9+
python3 --version  # Deve ser v3.8+
```

3. **Verificar firewall/antivírus:**
- Temporariamente desabilitar para testar
- Adicionar exceções para portas 5005 e 5175

4. **Usar portas alternativas:**
- Editar todas as configurações para usar 5006/5176
- Garantir consistência em todos os arquivos

**Se mesmo assim não funcionar, documente exatamente:**
- Sistema operacional e versão
- Versões do Node.js, npm, Python
- Mensagens de erro completas
- Logs do terminal

**Este guia foi testado e corrigido para funcionar em ambiente local!** 🎯

