-- Funções e Triggers para o Sistema Acervo Educacional

-- Função para atualizar automaticamente o campo updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Triggers para atualização automática de updated_at
CREATE TRIGGER update_usuarios_updated_at 
    BEFORE UPDATE ON usuarios 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_cursos_updated_at 
    BEFORE UPDATE ON cursos 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_arquivos_updated_at 
    BEFORE UPDATE ON arquivos 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_configuracoes_sistema_updated_at 
    BEFORE UPDATE ON configuracoes_sistema 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Função para log automático de alterações em cursos
CREATE OR REPLACE FUNCTION log_curso_changes()
RETURNS TRIGGER AS $$
DECLARE
    usuario_atual UUID;
    acao tipo_acao;
BEGIN
    -- Obtém o usuário atual do contexto da aplicação
    -- Será definido pela aplicação usando SET LOCAL
    usuario_atual := COALESCE(
        NULLIF(current_setting('app.current_user_id', true), ''),
        '00000000-0000-0000-0000-000000000000'
    )::UUID;
    
    IF TG_OP = 'INSERT' THEN
        acao := 'CriacaoCurso';
        INSERT INTO logs_atividade (
            usuario_id, curso_id, tipo_acao, descricao, 
            dados_novos, endereco_ip, user_agent
        ) VALUES (
            usuario_atual, NEW.id, acao, 
            'Curso criado: ' || NEW.nome_curso,
            to_jsonb(NEW), 
            COALESCE(NULLIF(current_setting('app.client_ip', true), ''), '127.0.0.1')::INET,
            current_setting('app.user_agent', true)
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        -- Verifica se houve mudança de status
        IF OLD.status != NEW.status THEN
            acao := 'MovimentacaoStatus';
            INSERT INTO logs_atividade (
                usuario_id, curso_id, tipo_acao, descricao, 
                dados_anteriores, dados_novos, endereco_ip, user_agent
            ) VALUES (
                usuario_atual, NEW.id, acao,
                'Status alterado de ' || OLD.status || ' para ' || NEW.status,
                jsonb_build_object('status', OLD.status),
                jsonb_build_object('status', NEW.status),
                COALESCE(NULLIF(current_setting('app.client_ip', true), ''), '127.0.0.1')::INET,
                current_setting('app.user_agent', true)
            );
        ELSE
            acao := 'EdicaoCurso';
            INSERT INTO logs_atividade (
                usuario_id, curso_id, tipo_acao, descricao, 
                dados_anteriores, dados_novos, endereco_ip, user_agent
            ) VALUES (
                usuario_atual, NEW.id, acao,
                'Curso editado: ' || NEW.nome_curso,
                to_jsonb(OLD),
                to_jsonb(NEW),
                COALESCE(NULLIF(current_setting('app.client_ip', true), ''), '127.0.0.1')::INET,
                current_setting('app.user_agent', true)
            );
        END IF;
        RETURN NEW;
    END IF;
    RETURN NULL;
END;
$$ language 'plpgsql';

-- Trigger para log automático de alterações em cursos
CREATE TRIGGER log_curso_changes_trigger
    AFTER INSERT OR UPDATE ON cursos
    FOR EACH ROW EXECUTE FUNCTION log_curso_changes();

-- Função para log automático de operações em arquivos
CREATE OR REPLACE FUNCTION log_arquivo_changes()
RETURNS TRIGGER AS $$
DECLARE
    usuario_atual UUID;
    acao tipo_acao;
BEGIN
    usuario_atual := COALESCE(
        NULLIF(current_setting('app.current_user_id', true), ''),
        '00000000-0000-0000-0000-000000000000'
    )::UUID;
    
    IF TG_OP = 'INSERT' THEN
        acao := 'UploadArquivo';
        INSERT INTO logs_atividade (
            usuario_id, curso_id, arquivo_id, tipo_acao, descricao, 
            dados_novos, endereco_ip, user_agent
        ) VALUES (
            usuario_atual, NEW.curso_id, NEW.id, acao,
            'Arquivo enviado: ' || NEW.nome || ' (' || NEW.categoria || ')',
            to_jsonb(NEW),
            COALESCE(NULLIF(current_setting('app.client_ip', true), ''), '127.0.0.1')::INET,
            current_setting('app.user_agent', true)
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        acao := 'ExclusaoArquivo';
        INSERT INTO logs_atividade (
            usuario_id, curso_id, arquivo_id, tipo_acao, descricao, 
            dados_anteriores, endereco_ip, user_agent
        ) VALUES (
            usuario_atual, OLD.curso_id, OLD.id, acao,
            'Arquivo excluído: ' || OLD.nome,
            to_jsonb(OLD),
            COALESCE(NULLIF(current_setting('app.client_ip', true), ''), '127.0.0.1')::INET,
            current_setting('app.user_agent', true)
        );
        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ language 'plpgsql';

-- Trigger para log automático de operações em arquivos
CREATE TRIGGER log_arquivo_changes_trigger
    AFTER INSERT OR DELETE ON arquivos
    FOR EACH ROW EXECUTE FUNCTION log_arquivo_changes();

-- Função para limpeza automática de tokens expirados
CREATE OR REPLACE FUNCTION cleanup_expired_tokens()
RETURNS void AS $$
BEGIN
    -- Remove tokens de recuperação expirados
    DELETE FROM tokens_recuperacao 
    WHERE expires_at < NOW() OR usado = true;
    
    -- Remove sessões expiradas
    DELETE FROM sessoes_usuario 
    WHERE expires_at < NOW();
    
    -- Log da limpeza
    RAISE NOTICE 'Limpeza de tokens expirados executada em %', NOW();
END;
$$ language 'plpgsql';

-- Função para busca full-text em cursos
CREATE OR REPLACE FUNCTION buscar_cursos(
    termo_busca TEXT,
    status_filtro status_curso DEFAULT NULL,
    origem_filtro origem_curso DEFAULT NULL,
    limite INTEGER DEFAULT 50,
    offset_valor INTEGER DEFAULT 0
)
RETURNS TABLE (
    id UUID,
    codigo_curso VARCHAR(50),
    nome_curso VARCHAR(500),
    descricao_academia TEXT,
    status status_curso,
    origem origem_curso,
    created_at TIMESTAMP WITH TIME ZONE,
    relevancia REAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.id,
        c.codigo_curso,
        c.nome_curso,
        c.descricao_academia,
        c.status,
        c.origem,
        c.created_at,
        ts_rank(
            to_tsvector('portuguese', 
                COALESCE(c.nome_curso, '') || ' ' || 
                COALESCE(c.codigo_curso, '') || ' ' || 
                COALESCE(c.descricao_academia, '')
            ),
            plainto_tsquery('portuguese', termo_busca)
        ) as relevancia
    FROM cursos c
    WHERE 
        (termo_busca IS NULL OR termo_busca = '' OR
         to_tsvector('portuguese', 
            COALESCE(c.nome_curso, '') || ' ' || 
            COALESCE(c.codigo_curso, '') || ' ' || 
            COALESCE(c.descricao_academia, '')
         ) @@ plainto_tsquery('portuguese', termo_busca))
        AND (status_filtro IS NULL OR c.status = status_filtro)
        AND (origem_filtro IS NULL OR c.origem = origem_filtro)
    ORDER BY 
        CASE WHEN termo_busca IS NULL OR termo_busca = '' THEN c.created_at END DESC,
        CASE WHEN termo_busca IS NOT NULL AND termo_busca != '' THEN relevancia END DESC
    LIMIT limite
    OFFSET offset_valor;
END;
$$ language 'plpgsql';

-- Função para estatísticas do dashboard
CREATE OR REPLACE FUNCTION obter_estatisticas_dashboard()
RETURNS JSON AS $$
DECLARE
    resultado JSON;
BEGIN
    SELECT json_build_object(
        'total_cursos', (SELECT COUNT(*) FROM cursos),
        'cursos_por_status', (
            SELECT json_object_agg(status, total)
            FROM (
                SELECT status, COUNT(*) as total
                FROM cursos
                GROUP BY status
            ) as stats
        ),
        'cursos_por_origem', (
            SELECT json_object_agg(origem, total)
            FROM (
                SELECT origem, COUNT(*) as total
                FROM cursos
                GROUP BY origem
            ) as stats
        ),
        'total_arquivos', (SELECT COUNT(*) FROM arquivos),
        'arquivos_por_categoria', (
            SELECT json_object_agg(categoria, total)
            FROM (
                SELECT categoria, COUNT(*) as total
                FROM arquivos
                GROUP BY categoria
            ) as stats
        ),
        'usuarios_ativos', (SELECT COUNT(*) FROM usuarios WHERE is_active = true),
        'ultima_sincronizacao', (
            SELECT MAX(synced_at) 
            FROM cursos 
            WHERE origem = 'Senior' AND synced_at IS NOT NULL
        )
    ) INTO resultado;
    
    RETURN resultado;
END;
$$ language 'plpgsql';

-- Comentários das funções
COMMENT ON FUNCTION update_updated_at_column() IS 'Atualiza automaticamente o campo updated_at';
COMMENT ON FUNCTION log_curso_changes() IS 'Registra automaticamente alterações em cursos';
COMMENT ON FUNCTION log_arquivo_changes() IS 'Registra automaticamente operações em arquivos';
COMMENT ON FUNCTION cleanup_expired_tokens() IS 'Remove tokens e sessões expirados';
COMMENT ON FUNCTION buscar_cursos(TEXT, status_curso, origem_curso, INTEGER, INTEGER) IS 'Busca full-text em cursos com filtros';
COMMENT ON FUNCTION obter_estatisticas_dashboard() IS 'Retorna estatísticas para o dashboard';

