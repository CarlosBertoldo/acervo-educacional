# 🚀 Instalação em 3 Passos - Acervo Educacional Clean Architecture

## ✅ **SIM, você ainda pode usar a instalação em 3 passos!**

O projeto foi atualizado para **Clean Architecture** e os scripts foram adaptados para funcionar perfeitamente, incluindo a instalação automática do `node_modules`.

---

## 📋 **Pré-requisitos**

- **Docker** e **Docker Compose** instalados
- **Git** para clonar o repositório
- **4GB RAM** mínimo disponível
- **Portas livres**: 3000, 5000, 5432, 6379

---

## 🎯 **Instalação em 3 Passos**

### **Passo 1: Clone e Configure**
```bash
# Clone o repositório
git clone https://github.com/CarlosBertoldo/acervo-educacional.git
cd acervo-educacional

# Configure variáveis de ambiente
cp .env.example .env
nano .env  # Edite conforme necessário
```

### **Passo 2: Execute a Instalação**
```bash
# Execute o script de instalação automática
./scripts/deploy.sh install
```

### **Passo 3: Acesse o Sistema**
```bash
# Aguarde a instalação concluir e acesse:
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

---

## 🔧 **O que o Script Faz Automaticamente**

### ✅ **Verificações Iniciais:**
- Verifica se Docker e Docker Compose estão instalados
- Valida arquivo `.env` (cria do exemplo se necessário)
- Cria estrutura de diretórios necessários

### ✅ **Instalação do Frontend:**
- **Detecta automaticamente** se `node_modules` não existe
- **Instala dependências** usando pnpm, yarn ou npm (nesta ordem)
- **Constrói** a aplicação React para produção

### ✅ **Configuração do Backend:**
- **Compila** o projeto Clean Architecture (.NET 8)
- **Configura** todas as 4 camadas (Domain, Application, Infrastructure, WebApi)
- **Aplica** migrations do banco de dados

### ✅ **Infraestrutura:**
- **PostgreSQL** para banco de dados
- **Redis** para cache e sessões
- **Nginx** como proxy reverso
- **Health checks** para todos os serviços

---

## 📊 **Estrutura Clean Architecture Suportada**

```
acervo-educacional/
├── src/                           # Backend Clean Architecture ✅
│   ├── AcervoEducacional.Domain/          # Entidades e regras de negócio
│   ├── AcervoEducacional.Application/     # Casos de uso e DTOs
│   ├── AcervoEducacional.Infrastructure/  # Repositórios e serviços
│   └── AcervoEducacional.WebApi/          # Controllers e API
├── frontend/                      # Frontend React ✅
│   └── acervo-educacional-frontend/
├── database/                      # Scripts SQL ✅
├── scripts/                       # Scripts de deploy ✅
└── docker-compose.yml             # Orquestração ✅
```

---

## 🛠️ **Comandos Disponíveis**

```bash
# Instalação inicial completa
./scripts/deploy.sh install

# Atualizar sistema existente
./scripts/deploy.sh update

# Verificar status dos serviços
./scripts/deploy.sh status

# Ver logs em tempo real
./scripts/deploy.sh logs

# Fazer backup
./scripts/deploy.sh backup

# Reiniciar serviços
./scripts/deploy.sh restart

# Parar todos os serviços
./scripts/deploy.sh stop

# Limpeza de recursos
./scripts/deploy.sh cleanup
```

---

## 🔍 **Verificação Pós-Instalação**

### ✅ **Serviços Funcionando:**
```bash
# Verificar se todos os containers estão rodando
docker ps

# Verificar logs se houver problemas
./scripts/deploy.sh logs
```

### ✅ **URLs de Acesso:**
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Hangfire**: http://localhost:5000/hangfire

### ✅ **Credenciais Padrão:**
- **Email**: admin@acervo.com
- **Senha**: Admin123!

---

## 🚨 **Resolução de Problemas**

### **Problema: node_modules não instalado**
```bash
# O script instala automaticamente, mas se precisar fazer manual:
cd frontend/acervo-educacional-frontend
npm install  # ou yarn install / pnpm install
cd ../..
./scripts/deploy.sh restart
```

### **Problema: Erro de compilação .NET**
```bash
# Verificar se todas as dependências estão corretas
cd src/AcervoEducacional.WebApi
dotnet restore
dotnet build
```

### **Problema: Banco não conecta**
```bash
# Verificar se PostgreSQL está rodando
docker ps | grep postgres
./scripts/deploy.sh logs postgres
```

### **Problema: Portas ocupadas**
```bash
# Verificar portas em uso
netstat -tulpn | grep -E "(3000|5000|5432|6379)"

# Parar serviços conflitantes ou alterar portas no docker-compose.yml
```

---

## 🎉 **Vantagens da Nova Versão**

### ✅ **Clean Architecture:**
- **Código organizado** em 4 camadas bem definidas
- **Fácil manutenção** e extensibilidade
- **Testes** mais simples de implementar

### ✅ **Instalação Melhorada:**
- **Detecção automática** de dependências
- **Instalação inteligente** do node_modules
- **Verificações** de saúde dos serviços

### ✅ **Sem node_modules no Git:**
- **Repositório mais leve** (18MB vs 500MB+)
- **Clone mais rápido**
- **Builds automáticos** via package.json

---

## 📞 **Suporte**

Se encontrar problemas:

1. **Verifique os logs**: `./scripts/deploy.sh logs`
2. **Consulte a documentação**: [README.md](README.md)
3. **Abra uma issue**: https://github.com/CarlosBertoldo/acervo-educacional/issues

---

**✨ A instalação em 3 passos funciona perfeitamente com Clean Architecture! ✨**

