#!/bin/bash

# Script de Backup Automático do Banco de Dados
# Acervo Educacional - Ferreira Costa
# Autor: Sistema Automatizado
# Data: $(date +%Y-%m-%d)

# Configurações
DB_HOST="localhost"
DB_PORT="5432"
DB_NAME="acervo_educacional"
DB_USER="postgres"
DB_PASSWORD="postgres"

# Diretórios
BACKUP_DIR="/home/ubuntu/acervo-educacional-latest/backups"
LOG_DIR="/home/ubuntu/acervo-educacional-latest/logs"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="backup_acervo_${TIMESTAMP}.sql"
BACKUP_COMPRESSED="backup_acervo_${TIMESTAMP}.sql.gz"

# Criar diretórios se não existirem
mkdir -p "$BACKUP_DIR"
mkdir -p "$LOG_DIR"

# Função de log
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_DIR/backup.log"
}

# Função de limpeza de backups antigos (manter apenas 7 dias)
cleanup_old_backups() {
    log_message "Iniciando limpeza de backups antigos..."
    find "$BACKUP_DIR" -name "backup_acervo_*.sql.gz" -mtime +7 -delete
    log_message "Limpeza concluída. Backups mantidos: últimos 7 dias"
}

# Função principal de backup
perform_backup() {
    log_message "=== INICIANDO BACKUP DO BANCO DE DADOS ==="
    log_message "Banco: $DB_NAME"
    log_message "Host: $DB_HOST:$DB_PORT"
    log_message "Arquivo: $BACKUP_COMPRESSED"
    
    # Definir senha via variável de ambiente
    export PGPASSWORD="$DB_PASSWORD"
    
    # Executar backup
    if pg_dump -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" \
        --verbose --clean --no-owner --no-privileges \
        -f "$BACKUP_DIR/$BACKUP_FILE" 2>> "$LOG_DIR/backup.log"; then
        
        log_message "Dump do banco realizado com sucesso"
        
        # Comprimir backup
        if gzip "$BACKUP_DIR/$BACKUP_FILE"; then
            log_message "Backup comprimido com sucesso: $BACKUP_COMPRESSED"
            
            # Verificar tamanho do arquivo
            BACKUP_SIZE=$(du -h "$BACKUP_DIR/$BACKUP_COMPRESSED" | cut -f1)
            log_message "Tamanho do backup: $BACKUP_SIZE"
            
            # Limpeza de backups antigos
            cleanup_old_backups
            
            log_message "=== BACKUP CONCLUÍDO COM SUCESSO ==="
            return 0
        else
            log_message "ERRO: Falha ao comprimir o backup"
            return 1
        fi
    else
        log_message "ERRO: Falha ao realizar dump do banco de dados"
        return 1
    fi
}

# Função de verificação de pré-requisitos
check_prerequisites() {
    log_message "Verificando pré-requisitos..."
    
    # Verificar se pg_dump está disponível
    if ! command -v pg_dump &> /dev/null; then
        log_message "ERRO: pg_dump não encontrado. Instale postgresql-client"
        return 1
    fi
    
    # Verificar se gzip está disponível
    if ! command -v gzip &> /dev/null; then
        log_message "ERRO: gzip não encontrado"
        return 1
    fi
    
    # Verificar conectividade com o banco
    export PGPASSWORD="$DB_PASSWORD"
    if ! pg_isready -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" &> /dev/null; then
        log_message "AVISO: Não foi possível conectar ao banco. Backup pode falhar."
    fi
    
    log_message "Pré-requisitos verificados"
    return 0
}

# Função de restauração (para uso manual)
restore_backup() {
    if [ -z "$1" ]; then
        echo "Uso: $0 restore <arquivo_backup.sql.gz>"
        echo "Exemplo: $0 restore backup_acervo_20241228_143022.sql.gz"
        return 1
    fi
    
    RESTORE_FILE="$1"
    
    if [ ! -f "$BACKUP_DIR/$RESTORE_FILE" ]; then
        log_message "ERRO: Arquivo de backup não encontrado: $RESTORE_FILE"
        return 1
    fi
    
    log_message "=== INICIANDO RESTAURAÇÃO DO BANCO ==="
    log_message "Arquivo: $RESTORE_FILE"
    
    # Descomprimir e restaurar
    export PGPASSWORD="$DB_PASSWORD"
    if gunzip -c "$BACKUP_DIR/$RESTORE_FILE" | psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME"; then
        log_message "=== RESTAURAÇÃO CONCLUÍDA COM SUCESSO ==="
        return 0
    else
        log_message "ERRO: Falha na restauração do banco"
        return 1
    fi
}

# Função de listagem de backups
list_backups() {
    log_message "=== BACKUPS DISPONÍVEIS ==="
    ls -lh "$BACKUP_DIR"/backup_acervo_*.sql.gz 2>/dev/null | while read line; do
        echo "$line"
    done
}

# Função principal
main() {
    case "${1:-backup}" in
        "backup")
            check_prerequisites && perform_backup
            ;;
        "restore")
            restore_backup "$2"
            ;;
        "list")
            list_backups
            ;;
        "cleanup")
            cleanup_old_backups
            ;;
        *)
            echo "Uso: $0 [backup|restore|list|cleanup]"
            echo ""
            echo "Comandos:"
            echo "  backup          - Realizar backup do banco (padrão)"
            echo "  restore <file>  - Restaurar backup específico"
            echo "  list           - Listar backups disponíveis"
            echo "  cleanup        - Limpar backups antigos"
            echo ""
            echo "Exemplos:"
            echo "  $0                                    # Backup automático"
            echo "  $0 restore backup_acervo_20241228_143022.sql.gz"
            echo "  $0 list"
            ;;
    esac
}

# Executar função principal
main "$@"

