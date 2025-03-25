using LibroManager.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;

namespace LibroManager.Services;

public class EmailService : IEmailSender<ApplicationUser>
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>()!;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            mensaje.To.Add(MailboxAddress.Parse(email));
            mensaje.Subject = subject;
            mensaje.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo a {Email}", email);
            throw;
        }
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return SendEmailAsync(email, "Confirma tu correo electrónico",
            $"Por favor confirma tu cuenta haciendo clic <a href='{confirmationLink}'>aquí</a>.");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return SendEmailAsync(email, "Restablecer contraseña",
            $"Por favor restablece tu contraseña haciendo click <a href='{resetLink}'>aquí</a>.");
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        return SendEmailAsync(email, "Código para restablecer contraseña",
            $"Tu código para restablecer la contraseña es: <strong>{resetCode}</strong>");
    }
}