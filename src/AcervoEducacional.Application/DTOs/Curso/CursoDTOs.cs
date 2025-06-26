using System.ComponentModel.DataAnnotations;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.DTOs.Curso;

public class CursoResponseDto
{
    public Guid Id { get; set; }
    public string DescricaoAcademia { get; set; } = string.Empty;
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public StatusCurso Status { get; set; }
    public TipoAmbiente TipoAmbiente { get; set; }
    public TipoAcesso TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }
    public OrigemCurso Origem { get; set; }
    public string? CriadoPor { get; set; }
    public string? ComentariosInternos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TotalArquivos { get; set; }
    public long TamanhoTotalArquivos { get; set; }
    public string TamanhoTotalFormatado { get; set; } = string.Empty;
}

public class CursoCreateDto
{
    [Required(ErrorMessage = "Descrição da Academia é obrigatória")]
    [MaxLength(200, ErrorMessage = "Descrição da Academia deve ter no máximo 200 caracteres")]
    public string DescricaoAcademia { get; set; } = string.Empty;

    [Required(ErrorMessage = "Código do Curso é obrigatório")]
    [MaxLength(50, ErrorMessage = "Código do Curso deve ter no máximo 50 caracteres")]
    public string CodigoCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome do Curso é obrigatório")]
    [MaxLength(300, ErrorMessage = "Nome do Curso deve ter no máximo 300 caracteres")]
    public string NomeCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status é obrigatório")]
    public StatusCurso Status { get; set; }

    [Required(ErrorMessage = "Tipo de Ambiente é obrigatório")]
    public TipoAmbiente TipoAmbiente { get; set; }

    [Required(ErrorMessage = "Tipo de Acesso é obrigatório")]
    public TipoAcesso TipoAcesso { get; set; }

    public DateTime? DataInicioOperacao { get; set; }

    public OrigemCurso Origem { get; set; } = OrigemCurso.Manual;

    public string? ComentariosInternos { get; set; }

    public Dictionary<string, object>? Metadados { get; set; }
}

public class CursoUpdateDto
{
    [Required(ErrorMessage = "Descrição da Academia é obrigatória")]
    [MaxLength(200, ErrorMessage = "Descrição da Academia deve ter no máximo 200 caracteres")]
    public string DescricaoAcademia { get; set; } = string.Empty;

    [Required(ErrorMessage = "Código do Curso é obrigatório")]
    [MaxLength(50, ErrorMessage = "Código do Curso deve ter no máximo 50 caracteres")]
    public string CodigoCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome do Curso é obrigatório")]
    [MaxLength(300, ErrorMessage = "Nome do Curso deve ter no máximo 300 caracteres")]
    public string NomeCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status é obrigatório")]
    public StatusCurso Status { get; set; }

    [Required(ErrorMessage = "Tipo de Ambiente é obrigatório")]
    public TipoAmbiente TipoAmbiente { get; set; }

    [Required(ErrorMessage = "Tipo de Acesso é obrigatório")]
    public TipoAcesso TipoAcesso { get; set; }

    public DateTime? DataInicioOperacao { get; set; }

    public string? ComentariosInternos { get; set; }

    public Dictionary<string, object>? Metadados { get; set; }
}

public class CursoListDto
{
    public Guid Id { get; set; }
    public string DescricaoAcademia { get; set; } = string.Empty;
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public StatusCurso Status { get; set; }
    public TipoAmbiente TipoAmbiente { get; set; }
    public TipoAcesso TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }
    public OrigemCurso Origem { get; set; }
    public string? CriadoPor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TotalArquivos { get; set; }
}

public class CursoKanbanDto
{
    public StatusCurso Status { get; set; }
    public List<CursoListDto> Cursos { get; set; } = new();
    public int Total { get; set; }
}

public class CursoFilterDto
{
    public string? Search { get; set; }
    public StatusCurso? Status { get; set; }
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public OrigemCurso? Origem { get; set; }
    public string? CriadoPor { get; set; }
    public DateTime? CriadoApartirDe { get; set; }
    public DateTime? CriadoAte { get; set; }
    public DateTime? InicioOperacaoApartirDe { get; set; }
    public DateTime? InicioOperacaoAte { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

public class MoveCursoDto
{
    [Required(ErrorMessage = "Status de destino é obrigatório")]
    public StatusCurso NovoStatus { get; set; }

    public string? Comentario { get; set; }
}

public class CursoStatisticsDto
{
    public int TotalCursos { get; set; }
    public int CursosBacklog { get; set; }
    public int CursosEmDesenvolvimento { get; set; }
    public int CursosVeiculados { get; set; }
    public int CursosManuais { get; set; }
    public int CursosSenior { get; set; }
    public int TotalArquivos { get; set; }
    public long TamanhoTotalArquivos { get; set; }
    public string TamanhoTotalFormatado { get; set; } = string.Empty;
}

