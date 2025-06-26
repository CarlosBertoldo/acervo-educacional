-- Script de dados iniciais para o Sistema Acervo Educacional

-- Inserção de configurações padrão do sistema
INSERT INTO configuracoes_sistema (chave, valor, descricao, tipo) VALUES
('jwt_secret_key', 'AcervoEducacional2025!@#$%^&*()_+SecretKey', 'Chave secreta para assinatura de tokens JWT', 'string'),
('jwt_expiration_hours', '24', 'Tempo de expiração do token JWT em horas', 'number'),
('jwt_refresh_expiration_days', '30', 'Tempo de expiração do refresh token em dias', 'number'),
('max_file_size_mb', '500', 'Tamanho máximo de arquivo em MB', 'number'),
('allowed_file_types', '["pdf","doc","docx","ppt","pptx","xls","xlsx","mp4","avi","mov","mp3","wav","jpg","jpeg","png","gif"]', 'Tipos de arquivo permitidos', 'json'),
('s3_bucket_name', 'acervo-educacional-files', 'Nome do bucket S3 para armazenamento', 'string'),
('s3_region', 'us-east-1', 'Região do AWS S3', 'string'),
('email_smtp_host', 'smtp.gmail.com', 'Servidor SMTP para envio de emails', 'string'),
('email_smtp_port', '587', 'Porta do servidor SMTP', 'number'),
('email_from_address', 'noreply@acervoeducacional.com', 'Endereço de email remetente', 'string'),
('email_from_name', 'Sistema Acervo Educacional', 'Nome do remetente de emails', 'string'),
('senior_sync_enabled', 'true', 'Habilita sincronização com sistema Senior', 'boolean'),
('senior_sync_interval_minutes', '60', 'Intervalo de sincronização com Senior em minutos', 'number'),
('senior_db_connection', '', 'String de conexão com banco do Senior', 'string'),
('backup_retention_days', '30', 'Dias de retenção de backups', 'number'),
('session_timeout_minutes', '480', 'Timeout de sessão em minutos (8 horas)', 'number'),
('max_login_attempts', '5', 'Máximo de tentativas de login antes do bloqueio', 'number'),
('account_lockout_minutes', '30', 'Tempo de bloqueio da conta em minutos', 'number'),
('password_min_length', '8', 'Comprimento mínimo da senha', 'number'),
('password_require_special_chars', 'true', 'Exige caracteres especiais na senha', 'boolean'),
('video_player_protection', 'true', 'Habilita proteções no player de vídeo', 'boolean'),
('watermark_enabled', 'true', 'Habilita marca d''água em vídeos', 'boolean'),
('download_protection_enabled', 'true', 'Habilita proteção contra download', 'boolean'),
('embed_domain_restriction', 'true', 'Habilita restrição de domínio para embeds', 'boolean'),
('audit_log_retention_days', '365', 'Dias de retenção dos logs de auditoria', 'number'),
('maintenance_mode', 'false', 'Modo de manutenção do sistema', 'boolean'),
('system_version', '1.0.0', 'Versão atual do sistema', 'string'),
('last_backup_date', '', 'Data do último backup realizado', 'string'),
('dashboard_refresh_interval', '30', 'Intervalo de atualização do dashboard em segundos', 'number');

-- Inserção de usuário administrador padrão
-- Senha padrão: Admin@123 (deve ser alterada no primeiro login)
INSERT INTO usuarios (id, email, password_hash, nome, is_active) VALUES
(
    '00000000-0000-0000-0000-000000000001',
    'admin@acervoeducacional.com',
    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBdXwtGtrKxQ4i', -- Hash para Admin@123
    'Administrador do Sistema',
    true
);

-- Inserção de cursos de exemplo para demonstração
INSERT INTO cursos (
    id, codigo_curso, nome_curso, descricao_academia, status, 
    tipo_ambiente, tipo_acesso, origem, criado_por, comentarios_internos
) VALUES
(
    uuid_generate_v4(),
    'DEMO001',
    'Introdução ao Sistema Acervo Educacional',
    'Academia de Demonstração',
    'Veiculado',
    'Online',
    'Público',
    'Manual',
    '00000000-0000-0000-0000-000000000001',
    'Curso de demonstração criado automaticamente para apresentar as funcionalidades do sistema.'
),
(
    uuid_generate_v4(),
    'DEMO002',
    'Gestão de Arquivos Educacionais',
    'Academia de Demonstração',
    'EmDesenvolvimento',
    'Híbrido',
    'Restrito',
    'Manual',
    '00000000-0000-0000-0000-000000000001',
    'Curso em desenvolvimento para demonstrar o fluxo de trabalho Kanban.'
),
(
    uuid_generate_v4(),
    'DEMO003',
    'Proteção de Conteúdo Digital',
    'Academia de Demonstração',
    'Backlog',
    'Online',
    'Premium',
    'Manual',
    '00000000-0000-0000-0000-000000000001',
    'Curso planejado para demonstrar recursos de proteção de conteúdo.'
);

-- Inserção de log inicial do sistema
INSERT INTO logs_atividade (
    usuario_id, tipo_acao, descricao, dados_novos, endereco_ip, user_agent
) VALUES (
    '00000000-0000-0000-0000-000000000001',
    'CriacaoCurso',
    'Sistema inicializado com dados de demonstração',
    '{"action": "system_initialization", "version": "1.0.0", "timestamp": "' || NOW() || '"}',
    '127.0.0.1',
    'System Initialization Script'
);

-- Criação de view para relatórios de cursos
CREATE VIEW vw_relatorio_cursos AS
SELECT 
    c.id,
    c.codigo_curso,
    c.nome_curso,
    c.descricao_academia,
    c.status,
    c.tipo_ambiente,
    c.tipo_acesso,
    c.origem,
    u.nome as criado_por_nome,
    c.data_inicio_operacao,
    c.created_at as data_criacao,
    c.updated_at as data_atualizacao,
    c.synced_at as data_sincronizacao,
    (SELECT COUNT(*) FROM arquivos a WHERE a.curso_id = c.id) as total_arquivos,
    (SELECT COUNT(*) FROM arquivos a WHERE a.curso_id = c.id AND a.categoria = 'Videos') as total_videos,
    (SELECT COUNT(*) FROM arquivos a WHERE a.curso_id = c.id AND a.categoria = 'PPT') as total_apresentacoes,
    (SELECT SUM(a.tamanho) FROM arquivos a WHERE a.curso_id = c.id) as tamanho_total_bytes
FROM cursos c
LEFT JOIN usuarios u ON c.criado_por = u.id;

-- Criação de view para estatísticas de arquivos
CREATE VIEW vw_estatisticas_arquivos AS
SELECT 
    categoria,
    COUNT(*) as total_arquivos,
    SUM(tamanho) as tamanho_total_bytes,
    AVG(tamanho) as tamanho_medio_bytes,
    MIN(created_at) as primeiro_upload,
    MAX(created_at) as ultimo_upload
FROM arquivos
GROUP BY categoria;

-- Criação de view para logs de auditoria resumidos
CREATE VIEW vw_logs_resumo AS
SELECT 
    DATE(created_at) as data,
    tipo_acao,
    COUNT(*) as total_acoes,
    COUNT(DISTINCT usuario_id) as usuarios_distintos
FROM logs_atividade
WHERE created_at >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY DATE(created_at), tipo_acao
ORDER BY data DESC, total_acoes DESC;

-- Comentários das views
COMMENT ON VIEW vw_relatorio_cursos IS 'View para relatórios completos de cursos com estatísticas';
COMMENT ON VIEW vw_estatisticas_arquivos IS 'View para estatísticas de arquivos por categoria';
COMMENT ON VIEW vw_logs_resumo IS 'View para resumo de logs de auditoria dos últimos 30 dias';

-- Criação de índices adicionais para performance
CREATE INDEX CONCURRENTLY idx_logs_atividade_data_usuario 
ON logs_atividade(DATE(created_at), usuario_id);

CREATE INDEX CONCURRENTLY idx_arquivos_curso_categoria 
ON arquivos(curso_id, categoria);

CREATE INDEX CONCURRENTLY idx_cursos_status_origem 
ON cursos(status, origem);

-- Atualização das estatísticas do PostgreSQL
ANALYZE usuarios;
ANALYZE cursos;
ANALYZE arquivos;
ANALYZE logs_atividade;
ANALYZE configuracoes_sistema;

-- Mensagem de conclusão
DO $$
BEGIN
    RAISE NOTICE 'Base de dados do Sistema Acervo Educacional inicializada com sucesso!';
    RAISE NOTICE 'Usuário administrador criado: admin@acervoeducacional.com';
    RAISE NOTICE 'Senha padrão: Admin@123 (altere no primeiro login)';
    RAISE NOTICE 'Cursos de demonstração criados: 3';
    RAISE NOTICE 'Configurações do sistema: % registros', (SELECT COUNT(*) FROM configuracoes_sistema);
END $$;

