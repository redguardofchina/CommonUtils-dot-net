using Npgsql;
using System.Collections.Generic;
using System.Data.Common;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for Npgsql
    /// </summary>
    public class DbNpgsql : DbBase
    {
        public class ConnectStrings
        {
            public static string Get(string host, string password, string database, int port = 5432)
            => string.Format("host={0};port={1};username=postgres;password={2};database={3};", host, port, password, database);
        }

        public DbNpgsql(string connectString)
        : base(connectString) { }

        public DbNpgsql(string host, string password, string database, int port = 5432)
        : this(ConnectStrings.Get(host, password, database, port)) { }

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        => new NpgsqlConnection(ConnectString);

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> args)
        {
            var list = new List<NpgsqlParameter>();
            foreach (var keyValue in args)
                list.Add(new NpgsqlParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        => new NpgsqlDataAdapter((NpgsqlCommand)cmd);
    }
}
