using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for OleDb
    /// </summary>
    public class DbOle : DbBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public DbOle(string connectString) : base(connectString) { }


        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        {
            return new OleDbConnection(ConnectString);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> map)
        {
            var list = new List<OleDbParameter>();
            foreach (var keyValue in map)
                list.Add(new OleDbParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            return new OleDbDataAdapter((OleDbCommand)cmd);
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        public override string[] GetTableNames()
        {
            OleDbConnection conn = new OleDbConnection(ConnectString);
            conn.Open();
            DataTable dt = conn.GetSchema("Tables");
            conn.Close();
            List<string> listName = new List<string>();
            string name;
            string[] sysKeys = { "MSys", "$'_", "$'Print_", "_xlnm" };
            bool isTableName;
            foreach (DataRow dr in dt.Rows)
            {
                name = dr["TABLE_NAME"].ToString();
                isTableName = true;
                foreach (string sysKey in sysKeys)
                {
                    if (name.Contains(sysKey))
                    {
                        isTableName = false;
                        break;
                    }
                }

                if (isTableName)
                    listName.Add(name);
            }
            return listName.ToArray();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        public override void BulkInsert(DataTable table, string tableName)
        {
            int colCount = table.Columns.Count;
            string columns = "";
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                if (colIndex != 0)
                {
                    columns += ",";
                }
                columns += "[" + table.Columns[colIndex].ColumnName + "]";
            }
            int rowCount = table.Rows.Count;
            OleDbConnection conn = new OleDbConnection(ConnectString);
            conn.Open();
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                string values = "";
                MapStringObject map = new MapStringObject();
                for (int colIndex = 0; colIndex < colCount; colIndex++)
                {
                    string value = "@r" + rowIndex + "c" + colIndex;
                    map.Add(value, table.Rows[rowIndex][colIndex]);
                    if (colIndex != 0)
                    {
                        values += ",";
                    }
                    values += value;
                }
                string sql = "insert into [" + tableName + "] (" + columns + ") values (" + values + ");";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddRange(GetParameters(map));
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        /// <summary>
        /// 连接字符串
        /// 没有安装office可以安装microsoft office access database engine
        /// AccessDatabaseEngine
        /// 推荐下载2010版32位
        /// https://www.microsoft.com/zh-cn/download/confirmation.aspx?id=13255
        /// </summary>
        public class ConnectStrings
        {
            /// <summary>
            /// Access07
            /// </summary>
            public static string AccessNew(string path)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", path);
            }

            /// <summary>
            /// Access03
            /// </summary>
            public static string AccessOld(string path)
            {
                return string.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source='{0}';", path);
            }

            /// <summary>
            /// Excel07 读取
            /// </summary>
            public static string ExcelNewReadOnly(string path)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';", path);
            }

            /// <summary>
            /// Excel07 读写
            /// </summary>
            public static string ExcelNewReadWrite(string path)
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=YES;IMEX=0;';", path);
            }

            /// <summary>
            /// Excel03 读取
            /// </summary>
            public static string ExcelOldReadOnly(string path)
            {
                return string.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=YES;IMEX=1;';", path);
            }

            /// <summary>
            /// Excel03 读写
            /// </summary>
            public static string ExcelOldReadWrite(string path)
            {
                return string.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=YES;IMEX=0;';", path);
            }

            /// <summary>
            /// Paradox引擎，传奇，征途等
            /// </summary>
            public static string Paradox(string floder)
            {
                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Paradox 5.x;", floder);
            }
        }
    }
}
