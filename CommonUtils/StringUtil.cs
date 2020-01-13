using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

namespace CommonUtils
{
    /// <summary>
    /// 字符串工具
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 支持带花括号的字符串格式化
        /// </summary>
        public static string Format(string format, params object[] args)
        {
            if (args.Length == 0)
                return format;

            var l = "{";
            var r = "}";
            var left = "!left!";
            var right = "!right!";

            format = format.Replace(l, left).Replace(r, right);

            for (int index = 0; index < args.Length; index++)
                format = format.Replace(left + index + right, l + index + r);

            format = string.Format(format, args);

            return format.Replace(left, l).Replace(right, r);
        }

        /// <summary>
        /// 文本编码
        /// </summary>
        public static byte[] GetBytes(string text, Encoding encoding = null)
        => text.GetBytes(encoding);

        /// <summary>
        /// ASCII枚举
        /// </summary>
        public enum Ascii
        {
            _0 = 48,
            _9 = 57,
            A = 65,
            Z = 90,
            a = 97,
            z = 122,
        }

        /// <summary>
        /// 文本行分隔符
        /// </summary>
        private static string[] _lineSeparator = new string[] { "\r\n", "\n", "\r" };

        /// <summary>
        /// 空格拆分符
        /// </summary>
        private static char[] _spaceSeparator = new char[] { ' ', '\t' };

        /// <summary>
        /// 添加双引号
        /// </summary>
        public static string AddQuotation(this object obj)
        => string.Format("\"{0}\"", obj);

        /// <summary>
        /// 添加双引号
        /// </summary>
        public static string AddQuotation(this string text)
        {
            if (text.StartWith('"'))
                return text;
            return string.Format("\"{0}\"", text);
        }

        /// <summary>
        /// 附加(非引用)
        /// </summary>
        public static string Append(this string left, char sperator, params object[] rights)
        {
            var sb = new StringBuilder(left);
            foreach (var right in rights)
            {
                if (sb.Length > 0)
                    sb.Append(sperator);
                sb.Append(right);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 附加(非引用)
        /// </summary>
        public static string Append(this string left, params object[] rights)
        {
            var sb = new StringBuilder(left);
            foreach (var right in rights)
                sb.Append(right);
            return sb.ToString();
        }

        /// <summary>
        /// 累加
        /// </summary>
        public static void Append(this StringBuilder sb, params object[] args)
        {
            foreach (var arg in args)
                sb.Append(arg);
        }

        /// <summary>
        /// 格式化
        /// </summary>
        public static void AppendFormatLine(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendFormat(format, args);
            sb.AppendLine();
        }

        /// <summary>
        /// 附加(非引用)
        /// </summary>
        public static string AppendLine(this string left, params object[] rights)
        {
            var sb = new StringBuilder(left);
            foreach (var right in rights)
            {
                sb.AppendLine();
                sb.Append(right);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 累加 一行
        /// </summary>
        public static void AppendLine(this StringBuilder sb, params object[] args)
        {
            foreach (var arg in args)
                sb.Append(arg);
            sb.AppendLine();
        }

        /// <summary>
        /// 累加 多行
        /// </summary>
        public static void AppendLines(this StringBuilder sb, params object[] args)
        {
            foreach (var arg in args)
                sb.AppendLine(arg);
        }

        /// <summary>
        /// 包含关键字
        /// </summary>
        public static bool Contains(this string str, params string[] keys)
        {
            foreach (string key in keys)
                if (str.Contains(key))
                    return true;
            return false;
        }

        public static bool Contains(this string text, char single)
        => text.IndexOf(single) != -1;

        public static int CountOf(this string text, char key)
        => CountOf(text, key.ToString());

        /// <summary>
        /// key数量
        /// </summary>
        public static int CountOf(this string text, string key)
        => (text.Length - text.Replace(key, "").Length) / key.Length;

        /// <summary>
        /// 判断起始字符
        /// </summary>
        public static bool EndWith(this string text, params char[] chars)
        {
            int index = text.Length - 1;
            foreach (char ch in chars)
                if (text[index] == ch)
                    return true;
            return false;
        }

        /// <summary>
        /// 以end结尾
        /// </summary>
        public static bool EndWith(this string text, string end)
        {
            if (text.Length < end.Length)
                return false;
            return text.Substring(text.Length - end.Length, end.Length) == end;
        }

        /// <summary>
        /// 多字符串比较
        /// </summary>
        public static bool Equals(this string str, params string[] strs)
        {
            foreach (var item in strs)
                if (str == item)
                    return true;
            return false;
        }

        /// <summary>
        /// 比较开始是否一样
        /// </summary>
        public static bool EqualsStart(this string left, string right)
        {
            if (left.Length > right.Length)
                left = left.Substring(0, right.Length);
            else
                right = right.Substring(0, left.Length);
            return left == right;
        }

        /// <summary>
        /// 忽略大小写判断是否相等
        /// </summary>
        public static bool EqualsWithoutCase(this string left, params string[] rights)
        {
            var lowerLeft = left.ToLower();
            foreach (var right in rights)
                if (lowerLeft == right.ToLower())
                    return true;
            return false;
        }

        public static bool EqualsCaseIgnored(this string left, params string[] rights)
        => EqualsWithoutCase(left, rights);

        /// <summary>
        /// 分隔字符串 
        /// </summary>
        public static string[] ExSplit(string src, string separator, bool ifTrim = true)
        {
            List<string> listStr = new List<string>();
            int index = src.IndexOf(separator);
            while (index > -1)
            {
                string str = src.Substring(0, index);
                if (ifTrim)
                {
                    str = str.Trim();
                }
                if (!string.IsNullOrEmpty(str))
                {
                    listStr.Add(str);
                }
                src = src.Substring(index + separator.Length);
                index = src.IndexOf(separator);
            }
            if (ifTrim)
            {
                src = src.Trim();
            }
            if (!string.IsNullOrEmpty(src))
            {
                listStr.Add(src);
            }
            return listStr.ToArray();
        }

        /// <summary>
        /// 分隔字符串 
        /// </summary>
        public static string[] ExSplit(this string src, char separator = ',', bool ifTrim = true)
        {
            string[] strs = new string[0];

            if (ifTrim && !string.IsNullOrEmpty(src))
            {
                src = src.Trim();
            }

            if (string.IsNullOrEmpty(src))
            {
                return strs;
            }

            List<string> listStr = new List<string>();
            strs = src.Split(separator);
            if (!ifTrim)
                return strs;
            foreach (string str in strs)
            {
                string str1 = str.Trim();
                if (!string.IsNullOrWhiteSpace(str1))
                    listStr.Add(str1);
            }
            return listStr.ToArray();
        }

        /// <summary>
        /// 分隔字符串 
        /// </summary>
        public static string[] ExSplit(this string[] srcs, char separator = ',', bool ifTrim = true)
        {
            List<string> list = new List<string>();
            foreach (string src in srcs)
                list.AddRange(src.ExSplit(separator, ifTrim));
            return list.ToArray();
        }

        public static string FiltHtml(this string html)
        {
            var sb = new StringBuilder();
            bool append = true;
            foreach (var ch in html)
            {
                if (ch == '<')
                    append = false;
                if (append)
                    sb.Append(ch);
                if (ch == '>')
                    append = true;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取中间部分 默认包含两头标签
        /// </summary>
        public static string[] GetBetweens(this string text, string start, string end, bool containsTag = true)
        {
            var removes = new List<string>();
            int endIndex = 0;
            while (true)
            {
                int startIndex = text.IndexOf(start, endIndex);
                if (startIndex == -1)
                    break;

                startIndex += start.Length;

                endIndex = text.IndexOf(end, startIndex);
                if (endIndex == -1)
                    break;

                if (containsTag)
                    removes.Add(text.SubstringByIndex(startIndex - start.Length, endIndex + end.Length));
                else
                    removes.Add(text.SubstringByIndex(startIndex, endIndex));
                endIndex += end.Length;
            }
            return removes.Distinct().ToArray();
        }

        /// <summary>
        /// 只取数字
        /// </summary>
        public static string GetDigits(this string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in text)
                if (ch >= (int)Ascii._0 && ch <= (int)Ascii._9)
                    sb.Append(ch);
            return sb.ToString();
        }

        public static string GetGuidWithoutBars()
        => System.Guid.NewGuid().ToString().Remove("-");

        /// <summary>
        /// 将字符串拼接
        /// </summary>
        public static string GetJoinedString(string str1, string str2, string separator = "")
        => string.IsNullOrEmpty(str1) ? str2 : (str1 + separator + str2);

        /// <summary>
        /// 将字符串数组拼接为字符串
        /// </summary>
        public static string GetJoinedString(string[] strs, string separator = "")
        => string.Join(separator, strs);

        /// <summary>
        /// 只取字母数字
        /// </summary>
        public static string GetLettersAndDigits(this string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in text)
            {
                if (ch >= (int)Ascii._0 && ch <= (int)Ascii._9)
                    sb.Append(ch);

                if (ch >= (int)Ascii.a && ch <= (int)Ascii.z)
                    sb.Append(ch);

                if (ch >= (int)Ascii.A && ch <= (int)Ascii.Z)
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取文本行
        /// </summary>
        public static string[] GetLines(this string text, StringSplitOptions splitOptions = StringSplitOptions.None)
        => text.Split(_lineSeparator, splitOptions);

        /// <summary>
        /// 分隔字符串 
        /// </summary>
        public static int[] GetSplitedInts(string src, char separator)
        {
            string[] strs = ExSplit(src, separator);
            List<int> listInt = new List<int>();
            foreach (string tempStr in strs)
            {
                listInt.Add(ConvertUtil.ToInt(tempStr));
            }
            return listInt.ToArray();
        }

        /// <summary>
        /// 将路由的参数标准化为{0}/{1}/{2} 使用此方法后，参数可简写为{{{
        /// </summary>
        public static string GetStandardStringFormatFromRoutePath(string route)
        {
            var count = route.CountOf('{');
            var dest = new StringBuilder();
            for (int index = 0; index < count; index++)
            {
                if (dest.Length > 0)
                    dest.Append('/');
                dest.Append('{');
                dest.Append(index);
                dest.Append('}');
            }
            dest.Insert(0, route.SubstringFromStartToChar('{'));
            return dest.ToString();
        }

        public static string GetStartSpace(this string text)
        => text.Substring(0, text.Length - text.TrimStart().Length);

        public static string GetText(this IEnumerable<string> lines)
        => JoinToText(lines);

        /// <summary>
        /// GUID
        /// </summary>
        public static string GetGuid(string tail = null)
        => System.Guid.NewGuid().ToString().Remove("-") + tail;

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        public static bool IsDigit(string str)
        => int.TryParse(str, out int result);

        /// <summary>
        /// 多字符串判空
        /// </summary>
        public static bool IsNullOrEmpty(params string[] strs)
        {
            foreach (string str in strs)
                if (string.IsNullOrEmpty(str))
                    return true;
            return false;
        }

        /// <summary>
        /// 字符串判空
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        => string.IsNullOrEmpty(str);

        /// <summary>
        /// 多字符串判空
        /// </summary>
        public static bool IsNullOrWhiteSpace(params string[] strs)
        {
            foreach (string str in strs)
                if (string.IsNullOrWhiteSpace(str))
                    return true;
            return false;
        }

        /// <summary>
        /// 拼接
        /// </summary>
        public static string Join<T>(this IEnumerable<T> values, object separator)
        => string.Join(separator.ToString(), values);

        /// <summary>
        /// 文本行连接
        /// </summary> 
        public static string JoinToText(this IEnumerable<string> lines)
        => lines.Join("\r\n");

        /// <summary>
        /// LongTimestamp
        /// </summary>
        public static string LongTimestamp(string tail = null)
        => DateTime.Now.LongStamp() + tail;

        /// <summary>
        /// 移除字符
        /// </summary>
        public static string Remove(this string text, params char[] removes)
        {
            if (text.IsNullOrEmpty())
                return text;

            var newText = new StringBuilder();
            foreach (char element in text)
                if (!removes.Contains(element))
                    newText.Append(element);
            return newText.ToString();
        }

        /// <summary>
        /// 移除字符串
        /// </summary>
        public static string Remove(this string text, params string[] removes)
        {
            foreach (string remove in removes)
                text = text.Replace(remove, null);
            return text;
        }

        /// <summary>
        /// 删除中间部分 默认删除两头标签
        /// </summary>
        public static string RemoveBetween(this string text, string start, string end, bool containsTag = true)
        => text.Remove(text.GetBetweens(start, end, containsTag));

        public static void RemoveEnd(this StringBuilder sb)
        => sb.Remove(sb.Length - 1, 1);

        /// <summary>
        /// 删除双引号
        /// </summary>
        public static string RemoveQuotation(this string text)
        {
            if (text[0] == '"' && text[text.Length - 1] == '"')
                return text.Substring(1, text.Length - 2);
            return text;
        }

        /// <summary>
        /// 替换
        /// </summary>
        public static string Replace(this string src, MapStringString map)
        {
            foreach (var keyValue in map)
                src = src.Replace(keyValue.Key, keyValue.Value);
            return src;
        }

        /// <summary>
        /// 替换
        /// </summary>
        public static void Replace(this string[] values, string src, string dest)
        {
            for (int index = 0; index < values.Length; index++)
                values[index] = values[index].Replace(src, dest);
        }

        /// <summary>
        /// 替换头部
        /// </summary>
        public static string ReplaceHead(this string text, string head)
        => head + text.Substring(head.Length);

        /// <summary>
        /// 递归替换，用于单词替换后重组的字符串形成新的关键词的情况
        /// </summary>
        public static string ReplaceRecursive(this string text, string src, string dest)
        {
            while (text.Contains(src))
                text = text.Replace(src, dest);
            return text;
        }

        /// <summary>
        /// 替换斜杠
        /// </summary>
        public static string ReplaceSprit(this string text, bool positive = true)
        {
            if (positive)
                return text.Replace('\\', '/');
            return text.Replace('/', '\\');
        }

        /// <summary>
        /// 文本存储
        /// </summary>
        public static void SaveTo(this string text, string path)
        => FileUtil.Save(path, text);

        /// <summary>
        /// 拆分
        /// </summary>
        public static string[] Split(this string text, object separator, StringSplitOptions option = StringSplitOptions.None)
        => text.Split(new string[] { separator.ToString() }, option);

        /// <summary>
        /// 分隔，保留空值
        /// </summary>
        public static string[] Split(this string value, params string[] separators)
        => value.Split(separators, StringSplitOptions.None);

        /// <summary>
        /// 空格拆分
        /// </summary>
        public static string[] SplitBySpace(this string text)
        => text.Split(_spaceSeparator, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// 分隔，忽略空值
        /// </summary>
        public static string[] SplitNoEmpty(this string value, params char[] separators)
        => value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// 分隔，忽略空值
        /// </summary>
        public static string[] SplitNoEmpty(this string value, params string[] separators)
        => value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        public static bool StartsWith(this string text, params string[] keys)
        {
            foreach (var item in keys)
                if (text.StartsWith(item))
                    return true;
            return false;
        }

        /// <summary>
        /// 判断起始字符
        /// </summary>
        public static bool StartWith(this string text, params char[] chars)
        {
            foreach (char ch in chars)
                if (text[0] == ch)
                    return true;
            return false;
        }

        /// <summary>
        /// 以start开头
        /// </summary>
        public static bool StartWith(this string text, string start)
        {
            if (text.Length < start.Length)
                return false;
            return text.Substring(0, start.Length) == start;
        }

        public static string SubEnd(this string text, char sperator)
        => text.Substring(text.IndexOf(sperator) + 1);

        public static string SubFront(this string text, char sperator)
        => text.Substring(0, text.IndexOf(sperator));

        /// <summary>
        /// 字符串截取
        /// </summary>
        public static string Substring(this string text, int startIndex, string end, bool needTag = true)
        {
            var endIndex = text.IndexOf(end, startIndex);
            if (needTag)
                endIndex += end.Length;
            return text.SubstringByIndex(startIndex, endIndex);
        }

        /// <summary>
        /// 字符串截取
        /// </summary>
        public static string Substring(this string text, string start, bool needTag = true)
        {
            var startIndex = text.IndexOf(start);
            if (!needTag)
                startIndex += start.Length;
            return text.Substring(startIndex);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        public static string Substring(this string text, string start, string end, bool needTag = true)
        {
            if (text.IsNullOrEmpty())
                return null;

            var startIndex = text.IndexOf(start);
            if (startIndex == -1)
                return null;

            startIndex += start.Length;

            var endIndex = text.IndexOf(end, startIndex);
            if (endIndex == -1)
                return null;

            if (needTag)
            {
                endIndex += end.Length;
                startIndex -= start.Length;
            }
            return text.SubstringByIndex(startIndex, endIndex);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        public static string SubstringByIndex(this string text, int startIndex, int endIndex)
        => text.Substring(startIndex, endIndex - startIndex);

        /// <summary>
        /// 截取第N个字符
        /// </summary>
        public static string SubstringByKeyIndex(this string text, string key, int index)
        {
            while (text.CountOf(key) > index)
                text = text.Substring(0, text.LastIndexOf(key));
            return text;
        }

        public static string SubstringEndByFirstKey(this string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var index = text.IndexOf(key);
            if (index == -1)
                return text;

            return text.Substring(index + 1);
        }

        public static string SubstringEndByFirstKey(this string text, char key)
        => SubstringEndByFirstKey(text, key.ToString());

        public static string SubstringEndWith(this string text, char key, int count)
        {
            var index = -1;
            while (count-- > 0)
            {
                index = text.IndexOf(key, index + 1);
                if (index == -1)
                    break;
            }
            if (index == -1)
                index = text.Length;
            return text.Substring(0, index);
        }

        public static string SubstringFromStartToChar(this string text, char key)
        {
            var index = text.IndexOf(key);
            if (index != -1)
                return text.Substring(0, index);
            return text;
        }

        /// <summary>
        /// 字符串截取
        /// </summary>
        public static string SubstringLast(this string text, string start, bool needTag = true)
        {
            var startIndex = text.LastIndexOf(start);
            if (!needTag)
                startIndex += start.Length;
            return text.Substring(startIndex);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        public static string[] Substrings(this string text, string start, string end, bool needTag = true)
        {
            var lines = new List<string>();
            int endIndex = 0;
            while (true)
            {
                int startIndex = text.IndexOf(start, endIndex);
                if (startIndex == -1)
                    break;

                startIndex += start.Length;

                endIndex = text.IndexOf(end, startIndex);
                if (endIndex == -1)
                    break;

                if (needTag)
                    lines.Add(start + text.SubstringByIndex(startIndex, endIndex) + end);
                else
                    lines.Add(text.SubstringByIndex(startIndex, endIndex));

                endIndex += end.Length;
            }
            return lines.ToArray();
        }

        public static string SubstringStartByFirst(this string text, char key)
        => SubstringStartByFirst(text, key.ToString());

        public static string SubstringStartByFirst(this string text, string key)
        {
            var index = text.IndexOf(key);
            if (index == -1)
                return text;
            return text.Substring(0, index);
        }

        /// <summary>
        /// Timestamp
        /// </summary>
        public static string Timestamp(string tail = null)
        => DateTime.Now.Stamp() + tail;

        public static string ToText(this IEnumerable<string> lines)
        => JoinToText(lines);

        /// <summary>
        /// 去空格和换行
        /// </summary>
        public static void Trim(this string[] lines)
        {
            for (int index = 0; index < lines.Length; index++)
                lines[index] = lines[index].Trim();
        }
    }
}
