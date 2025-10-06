using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendConfirmationEmail(string to, string username)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback =
        (sender, certificate, chain, sslPolicyErrors) => true;

        var fromAddress = new MailAddress(_config["EmailSettings:From"], "Gabrielan't");
        var toAddress = new MailAddress(to);
        string fromPassword = _config["EmailSettings:Password"];
        string smtpUser = _config["EmailSettings:UserName"];
        string subject = "Confirmación de Registro";

        string body = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    max-width: 600px;
                    margin: auto;
                    background-color: #ffffff;
                    border: 1px solid #dddddd;
                    border-radius: 8px;
                    overflow: hidden;
                }}
                .header {{
                    background-color: #3D5481;
                    color: white;
                    padding: 20px;
                    display: flex;
                    align-items: center;
                }}
                .header img {{
                    width: 80px;
                    height: auto;
                    margin-right: 20px;
                }}
                .header-text h1 {{
                    margin: 0;
                    font-size: 24px;
                }}
                .header-text p {{
                    margin: 5px 0 0;
                    font-size: 14px;
                    font-weight: lighter;
                }}
                .content {{
                    padding: 20px;
                    color: #333333;
                }}
                .footer {{
                    background-color: #5671A2;
                    color: white;
                    text-align: center;
                    padding: 10px;
                    font-size: 12px;
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='header'>
                    <img src='https://i.imgur.com/DIiFJZq.png' alt='Logo del centro de eventos' />
                    <div class='header-text'>
                        <h1>Gabrielan't</h1>
                        <p>Centro de Eventos</p>
                    </div>
                </div>
                <div class='content'>
                    <p>Hola <strong>{username}</strong>,</p>
                    <p>¡Gracias por registrarte! Tu cuenta ha sido creada exitosamente en nuestro sistema.</p>
                    <p>Desde ahora podrás acceder a todas nuestras funcionalidades para gestionar eventos, reservar salas y más.</p>
                    <p>Si tienes alguna duda o necesitas ayuda, no dudes en contactarnos.</p>
                </div>
                <div class='footer'>
                    © 2025 Centro de Eventos Gabrielan't.
                </div>
            </div>
        </body>
        </html>";

        var smtp = new SmtpClient
        {
            Host = _config["EmailSettings:SmtpServer"],
            Port = int.Parse(_config["EmailSettings:Port"]),
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtpUser, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        await smtp.SendMailAsync(message);
    }

    public async Task SendReservationConfirmationEmail(
            string toEmail,
            string userFullName,
            int    reservaId)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback =
        (sender, certificate, chain, sslPolicyErrors) => true;

        var from = new MailAddress(_config["EmailSettings:From"], "Gabrielan't");
        var to   = new MailAddress(toEmail);

        var smtp = new SmtpClient
        {
            Host = _config["EmailSettings:SmtpServer"],
            Port = int.Parse(_config["EmailSettings:Port"]),
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _config["EmailSettings:UserName"],
                _config["EmailSettings:Password"])
        };

        string subject = "Tu reserva ha sido confirmada";
        string body = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans‑serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .email-container {{
                    max-width: 600px;
                    margin: auto;
                    background-color: #ffffff;
                    border: 1px solid #dddddd;
                    border-radius: 8px;
                    overflow: hidden;
                }}
                .header {{
                    background-color: #3D5481;
                    color: white;
                    padding: 20px;
                    display: flex;
                    align-items: center;
                }}
                .header img {{
                    width: 80px;
                    height: auto;
                    margin-right: 20px;
                }}
                .header-text h1 {{
                    margin: 0;
                    font-size: 24px;
                }}
                .header-text p {{
                    margin: 5px 0 0;
                    font-size: 14px;
                    font-weight: lighter;
                }}
                .content {{
                    padding: 20px;
                    color: #333333;
                }}
                .footer {{
                    background-color: #5671A2;
                    color: white;
                    text-align: center;
                    padding: 10px;
                    font-size: 12px;
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='header'>
                    <img src='https://i.imgur.com/DIiFJZq.png' alt='Logo del centro de eventos' />
                    <div class='header-text'>
                        <h1>Gabrielan't</h1>
                        <p>Centro de Eventos</p>
                    </div>
                </div>
                <div class='content'>
                    <p>Hola <strong>{userFullName}</strong>,</p>

                    <p>
                        Nos complace informarte que tu <strong>reserva #{reservaId}</strong>
                        ha sido <strong>aceptada</strong>.
                    </p>

                    <p>
                        Puedes ingresar a la sección <em>Mis reservas</em> de tu cuenta
                        cuando desees para completar el pago y consultar los detalles.
                    </p>

                    <p>
                        ¡Gracias por confiar en Gabrielan't!<br/>
                        Si necesitas ayuda adicional, contáctanos con gusto.
                    </p>
                </div>
                <div class='footer'>
                    © 2025 Centro de Eventos Gabrielan't
                </div>
            </div>
        </body>
        </html>";


        using var msg = new MailMessage(from, to)
        {
            Subject    = subject,
            Body       = body,
            IsBodyHtml = true
        };

        await smtp.SendMailAsync(msg);
    }

}
