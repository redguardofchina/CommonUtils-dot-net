using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonUtils
{
    /// <summary>
    /// 字符串处理类
    /// </summary>
    public static class StringUtil_
    {
        /// <summary>
        /// success常量
        /// </summary>
        public const string Success = "success";

        /// <summary>
        /// 数字字符枚举
        /// </summary>
        private static char[] _digitChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// 字符标点
        /// </summary>
        private static char[] _punctuations = { ',', '.', ';', ':', '?', '!', '(', ')', '\'', '"', ' ' };

        /// <summary>
        /// 字符串标点
        /// </summary>
        private static string[] _utfPunctuations = { "，", "。", "；", "、", "：", "？", "！", "‘", "’", "“", "”", "（", "）", "《", "》" };

        /// <summary>
        /// 填充/截断字符串
        /// </summary>
        public static string AggrString(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                str = "0000000000";

            while (str.Length < length)
                str += str;

            if (str.Length > length)
                str = str.Substring(0, length);

            return str;
        }

        /// <summary>
        /// 集合ABC
        /// </summary>
        public static string AggrString(string str1, string str2, string aggr)
        {
            if (string.IsNullOrEmpty(str1))
                return str2;

            if (string.IsNullOrEmpty(str2))
                return str1;

            return str1 + aggr + str2;
        }

        /// <summary>
        /// 字符串前补零
        /// </summary>
        public static string AggrZero(object obj, int length)
        {
            string str = obj.ToString();

            if (str.Length >= length)
                return str.Substring(str.Length - length, length);

            while (str.Length < length)
                str = "0" + str;

            return str;
        }

        /// <summary>
        /// 截取字符串后部
        /// </summary>
        public static string End(this string src, int length)
        {
            if (src.Length < length)
                return src;
            return src.Substring(src.Length - length);
        }

        /// <summary>
        /// 长度不够 用字符填充
        /// </summary>
        public static string Fill(this string text, int length, char ch = '0')
        {
            if (text.Length >= length)
                return text;

            var sb = new StringBuilder();
            for (int index = 0; index < length - text.Length; index++)
                sb.Append(ch);
            sb.Append(text);
            return sb.ToString();
        }

        /// <summary>
        /// 删除数字
        /// </summary>
        public static string FiltDigit(this string value)
        => value.Remove(_digitChars);

        /// <summary>
        /// 截取字符串前部
        /// </summary>
        public static string Front(this string src, int length)
        {
            if (src.Length < length)
                return src;

            return src.Substring(0, length);
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        public static string GetDescription(this string text)
        => text.Front(20);

        /// <summary>
        /// 获取数值信息
        /// </summary>
        public static string GetDigitInfo(decimal value, bool IsPlusContained = false)
        {
            string symbol = "";

            if (IsPlusContained && value > 0)
                symbol = "+";

            if (value < 0)
            {
                symbol = "-";
                value = -value;
            }

            if (value < 10000)
                return symbol + value.ToString("0.00");

            decimal valueD = value / 10000;

            if (valueD < 10000)
                return symbol + valueD.ToString("0.00") + "万";

            valueD /= 10000;
            return symbol + valueD.ToString("0.00") + "亿";
        }

        /// <summary>
        /// 获取数值信息
        /// </summary>
        public static string GetDigitInfo(int value, int baseValue = 16)
        => Convert.ToString(value, baseValue);

        /// <summary>
        /// 文件大小单位换算
        /// </summary>
        public static string GetFileSize(object fileLengthObject)
        {
            long fileLength = ConvertUtil.ToLong(fileLengthObject);

            if (fileLength / 1024 >= 1024 * 1024 * 1024)
                return (fileLength / (1024 * 1024 * 1024 * 1024.0)).ToString("0.0") + "TB";

            if (fileLength >= 1024 * 1024 * 1024)
                return (fileLength / (1024 * 1024 * 1024.0)).ToString("0.0") + "GB";

            if (fileLength >= 1024 * 1024)
                return (fileLength / (1024 * 1024.0)).ToString("0.0") + "MB";

            if (fileLength >= 1024)
                return (fileLength / 1024.0).ToString("0.0") + "KB";

            if (fileLength < 1024)
                return "<1KB";

            return fileLengthObject + "B";
        }

        /// <summary>
        /// 获取两值之间字符串
        /// </summary>
        public static string GetFill(string src, string start, string end, bool isTagContained)
        {
            string fill = null;
            int startIndex = src.IndexOf(start);
            if (startIndex > -1)
            {
                int endIndex = src.IndexOf(end, startIndex + start.Length);
                if (endIndex > -1)
                {
                    fill = src.Substring(startIndex + start.Length, endIndex - (startIndex + start.Length));
                    if (isTagContained)
                    {
                        fill = start + fill + end;
                    }
                }
            }
            return fill;
        }

        /// <summary>
        /// 获取两字符之前的关键字
        /// </summary>
        public static string[] GetFills(string src, string start, string end, bool isTagContained = false)
        {
            List<string> listFill = new List<string>();

            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                return listFill.ToArray();

            string tempSrc = src;
            int endIndex = tempSrc.IndexOf(end);
            while (endIndex > -1)
            {
                string fillContainer = tempSrc.Substring(0, endIndex);
                int startIndex = fillContainer.LastIndexOf(start);
                if (startIndex > -1)
                {
                    string fill = fillContainer.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);
                    if (isTagContained)
                    {
                        fill = start + fill + end;
                    }
                    listFill.Add(fill);
                }

                if (endIndex + end.Length + 1 > tempSrc.Length)
                    break;

                tempSrc = tempSrc.Substring(endIndex + end.Length, tempSrc.Length - endIndex - end.Length);
                endIndex = tempSrc.IndexOf(end);
            }
            return listFill.ToArray();
        }

        /// <summary>
        /// 加长填充字符
        /// </summary>
        public static string GetFillsAppendedString(string src, string start, string end, string append)
        {
            string[] fills = GetFills(src, start, end, true);
            foreach (string fill in fills)
                src = src.Replace(fill, fill + append);
            return src;
        }

        /// <summary>
        /// 改变填充字符
        /// </summary>
        public static string GetFillsChangedString(string src, string start, string end, string newValue)
        => GetFillsChangedString(src, start, end, end, newValue);

        /// <summary>
        /// 改变填充字符
        /// </summary>
        public static string GetFillsChangedString(string src, string start, string end, string oldValue, string newValue)
        {
            string[] fills = GetFills(src, start, end, true);
            foreach (string fill in fills)
                src = src.Replace(fill, fill.Replace(oldValue, newValue));
            return src;
        }

        /// <summary>
        /// 删除填充字符
        /// </summary>
        public static string GetFillsDeletedString(string src, string start, string end, bool isTagContained)
        {
            var fills = GetFills(src, start, end, isTagContained);
            return GetFillsDeletedString(src, fills);
        }

        /// <summary>
        /// 删除填充字符
        /// </summary>
        public static string GetFillsDeletedString(string src, string[] fills)
        {
            foreach (string fill in fills)
                src = src.Replace(fill, "");
            return src;
        }

        /// <summary>
        /// 删除填充字符
        /// </summary>
        public static string GetFillsInsertedString(string src, string start, string end, string append)
        {
            var fills = GetFills(src, start, end, true);
            foreach (string fill in fills)
            {
                src = src.Replace(fill, fill.Replace(end, append + end));
            }
            return src;
        }

        /// <summary>
        /// 替换填充字符
        /// </summary>
        public static string GetFillsReplacedString(string src, string start, string end, string newValue)
        {
            var fills = GetFills(src, start, end, true);
            foreach (string fill in fills)
            {
                src = src.Replace(fill, newValue);
            }
            return src;
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary> 
        public static string GetFiltGuidString(int length = 0, bool isMark = false)
        {
            string randomString = Guid.NewGuid().ToString().Replace("-", "");

            if (isMark)
                randomString = "_" + randomString;

            if (length == 0)
                return randomString;

            while (randomString.Length < length)
                randomString += Guid.NewGuid().ToString().Replace("-", "");

            return randomString.Substring(0, length).ToUpper();
        }

        /// <summary>
        /// 获取字符串前半部分
        /// </summary> 
        public static string GetFront(string src, string separator)
        {
            int index = src.IndexOf(separator);
            if (index > -1)
                src = src.Substring(0, index);
            return src;
        }

        /// <summary>
        /// 获取带*的Ip
        /// </summary> 
        public static string GetHiddenIpInfo(string ip)
        {
            int firstIndex = ip.IndexOf(".");
            int lastIndex = ip.LastIndexOf(".");
            if (firstIndex != -1)
                return ip.Substring(0, firstIndex) + ".*.*" + ip.Substring(lastIndex);
            return "*";
        }

        /// <summary>
        /// 获取关键字
        /// </summary>
        public static string GetKeywords(this string text)
        {
            text = text.RemovePunctuation();
            string[] lines = text.GetLines();
            string keywords = "";
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    keywords += line.End(4) + ',';
                }
            }
            return keywords;
        }

        /// <summary>
        /// 获取两值之间字符串
        /// </summary>
        public static string GetMiddleString(string src, string start, string end, bool isTagContained = false)
        => GetFill(src, start, end, isTagContained);

        /// <summary>
        /// 获取两字符之前的字符数组
        /// </summary>
        public static string[] GetMiddleStrings(this string src, string start, string end, bool isTagContained = false)
        => GetFills(src, start, end, isTagContained);

        /// <summary>
        /// 获取关键字出现次数
        /// </summary>
        public static int GetOccuredTime(string sentence, string word)
        {
            int i = -1, count = -1;
            do
            {
                i = sentence.IndexOf(word, i + 1);
                count++;
            }
            while (i != -1);
            return count;
        }

        /// <summary>
        /// 获取百分比字符串
        /// </summary>
        public static string GetRateInfo(decimal value)
        {
            if (value > 0)
                return "+" + value.ToString("0.00") + "% ";
            return value.ToString("0.00") + "% ";
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        public static string GetStrAfterKeyword(string str, string mark)
        {
            if (string.IsNullOrEmpty(mark))
                return str;

            int markLastIndex = str.LastIndexOf(mark);

            if (markLastIndex != -1)
                str = str.Substring(markLastIndex + mark.Length, str.Length - (markLastIndex + mark.Length));

            return str;
        }

        /// <summary>
        /// 字符串截取,不添加省略号
        /// </summary>
        public static string GetSubedString(object obj, int length)
        {
            string str = obj.ToString();

            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return (str.Length <= length) ? str : (str.Substring(0, length));
        }

        /// <summary>
        /// 字符串截取,添加省略号
        /// </summary>
        public static string GetSubedStringWhitDot(object obj, int length)
        {
            string str = obj.ToString();

            if (string.IsNullOrEmpty(str))
                return "...";

            return (str.Length <= length) ? str : (str.Substring(0, length) + "...");
        }

        /// <summary>
        /// 获取当前时间路径
        /// </summary> 
        public static string GetTimePath(string ext = "")
        => DateTime.Now.Info("yyyyMMddHHmmssfff") + ext;

        /// <summary>
        /// 删除换行,空格
        /// </summary>
        public static string GetWarpDeletedTrimedString(string str)
        {
            str = str.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            while (str.Contains("  "))
                str = str.Replace("  ", " ");
            return str;
        }

        /// <summary>
        /// 删除换行,空格
        /// </summary>
        public static string GetWarpTrimedString(string str)
        {
            while (str.Contains("\r\r"))
                str = str.Replace("\r\r", "\r");

            while (str.Contains("\n\n"))
                str = str.Replace("\n\n", "\n");

            while (str.Contains("\r\n\r\n"))
                str = str.Replace("\r\n\r\n", "\r\n");

            while (str.Contains("  "))
                str = str.Replace("  ", " ");

            return str;
        }

        /// <summary>
        /// 固定长度,随机补偿
        /// </summary>
        public static string InitLength(this string value, int length)
        {
            if (value.Length >= length)
                return value.Substring(value.Length - length);
            return RandomUtil.GetString(length - value.Length) + value;
        }

        /// <summary>
        /// 判断字符串是否包含关键字
        /// </summary>
        public static bool IsContain(string str, string[] keys)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            foreach (string key in keys)
            {
                if (str.Contains(key))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断数组是否包含字符串
        /// </summary>
        public static bool IsContain(string[] strings, string str)
        => strings.Contains(str);

        /// <summary>
        /// 判断是否包含关键字
        /// </summary>
        public static bool IsKeywordIn(string keyword, string str)
        {
            str = str.ToUpper();
            keyword = keyword.ToUpper();
            return str.Contains(keyword);
        }

        /// <summary>
        /// 判断数组是否包含项目
        /// </summary>
        public static bool IsKeywordIn(string keyword, string[] strs)
        {
            foreach (string s in strs)
            {
                if (s.ToUpper() == keyword.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除标点
        /// </summary>
        public static string RemovePunctuation(this string text)
        {
            text = text.Remove(_punctuations);
            return text.Remove(_utfPunctuations);
        }
    }
}
