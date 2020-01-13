using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;

namespace CommonUtils
{
    /// <summary>
    /// 页面返回定义
    /// </summary>
    public static class ResponseUtil
    {
        /// <summary>
        /// 当前Response
        /// </summary>
        public static HttpResponse Response
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }

        /// <summary>
        /// 页面提示
        /// </summary>
        public static void Alert(string msg)
        {
            string script = "<script>alert('" + msg + "');</script>";
            Write(script);
        }

        /// <summary>
        /// 清理页面,网页是清不掉的,建议使用ashx
        /// </summary>
        public static void Clear()
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        public static void ClosePage()
        {
            Write(HtmlUtil.CloseScript);
        }

        /// <summary>
        /// 服务器异常
        /// </summary>
        public static void WriteError()
        {
            Write("服务器异常");
        }

        /// <summary>
        /// 返回文件
        /// </summary>
        public static void File(string filePath, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = FileUtil.GetName(filePath);

            Clear();
            Response.Headers.Set("Content-Disposition", "attachment;" + "filename=\"" + name + "\"");
            Response.ContentType = CommonUtils.ContentType.Get();
            Response.WriteFile(filePath);
        }

        /// <summary>
        /// 返回文件
        /// </summary>
        public static void File(Stream stream, string name)
        {
            Clear();
            Response.Headers.Set("Content-Disposition", "attachment;" + "filename=\"" + name + "\"");
            Response.ContentType = CommonUtils.ContentType.Get();
            stream.CopyTo(Response.OutputStream);
        }

        /// <summary>
        /// 返回csv文件
        /// </summary>
        public static void Csv(string csv, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = Guid.NewGuid() + ".csv";
            TextFile(csv, name, Encodings.UTF8Bom);
        }

        /// <summary>
        /// 返回文本文件
        /// </summary>
        public static void TextFile(string text, string name = null, Encoding encode = null)
        {
            if (string.IsNullOrEmpty(name))
                name = Guid.NewGuid() + ".txt";
            if (encode == null)
                encode = Encoding.UTF8;

            Clear();
            Response.Headers.Set("Content-Disposition", "attachment;" + "filename=\"" + name + "\"");
            Response.ContentType = CommonUtils.ContentType.Get();
            StreamWriter streamWriter = new StreamWriter(Response.OutputStream, encode);
            streamWriter.Write(text);
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// 结束
        /// </summary>
        public static void End()
        {
            Response.End();
        }

        /// <summary>
        /// 返回空
        /// </summary>
        public static void Empty()
        {
            Clear();
            Response.ContentType = "text/plain";
        }

        /// <summary>
        /// 返回普通消息
        /// </summary>
        public static void Write(object msg)
        {
            Response.Write(msg);
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        public static void WriteSuccess()
        {
            Response.Write("提交成功");
        }

        /// <summary>
        /// 返回失败消息
        /// </summary>
        public static void WriteFail()
        {
            Response.Write("提交失败");
        }

        /// <summary>
        /// 返回结果消息
        /// </summary>
        public static void WriteResult(bool success)
        {
            if (success)
                WriteSuccess();
            else
                WriteFail();
        }

        /// <summary>
        /// 返回普通消息
        /// </summary>
        public static void WriteLine(object msg)
        {
            Response.Write(msg + "<br />");
        }

        /// <summary>
        /// 返回编码消息
        /// </summary>
        public static void WriteUrlEncoded(string msg)
        {
            Write(msg.UrlEncode());
        }

        /// <summary>
        /// 返回网页实体
        /// </summary>
        public static void WriteHtml(string msg)
        {
            msg = HtmlUtil.CompletePage(msg);
            Write(msg);
        }

        /// <summary>
        /// 返回网页实体
        /// </summary>
        public static void WriteJson(object obj)
        {
            string msg = JsonUtil.ToJson(obj);
            Write(msg);
        }

        /// <summary>
        /// 返回网页实体
        /// </summary>
        public static void WriteJsonHtml(object obj)
        {
            string msg = JsonUtil.ToJson(obj);
            WriteHtml(msg);
        }

        /// <summary>
        /// 返回普通消息
        /// </summary>
        public static void Test()
        {
            Response.Write(KeysUtil.ChinaInnovation);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="response"></param>
        public static void RedirectCurrent(this HttpResponse response)
        {
            response.Redirect(RequestUtil.Current.Path);
        }

        /// <summary>
        /// 设置
        /// </summary>
        public static void ContentType(string contentType)
        {
            Response.ContentType = contentType;
        }

        /// <summary>
        /// 在页面返回图片
        /// </summary>
        public static void Image(Image image, string filePathOrExtentionWithDot)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageUtil.GetFormat(filePathOrExtentionWithDot));
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = CommonUtils.ContentType.Get(filePathOrExtentionWithDot);
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            stream.Dispose();
            image.Dispose();
        }

        /// <summary>
        /// 在网页页面返回图片
        /// </summary>
        public static void Image(string path)
        {
            Image image = ImageUtil.Get(path);
            Image(image, path);
        }

        /// <summary>
        /// 在网页页面返回空图片
        /// </summary>
        public static void ImageNoSign()
        {
            Image(RequestUtil.GetPath("~/Img/NoPic.jpg"));
        }

        /// <summary>
        /// 在网页页面返回图片
        /// </summary>
        public static void ImageGifLast(string path)
        {
            Image gif = ImageUtil.GetGifLast(path);
            Image(gif, ".png");
        }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public static string ImageIdentifyCode()
        {
            Image image = ImageUtil.GetIdentifyCode(out string text);
            SessionUtil.Add(KeysUtil.IdentifyCode, text);
            Image(image, ".gif");
            return text;
        }

        /// <summary>
        /// 校验验证码
        /// </summary>
        public static bool VerifyIdentifyCode(string text)
        {
            return SessionUtil.GetString(KeysUtil.IdentifyCode) == text;
        }

        /// <summary>
        /// 在页面返回二维码图片
        /// </summary>
        public static void ImageQrCode(string text)
        {
            Bitmap bitmap = QrCodeUtil.Encode(text);
            Image(bitmap, ".bmp");
        }
    }
}
