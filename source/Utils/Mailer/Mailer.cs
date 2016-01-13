using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;
using System.Threading;

using System.Configuration;
using System.Net;

namespace UniMail
{
    public class Mailer
    {
        protected static MailerConfiguration _smtpConfig = new MailerConfiguration();
        protected static MailAddress replyTo;

        public delegate void OperationCompletedEventHandler(object sender, OperationCompletedEventArgs args);

        public event OperationCompletedEventHandler OperationCompleted;

        protected void ReportMessageResult(string MessageId, bool IsSent)
        {
            OperationCompletedEventArgs args = new OperationCompletedEventArgs(MessageId, IsSent);

            OperationCompleted(this, args);
        }


        static Mailer()
        {
            //MailerConfiguration readConfig = (MailerConfiguration)System.Configuration.ConfigurationManager.GetSection("mail");            
            MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], "mail");
            MailerConfiguration boundConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], "returnpath");
            
            if (readConfig != null)
            {
                _smtpConfig = readConfig;
            }
            if (boundConfig != null)
            {
                replyTo = new MailAddress(boundConfig.SenderEmail, boundConfig.SenderName);                 
            }
            else
            {
                replyTo = null;
            }
        }

        protected static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            int messageId = (int)e.UserState;

            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", messageId, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message {0} sent successfully.", messageId);
            }

        }

        protected static void MailSenderThread(object messageObject)
        {
            try
            {
                SmtpClient client = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port);
                client.Credentials = new NetworkCredential(_smtpConfig.User, _smtpConfig.Password);
                /*client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;*/
                //client.ServicePoint = 
                // Set the method that is called back when the send operation ends.
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                // Blocking send
                if (messageObject is MailMessage)
                {
                    SendSingleMessage(client, messageObject as MailMessage);
                }
                else
                {
                    if (messageObject is IEnumerable<MailMessage>)
                    {
                        foreach (MailMessage message in (messageObject as IEnumerable<MailMessage>))
                        {
                            SendSingleMessage(client, message);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("{1}", e.Message);
            }
        }

        private static void SendSingleMessage(SmtpClient client, MailMessage message)
        {

            if (replyTo != null)
                message.ReplyTo = replyTo;
            try
            {   
                client.Send(message);
            }
            catch (System.Exception e)
            {                
                Console.WriteLine("[{0}] {1}", message.GetHashCode(), e.Message);                
            }
            finally
            {
                message.Dispose();
            }
        }


        public static void Enqueue(MailMessage message)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(MailSenderThread, message);
        }

        public static void Enqueue(IEnumerable<MailMessage> messages)
        {
            List<MailMessage> messageList = messages.ToList();

            if (messages.Count() > _smtpConfig.MessagesPerSession)
            {

                while (messageList.Count > _smtpConfig.MessagesPerSession)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(MailSenderThread, messageList.Take(_smtpConfig.MessagesPerSession));
                    messageList.RemoveRange(0, _smtpConfig.MessagesPerSession);
                }
            }

            if (messageList.Count > 0)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(MailSenderThread, messageList);
            }
        }

        public static void Send(MailMessage message)
        {
            Thread MailSender = new Thread(MailSenderThread);
            MailSender.Start(message);
        }

        public static void Send(IEnumerable<MailMessage> messages)
        {
            Thread MailSender = new Thread(MailSenderThread);
            MailSender.Start(messages);
        }
    }
}
