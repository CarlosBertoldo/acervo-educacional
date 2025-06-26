#!/bin/bash

# Script de configuração do banco de dados PostgreSQL
# Sistema Acervo Educacional

set -e  # Sair em caso de erro

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log colorido
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING: $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR: $1${NC}"
}

info() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')] INFO: $1${NC}"
}

# Configurações padrão
DB_HOST=${DB_HOST:-localhost}
DB_PORT=${DB_PORT:-5432}
DB_NAME=${DB_NAME:-acervo_educacional}
DB_USER=${DB_USER:-postgres}
DB_PASSWORD=${DB_PASSWORD:-postgres}
PGPASSWORD=$DB_PASSWORD

# Diretório dos scripts
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Função para verificar se PostgreSQL está rodando
check_postgres() {
    info "Verificando conexão com PostgreSQL..."
    if ! pg_isready -h $DB_HOST -p $DB_PORT -U $DB_USER > /dev/null 2>&1; then
        error "PostgreSQL não está acessível em $DB_HOST:$DB_PORT"
        error "Verifique se o PostgreSQL está rodando e as credenciais estão corretas"
        exit 1
    fi
    log "PostgreSQL está acessível"
}

# Função para verificar se o banco já existe
check_database_exists() {
    info "Verificando se o banco de dados '$DB_NAME' já existe..."
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -lqt | cut -d \| -f 1 | grep -qw $DB_NAME; then
        warn "Banco de dados '$DB_NAME' já existe"
        read -p "Deseja recriar o banco? (s/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Ss]$ ]]; then
            info "Removendo banco existente..."
            psql -h $DB_HOST -p $DB_PORT -U $DB_USER -c "DROP DATABASE IF EXISTS $DB_NAME;"
            log "Banco removido com sucesso"
        else
            info "Mantendo banco existente. Executando apenas atualizações..."
            return 1
        fi
    fi
    return 0
}

# Função para executar script SQL
execute_sql_script() {
    local script_file=$1
    local description=$2
    
    if [ ! -f "$script_file" ]; then
        error "Script não encontrado: $script_file"
        return 1
    fi
    
    info "Executando: $description"
    info "Arquivo: $(basename $script_file)"
    
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -f "$script_file" > /dev/null 2>&1; then
        log "✓ $description executado com sucesso"
    else
        error "✗ Falha ao executar: $description"
        error "Verifique o arquivo: $script_file"
        return 1
    fi
}

# Função para executar script SQL em banco específico
execute_sql_script_on_db() {
    local script_file=$1
    local description=$2
    local database=$3
    
    if [ ! -f "$script_file" ]; then
        error "Script não encontrado: $script_file"
        return 1
    fi
    
    info "Executando: $description"
    info "Arquivo: $(basename $script_file)"
    info "Banco: $database"
    
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $database -f "$script_file" > /dev/null 2>&1; then
        log "✓ $description executado com sucesso"
    else
        error "✗ Falha ao executar: $description"
        error "Verifique o arquivo: $script_file"
        return 1
    fi
}

# Função para criar backup
create_backup() {
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -lqt | cut -d \| -f 1 | grep -qw $DB_NAME; then
        local backup_file="backup_${DB_NAME}_$(date +%Y%m%d_%H%M%S).sql"
        info "Criando backup do banco existente..."
        if pg_dump -h $DB_HOST -p $DB_PORT -U $DB_USER $DB_NAME > "$backup_file"; then
            log "Backup criado: $backup_file"
        else
            warn "Falha ao criar backup"
        fi
    fi
}

# Função principal
main() {
    echo "=================================================="
    echo "  Sistema Acervo Educacional - Setup Database"
    echo "=================================================="
    echo
    
    info "Configurações:"
    info "Host: $DB_HOST"
    info "Porta: $DB_PORT"
    info "Banco: $DB_NAME"
    info "Usuário: $DB_USER"
    echo
    
    # Verificar PostgreSQL
    check_postgres
    
    # Criar backup se necessário
    create_backup
    
    # Verificar se banco existe
    local create_new_db=true
    if ! check_database_exists; then
        create_new_db=false
    fi
    
    if [ "$create_new_db" = true ]; then
        # Executar scripts de criação
        execute_sql_script "$SCRIPT_DIR/01_create_database.sql" "Criação do banco de dados"
        execute_sql_script_on_db "$SCRIPT_DIR/02_create_tables.sql" "Criação das tabelas" $DB_NAME
        execute_sql_script_on_db "$SCRIPT_DIR/03_functions_triggers.sql" "Criação de funções e triggers" $DB_NAME
        execute_sql_script_on_db "$SCRIPT_DIR/04_initial_data.sql" "Inserção de dados iniciais" $DB_NAME
    else
        # Executar apenas atualizações
        info "Executando atualizações no banco existente..."
        execute_sql_script_on_db "$SCRIPT_DIR/03_functions_triggers.sql" "Atualização de funções e triggers" $DB_NAME
    fi
    
    echo
    log "Setup do banco de dados concluído com sucesso!"
    echo
    info "Próximos passos:"
    info "1. Configure as variáveis de ambiente no backend"
    info "2. Execute o projeto .NET para testar a conexão"
    info "3. Acesse o sistema com: admin@acervoeducacional.com / Admin@123"
    echo
    warn "IMPORTANTE: Altere a senha padrão do administrador no primeiro login!"
    echo
}

# Verificar se está sendo executado diretamente
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi

