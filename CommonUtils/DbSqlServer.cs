using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for SqlServer
    /// </summary>
    public class DbSqlServer : DbBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public DbSqlServer(string connectString)
        : base(connectString) { }

        public DbSqlServer(string host, string user, string password, string database)
        : this(string.Format("Data Source={0};User ID={1};pwd={2};Initial Catalog={3};", host, user, password, database)) { }

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        => new SqlConnection(ConnectString);

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> args)
        {
            var list = new List<SqlParameter>();
            foreach (var keyValue in args)
                list.Add(new SqlParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        => new SqlDataAdapter((SqlCommand)cmd);

        /// <summary>
        /// 获取表名
        /// </summary>
        public override string[] GetTableNames()
        {
            string sql = "show tables;";
            List<string> names = new List<string>();
            DbDataReader dr = GetExecuteReader(sql);
            while (dr.Read())
                names.Add(dr[0].ToString());
            return names.ToArray();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        public override void BulkInsert(DataTable table, string tableName)
        {
            using (SqlBulkCopy bulk = new SqlBulkCopy(ConnectString))
            {
                bulk.BatchSize = table.Rows.Count;
                bulk.DestinationTableName = tableName;
                foreach (DataColumn dc in table.Columns)
                {
                    bulk.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                bulk.WriteToServer(table);
            }
        }
    }
}
