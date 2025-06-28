using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using System.Text.Json;

namespace AcervoEducacional.Infrastructure.Data;

public class AcervoEducacionalContext : DbContext
{
    public AcervoEducacionalContext(DbContextOptions<AcervoEducacionalContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Arquivo> Arquivos { get; set; }
    public DbSet<LogAtividade> LogsAtividade { get; set; }
    public DbSet<SessaoUsuario> SessoesUsuario { get; set; }
    public DbSet<TokenRecuperacao> TokensRecuperacao { get; set; }
    public DbSet<ConfiguracaoSistema> ConfiguracoesSistema { get; set; }
    public DbSet<SincronizacaoSenior> SincronizacoesSenior { get; set; }
    public DbSet<ConflitoCurso> ConflitoCursos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UltimoIp).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configurações de Curso
        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DescricaoAcademia).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CodigoCurso).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NomeCurso).IsRequired().HasMaxLength(300);
            entity.Property(e => e.CodigoSenior).HasMaxLength(50);
            
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();
            entity.Property(e => e.TipoAmbiente)
                .HasConversion<int>()
                .IsRequired();
            entity.Property(e => e.TipoAcesso)
                .HasConversion<int>()
                .IsRequired();
            entity.Property(e => e.Origem)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.Metadados)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null));

            entity.HasIndex(e => e.CodigoCurso).IsUnique();
            entity.HasIndex(e => e.CodigoSenior);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Origem);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.CriadoPor)
                .WithMany(u => u.CursosCriados)
                .HasForeignKey(e => e.CriadoPorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações de Arquivo
        modelBuilder.Entity<Arquivo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(500);
            entity.Property(e => e.NomeOriginal).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TipoMime).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CaminhoS3).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.UrlPublica).HasMaxLength(1000);
            entity.Property(e => e.TokenCompartilhamento).HasMaxLength(50);

            entity.Property(e => e.Categoria)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.DominiosPermitidos)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null));

            entity.Property(e => e.BloqueiosAtivos)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null));

            entity.Property(e => e.Metadados)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null));

            entity.HasIndex(e => e.CursoId);
            entity.HasIndex(e => e.Categoria);
            entity.HasIndex(e => e.TokenCompartilhamento);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Curso)
                .WithMany(c => c.Arquivos)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurações de LogAtividade
        modelBuilder.Entity<LogAtividade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.Property(e => e.Acao)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.DadosAdicionais)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null));

            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.CursoId);
            entity.HasIndex(e => e.ArquivoId);
            entity.HasIndex(e => e.Acao);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.LogsAtividade)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Curso)
                .WithMany(c => c.LogsAtividade)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Arquivo)
                .WithMany(a => a.LogsAtividade)
                .HasForeignKey(e => e.ArquivoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações de SessaoUsuario
        modelBuilder.Entity<SessaoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RefreshToken).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasIndex(e => e.RefreshToken).IsUnique();
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.ExpiresAt);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Sessoes)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurações de TokenRecuperacao
        modelBuilder.Entity<TokenRecuperacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(45);

            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.ExpiresAt);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.TokensRecuperacao)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurações de ConfiguracaoSistema
        modelBuilder.Entity<ConfiguracaoSistema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Chave).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Valor).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.Categoria).HasMaxLength(100);

            entity.HasIndex(e => e.Chave).IsUnique();
            entity.HasIndex(e => e.Categoria);
        });

        // Configurar nomes de tabelas em snake_case
        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<Curso>().ToTable("cursos");
        modelBuilder.Entity<Arquivo>().ToTable("arquivos");
        modelBuilder.Entity<LogAtividade>().ToTable("logs_atividade");
        modelBuilder.Entity<SessaoUsuario>().ToTable("sessoes_usuario");
        modelBuilder.Entity<TokenRecuperacao>().ToTable("tokens_recuperacao");
        modelBuilder.Entity<ConfiguracaoSistema>().ToTable("configuracoes_sistema");
        modelBuilder.Entity<SincronizacaoSenior>().ToTable("sincronizacoes_senior");
        modelBuilder.Entity<ConflitoCurso>().ToTable("conflitos_curso");
    }

    private static void ConfigureEnumsForPostgreSQL(ModelBuilder modelBuilder)
    {
        // Esta configuração será necessária se usar enums nativos do PostgreSQL
        // Por enquanto, usando conversão para int que é mais compatível
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTimestamp();
            }
        }
    }
}

