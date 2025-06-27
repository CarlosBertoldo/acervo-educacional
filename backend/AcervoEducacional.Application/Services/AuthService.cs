using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AcervoEducacional.Application.DTOs.Auth;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISessaoUsuarioRepository _sessaoRepository;
    private readonly ITokenRecuperacaoRepository _tokenRepository;
    private readonly ILogAtividadeRepository _logRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ISessaoUsuarioRepository sessaoRepository,
        ITokenRecuperacaoRepository tokenRepository,
        ILogAtividadeRepository logRepository,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _sessaoRepository = sessaoRepository;
        _tokenRepository = tokenRepository;
        _logRepository = logRepository;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);
            if (usuario == null || !usuario.IsAtivo)
            {
                await LogAsync(TipoAcao.Login, $"Tentativa de login com email inválido: {loginDto.Email}", null, ipAddress, userAgent);
                return ApiResponse<LoginResponseDto>.ErrorResult("Credenciais inválidas");
            }

            if (!VerifyPassword(loginDto.Senha, usuario.SenhaHash))
            {
                await LogAsync(TipoAcao.Login, $"Tentativa de login com senha incorreta para: {loginDto.Email}", usuario.Id, ipAddress, userAgent);
                return ApiResponse<LoginResponseDto>.ErrorResult("Credenciais inválidas");
            }

            // Gerar tokens
            var accessToken = GenerateAccessToken(usuario);
            var refreshToken = GenerateRefreshToken();

            // Criar sessão
            var sessao = new SessaoUsuario
            {
                UsuarioId = usuario.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _sessaoRepository.AddAsync(sessao);

            // Atualizar último login
            usuario.AtualizarUltimoLogin(ipAddress, userAgent);
            await _usuarioRepository.UpdateAsync(usuario);

            await _unitOfWork.SaveChangesAsync();

            await LogAsync(TipoAcao.Login, $"Login realizado com sucesso", usuario.Id, ipAddress, userAgent);

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
                Usuario = new UsuarioLogadoDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    IsAdmin = usuario.IsAdmin,
                    UltimoLogin = usuario.UltimoLogin ?? DateTime.UtcNow
                }
            };

            return ApiResponse<LoginResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante login para email: {Email}", loginDto.Email);
            return ApiResponse<LoginResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, string? ipAddress = null)
    {
        try
        {
            var sessao = await _sessaoRepository.GetByRefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (sessao == null || !sessao.IsValido())
            {
                await LogAsync(TipoAcao.Login, "Tentativa de refresh com token inválido", null, ipAddress);
                return ApiResponse<RefreshTokenResponseDto>.ErrorResult("Token de refresh inválido");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(sessao.UsuarioId);
            if (usuario == null || !usuario.IsAtivo)
            {
                await LogAsync(TipoAcao.Login, "Tentativa de refresh para usuário inativo", sessao.UsuarioId, ipAddress);
                return ApiResponse<RefreshTokenResponseDto>.ErrorResult("Usuário inativo");
            }

            // Gerar novos tokens
            var newAccessToken = GenerateAccessToken(usuario);
            var newRefreshToken = GenerateRefreshToken();

            // Revogar sessão antiga
            sessao.Revogar();
            await _sessaoRepository.UpdateAsync(sessao);

            // Criar nova sessão
            var novaSessao = new SessaoUsuario
            {
                UsuarioId = usuario.Id,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IpAddress = ipAddress
            };

            await _sessaoRepository.AddAsync(novaSessao);
            await _unitOfWork.SaveChangesAsync();

            await LogAsync(TipoAcao.Login, "Token renovado com sucesso", usuario.Id, ipAddress);

            var response = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes())
            };

            return ApiResponse<RefreshTokenResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante refresh token");
            return ApiResponse<RefreshTokenResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
    {
        try
        {
            var sessao = await _sessaoRepository.GetByRefreshTokenAsync(refreshToken);
            if (sessao != null)
            {
                sessao.Revogar();
                await _sessaoRepository.UpdateAsync(sessao);
                await _unitOfWork.SaveChangesAsync();

                await LogAsync(TipoAcao.Logout, "Logout realizado", sessao.UsuarioId);
            }

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(forgotPasswordDto.Email);
            if (usuario == null || !usuario.IsAtivo)
            {
                // Por segurança, sempre retorna sucesso mesmo se o email não existir
                return ApiResponse<bool>.SuccessResult(true);
            }

            // Invalidar tokens anteriores
            var todosTokens = await _tokenRepository.GetAllAsync();
            var tokensAnteriores = todosTokens.Where(t => t.UsuarioId == usuario.Id && t.IsValido()).ToList();
            foreach (var token in tokensAnteriores)
            {
                token.Usar();
                await _tokenRepository.UpdateAsync(token);
            }

            // Gerar novo token
            var resetToken = GenerateSecureToken();
            var tokenRecuperacao = new TokenRecuperacao
            {
                UsuarioId = usuario.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IpAddress = null // Será definido pelo controller se necessário
            };

            await _tokenRepository.AddAsync(tokenRecuperacao);
            await _unitOfWork.SaveChangesAsync();

            // Enviar email
            await _emailService.SendPasswordResetEmailAsync(usuario.Email, resetToken, usuario.Nome);

            await LogAsync(TipoAcao.RecuperarSenha, "Solicitação de recuperação de senha", usuario.Id);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante forgot password para email: {Email}", forgotPasswordDto.Email);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var tokenRecuperacao = await _tokenRepository.GetByTokenAsync(resetPasswordDto.Token);
            if (tokenRecuperacao == null || !tokenRecuperacao.IsValido())
            {
                return ApiResponse<bool>.ErrorResult("Token de recuperação inválido ou expirado");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(tokenRecuperacao.UsuarioId);
            if (usuario == null || !usuario.IsAtivo)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado ou inativo");
            }

            // Alterar senha
            var novaSenhaHash = HashPassword(resetPasswordDto.NovaSenha);
            usuario.AlterarSenha(novaSenhaHash);
            await _usuarioRepository.UpdateAsync(usuario);

            // Marcar token como usado
            tokenRecuperacao.Usar();
            await _tokenRepository.UpdateAsync(tokenRecuperacao);

            // Revogar todas as sessões ativas (método simplificado)
            var sessoesAtivas = await _sessaoRepository.GetAllAsync();
            var sessoesUsuario = sessoesAtivas.Where(s => s.UsuarioId == usuario.Id && s.IsValido());
            foreach (var sessao in sessoesUsuario)
            {
                sessao.Revogar();
                await _sessaoRepository.UpdateAsync(sessao);
            }

            await _unitOfWork.SaveChangesAsync();

            await LogAsync(TipoAcao.AlterarSenha, "Senha alterada via recuperação", usuario.Id);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante reset password");
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, Guid usuarioId)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null || !usuario.IsAtivo)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            if (!VerifyPassword(changePasswordDto.SenhaAtual, usuario.SenhaHash))
            {
                return ApiResponse<bool>.ErrorResult("Senha atual incorreta");
            }

            var novaSenhaHash = HashPassword(changePasswordDto.NovaSenha);
            usuario.AlterarSenha(novaSenhaHash);
            await _usuarioRepository.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            await LogAsync(TipoAcao.AlterarSenha, "Senha alterada pelo usuário", usuarioId);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante change password para usuário: {UsuarioId}", usuarioId);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecretKey());

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = GetJwtAudience(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return ApiResponse<bool>.SuccessResult(true);
        }
        catch
        {
            return ApiResponse<bool>.SuccessResult(false);
        }
    }

    public async Task LimparSessoesExpiradasAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando limpeza de sessões expiradas");

            var todasSessoes = await _sessaoRepository.GetAllAsync();
            var sessoesExpiradas = todasSessoes.Where(s => !s.IsValido()).ToList();
            var count = sessoesExpiradas.Count;

            foreach (var sessao in sessoesExpiradas)
            {
                await _sessaoRepository.DeleteAsync(sessao.Id);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Limpeza de sessões expiradas concluída. {Count} sessões removidas", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante limpeza de sessões expiradas");
        }
    }

    public async Task LimparTokensExpiradosAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando limpeza de tokens de recuperação expirados");

            var todosTokens = await _tokenRepository.GetAllAsync();
            var tokensExpirados = todosTokens.Where(t => !t.IsValido()).ToList();
            var count = tokensExpirados.Count;

            foreach (var token in tokensExpirados)
            {
                await _tokenRepository.DeleteAsync(token.Id);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Limpeza de tokens expirados concluída. {Count} tokens removidos", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante limpeza de tokens expirados");
        }
    }

    #region Private Methods

    private string GenerateAccessToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(GetJwtSecretKey());
        
        var claims = new[]
        {
            new Claim("sub", usuario.Id.ToString()),
            new Claim("name", usuario.Nome),
            new Claim("email", usuario.Email),
            new Claim("admin", usuario.IsAdmin.ToString().ToLower()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience(),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private async Task LogAsync(TipoAcao acao, string descricao, Guid? usuarioId = null, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var log = new LogAtividade
            {
                UsuarioId = usuarioId,
                Acao = acao,
                Descricao = descricao,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar log de atividade");
        }
    }

    private string GetJwtSecretKey()
    {
        return _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada");
    }

    private string GetJwtIssuer()
    {
        return _configuration["JwtSettings:Issuer"] ?? "AcervoEducacional";
    }

    private string GetJwtAudience()
    {
        return _configuration["JwtSettings:Audience"] ?? "AcervoEducacionalUsers";
    }

    private int GetTokenExpirationMinutes()
    {
        return int.TryParse(_configuration["JwtSettings:ExpirationMinutes"], out var minutes) ? minutes : 60;
    }

    #endregion
}

