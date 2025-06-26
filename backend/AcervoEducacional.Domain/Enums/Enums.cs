namespace AcervoEducacional.Domain.Enums;

public enum StatusCurso
{
    Backlog = 1,
    EmDesenvolvimento = 2,
    Veiculado = 3
}

public enum TipoAmbiente
{
    Producao = 1,
    Homologacao = 2,
    Desenvolvimento = 3
}

public enum TipoAcesso
{
    Publico = 1,
    Restrito = 2,
    Privado = 3
}

public enum OrigemCurso
{
    Manual = 1,
    Senior = 2
}

public enum CategoriaArquivo
{
    BriefingDesenvolvimento = 1,
    BriefingExecucao = 2,
    PPT = 3,
    CadernoExercicio = 4,
    PlanoAula = 5,
    Videos = 6,
    Podcast = 7,
    OutrosArquivos = 8
}

public enum TipoAcao
{
    Criar = 1,
    Atualizar = 2,
    Excluir = 3,
    Visualizar = 4,
    Download = 5,
    Upload = 6,
    Compartilhar = 7,
    Login = 8,
    Logout = 9,
    AlterarSenha = 10,
    RecuperarSenha = 11,
    SincronizarSenior = 12,
    MoverCurso = 13,
    GerarRelatorio = 14,
    ExportarDados = 15
}

public enum StatusSincronizacao
{
    Pendente = 1,
    EmAndamento = 2,
    Concluida = 3,
    Erro = 4,
    Cancelada = 5
}

public enum TipoConflito
{
    CursoAtualizado = 1,
    CursoExcluido = 2,
    CursoNovo = 3,
    DadosDivergentes = 4
}

public enum ResolucaoConflito
{
    ManterLocal = 1,
    UsarSenior = 2,
    Ignorar = 3,
    ResolverManualmente = 4
}

