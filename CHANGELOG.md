# 📝 Changelog - Acervo Educacional

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Versionamento Semântico](https://semver.org/lang/pt-BR/).

---

## [1.2.0] - 2025-06-28

### ✅ Adicionado
- **Reorganização completa da documentação**
  - Novo README.md consolidado e organizado
  - Guia QUICK-START.md para início rápido
  - TROUBLESHOOTING.md para solução de problemas
  - CHANGELOG.md para controle de versões
- **Estrutura docs/ organizada**
  - ARCHITECTURE.md (movido de CLEAN-ARCHITECTURE.md)
  - DEPLOYMENT.md (movido de DEPLOY-INSTRUCTIONS.md)
  - INSTALLATION.md (movido de INSTALACAO-3-PASSOS.md)

### 🔧 Corrigido
- **Sistema de autenticação 100% funcional**
  - Variável VITE_API_URL configurada corretamente
  - Backend mock com CORS atualizado
  - Resposta da API corrigida com campos corretos
  - Login funcionando com redirecionamento para dashboard

### 🗑️ Removido
- **Arquivos obsoletos de documentação**
  - CREDENCIAIS-LOGIN.md (credenciais desatualizadas)
  - DEBUG-LOGIN.md (problemas já resolvidos)
  - DEPLOY-NOTES.md (notas temporárias)
  - GUIA-COMPLETO-LINKS-PROJETO.md (informações duplicadas)

### 📝 Alterado
- **Estrutura de documentação simplificada**
  - Redução de 15 → 8 arquivos .md (47% menos)
  - Informações centralizadas e atualizadas
  - Navegação mais clara entre documentos

---

## [1.1.0] - 2025-06-27

### ✅ Adicionado
- **Design da marca Ferreira Costa aplicado**
  - Cores oficiais (#C12D00, #8FBF00, #C2D100)
  - Tipografia Barlow aplicada globalmente
  - Tema CSS completo com variáveis da marca
  - Componentes redesenhados (Login, Dashboard, Kanban)

### 🔧 Corrigido
- **Backend mock funcional**
  - API completa com autenticação JWT
  - Swagger UI integrado
  - CORS configurado corretamente
  - Endpoints de dashboard, cursos e usuários

### 📝 Alterado
- **Interface do usuário modernizada**
  - Design responsivo para desktop e mobile
  - Animações e transições suaves
  - Experiência consistente em todas as páginas

---

## [1.0.0] - 2025-06-26

### ✅ Adicionado
- **Arquitetura Clean Architecture implementada**
  - Camada de Domínio (Entities, Interfaces)
  - Camada de Aplicação (DTOs, Services, Mappings)
  - Camada de Infraestrutura (Repositories, Data)
  - Camada de Apresentação (Controllers, WebApi)

- **Frontend React + Vite**
  - Interface administrativa moderna
  - Sistema de roteamento com React Router
  - Componentes UI com shadcn/ui
  - Estilização com Tailwind CSS

- **Sistema de autenticação**
  - JWT tokens para segurança
  - Context API para gerenciamento de estado
  - Proteção de rotas
  - Login/logout funcional

- **Funcionalidades principais**
  - Dashboard com métricas e estatísticas
  - Sistema Kanban para gestão de cursos
  - CRUD de cursos educacionais
  - Gestão de usuários e permissões
  - Sistema de logs e auditoria

- **Infraestrutura**
  - PostgreSQL como banco principal
  - Redis para cache e sessões
  - Docker e Docker Compose
  - Nginx como proxy reverso
  - Hangfire para jobs em background

### 🛠️ Tecnologias
- **Backend:** .NET 8, Entity Framework Core, PostgreSQL
- **Frontend:** React 18, Vite, Tailwind CSS, shadcn/ui
- **DevOps:** Docker, Docker Compose, Nginx
- **Ferramentas:** Swagger, Hangfire, Redis

---

## [Não Lançado]

### 🚀 Planejado
- **Funcionalidades avançadas**
  - Upload e gestão de arquivos
  - Sistema de notificações
  - Relatórios e analytics
  - API para integrações externas

- **Melhorias de UX**
  - Modo escuro/claro
  - Personalização de tema
  - Atalhos de teclado
  - Busca avançada

- **DevOps e Produção**
  - CI/CD com GitHub Actions
  - Deploy automatizado
  - Monitoramento e alertas
  - Backup automatizado

---

## 📋 **Tipos de Mudanças**

- **✅ Adicionado** - para novas funcionalidades
- **📝 Alterado** - para mudanças em funcionalidades existentes
- **🔧 Corrigido** - para correções de bugs
- **🗑️ Removido** - para funcionalidades removidas
- **🔒 Segurança** - para correções de vulnerabilidades

---

## 📞 **Links Úteis**

- **Repositório:** https://github.com/CarlosBertoldo/acervo-educacional
- **Issues:** https://github.com/CarlosBertoldo/acervo-educacional/issues
- **Documentação:** [docs/](docs/)
- **Guia Rápido:** [QUICK-START.md](QUICK-START.md)

