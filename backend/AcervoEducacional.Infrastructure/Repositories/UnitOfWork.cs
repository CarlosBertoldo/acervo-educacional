using Microsoft.EntityFrameworkCore.Storage;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AcervoEducacionalContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AcervoEducacionalContext context)
    {
        _context = context;
        
        Usuarios = new UsuarioRepository(_context);
        Cursos = new CursoRepository(_context);
        Arquivos = new ArquivoRepository(_context);
        LogsAtividade = new LogAtividadeRepository(_context);
        SessoesUsuario = new SessaoUsuarioRepository(_context);
        TokensRecuperacao = new TokenRecuperacaoRepository(_context);
        ConfiguracoesSistema = new ConfiguracaoSistemaRepository(_context);
        SincronizacoesSenior = new SincronizacaoSeniorRepository(_context);
        ConflitoCursos = new ConflitoCursoRepository(_context);
    }

    public IUsuarioRepository Usuarios { get; }
    public ICursoRepository Cursos { get; }
    public IArquivoRepository Arquivos { get; }
    public ILogAtividadeRepository LogsAtividade { get; }
    public ISessaoUsuarioRepository SessoesUsuario { get; }
    public ITokenRecuperacaoRepository TokensRecuperacao { get; }
    public IConfiguracaoSistemaRepository ConfiguracoesSistema { get; }
    public ISincronizacaoSeniorRepository SincronizacoesSenior { get; }
    public IConflitoCursoRepository ConflitoCursos { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

