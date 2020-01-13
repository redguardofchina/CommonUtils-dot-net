using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 资源读取
    /// </summary>
    public static class ResourceUtil
    {
        /// <summary>
        /// 获取当前类库嵌入的资源 Resources.xxx
        /// </summary>
        public static Stream ReadStream(string path)
        {
            var type = ReflectionUtil.IndexType(2);
            return type.Assembly.GetManifestResourceStream(type.Namespace.Append(".", path));
        }

        public enum CommonResourceName
        {
            LoadingImage,
            MissingImage,
            Sqlite,
            TestText
        }

        /// <summary>
        /// 获取公用类库嵌入的资源
        /// </summary>
        public static Stream ReadCommonStream(CommonResourceName commonResourceName)
        {
            string name = "";

            switch (commonResourceName)
            {
                case CommonResourceName.LoadingImage:
                    name = "loading.gif";
                    break;
                case CommonResourceName.MissingImage:
                    name = "missing.png";
                    break;
                case CommonResourceName.Sqlite:
                    name = "sqlite.db";
                    break;
                case CommonResourceName.TestText:
                    name = "test.txt";
                    break;
            }

            //这里有时候是CommonNamespace有时候是CommonAssemblyName，烦
            var path = ReflectionUtil.CommonNamespace.Append(".Resources.", name);
            //var path = ReflectionUtil.CommonAssemblyName.Append(".Resources.", name);

            var stream = ReflectionUtil.CommonAssembly.GetManifestResourceStream(path);
            if (stream == null)
                LogUtil.Log(new ExceptionPlus("读取资源文件失败：" + path));
            return stream;
        }

        /// <summary>
        /// 生成读取器
        /// </summary>
        public static ResourceReader GetReader(Stream stream)
        => new ResourceReader(stream);

        /// <summary>
        /// 获取读取器数据
        /// </summary>
        public static byte[] ReadBytes(this ResourceReader reader, string path)
        {
            reader.GetResourceData(path, out _, out byte[] data);
            return data;
        }
    }
}
