using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.DTOs.Arquivo;

public class ArquivoResponseDto
{
    public Guid Id { get; set; }
    public Guid CursoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public CategoriaArquivo Categoria { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string TamanhoFormatado { get; set; } = string.Empty;
    public bool IsPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UrlDownload { get; set; }
    public string? UrlVisualizacao { get; set; }
}

public class ArquivoUploadDto
{
    [Required(ErrorMessage = "Arquivo é obrigatório")]
    public IFormFile Arquivo { get; set; } = null!;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public CategoriaArquivo Categoria { get; set; }

    public bool IsPublico { get; set; } = false;

    public DateTime? DataExpiracao { get; set; }

    public string[]? DominiosPermitidos { get; set; }

    public Dictionary<string, object>? BloqueiosAtivos { get; set; }

    public Dictionary<string, object>? Metadados { get; set; }
}

public class ArquivoUpdateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(500, ErrorMessage = "Nome deve ter no máximo 500 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public CategoriaArquivo Categoria { get; set; }

    public bool IsPublico { get; set; }

    public DateTime? DataExpiracao { get; set; }

    public string[]? DominiosPermitidos { get; set; }

    public Dictionary<string, object>? BloqueiosAtivos { get; set; }

    public Dictionary<string, object>? Metadados { get; set; }
}

public class ArquivoListDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public CategoriaArquivo Categoria { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string TamanhoFormatado { get; set; } = string.Empty;
    public bool IsPublico { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ArquivosPorCategoriaDto
{
    public CategoriaArquivo Categoria { get; set; }
    public List<ArquivoListDto> Arquivos { get; set; } = new();
    public int Total { get; set; }
    public long TamanhoTotal { get; set; }
    public string TamanhoTotalFormatado { get; set; } = string.Empty;
}

public class ShareArquivoDto
{
    public bool IsPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public bool GerarCodigoEmbed { get; set; } = false;
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
}

public class ShareArquivoResponseDto
{
    public string ShareUrl { get; set; } = string.Empty;
    public string? EmbedCode { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsPublico { get; set; }
    public string[]? DominiosPermitidos { get; set; }
}

public class DownloadArquivoResponseDto
{
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Nome { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string TipoMime { get; set; } = string.Empty;
}

public class ArquivoFilterDto
{
    public Guid? CursoId { get; set; }
    public CategoriaArquivo? Categoria { get; set; }
    public string? Search { get; set; }
    public string? TipoMime { get; set; }
    public bool? IsPublico { get; set; }
    public DateTime? CriadoApartirDe { get; set; }
    public DateTime? CriadoAte { get; set; }
    public long? TamanhoMin { get; set; }
    public long? TamanhoMax { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

