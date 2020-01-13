using Microsoft.Win32;
using System.Net.Mime;

namespace CommonUtils
{
    public static class ContentType
    {
        public const string Stream = MediaTypeNames.Application.Octet;

        public const string Jpeg = MediaTypeNames.Image.Jpeg;

        public const string Form = "application/x-www-form-urlencoded";

        public const string Json = "application/json";

        public const string JsonPatch = "application/json-patch+json";

        public const string Html = MediaTypeNames.Text.Html;

        /// <summary>
        /// 获取文件的ContentType
        /// </summary>
        public static string Get(string path = null)
        {
            string defaultContentType = Stream;
            if (string.IsNullOrWhiteSpace(path))
                return defaultContentType;

            string extension = FileUtil.GetExtension(path);
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension);
            object contentType = registryKey.GetValue("Content Type", defaultContentType);
            registryKey.Close();
            return contentType.ToString();
        }
    }
}
