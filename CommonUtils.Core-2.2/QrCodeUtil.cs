using System;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace CommonUtils
{
    /// <summary>
    /// 二维码工具
    /// </summary>
    public static class QrCodeUtil
    {
        /// <summary>
        /// 返回二维码图片
        /// </summary>
        public static Bitmap Encode(string text)
        {
            try
            {
                var qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 4;
                qrCodeEncoder.QRCodeVersion = 6;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                return qrCodeEncoder.Encode(text);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return null;
            }
        }

        public static string GetWebBase64(string text)
        {
            var image = Encode(text);
            return "data:image/png;base64," + image.GetBase64();
        }

        /// <summary>
        /// 定义参数,生成二维码
        /// </summary>
        public static void Create(string text, string path)
        {
            try
            {
                var image = Encode(text);
                if (image == null)
                    return;
                image.SaveTo(path);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
            }
        }

        /// <summary>
        /// 返回二维码定义的字符串
        /// </summary>
        public static string Decode(Bitmap image)
        {
            try
            {
                var qrCodeBitmapImage = new QRCodeBitmapImage(image);
                var qrCodeDecoder = new QRCodeDecoder();
                return qrCodeDecoder.decode(qrCodeBitmapImage); ;
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return "#";
            }
        }

        /// <summary>
        /// 返回二维码定义的字符串
        /// </summary>
        public static string Decode(string path)
        {
            return Decode(new Bitmap(path));
        }
    }
}
