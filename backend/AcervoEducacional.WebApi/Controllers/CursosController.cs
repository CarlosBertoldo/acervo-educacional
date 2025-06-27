using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcervoEducacional.Application.DTOs.Curso;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/cursos")]
[Authorize]
public class CursosController : ControllerBase
{
    private readonly ICursoService _cursoService;
    private readonly ILogger<CursosController> _logger;

    public CursosController(ICursoService cursoService, ILogger<CursosController> logger)
    {
        _cursoService = cursoService;
        _logger = logger;
    }

    /// <summary>
    /// Obter lista de cursos com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CursoListDto>>>> GetCursos([FromQuery] CursoFilterDto filter)
    {
        var result = await _cursoService.GetCursosAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Obter curso por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CursoResponseDto>>> GetCurso(Guid id)
    {
        var result = await _cursoService.GetCursoByIdAsync(id);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Criar novo curso
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CursoResponseDto>>> CreateCurso([FromBody] CursoCreateDto createDto)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _cursoService.CreateCursoAsync(createDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Curso {CodigoCurso} criado com sucesso por {UsuarioId}", createDto.CodigoCurso, usuarioId);
            return CreatedAtAction(nameof(GetCurso), new { id = result.Data!.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Atualizar curso existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CursoResponseDto>>> UpdateCurso(Guid id, [FromBody] CursoUpdateDto updateDto)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _cursoService.UpdateCursoAsync(id, updateDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Curso {Id} atualizado com sucesso por {UsuarioId}", id, usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Excluir curso
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCurso(Guid id)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _cursoService.DeleteCursoAsync(id, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Curso {Id} excluído com sucesso por {UsuarioId}", id, usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Mover curso para outro status (Kanban)
    /// </summary>
    [HttpPatch("{id:guid}/move")]
    public async Task<ActionResult<ApiResponse<CursoResponseDto>>> MoveCurso(Guid id, [FromBody] MoveCursoDto moveDto)
    {
        var usuarioId = GetCurrentUserId();
        var result = await _cursoService.MoveCursoAsync(id, moveDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Curso {Id} movido para {NovoStatus} por {UsuarioId}", id, moveDto.NovoStatus, usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Obter dados do Kanban (cursos agrupados por status)
    /// </summary>
    [HttpGet("kanban")]
    public async Task<ActionResult<ApiResponse<List<CursoKanbanDto>>>> GetKanban()
    {
        var result = await _cursoService.GetKanbanAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obter estatísticas dos cursos
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<CursoStatisticsDto>>> GetStatistics()
    {
        var result = await _cursoService.GetStatisticsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obter cursos para dashboard
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<object>>> GetDashboard()
    {
        var kanbanResult = await _cursoService.GetKanbanAsync();
        var statsResult = await _cursoService.GetStatisticsAsync();

        if (kanbanResult.Success && statsResult.Success)
        {
            var dashboard = new
            {
                Kanban = kanbanResult.Data,
                Statistics = statsResult.Data
            };

            return Ok(ApiResponse<object>.SuccessResult(dashboard));
        }

        return BadRequest(ApiResponse<object>.ErrorResult("Erro ao carregar dados do dashboard"));
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
}

