using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class CursoRepository : BaseRepository<Curso>, ICursoRepository
{
    public CursoRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<Curso?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .FirstOrDefaultAsync(c => c.CodigoCurso == codigo);
    }

    public async Task<Curso?> GetByCodigoSeniorAsync(string codigoSenior)
    {
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .FirstOrDefaultAsync(c => c.CodigoSenior == codigoSenior);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, Guid? excludeId = null)
    {
        var query = _dbSet.Where(c => c.CodigoCurso == codigo);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Curso>> GetByStatusAsync(StatusCurso status)
    {
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .Where(c => c.Status == status)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Curso>> GetByOrigemAsync(OrigemCurso origem)
    {
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .Where(c => c.Origem == origem)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Curso>> GetCursosParaSincronizacaoAsync()
    {
        var dataLimite = DateTime.UtcNow.AddHours(-24);
        
        return await _dbSet
            .Where(c => c.Origem == OrigemCurso.Senior && 
                       (c.UltimaSincronizacaoSenior == null || 
                        c.UltimaSincronizacaoSenior < dataLimite))
            .ToListAsync();
    }

    public async Task<IEnumerable<Curso>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _dbSet
                .Include(c => c.CriadoPor)
                .Include(c => c.Arquivos)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        var term = searchTerm.ToLower();
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .Where(c => c.NomeCurso.ToLower().Contains(term) || 
                       c.CodigoCurso.ToLower().Contains(term) ||
                       c.DescricaoAcademia.ToLower().Contains(term))
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<StatusCurso, int>> GetCursosPorStatusAsync()
    {
        var result = await _dbSet
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return result.ToDictionary(x => x.Status, x => x.Count);
    }

    public async Task<Dictionary<OrigemCurso, int>> GetCursosPorOrigemAsync()
    {
        var result = await _dbSet
            .GroupBy(c => c.Origem)
            .Select(g => new { Origem = g.Key, Count = g.Count() })
            .ToListAsync();

        return result.ToDictionary(x => x.Origem, x => x.Count);
    }

    public override async Task<Curso?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .Include(c => c.LogsAtividade)
                .ThenInclude(l => l.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Curso>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.CriadoPor)
            .Include(c => c.Arquivos)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}

