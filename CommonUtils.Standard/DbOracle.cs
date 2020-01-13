using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data.Common;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for Oracle
    /// </summary>
    public class DbOracle : DbBase
    {
        public static class ConnectStrings
        {
            public static string Get(string host, string database, string user, string pwd)
            => string.Format("Data Source={0}/{1};User ID={2};Password={3};", host, database, user, pwd);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public DbOracle(string connectString)
        : base(connectString) { }

        public DbOracle(string host, string database, string user, string pwd)
        : this(ConnectStrings.Get(host, database, user, pwd)) { }

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        => new OracleConnection(ConnectString);

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> args)
        {
            var list = new List<OracleParameter>();
            foreach (var keyValue in args)
                list.Add(new OracleParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        => new OracleDataAdapter((OracleCommand)cmd);
    }
}
