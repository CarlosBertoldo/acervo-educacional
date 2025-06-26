-- Script de criação do banco de dados Acervo Educacional
-- PostgreSQL 14+

-- Criação do banco de dados
CREATE DATABASE acervo_educacional
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE = 'pt_BR.UTF-8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Conectar ao banco criado
\c acervo_educacional;

-- Criação de extensões necessárias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Criação de tipos ENUM
CREATE TYPE status_curso AS ENUM ('Backlog', 'EmDesenvolvimento', 'Veiculado');
CREATE TYPE origem_curso AS ENUM ('Manual', 'Senior');
CREATE TYPE categoria_arquivo AS ENUM (
    'BriefingDesenvolvimento',
    'BriefingExecucao', 
    'PPT',
    'CadernoExercicio',
    'PlanoAula',
    'Videos',
    'Podcast',
    'OutrosArquivos'
);
CREATE TYPE tipo_acao AS ENUM (
    'CriacaoCurso',
    'EdicaoCurso',
    'MovimentacaoStatus',
    'UploadArquivo',
    'DownloadArquivo',
    'CompartilhamentoArquivo',
    'ExclusaoArquivo',
    'Login',
    'Logout',
    'AlteracaoSenha'
);

-- Comentários dos tipos
COMMENT ON TYPE status_curso IS 'Status possíveis para um curso no sistema Kanban';
COMMENT ON TYPE origem_curso IS 'Origem dos dados do curso - Manual ou sincronizado do Senior';
COMMENT ON TYPE categoria_arquivo IS 'Categorias de arquivos educacionais';
COMMENT ON TYPE tipo_acao IS 'Tipos de ações para log de auditoria';

