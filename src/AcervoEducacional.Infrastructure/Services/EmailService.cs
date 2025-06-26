using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPassword)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            
            _logger.LogInformation("Email enviado com sucesso para {To} com assunto '{Subject}'", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {To} com assunto '{Subject}'", to, subject);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string userName)
    {
        var resetUrl = $"{_configuration["App:BaseUrl"]}/reset-password?token={resetToken}";
        
        var subject = "Recuperação de Senha - Acervo Educacional";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Recuperação de Senha</h2>
                    
                    <p>Olá <strong>{userName}</strong>,</p>
                    
                    <p>Você solicitou a recuperação de senha para sua conta no Acervo Educacional.</p>
                    
                    <p>Para redefinir sua senha, clique no botão abaixo:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}' 
                           style='background-color: #2563eb; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Redefinir Senha
                        </a>
                    </div>
                    
                    <p>Ou copie e cole o link abaixo em seu navegador:</p>
                    <p style='word-break: break-all; background-color: #f3f4f6; padding: 10px; border-radius: 5px;'>
                        {resetUrl}
                    </p>
                    
                    <p><strong>Este link expira em 1 hora.</strong></p>
                    
                    <p>Se você não solicitou esta recuperação, ignore este email.</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
                    
                    <p style='font-size: 12px; color: #6b7280;'>
                        Este é um email automático, não responda a esta mensagem.<br>
                        Acervo Educacional - Sistema de Gestão de Cursos
                    </p>
                </div>
            </body>
            </html>";

        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendWelcomeEmailAsync(string to, string userName, string temporaryPassword)
    {
        var loginUrl = $"{_configuration["App:BaseUrl"]}/login";
        
        var subject = "Bem-vindo ao Acervo Educacional";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Bem-vindo ao Acervo Educacional!</h2>
                    
                    <p>Olá <strong>{userName}</strong>,</p>
                    
                    <p>Sua conta foi criada com sucesso no sistema Acervo Educacional.</p>
                    
                    <div style='background-color: #f3f4f6; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='margin-top: 0; color: #374151;'>Dados de Acesso:</h3>
                        <p><strong>Email:</strong> {to}</p>
                        <p><strong>Senha Temporária:</strong> {temporaryPassword}</p>
                    </div>
                    
                    <p><strong>⚠️ Importante:</strong> Por segurança, altere sua senha no primeiro acesso.</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{loginUrl}' 
                           style='background-color: #2563eb; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Acessar Sistema
                        </a>
                    </div>
                    
                    <h3 style='color: #374151;'>Recursos Disponíveis:</h3>
                    <ul>
                        <li>Gerenciamento de cursos via Kanban</li>
                        <li>Upload e organização de arquivos educacionais</li>
                        <li>Sistema de proteção de conteúdo</li>
                        <li>Integração com Senior</li>
                        <li>Relatórios e exportação de dados</li>
                    </ul>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
                    
                    <p style='font-size: 12px; color: #6b7280;'>
                        Este é um email automático, não responda a esta mensagem.<br>
                        Acervo Educacional - Sistema de Gestão de Cursos
                    </p>
                </div>
            </body>
            </html>";

        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendNotificationEmailAsync(string to, string subject, string message)
    {
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>Acervo Educacional - Notificação</h2>
                    
                    <div style='background-color: #f3f4f6; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        {message}
                    </div>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #e5e7eb;'>
                    
                    <p style='font-size: 12px; color: #6b7280;'>
                        Este é um email automático, não responda a esta mensagem.<br>
                        Acervo Educacional - Sistema de Gestão de Cursos<br>
                        Data: {DateTime.Now:dd/MM/yyyy HH:mm}
                    </p>
                </div>
            </body>
            </html>";

        return await SendEmailAsync(to, subject, body, true);
    }
}

