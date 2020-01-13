using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 随机工具
    /// </summary>
    public class RandomUtil
    {
        /// <summary>
        /// 公用随机器
        /// </summary>
        public static Random Random { get; } = new Random();

        /// <summary>
        /// 随机整数
        /// </summary>
        public static int Intger(int max = 100)
        {
            return Random.Next(max);
        }

        /// <summary>
        /// 随机整数
        /// </summary>
        public static int GetIntger(int min, int max)
        {
            return Random.Next(min, max);
        }

        /// <summary>
        /// 随机整数
        /// </summary>
        public static int Get(int max = 100)
        {
            return Intger(max);
        }

        /// <summary>
        /// 随机整数
        /// </summary>
        public static int Get(int min, int max)
        {
            return GetIntger(min, max);
        }

        /// <summary>
        /// 随机小数
        /// </summary>
        public static float Float(int max = 100, int floatSize = 2)
        {
            int baseMax = (int)Math.Pow(10, floatSize);
            float value = (float)Random.Next(baseMax) / baseMax;
            return Random.Next(max) + value;
        }

        /// <summary>
        /// 字符池
        /// </summary>
        private static string _charPool { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// 获取随机字符串
        /// </summary> 
        public static string GetString(int length = 8, bool isLetterFirst = true, bool Upper = true)
        {
            var sb = new StringBuilder();
            if (isLetterFirst)
                sb.Append(_charPool[Random.Next(_charPool.Length - 10)]);
            while (sb.Length < length)
                sb.Append(_charPool[Random.Next(_charPool.Length)]);
            if (Upper)
                return sb.ToString();
            else
                return sb.ToString().ToLower();
        }

        private static string _letterPool { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GetLetterString(int length = 8, bool Upper = true)
        {
            var sb = new StringBuilder();
            while (sb.Length < length)
                sb.Append(_letterPool[Random.Next(_letterPool.Length)]);
            if (Upper)
                return sb.ToString();
            else
                return sb.ToString().ToLower();
        }

        /// <summary>
        /// 每分钟随机累加，当前时间的数量
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <param name="daySum">一天结束达到总数</param>
        /// <param name="refrshMinute">刷新间隔</param>
        public static int MintueCount(DateTime time, int daySum, int refrshMinute)
        {
            //数据存储路径
            string key = string.Format("random-minute-{0}-{1}", daySum, refrshMinute);
            //查找缓存
            var sums = CacheUtil.GetFromFile<List<int>>(key);
            if (sums == null)
            {
                //一天有60*24分钟，分割间隔refrshMinute
                var splitCount = 60 * 24 / refrshMinute;
                //每分钟的平均数量
                var avg = daySum / splitCount;
                //振幅30%
                var value = (int)(avg * 0.3);
                if (value < 3)
                    throw new Exception("RandomUtil.MintueCount：拆分数据太小！");
                //记录每分钟应该达到的数量
                sums = new List<int>();
                int sum = 0;
                for (int index = 0; index < splitCount; index++)
                {
                    sum += avg + GetIntger(-value, value);
                    sums.Add(sum);
                }
                //写入缓存
                CacheUtil.SaveWithFile(key, sums);
            }
            //获取当前分钟序列
            var sunIndex = time.MinuteIndex() / refrshMinute;
            return sums[sunIndex];
        }
    }
}
