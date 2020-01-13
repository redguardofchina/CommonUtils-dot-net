using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Test.Web
{
    public static class Config
    {
        public static string StartUrl { get; private set; }

        static Config()
        {
            StartUrl = ConfigUtil.Default.GetString("StartUrl");
        }
    }
}
