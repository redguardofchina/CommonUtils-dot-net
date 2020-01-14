using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CommonUtils
{
    /// <summary>
    /// 反射处理
    /// </summary>
    public static class ReflectionUtil
    {
        public static string GetNamespace(Type type)
        => type.Namespace;

        public static string GetNamespace<T>()
        => GetNamespace(typeof(T));

        /// <summary>
        /// 深复制（字段属性）
        /// </summary>
        public static void FillMembers<T>(this T target, T source, params string[] ignores) where T : class
        {
            var type = typeof(T);
            var members = type.GetMembers();
            foreach (var member in members)
            {
                //log
                //member.Print();
                //member.MemberType.Print();
                //Console.WriteLine();

                if (ignores.Contains(member.Name))
                    continue;

                switch (member.MemberType)
                {
                    //字段
                    case MemberTypes.Field:
                        var fieldInfo = member.As<FieldInfo>();
                        var fieldValue = fieldInfo.GetValue(source);
                        fieldInfo.SetValue(target, fieldValue);
                        break;

                    //属性
                    case MemberTypes.Property:
                        var propertyInfo = member.As<PropertyInfo>();
                        if (!propertyInfo.CanWrite)
                            continue;
                        var propertyValue = propertyInfo.GetValue(source);
                        propertyInfo.SetValue(target, propertyValue);
                        break;
                }
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        public static T Clone<T>(this T source) where T : class
        {
            var target = Activator.CreateInstance<T>();
            target.FillMembers(source);
            return target;
        }

        public static void CopyMembersFrom<T>(this T target, T source, params string[] ignores) where T : class
        => target.FillMembers(source);

        /// <summary>
        /// 根据名字得到类型
        /// 这个要在外层调用，CommonUtils属于被引用层，权限较低，此处示例
        /// </summary>
        public static Type GetTypeByName(string name)
        => Type.GetType(name, true, true);

        /// <summary>
        /// 根据名字得到成员
        /// 这个要在外层调用，CommonUtils属于被引用层，权限较低，此处示例
        /// </summary>
        public static MemberInfo GetMember(string typeName, string memberName)
        {
            var type = GetTypeByName(typeName);
            return type.GetMembers().FirstOrDefault(m => m.Name.ToLower() == memberName.ToLower());
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        public static List<object> GetAttributes(this MemberInfo member)
        {
            var list = new List<object>();
            list.AddRange(member.CustomAttributes);
            list.AddRange(member.DeclaringType.CustomAttributes);
            return list;
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        public static List<T> GetAttributes<T>(this MemberInfo member, bool inherit = true) where T : Attribute
        {
            var list = new List<T>();
            list.AddRange(member.GetCustomAttributes<T>());
            if (inherit && member.DeclaringType != null)
                list.AddRange(member.DeclaringType.GetCustomAttributes<T>());
            return list;
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        public static T GetAttribute<T>(this MemberInfo member, bool inherit = true) where T : Attribute
        => member.GetAttributes<T>(inherit).FirstOrDefault();

        /// <summary>
        /// 获取特性(包含继承)
        /// </summary>
        public static List<T> GetAttributes<T>() where T : Attribute
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            var parent = method.DeclaringType;
            var list = new List<T>();
            list.AddRange(method.GetCustomAttributes<T>(true));
            list.AddRange(parent.GetCustomAttributes<T>(true));
            return list;
        }

        /// <summary>
        /// 获取特性(包含继承)
        /// </summary>
        public static T GetAttribute<T>() where T : Attribute
        {
            var method = new StackTrace().GetFrame(1).GetMethod();
            var parent = method.DeclaringType;
            var list = new List<T>();
            list.AddRange(method.GetCustomAttributes<T>(true));
            list.AddRange(parent.GetCustomAttributes<T>(true));
            return list.FirstOrDefault();
        }


        #region CommonUtils层级
        /// <summary>
        /// 底层类型
        /// </summary>
        public static readonly Type CommonType = new StackTrace().GetFrame(0).GetMethod().ReflectedType;

        /// <summary>
        /// 公用命名空间
        /// </summary>
        public static readonly string CommonNamespace = CommonType.Namespace;

        /// <summary>
        /// 公用程序集
        /// </summary>
        public static readonly Assembly CommonAssembly = CommonType.Assembly;

        /// <summary>
        /// 公用程序集
        /// </summary>
        public static readonly string CommonAssemblyName = CommonAssembly.ManifestModule.Name;
        #endregion

        #region 自定义层级
        /// <summary>
        /// 类型
        /// </summary>
        public static Type GetType(int frameIndex)
        => new StackTrace().GetFrame(frameIndex).GetMethod().ReflectedType;

        /// <summary>
        /// 命名空间
        /// </summary>
        public static string GetNamespace(int frameIndex)
        => new StackTrace().GetFrame(frameIndex).GetMethod().ReflectedType.Namespace;

        /// <summary>
        /// 程序集
        /// </summary>
        public static Assembly GetAssembly(int frameIndex)
        => new StackTrace().GetFrame(frameIndex).GetMethod().ReflectedType.Assembly;

        /// <summary>
        /// 程序集名
        /// </summary>
        public static string GetAssemblyName(int frameIndex)
        => new StackTrace().GetFrame(frameIndex).GetMethod().Module.Name;
        #endregion

        #region 调用层级
        /// <summary>
        /// 当前类型
        /// </summary>
        public static Type CurrentType()
        => new StackTrace().GetFrame(1).GetMethod().ReflectedType;

        /// <summary>
        /// 当前命名空间
        /// </summary>
        public static string CurrentNamespace()
        => new StackTrace().GetFrame(1).GetMethod().ReflectedType.Namespace;

        /// <summary>
        /// 当前程序集
        /// </summary>
        public static Assembly CurrentAssembly()
        => new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;

        /// <summary>
        /// 当前方法名
        /// </summary>
        public static string CurrentMethodName()
        {
            MethodBase method = new StackTrace().GetFrame(1).GetMethod();
            string className = method.ReflectedType.FullName;
            int index = className.IndexOf('+');
            if (index != -1)
                className = className.Substring(0, index);
            return className + "." + method.Name;
        }

        /// <summary>
        /// 当前类名
        /// </summary>
        public static string GetFullClassName()
        {
            string name = new StackTrace().GetFrame(1).GetMethod().ReflectedType.FullName;
            int index = name.IndexOf('+');
            if (index != -1)
                name = name.Substring(0, index);
            return name;
        }
        #endregion

        #region 反射Dll
        /// <summary>
        /// 获取程序集
        /// </summary>
        public static Assembly AssemblyLoad(string path)
        {
            var bytes = File.ReadAllBytes(path);
            return Assembly.Load(bytes);
        }

        /// <summary>
        /// 获取第一个公共类
        /// </summary>
        public static string AssemblyFirstClass(string path)
        {
            var assembly = AssemblyLoad(path);
            var types = assembly.GetExportedTypes();
            if (types.Length == 0)
                return null;
            return types[0].FullName;
        }

        /// <summary>
        /// 获取第一个公共类
        /// </summary>
        public static string FirstClass(this Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            if (types.Length == 0)
                return null;
            return types[0].FullName;
        }

        /// <summary>
        /// 获取类列表
        /// </summary>
        public static string[] AssemblyClasses(string path)
        {
            var assembly = AssemblyLoad(path);
            var types = assembly.GetExportedTypes();
            return types.Select(m => m.FullName).ToArray();
        }
        #endregion
    }
}
