using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class LogAtividade : BaseEntity
{
    public Guid? UsuarioId { get; set; }
    public Guid? CursoId { get; set; }
    public Guid? ArquivoId { get; set; }
    public TipoAcao Acao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Dictionary<string, object>? DadosAdicionais { get; set; }

    // Relacionamentos
    public virtual Usuario? Usuario { get; set; }
    public virtual Curso? Curso { get; set; }
    public virtual Arquivo? Arquivo { get; set; }
}

public class SessaoUsuario : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevogado { get; set; } = false;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;

    public bool IsValido()
    {
        return !IsRevogado && ExpiresAt > DateTime.UtcNow;
    }

    public void Revogar()
    {
        IsRevogado = true;
        UpdateTimestamp();
    }
}

public class TokenRecuperacao : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsado { get; set; } = false;
    public string? IpAddress { get; set; }

    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;

    public bool IsValido()
    {
        return !IsUsado && ExpiresAt > DateTime.UtcNow;
    }

    public void Usar()
    {
        IsUsado = true;
        UpdateTimestamp();
    }
}

public class ConfiguracaoSistema : BaseEntity
{
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Categoria { get; set; }
}

public class SincronizacaoSenior : BaseEntity
{
    public StatusSincronizacao Status { get; set; }
    public DateTime IniciadaEm { get; set; }
    public DateTime? ConcluidaEm { get; set; }
    public int TotalCursos { get; set; }
    public int CursosProcessados { get; set; }
    public int CursosComErro { get; set; }
    public int CursosAtualizados { get; set; }
    public int CursosCriados { get; set; }
    public string? MensagemErro { get; set; }
    public Dictionary<string, object>? Detalhes { get; set; }

    public void Iniciar(int totalCursos)
    {
        Status = StatusSincronizacao.EmAndamento;
        IniciadaEm = DateTime.UtcNow;
        TotalCursos = totalCursos;
        UpdateTimestamp();
    }

    public void Concluir()
    {
        Status = StatusSincronizacao.Concluida;
        ConcluidaEm = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void MarcarErro(string mensagem)
    {
        Status = StatusSincronizacao.Erro;
        MensagemErro = mensagem;
        ConcluidaEm = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void IncrementarProcessado(bool sucesso = true, bool criado = false)
    {
        CursosProcessados++;
        if (sucesso)
        {
            if (criado)
                CursosCriados++;
            else
                CursosAtualizados++;
        }
        else
        {
            CursosComErro++;
        }
        UpdateTimestamp();
    }
}

public class ConflitoCurso : BaseEntity
{
    public Guid CursoId { get; set; }
    public string CodigoSenior { get; set; } = string.Empty;
    public TipoConflito TipoConflito { get; set; }
    public ResolucaoConflito? Resolucao { get; set; }
    public Dictionary<string, object>? DadosLocal { get; set; }
    public Dictionary<string, object>? DadosSenior { get; set; }
    public string? Observacoes { get; set; }
    public bool IsResolvido { get; set; } = false;
    public DateTime? ResolvidoEm { get; set; }
    public Guid? ResolvidoPorId { get; set; }

    // Relacionamentos
    public virtual Curso? Curso { get; set; }
    public virtual Usuario? ResolvidoPor { get; set; }

    public void Resolver(ResolucaoConflito resolucao, Guid usuarioId, string? observacoes = null)
    {
        Resolucao = resolucao;
        IsResolvido = true;
        ResolvidoEm = DateTime.UtcNow;
        ResolvidoPorId = usuarioId;
        if (!string.IsNullOrEmpty(observacoes))
            Observacoes = observacoes;
        UpdateTimestamp();
    }
}

