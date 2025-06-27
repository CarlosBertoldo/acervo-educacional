using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire.Dashboard;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Infrastructure.Data;
using AcervoEducacional.Domain.Entities;
using BCrypt.Net;

namespace AcervoEducacional.WebApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IAuthService, AcervoEducacional.Application.Services.AuthService>();
        // services.AddScoped<IUsuarioService, UsuarioService>();
        // services.AddScoped<ICursoService, CursoService>();
        // services.AddScoped<IArquivoService, ArquivoService>();
        // services.AddScoped<ILogService, LogService>();
        // services.AddScoped<IReportService, ReportService>();
        // services.AddScoped<ISeniorIntegrationService, SeniorIntegrationService>();

        return services;
    }

    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }

    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AcervoEducacionalContext>();

        try
        {
            // Executar as migrações automaticamente
            await context.Database.MigrateAsync();

            // Seed Admin User
            if (!context.Usuarios.Any())
            {
                var adminUser = new Usuario
                {
                    Nome = "Administrador",
                    Email = "admin@acervo.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    IsAdmin = true,
                    IsAtivo = true
                };

                context.Usuarios.Add(adminUser);
                await context.SaveChangesAsync();
                Console.WriteLine("[SeedData] Usuário administrador criado com sucesso.");
            }

            // Seed System Configurations
            if (!context.ConfiguracoesSistema.Any())
            {
                var configs = new[]
                {
                    new ConfiguracaoSistema
                    {
                        Chave = "sistema.nome",
                        Valor = "Acervo Educacional",
                        Descricao = "Nome do sistema",
                        Categoria = "Sistema"
                    },
                    new ConfiguracaoSistema
                    {
                        Chave = "sistema.versao",
                        Valor = "1.0.0",
                        Descricao = "Versão do sistema",
                        Categoria = "Sistema"
                    },
                    new ConfiguracaoSistema
                    {
                        Chave = "senior.sync.enabled",
                        Valor = "true",
                        Descricao = "Habilitar sincronização com Senior",
                        Categoria = "Senior"
                    },
                    new ConfiguracaoSistema
                    {
                        Chave = "senior.sync.cron",
                        Valor = "0 2 * * *",
                        Descricao = "Expressão cron para sincronização automática",
                        Categoria = "Senior"
                    },
                    new ConfiguracaoSistema
                    {
                        Chave = "arquivos.tamanho.maximo",
                        Valor = "524288000",
                        Descricao = "Tamanho máximo de arquivo em bytes (500MB)",
                        Categoria = "Arquivos"
                    }
                };

                context.ConfiguracoesSistema.AddRange(configs);
                await context.SaveChangesAsync();
                Console.WriteLine("[SeedData] Configurações de sistema criadas com sucesso.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SeedData] Erro ao executar seed de dados: {ex.Message}");
        }
    }
}

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Em produção, implementar autenticação adequada
        return true;
    }
}

