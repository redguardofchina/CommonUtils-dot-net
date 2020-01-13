using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CommonUtils
{
    public static class EncodeUtil
    {
        #region 文本编码
        /// <summary>
        /// 文本编码
        /// </summary>
        public static byte[] Encode(this string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// 文本编码
        /// </summary>
        public static byte[] GetBytes(this string text, Encoding encoding = null)
        => text.Encode(encoding);

        /// <summary>
        /// 文本编码
        /// </summary>
        public static byte[] Utf8Encode(this string text)
        => Encoding.UTF8.GetBytes(text);

        /// <summary>
        /// 文本解码
        /// </summary>
        public static string Decode(this byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 文本解码
        /// </summary>
        public static string Decode(this string text, Encoding encoding)
        => text.Utf8Encode().Decode(encoding);

        /// <summary>
        /// 文本解码
        /// </summary>
        public static string Utf8Decode(this byte[] bytes)
        => Encoding.UTF8.GetString(bytes);

        /// <summary>
        /// 将string转成byte[]
        /// </summary>
        public static byte[] ToBytes(this string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            var bytes = encoding.GetBytes(text);
            //Encoding.UTF8没有加Bom，这里判断Encodings.UTF8Bom强制加上了
            if (encoding == Encodings.UTF8Bom)
                //0xEF, 0xBB, 0xBF 239, 187, 191
                bytes = Encodings.UTF8BomBytes.Append(bytes);
            return bytes;
        }

        /// <summary>
        /// 将string转成stream
        /// </summary>
        public static Stream GetStream(this string text, Encoding encoding = null)
        {
            var stream = new MemoryStream();
            stream.Load(text, encoding, false);
            stream.Seek();
            return stream;
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        public static string ToText(this byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            //这里判断Encodings.UTF8Bom强制解除bom
            if (encoding == Encodings.UTF8Bom && bytes.Cut(3).EqualValue(Encodings.UTF8BomBytes))
                bytes = bytes.CutAt(3);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 将string转成sbyte[]
        /// </summary>
        public static sbyte[] ToSbytes(this string text, Encoding encoding = null)
        {
            byte[] buffer = text.ToBytes(encoding);
            sbyte[] sbuffer = new sbyte[buffer.Length];
            for (int index = 0; index < buffer.Length; index++)
                sbuffer[index] = (sbyte)buffer[index];
            return sbuffer;
        }

        /// <summary>
        /// sbyte[]转成Text
        /// </summary>
        public static string ToText(this sbyte[] sbytes, Encoding encoding = null)
        {
            byte[] buffer = new byte[sbytes.Length];
            for (int index = 0; index < sbytes.Length; index++)
                buffer[index] = (byte)sbytes[index];
            if (encoding == null)
                encoding = Encoding.UTF8;
            string text = encoding.GetString(buffer);
            int length = text.IndexOf("\0");
            if (length > 0)
                text = text.Substring(0, length);
            return text;
        }
        #endregion

        /// <summary>
        /// UrlEncode
        /// </summary>
        public static string UrlEncode(this string text)
        => HttpUtility.UrlEncode(text);

        /// <summary>
        /// UrlDecode
        /// </summary>
        public static string UrlDecode(this string text)
        => HttpUtility.UrlDecode(text);

        /// <summary>
        /// base64编码
        /// </summary>
        public static string Base64Encode(this byte[] bytes)
        => Convert.ToBase64String(bytes);

        /// <summary>
        /// base64编码
        /// </summary>
        public static string Base64Encode(this Stream stream)
        => stream.ToBytes().Base64Encode();

        /// <summary>
        /// base64编码
        /// </summary>
        public static string Base64Encode(this string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetBytes(text).Base64Encode();
        }

        /// <summary>
        /// 将base64整理为C#可用格式
        /// </summary>
        public static string Base64Sort(this string base64)
        => base64.Replace(' ', '+');

        /// <summary>
        /// base64解码
        /// </summary>
        public static byte[] Base64Decode(this string base64)
        {
            try
            {
                return Convert.FromBase64String(base64);
            }
            catch (Exception ex)
            {
                return ex.Message.ToBytes();
            }
        }

        /// <summary>
        /// base64解码
        /// </summary>
        public static string Base64DecodeString(this string base64, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(base64.Base64Decode());
        }

        #region Encrypt Decrypt

        //OracleInternal 奥睿科的尽量不用吧！
        //OracleInternal.Secure.Network.DataIntegrityAlgorithm;
        //OracleInternal.Secure.Network.DES112;
        //OracleInternal.Secure.Network.DES168;
        //OracleInternal.Secure.Network.EncryptionAlgorithm;
        //OracleInternal.Secure.Network.MD5;
        //OracleInternal.Secure.Network.SHA1;
        //OracleInternal.Secure.Network.SHA256;
        //OracleInternal.Secure.Network.SHA384;
        //OracleInternal.Secure.Network.SHA512;

        //System 还是原生的好
        //System.Security.Cryptography.Aes;
        //System.Security.Cryptography.DES;
        //System.Security.Cryptography.HMAC;
        //System.Security.Cryptography.HMACMD5;
        //System.Security.Cryptography.HMACSHA1;
        //System.Security.Cryptography.HMACSHA256;
        //System.Security.Cryptography.HMACSHA384;
        //System.Security.Cryptography.HMACSHA512;
        //System.Security.Cryptography.MD5;
        //System.Security.Cryptography.Oid;
        //System.Security.Cryptography.RSA;
        //System.Security.Cryptography.SHA1;
        //System.Security.Cryptography.SHA256;
        //System.Security.Cryptography.SHA384;
        //System.Security.Cryptography.SHA512;
        //System.Security.Cryptography.X509Certificates;

        public static string GetSHA1(byte[] bytes, bool needHead = false)
        {
            var cpu = SHA1.Create();
            var result = cpu.ComputeHash(bytes);
            return result.ToHexadecimalString(needHead).ToUpper();
        }

        public static string GetSHA1FromFile(string path, bool needHead = false)
         => GetSHA1(FileUtil.GetBytes(path), needHead);

        public static byte[] Md5Encode(byte[] data)
        => MD5.Create().ComputeHash(data);

        public static byte[] AesEncode(byte[] data)
        => Aes.Create().CreateEncryptor().TransformFinalBlock(data, 0, data.Length);

        public static byte[] AesDecode(byte[] data)
        => Aes.Create().CreateDecryptor().TransformFinalBlock(data, 0, data.Length);

        public static byte[] DesEncode(byte[] data)
        => DES.Create().CreateDecryptor().TransformFinalBlock(data, 0, data.Length);

        public static byte[] DesDecode(byte[] data)
        => DES.Create().CreateDecryptor().TransformFinalBlock(data, 0, data.Length);

        public static byte[] HmacSha256Encode(byte[] seed, byte[] data)
        => new HMACSHA256(seed).ComputeHash(data);
        #endregion
    }
}
