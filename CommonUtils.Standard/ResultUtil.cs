using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    public static class ResultUtil
    {
        /// <summary>
        /// 多bool判断
        /// </summary>
        public static bool True(params bool[] args)
        {
            foreach (var arg in args)
            {
                if (!arg)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 多bool判断
        /// </summary>
        public static bool False(params bool[] args)
        {
            foreach (var arg in args)
            {
                if (!arg)
                    return true;
            }
            return false;
        }
    }
}
