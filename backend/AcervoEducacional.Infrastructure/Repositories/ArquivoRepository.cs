using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class ArquivoRepository : BaseRepository<Arquivo>, IArquivoRepository
{
    public ArquivoRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Arquivo>> GetByCursoIdAsync(Guid cursoId)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.CursoId == cursoId)
            .OrderBy(a => a.Categoria)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetByCategoriaAsync(CategoriaArquivo categoria)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.Categoria == categoria)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Arquivo?> GetByTokenCompartilhamentoAsync(string token)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .FirstOrDefaultAsync(a => a.TokenCompartilhamento == token);
    }

    public async Task<IEnumerable<Arquivo>> GetArquivosExpiradosAsync()
    {
        var agora = DateTime.UtcNow;
        return await _dbSet
            .Where(a => a.DataExpiracao.HasValue && a.DataExpiracao.Value <= agora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetArquivosPublicosAsync()
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.IsPublico)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<long> GetTamanhoTotalByCursoAsync(Guid cursoId)
    {
        return await _dbSet
            .Where(a => a.CursoId == cursoId)
            .SumAsync(a => a.Tamanho);
    }

    public async Task<Dictionary<CategoriaArquivo, int>> GetArquivosPorCategoriaAsync(Guid? cursoId = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (cursoId.HasValue)
        {
            query = query.Where(a => a.CursoId == cursoId.Value);
        }

        var result = await query
            .GroupBy(a => a.Categoria)
            .Select(g => new { Categoria = g.Key, Count = g.Count() })
            .ToListAsync();

        return result.ToDictionary(x => x.Categoria, x => x.Count);
    }

    public override async Task<Arquivo?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.LogsAtividade)
                .ThenInclude(l => l.Usuario)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public override async Task<IEnumerable<Arquivo>> GetAllAsync()
    {
        return await _dbSet
            .Include(a => a.Curso)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }
}

