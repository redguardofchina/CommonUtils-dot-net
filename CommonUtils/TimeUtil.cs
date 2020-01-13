using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 时间相关工具
    /// </summary>
    public static class TimeUtil
    {
        /// <summary>
        /// 网络时间,失败返回 DateTime.Now
        /// </summary>
        private static DateTime GetNet()
        {
            try
            {
                var res = HttpUtil.Post("https://www.baidu.com/");
                if (DateTime.TryParse(res.Headers.GetValues("Date").FirstOrDefault(), out DateTime time))
                    return time;

                LogUtil.Log("网络时间获取失败: " + res);
                return DateTime.Now;
            }
            catch (Exception ex)
            {
                LogUtil.Log("网络时间获取失败: " + ex.Message);
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 时间差
        /// </summary>
        private static long _netDiff { get; set; }

        /// <summary>
        /// 刷新网络时间，一般在程序更改时间后调用
        /// </summary>
        public static void NetRefresh()
        {
            _netDiff = (GetNet() - DateTime.Now).Ticks;
        }

        /// <summary>
        /// 网络时间
        /// </summary>
        public static DateTime Net
        {
            get
            {
                if (_netDiff == 0)
                    NetRefresh();
                return DateTime.Now.AddTicks(_netDiff);
            }
        }

        /// <summary>
        /// 时间戳起点
        /// </summary>
        private static DateTime mTimestampStart { get; } = DateTime.Parse("1970-01-01 00:00:00") + TimeZoneInfo.Local.BaseUtcOffset;

        /// <summary>
        /// 从时间戳获取时间
        /// </summary> 
        public static DateTime FromStamp(long stamp)
        {
            while (stamp < 1000000000000)
                stamp *= 10;
            return mTimestampStart.AddMilliseconds(stamp % 10000000000000);
        }

        /// <summary>
        /// 时间戳 10位 秒级
        /// </summary> 
        public static int Stamp(this DateTime time)
        {
            return (int)(time - mTimestampStart).TotalSeconds;
        }

        /// <summary>
        /// 时间戳 13位 毫秒级
        /// </summary> 
        public static long LongStamp(this DateTime time)
        {
            return (long)(time - mTimestampStart).TotalMilliseconds;
        }

        /// <summary>
        /// 日期时间戳 10位 秒级
        /// </summary>
        public static int DateStamp(this DateTime time)
        {
            return time.Date.Stamp();
        }

        /// <summary>
        /// 月份时间戳 10位 秒级
        /// </summary>
        public static int MonthStamp(this DateTime time)
        {
            return time.AddDays(1 - time.Day).DateStamp();
        }

        /// <summary>
        /// 日期时间戳
        /// </summary>
        public static long DateLongStamp(this DateTime time)
        {
            return time.Date.LongStamp();
        }

        /// <summary>
        /// 获取时间
        /// </summary> 
        public static DateTime FromTicks(long ticks)
        {
            return new DateTime(ticks);
        }

        /// <summary>
        /// 获取时间
        /// </summary> 
        public static DateTime Parse(object time)
        {
            try
            {
                return DateTime.Parse(time.ToString());
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 获取时间
        /// </summary> 
        public static DateTime ToTime(this object time)
        {
            return Parse(time);
        }

        /// <summary>
        /// 非标准时间字符串转为时间,标准时间字符串直接使用Parse或ToDateTime
        /// </summary>
        public static DateTime FromFormat(string str, string format = "yyyyMMddHHmmssfff")
        {
            try
            {
                //截取相同长度
                if (str.Length < format.Length)
                    format = format.Substring(0, str.Length);
                if (str.Length > format.Length)
                    str = str.Substring(0, format.Length);
                //转换
                return DateTime.ParseExact(str, format, null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string YearString(this DateTime time)
        {
            return time.ToString("yyyy");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string MonthString(this DateTime time)
        {
            return time.ToString("MM");
        }

        /// <summary>
        /// 获取星期信息
        /// </summary>
        public static string WeekString(this DateTime time)
        {
            return time.ToString("dddd");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string DayString(this DateTime time)
        {
            return time.ToString("dd");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string HourString(this DateTime time)
        {
            return time.ToString("HH");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string MinuteString(this DateTime time)
        {
            return time.ToString("mm");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string SecondString(this DateTime time)
        {
            return time.ToString("ss");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string DateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 时间拆分字符串
        /// </summary>
        public static string TimeString(this DateTime time)
        {
            return time.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 精确到毫秒
        /// </summary>
        public static string FullInfo(this DateTime time)
        {
            return time.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        /// <summary>
        /// 时间字符串
        /// </summary>
        public static string Info(this DateTime time, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return time.ToString(format);
        }

        /// <summary>
        /// 带小数的小时
        /// </summary>
        public static float FloatHour(this DateTime time)
        {
            return time.Hour + (float)time.Minute / 60;
        }

        /// <summary>
        /// 不计日期 只计时间
        /// </summary>
        public static DateTime Time(this DateTime time)
        {
            return new DateTime().Add(time - time.Date);
        }

        /// <summary>
        /// 时间计算的中间状态
        /// </summary>
        public static TimeSpan Span(this DateTime time)
        {
            return time - new DateTime();
        }

        /// <summary>
        /// 时间累加
        /// </summary>
        public static DateTime Add(this DateTime time1, DateTime time2)
        {
            return time1.AddTicks(time2.Ticks); ;
        }

        /// <summary>
        /// 时间累加,仅仅时间
        /// </summary>
        public static DateTime AddTime(this DateTime time1, DateTime time2)
        {
            return time1.AddTicks(time2.Time().Ticks); ;
        }

        /// <summary>
        /// 替换日期
        /// </summary>
        public static DateTime ReplaceDate(this DateTime time, DateTime dateTime)
        {
            return dateTime.Date.AddTime(time.Time());
        }

        /// <summary>
        /// 返回当前的分钟序列，一天有60*24个序列
        /// </summary>
        public static int MinuteIndex(this DateTime time)
        {
            return (int)time.Time().Span().TotalMinutes;
        }

        /// <summary>
        /// 格林威治时间
        /// </summary>
        public static string ToGmt(this DateTime time)
        {
            return time.ToUniversalTime().ToString("r");
        }

        /// <summary>
        /// 格林威治时间
        /// </summary>
        public static DateTime FromGmt(string gmt)
        {
            return DateTime.Parse(gmt);
        }
    }
}