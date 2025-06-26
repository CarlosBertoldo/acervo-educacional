using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcervoEducacional.Application.DTOs.Auth;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Realizar login no sistema
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var result = await _authService.LoginAsync(loginDto, ipAddress, userAgent);
        
        if (result.Success)
        {
            _logger.LogInformation("Login realizado com sucesso para {Email}", loginDto.Email);
            return Ok(result);
        }

        _logger.LogWarning("Tentativa de login falhada para {Email}", loginDto.Email);
        return BadRequest(result);
    }

    /// <summary>
    /// Renovar token de acesso
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        var result = await _authService.RefreshTokenAsync(refreshTokenDto, ipAddress);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Realizar logout do sistema
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var result = await _authService.LogoutAsync(refreshTokenDto.RefreshToken);
        
        if (result.Success)
        {
            _logger.LogInformation("Logout realizado com sucesso");
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Solicitar recuperação de senha
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
        
        // Sempre retorna sucesso por segurança, mesmo se o email não existir
        return Ok(ApiResponse<bool>.SuccessResult(true, "Se o email existir, você receberá instruções para recuperação de senha"));
    }

    /// <summary>
    /// Redefinir senha com token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);
        
        if (result.Success)
        {
            _logger.LogInformation("Senha redefinida com sucesso");
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Alterar senha do usuário logado
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var usuarioId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());
        
        var result = await _authService.ChangePasswordAsync(changePasswordDto, usuarioId);
        
        if (result.Success)
        {
            _logger.LogInformation("Senha alterada com sucesso para usuário {UsuarioId}", usuarioId);
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Validar token de acesso
    /// </summary>
    [HttpPost("validate")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateToken()
    {
        var token = HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        var result = await _authService.ValidateTokenAsync(token);
        
        return Ok(result);
    }

    /// <summary>
    /// Obter informações do usuário logado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<object>> GetCurrentUser()
    {
        var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
        
        var userInfo = new
        {
            Id = claims.GetValueOrDefault("sub"),
            Nome = claims.GetValueOrDefault("name"),
            Email = claims.GetValueOrDefault("email"),
            IsAdmin = bool.Parse(claims.GetValueOrDefault("admin", "false"))
        };

        return Ok(ApiResponse<object>.SuccessResult(userInfo));
    }
}

