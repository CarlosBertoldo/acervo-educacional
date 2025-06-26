-- Script de criação das tabelas principais
-- Sistema Acervo Educacional

-- Tabela de usuários
CREATE TABLE usuarios (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    nome VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices para tabela usuarios
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_active ON usuarios(is_active);

-- Comentários da tabela usuarios
COMMENT ON TABLE usuarios IS 'Tabela de usuários do sistema';
COMMENT ON COLUMN usuarios.id IS 'Identificador único do usuário';
COMMENT ON COLUMN usuarios.email IS 'Email para login e comunicação';
COMMENT ON COLUMN usuarios.password_hash IS 'Hash da senha para autenticação segura';
COMMENT ON COLUMN usuarios.nome IS 'Nome completo do usuário';
COMMENT ON COLUMN usuarios.is_active IS 'Status ativo/inativo do usuário';

-- Tabela de cursos
CREATE TABLE cursos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    codigo_curso VARCHAR(50) NOT NULL UNIQUE,
    nome_curso VARCHAR(500) NOT NULL,
    descricao_academia TEXT,
    status status_curso NOT NULL DEFAULT 'Backlog',
    tipo_ambiente VARCHAR(100),
    tipo_acesso VARCHAR(100),
    data_inicio_operacao TIMESTAMP WITH TIME ZONE,
    origem origem_curso NOT NULL DEFAULT 'Manual',
    criado_por UUID REFERENCES usuarios(id),
    comentarios_internos TEXT,
    dados_senior JSONB, -- Para armazenar dados específicos do Senior
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    synced_at TIMESTAMP WITH TIME ZONE -- Data da última sincronização com Senior
);

-- Índices para tabela cursos
CREATE INDEX idx_cursos_codigo ON cursos(codigo_curso);
CREATE INDEX idx_cursos_status ON cursos(status);
CREATE INDEX idx_cursos_origem ON cursos(origem);
CREATE INDEX idx_cursos_criado_por ON cursos(criado_por);
CREATE INDEX idx_cursos_nome ON cursos USING gin(to_tsvector('portuguese', nome_curso));
CREATE INDEX idx_cursos_created_at ON cursos(created_at);

-- Comentários da tabela cursos
COMMENT ON TABLE cursos IS 'Tabela principal de cursos educacionais';
COMMENT ON COLUMN cursos.codigo_curso IS 'Código único identificador do curso';
COMMENT ON COLUMN cursos.nome_curso IS 'Nome completo do curso';
COMMENT ON COLUMN cursos.descricao_academia IS 'Descrição da academia responsável';
COMMENT ON COLUMN cursos.status IS 'Status atual no Kanban (Backlog, EmDesenvolvimento, Veiculado)';
COMMENT ON COLUMN cursos.origem IS 'Origem dos dados (Manual ou Senior)';
COMMENT ON COLUMN cursos.dados_senior IS 'Dados específicos sincronizados do Senior em formato JSON';

-- Tabela de arquivos
CREATE TABLE arquivos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    curso_id UUID NOT NULL REFERENCES cursos(id) ON DELETE CASCADE,
    nome VARCHAR(500) NOT NULL,
    nome_armazenamento VARCHAR(500) NOT NULL,
    categoria categoria_arquivo NOT NULL,
    tipo_mime VARCHAR(100) NOT NULL,
    tamanho BIGINT NOT NULL,
    url_s3 TEXT NOT NULL,
    is_publico BOOLEAN NOT NULL DEFAULT false,
    data_expiracao TIMESTAMP WITH TIME ZONE,
    dominios_permitidos TEXT[], -- Array de domínios permitidos para embed
    bloqueios_ativos JSONB, -- Configurações de bloqueio em JSON
    metadados JSONB, -- Metadados adicionais do arquivo
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices para tabela arquivos
CREATE INDEX idx_arquivos_curso_id ON arquivos(curso_id);
CREATE INDEX idx_arquivos_categoria ON arquivos(categoria);
CREATE INDEX idx_arquivos_tipo_mime ON arquivos(tipo_mime);
CREATE INDEX idx_arquivos_publico ON arquivos(is_publico);
CREATE INDEX idx_arquivos_nome ON arquivos USING gin(to_tsvector('portuguese', nome));

-- Comentários da tabela arquivos
COMMENT ON TABLE arquivos IS 'Tabela de arquivos educacionais vinculados aos cursos';
COMMENT ON COLUMN arquivos.nome IS 'Nome original do arquivo';
COMMENT ON COLUMN arquivos.nome_armazenamento IS 'Nome do arquivo no S3';
COMMENT ON COLUMN arquivos.categoria IS 'Categoria do arquivo educacional';
COMMENT ON COLUMN arquivos.url_s3 IS 'URL do arquivo no AWS S3';
COMMENT ON COLUMN arquivos.dominios_permitidos IS 'Lista de domínios permitidos para embed';
COMMENT ON COLUMN arquivos.bloqueios_ativos IS 'Configurações de bloqueio em formato JSON';

-- Tabela de logs de atividade
CREATE TABLE logs_atividade (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    curso_id UUID REFERENCES cursos(id),
    arquivo_id UUID REFERENCES arquivos(id),
    tipo_acao tipo_acao NOT NULL,
    descricao TEXT NOT NULL,
    dados_anteriores JSONB, -- Estado anterior em JSON
    dados_novos JSONB, -- Estado novo em JSON
    endereco_ip INET,
    user_agent TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices para tabela logs_atividade
CREATE INDEX idx_logs_usuario_id ON logs_atividade(usuario_id);
CREATE INDEX idx_logs_curso_id ON logs_atividade(curso_id);
CREATE INDEX idx_logs_arquivo_id ON logs_atividade(arquivo_id);
CREATE INDEX idx_logs_tipo_acao ON logs_atividade(tipo_acao);
CREATE INDEX idx_logs_created_at ON logs_atividade(created_at);

-- Comentários da tabela logs_atividade
COMMENT ON TABLE logs_atividade IS 'Tabela de logs de auditoria do sistema';
COMMENT ON COLUMN logs_atividade.tipo_acao IS 'Tipo da ação executada';
COMMENT ON COLUMN logs_atividade.dados_anteriores IS 'Estado dos dados antes da alteração em JSON';
COMMENT ON COLUMN logs_atividade.dados_novos IS 'Estado dos dados após a alteração em JSON';

-- Tabela de tokens de recuperação de senha
CREATE TABLE tokens_recuperacao (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    usuario_id UUID NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    token VARCHAR(255) NOT NULL UNIQUE,
    usado BOOLEAN NOT NULL DEFAULT false,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices para tabela tokens_recuperacao
CREATE INDEX idx_tokens_usuario_id ON tokens_recuperacao(usuario_id);
CREATE INDEX idx_tokens_token ON tokens_recuperacao(token);
CREATE INDEX idx_tokens_expires_at ON tokens_recuperacao(expires_at);

-- Comentários da tabela tokens_recuperacao
COMMENT ON TABLE tokens_recuperacao IS 'Tabela de tokens para recuperação de senha';
COMMENT ON COLUMN tokens_recuperacao.token IS 'Token único para recuperação';
COMMENT ON COLUMN tokens_recuperacao.usado IS 'Indica se o token já foi utilizado';

-- Tabela de configurações do sistema
CREATE TABLE configuracoes_sistema (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    chave VARCHAR(100) NOT NULL UNIQUE,
    valor TEXT NOT NULL,
    descricao TEXT,
    tipo VARCHAR(50) NOT NULL DEFAULT 'string', -- string, number, boolean, json
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices para tabela configuracoes_sistema
CREATE INDEX idx_configuracoes_chave ON configuracoes_sistema(chave);

-- Comentários da tabela configuracoes_sistema
COMMENT ON TABLE configuracoes_sistema IS 'Tabela de configurações gerais do sistema';
COMMENT ON COLUMN configuracoes_sistema.chave IS 'Chave única da configuração';
COMMENT ON COLUMN configuracoes_sistema.valor IS 'Valor da configuração';
COMMENT ON COLUMN configuracoes_sistema.tipo IS 'Tipo do valor (string, number, boolean, json)';

-- Tabela de sessões de usuário (para controle de JWT)
CREATE TABLE sessoes_usuario (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    usuario_id UUID NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL,
    refresh_token_hash VARCHAR(255),
    endereco_ip INET,
    user_agent TEXT,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    last_activity TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices para tabela sessoes_usuario
CREATE INDEX idx_sessoes_usuario_id ON sessoes_usuario(usuario_id);
CREATE INDEX idx_sessoes_token_hash ON sessoes_usuario(token_hash);
CREATE INDEX idx_sessoes_expires_at ON sessoes_usuario(expires_at);

-- Comentários da tabela sessoes_usuario
COMMENT ON TABLE sessoes_usuario IS 'Tabela de controle de sessões JWT dos usuários';
COMMENT ON COLUMN sessoes_usuario.token_hash IS 'Hash do token JWT para validação';
COMMENT ON COLUMN sessoes_usuario.refresh_token_hash IS 'Hash do refresh token';

