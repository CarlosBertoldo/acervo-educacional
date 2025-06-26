# Banco de Dados - Sistema Acervo Educacional

Este diretório contém todos os scripts e configurações necessárias para configurar o banco de dados PostgreSQL do Sistema Acervo Educacional.

## 📁 Estrutura de Arquivos

```
database/
├── 01_create_database.sql      # Criação do banco e tipos ENUM
├── 02_create_tables.sql        # Criação das tabelas principais
├── 03_functions_triggers.sql   # Funções e triggers automáticos
├── 04_initial_data.sql         # Dados iniciais e configurações
├── setup_database.sh           # Script automatizado de setup
├── .env.example               # Exemplo de configurações
└── README.md                  # Esta documentação
```

## 🚀 Configuração Rápida

### 1. Pré-requisitos

- PostgreSQL 14+ instalado e rodando
- Usuário com privilégios de criação de banco
- Extensões `uuid-ossp` e `pgcrypto` disponíveis

### 2. Configuração de Ambiente

```bash
# Copie o arquivo de exemplo
cp .env.example .env

# Edite as configurações conforme seu ambiente
nano .env
```

### 3. Execução Automatizada

```bash
# Execute o script de setup
./setup_database.sh
```

### 4. Configuração Manual (Alternativa)

```bash
# Execute os scripts na ordem
psql -U postgres -f 01_create_database.sql
psql -U postgres -d acervo_educacional -f 02_create_tables.sql
psql -U postgres -d acervo_educacional -f 03_functions_triggers.sql
psql -U postgres -d acervo_educacional -f 04_initial_data.sql
```

## 📊 Estrutura do Banco

### Tabelas Principais

| Tabela | Descrição | Registros Iniciais |
|--------|-----------|-------------------|
| `usuarios` | Usuários do sistema | 1 (admin) |
| `cursos` | Cursos educacionais | 3 (demo) |
| `arquivos` | Arquivos dos cursos | 0 |
| `logs_atividade` | Logs de auditoria | 1 (inicialização) |
| `configuracoes_sistema` | Configurações gerais | 25+ |
| `tokens_recuperacao` | Tokens de recuperação de senha | 0 |
| `sessoes_usuario` | Controle de sessões JWT | 0 |

### Tipos ENUM

- `status_curso`: Backlog, EmDesenvolvimento, Veiculado
- `origem_curso`: Manual, Senior
- `categoria_arquivo`: BriefingDesenvolvimento, BriefingExecucao, PPT, CadernoExercicio, PlanoAula, Videos, Podcast, OutrosArquivos
- `tipo_acao`: CriacaoCurso, EdicaoCurso, MovimentacaoStatus, UploadArquivo, DownloadArquivo, etc.

### Views Criadas

- `vw_relatorio_cursos`: Relatórios completos de cursos
- `vw_estatisticas_arquivos`: Estatísticas por categoria
- `vw_logs_resumo`: Resumo de logs dos últimos 30 dias

## 🔧 Funções Especiais

### Funções de Negócio

- `buscar_cursos()`: Busca full-text com filtros
- `obter_estatisticas_dashboard()`: Estatísticas para dashboard
- `cleanup_expired_tokens()`: Limpeza automática de tokens

### Triggers Automáticos

- Atualização automática de `updated_at`
- Log automático de alterações em cursos
- Log automático de operações em arquivos

## 👤 Usuário Padrão

**Email:** admin@acervoeducacional.com  
**Senha:** Admin@123

> ⚠️ **IMPORTANTE:** Altere a senha padrão no primeiro login!

## 🔒 Configurações de Segurança

### Senhas
- Hash bcrypt com salt
- Mínimo 8 caracteres
- Caracteres especiais obrigatórios

### Tokens JWT
- Expiração configurável (padrão: 24h)
- Refresh tokens (padrão: 30 dias)
- Controle de sessões ativas

### Auditoria
- Log de todas as ações sensíveis
- Rastreamento de IP e User-Agent
- Retenção configurável (padrão: 365 dias)

## 📈 Performance

### Índices Criados

- Índices únicos em campos chave
- Índices compostos para consultas frequentes
- Índices full-text para busca
- Índices de data para logs

### Otimizações

- Particionamento de logs por data
- Limpeza automática de dados expirados
- Views materializadas para relatórios
- Pool de conexões configurável

## 🔄 Backup e Recuperação

### Backup Automático

```bash
# Backup diário (configurar no cron)
0 2 * * * pg_dump acervo_educacional > backup_$(date +\%Y\%m\%d).sql
```

### Restauração

```bash
# Restaurar backup
psql -U postgres -d acervo_educacional < backup_20250626.sql
```

### Versionamento

- Controle de versão do schema
- Migrations automáticas
- Rollback de alterações

## 🔍 Monitoramento

### Queries Úteis

```sql
-- Estatísticas gerais
SELECT * FROM obter_estatisticas_dashboard();

-- Cursos por status
SELECT status, COUNT(*) FROM cursos GROUP BY status;

-- Arquivos por categoria
SELECT categoria, COUNT(*), SUM(tamanho) FROM arquivos GROUP BY categoria;

-- Logs recentes
SELECT * FROM logs_atividade ORDER BY created_at DESC LIMIT 10;

-- Usuários ativos
SELECT COUNT(*) FROM usuarios WHERE is_active = true;
```

### Health Check

```sql
-- Verificar conexões ativas
SELECT count(*) FROM pg_stat_activity WHERE datname = 'acervo_educacional';

-- Verificar tamanho do banco
SELECT pg_size_pretty(pg_database_size('acervo_educacional'));

-- Verificar índices não utilizados
SELECT schemaname, tablename, indexname, idx_scan 
FROM pg_stat_user_indexes 
WHERE idx_scan = 0;
```

## 🐛 Troubleshooting

### Problemas Comuns

1. **Erro de conexão**
   - Verificar se PostgreSQL está rodando
   - Conferir credenciais no .env
   - Verificar firewall/rede

2. **Erro de permissão**
   - Verificar privilégios do usuário
   - Conferir ownership das tabelas
   - Verificar configurações do pg_hba.conf

3. **Performance lenta**
   - Analisar queries com EXPLAIN
   - Verificar índices utilizados
   - Conferir configurações de memória

### Logs de Debug

```bash
# Habilitar log de queries
ALTER SYSTEM SET log_statement = 'all';
SELECT pg_reload_conf();

# Verificar logs
tail -f /var/log/postgresql/postgresql-14-main.log
```

## 📚 Referências

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Best Practices](https://auth0.com/blog/a-look-at-the-latest-draft-for-jwt-bcp/)
- [Database Security](https://www.postgresql.org/docs/current/security.html)

## 📞 Suporte

Para problemas relacionados ao banco de dados:

1. Verificar logs do PostgreSQL
2. Consultar esta documentação
3. Verificar issues conhecidos no repositório
4. Contatar a equipe de desenvolvimento

