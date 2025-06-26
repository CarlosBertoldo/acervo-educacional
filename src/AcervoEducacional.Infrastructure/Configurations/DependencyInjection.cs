using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Infrastructure.Data;
using AcervoEducacional.Infrastructure.Repositories;
using AcervoEducacional.Infrastructure.Services;

namespace AcervoEducacional.Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AcervoEducacionalContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<IArquivoRepository, ArquivoRepository>();
        services.AddScoped<ILogAtividadeRepository, LogAtividadeRepository>();
        services.AddScoped<ISessaoUsuarioRepository, SessaoUsuarioRepository>();
        services.AddScoped<ITokenRecuperacaoRepository, TokenRecuperacaoRepository>();
        services.AddScoped<IConfiguracaoSistemaRepository, ConfiguracaoSistemaRepository>();
        services.AddScoped<ISincronizacaoSeniorRepository, SincronizacaoSeniorRepository>();
        services.AddScoped<IConflitoCursoRepository, ConflitoCursoRepository>();

        // AWS S3
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IAwsS3Service, AwsS3Service>();

        // Email Service
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}

