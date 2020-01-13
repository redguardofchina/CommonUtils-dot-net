using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonUtils
{
    /// <summary>
    /// Windows图片处理
    /// </summary>
    public static class WindowsImage
    {
        /// <summary>
        /// 颜色刷
        ///  System.Windows.Media.Colors.XXX
        /// </summary>
        public static SolidColorBrush GetColorBrush(Color color)
        {
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// 透明色
        /// </summary>
        public static SolidColorBrush TransparentBrush { get; } = GetColorBrush(Colors.Transparent);

        /// <summary>
        /// 蓝色
        /// </summary>
        public static SolidColorBrush BlueBrush { get; } = GetColorBrush(Colors.Blue);


        /// <summary>
        /// 获取图片源
        /// </summary>
        public static BitmapSource GetSource(Stream stream)
        {
            //已验证stream不可关闭
            return BitmapFrame.Create(stream);
        }

        /// <summary>
        /// 获取图片源
        /// </summary>
        public static BitmapSource GetSource(this System.Drawing.Image image)
        {
            return GetSource(image.GetStream());
        }

        /// <summary>
        /// 获取图片源
        /// </summary>
        public static BitmapSource GetSource(byte[] bytes)
        {
            return GetSource(bytes.ToStream());
        }

        /// <summary>
        /// 获取图片源
        /// </summary>
        public static BitmapSource GetSourceFromBase64(string base64)
        {
            return GetSource(base64.Base64Decode());
        }

        /// <summary>
        /// 获取图片源
        /// </summary>
        public static BitmapSource GetSourceFromUrlOrPath(string urlOrPath)
        {
            return new BitmapImage(new Uri(urlOrPath));
        }
    }
}
