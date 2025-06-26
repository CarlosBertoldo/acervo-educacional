# Acervo Educacional - Clean Architecture

Sistema web para gerenciamento de cursos e arquivos educacionais com painel administrativo Kanban.

## 🏗️ Arquitetura

Este projeto segue os princípios da **Clean Architecture**, organizando o código em camadas bem definidas:

```
src/
├── AcervoEducacional.Domain/          # Camada de Domínio
│   ├── Entities/                      # Entidades de negócio
│   ├── Enums/                         # Enumerações
│   └── Interfaces/                    # Interfaces de repositórios
├── AcervoEducacional.Application/     # Camada de Aplicação
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Interfaces/                    # Interfaces de serviços
│   ├── Services/                      # Casos de uso
│   └── Mappings/                      # Mapeamentos AutoMapper
├── AcervoEducacional.Infrastructure/  # Camada de Infraestrutura
│   ├── Data/                          # Contexto Entity Framework
│   ├── Repositories/                  # Implementações de repositórios
│   ├── Services/                      # Serviços externos (AWS S3, Email)
│   └── Configurations/                # Configurações de DI
└── AcervoEducacional.WebApi/          # Camada de Apresentação
    ├── Controllers/                   # Controllers da API
    ├── Middleware/                    # Middlewares customizados
    └── Extensions/                    # Extensões de configuração
```

## 🚀 Funcionalidades

### Painel Kanban
- **3 colunas fixas**: Backlog, Em Desenvolvimento, Veiculado
- **Cards interativos** com informações detalhadas dos cursos
- **Drag & drop** via menu de contexto
- **Filtros e busca** em tempo real

### Gestão de Arquivos
- **Upload seguro** com validação de tipo e tamanho (500MB)
- **Categorização** em 8 tipos específicos para educação
- **Compartilhamento** com links públicos, restritos ou com expiração
- **Proteção de conteúdo** multicamada

### Sistema de Proteção
- **Players customizados** com Video.js
- **Bloqueios avançados**: download, seek, teclas de atalho
- **Domínios restritos** para embed
- **Watermarks** e overlays de proteção

### Integração Senior
- **Sincronização automática** via cron
- **Detecção de conflitos** com resolução manual
- **Mapeamento de dados** flexível

### Relatórios e Analytics
- **Dashboard executivo** com métricas em tempo real
- **Exportação** em PDF, Excel e CSV
- **Logs de auditoria** completos

## 🛠️ Stack Tecnológica

### Backend (.NET 8)
- **ASP.NET Core 8** - Framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **JWT** - Autenticação
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação
- **Hangfire** - Tarefas em background
- **Serilog** - Logging estruturado
- **Swagger** - Documentação da API

### Frontend (React)
- **React 18** - Framework frontend
- **TypeScript** - Tipagem estática
- **TailwindCSS** - Framework CSS
- **Vite** - Build tool
- **React Router** - Roteamento
- **Axios** - Cliente HTTP

### Infraestrutura
- **Docker** - Containerização
- **Docker Compose** - Orquestração
- **AWS S3** - Armazenamento de arquivos
- **GitLab CI/CD** - Integração contínua
- **Nginx** - Proxy reverso

## 📋 Pré-requisitos

- **.NET 8 SDK**
- **Node.js 18+**
- **PostgreSQL 14+**
- **Docker & Docker Compose**
- **Conta AWS** (para S3)

## 🚀 Instalação e Execução

### 1. Clone o repositório
```bash
git clone https://github.com/CarlosBertoldo/acervo-educacional.git
cd acervo-educacional
```

### 2. Configuração do ambiente
```bash
# Copie o arquivo de exemplo
cp .env.example .env

# Edite as configurações
nano .env
```

### 3. Configuração do banco de dados
```bash
# Execute os scripts de criação
cd database
chmod +x setup_database.sh
./setup_database.sh
```

### 4. Execução com Docker
```bash
# Instalar e executar todos os serviços
./scripts/deploy.sh install

# Ou executar individualmente
docker-compose up -d
```

### 5. Execução em desenvolvimento

#### Backend
```bash
cd src/AcervoEducacional.WebApi
dotnet restore
dotnet run
```

#### Frontend
```bash
cd frontend/acervo-educacional-frontend
npm install
npm run dev
```

## 🌐 URLs de Acesso

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Hangfire Dashboard**: http://localhost:5000/hangfire

### Credenciais padrão
- **Email**: admin@acervo.com
- **Senha**: Admin123!

## 📚 Documentação

- [📋 Documentação Técnica](docs/documentacao-tecnica.md)
- [👤 Manual do Usuário](docs/manual-usuario.md)
- [⚙️ Guia de Instalação](docs/guia-instalacao.md)
- [🏗️ Arquitetura](docs/arquitetura.md)

## 🔧 Scripts Disponíveis

```bash
# Instalação completa
./scripts/deploy.sh install

# Atualização
./scripts/deploy.sh update

# Backup
./scripts/deploy.sh backup

# Logs
./scripts/deploy.sh logs api

# Status dos serviços
./scripts/deploy.sh status

# Limpeza
./scripts/deploy.sh cleanup
```

## 🧪 Testes

```bash
# Backend
cd src/AcervoEducacional.WebApi
dotnet test

# Frontend
cd frontend/acervo-educacional-frontend
npm test
```

## 📦 Build para Produção

```bash
# Backend
cd src/AcervoEducacional.WebApi
dotnet publish -c Release

# Frontend
cd frontend/acervo-educacional-frontend
npm run build
```

## 🔒 Segurança

- **Autenticação JWT** com refresh tokens
- **Autorização** baseada em claims
- **Validação** de entrada em todas as APIs
- **Logs de auditoria** para todas as ações
- **Proteção CORS** configurada
- **Headers de segurança** implementados

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Suporte

Para suporte técnico ou dúvidas:
- **Email**: dev@acervoeducacional.com
- **Issues**: [GitHub Issues](https://github.com/CarlosBertoldo/acervo-educacional/issues)

## 🎯 Roadmap

- [ ] Implementação completa dos serviços de aplicação
- [ ] Testes unitários e de integração
- [ ] Monitoramento com Application Insights
- [ ] Cache distribuído com Redis
- [ ] Notificações em tempo real com SignalR
- [ ] API GraphQL
- [ ] Mobile app com React Native

---

**Desenvolvido com ❤️ pela equipe Acervo Educacional**

