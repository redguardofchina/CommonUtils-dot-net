using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 邮件处理工具
    /// </summary>
    public static class MailUtil
    {
        /// <summary>
        /// 从配置初始化Smtp
        /// </summary>
        public static MailSmtp Smtp { get; } = MailSmtp.FromConfig();

        /// <summary>
        /// 发送邮件
        /// </summary>
        public static void Send(this Mail email)
        {
            Smtp.Send(email);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public static void Send(string email, string content)
        {
            new Mail() { ReceiverAddress = email, Content = content }.Send();
        }
    }
}
