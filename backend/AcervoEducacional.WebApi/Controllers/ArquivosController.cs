using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcervoEducacional.Application.DTOs.Arquivo;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ArquivosController : ControllerBase
{
    private readonly IArquivoService _arquivoService;
    private readonly ILogger<ArquivosController> _logger;

    public ArquivosController(IArquivoService arquivoService, ILogger<ArquivosController> logger)
    {
        _arquivoService = arquivoService;
        _logger = logger;
    }

    /// <summary>
    /// Obter lista de arquivos com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ArquivoListDto>>>> GetArquivos([FromQuery] ArquivoFilterDto filter)
    {
        var result = await _arquivoService.GetArquivosAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Obter arquivo por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ArquivoResponseDto>>> GetArquivo(Guid id)
    {
        var result = await _arquivoService.GetArquivoByIdAsync(id);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Obter arquivos de um curso agrupados por categoria
    /// </summary>
    [HttpGet("curso/{cursoId:guid}")]
    public async Task<ActionResult<ApiResponse<List<ArquivosPorCategoriaDto>>>> GetArquivosByCurso(Guid cursoId)
    {
        var result = await _arquivoService.GetArquivosByCursoAsync(cursoId);
        return Ok(result);
    }

    /// <summary>
    /// Upload de arquivo
    /// </summary>
    [HttpPost("curso/{cursoId:guid}/upload")]
    public async Task<ActionResult<ApiResponse<ArquivoResponseDto>>> UploadArquivo(
        Guid cursoId,
        [FromForm] ArquivoUploadDto uploadDto)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _arquivoService.UploadArquivoAsync(cursoId, uploadDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Arquivo {Nome} enviado com sucesso para curso {CursoId} por {UsuarioId}", 
                uploadDto.Arquivo.FileName, cursoId, usuarioId);
            return CreatedAtAction(nameof(GetArquivo), new { id = result.Data!.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Atualizar informações do arquivo
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ArquivoResponseDto>>> UpdateArquivo(Guid id, [FromBody] ArquivoUpdateDto updateDto)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _arquivoService.UpdateArquivoAsync(id, updateDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Arquivo {Id} atualizado com sucesso por {UsuarioId}", id, usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Excluir arquivo
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteArquivo(Guid id)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _arquivoService.DeleteArquivoAsync(id, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Arquivo {Id} excluído com sucesso por {UsuarioId}", id, usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Obter URL de download do arquivo
    /// </summary>
    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult<ApiResponse<DownloadArquivoResponseDto>>> GetDownloadUrl(Guid id)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _arquivoService.GetDownloadUrlAsync(id, usuarioId);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Compartilhar arquivo
    /// </summary>
    [HttpPost("{id:guid}/share")]
    public async Task<ActionResult<ApiResponse<ShareArquivoResponseDto>>> ShareArquivo(Guid id, [FromBody] ShareArquivoDto shareDto)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _arquivoService.ShareArquivoAsync(id, shareDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Arquivo {Id} compartilhado com sucesso por {UsuarioId}", id, usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Acessar arquivo compartilhado via token (público)
    /// </summary>
    [HttpGet("shared/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ArquivoResponseDto>>> GetSharedArquivo(string token)
    {
        var result = await _arquivoService.GetSharedArquivoAsync(token);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Gerar código embed para arquivo
    /// </summary>
    [HttpGet("{id:guid}/embed")]
    public async Task<ActionResult<string>> GetEmbedCode(Guid id, [FromQuery] string? domain = null)
    {
        var arquivo = await _arquivoService.GetArquivoByIdAsync(id);
        
        if (!arquivo.Success || arquivo.Data == null)
        {
            return NotFound();
        }

        var embedCode = GenerateEmbedCode(arquivo.Data, domain);
        return Ok(embedCode);
    }

    /// <summary>
    /// Visualizar arquivo (para embed)
    /// </summary>
    [HttpGet("{id:guid}/view")]
    [AllowAnonymous]
    public async Task<ActionResult> ViewArquivo(Guid id, [FromQuery] string? domain = null)
    {
        var arquivo = await _arquivoService.GetArquivoByIdAsync(id);
        
        if (!arquivo.Success || arquivo.Data == null)
        {
            return NotFound();
        }

        // Verificar domínio se especificado
        if (!string.IsNullOrEmpty(domain) && arquivo.Data.DominiosPermitidos != null)
        {
            var referer = Request.Headers["Referer"].ToString();
            var isAllowed = arquivo.Data.DominiosPermitidos.Any(d => 
                referer.Contains(d, StringComparison.OrdinalIgnoreCase));
            
            if (!isAllowed)
            {
                return Forbid("Domínio não autorizado");
            }
        }

        // Redirecionar para URL de visualização
        if (!string.IsNullOrEmpty(arquivo.Data.UrlVisualizacao))
        {
            return Redirect(arquivo.Data.UrlVisualizacao);
        }

        return NotFound();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Usuário não identificado");
        }
        return userId;
    }

    private string GenerateEmbedCode(ArquivoResponseDto arquivo, string? domain = null)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var viewUrl = $"{baseUrl}/api/v1/arquivos/{arquivo.Id}/view";
        
        if (!string.IsNullOrEmpty(domain))
        {
            viewUrl += $"?domain={Uri.EscapeDataString(domain)}";
        }

        var embedCode = arquivo.TipoMime.StartsWith("video/") 
            ? GenerateVideoEmbed(viewUrl, arquivo)
            : arquivo.TipoMime.StartsWith("audio/")
                ? GenerateAudioEmbed(viewUrl, arquivo)
                : GenerateDocumentEmbed(viewUrl, arquivo);

        return embedCode;
    }

    private static string GenerateVideoEmbed(string url, ArquivoResponseDto arquivo)
    {
        var bloqueios = arquivo.BloqueiosAtivos ?? new Dictionary<string, object>();
        var allowSeek = !bloqueios.ContainsKey("blockSeek") || !(bool)bloqueios["blockSeek"];
        var allowDownload = !bloqueios.ContainsKey("blockDownload") || !(bool)bloqueios["blockDownload"];

        return $@"
<div style='position: relative; width: 100%; max-width: 800px;'>
    <video 
        width='100%' 
        height='auto' 
        controls='{(allowSeek ? "true" : "false")}'
        controlsList='{(allowDownload ? "" : "nodownload")}'
        oncontextmenu='return false;'
        style='outline: none;'>
        <source src='{url}' type='{arquivo.TipoMime}'>
        Seu navegador não suporta o elemento de vídeo.
    </video>
    <div style='position: absolute; top: 0; left: 0; width: 100%; height: 100%; pointer-events: none; background: transparent;'></div>
</div>";
    }

    private static string GenerateAudioEmbed(string url, ArquivoResponseDto arquivo)
    {
        var bloqueios = arquivo.BloqueiosAtivos ?? new Dictionary<string, object>();
        var allowDownload = !bloqueios.ContainsKey("blockDownload") || !(bool)bloqueios["blockDownload"];

        return $@"
<audio 
    controls 
    style='width: 100%; max-width: 400px;'
    controlsList='{(allowDownload ? "" : "nodownload")}'
    oncontextmenu='return false;'>
    <source src='{url}' type='{arquivo.TipoMime}'>
    Seu navegador não suporta o elemento de áudio.
</audio>";
    }

    private static string GenerateDocumentEmbed(string url, ArquivoResponseDto arquivo)
    {
        return $@"
<iframe 
    src='{url}' 
    width='100%' 
    height='600px' 
    style='border: none; outline: none;'
    oncontextmenu='return false;'>
    <p>Seu navegador não suporta iframes. <a href='{url}' target='_blank'>Clique aqui para visualizar o documento</a>.</p>
</iframe>";
    }
}

