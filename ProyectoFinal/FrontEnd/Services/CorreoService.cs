using System.Net;
using System.Net.Mail;

namespace FrontEnd.Services
{
    public interface ICorreoService
    {
        Task EnviarCorreo(string emailReceiver, string subject, string body); 
    }

    public class CorreoService : ICorreoService
    {
        private readonly IConfiguration configuration;

        public CorreoService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // mi metodo de enviar correos por google - Ariel

        public async Task EnviarCorreo(string emailReceiver, string subject, string body)
        {
            //Contenido del achivo de config y archivo seguro de config
            var emailSender = configuration.GetValue<string>("CONFIG_EMAIL:EMAIL");
            var password = configuration.GetValue<string>("CONFIG_EMAIL:PASSWORD");
            var host = configuration.GetValue<string>("CONFIG_EMAIL:HOST");
            var port = configuration.GetValue<int>("CONFIG_EMAIL:PORT");

            // se crea el cliente protocolo de correo smtp
            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false; // se usan credenciales de la cuenta google

            smtpClient.Credentials = new NetworkCredential(emailSender, password);
            // se crea el mensaje
            var message = new MailMessage(emailSender!, emailReceiver, subject, body);

            // se envia el mensaje
            await smtpClient.SendMailAsync(message);
        }



    }
}
