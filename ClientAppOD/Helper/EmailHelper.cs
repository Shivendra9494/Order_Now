using System;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ClientAppOD.CustomModels;

namespace ClientAppOD.Helper
{
    public class EmailHelper
    {
        static string EmailFrom = "noreply@orderdirectly.com";
        static string host = "smtp-relay.sendinblue.com";
        static string Username = "osman@orderdirectly.com";
        static string Password = "K7nwfqDJQbGYP8IL";
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static void SendMailResetPassword(string address, string sub, string sender, string CustId, string code)
        {

            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(EmailFrom, sender);            // Recipient e-mail address.
                msg.To.Add(address);
                msg.Subject = sub;

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
                string MessageBody = "";
                MessageBody = "<html><head><title>Forgot Password</title></head><body>";
                MessageBody = MessageBody + "Hi," + "<br />";
                MessageBody = MessageBody + "Please click the link below to reset your password<br /><br />";
                MessageBody = MessageBody + "<b><a href='" + StaticFields.ServerURL + "/Home/ResetPassword/?id_c=" + CustId + "&sdert78uJI=" + code + "' target='_blank'>Reset your password</a></b>" + "<br />";

                //string webData = "Test";
                msg.Body = MessageBody;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = System.Text.Encoding.UTF8;

                // your remote SMTP server IP.
                SmtpClient smtp = new SmtpClient();

                smtp.Host = host;
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(Username, Password);

                smtp.Send(msg);


            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }


        }
        public static void SendMail(string address, string sub, string sender,string message)
        {

            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(EmailFrom, sender);            // Recipient e-mail address.
                msg.To.Add(address);
                msg.Subject = sub;

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
               
                //string webData = "Test";
                msg.Body = message;
                msg.IsBodyHtml = true;
                msg.BodyEncoding = System.Text.Encoding.UTF8;

                // your remote SMTP server IP.
                SmtpClient smtp = new SmtpClient();

                smtp.Host = host;
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(Username, Password);

                smtp.Send(msg);


            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }


        }
    }
}
