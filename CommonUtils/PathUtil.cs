using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 路径处理
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// 基础路径 带/
        /// </summary>
        public static string Base { get; }

        /// <summary>
        /// 当前程序目录 同Base
        /// </summary>
        public static string Current => Base;

        /// <summary>
        /// 项目路径 仅用于开发环境
        /// </summary>
        public static string ProjectRoot { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        static PathUtil()
        {
            Base = Environment.CurrentDirectory.ReplaceSprit();
            Base = AppDomain.CurrentDomain.BaseDirectory.ReplaceSprit();
            ProjectRoot = Base.SubstringStartByFirst("/bin/");
        }

        /// <summary>
        /// 全路径
        /// </summary>
        public static string GetFull(params string[] partPaths)
        => Base.Combine(partPaths);

        /// <summary>
        /// 项目路径 仅用于开发环境
        /// </summary>
        public static string GetProjectFull(params string[] partPaths)
        => ProjectRoot.Combine(partPaths);

        /// <summary>
        /// 通过磁盘路径，获取虚拟路径
        /// </summary>
        public static string GetVirtual(string fullPath)
        {
            string rootPath = Base.Replace('\\', '/');
            fullPath = fullPath.Replace('\\', '/');
            return "/" + fullPath.Replace(rootPath, "");
        }

        /// <summary>
        /// 临时路径
        /// </summary>
        public static string GetTemp(string nameOrExt)
        {
            //以/开头，如果没使用本工具的GetFull方法，会定位到磁盘根目录，所以不能以/开头
            if (nameOrExt.StartWith('.'))
                nameOrExt = StringUtil.LongTimestamp(nameOrExt);
            return GetFull("temp", nameOrExt);
        }

        /// <summary>
        /// 路径合并 \→/
        /// </summary>
        public static string Combine(this string left, params string[] rights)
        {
            //统一字符
            StringBuilder newLeft = new StringBuilder(left.Replace('\\', '/'));
            foreach (var value in rights)
            {
                //忽略空
                if (string.IsNullOrEmpty(value))
                    continue;

                //统一字符
                var right = value.Replace('\\', '/');
                // ../../处理 System.IO.Path.Combine(paths)如果right是/开头，那么left就会丢失，../也不会处理
                int up = right.CountOf("../");
                right = right.Remove("../", "./");
                //寻找上级
                if (up > 0)
                {
                    var tempLeft = newLeft.ToString().ReplaceRecursive("//", "/");
                    if (tempLeft[tempLeft.Length - 1] == '/')
                        tempLeft = tempLeft.Substring(0, tempLeft.Length - 1);
                    while (up-- > 0)
                        tempLeft = tempLeft.Substring(0, tempLeft.LastIndexOf('/'));
                    newLeft = new StringBuilder(tempLeft);
                }

                //忽略空 因为right有改变，所以这里再判断一遍
                if (string.IsNullOrEmpty(right))
                    continue;

                //判断是否有隔离符
                var leftHas = newLeft[newLeft.Length - 1] == '/';
                var rightHas = right[0] == '/';

                //都有
                if (leftHas && rightHas)
                {
                    newLeft.Append(right.Substring(1));
                    continue;
                }

                //一个有
                if (leftHas || rightHas)
                {
                    newLeft.Append(right);
                    continue;
                }

                //都没有
                if (!leftHas && !rightHas)
                {
                    newLeft.Append('/');
                    newLeft.Append(right);
                }
            }
            return newLeft.ToString();
        }

        /// <summary>
        /// 后缀前名字后附加
        /// </summary>
        public static string NameAppend(string path, string append)
        {
            var index = path.LastIndexOf('.');
            if (index < 0)
                return path + append;
            return path.Substring(0, index) + append + path.Substring(index);
        }

        /// <summary>
        /// 更改后缀名
        /// </summary>
        public static string ChangeExtension(string path, string extension)
        {
            var index = path.LastIndexOf('.');
            if (index < 0)
                return path + extension;
            return path.Substring(0, index) + extension;
        }

        /// <summary>
        /// 替换后缀名
        /// </summary>
        public static string ReplaceExtention(string path, string extension)
        => ChangeExtension(path, extension);

        /// <summary>
        /// 枚举路径源
        /// </summary>
        public static string EnumSources()
        {
            var sb = new StringBuilder();
            var list = new List<KeyValue>();
            list.Add("Base", Base);
            list.Add("ProjectRoot", ProjectRoot);
            list.Add("Current", Current);
            foreach (var item in list.OrderBy(m => m.Value).ToArray())
                sb.AppendFormatLine("[{0}] ({1})", item.Value, item.Key);
            sb.AppendLine();

            list = new List<KeyValue>();
            list.Add("AppDomain.CurrentDomain.BaseDirectory", AppDomain.CurrentDomain.BaseDirectory);
            list.Add("Environment.SystemDirectory", Environment.SystemDirectory);
            list.Add("Environment.CurrentDirectory", Environment.CurrentDirectory);
            list.Add("Directory.GetCurrentDirectory()", Directory.GetCurrentDirectory());
            list.Add("Process.GetCurrentProcess().MainModule.FileName", Process.GetCurrentProcess().MainModule.FileName);
            foreach (var item in list.OrderBy(m => m.Value).ToArray())
                sb.AppendFormatLine("[{0}] ({1})", item.Value, item.Key);
            sb.AppendLine();

            //不合法
            //Directory.GetDirectoryRoot("");
            //Directory.GetDirectoryRoot(" ");
            //new FileInfo("");
            //new FileInfo(" ");
            //new DirectoryInfo("");
            //new DirectoryInfo(" ");

            list = new List<KeyValue>();
            list.Add("Directory.GetDirectoryRoot(\"/\")", Directory.GetDirectoryRoot("/"));
            list.Add("Directory.GetDirectoryRoot(\"./\")", Directory.GetDirectoryRoot("./"));
            list.Add("Directory.GetDirectoryRoot(\"hello\")", Directory.GetDirectoryRoot("hello"));
            foreach (var item in list.OrderBy(m => m.Value).ToArray())
                sb.AppendFormatLine("[{0}] ({1})", item.Value, item.Key);
            sb.AppendLine();

            list = new List<KeyValue>();
            list.Add("new FileInfo(\"/\").FullName", new FileInfo("/").FullName);
            list.Add("new FileInfo(\"./\").FullName", new FileInfo("./").FullName);
            list.Add("new FileInfo(\"hello\").FullName", new FileInfo("hello").FullName);
            foreach (var item in list.OrderBy(m => m.Value).ToArray())
                sb.AppendFormatLine("[{0}] ({1})", item.Value, item.Key);
            sb.AppendLine();

            list = new List<KeyValue>();
            list.Add("new DirectoryInfo(\"/\").FullName", new DirectoryInfo("/").FullName);
            list.Add("new DirectoryInfo(\"./\").FullName", new DirectoryInfo("./").FullName);
            list.Add("new DirectoryInfo(\"hello\").FullName", new DirectoryInfo("hello").FullName);
            foreach (var item in list.OrderBy(m => m.Value).ToArray())
                sb.AppendFormatLine("[{0}] ({1})", item.Value, item.Key);
            sb.AppendLine();

            sb.Print();
            return sb.ToString();
        }
    }
}
