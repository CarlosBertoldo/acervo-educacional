using System.ComponentModel.DataAnnotations;

namespace AcervoEducacional.Application.DTOs.Usuario;

public class UsuarioResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsAtivo { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public string? UltimoIp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UsuarioCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string Senha { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = true; // Todos começam como admin conforme especificação
}

public class UsuarioUpdateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    public bool IsAdmin { get; set; }

    public bool IsAtivo { get; set; }
}

public class UsuarioListDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsAtivo { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UsuarioFilterDto
{
    public string? Search { get; set; }
    public bool? IsAdmin { get; set; }
    public bool? IsAtivo { get; set; }
    public DateTime? CriadoApartirDe { get; set; }
    public DateTime? CriadoAte { get; set; }
    public DateTime? UltimoLoginApartirDe { get; set; }
    public DateTime? UltimoLoginAte { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

