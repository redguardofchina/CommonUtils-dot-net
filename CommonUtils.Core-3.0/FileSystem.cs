using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace CommonUtils
{
    /// <summary>
    /// 文件系统，文件和文件夹的基类处理
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// 删除文件到回收站
        /// </summary>
        public static void DeleteToBin(this FileInfo file)
        => Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

        /// <summary>
        /// 删除文件夹到回收站
        /// </summary>
        public static void DeleteToBin(this DirectoryInfo floder)
        => Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(floder.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
    }
}
