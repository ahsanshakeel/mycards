using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Helper
{
    public class MailHelper
    {
        private const int Timeout = 180000;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _sender;
        private readonly string _pass;
        private readonly bool _ssl;

        //public string Recipient { get; set; }
        //public string RecipientCC { get; set; }
        //public string Subject { get; set; }
        //public string Body { get; set; }
        //public string AttachmentFile { get; set; }

        public MailHelper()
        {
            //MailServer - Represents the SMTP Server
            _host = ConfigurationManager.AppSettings["EmailHost"];
            //Port- Represents the port number
            _port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]);
            //MailAuthUser and MailAuthPass - Used for Authentication for sending email
            _sender = ConfigurationManager.AppSettings["EmailID"];
            _user = ConfigurationManager.AppSettings["EmailUserID"];
            _pass = ConfigurationManager.AppSettings["EmailUserPassword"];
            _ssl = true;// Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
        }

        public async Task SendAsync(string recipient, string subject, string body, string attachmentFile)
        {
            MailMessage message = null;
            SmtpClient smtp = null;
            Attachment att = null;
            try
            {
                message = new MailMessage(_sender, recipient, subject, body) { IsBodyHtml = true };
                //if (RecipientCC != null)
                //{
                //    message.CC.Add(RecipientCC);
                //}
                smtp = new SmtpClient(_host, _port);

                if (!String.IsNullOrEmpty(attachmentFile))
                {
                    if (File.Exists(attachmentFile))
                    {
                        att = new Attachment(attachmentFile);
                        message.Attachments.Add(att);
                    }
                }

                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = _ssl;
                }
                
                await smtp.SendMailAsync(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (message != null)
                    message.Dispose();
                if (smtp != null)
                    smtp.Dispose();
                if (att != null)
                    att.Dispose();
            }
        }

        public async Task SendAsyncAttach(string recipient, string subject, string body, byte[] attachmentFile, string attachmentName)
        {
            MailMessage message = null;
            SmtpClient smtp = null;
            Attachment att = null;
            try
            {
                message = new MailMessage(_sender, recipient, subject, body) { IsBodyHtml = true };
                //if (RecipientCC != null)
                //{
                //    message.CC.Add(RecipientCC);
                //}
                smtp = new SmtpClient(_host, _port);

                //if (!String.IsNullOrEmpty(attachmentFile))
                //{
                //    if (File.Exists(attachmentFile))
                //    {
                //        att = new Attachment(attachmentFile);
                //        message.Attachments.Add(att);
                //    }
                //}
                message.Attachments.Add(new Attachment(new MemoryStream(attachmentFile), attachmentName));


                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = _ssl;
                }

                await smtp.SendMailAsync(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (message != null)
                    message.Dispose();
                if (smtp != null)
                    smtp.Dispose();
                if (att != null)
                    att.Dispose();
            }
        }

        public async Task SendAsyncAttachPO(string recipient, string subject, string body, byte[] attachmentFile, string attachmentName, byte[] poAttachmentFile, string poAttachmentName)
        {
            MailMessage message = null;
            SmtpClient smtp = null;
            Attachment att = null;
            try
            {
                message = new MailMessage(_sender, recipient, subject, body) { IsBodyHtml = true };
                //if (RecipientCC != null)
                //{
                //    message.CC.Add(RecipientCC);
                //}
                smtp = new SmtpClient(_host, _port);

                //if (!String.IsNullOrEmpty(attachmentFile))
                //{
                //    if (File.Exists(attachmentFile))
                //    {
                //        att = new Attachment(attachmentFile);
                //        message.Attachments.Add(att);
                //    }
                //}
                message.Attachments.Add(new Attachment(new MemoryStream(attachmentFile), attachmentName));
                message.Attachments.Add(new Attachment(new MemoryStream(poAttachmentFile), poAttachmentName));

                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = _ssl;
                }

                await smtp.SendMailAsync(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (message != null)
                    message.Dispose();
                if (smtp != null)
                    smtp.Dispose();
                if (att != null)
                    att.Dispose();
            }
        }

        public async Task SendAsyncBCC(string recipient, string subject, string body, string attachmentFile)
        {
            MailMessage message = null;
            SmtpClient smtp = null;
            Attachment att = null;
            try
            {
                message = new MailMessage(_sender, recipient, subject, body) { IsBodyHtml = true };
                //if (RecipientCC != null)
                //{
                //    message.CC.Add(RecipientCC);
                //}
                MailAddress bcc = new MailAddress("accounts@mycards.com");
                message.Bcc.Add(bcc);

                smtp = new SmtpClient(_host, _port);

                if (!String.IsNullOrEmpty(attachmentFile))
                {
                    if (File.Exists(attachmentFile))
                    {
                        att = new Attachment(attachmentFile);
                        message.Attachments.Add(att);
                    }
                }

                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = _ssl;
                }

                await smtp.SendMailAsync(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (message != null)
                    message.Dispose();
                if (smtp != null)
                    smtp.Dispose();
                if (att != null)
                    att.Dispose();
            }
        }

    }
}
