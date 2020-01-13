using System.IO;

namespace CommonUtils
{
    /// <summary>
    /// 文件系统，文件和文件夹的基类处理
    /// </summary>
    public static class FileSystem
    {
        public static bool IsFile(string path)
        => FileUtil.Exists(path);

        public static bool IsFloder(string path)
       => FloderUtil.Exists(path);

        /// <summary>
        /// 不为空，且存在
        /// </summary>
        public static bool IsNotNullAndExists(this FileSystemInfo file)
        => file != null && file.Exists;

        /// <summary>
        /// 不为空，且存在
        /// </summary>
        public static bool IsNotNullAndExists(string path)
        => FileUtil.GetInfo(path).IsNotNullAndExists() || FloderUtil.GetInfo(path).IsNotNullAndExists();

        /// <summary>
        /// 为空或不存在
        /// </summary>
        public static bool IsNullOrNotExists(this FileSystemInfo file)
        => file == null || !file.Exists;

        /// <summary>
        /// 为空或不存在
        /// </summary>
        public static bool IsNullOrNotExists(string path)
        => FileUtil.GetInfo(path).IsNullOrNotExists() && FloderUtil.GetInfo(path).IsNullOrNotExists();
    }
}
