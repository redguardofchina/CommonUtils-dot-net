using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 文件夹操作
    /// </summary>
    public static class FloderUtil
    {
        /// <summary>
        /// 获取文件夹
        /// </summary>
        public static DirectoryInfo[] GetFloders(this DirectoryInfo floder)
        => floder.GetDirectories();

        /// <summary>
        /// 相对路径 不以/开头，不以/结尾
        /// </summary>
        public static string Relative(this DirectoryInfo floder, string path)
        {
            var name = path.Replace('\\', '/').Remove(floder.FullName.Replace('\\', '/'));
            if (name.StartWith('/'))
                name = name.Substring(1);
            if (!name.EndWith('/'))
                name += '/';
            return name;
        }

        /// <summary>
        /// 相对路径 不以/开头，不以/结尾
        /// </summary>
        public static string GetSubRelativePath(this DirectoryInfo floder, FileSystemInfo file)
        => floder.Relative(file.FullName);

        /// <summary>
        /// 判断文件夹下的子文件或子文件夹是否存在
        /// </summary>
        public static bool Exists(this DirectoryInfo floder, string relativePath)
        => floder.File(relativePath).Exists || floder.Floder(relativePath).Exists;

        /// <summary>
        /// 路径拼接
        /// </summary>
        public static string Combine(this DirectoryInfo floder, params string[] right)
        => floder.FullName.Combine(right);

        /// <summary>
        /// 获取子文件夹
        /// </summary>
        public static string Child(this DirectoryInfo floder, string relativePath)
        => floder.Combine(relativePath);

        /// <summary>
        /// 文件夹下的文件
        /// </summary>
        public static FileInfo File(this DirectoryInfo floder, string relativePath)
        => FileUtil.GetInfo(floder.Child(relativePath));

        /// <summary>
        /// 获取子文件夹
        /// </summary>
        public static DirectoryInfo Floder(this DirectoryInfo floder, string relativePath)
        => GetInfo(floder.Child(relativePath));

        /// <summary>
        /// 获取所有文件
        /// </summary>
        public static FileInfo[] GetAllFiles(this DirectoryInfo floder)
        {
            var files = new List<FileInfo>();
            try
            {
                files.AddRange(floder.GetFiles());
                foreach (var subFloder in floder.GetDirectories())
                    files.AddRange(subFloder.GetAllFiles());
            }
            catch { }
            return files.ToArray();
        }

        public static FileInfo[] GetAllFiles(string floder)
        => Get(floder).GetAllFiles();

        public static List<DirectoryInfo> GetAllFloders(this DirectoryInfo floder)
        {
            var floders = new List<DirectoryInfo>();
            var temp = floder.GetDirectories();
            floders.AddRange(temp);
            foreach (var subFloder in temp)
                floders.AddRange(subFloder.GetAllFloders());
            return floders;
        }

        public static List<DirectoryInfo> GetAllFloders(string floder)
        => Get(floder).GetAllFloders();

        /// <summary>
        /// 文件夹是否为空
        /// </summary>
        public static bool IsEmpty(this DirectoryInfo floder)
        => floder.GetFileSystemInfos().Length == 0;

        public static bool HasNoFile(this DirectoryInfo floder)
        => floder.GetAllFiles().Length == 0;

        public static void TryToDelete(this DirectoryInfo floder)
        {
            try
            {
                floder.Delete();
            }
            catch (Exception ex)
            {
                LogUtil.Print(ex.Message + ": \r\n" + floder.FullName);
            }
        }

        /// <summary>
        /// 获取所有空文件夹
        /// </summary>
        public static DirectoryInfo[] GetEmptyFloders(this DirectoryInfo floder)
        {
            var list = new List<DirectoryInfo>();
            foreach (var subFloder in floder.GetDirectories())
                if (subFloder.IsEmpty())
                    list.Add(subFloder);
                else
                    list.AddRange(subFloder.GetEmptyFloders());
            return list.ToArray();
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        public static void Open(string path)
        => ProcessUtil.Run(@"C:\Windows\explorer.exe", path.ReplaceSprit(false).AddQuotation());

        /// <summary>
        /// 打开文件夹并选中文件
        /// </summary>
        public static void OpenFileFloder(string path)
        => ProcessUtil.Run(@"C:\Windows\explorer.exe", "/select," + path.ReplaceSprit(false).AddQuotation());

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public static void Create(string path)
        => Directory.CreateDirectory(path);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        public static bool Exists(string path)
        => Directory.Exists(path);

        /// <summary>
        /// 删除文件夹
        /// </summary>
        public static void Delete(string path)
        {
            try
            {
                if (Exists(path))
                    Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        public static void DeleteRecursive(this DirectoryInfo floder)
        => floder.Delete(true);

        /// <summary>
        /// 删除空文件
        /// </summary>
        public static void DeleteEmpty(string path)
        => Get(path).DeleteEmpty();

        /// <summary>
        /// 删除空文件
        /// </summary>
        public static void DeleteEmpty(this DirectoryInfo floder)
        {
            if (floder.HasNoFile())
            {
                floder.TryToDelete();
            }
            else
            {
                foreach (var subFloder in floder.GetFloders())
                    subFloder.DeleteEmpty();
            }
        }

        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        public static DirectoryInfo GetInfo(string path)
        {
            if (path.IsNullOrEmpty())
                return null;
            return new DirectoryInfo(path);
        }

        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        public static DirectoryInfo Get(string path)
        => GetInfo(path);

        public static FileInfo[] GetFiles(string path)
        {
            var floder = GetInfo(path);
            if (floder.Exists)
                return floder.GetFiles();
            return FileUtil.EmptyFileInfos;
        }

        /// <summary>
        /// 文件夹中的文件最后修改时间
        /// </summary>
        /// <param name="sub">包含子文件夹</param>
        public static DateTime LastFileWriteTime(this DirectoryInfo floder, bool sub = true)
        {
            DateTime time = DateTime.MinValue;
            foreach (var file in floder.GetFiles())
                if (file.LastWriteTime > time)
                    time = file.LastWriteTime;
            if (sub)
            {
                foreach (var subFloder in floder.GetDirectories())
                {
                    var subTime = subFloder.LastFileWriteTime(sub);
                    if (subTime > time)
                        time = subTime;
                }
            }
            return time;
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sub">包含子文件夹</param>
        public static void CopyTo(this DirectoryInfo src, DirectoryInfo dest, bool sub = true, params string[] ignores)
        {
            if (src.IsNullOrNotExists())
                return;

            //创建文件夹，兼容了空文件夹
            dest.Create();

            foreach (var file in src.GetFiles())
            {
                if (ignores.Contains(file.Name))
                    continue;
                file.CopyTo(dest.File(file.Name));
            }

            if (!sub)
                return;

            foreach (var floder in src.GetDirectories())
            {
                if (ignores.Contains(floder.Name))
                    continue;
                floder.CopyTo(dest.Floder(floder.Name), sub);
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sub">包含子文件夹</param>
        public static void CopyTo(this DirectoryInfo src, string dest, bool sub = true)
        => src.CopyTo(GetInfo(dest), sub);

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sub">包含子文件夹</param>
        public static void Copy(string src, string dest, bool sub = true)
        => GetInfo(src).CopyTo(dest, sub);

        /// <summary>
        /// 移动、覆盖、合并文件夹
        /// 虽然和复制很像，但确实不是复制，文件进行的移动操作，比复制效率高
        /// </summary>
        public static void MoveToEx(this DirectoryInfo src, string dest)
        {
            //src.MoveTo(dest);不支持合并覆盖，只支持移动

            if (src.IsNullOrNotExists())
                return;

            //创建文件夹，兼容了空文件夹
            Create(dest);

            foreach (var file in src.GetFiles())
                file.FixFloderAndMoveTo(Path.Combine(dest, file.Name));

            foreach (var floder in src.GetDirectories())
                floder.MoveToEx(Path.Combine(dest, floder.Name));
        }

        /// <summary>
        /// 移动、覆盖、合并文件夹
        /// </summary>
        public static void Move(string from, string to)
        => GetInfo(from).MoveToEx(to);

        /// <summary>
        /// 删除不对称的文件
        /// </summary>
        /// <param name="sub">包含子文件夹</param>
        public static void DeleteNotExist(this DirectoryInfo src, DirectoryInfo dest, bool sub = true, params string[] ignores)
        {
            if (src.IsNullOrNotExists() || dest.IsNullOrNotExists())
                return;

            foreach (var file in dest.GetFiles())
            {
                if (ignores.Contains(file.Name))
                    continue;
                if (!src.Exists(dest.GetSubRelativePath(file)))
                    file.Delete();
            }

            if (!sub)
                return;

            foreach (var floder in dest.GetFloders())
            {
                if (ignores.Contains(floder.Name))
                    continue;
                if (!src.Exists(dest.GetSubRelativePath(floder)))
                    floder.DeleteRecursive();
                else
                    DeleteNotExist(src.Floder(dest.GetSubRelativePath(floder)), floder, sub);
            }
        }
    }
}
