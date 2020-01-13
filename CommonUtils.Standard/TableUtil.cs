using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 专门处理DataTable
    /// </summary>
    public static class TableUtil
    {
        /// <summary>
        /// ToArray
        /// </summary>
        public static T[] ToArray<T>(this DataTable table)
        {
            return JsonUtil.Deserialize<T[]>(table.ToJson(true));
        }

        /// <summary>
        /// ToJArray
        /// </summary>
        public static JArray ToJArray<T>(this DataTable table)
        {
            return JsonUtil.ParseToJArray(table.ToJson(true));
        }

        /// <summary>
        /// ToTable
        /// </summary>
        public static DataTable ToTable<T>(this IEnumerable<T> array)
        {
            var arrayJson = array.ToJson(true);
            var table = JsonUtil.Deserialize<DataTable>(arrayJson);
            if (table == null)
                throw new Exception("【array to table exception】");
            return table;
        }

        /// <summary>
        /// ToTable
        /// </summary>
        public static DataTable ToTable<T>(this JArray array)
        {
            return JsonUtil.Deserialize<DataTable>(array.ToJson(true));
        }

        /// <summary>
        /// 删除标题中的_,引用类型不用return,return是为了兼容连写
        /// </summary>
        public static DataTable RemoveTitileUnderline(this DataTable table)
        {
            for (int index = 0; index < table.Columns.Count; index++)
                table.Columns[index].ColumnName = StringUtil.Remove(table.Columns[index].ColumnName, '_');
            return table;
        }

        #region DataRow
        /// <summary>
        /// 获取object
        /// </summary>
        public static object Get(this DataRow row, string columnName)
        {
            try
            {
                return row[columnName];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取String
        /// </summary>
        public static string GetString(this DataRow row, string columnName)
        {
            return row.Get(columnName).ToString();
        }

        /// <summary>
        /// 获取Int
        /// </summary>
        public static int GetInt(this DataRow row, string columnName)
        {
            return row.Get(columnName).ToInt();
        }

        /// <summary>
        /// 获取Long
        /// </summary>
        public static long GetLong(this DataRow row, string columnName)
        {
            return row.Get(columnName).ToLong();
        }

        /// <summary>
        /// 获取Long
        /// </summary>
        public static float GetFloat(this DataRow row, string columnName)
        {
            return row.Get(columnName).ToFloat();
        }

        /// <summary>
        /// 获取Bool
        /// </summary>
        public static bool GetBool(this DataRow row, string columnName)
        {
            return row.Get(columnName).ToBool();
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        public static DateTime GetTime(this DataRow row, string columnName)
        {
            return TimeUtil.Parse(row.Get(columnName));
        }

        /// <summary>
        /// 获取二进制
        /// </summary>
        public static byte[] GetBytes(this DataRow row, string columnName)
        {
            return row.Get(columnName) as byte[];
        }

        /// <summary>
        /// 获取枚举
        /// </summary>
        public static T GetEnum<T>(this DataRow row, string columnName)
        {
            return row.Get(columnName).ToEnum<T>();
        }
        #endregion
    }
}
