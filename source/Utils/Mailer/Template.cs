using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace UniMail
{
    public class Template
    {
        public string FromName;
        public string FromEmail;
        public string ToName;
        public string ToEmail;
        public string Subject;
        public bool IsHTML;
        
        public string Body;
        public System.Text.Encoding Encoding = System.Text.Encoding.Unicode;

        public string PlainTextBody;
        public System.Text.Encoding AlternateEncoding = System.Text.Encoding.ASCII;

        public IDictionary<object, object> Data = new Dictionary<object, object>();

        /// <summary>
        /// Constructs template form given ABSOLUTE file path.
        /// First line in template is subject. 
        /// Template should look like:
        /// Subject: Hello there
        /// 
        /// Message text goes here
        /// </summary>
        /// <param name="filename">File containing template</param>
        /// <returns></returns>
        public Template(string filename)
        {
            //MailerConfiguration readConfig = (MailerConfiguration)System.Configuration.ConfigurationManager.GetSection("mail");
           
            MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], "mail");
            FromName = readConfig.SenderName;
            FromEmail = readConfig.SenderEmail;

            try
            {
                System.IO.TextReader fileReader = new System.IO.StreamReader(filename);
                StringBuilder templateBuilder = new StringBuilder("");

                Subject = fileReader.ReadLine();
                Subject.Remove(0, 8);

                string line;
                // Read lines from the file until the end of 
                // the file is reached.
                while ((line = fileReader.ReadLine()) != null)
                {
                    templateBuilder.AppendLine(line);
                }

                this.Body = templateBuilder.ToString();
            }
            catch (System.Exception){}
        }

        public Template(string filename, bool IsHTML)
        {
          //MailerConfiguration readConfig = (MailerConfiguration)System.Configuration.ConfigurationManager.GetSection("mail");

          MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], "mail");
          FromName = readConfig.SenderName;
          FromEmail = readConfig.SenderEmail;

          this.IsHTML = IsHTML;

          try
          {
            System.IO.TextReader fileReader = new System.IO.StreamReader(filename);
            StringBuilder templateBuilder = new StringBuilder("");

            Subject = fileReader.ReadLine();
            Subject.Remove(0, 8);

            string line;
            // Read lines from the file until the end of 
            // the file is reached.
            while ((line = fileReader.ReadLine()) != null)
            {
              templateBuilder.AppendLine(line);
            }

            this.Body = templateBuilder.ToString();            
          }
          catch (System.Exception) { }
        }

        public Template()
        {            
            //MailerConfiguration readConfig = (MailerConfiguration)System.Configuration.ConfigurationManager.GetSection("mail");
            MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], "mail");
            FromName =  readConfig.SenderName;
            FromEmail = readConfig.SenderEmail;
        }

        public MailMessage Render()
        {
            MailMessage message = new MailMessage(new MailAddress(FromEmail, FromName), new MailAddress(ToEmail, ToName));
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = IsHTML;

            string AlternateBody = PlainTextBody;

            if (Data != null)
            {
                foreach (object mask in Data.Keys)
                {
                    message.Body = message.Body.Replace(mask.ToString(), Data[mask.ToString()].ToString());
                    message.Subject = message.Subject.Replace(mask.ToString(), Data[mask.ToString()].ToString());                    
                    if (AlternateBody != null)
                    {
                        AlternateBody = AlternateBody.Replace(mask.ToString(), Data[mask.ToString()].ToString());
                    }
                }
            }

            message.BodyEncoding = Encoding;

            if (AlternateBody != null)
            {
                if (AlternateBody.Length > 0)
                {
                    message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(AlternateBody, AlternateEncoding, "text/plain"));
                }
            }

            return message;
        }

        public MailMessage Render(string fileAttechment)
        {
          MailMessage message = new MailMessage(new MailAddress(FromEmail, FromName), new MailAddress(ToEmail, ToName));
          message.Subject = Subject;
          message.Body = Body;
          message.IsBodyHtml = IsHTML;
          message.Attachments.Add(new Attachment(fileAttechment));

          string AlternateBody = PlainTextBody;

          if (Data != null)
          {
            foreach (object mask in Data.Keys)
            {
              message.Body = message.Body.Replace(mask.ToString(), Data[mask.ToString()].ToString());
              message.Subject = message.Subject.Replace(mask.ToString(), Data[mask.ToString()].ToString());
              if (AlternateBody != null)
              {
                AlternateBody = AlternateBody.Replace(mask.ToString(), Data[mask.ToString()].ToString());
              }
            }
          }

          message.BodyEncoding = Encoding;

          if (AlternateBody != null)
          {
            if (AlternateBody.Length > 0)
            {
              message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(AlternateBody, AlternateEncoding, "text/plain"));
            }
          }

          return message;
        }
    }
}
