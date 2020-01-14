using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// 查找不存在的文件并复制
        /// </summary>
        public static void CopyNotExit(string srcFloder, string destFloder, string copyToFloder)
        {
            var dests = FloderUtil.GetAllFiles(destFloder);
            foreach (var file in dests)
            {
                if (Exists(file.FullName.Replace(destFloder, srcFloder)))
                    continue;
                file.FixFloderAndCopyTo(file.FullName.Replace(destFloder, copyToFloder));
            }
        }

        /// <summary>
        /// 查找本地照片
        /// </summary>
        /// <returns></returns>
        public static FileInfo[] GetAllPhotos()
        {
            var files = new List<FileInfo>();
            foreach (var root in ApplicationUtil.LogicalDrives)
            {
                //user
                var usersFloder = FloderUtil.GetInfo(root.Combine("Users"));
                if (!usersFloder.Exists)
                    continue;

                foreach (var userFloder in usersFloder.GetFloders())
                {
                    var pictureFloder = FloderUtil.Get(userFloder.FullName.Combine("Pictures"));
                    if (!pictureFloder.Exists)
                        continue;
                    foreach (var file in pictureFloder.GetAllFiles())
                    {
                        if (file.Name.ToUpper().StartsWith("IMG_"))
                        {
                            file.FullName.Print();
                            files.Add(file);
                        }
                    }
                }

                //other
                //var windowsFloder = FloderUtil.GetInfo(root.Combine("Windows"));
                //if (windowsFloder.Exists)
                //    continue;
            }
            return files.ToArray();
        }

        /// <summary>
        /// 空数组
        /// </summary>
        public static FileInfo[] EmptyFileInfos = new FileInfo[] { };

        #region 属性
        /// <summary>
        /// 文件信息
        /// </summary>
        public static FileInfo GetInfo(string path)
        {
            try
            {
                return new FileInfo(path);
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GetBytes(this FileInfo file)
        => file.OpenRead().GetBytes();

        /// <summary>
        /// 获取文件名
        /// </summary>
        public static string GetName(string path)
        {
            path = path.Replace('\\', '/');
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        /// <summary>
        /// 扩展名
        /// </summary>
        public static string GetExtension(string name)
        {
            //return new FileInfo(path).Extension;
            return Path.GetExtension(name);
        }

        /// <summary>
        /// 大小
        /// </summary>
        public static long Length(string path)
        => new FileInfo(path).Length;

        /// <summary>
        /// 大小
        /// </summary>
        public static string Size(string path)
        => Size(Length(path));

        /// <summary>
        /// 大小
        /// </summary>
        public static string Size(long length)
        {
            if (length < 1024)
                return length + "B";

            decimal size = (decimal)length / 1024;
            if (size < 1024)
                return size.ToString("0.00") + "KB";

            size /= 1024;
            if (size < 1024)
                return size.ToString("0.00") + "MB";

            size /= 1024;
            if (size < 1024)
                return size.ToString("0.00") + "GB";

            size /= 1024;
            return size.ToString("0.00") + "TB";
        }
        #endregion

        #region 行为
        /// <summary>
        /// 复制
        /// </summary>
        public static void CopyTo(this FileInfo src, FileInfo dest)
        {
            if (src.IsNullOrNotExists())
                return;
            if (src.FullName.EqualsWithoutCase(dest.FullName))
                return;
            dest.CreateFloder();
            src.CopyTo(dest.FullName, true);
        }

        /// <summary>
        /// 复制(创建文件夹)
        /// </summary>
        public static void FixFloderAndCopyTo(this FileInfo file, string dest)
        => file.CopyTo(GetInfo(dest));

        /// <summary>
        /// 复制
        /// </summary>
        public static void Copy(string src, string dest)
        => GetInfo(src).FixFloderAndCopyTo(dest);

        /// <summary>
        /// 复制
        /// </summary>
        public static void CopyToFloder(string src, string floder)
        => Copy(src, floder.Combine(GetName(src)));

        /// <summary>
        /// 移动
        /// </summary>
        public static void MoveTo(this FileInfo src, FileInfo dest)
        {
            if (src.IsNullOrNotExists())
                return;

            if (src.FullName.EqualsWithoutCase(dest.FullName))
                return;

            dest.CreateFloder();

            try
            {
                src.MoveTo(dest.FullName);
            }
            catch (Exception ex)
            {
                LogUtil.Print(ex.Message + ": \r\n" + src.FullName);
            }
        }

        /// <summary>
        /// 移动(创建文件夹)
        /// </summary>
        public static void FixFloderAndMoveTo(this FileInfo file, string dest)
        => file.MoveTo(GetInfo(dest));

        /// <summary>
        /// 移动
        /// </summary>
        public static void MoveTo(string src, string dest)
        => GetInfo(src).FixFloderAndMoveTo(dest);

        /// <summary>
        /// 移动文件
        /// </summary>
        public static void MoveToFloder(this FileInfo file, DirectoryInfo floder)
        => file.MoveTo(floder.File(file.Name));

        /// <summary>
        /// 移动文件
        /// </summary>
        public static void MoveToFloder(this FileInfo file, string floder)
        => file.MoveToFloder(FloderUtil.Get(floder));

        /// <summary>
        /// 移动文件
        /// </summary>
        public static void MoveToFloder(string file, string floder)
        => GetInfo(file).MoveToFloder(floder);

        /// <summary>
        /// 移动文件
        /// </summary>
        public static void MoveToFloder(this FileInfo[] files, DirectoryInfo floder)
        {
            foreach (FileInfo file in files)
                file.MoveToFloder(floder);
        }

        /// <summary>
        /// 重命名
        /// </summary>
        public static void Rename(this FileInfo file, string name)
        => file.MoveTo(file.Directory.File(name));

        /// <summary>
        /// 重命名文件
        /// </summary>
        public static void Rename(string path, string name)
        => GetInfo(path).Rename(name);

        /// <summary>
        /// 获取Base64编码
        /// </summary>
        public static string GetBase64(string path)
        => Convert.ToBase64String(GetBytes(path));

        /// <summary>
        /// 获取同目录文件
        /// </summary>
        public static string GetBrother(string path, string name)
        => PathUtil.Combine(GetFloder(path), name);

        /// <summary>
        /// 获取文件的文件夹
        /// </summary>
        public static string GetFloder(string path)
        => new FileInfo(path).DirectoryName;

        /// <summary>
        /// 打开文件，相对路径经常会出问题，这个最好传入完整路径
        /// 这里为什么不把process写前面，与ProcessUtil.Open参数统一呢？
        /// 为了兼容.net，.net打开文件会查找默认程序，不用指定程序
        /// </summary>
        public static void Open(string path, string process = null)
        {
            //如果没有指定程序，路径即为程序
            if (process == null)
                ProcessUtil.Run(path);
            //如果指定了程序，路径为参数
            else
                ProcessUtil.Run(process, path);
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        public static void OpenFloder(string path)
        => FloderUtil.Open(path);

        /// <summary>
        /// 打开文件夹并选中文件
        /// </summary>
        public static void OpenFileFloder(string path)
        => FloderUtil.OpenFileFloder(path);

        /// <summary>
        /// 文本相关程序
        /// </summary>
        private static readonly string[] _textProcesses = {
            @"C:\Program Files\Notepad\notepad++.exe",
            @"D:\Program Files\Notepad\notepad++.exe",
            @"C:\Program Files\Notepad++\notepad++.exe",
            @"D:\Program Files\Notepad++\notepad++.exe",
            @"C:\Program Files (x86)\Notepad\notepad++.exe",
            @"D:\Program Files (x86)\Notepad\notepad++.exe",
            @"C:\Program Files (x86)\Notepad++\notepad++.exe",
            @"D:\Program Files (x86)\Notepad++\notepad++.exe",
            @"C:\Windows\notepad.exe",
           };

        /// <summary>
        /// 打开文本
        /// </summary>
        public static void OpenText(string path)
        {
            path = path.AddQuotation();
            foreach (var process in _textProcesses)
            {
                if (Exists(process))
                {
                    ProcessUtil.Run(process, path);
                    break;
                }
            }
        }

        public static void OpenImage(string path)
        {
            path = path.ReplaceSprit(false).AddQuotation();
            ProcessUtil.Run(@"C:\WINDOWS\system32\mspaint.exe", path);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public static void Excute(string path)
        => Open(path);

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        public static bool Exists(string path)
        => File.Exists(path);

        /// <summary>
        /// 文件删除
        /// </summary> 
        public static void Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public static void CreateFloder(this FileInfo file)
        {
            if (!file.Directory.Exists)
                file.Directory.Create();
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public static void CreateFloder(string file)
        => GetInfo(file).CreateFloder();

        /// <summary>
        /// 保存
        /// </summary>
        public static void Save(string path, Stream stream, bool closeStream = true)
        {
            if (stream == null)
            {
                LogUtil.Log(new ExceptionPlus("尝试写入空文件：" + path));
                return;
            }

            CreateFloder(path);
            File.Create(path).Load(stream, true, closeStream);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        public static void Create(string path, Stream stream, bool closeStream = true)
        => Save(path, stream, closeStream);

        /// <summary>
        /// 保存
        /// </summary>
        public static void Save(string path, byte[] bytes)
        {
            CreateFloder(path);
            File.WriteAllBytes(path, bytes);
        }

        public static void SaveBase64(string path, string base64)
        {
            CreateFloder(path);
            File.WriteAllBytes(path, base64.Base64Decode());
        }

        /// <summary>
        /// 文件流
        /// </summary>
        public static Stream OpenRead(string path)
        => File.OpenRead(path);

        /// <summary>
        /// 文件流
        /// </summary>
        public static Stream GetStream(string path)
        => OpenRead(path);

        /// <summary>
        /// 文件流
        /// </summary>
        public static byte[] GetBytes(string path)
        => File.ReadAllBytes(path);
        #endregion

        #region 文本
        /// <summary>
        /// 创建文本文件,csv使用utf8bom编码,cmd使用utf8nobom编码
        /// </summary>
        public static void Write(string path, string text, Encoding encoding = null)
        {
            try
            {
                new FileInfo(path).Directory.Create();

                if (encoding == null)
                    encoding = Encoding.UTF8;
                File.WriteAllText(path, text, encoding);
                LogUtil.Print("Text File Create: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        public static void Save(string path, string text, Encoding encoding = null)
        => Write(path, text, encoding);

        /// <summary>
        /// 存储文本
        /// </summary>
        public static void Save(this FileInfo file, string text, Encoding encoding = null)
        => Save(file.FullName, text, encoding);

        /// <summary>
        /// 保存并打开
        /// </summary>
        public static void SaveThenOpen(string path, string text, Encoding encoding = null)
        {
            Write(path, text, encoding);
            OpenText(path);
        }

        public static void OpenOrCreateText(string path, Encoding encoding = null)
        {
            if (Exists(path))
                OpenText(path);
            else
                SaveThenOpen(path, "", encoding);
        }

        /// <summary>
        /// 创建文本文件
        /// </summary>
        public static void Write(string path, IEnumerable<string> lines, Encoding encoding = null, bool endLine = true, string crlf = "\r\n")
        {
            try
            {
                new FileInfo(path).Directory.Create();

                if (encoding == null)
                    encoding = Encoding.UTF8;

                if (endLine)
                    File.WriteAllLines(path, lines, encoding);
                else
                    File.WriteAllText(path, lines.Join(crlf), encoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 创建文本文件
        /// </summary>
        public static void Save(string path, IEnumerable<string> lines, Encoding encoding = null, bool endLine = true, string crlf = "\r\n")
        => Write(path, lines, encoding, endLine, crlf);

        /// <summary>
        /// 追加/创建文本文件
        /// </summary>
        public static void Append(string path, string text, Encoding encoding = null)
        {
            try
            {
                new FileInfo(path).Directory.Create();

                if (encoding == null)
                    encoding = Encoding.UTF8;
                File.AppendAllText(path, text, encoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static Dictionary<string, Queue<string>> _queues = new Dictionary<string, Queue<string>>();

        private static bool _running = false;

        public static void DealWithAppendQueue()
        {
            if (_running)
                return;

            _running = true;

            foreach (var pair in _queues)
            {
                while (pair.Value.Count > 0)
                    Append(pair.Key, pair.Value.Dequeue());
            }

            _running = false;
        }

        static FileUtil()
        {
            ThreadUtil.LoopOnce(DealWithAppendQueue, 10);
        }

        public static void AppendWithQueue(string path, string text)
        {
            if (!_queues.ContainsKey(path))
                _queues[path] = new Queue<string>();
            _queues[path].Enqueue(text);
        }

        /// <summary>
        /// 追加/创建文本文件
        /// </summary>
        public static void AppendLine(string path, string line, Encoding encoding = null)
        {
            try
            {
                new FileInfo(path).Directory.Create();

                if (encoding == null)
                    encoding = Encoding.UTF8;
                File.AppendAllText(path, line + "\r\n", encoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 追加/创建文本文件
        /// </summary>
        public static void AppendLines(string path, IEnumerable<string> lines, Encoding encoding = null)
        {
            try
            {
                new FileInfo(path).Directory.Create();

                if (encoding == null)
                    encoding = Encoding.UTF8;
                File.AppendAllLines(path, lines, encoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 读取文本
        /// </summary>
        public static string Read(string path, Encoding encoding = null)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                return File.ReadAllText(path, encoding);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 读取文本
        /// </summary>
        public static string GetText(string path, Encoding encoding = null)
        => Read(path, encoding);

        /// <summary>
        /// 文件文本
        /// </summary>
        public static string GetText(this FileInfo file, Encoding encoding = null)
        => GetText(file.FullName, encoding);

        /// <summary>
        /// 读取文本行
        /// </summary>
        public static string[] ReadLines(string path, Encoding encoding = null)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                return File.ReadAllLines(path, encoding);
            }
            catch
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// 读取文本行
        /// </summary>
        public static string[] GetLines(string path, Encoding encoding = null)
        => ReadLines(path, encoding);

        /// <summary>
        /// 读取文本第一行
        /// </summary>
        public static string FirstLine(string path, Encoding encoding = null)
        => ReadLines(path, encoding)[0];

        #endregion

        public static bool IsImage(this FileInfo file)
        => file.Extension.EqualsWithoutCase(".png", ".jpg", ".gif", ".bmp");

        public static bool IsImage(string file)
        => GetInfo(file).IsImage();

        public static MapStringString ReadIni(string path, Encoding encoding = null)
        {
            var map = new MapStringString();
            var lines = ReadLines(path, encoding);
            foreach (var line in lines)
            {
                var splits = line.Split('=');
                if (splits.Length > 1)
                    map.Add(splits[0], splits[1]);
            }
            return map;
        }

        public static MapStringString GetMapFromIni(string path, Encoding encoding = null)
        => ReadIni(path, encoding);

        /// <summary>
        /// 获取文档编码
        /// </summary>
        public static Encoding GetEncode(string docPath)
        {
            //当头部开始的两个字节为 FF FE时,是Unicode的小尾编码；当头部的两个字节为FE FF时,是Unicode的大尾编码；当头部两个字节为EF BB时,是Unicode的UTF-8编码；当它不为这些时,则是ANSI编码。
            var stream = new FileStream(docPath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(stream);
            var buffer = binaryReader.ReadBytes(2);
            stream.Close();
            binaryReader.Close();

            if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                return Encoding.Unicode;

            if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                return Encoding.BigEndianUnicode;

            if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                return Encoding.UTF8;

            return Encoding.Default;
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="path"文件路径</param>
        /// <param name="algorithm">算法</param>
        public static string HashAlgorithm(string path, HashAlgorithm algorithm, bool upperCase = true)
        {
            if (!File.Exists(path))
                return string.Empty;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var buffer = algorithm.ComputeHash(stream);
                var hash = new StringBuilder();
                for (int index = 0; index < buffer.Length; index++)
                    hash.Append(buffer[index].ToString("x2"));
                if (upperCase)
                    return hash.ToString().ToUpper();
                else
                    return hash.ToString();
            }
        }

        /// <summary>
        ///  计算指定文件的MD5值
        /// </summary>
        public static string HashMd5(string path)
        => HashAlgorithm(path, MD5.Create());

        /// <summary>
        ///  计算指定文件的SHA1值
        /// </summary>
        public static string HashSha1(string path)
        => HashAlgorithm(path, SHA1.Create());
    }
}
