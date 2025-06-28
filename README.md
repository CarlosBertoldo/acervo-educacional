# 🎓 Acervo Educacional - Sistema de Gestão de Cursos

Sistema web moderno para gerenciamento de cursos e arquivos educacionais com painel administrativo Kanban, desenvolvido com **Clean Architecture** e tecnologias atuais.

## 🚀 **Início Rápido**

```bash
# 1. Clonar o repositório
git clone https://github.com/CarlosBertoldo/acervo-educacional.git
cd acervo-educacional

# 2. Configurar frontend
cd frontend/acervo-educacional-frontend
cp .env.example .env
npm install

# 3. Terminal 1 - Backend Mock
cd ../../backend-mock
pip3 install flask flask-cors pyjwt
python3 server.py

# 4. Terminal 2 - Frontend
cd ../frontend/acervo-educacional-frontend
npm run dev

# 5. Acessar: http://localhost:5175
# Login: admin@acervoeducacional.com / Admin@123
```

> 📖 **Para instruções detalhadas, consulte:** [QUICK-START.md](QUICK-START.md)

---

## 🏗️ **Arquitetura**

Este projeto segue os princípios da **Clean Architecture**, organizando o código em camadas bem definidas:

```
src/
├── AcervoEducacional.Domain/          # 🏛️ Camada de Domínio
│   ├── Entities/                      # Entidades de negócio
│   ├── Enums/                         # Enumerações
│   └── Interfaces/                    # Interfaces de repositórios
├── AcervoEducacional.Application/     # 🔧 Camada de Aplicação
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Interfaces/                    # Interfaces de serviços
│   ├── Services/                      # Casos de uso
│   └── Mappings/                      # Mapeamentos AutoMapper
├── AcervoEducacional.Infrastructure/  # 🔌 Camada de Infraestrutura
│   ├── Data/                          # Contexto Entity Framework
│   ├── Repositories/                  # Implementações de repositórios
│   ├── Services/                      # Serviços externos (AWS S3, Email)
│   └── Configurations/                # Configurações de DI
└── AcervoEducacional.WebApi/          # 🌐 Camada de Apresentação
    ├── Controllers/                   # Controllers da API
    ├── Middleware/                    # Middlewares customizados
    └── Extensions/                    # Extensões de configuração
```

> 📖 **Documentação completa:** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

---

## ✨ **Funcionalidades**

### 🎯 **Principais Recursos:**
- ✅ **Dashboard Administrativo** - Visão geral com métricas e estatísticas
- ✅ **Sistema Kanban** - Gerenciamento visual do fluxo de cursos
- ✅ **Gestão de Cursos** - CRUD completo para cursos educacionais
- ✅ **Gestão de Usuários** - Controle de acesso e permissões
- ✅ **Sistema de Logs** - Auditoria e monitoramento de ações
- ✅ **Configurações** - Personalização do sistema

### 🎨 **Design e UX:**
- ✅ **Design da Marca Ferreira Costa** - Cores e tipografia oficiais
- ✅ **Interface Responsiva** - Funciona em desktop e mobile
- ✅ **Tipografia Barlow** - Fonte oficial da marca
- ✅ **Componentes Modernos** - shadcn/ui + Tailwind CSS

### 🔐 **Segurança:**
- ✅ **Autenticação JWT** - Tokens seguros para acesso
- ✅ **Controle de Acesso** - Permissões baseadas em roles
- ✅ **Middleware de Segurança** - Proteção contra ataques comuns
- ✅ **Validação de Dados** - Sanitização e validação de inputs

---

## 🛠️ **Tecnologias**

### **Frontend:**
- **React 18** - Biblioteca para interfaces de usuário
- **Vite** - Build tool moderna e rápida
- **React Router DOM** - Roteamento SPA
- **Tailwind CSS** - Framework CSS utilitário
- **shadcn/ui** - Componentes UI modernos
- **Recharts** - Gráficos e visualizações

### **Backend:**
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para banco de dados
- **PostgreSQL** - Banco de dados principal
- **Redis** - Cache e sessões
- **Hangfire** - Jobs em background
- **Swagger** - Documentação da API

### **DevOps:**
- **Docker** - Containerização
- **Docker Compose** - Orquestração local
- **Nginx** - Proxy reverso
- **GitHub Actions** - CI/CD (planejado)

---

## 📚 **Documentação**

### **🚀 Guias de Início:**
- [QUICK-START.md](QUICK-START.md) - Guia rápido para começar
- [docs/INSTALLATION.md](docs/INSTALLATION.md) - Instalação completa
- [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) - Deploy em produção

### **📖 Documentação Técnica:**
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) - Arquitetura do sistema
- [docs/documentacao-tecnica.md](docs/documentacao-tecnica.md) - Especificações técnicas
- [docs/especificacao-tecnica.md](docs/especificacao-tecnica.md) - Requisitos detalhados

### **👤 Documentação do Usuário:**
- [docs/manual-usuario.md](docs/manual-usuario.md) - Manual do usuário final
- [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) - Solução de problemas

---

## 🌐 **URLs de Acesso**

### **Desenvolvimento:**
- **Frontend:** http://localhost:5175
- **Backend API:** http://localhost:5006/api
- **Swagger UI:** http://localhost:5006/swagger

### **Credenciais Padrão:**
```
Email: admin@acervoeducacional.com
Senha: Admin@123
```

---

## 🔧 **Desenvolvimento**

### **Pré-requisitos:**
- Node.js 18+
- Python 3.8+
- .NET 8 SDK
- PostgreSQL 15+
- Git

### **Estrutura do Projeto:**
```
acervo-educacional/
├── 📁 src/                    # Backend .NET (Clean Architecture)
├── 📁 frontend/               # Frontend React + Vite
├── 📁 backend-mock/           # Backend mock para desenvolvimento
├── 📁 database/               # Scripts e migrations do banco
├── 📁 docs/                   # Documentação técnica
├── 📁 scripts/                # Scripts de automação
├── 📄 docker-compose.yml      # Orquestração Docker
├── 📄 README.md               # Este arquivo
└── 📄 QUICK-START.md          # Guia rápido
```

### **Scripts Disponíveis:**
```bash
# Frontend
npm run dev          # Servidor de desenvolvimento
npm run build        # Build para produção
npm run preview      # Preview do build

# Backend Mock
python3 server.py    # Iniciar API mock

# Docker
docker compose up    # Iniciar todos os serviços
docker compose down  # Parar todos os serviços
```

---

## 🤝 **Contribuição**

1. **Fork** o projeto
2. **Crie** uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. **Push** para a branch (`git push origin feature/nova-funcionalidade`)
5. **Abra** um Pull Request

---

## 📄 **Licença**

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 📞 **Suporte**

- **Issues:** https://github.com/CarlosBertoldo/acervo-educacional/issues
- **Documentação:** [docs/](docs/)
- **Guia Rápido:** [QUICK-START.md](QUICK-START.md)

---

**Desenvolvido com ❤️ para a Ferreira Costa**

