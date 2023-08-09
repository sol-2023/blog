using System.Net;
using System.Net.Mail;

namespace Blog.Services;

public class EmailService
{
    public bool Send(
        string toName,
        string toEmail,
        string subject,
        string body,
        string fromName = "Equipe balta-io",
        string fromEmail = "email@balta.io"  )
    {
        // dá pra usar com qq provedor de emais: sendGrip, zoho, onesignal,etc
        var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);

        smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = true; // porqu esta usando porta 587 : appsettings.json

        var mail = new MailMessage(); // criando a mensagem
        mail.From = new MailAddress(fromEmail, fromName);
        // add, pq é uma lista, posso incluir varios destinatarios
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;// 
        // para enviar o email
        try
        {
            smtpClient.Send(mail);
            return true;

        }catch           
        {
            return false;
        }

    }
    
}