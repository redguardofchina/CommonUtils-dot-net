//using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace CommonUtils
{
    /// <summary>
    /// 文件系统，文件和文件夹的基类处理
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// 不为空，且存在
        /// </summary>
        public static bool IsNotNullAndExists(this FileSystemInfo file)
        {
            return file != null && file.Exists;
        }

        /// <summary>
        /// 不为空，且存在
        /// </summary>
        public static bool IsNotNullAndExists(string path)
        {
            return FileUtil.GetInfo(path).IsNotNullAndExists() || FloderUtil.GetInfo(path).IsNotNullAndExists();
        }

        /// <summary>
        /// 为空或不存在
        /// </summary>
        public static bool IsNullOrNotExists(this FileSystemInfo file)
        {
            return file == null || !file.Exists;
        }

        /// <summary>
        /// 为空或不存在
        /// </summary>
        public static bool IsNullOrNotExists(string path)
        {
            return FileUtil.GetInfo(path).IsNullOrNotExists() && FloderUtil.GetInfo(path).IsNullOrNotExists();
        }

        ///// <summary>
        ///// 删除文件到回收站
        ///// </summary>
        //public static void DeleteToBin(this FileInfo file)
        //{
        //    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        //}

        ///// <summary>
        ///// 删除文件夹到回收站
        ///// </summary>
        //public static void DeleteToBin(this DirectoryInfo floder)
        //{
        //    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(floder.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        //}
    }
}
