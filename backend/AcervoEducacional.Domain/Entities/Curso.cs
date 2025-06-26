using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class Curso : BaseEntity
{
    public string DescricaoAcademia { get; set; } = string.Empty;
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public StatusCurso Status { get; set; }
    public TipoAmbiente TipoAmbiente { get; set; }
    public TipoAcesso TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }
    public OrigemCurso Origem { get; set; }
    public Guid? CriadoPorId { get; set; }
    public string? ComentariosInternos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
    public string? CodigoSenior { get; set; } // Para cursos vindos do Senior
    public DateTime? UltimaSincronizacaoSenior { get; set; }

    // Relacionamentos
    public virtual Usuario? CriadoPor { get; set; }
    public virtual ICollection<Arquivo> Arquivos { get; set; } = new List<Arquivo>();
    public virtual ICollection<LogAtividade> LogsAtividade { get; set; } = new List<LogAtividade>();

    public void MoverPara(StatusCurso novoStatus, Guid usuarioId, string? comentario = null)
    {
        var statusAnterior = Status;
        Status = novoStatus;
        UpdateTimestamp();

        // Log da movimentação será criado pelo serviço
    }

    public void AtualizarDadosSenior(string codigoSenior)
    {
        CodigoSenior = codigoSenior;
        UltimaSincronizacaoSenior = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool PodeSerEditado()
    {
        return Origem == OrigemCurso.Manual;
    }

    public bool PrecisaSincronizacao()
    {
        return Origem == OrigemCurso.Senior && 
               (UltimaSincronizacaoSenior == null || 
                UltimaSincronizacaoSenior < DateTime.UtcNow.AddHours(-24));
    }

    public int TotalArquivos => Arquivos?.Count ?? 0;
    
    public long TamanhoTotalArquivos => Arquivos?.Sum(a => a.Tamanho) ?? 0;
}

