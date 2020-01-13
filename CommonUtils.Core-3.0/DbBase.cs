using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// ADO.Net Base
    /// </summary>
    public abstract class DbBase
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected string ConnectString { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        protected DbBase(string connectString)
        => ConnectString = connectString;

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected abstract DbConnection GetConnection();

        /// <summary>
        /// 连接测试
        /// </summary>
        public string ConnectTest()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(ConnectString);
            result.AppendLine("Db Connect Test...");
            var connection = GetConnection();
            try
            {
                connection.Open();
                connection.Close();
                result.AppendLine("Db Connect Success.");
            }
            catch (Exception ex)
            {
                result.AppendLine("Db Connect Fail:" + ex.Message);
            }
            finally
            {
                connection.Dispose();
            }
            Console.WriteLine(result);
            return result.ToString();
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        protected abstract DbParameter[] GetParameters(Dictionary<string, object> args);

        /// <summary>
        /// 获取Command
        /// </summary>
        private DbCommand GetCommand(string sql, Dictionary<string, object> args, out DbConnection connection)
        {
            connection = GetConnection();
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            if (args != null)
                cmd.Parameters.AddRange(GetParameters(args));
            sql.AppendLine("args:" + args.ToJson(true)).Print();
            return cmd;
        }

        /// <summary>
        /// 获取执行行数
        /// </summary>
        public int GetExecuteNonQuery(string sql, Dictionary<string, object> args = null)
        {
            var cmd = GetCommand(sql, args, out var connection);
            connection.Open();
            var count = cmd.ExecuteNonQuery();
            connection.Close();
            return count;
        }

        /// <summary>
        /// 判断影响行数是否大于0
        /// </summary>
        public bool GetExecuteResult(string sql, Dictionary<string, object> args = null)
        => GetExecuteNonQuery(sql, args) > 0;

        /// <summary>
        /// 获取单个数据
        /// </summary>
        private object GetExecuteScalar(string sql, Dictionary<string, object> args)
        {
            var cmd = GetCommand(sql, args, out var connection);
            connection.Open();
            var value = cmd.ExecuteScalar();
            connection.Close();
            return value;
        }

        /// <summary>
        /// 获取单个数据
        /// </summary>
        public object SelectValue(string sql, Dictionary<string, object> args = null)
        => GetExecuteScalar(sql, args);

        /// <summary>
        /// 获取单个字符串
        /// </summary>
        public string SelectString(string sql, Dictionary<string, object> args = null)
        => GetExecuteScalar(sql, args).ToString();

        /// <summary>
        /// 获取单个数字
        /// </summary>
        public int SelectIntger(string sql, Dictionary<string, object> args = null)
        => GetExecuteScalar(sql, args).ToInt();

        public object GetValue(string sql, Dictionary<string, object> args = null)
       => SelectValue(sql, args);

        /// <summary>
        /// 获取单个字符串
        /// </summary>
        public string GetString(string sql, Dictionary<string, object> args = null)
        => SelectString(sql, args).ToString();

        /// <summary>
        /// 获取单个数字
        /// </summary>
        public int GetIntger(string sql, Dictionary<string, object> args = null)
        => SelectIntger(sql, args).ToInt();

        /// <summary>
        /// 获取数据指针（需关闭）
        /// </summary>
        public DbDataReader GetExecuteReader(string sql, Dictionary<string, object> args = null)
        {
            var cmd = GetCommand(sql, args, out var connection);
            connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected abstract DbDataAdapter GetDataAdapter(DbCommand cmd);

        /// <summary>
        /// 获取数据表
        /// </summary>
        //public DataTable GetDataTable(string sql, Dictionary<string, object> args = null)
        //{
        //    var cmd = GetCommand(sql, args, out var connection);
        //    //connection没开
        //    var adapter = GetDataAdapter(cmd);
        //    var table = new DataTable();
        //    adapter.Fill(table);
        //    return table;
        //}
        public DataTable GetDataTable(string sql, Dictionary<string, object> args = null)
        {
            var reader = GetExecuteReader(sql, args);

            var table = new DataTable();
            for (int index = 0; index < reader.FieldCount; index++)
            {
                //reader.GetFieldType(index).Print();
                table.Columns.Add(reader.GetName(index), reader.GetFieldType(index));
            }

            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                table.Rows.Add(values);
            }

            reader.Close();
            return table;
        }

        /// <summary>
        /// 获取数组
        /// </summary>
        public JArray GetJArray(string sql, Dictionary<string, object> args = null)
        {
            var reader = GetExecuteReader(sql, args);
            var jArray = new JArray();
            while (reader.Read())
            {
                var jObject = new JObject();
                for (int index = 0; index < reader.FieldCount; index++)
                    jObject.Put(reader.GetName(index), reader.GetValue(index));
                jArray.Add(jObject);
            }
            reader.Close();
            return jArray;
        }

        /// <summary>
        /// 获取数组
        /// </summary>
        //public T[] GetClassArray<T>(string sql, Dictionary<string, object> args = null)
        //=> GetDataTable(sql, args).ToArray<T>();
        public T[] GetClassArray<T>(string sql, Dictionary<string, object> args = null)
        => GetJArray(sql, args).ToObject<T[]>();

        /// <summary>
        /// 获取数据集
        /// </summary>
        [Obsolete]
        public DataSet GetDataSet(string sql, Dictionary<string, object> args = null)
        {
            var cmd = GetCommand(sql, args, out var connection);
            //connection没开
            var adapter = GetDataAdapter(cmd);
            var set = new DataSet();
            adapter.Fill(set);
            adapter.Dispose();
            return set;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        public virtual string[] GetTableNames()
        => throw new NotImplementedException();

        /// <summary>
        /// 插入类（属性名与字段名一致）
        /// </summary>
        public virtual bool Insert<T>(T @struct, string tableName)
        => throw new NotImplementedException();

        public virtual bool Update<T>(T @struct, string tableName, string keyName, params string[] updateIgnoreKeyNames)
        => throw new NotImplementedException();

        public virtual bool Exist<T>(T @struct, string tableName, string keyName)
        => throw new NotImplementedException();

        public virtual bool UpdateOrInsert<T>(T @struct, string tableName, string keyName, params string[] updateIgnoreKeyNames)
        => throw new NotImplementedException();

        public virtual bool IgnoreOrInsert<T>(T @struct, string tableName, string keyName)
        => throw new NotImplementedException();

        /// <summary>
        /// 批量插入
        /// </summary>
        public virtual void BulkInsert(DataTable table, string tableName)
        {
            //模板 insert into test values (1,'1','1'),(1,'1','2'),(1,'1','3');
            var sql = new StringBuilder(string.Format("insert into {0} values ", tableName));
            var @params = new Dictionary<string, object>();
            var columnCount = table.Columns.Count;
            var rowCount = table.Rows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                if (rowIndex != 0)
                    sql.Append(',');
                sql.Append('(');
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    if (columnIndex != 0)
                        sql.Append(',');
                    var name = string.Format("@r{0}c{1}", rowIndex, columnIndex);
                    sql.Append(name);
                    @params.Add(name, table.Rows[rowIndex][columnIndex]);
                }
                sql.Append(')');
            }
            sql.Append(';');
            GetExecuteResult(sql.ToString(), @params);
        }
    }
}
