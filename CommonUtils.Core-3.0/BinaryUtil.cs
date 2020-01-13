using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CommonUtils
{
    /// <summary>
    /// 比特流，字节流，内存流处理（内存流优先，字节流容易崩）
    /// </summary>
    public static class BinaryUtil
    {
        private static T DeepClone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (object.ReferenceEquals(source, null))
            {
                return default;
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 通过十六进制字符串获取bytes,例如0x0A097A68616E672073616E100C1A0C58585858584071712E636F6D
        /// </summary>
        public static byte[] GetBytesFromHexadecimal(string text, bool containsHead = true)
        {
            int index = 0;
            if (containsHead)
                index = 2;
            string number;
            List<byte> buff = new List<byte>();
            while (true)
            {
                if (index >= text.Length)
                    break;
                number = text.Substring(index, 2);
                buff.Add((byte)Convert.ToInt32(number, 16));
                index += 2;
            }
            return buff.ToArray();
        }

        /// <summary>
        /// 16进制字符串
        /// </summary>
        public static string ToHexadecimalString(this ICollection<byte> bytes, bool containsHead = true)
        {
            var sb = new StringBuilder();
            foreach (var @byte in bytes)
            {
                var str = Convert.ToString(@byte, 16);
                if (str.Length == 1)
                    sb.Append('0');
                sb.Append(str);
            }
            if (containsHead)
                return "0x" + sb.ToString().ToUpper();
            else
                return sb.ToString().ToUpper(); ;
        }

        /// <summary>
        /// 16进制字符串
        /// </summary>
        public static string To16String(this ICollection<byte> bytes, bool containsHead = true)
        => ToHexadecimalString(bytes, containsHead);

        public static string GetBytesString(this ICollection<byte> bytes)
        {
            var builder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                if (builder.Length > 0)
                    builder.Append(',');
                builder.Append(@byte);
            }
            builder.Insert(0, "[");
            builder.Append("]");
            return builder.ToString();
        }

        public static string ToBytesString(this ICollection<byte> bytes)
        => GetBytesString(bytes);

        /// <summary>
        /// 判断值是否相等
        /// </summary>
        public static bool EqualValue(this byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
                return false;
            for (int index = 0; index < left.Length; index++)
                if (left[index] != right[index])
                    return false;
            return true;
        }

        /// <summary>
        /// 判断值是否相等
        /// </summary>
        public static bool EqualFllow(this byte[] left, byte[] right, int startIndex)
        {
            if (left.Length < startIndex + right.Length)
                return false;
            for (int index = 0; index < right.Length; index++)
                if (left[startIndex + index] != right[index])
                    return false;
            return true;
        }

        public static byte[][] SplitBy(this byte[] bytes, byte[] separator)
        {
            var list = new List<byte[]>();
            var indexs = bytes.IndexsOf(separator);
            indexs.Add(bytes.Length);//逻辑需要
            var startIndex = 0;
            foreach (var index in indexs)
            {
                var length = index - startIndex;
                if (length > 0)
                    list.Add(bytes.Take(startIndex, length));
                startIndex = index + separator.Length;
            }
            return list.ToArray();
        }

        public static List<int> IndexsOf(this byte[] bytes, byte[] separator)
        {
            var indexs = new List<int>();
            for (int index = 0; index < bytes.Length; index++)
            {
                if (bytes[index] == separator[0] && bytes.EqualFllow(separator, index))
                {
                    //index.Print();
                    indexs.Add(index);
                    index += separator.Length - 1;//接下来会++的，所以要-1
                }
            }
            return indexs;
        }

        /// <summary>
        /// 数组截取,非引用,需赋值
        /// </summary>
        public static byte[] Cut(this byte[] left, long length)
        {
            byte[] right = new byte[length];
            Array.Copy(left, right, length);
            return right;
        }

        /// <summary>
        /// 数组截取,非引用,需赋值
        /// </summary>
        public static byte[] CutAt(this byte[] left, long index)
        {
            byte[] right = new byte[left.Length - index];
            Array.Copy(left, index, right, 0, left.Length - index);
            return right;
        }

        /// <summary>
        /// 字节流拼接
        /// </summary>
        public static byte[] Append(this byte[] left, byte[] right)
        {
            var list = new List<byte>();
            list.AddRange(left);
            list.AddRange(right);
            return list.ToArray();
        }

        /// <summary>
        /// Seek，内存指针回到起点，用于指针在末尾内存流需要继续使用的情况
        /// </summary>
        public static void Seek(this Stream stream) => stream.Seek(0, SeekOrigin.Begin);

        public static void Reset(this Stream stream) => stream.Seek();

        /// <summary>
        /// 存储
        /// </summary>
        public static void SaveTo(this Stream stream, string path, bool closeStream = true)
        => FileUtil.Save(path, stream, closeStream);

        /// <summary>
        /// 存储文件
        /// </summary>
        public static void CreateFile(this Stream stream, string path, bool closeStream = true)
        => stream.SaveTo(path, closeStream);

        public static void SaveTo(this byte[] bytes, string path)
        => FileUtil.Save(path, bytes);

        /// <summary>
        /// 状态信息
        /// </summary>
        public static void StatePrint(this Stream stream)
        {
            JObject state = new JObject();
            state.Add("stream.CanTimeout", stream.CanTimeout);
            state.Add("stream.CanRead", stream.CanRead);
            state.Add("stream.CanWrite", stream.CanWrite);
            state.Add("stream.CanSeek", stream.CanSeek);
            if (stream.CanSeek)
            {
                state.Add("stream.Position", stream.Position);
                state.Add("stream.Length", stream.Length);
            }
            Console.WriteLine(state);
        }

        /// <summary>
        /// 序列化 typeof(value) 必须声明[Serializable]
        /// </summary>
        public static Stream Serialize(object value)
        {
            MemoryStream memory = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memory, value);
            //不可省略，不可用Flush代替
            memory.Seek();
            return memory;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static object Deserialize(Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            var value = binaryFormatter.Deserialize(stream);
            stream.Close();
            return value;
        }

        #region 解析

        /// <summary>
        /// 读取字符串
        /// </summary>
        public static string GetString(this Stream stream, Encoding encoding = null, bool close = true)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encoding);
            string text = reader.ReadToEnd();
            if (close)
            {
                reader.Close();
                stream.Close();
            }
            return text;
        }

        public static string ReadString(this Stream stream)
         => stream.GetString();

        public static string GetText(this Stream stream)
        => stream.GetString();

        public static string ReadText(this Stream stream)
       => stream.GetString();

        /// <summary>
        /// 文本行
        /// </summary>
        public static List<string> GetLines(this Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            List<string> lines = new List<string>();
            StreamReader reader = new StreamReader(stream, encoding);
            while (!reader.EndOfStream)
                lines.Add(reader.ReadLine());
            reader.Close();
            stream.Close();
            return lines;
        }

        /// <summary>
        /// 字符串
        /// </summary>
        public static string GetString(this byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        #endregion

        #region 转换
        /// <summary>
        /// 将bytes转换成bytes字符串
        /// </summary>
        public static string Join(byte[] bytes)
        {
            return bytes.Join(',');
        }

        /// <summary>
        /// 将bytes字符串转换成bytes
        /// </summary>
        public static byte[] Split(string bytesString)
        {
            string[] byteStrings = bytesString.Split(',');
            byte[] bytes = new byte[byteStrings.Length];
            for (int index = 0; index < bytes.Length; index++)
            {
                int byteInt = Convert.ToInt32(byteStrings[index]);
                bytes[index] = (byte)byteInt;
            }
            return bytes;
        }

        /// <summary>
        /// 内存流
        /// </summary>
        public static Stream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// 内存流
        /// </summary>
        public static MemoryStream ToMemory(this Stream stream, bool endStream = true)
        {
            var memory = new MemoryStream();
            memory.Load(stream, false, endStream);
            memory.Seek();
            return memory;
        }

        /// <summary>
        /// 字节流
        /// </summary>
        public static byte[] ToBytes(this Stream stream, bool closeSource = true)
        {
            var memory = new MemoryStream();
            memory.Load(stream, false, closeSource);
            var bytes = memory.ToArray();
            memory.Close();
            return bytes;
        }

        public static byte[] GetBytes(this Stream stream, bool closeSource = true)
        => stream.ToBytes(closeSource);
        #endregion

        #region 内存流装载
        /// <summary>
        /// 装载字符串，源Stream默认不关闭，如果需要关闭，传个end=true进来
        /// </summary>
        public static void Load(this Stream stream, string text, Encoding encoding = null, bool endStream = false)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            StreamWriter writer = new StreamWriter(stream, encoding);
            writer.Write(text);

            if (endStream)
                writer.Close();
            else
                //如果流继续使用，此处必须flush,因为writer不能close，无法触发内存输出
                writer.Flush();
        }

        /// <summary>
        /// 装载内存流，源Stream默认不关闭，如果需要关闭，传个end=true进来
        /// </summary>
        public static void Load(this Stream container, Stream content, bool closeContainer, bool closeContent)
        {
            content.CopyTo(container);

            if (closeContent)
                content.Close();

            if (closeContainer)
                container.Close();
        }

        /// <summary>
        /// 装载内存流，源Stream默认不关闭，如果需要关闭，传个end=true进来
        /// </summary>
        public static void Load(this Stream stream, byte[] bytes, bool endStream = false)
        {
            stream.Write(bytes, 0, bytes.Length);
            if (endStream)
                stream.Close();
        }

        /// <summary>
        /// 装载文件，源Stream默认不关闭，如果需要关闭，传个end=true进来
        /// </summary>
        public static void LoadFile(this Stream stream, string path, bool endStream = false)
        {
            stream.Load(File.OpenRead(path), endStream, true);
        }
        #endregion

        #region 压缩/解压 GZipStream

        //GZipStream与初始化的Stream会关联Close,所以MemoryStream不用关，外部关闭GZipStream即可
        //GZipStream的CanSeek为False，不支持Position和Length

        /// <summary>
        /// 解压
        /// </summary>
        public static byte[] Ungzip(this byte[] bytes)
        {
            return new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress).ToBytes();
        }

        /// <summary>
        /// 解压(此方法代替Unzip().ToStream(),少一步字节遍历)
        /// </summary>
        public static Stream UngzipStream(this byte[] bytes)
        {
            return new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress);
        }

        /// <summary>
        /// 解压
        /// </summary>
        public static Stream Ungzip(this Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }

        /// <summary>
        /// 解压
        /// </summary>
        public static byte[] UngzipBytes(this Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress).ToBytes();
        }


        /// <summary>
        /// 获取解压后的UTF8编码的字符串
        /// </summary>
        public static string UngzipString(this byte[] bytes, Encoding encoding = null)
        {
            return new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress).GetString(encoding);
        }

        /// <summary>
        /// 获取解压后的UTF8编码的字符串
        /// </summary>
        public static string UngzipString(this Stream stream, Encoding encoding = null)
        {
            return new GZipStream(stream, CompressionMode.Decompress).GetString(encoding);
        }
        #endregion
    }
}
