using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<IEnumerable<Usuario>> GetActiveUsersAsync();
    Task<IEnumerable<Usuario>> SearchAsync(string searchTerm);
}

public interface ICursoRepository : IBaseRepository<Curso>
{
    Task<Curso?> GetByCodigoAsync(string codigo);
    Task<Curso?> GetByCodigoSeniorAsync(string codigoSenior);
    Task<bool> CodigoExistsAsync(string codigo, Guid? excludeId = null);
    Task<IEnumerable<Curso>> GetByStatusAsync(StatusCurso status);
    Task<IEnumerable<Curso>> GetByOrigemAsync(OrigemCurso origem);
    Task<IEnumerable<Curso>> GetCursosParaSincronizacaoAsync();
    Task<IEnumerable<Curso>> SearchAsync(string searchTerm);
    Task<Dictionary<StatusCurso, int>> GetCursosPorStatusAsync();
    Task<Dictionary<OrigemCurso, int>> GetCursosPorOrigemAsync();
}

public interface IArquivoRepository : IBaseRepository<Arquivo>
{
    Task<IEnumerable<Arquivo>> GetByCursoIdAsync(Guid cursoId);
    Task<IEnumerable<Arquivo>> GetByCategoriaAsync(CategoriaArquivo categoria);
    Task<Arquivo?> GetByTokenCompartilhamentoAsync(string token);
    Task<IEnumerable<Arquivo>> GetArquivosExpiradosAsync();
    Task<IEnumerable<Arquivo>> GetArquivosPublicosAsync();
    Task<long> GetTamanhoTotalByCursoAsync(Guid cursoId);
    Task<Dictionary<CategoriaArquivo, int>> GetArquivosPorCategoriaAsync(Guid? cursoId = null);
}

public interface ILogAtividadeRepository : IBaseRepository<LogAtividade>
{
    Task<IEnumerable<LogAtividade>> GetByUsuarioIdAsync(Guid usuarioId);
    Task<IEnumerable<LogAtividade>> GetByCursoIdAsync(Guid cursoId);
    Task<IEnumerable<LogAtividade>> GetByArquivoIdAsync(Guid arquivoId);
    Task<IEnumerable<LogAtividade>> GetByAcaoAsync(TipoAcao acao);
    Task<IEnumerable<LogAtividade>> GetRecentAsync(int count = 50);
    Task<IEnumerable<LogAtividade>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
}

public interface ISessaoUsuarioRepository : IBaseRepository<SessaoUsuario>
{
    Task<SessaoUsuario?> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<SessaoUsuario>> GetByUsuarioIdAsync(Guid usuarioId);
    Task RevogarTodasSessoesUsuarioAsync(Guid usuarioId);
    Task LimparSessoesExpiradasAsync();
}

public interface ITokenRecuperacaoRepository : IBaseRepository<TokenRecuperacao>
{
    Task<TokenRecuperacao?> GetByTokenAsync(string token);
    Task<IEnumerable<TokenRecuperacao>> GetByUsuarioIdAsync(Guid usuarioId);
    Task InvalidarTokensUsuarioAsync(Guid usuarioId);
    Task LimparTokensExpiradosAsync();
}

public interface IConfiguracaoSistemaRepository : IBaseRepository<ConfiguracaoSistema>
{
    Task<ConfiguracaoSistema?> GetByChaveAsync(string chave);
    Task<IEnumerable<ConfiguracaoSistema>> GetByCategoriaAsync(string categoria);
    Task<string?> GetValorAsync(string chave);
    Task SetValorAsync(string chave, string valor, string? descricao = null, string? categoria = null);
}

public interface ISincronizacaoSeniorRepository : IBaseRepository<SincronizacaoSenior>
{
    Task<SincronizacaoSenior?> GetUltimaSincronizacaoAsync();
    Task<IEnumerable<SincronizacaoSenior>> GetHistoricoAsync(int count = 10);
    Task<SincronizacaoSenior?> GetSincronizacaoEmAndamentoAsync();
}

public interface IConflitoCursoRepository : IBaseRepository<ConflitoCurso>
{
    Task<IEnumerable<ConflitoCurso>> GetConflitosNaoResolvidosAsync();
    Task<IEnumerable<ConflitoCurso>> GetByCursoIdAsync(Guid cursoId);
    Task<IEnumerable<ConflitoCurso>> GetByTipoConflitoAsync(TipoConflito tipo);
    Task ResolverConflitosEmLoteAsync(IEnumerable<Guid> conflitosIds, ResolucaoConflito resolucao, Guid usuarioId);
}

public interface IUnitOfWork : IDisposable
{
    IUsuarioRepository Usuarios { get; }
    ICursoRepository Cursos { get; }
    IArquivoRepository Arquivos { get; }
    ILogAtividadeRepository LogsAtividade { get; }
    ISessaoUsuarioRepository SessoesUsuario { get; }
    ITokenRecuperacaoRepository TokensRecuperacao { get; }
    IConfiguracaoSistemaRepository ConfiguracoesSistema { get; }
    ISincronizacaoSeniorRepository SincronizacoesSenior { get; }
    IConflitoCursoRepository ConflitoCursos { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

