# 🔧 Solução de Problemas - Acervo Educacional

Guia para resolver problemas comuns durante instalação, configuração e uso do sistema.

---

## 🚨 **Problemas de Instalação**

### **❌ Erro: "node_modules não encontrado"**

**Sintomas:**
- Erro ao executar `npm run dev`
- Mensagem "Cannot find module"

**Solução:**
```bash
cd frontend/acervo-educacional-frontend
rm -rf node_modules package-lock.json
npm install
```

### **❌ Erro: "Python dependencies não instaladas"**

**Sintomas:**
- Erro ao executar `python3 server.py`
- Mensagem "ModuleNotFoundError"

**Solução:**
```bash
cd backend-mock
pip3 install --upgrade flask flask-cors pyjwt
```

### **❌ Erro: "Porta já em uso"**

**Sintomas:**
- Erro "EADDRINUSE" ou "Address already in use"

**Solução:**
```bash
# Verificar processos nas portas
lsof -i :5005  # Backend
lsof -i :5175  # Frontend

# Parar processos se necessário
pkill -f "python3.*server.py"
pkill -f "node.*vite"
```

---

## 🔐 **Problemas de Autenticação**

### **❌ Login não funciona**

**Sintomas:**
- Botão "Entrar" não responde
- Campos se limpam após clicar
- Erro no console do navegador

**Diagnóstico:**
1. Abrir DevTools (F12)
2. Verificar aba Console para erros JavaScript
3. Verificar aba Network para requisições falhando

**Soluções:**

#### **Problema: CORS**
```bash
# Verificar se backend está rodando
curl http://localhost:5005/api/health

# Se não responder, reiniciar backend
cd backend-mock
python3 server.py
```

#### **Problema: Variável VITE_API_URL**
```bash
# Verificar se arquivo .env existe
ls -la frontend/acervo-educacional-frontend/.env

# Se não existir, criar
cd frontend/acervo-educacional-frontend
cp .env.example .env

# Verificar conteúdo
cat .env
# Deve conter: VITE_API_URL=http://localhost:5005/api
```

#### **Problema: Credenciais Incorretas**
**Credenciais corretas:**
- Email: `admin@acervoeducacional.com`
- Senha: `Admin@123`

### **❌ Redirecionamento não funciona após login**

**Sintomas:**
- Login bem-sucedido mas não vai para dashboard
- Fica na tela de login

**Solução:**
```bash
# Verificar se AuthContext está funcionando
# No console do navegador:
localStorage.getItem('token')
# Deve retornar um token JWT

# Se não houver token, verificar resposta da API
# Na aba Network, verificar resposta de /api/v1/auth/login
```

---

## 🌐 **Problemas de Rede**

### **❌ API não responde**

**Sintomas:**
- Erro "Failed to fetch"
- Timeout nas requisições

**Diagnóstico:**
```bash
# Testar conectividade
curl -v http://localhost:5005/api/health

# Verificar se serviço está rodando
ps aux | grep python3
```

**Solução:**
```bash
# Reiniciar backend mock
cd backend-mock
python3 server.py

# Verificar logs para erros
```

### **❌ Frontend não carrega**

**Sintomas:**
- Página em branco
- Erro 404 ou conexão recusada

**Solução:**
```bash
# Verificar se Vite está rodando
ps aux | grep vite

# Reiniciar frontend
cd frontend/acervo-educacional-frontend
npm run dev

# Verificar porta correta (geralmente 5173 ou 5175)
```

---

## 🐳 **Problemas com Docker**

### **❌ Containers não iniciam**

**Sintomas:**
- Erro ao executar `docker compose up`
- Containers param imediatamente

**Diagnóstico:**
```bash
# Verificar status dos containers
docker compose ps

# Verificar logs
docker compose logs
```

**Soluções:**

#### **Problema: Portas ocupadas**
```bash
# Verificar portas em uso
netstat -tulpn | grep -E "(3000|5000|5432|6379)"

# Parar serviços conflitantes
sudo systemctl stop postgresql
sudo systemctl stop redis
```

#### **Problema: Falta de recursos**
```bash
# Verificar espaço em disco
df -h

# Verificar memória
free -h

# Limpar Docker se necessário
docker system prune -a
```

### **❌ Build falha**

**Sintomas:**
- Erro durante `docker compose build`
- Dependências não encontradas

**Solução:**
```bash
# Rebuild forçado
docker compose build --no-cache

# Verificar Dockerfile
cat Dockerfile
```

---

## 🗄️ **Problemas de Banco de Dados**

### **❌ Conexão com PostgreSQL falha**

**Sintomas:**
- Erro "connection refused"
- Timeout na conexão

**Solução:**
```bash
# Verificar se PostgreSQL está rodando
sudo systemctl status postgresql

# Iniciar se necessário
sudo systemctl start postgresql

# Verificar configurações de conexão
cat .env | grep DATABASE
```

### **❌ Migrations não aplicadas**

**Sintomas:**
- Erro "table does not exist"
- Schema desatualizado

**Solução:**
```bash
# Aplicar migrations manualmente
cd src/AcervoEducacional.WebApi
dotnet ef database update
```

---

## 🔍 **Problemas de Performance**

### **❌ Frontend lento**

**Sintomas:**
- Carregamento demorado
- Interface travando

**Soluções:**
```bash
# Verificar modo de desenvolvimento
npm run dev

# Para produção, usar build otimizado
npm run build
npm run preview
```

### **❌ Backend lento**

**Sintomas:**
- Requisições demoradas
- Timeout nas APIs

**Diagnóstico:**
```bash
# Verificar logs do backend
tail -f logs/app.log

# Monitorar recursos
htop
```

---

## 🛠️ **Ferramentas de Diagnóstico**

### **Verificação Rápida do Sistema:**
```bash
# Script de verificação completa
#!/bin/bash
echo "=== Verificação do Sistema ==="

echo "1. Verificando Node.js..."
node --version

echo "2. Verificando Python..."
python3 --version

echo "3. Verificando serviços..."
curl -s http://localhost:5005/api/health && echo "✅ Backend OK" || echo "❌ Backend FALHOU"
curl -s http://localhost:5175 && echo "✅ Frontend OK" || echo "❌ Frontend FALHOU"

echo "4. Verificando processos..."
ps aux | grep -E "(python3|node)" | grep -v grep

echo "=== Verificação Concluída ==="
```

### **Logs Importantes:**
```bash
# Logs do backend mock
tail -f backend-mock/logs/app.log

# Logs do frontend (no terminal onde rodou npm run dev)
# Logs do Docker
docker compose logs -f

# Logs do sistema
journalctl -f
```

---

## 📞 **Quando Pedir Ajuda**

Se os problemas persistirem após tentar as soluções acima:

1. **Colete informações:**
   - Versão do sistema operacional
   - Versões do Node.js e Python
   - Logs de erro completos
   - Passos para reproduzir o problema

2. **Abra uma issue:**
   - Repositório: https://github.com/CarlosBertoldo/acervo-educacional/issues
   - Use o template de bug report
   - Inclua todas as informações coletadas

3. **Documentação adicional:**
   - [QUICK-START.md](../QUICK-START.md) - Guia rápido
   - [INSTALLATION.md](INSTALLATION.md) - Instalação detalhada
   - [DEPLOYMENT.md](DEPLOYMENT.md) - Deploy em produção

---

**💡 Dica:** Mantenha sempre as dependências atualizadas e consulte este guia antes de abrir issues!

