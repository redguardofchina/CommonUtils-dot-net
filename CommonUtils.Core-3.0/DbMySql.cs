using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for MySql
    /// </summary>
    public class DbMySql : DbBase
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static class ConnectStrings
        {
            public static string Get(string host, string user, string pwd, string database, int port = 3306)
            => string.Format("server={0};port={1};user={2};pwd={3};database={4};sslmode=none;", host, port, user, pwd, database);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public DbMySql(string connectString)
        : base(connectString) { }

        public DbMySql(string host, string user, string pwd, string database, int port = 3306)
        : this(ConnectStrings.Get(host, user, pwd, database, port)) { }

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        => new MySqlConnection(ConnectString);

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> args)
        {
            var list = new List<MySqlParameter>();
            foreach (var keyValue in args)
                list.Add(new MySqlParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        => new MySqlDataAdapter((MySqlCommand)cmd);

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

        public override bool Exist<T>(T @struct, string tableName, string keyName)
        {
            var type = typeof(T);
            var sql = string.Format("select count(*) from `{0}` where `{1}`=@key;", tableName, keyName);
            return SelectIntger(sql, new MapStringObject("@key", type.GetProperty(keyName).GetValue(@struct))) > 0;
        }

        public override bool Insert<T>(T @struct, string tableName)
        {
            var type = typeof(T);
            var sets = new StringBuilder();
            var args = new MapStringObject();
            foreach (var property in type.GetProperties())
            {
                args.Add('@' + property.Name, property.GetValue(@struct));

                if (sets.Length > 0)
                    sets.Append(',');

                sets.Append('`');
                sets.Append(property.Name);
                sets.Append('`');
                sets.Append('=');
                sets.Append('@');
                sets.Append(property.Name);
            }
            var sql = string.Format("insert into `{0}` set {1};", tableName, sets);
            return GetExecuteResult(sql, args);
        }

        public override bool IgnoreOrInsert<T>(T @struct, string tableName, string keyName)
        {
            if (Exist(@struct, tableName, keyName))
                return false;
            return Insert(@struct, tableName);
        }

        public override bool Update<T>(T @struct, string tableName, string keyName, params string[] updateIgnoreKeyNames)
        {
            var type = typeof(T);
            var sets = new StringBuilder();
            var args = new MapStringObject();
            foreach (var property in type.GetProperties())
            {
                if (updateIgnoreKeyNames.Contains(property.Name))
                    continue;

                args.Add('@' + property.Name, property.GetValue(@struct));

                if (property.Name == keyName)
                    continue;

                if (sets.Length > 0)
                    sets.Append(',');

                sets.Append('`');
                sets.Append(property.Name);
                sets.Append('`');
                sets.Append('=');
                sets.Append('@');
                sets.Append(property.Name);

            }
            var sql = string.Format("update `{0}` set {1} where `{2}`=@{2};", tableName, sets, keyName);
            return GetExecuteResult(sql, args);
        }

        public override bool UpdateOrInsert<T>(T @struct, string tableName, string keyName, params string[] updateIgnoreKeyNames)
        {
            if (Exist(@struct, tableName, keyName))
                return Update(@struct, tableName, keyName, updateIgnoreKeyNames);
            return Insert(@struct, tableName);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        public override void BulkInsert(DataTable table, string tableName)
        {
            string columns = "";
            int columnCount = table.Columns.Count;
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                if (columnIndex != 0)
                {
                    columns += ",";
                }
                columns += "`" + table.Columns[columnIndex].ColumnName + "`";
            }
            string values = "";
            int rowCount = table.Rows.Count;
            Dictionary<string, object> args = new Dictionary<string, object>();
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                if (rowIndex != 0)
                {
                    values += ",";
                }
                values += "(";
                DataRow dr = table.Rows[rowIndex];
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    string parameterName = "@r" + rowIndex + "c" + columnIndex;
                    args.Add(parameterName, dr[columnIndex]);
                    if (columnIndex != 0)
                    {
                        values += ",";
                    }
                    values += parameterName;
                }
                values += ")";

            }
            string sql = "insert into `" + tableName + "` (" + columns + ") values " + values + ";";
            GetExecuteNonQuery(sql, args);
        }

        public static DbMySql _testDb = new DbMySql("127.0.0.1", "root", "123456", "mysql", 3306);

        public static void ShowLogState()
        {
            _testDb.GetDataTable("show variables like 'general_log';").PrintJson();
            _testDb.GetDataTable("show variables like 'log_output';").PrintJson();
            _testDb.GetDataTable("show variables like 'general_log_file';").PrintJson();
        }

        public static void OpenLog()
        => _testDb.GetExecuteNonQuery("set global general_log = 'ON';");

        public static void CloseLog()
        => _testDb.GetExecuteNonQuery("set global general_log = 'OFF';");

        public static void SetLogOutputToTable()
        => _testDb.GetExecuteNonQuery("set global log_output = 'table';");

        public static void SetLogOutputToFile()
        {
            _testDb.GetExecuteNonQuery("set global general_log_file='D:/mysql.log';");
            _testDb.GetExecuteNonQuery("set global log_output='file';");
        }
    }
}
