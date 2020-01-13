using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 邮件发送服务器
    /// </summary>
    public class MailSmtp
    {
        /// <summary>
        /// 账号
        /// </summary>
        private string _account { get; set; }

        /// <summary>
        /// smtp客户端
        /// </summary>
        private SmtpClient _smtpClient { get; set; }

        /// <summary>
        /// Smtp是否有效
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public MailSmtp(string host, int port, string account, string password, bool test = false)
        {
            _account = account;
            _smtpClient = new SmtpClient(host, port);
            //587 465 为ssl 25已被阿里云封杀
            if (port != 25)
                _smtpClient.EnableSsl = true;
            _smtpClient.Credentials = new NetworkCredential(_account, password);

            if (test)
            {
                var mail = new Mail("test@test.test");
                Send(mail);
                LogUtil.Log("MailSmtp.Test:" + mail.ResultInfo);
                Enable = mail.SendResult;
            }
        }

        /// <summary>
        /// 从配置初始化
        /// </summary>
        public static MailSmtp FromConfig()
        {
            var host = ConfigUtil.GetString("mail-smtp-host");
            var port = ConfigUtil.GetValue("mail-smtp-port", 587);
            var account = ConfigUtil.GetString("mail-smtp-account");
            var password = ConfigUtil.GetString("mail-smtp-password");
            var test = ConfigUtil.Get<bool>("mail-smtp-test");
            if (StringUtil.IsNullOrWhiteSpace(host, account, password))
            {
                ConfigUtil.Set("mail-smtp", "--------------mail-smtp---------------");
                ConfigUtil.Set("mail-smtp-host", "smtp.xxxx.com", false);
                ConfigUtil.Set("mail-smtp-port", 587, false);
                ConfigUtil.Set("mail-smtp-account", "xxxx@xxxx.com", false);
                ConfigUtil.Set("mail-smtp-password", "", false);
                ConfigUtil.Set("mail-smtp-test", false, false);
                var ex = ConfigUtil.GetMissingException(new StackTrace(true));
                LogUtil.Log(ex);
                throw ex;
            }
            return new MailSmtp(host, port, account, password, test);
        }

        /// <summary>
        /// 发送
        /// </summary>
        public bool Send(Mail mail)
        {
            try
            {
                mail.ModifyTime = DateTime.Now;
                mail.SenderAddress = _account;

                //定义正文
                var message = new MailMessage();
                //发件人
                message.From = new MailAddress(mail.SenderAddress, mail.SenderName, Encoding.UTF8);
                //收件人
                if (!string.IsNullOrEmpty(mail.ReceiverAddress))
                    message.To.Add(new MailAddress(mail.ReceiverAddress, mail.ReceiverName, Encoding.UTF8));
                foreach (var receiver in mail.Receivers)
                    message.To.Add(new MailAddress(receiver.Key, receiver.Value, Encoding.UTF8));
                if (message.To.Count == 0)
                {
                    LogUtil.Log("Mail loses receiver with title: " + mail.Title);
                    return false;
                }
                //主题
                message.Subject = mail.Title;
                //正文
                message.Body = mail.Content;
                //html支持
                message.IsBodyHtml = true;
                //编码
                message.BodyEncoding = message.HeadersEncoding = message.SubjectEncoding = Encoding.UTF8;
                //发送
                _smtpClient.Send(message);

                mail.SendResult = true;
                mail.ResultInfo = "发送成功";
                LogUtil.Log(string.Format("Program has sended mail to {0} with result {1} and title: {2}", mail.ReceiverAddress, mail.ResultInfo, mail.Title));
                return true;
            }
            catch (Exception ex)
            {
                mail.SendResult = false;
                mail.ResultInfo = "发送失败：" + ex.Message;
                LogUtil.Log(ex);
                return false;
            }
        }
    }
}
