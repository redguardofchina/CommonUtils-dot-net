using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// zip工具
    /// </summary>
    public static class ZipUtil
    {
        /// <summary>
        /// 获取压缩包中的文件
        /// </summary>
        public static ReadOnlyCollection<ZipArchiveEntry> GetFiles(string path, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            var zipFile = ZipFile.Open(path, ZipArchiveMode.Read, encoding);
            return zipFile.Entries;
        }

        /// <summary>
        /// 获取压缩包中的文件
        /// </summary>
        public static ReadOnlyCollection<ZipArchiveEntry> GetFiles(Stream stream, Encoding encoding = null)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                ZipArchive zipFile = new ZipArchive(stream, ZipArchiveMode.Read, false, encoding);
                return zipFile.Entries;
            }
            catch (Exception ex)
            {
                ex.Message.Print();
                return null;
            }
        }

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        public static void Create(string floder, string path, bool includeBaseDirectory = true, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            ZipFile.CreateFromDirectory(floder, path, compressionLevel, includeBaseDirectory);
        }

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        public static Stream CreateStream(Map<string, Stream> streams)
        {
            try
            {
                var memory = new MemoryStream();
                var zip = new ZipArchive(memory, ZipArchiveMode.Create, true);
                foreach (var nameStream in streams)
                {
                    var entry = zip.CreateEntry(nameStream.Key);
                    entry.Open().Load(nameStream.Value, true, true);
                }
                //这里不释放，文件下载完会出错
                zip.Dispose();
                memory.Seek();
                return memory;
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return default;
            }
        }

        public static Stream Create(Map<string, Stream> streams)
        => CreateStream(streams);

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        public static Stream CreateZip(this DirectoryInfo floder, bool includeBaseDirectory = true, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            var memory = new MemoryStream();
            var zip = new ZipArchive(memory, ZipArchiveMode.Create, true);

            //添加文件
            foreach (var file in floder.GetAllFiles())
            {
                var entryName = floder.GetSubRelativePath(file);
                if (includeBaseDirectory)
                    entryName = floder.Name.Combine(entryName);
                var entry = zip.CreateEntry(entryName, compressionLevel);
                entry.Open().LoadFile(file.FullName, true);
            }

            //空文件夹也要保留
            foreach (var subFloder in floder.GetEmptyFloders())
            {
                var entryName = floder.GetSubRelativePath(subFloder);
                if (includeBaseDirectory)
                    entryName = floder.Name.Combine(entryName);
                zip.CreateEntry(entryName, compressionLevel);
            }

            //这里不释放，文件下载结束时,会报错
            zip.Dispose();
            memory.Seek();
            return memory;
        }

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        public static void CreateZip(this DirectoryInfo floder, string file, bool includeBaseDirectory = true, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            Create(floder.FullName, file, includeBaseDirectory, compressionLevel);
        }
    }
}
