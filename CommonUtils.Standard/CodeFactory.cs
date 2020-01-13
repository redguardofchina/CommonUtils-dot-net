using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 代码工厂
    /// </summary>
    public static class CodeFactory
    {
        #region TypeName
        /// <summary>
        /// 类型封装名 for 创建Class
        /// </summary>
        private static string GetPackageName(this Type type, bool forceNullable = false)
        {
            var mark = type.ToString();
            LogUtil.Print("GetPackageName from " + mark);
            switch (mark)
            {
                case "System.String":
                    return "string";

                case "System.String[]":
                    return "string[]";

                case "System.Boolean":
                    if (forceNullable)
                        return "bool?";
                    return "bool";

                case "System.Nullable`1[System.Boolean]":
                    return "bool?";

                case "System.Boolean[]":
                    if (forceNullable)
                        return "bool?[]";
                    return "bool[]";

                case "System.Nullable`1[System.Boolean][]":
                    return "bool?[]";

                case "System.Int32":
                    if (forceNullable)
                        return "int?";
                    return "int";

                case "System.Nullable`1[System.Int32]":
                    return "int?";

                case "System.Int32[]":
                    if (forceNullable)
                        return "int?[]";
                    return "int[]";

                case "System.Nullable`1[System.Int32][]":
                    return "int?[]";

                case "System.Int16":
                    if (forceNullable)
                        return "short?";
                    return "short";

                case "System.Nullable`1[System.Int16]":
                    return "short?";

                case "System.Int16[]":
                    if (forceNullable)
                        return "short?[]";
                    return "short[]";

                case "System.Nullable`1[System.Int16][]":
                    return "short?[]";

                case "System.Int64":
                    if (forceNullable)
                        return "long?";
                    return "long";

                case "System.Nullable`1[System.Int64]":
                    return "long?";

                case "System.Int64[]":
                    if (forceNullable)
                        return "long?[]";
                    return "long[]";

                case "System.Nullable`1[System.Int64][]":
                    return "long?[]";

                case "System.Single":
                    if (forceNullable)
                        return "float?";
                    return "float";

                case "System.Nullable`1[System.Single]":
                    return "float?";

                case "System.Single[]":
                    if (forceNullable)
                        return "float?[]";
                    return "float[]";

                case "System.Nullable`1[System.Single][]":
                    return "float?[]";

                case "System.Double":
                    if (forceNullable)
                        return "double?";
                    return "double";

                case "System.Nullable`1[System.Double]":
                    return "double?";

                case "System.Double[]":
                    if (forceNullable)
                        return "double?[]";
                    return "double[]";

                case "System.Nullable`1[System.Double][]":
                    return "double?[]";

                case "System.Decimal":
                    if (forceNullable)
                        return "decimal?";
                    return "decimal";

                case "System.Nullable`1[System.Decimal]":
                    return "decimal?";

                case "System.Decimal[]":
                    if (forceNullable)
                        return "decimal?[]";
                    return "decimal[]";

                case "System.Nullable`1[System.Decimal][]":
                    return "decimal?[]";

                case "System.Byte":
                    if (forceNullable)
                        return "byte?";
                    return "byte";

                case "System.Nullable`1[System.Byte]":
                    return "byte?";

                case "System.Byte[]":
                    if (forceNullable)
                        return "byte?[]";
                    return "byte[]";

                case "System.Nullable`1[System.Byte][]":
                    return "byte?[]";

                case "System.DateTime":
                    if (forceNullable)
                        return "DateTime?";
                    return "DateTime";

                case "System.Nullable`1[System.DateTime]":
                    return "DateTime?";

                case "System.DateTime[]":
                    if (forceNullable)
                        return "DateTime?[]";
                    return "DateTime[]";

                case "System.Nullable`1[System.DateTime][]":
                    return "DateTime?[]";

                case "System.Guid":
                    return "Guid";
            }
            ("GetPackageName no case: " + type.FullName).Print();
            return mark;
        }

        /// <summary>
        /// 类型数据库名 for 生成Sql创建语句
        /// </summary>
        private static string GetDatabaseName(this Type type)
        {
            if (type.IsArray)
                return "text";

            if (type.IsEnum)
                return "int";

            var mark = type.ToString();
            switch (mark)
            {
                case "System.String":
                    return "text";

                case "System.Boolean":
                    return "blob not null";

                case "System.Nullable`1[System.Boolean]":
                    return "blob";

                case "System.Int32":
                    return "int not null";

                case "System.Nullable`1[System.Int32]":
                    return "int";

                case "System.Int16":
                    return "smallint not null";

                case "System.Nullable`1[System.Int16]":
                    return "smallint";

                case "System.Int64":
                    return "bigint not null";

                case "System.Nullable`1[System.Int64]":
                    return "bigint";

                case "System.Single":
                    return "float not null";

                case "System.Nullable`1[System.Single]":
                    return "float";

                case "System.Double":
                    return "double not null";

                case "System.Nullable`1[System.Double]":
                    return "double";

                case "System.Decimal":
                    return "decimal not null";

                case "System.Nullable`1[System.Decimal]":
                    return "decimal";

                case "System.Byte":
                    return "binary not null";

                case "System.Nullable`1[System.Byte]":
                    return "binary";

                case "System.DateTime":
                    return "dateTime not null";

                case "System.Nullable`1[System.DateTime]":
                    return "dateTime";
            }
            Console.WriteLine("GetDatabaseName no case: " + type.FullName);
            return "json";
        }
        #endregion

        #region class
        /// <summary>
        /// 获取DataTable隐含的类结构
        /// </summary>
        /// <param name="compatible">值类型兼容,数据如果值类型允许为空，一般需要开启此选项</param>
        public static string GetClassCode(this DataTable table, string className = null, bool forceNullable = false)
        {
            if (className.IsNullOrEmpty())
                className = table.TableName;
            if (className.IsNullOrEmpty())
                className = "Class1";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine();
            sb.AppendLine("namespace CodeFactory");
            sb.AppendLine("{");
            sb.AppendLine("    public class " + className);
            sb.AppendLine("    {");
            foreach (DataColumn column in table.Columns)
                sb.AppendLine(string.Format("        public {0} {1} ", column.DataType.GetPackageName(forceNullable), column.ColumnName) + "{ get; set; }");
            sb.AppendLine("    }");
            sb.Append("}");
            ("GetClassCodeFromDataTable:\r\n\r\n" + sb).Print();
            return sb.ToString();
        }

        /// <summary>
        /// JArrayToClass
        /// </summary>
        public static string GetClassCode(this JArray jArray, string className = "Class1")
        => jArray.ToTable().GetClassCode(className);

        /// <summary>
        /// JObjectToClass
        /// </summary>
        public static string GetClassCode(this JObject jObject, string className = "Class1")
        {
            var array = new JArray();
            array.Add(jObject);
            return array.GetClassCode(className);
        }

        public static string GetClassCode(this JToken jToken, string className = "Class1")
        {
            if (jToken == null)
                return KeysUtil.Error;

            switch (jToken.Type)
            {
                case JTokenType.Array:
                    return ((JArray)jToken).GetClassCode(className);
                case JTokenType.Object:
                    return ((JObject)jToken).GetClassCode(className);
                default:
                    return default;
            }
        }

        /// <summary>
        /// 存储代码到Models文件夹
        /// </summary>
        private static void SaveToModels(string className, string classCode)
        {
            var path = PathUtil.GetFull("Models", className + ".cs");
            FileUtil.Save(path, classCode);
            Console.WriteLine(path);
        }

        /// <summary>
        /// 生成类代码
        /// </summary>
        /// <param name="jsonPath">输入绝对路径</param>
        public static void GenerateClassFromJsonPath(string jsonPath, string className = "GeneratedClassFromJsonPath")
        {
            var @struct = JsonUtil.GetJObjectFromFile(jsonPath);
            SaveToModels(className, @struct.GetClassCode(className));
        }

        public static void GenerateClassFromDataTable(DataTable table, string className = "GeneratedClassFromDataTable")
        => SaveToModels(className, table.GetClassCode(className));

        public static void GenerateClassFromJArray(JArray array, string className = "GeneratedClassFromJArray")
        => SaveToModels(className, array.GetClassCode(className));

        public static void GenerateClassFromJObject(JObject @struct, string className = "GeneratedClassFromJObject")
        => SaveToModels(className, @struct.GetClassCode(className));
        #endregion

        #region function
        class Function
        {
            public static bool Judge(string line, out string key, out bool isStruct)
            {
                key = null;
                isStruct = false;

                line = line.Trim();
                if (line.StartsWith("//"))
                    return false;

                line = line.SubstringStartByFirst('\'');
                line = line.SubstringStartByFirst('"');
                if (!line.Contains('('))
                    return false;

                line = line.SubstringStartByFirst('(');
                var cells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (cells.Contains("="))
                    return false;
                if (cells.Length < 2)
                    return false;

                isStruct = (cells.Length == 2) && (cells[0].Equals("private", "public", "static"));

                key = cells[cells.Length - 1];
                return true;
            }

            public string Define { get; private set; }

            public string Key { get; private set; }

            public bool IsStruct { get; private set; }

            public Function(string define, string key, bool isStruct)
            {
                Define = define;
                Key = key;
                IsStruct = isStruct;
            }

            private StringBuilder _codeComment = new StringBuilder();

            /// <summary>
            /// 获取注释，并记录插入点
            /// </summary>
            public int AddComment(List<string> lines)
            {
                while (lines.Count > 0)
                {
                    var lastIndex = lines.Count - 1;
                    var lastLine = lines[lastIndex];
                    var trimLine = lastLine.Trim();
                    if (string.IsNullOrEmpty(trimLine))
                    {
                        lines.RemoveAt(lastIndex);
                    }
                    else if (trimLine.StartsWith("//") || trimLine.StartsWith("["))
                    {
                        lines.RemoveAt(lastIndex);
                        _codeComment.Insert(0, lastLine + "\r\n");
                    }
                    else
                    {
                        break;
                    }
                }
                return lines.Count;
            }

            private List<string> _codeBody = new List<string>();

            private int _arrowCount;

            private int _semicolonCount;

            private int _leftBraceCount;

            private int _rightBraceCount;

            public bool CountKeyAndJudgeFinish(string line)
            {
                _codeBody.Add(line);

                line = line.Remove("\\\'", "\\\"");
                line = line.RemoveBetween("'", "'");
                line = line.RemoveBetween("\"", "\"");

                _arrowCount += line.CountOf("=>");
                _semicolonCount += line.CountOf(';');
                _leftBraceCount += line.CountOf('{');
                _rightBraceCount += line.CountOf('}');

                return (_arrowCount >= 1 && _semicolonCount >= 1) || ((_leftBraceCount > 0) && (_leftBraceCount == _rightBraceCount));
            }

            public string GetCode(FunctionOperation functionOperation = FunctionOperation.Default)
            {
                if (functionOperation == FunctionOperation.Simplify && _codeBody.Count == 4 && _codeBody[1].Trim() == "{" && _codeBody[3].Trim() == "}")
                {
                    _codeBody[2] = _codeBody[2].Trim();
                    if (_codeBody[2].StartsWith("return "))
                        _codeBody[2] = _codeBody[2].Remove(0, "return ".Length);
                    _codeBody[2] = _codeBody[1].Replace("{", "=> ") + _codeBody[2];
                    _codeBody.RemoveAt(1);
                    _codeBody.RemoveAt(2);
                }

                if (functionOperation == FunctionOperation.LambdaFormat)
                {
                    var space = _codeBody[0].GetStartSpace();
                    if (_codeBody.Count == 2)
                        _codeBody[1] = space + _codeBody[1].Trim();
                    if (_codeBody.Count == 1)
                        _codeBody[0] = _codeBody[0].Replace(" =>", "\r\n" + space + "=>");
                }

                return _codeComment + _codeBody.GetText();
            }
        }

        enum FunctionOperation
        {
            Default,
            Simplify,
            LambdaFormat
        }

        public static void FunctionSimplify(string path)
        {
            var code = FileUtil.GetText(path);
            var comments = code.GetBetweens("/*", "*/");
            code = code.Remove(comments);
            var lines = code.GetLines();
            var newCode = new List<string>();

            var hasFunction = false;
            var function = new Function("", "", false);

            foreach (var line in lines)
            {
                if (!hasFunction && Function.Judge(line, out string key, out bool isStruct))
                {
                    function = new Function(line, key, isStruct);
                    hasFunction = true;
                }

                if (hasFunction)
                {
                    if (function.CountKeyAndJudgeFinish(line))
                    {
                        newCode.Add(function.GetCode(FunctionOperation.Simplify));
                        hasFunction = false;
                        function.Key.Print();
                        function.GetCode().Print();
                        function.GetCode(FunctionOperation.Simplify).Print();
                    }
                    continue;
                }

                newCode.Add(line);
            }

            if (hasFunction)
            {
                ConsoleUtil.Print("Bug Function !");
                function.GetCode().Print();
            }

            foreach (var item in comments)
                newCode.Insert(0, item);

            FileUtil.Save(path, newCode.GetText());
        }

        public static void FunctionSort(string path)
        {
            var code = FileUtil.GetText(path);
            var comments = code.GetBetweens("/*", "*/");
            code = code.Remove(comments);
            var lines = code.GetLines();
            var newCode = new List<string>();

            var hasFunction = false;
            var function = new Function("", "", false);
            var functions = new List<Function>();
            var functionIndex = 0;

            foreach (var line in lines)
            {
                if (!hasFunction && Function.Judge(line, out string key, out bool isStruct))
                {
                    function = new Function(line, key, isStruct);
                    functionIndex = function.AddComment(newCode);
                    hasFunction = true;
                }

                if (hasFunction)
                {
                    if (function.CountKeyAndJudgeFinish(line))
                    {
                        functions.Add(function);
                        hasFunction = false;
                        function.Key.Print();
                        function.GetCode().Print();
                    }
                    continue;
                }

                newCode.Add(line);
            }

            if (hasFunction)
            {
                ConsoleUtil.Print("Bug Function !");
                function.GetCode().Print();
            }

            functions = functions.OrderByDescending(m => m.Define).OrderByDescending(m => m.Key).OrderBy(m => m.IsStruct).ToList();
            var firstLine = newCode[functionIndex - 1].Trim() == "{";
            for (int index = 0; index < functions.Count; index++)
            {
                var item = functions[index];
                newCode.Insert(functionIndex, item.GetCode());
                if (!firstLine || index != functions.Count - 1)
                    newCode.Insert(functionIndex, null);
            }

            foreach (var item in comments)
                newCode.Insert(0, item);

            FileUtil.Save(path, newCode.GetText());
        }

        public static void LambdaFunctionFormat(string path)
        {
            var code = FileUtil.GetText(path);
            var comments = code.GetBetweens("/*", "*/");
            code = code.Remove(comments);
            var lines = code.GetLines();
            var newCode = new List<string>();

            var hasFunction = false;
            var function = new Function("", "", false);

            foreach (var line in lines)
            {
                if (!hasFunction && Function.Judge(line, out string key, out bool isStruct))
                {
                    function = new Function(line, key, isStruct);
                    hasFunction = true;
                }

                if (hasFunction)
                {
                    if (function.CountKeyAndJudgeFinish(line))
                    {
                        newCode.Add(function.GetCode(FunctionOperation.LambdaFormat));
                        hasFunction = false;
                        function.Key.Print();
                        function.GetCode().Print();
                        function.GetCode(FunctionOperation.LambdaFormat).Print();
                    }
                    continue;
                }

                newCode.Add(line);
            }

            if (hasFunction)
            {
                ConsoleUtil.Print("Bug Function !");
                function.GetCode().Print();
            }

            foreach (var item in comments)
                newCode.Insert(0, item);

            FileUtil.Save(path, newCode.GetText());
        }
        #endregion

        #region sql
        /// <summary>
        /// 生成Sql创建语句
        /// </summary>
        public static string GenerateCreateSql<T>()
        {
            Type type = typeof(T);
            var sb = new StringBuilder();
            sb.AppendFormatLine("CREATE TABLE `{0}` (", type.Name);
            var properties = type.GetProperties();
            for (int index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                if (index != 0)
                    sb.AppendLine(",");
                sb.AppendFormat(" `{0}` {1}", property.Name, property.PropertyType.GetDatabaseName());
            }
            sb.AppendLine();
            sb.AppendLine(");");
            return sb.ToString();
        }

        /// <summary>
        /// CsvToSql
        /// </summary>
        public static string GenerateInsertSql(string csvPath, string tableName = "@tableName")
        {
            string[] lines = FileUtil.ReadLines(csvPath);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("insert into {0} values ", tableName));
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                sb.Append('(');

                var cells = line.Split(',');
                for (int subIndex = 0; subIndex < cells.Length; subIndex++)
                {
                    var cell = cells[subIndex];
                    if (subIndex != cells.Length - 1)
                        sb.Append(string.Format("'{0}',", cell));
                    else
                        sb.Append(string.Format("'{0}'", cell));
                }

                if (index != lines.Length - 1)
                    sb.AppendLine("),");
                else
                    sb.AppendLine(");");
            }
            return sb.ToString();
        }
        #endregion

        #region project
        /// <summary>
        /// 获取sln包含的项目
        /// </summary>
        public static Dictionary<string, string> GetSlnFloders(string path)
        {
            var sln = FileUtil.GetInfo(path);
            var content = sln.GetText();
            var lines = content.Substrings("\", \"", ".csproj\", \"", false);
            var floders = new Dictionary<string, string>();
            foreach (var line in lines)
                floders.Add(FileUtil.GetName(line), FileUtil.GetFloder(FileUtil.GetFloder(path).Combine(line)));
            return floders;
        }

        /// <summary>
        /// 删除注释 用于反编译出来的代码的无用注释
        /// </summary>
        public static void RemoveComment(string floderPath, string extension = ".java")
        {
            var floder = FloderUtil.GetInfo(floderPath);
            var files = floder.GetAllFiles();
            foreach (var file in files)
            {
                if (file.Extension != extension)
                    continue;
                var text = file.GetText();
                file.Save(text.RemoveBetween("/*", "*/"));
            }
        }

        /// <summary>
        /// 框架升级 2.0-2.2
        /// </summary>
        public static void UpgradeFrame(string floder)
        {
            var files = FloderUtil.GetInfo(floder).GetAllFiles();
            files = files.Where(m => m.Extension == ".pubxml" || m.Extension == ".csproj").ToArray();
            foreach (var file in files)
                file.Save(file.GetText()
                    .Replace("netcoreapp2.0", "netcoreapp2.2")
                    .Replace("Microsoft.AspNetCore.All", "Microsoft.AspNetCore.App_deleteversion")
                    .Replace("Microsoft.VisualStudio.Web.CodeGeneration.Design", "_deleteline")
                    .Replace("Microsoft.VisualStudio.Web.CodeGeneration.Tools", "_deleteline"));
        }
        #endregion

        #region test
        /// <summary>
        /// 测试类
        /// </summary>
        private class TestClass
        {
            public string @string { get; set; }
            //public string? @string_ { get; set; }
            public string[] @stringArray { get; set; }
            //public string?[] @string_Array { get; set; }

            public bool @bool { get; set; }
            public bool? @bool_ { get; set; }
            public bool[] @boolArray { get; set; }
            public bool?[] @bool_Array { get; set; }

            public int @int { get; set; }
            public int? @int_ { get; set; }
            public int[] @intArray { get; set; }
            public int?[] @int_Array { get; set; }

            public short @short { get; set; }
            public short? @short_ { get; set; }
            public short[] @shortArray { get; set; }
            public short?[] @short_Array { get; set; }

            public long @long { get; set; }
            public long? @long_ { get; set; }
            public long[] @longArray { get; set; }
            public long?[] @long_Array { get; set; }

            public float @float { get; set; }
            public float? @float_ { get; set; }
            public float[] @floatArray { get; set; }
            public float?[] @float_Array { get; set; }

            public double @double { get; set; }
            public double? @double_ { get; set; }
            public double[] @doubleArray { get; set; }
            public double?[] @double_Array { get; set; }

            public decimal @decimal { get; set; }
            public decimal? @decimal_ { get; set; }
            public decimal[] @decimalArray { get; set; }
            public decimal?[] @decimal_Array { get; set; }

            public byte @byte { get; set; }
            public byte? @byte_ { get; set; }
            public byte[] @byteArray { get; set; }
            public byte?[] @byte_Array { get; set; }

            public DateTime DateTime { get; set; }
            public DateTime? DateTime_ { get; set; }
            public DateTime[] DateTimeArray { get; set; }
            public DateTime?[] DateTime_Array { get; set; }
        }

        /// <summary>
        /// 生成Switch.Case
        /// </summary>
        public static string GenerateCasesOfSwitch()
        {
            var type = typeof(TestClass);
            var sb = new StringBuilder();
            foreach (var pro in type.GetProperties())
                sb.AppendFormatLine("case \"{0}\":\r\nreturn \"{1}\";\r\n", pro.PropertyType, pro.Name.Replace('_', '?').Replace("Array", "[]"));
            return sb.ToString();
        }
        #endregion
    }
}
