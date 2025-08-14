using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SistemaPagos.Server.Services;


public interface IEmailService
{
    
    Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }
    //Configura los datos que enviará el correo electronico y lo envia
    public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        try
        {
            using (var smtpClient = new SmtpClient(_config["Smtp:Host"]))
            {
                // Configura el SMTP con los datos del servidor y las credenciales
                smtpClient.Port = int.Parse(_config["Smtp:Port"] ?? "587");
                smtpClient.Credentials = new NetworkCredential(
                    _config["Smtp:Username"],
                    _config["Smtp:Password"]);
                smtpClient.EnableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");

                // Crea el mensaje de correo
                var mensaje = new MailMessage
                {
                    From = new MailAddress(
                        _config["Smtp:FromEmail"] ?? "notificaciones@sistemapagos.com",
                        _config["Smtp:FromName"] ?? "Sistema de Pagos"),
                    Subject = asunto,
                    Body = cuerpoHtml,
                    IsBodyHtml = true
                };
                mensaje.To.Add(destinatario);

                await smtpClient.SendMailAsync(mensaje);

                _logger.LogInformation($"Correo enviado a {destinatario}");
            } 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al enviar correo a {destinatario}");
            throw; 
        }
    }
}