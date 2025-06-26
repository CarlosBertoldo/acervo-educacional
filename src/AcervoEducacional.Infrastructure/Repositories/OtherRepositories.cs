using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class LogAtividadeRepository : BaseRepository<LogAtividade>, ILogAtividadeRepository
{
    public LogAtividadeRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LogAtividade>> GetByUsuarioIdAsync(Guid usuarioId)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Include(l => l.Arquivo)
            .Where(l => l.UsuarioId == usuarioId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetByCursoIdAsync(Guid cursoId)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Include(l => l.Arquivo)
            .Where(l => l.CursoId == cursoId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetByArquivoIdAsync(Guid arquivoId)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Include(l => l.Arquivo)
            .Where(l => l.ArquivoId == arquivoId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetByAcaoAsync(TipoAcao acao)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Include(l => l.Arquivo)
            .Where(l => l.Acao == acao)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetRecentAsync(int count = 50)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Include(l => l.Arquivo)
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Include(l => l.Arquivo)
            .Where(l => l.CreatedAt >= inicio && l.CreatedAt <= fim)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }
}

public class SessaoUsuarioRepository : BaseRepository<SessaoUsuario>, ISessaoUsuarioRepository
{
    public SessaoUsuarioRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<SessaoUsuario?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
    }

    public async Task<IEnumerable<SessaoUsuario>> GetByUsuarioIdAsync(Guid usuarioId)
    {
        return await _dbSet
            .Where(s => s.UsuarioId == usuarioId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task RevogarTodasSessoesUsuarioAsync(Guid usuarioId)
    {
        var sessoes = await _dbSet
            .Where(s => s.UsuarioId == usuarioId && !s.IsRevogado)
            .ToListAsync();

        foreach (var sessao in sessoes)
        {
            sessao.Revogar();
        }
    }

    public async Task LimparSessoesExpiradasAsync()
    {
        var sessoesExpiradas = await _dbSet
            .Where(s => s.ExpiresAt <= DateTime.UtcNow || s.IsRevogado)
            .ToListAsync();

        _dbSet.RemoveRange(sessoesExpiradas);
    }
}

public class TokenRecuperacaoRepository : BaseRepository<TokenRecuperacao>, ITokenRecuperacaoRepository
{
    public TokenRecuperacaoRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<TokenRecuperacao?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<IEnumerable<TokenRecuperacao>> GetByUsuarioIdAsync(Guid usuarioId)
    {
        return await _dbSet
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task InvalidarTokensUsuarioAsync(Guid usuarioId)
    {
        var tokens = await _dbSet
            .Where(t => t.UsuarioId == usuarioId && !t.IsUsado)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Usar();
        }
    }

    public async Task LimparTokensExpiradosAsync()
    {
        var tokensExpirados = await _dbSet
            .Where(t => t.ExpiresAt <= DateTime.UtcNow || t.IsUsado)
            .ToListAsync();

        _dbSet.RemoveRange(tokensExpirados);
    }
}

public class ConfiguracaoSistemaRepository : BaseRepository<ConfiguracaoSistema>, IConfiguracaoSistemaRepository
{
    public ConfiguracaoSistemaRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<ConfiguracaoSistema?> GetByChaveAsync(string chave)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Chave == chave);
    }

    public async Task<IEnumerable<ConfiguracaoSistema>> GetByCategoriaAsync(string categoria)
    {
        return await _dbSet
            .Where(c => c.Categoria == categoria)
            .OrderBy(c => c.Chave)
            .ToListAsync();
    }

    public async Task<string?> GetValorAsync(string chave)
    {
        var config = await GetByChaveAsync(chave);
        return config?.Valor;
    }

    public async Task SetValorAsync(string chave, string valor, string? descricao = null, string? categoria = null)
    {
        var config = await GetByChaveAsync(chave);
        
        if (config == null)
        {
            config = new ConfiguracaoSistema
            {
                Chave = chave,
                Valor = valor,
                Descricao = descricao,
                Categoria = categoria
            };
            await _dbSet.AddAsync(config);
        }
        else
        {
            config.Valor = valor;
            if (!string.IsNullOrEmpty(descricao))
                config.Descricao = descricao;
            if (!string.IsNullOrEmpty(categoria))
                config.Categoria = categoria;
            config.UpdateTimestamp();
        }
    }
}

public class SincronizacaoSeniorRepository : BaseRepository<SincronizacaoSenior>, ISincronizacaoSeniorRepository
{
    public SincronizacaoSeniorRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<SincronizacaoSenior?> GetUltimaSincronizacaoAsync()
    {
        return await _dbSet
            .OrderByDescending(s => s.IniciadaEm)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SincronizacaoSenior>> GetHistoricoAsync(int count = 10)
    {
        return await _dbSet
            .OrderByDescending(s => s.IniciadaEm)
            .Take(count)
            .ToListAsync();
    }

    public async Task<SincronizacaoSenior?> GetSincronizacaoEmAndamentoAsync()
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Status == StatusSincronizacao.EmAndamento);
    }
}

public class ConflitoCursoRepository : BaseRepository<ConflitoCurso>, IConflitoCursoRepository
{
    public ConflitoCursoRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ConflitoCurso>> GetConflitosNaoResolvidosAsync()
    {
        return await _dbSet
            .Include(c => c.Curso)
            .Include(c => c.ResolvidoPor)
            .Where(c => !c.IsResolvido)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ConflitoCurso>> GetByCursoIdAsync(Guid cursoId)
    {
        return await _dbSet
            .Include(c => c.Curso)
            .Include(c => c.ResolvidoPor)
            .Where(c => c.CursoId == cursoId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ConflitoCurso>> GetByTipoConflitoAsync(TipoConflito tipo)
    {
        return await _dbSet
            .Include(c => c.Curso)
            .Include(c => c.ResolvidoPor)
            .Where(c => c.TipoConflito == tipo)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task ResolverConflitosEmLoteAsync(IEnumerable<Guid> conflitosIds, ResolucaoConflito resolucao, Guid usuarioId)
    {
        var conflitos = await _dbSet
            .Where(c => conflitosIds.Contains(c.Id) && !c.IsResolvido)
            .ToListAsync();

        foreach (var conflito in conflitos)
        {
            conflito.Resolver(resolucao, usuarioId);
        }
    }
}

