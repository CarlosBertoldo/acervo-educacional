using AcervoEducacional.Domain.Entities;

namespace AcervoEducacional.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = true;
    public bool IsAtivo { get; set; } = true;
    public DateTime? UltimoLogin { get; set; }
    public string? UltimoIp { get; set; }
    public string? UserAgent { get; set; }

    // Relacionamentos
    public virtual ICollection<SessaoUsuario> Sessoes { get; set; } = new List<SessaoUsuario>();
    public virtual ICollection<TokenRecuperacao> TokensRecuperacao { get; set; } = new List<TokenRecuperacao>();
    public virtual ICollection<LogAtividade> LogsAtividade { get; set; } = new List<LogAtividade>();
    public virtual ICollection<Curso> CursosCriados { get; set; } = new List<Curso>();

    public void AtualizarUltimoLogin(string? ip = null, string? userAgent = null)
    {
        UltimoLogin = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(ip))
            UltimoIp = ip;
        if (!string.IsNullOrEmpty(userAgent))
            UserAgent = userAgent;
        UpdateTimestamp();
    }

    public void AlterarSenha(string novaSenhaHash)
    {
        SenhaHash = novaSenhaHash;
        UpdateTimestamp();
    }

    public void Ativar()
    {
        IsAtivo = true;
        UpdateTimestamp();
    }

    public void Desativar()
    {
        IsAtivo = false;
        UpdateTimestamp();
    }
}

