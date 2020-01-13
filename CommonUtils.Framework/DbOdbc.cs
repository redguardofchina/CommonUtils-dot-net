using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// ADO.NET for Odbc
    /// </summary>
    public class DbOdbc : DbBase
    {
        /// <summary>
        /// 打印所有驱动
        /// </summary>
        public static void PrintDrivers()
        {
            var info = new StringBuilder();
            info.AppendLine("-----------------------------WOW6432Node-------------------------");
            info.AppendLines(RegeditUtil.GetItemNames(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\ODBC\ODBCINST.INI\ODBC Drivers"));
            info.AppendLine();
            info.AppendLine("-----------------------------DEFAULT-------------------------");
            info.AppendLines(RegeditUtil.GetItemNames(@"HKEY_LOCAL_MACHINE\SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers"));
            info.RemoveEnd();
            info.Print();
        }

        /// <summary>
        /// 连接字符串
        /// vale/path兼容空格，不要用""''
        /// </summary>
        public class ConnectStrings
        {
            /// <summary>
            /// 通过名字获取连接字符串
            /// <para>如果使用Paradox，需要安装DB Commander 2000 RPO后驱动才能使用</para>
            /// </summary>
            /// <param name="name">系统中配置过的ODBC</param>
            public static string DsnName(string name)
            => string.Format("Dsn={0};", name);

            /*
            * Driver名必须与控制面板ODBC中的完全一致，尤其是空格
            * 比如 Driver={Microsoft Access Driver (*.mdb, *.accdb) 【, *】中有空格
            * 比如 Microsoft Paradox Driver (*.db ) 【(*.db )】中有空格
            * 注册表HKEY_LOCAL_MACHINE\SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers中可复制具体名称
            */

            /// <summary>
            /// 通过驱动名和文件路径获取连接字符串
            /// </summary>
            /// <param name="driver">驱动名</param>
            /// <param name="path">文件(夹)路径</param>
            public static string Diver(string driver, string path)
            => string.Format("Driver={0};Dbq={1};", driver, path);

            /// <summary>
            /// *.xls, *.xlsx, *.xlsm, *.xlsb
            /// Microsoft Access Database Engine Required
            /// </summary>
            public static string MicrosoftExcelDriver(string path)
            => Diver("Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)", path);

            /// <summary>
            /// *.mdb, *.accdb
            /// Microsoft Access Database Engine Required
            /// </summary>
            public static string MicrosoftAccessDriver(string path)
            => Diver("Microsoft Access Driver (*.mdb, *.accdb)", path);

            /// <summary>
            /// *.db floder不要以斜杠结尾
            /// Windows默认集成 但只支持32位模式，目标平台需切换为x86，调试需安装32位sdk，运行需要32位runtime
            /// driver模式不好用 改用DsnName模式 放弃吧 只能取TableNames 取不到数据
            /// 在 ODBC新建 中可以查看引擎版本
            /// 好像安装了DB Commander 2000 RPO后就正常了
            /// </summary>
            public static string MicrosoftParadoxDriver(string floder)
            => Diver("Microsoft Paradox Driver (*.db )", floder);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public DbOdbc(string connectString)
        : base(connectString) { }

        /// <summary>
        /// 获取连接器
        /// </summary>
        protected override DbConnection GetConnection()
        => new OdbcConnection(ConnectString);

        /// <summary>
        /// 获取参数
        /// </summary>
        protected override DbParameter[] GetParameters(Dictionary<string, object> args)
        {
            var list = new List<OdbcParameter>();
            foreach (var keyValue in args)
                list.Add(new OdbcParameter(keyValue.Key, keyValue.Value));
            return list.ToArray();
        }

        /// <summary>
        /// 获取数据器
        /// </summary>
        protected override DbDataAdapter GetDataAdapter(DbCommand cmd)
        => new OdbcDataAdapter((OdbcCommand)cmd);

        private static string[] _sysKeys = { "$", "MSys", "_xlnm" };

        /// <summary>
        /// 获取所有表名
        /// </summary>
        public override string[] GetTableNames()
        {
            var conn = GetConnection();
            conn.Open();
            var table = conn.GetSchema("Tables");
            conn.Close();
            var tableNames = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                var name = row["TABLE_NAME"].ToString();
                if (!name.Contains(_sysKeys))
                    tableNames.Add(name);
            }
            return tableNames.ToArray();
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
                    columns += ",";
                columns += "[" + table.Columns[colIndex].ColumnName + "]";
            }
            int rowCount = table.Rows.Count;
            OdbcConnection conn = new OdbcConnection(ConnectString);
            conn.Open();
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                string values = "";
                Dictionary<string, object> args = new Dictionary<string, object>();
                for (int colIndex = 0; colIndex < colCount; colIndex++)
                {
                    string value = "@r" + rowIndex + "c" + colIndex;
                    args.Add(value, table.Rows[rowIndex][colIndex]);
                    if (colIndex != 0)
                        values += ",";
                    values += value;
                }
                string sql = "insert into [" + tableName + "] (" + columns + ") values (" + values + ");";
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                cmd.Parameters.AddRange(GetParameters(args));
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}
