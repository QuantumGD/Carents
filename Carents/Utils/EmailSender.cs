using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Carents.Utils
{
    public class EmailSender
    {
        private const String API_KEY = "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";

        public void Send(String toEmail, String subject, String contents, HttpPostedFileBase postedFile)
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@localhost.com", "carent");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                try
                {
                    byte[] file = new byte[postedFile.ContentLength];
                    postedFile.InputStream.Read(file, 0, file.Length);
                    var attachment = new Attachment()
                    {
                        Content = Convert.ToBase64String(file),
                        Type = Convert.ToString(postedFile.ContentType),
                        Filename = Path.GetFileName(postedFile.FileName)
                    };
                    msg.AddAttachment(attachment);
                }
                catch (Exception) { }
            }
            var response = client.SendEmailAsync(msg);
        }

    }
}