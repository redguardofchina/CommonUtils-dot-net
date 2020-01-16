using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for Sqlite
    /// </summary>
    public class DbSqlite : DbBase
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public class ConnectStrings
        {
            public static string FromPath(string path)
            => "Data Source =" + path;

            /// <summary>
            /// EF会创建数据库，只需建好文件夹即可
            /// </summary>
            public static string GetDefault(bool create = false)
            {
                string path = PathConfig.DefaultSqlitePath;
                if (!FileUtil.Exists(path))
                {
                    if (create)
                        ResourceUtil.ReadCommonStream(ResourceUtil.CommonResourceName.Sqlite).CreateFile(path);
                    else
                        FileUtil.CreateFloder(path);
                }
                return FromPath(path);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public DbSqlite(string connectString)
        : base(connectString) { }

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        => new SQLiteConnection(ConnectString);

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> args)
        {
            var list = new List<SQLiteParameter>();
            foreach (var keyValue in args)
                list.Add(new SQLiteParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        => throw new Exception("No SqliteDataAdapter");
    }
}
