using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 邮件
    /// Json Uses Too
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// 逻辑ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 变更时间
        /// </summary>
        public DateTime ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 主题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发件人
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 邮件群发
        /// </summary>
        public MapStringString Receivers { get; set; } = new MapStringString();

        /// <summary>
        /// 邮件群发
        /// </summary>  
        public void AddReceiver(string receiverAddress, string receiverName = null)
        {
            Receivers.Set(receiverAddress, receiverName);
        }

        /// <summary>
        /// 邮件群发
        /// </summary>
        public void AddReceiver(params MailAddress[] receivers)
        {
            foreach (var receiver in receivers)
                AddReceiver(receiver.Address, receiver.DisplayName);
        }

        /// <summary>
        /// 邮件群发
        /// </summary>  
        public void AddReceiver(IEnumerable<MailAddress> receivers)
        {
            foreach (var receiver in receivers)
                AddReceiver(receiver.Address, receiver.DisplayName);
        }

        /// <summary>
        /// 发送结果 这个发送结果不代表收信方能收到 代表服务器能访问，发送正常
        /// </summary>
        public bool SendResult { get; set; }

        /// <summary>
        /// 发送结果
        /// </summary>
        public string ResultInfo { get; set; } = "未发送";

        /// <summary>
        /// 初始化
        /// </summary>
        public Mail(string receiverAddress = null, string receiverName = null)
        {
            ReceiverAddress = receiverAddress;
            ReceiverName = receiverName;
        }
    }
}
