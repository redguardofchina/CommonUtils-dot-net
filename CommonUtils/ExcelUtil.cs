using System.Collections.Generic;
using System.Data;

namespace CommonUtils
{
    /// <summary>
    /// 访问Excel
    /// </summary>
    public static class ExcelUtil
    {
        //public static DataTable GetDataTable(string path)
        //{
        //    var odbc = new DbOdbc(DbOdbc.ConnectStrings.MicrosoftExcelDriver(path));
        //    var tableNames = odbc.GetTableNames();
        //    return odbc.GetDataTable("select * from " + tableNames[0]);
        //}

        /// <summary>
        /// 从Excel中获取DataTable
        /// </summary>
        public static DataTable GetDataTable(string path, bool ifCache = true, string tableName = null)
        {
            //读缓存
            string mark = "GetDataTable" + path + tableName;
            DataTable table = null;
            if (ifCache)
                table = CacheUtil.Get<DataTable>(mark);
            if (table != null)
                return table;

            //获取连接字符串
            bool oldVersion = StringUtil.EqualsWithoutCase(FileUtil.GetExtension(path), ".xls");
            string connectString;
            if (oldVersion)
                connectString = DbOle.ConnectStrings.ExcelOldReadOnly(path);
            else
                connectString = DbOle.ConnectStrings.ExcelNewReadOnly(path);

            //获取数据
            DbOle db = new DbOle(connectString);
            if (string.IsNullOrEmpty(tableName))
                tableName = db.GetTableNames()[0];
            else
                tableName += "$";
            table = db.GetDataTable(string.Format("select * from [{0}]", tableName));

            //写缓存
            if (ifCache)
                CacheUtil.Save(mark, table);
            return table;
        }

        /// <summary>
        /// 从Excel中获取DataTables
        /// </summary>
        public static List<DataTable> GetDataTables(string path, bool ifCache = true)
        {
            //读缓存
            string mark = "GetDataTables" + path;
            List<DataTable> list = null;
            if (ifCache)
                list = CacheUtil.Get<List<DataTable>>(mark);
            if (list != null)
                return list;

            //获取连接字符串
            bool Is03 = FileUtil.GetExtension(path).ToLower() == ".xls";
            string connectString;
            if (Is03)
                connectString = DbOle.ConnectStrings.ExcelOldReadOnly(path);
            else
                connectString = DbOle.ConnectStrings.ExcelNewReadOnly(path);

            //获取数据
            DbOle db = new DbOle(connectString);
            list = new List<DataTable>();
            foreach (string tableName in db.GetTableNames())
            {
                DataTable table = db.GetDataTable(string.Format("select * from [{0}]", tableName));
                table.TableName = tableName;
                list.Add(table);
            }

            //写缓存
            if (ifCache)
                CacheUtil.Save(mark, list);
            return list;
        }
    }
}
