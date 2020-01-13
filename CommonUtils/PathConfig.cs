namespace CommonUtils
{
    public static class PathConfig
    {
        //默认配置文件名
        public const string DefaultConfigFileName = "settings.json";
        //默认分支（没有分支）标识 网页拿不到值会传入"undefined",所以用这个
        public const string DefaultSubMark = "undefined";
        //分支根文件夹名
        private const string _subRootName = "sub";

        //文件根目录 相对路径在发布到服务、IIS时会出错，所以此处使用绝对路径
        public static string RootFloder { get; private set; }

        //开销文件夹
        public static string RuntimesFloder { get; private set; }
        //默认数据库路径
        public static string DefaultSqlitePath { get; private set; }

        //日志文件夹
        public static string LogFloder { get; private set; }
        //分支日志文件夹
        public static string SubLogFloder { get; private set; }

        //记录异常文件夹
        public static string ErrorFloder { get; private set; }
        //记录分支异常文件夹
        public static string SubErrorFloder { get; private set; }

        //配置文件夹 相对路径在发布到服务、IIS时会出错，所以此处使用绝对路径
        public static string ConfigFloder { get; private set; }

        //分支配置文件夹
        public static string SubConfigFloder { get; private set; }

        private static void SetPaths()
        {
            RuntimesFloder = RootFloder.Combine("~runtimes");
            DefaultSqlitePath = RuntimesFloder.Combine("sqlite.db");

            LogFloder = RuntimesFloder.Combine("log");
            SubLogFloder = LogFloder.Combine(_subRootName);

            ErrorFloder = RuntimesFloder.Combine("error");
            SubErrorFloder = ErrorFloder.Combine(_subRootName);

            ConfigFloder = RootFloder.Combine("config");
            SubConfigFloder = ConfigFloder.Combine(_subRootName);
        }

        static PathConfig()
        {
            RootFloder = PathUtil.Base;
            SetPaths();
        }

        /// <summary>
        /// 重置文件根目录 请使用绝对路径
        /// </summary>
        public static void SetRootFloder(string floder)
        {
            RootFloder = floder;
            SetPaths();
        }
    }
}
