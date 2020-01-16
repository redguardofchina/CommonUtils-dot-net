using Microsoft.Win32;

namespace CommonUtils
{
    /// <summary>
    /// 分隔符必须使用\\
    /// </summary>
    public static class RegistryUtil
    {
        public static RegistryKey ClassesRoot
        => Registry.ClassesRoot;

        public static RegistryKey CurrentConfig
        => Registry.CurrentConfig;

        public static RegistryKey CurrentUser
        => Registry.CurrentUser;

        public static RegistryKey LocalMachine
        => Registry.LocalMachine;

        public static RegistryKey PerformanceData
        => Registry.PerformanceData;

        public static RegistryKey Users
        => Registry.Users;

        public static string[] GetConfigKeys()
        => CurrentConfig.GetSubKeyNames();

        public static void SetToConfig(string key, string value)
        => CurrentConfig.SetValue(key, value);

        public static string GetFromConfig(string key)
        => CurrentConfig.GetValue(key) as string;

        public static void RemoveFromConfig(string key)
        => CurrentConfig.DeleteValue(key);

        public static void SetValue(string path, string key, object value)
        => Registry.SetValue(path, key, value);

        public static object GetValue(string path, string key)
        => Registry.GetValue(path, key, null);

        public static RegistryKey GetRoot(string path)
        {
            var root = path.SubstringStartByFirst('\\').ToUpper();
            switch (root)
            {
                case "HKEY_CLASSES_ROOT":
                    return Registry.ClassesRoot;
                case "HKEY_CURRENT_USER":
                    return Registry.CurrentUser;
                case "HKEY_LOCAL_MACHINE":
                    return Registry.LocalMachine;
                case "HKEY_USERS":
                    return Registry.Users;
                case "HKEY_CURRENT_CONFIG":
                    return Registry.CurrentConfig;
                default: return null;
            }
        }

        public static RegistryKey GetFloder(string path)
        {
            var root = GetRoot(path);
            if (root == null)
                return null;
            path = path.SubstringEndByFirstKey('\\');
            return root.OpenSubKey(path);
        }

        public static bool IsFloderExisting(string path)
        => GetFloder(path) != null;

        public static string[] GetFloderNames(this RegistryKey floder)
        => floder.GetSubKeyNames();

        public static string[] GetItemNames(this RegistryKey floder)
        => floder.GetValueNames();

        public static string[] GetItemNames(string path)
        => GetFloder(path).GetItemNames();
    }
}
