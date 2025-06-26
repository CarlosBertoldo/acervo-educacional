using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Usuario>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsAtivo)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(u => u.Nome.ToLower().Contains(term) || 
                       u.Email.ToLower().Contains(term))
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public override async Task<Usuario?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.Sessoes)
            .Include(u => u.TokensRecuperacao)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}

