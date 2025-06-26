using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class Arquivo : BaseEntity
{
    public Guid CursoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeOriginal { get; set; } = string.Empty;
    public CategoriaArquivo Categoria { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string CaminhoS3 { get; set; } = string.Empty;
    public string? UrlPublica { get; set; }
    public bool IsPublico { get; set; } = false;
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
    public string? TokenCompartilhamento { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public int DownloadCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;

    // Relacionamentos
    public virtual Curso Curso { get; set; } = null!;
    public virtual ICollection<LogAtividade> LogsAtividade { get; set; } = new List<LogAtividade>();

    public string TamanhoFormatado => FormatFileSize(Tamanho);

    public bool IsVideo => TipoMime.StartsWith("video/");
    public bool IsAudio => TipoMime.StartsWith("audio/");
    public bool IsImage => TipoMime.StartsWith("image/");
    public bool IsDocument => TipoMime.Contains("pdf") || 
                             TipoMime.Contains("word") || 
                             TipoMime.Contains("powerpoint") ||
                             TipoMime.Contains("presentation");

    public void GerarTokenCompartilhamento(DateTime? expiresAt = null)
    {
        TokenCompartilhamento = Guid.NewGuid().ToString("N");
        TokenExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(30);
        UpdateTimestamp();
    }

    public void RevogarTokenCompartilhamento()
    {
        TokenCompartilhamento = null;
        TokenExpiresAt = null;
        UpdateTimestamp();
    }

    public bool TokenValido()
    {
        return !string.IsNullOrEmpty(TokenCompartilhamento) &&
               (TokenExpiresAt == null || TokenExpiresAt > DateTime.UtcNow);
    }

    public void IncrementarDownload()
    {
        DownloadCount++;
        UpdateTimestamp();
    }

    public void IncrementarVisualizacao()
    {
        ViewCount++;
        UpdateTimestamp();
    }

    public void AtualizarPermissoes(bool isPublico, DateTime? dataExpiracao = null, string[]? dominiosPermitidos = null)
    {
        IsPublico = isPublico;
        DataExpiracao = dataExpiracao;
        DominiosPermitidos = dominiosPermitidos;
        UpdateTimestamp();
    }

    public void AtualizarBloqueios(Dictionary<string, object>? bloqueios)
    {
        BloqueiosAtivos = bloqueios;
        UpdateTimestamp();
    }

    public bool EstaExpirado()
    {
        return DataExpiracao.HasValue && DataExpiracao.Value <= DateTime.UtcNow;
    }

    public bool DominioPermitido(string dominio)
    {
        return DominiosPermitidos == null || 
               DominiosPermitidos.Length == 0 || 
               DominiosPermitidos.Contains(dominio, StringComparer.OrdinalIgnoreCase);
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

