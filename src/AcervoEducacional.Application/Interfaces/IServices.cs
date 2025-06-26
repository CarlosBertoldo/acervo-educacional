using AcervoEducacional.Application.DTOs.Auth;
using AcervoEducacional.Application.DTOs.Usuario;
using AcervoEducacional.Application.DTOs.Curso;
using AcervoEducacional.Application.DTOs.Arquivo;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AcervoEducacional.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto, string? ipAddress = null, string? userAgent = null);
    Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, string? ipAddress = null);
    Task<ApiResponse<bool>> LogoutAsync(string refreshToken);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, Guid usuarioId);
    Task<ApiResponse<bool>> ValidateTokenAsync(string token);
    Task LimparSessoesExpiradasAsync();
    Task LimparTokensExpiradosAsync();
}

public interface IUsuarioService
{
    Task<ApiResponse<PagedResult<UsuarioListDto>>> GetUsuariosAsync(UsuarioFilterDto filter);
    Task<ApiResponse<UsuarioResponseDto>> GetUsuarioByIdAsync(Guid id);
    Task<ApiResponse<UsuarioResponseDto>> CreateUsuarioAsync(UsuarioCreateDto createDto);
    Task<ApiResponse<UsuarioResponseDto>> UpdateUsuarioAsync(Guid id, UsuarioUpdateDto updateDto);
    Task<ApiResponse<bool>> DeleteUsuarioAsync(Guid id);
    Task<ApiResponse<bool>> ToggleUsuarioStatusAsync(Guid id);
}

public interface ICursoService
{
    Task<ApiResponse<PagedResult<CursoListDto>>> GetCursosAsync(CursoFilterDto filter);
    Task<ApiResponse<CursoResponseDto>> GetCursoByIdAsync(Guid id);
    Task<ApiResponse<CursoResponseDto>> CreateCursoAsync(CursoCreateDto createDto, Guid usuarioId);
    Task<ApiResponse<CursoResponseDto>> UpdateCursoAsync(Guid id, CursoUpdateDto updateDto, Guid usuarioId);
    Task<ApiResponse<bool>> DeleteCursoAsync(Guid id, Guid usuarioId);
    Task<ApiResponse<CursoResponseDto>> MoveCursoAsync(Guid id, MoveCursoDto moveDto, Guid usuarioId);
    Task<ApiResponse<List<CursoKanbanDto>>> GetKanbanAsync();
    Task<ApiResponse<CursoStatisticsDto>> GetStatisticsAsync();
}

public interface IArquivoService
{
    Task<ApiResponse<PagedResult<ArquivoListDto>>> GetArquivosAsync(ArquivoFilterDto filter);
    Task<ApiResponse<ArquivoResponseDto>> GetArquivoByIdAsync(Guid id);
    Task<ApiResponse<ArquivoResponseDto>> UploadArquivoAsync(Guid cursoId, ArquivoUploadDto uploadDto, Guid usuarioId);
    Task<ApiResponse<ArquivoResponseDto>> UpdateArquivoAsync(Guid id, ArquivoUpdateDto updateDto, Guid usuarioId);
    Task<ApiResponse<bool>> DeleteArquivoAsync(Guid id, Guid usuarioId);
    Task<ApiResponse<DownloadArquivoResponseDto>> GetDownloadUrlAsync(Guid id, Guid usuarioId);
    Task<ApiResponse<ShareArquivoResponseDto>> ShareArquivoAsync(Guid id, ShareArquivoDto shareDto, Guid usuarioId);
    Task<ApiResponse<ArquivoResponseDto>> GetSharedArquivoAsync(string token);
    Task<ApiResponse<List<ArquivosPorCategoriaDto>>> GetArquivosByCursoAsync(Guid cursoId);
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string userName);
    Task<bool> SendWelcomeEmailAsync(string to, string userName, string temporaryPassword);
    Task<bool> SendNotificationEmailAsync(string to, string subject, string message);
}

public interface IAwsS3Service
{
    Task<string> UploadFileAsync(IFormFile file, string bucketName, string keyName);
    Task<string> GetPresignedUrlAsync(string bucketName, string keyName, TimeSpan expiration);
    Task<bool> DeleteFileAsync(string bucketName, string keyName);
    Task<bool> FileExistsAsync(string bucketName, string keyName);
    Task<long> GetFileSizeAsync(string bucketName, string keyName);
    Task<string> CopyFileAsync(string sourceBucket, string sourceKey, string destBucket, string destKey);
}

public interface ISeniorIntegrationService
{
    Task<ApiResponse<bool>> TestConnectionAsync();
    Task<ApiResponse<List<SeniorCursoDto>>> GetCursosSeniorAsync(SeniorFilterDto filter);
    Task<ApiResponse<SeniorCursoDto>> GetCursoSeniorByCodigoAsync(string codigo);
    Task<ApiResponse<bool>> SincronizarCursosAsync(bool forceUpdate = false);
    Task<ApiResponse<List<ConflitoCursoDto>>> GetConflitosAsync();
    Task<ApiResponse<bool>> ResolverConflitoAsync(Guid conflitoId, ResolucaoConflito resolucao, Guid usuarioId);
    Task<ApiResponse<SincronizacaoStatusDto>> GetStatusSincronizacaoAsync();
    Task<ApiResponse<bool>> AgendarSincronizacaoAsync(string cronExpression);
    Task<ApiResponse<bool>> CancelarSincronizacaoAsync();
}

public interface IReportService
{
    Task<ApiResponse<byte[]>> GerarRelatorioCursosAsync(RelatorioCursosFilterDto filter, string formato);
    Task<ApiResponse<byte[]>> GerarRelatorioAtividadesAsync(RelatorioAtividadesFilterDto filter, string formato);
    Task<ApiResponse<byte[]>> GerarRelatorioArquivosAsync(RelatorioArquivosFilterDto filter, string formato);
    Task<ApiResponse<byte[]>> GerarRelatorioSeniorAsync(RelatorioSeniorFilterDto filter, string formato);
    Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
    Task<ApiResponse<List<AtividadeRecenteDto>>> GetAtividadesRecentesAsync(int count = 10);
}

public interface ILogService
{
    Task LogAsync(TipoAcao acao, string descricao, Guid? usuarioId = null, Guid? cursoId = null, Guid? arquivoId = null, 
                  string? ipAddress = null, string? userAgent = null, Dictionary<string, object>? dadosAdicionais = null);
    Task<ApiResponse<PagedResult<LogAtividadeDto>>> GetLogsAsync(LogFilterDto filter);
    Task<ApiResponse<List<LogAtividadeDto>>> GetLogsByCursoAsync(Guid cursoId, int count = 50);
    Task<ApiResponse<List<LogAtividadeDto>>> GetLogsByUsuarioAsync(Guid usuarioId, int count = 50);
}

// DTOs adicionais necessários para os serviços
public class SeniorCursoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    // Outros campos conforme estrutura do Senior
}

public class SeniorFilterDto
{
    public string? Search { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}

public class ConflitoCursoDto
{
    public Guid Id { get; set; }
    public string CodigoSenior { get; set; } = string.Empty;
    public string TipoConflito { get; set; } = string.Empty;
    public Dictionary<string, object>? DadosLocal { get; set; }
    public Dictionary<string, object>? DadosSenior { get; set; }
}

public class SincronizacaoStatusDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime? UltimaSincronizacao { get; set; }
    public DateTime? ProximaSincronizacao { get; set; }
    public int TotalCursos { get; set; }
    public int CursosProcessados { get; set; }
}

public class RelatorioCursosFilterDto : CursoFilterDto { }
public class RelatorioAtividadesFilterDto : LogFilterDto { }
public class RelatorioArquivosFilterDto : ArquivoFilterDto { }
public class RelatorioSeniorFilterDto : SeniorFilterDto { }

public class DashboardStatsDto
{
    public int TotalCursos { get; set; }
    public int TotalUsuarios { get; set; }
    public int TotalArquivos { get; set; }
    public long TamanhoTotalArquivos { get; set; }
    public Dictionary<string, int> CursosPorStatus { get; set; } = new();
    public Dictionary<string, int> ArquivosPorCategoria { get; set; } = new();
}

public class AtividadeRecenteDto
{
    public string Acao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Usuario { get; set; }
    public DateTime DataHora { get; set; }
}

public class LogAtividadeDto
{
    public Guid Id { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Usuario { get; set; }
    public string? Curso { get; set; }
    public string? Arquivo { get; set; }
    public string? IpAddress { get; set; }
    public DateTime DataHora { get; set; }
}

public class LogFilterDto
{
    public Guid? UsuarioId { get; set; }
    public Guid? CursoId { get; set; }
    public Guid? ArquivoId { get; set; }
    public string? Acao { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

