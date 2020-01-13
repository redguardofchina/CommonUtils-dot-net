//using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CommonUtils
{
    /// <summary>
    /// 窗口管理
    /// </summary>
    public static class WindowsUtil
    {
        /// <summary>
        /// UI停止刷新
        /// </summary>
        private static object StopFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        /// <summary>
        /// UI刷新
        /// this.Dispatcher x.Dispatcher Application.Current.Dispatcher
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void RefreshUi(Dispatcher dispatcher)
        {
            DispatcherFrame frame = new DispatcherFrame();
            dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(StopFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// 线程调用,自带UI刷新
        /// this.Dispatcher x.Dispatcher Application.Current.Dispatcher
        /// </summary> 
        public static void DispatcherInvoke(Action action, Dispatcher dispatcher)
        {
            dispatcher.Invoke(action);
            RefreshUi(dispatcher);
        }

        /// <summary>
        /// 获取文件夹路径,末尾不带斜杠"\\"
        /// if void main() add[STAThread]
        /// </summary>
        public static string SelectFloder()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = CacheUtil.GetFromFile<string>("FolderBrowserDialog.SelectedPath");
            dialog.ShowDialog();
            CacheUtil.SaveWithFile("FolderBrowserDialog.SelectedPath", dialog.SelectedPath);
            return dialog.SelectedPath;
        }

        /// <summary>
        /// 文件格式|名字过滤
        /// </summary>
        private static string GetFileDialogFilter(string[] namesOrExtentions)
        {
            //任意文件
            if (namesOrExtentions == null || namesOrExtentions.Length == 0)
                return "|*.*";
            //指定文件或类型
            var filter = new StringBuilder("|");
            foreach (string value in namesOrExtentions)
            {
                //加不加*都有效，加了好看
                if (value[0] == '.')
                    filter.Append('*');
                filter.Append(value);
                filter.Append(';');
            }
            return filter.ToString();
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        public static string SelectFile(params string[] namesOrExtentions)
        {
            var dialog = new OpenFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Filter = GetFileDialogFilter(namesOrExtentions);
            dialog.ShowDialog();
            return dialog.FileName;
        }

        /// <summary>
        /// 获取存储路径
        /// </summary>
        public static string SelectSavePath(params string[] namesOrExtentions)
        {
            var dialog = new SaveFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Filter = GetFileDialogFilter(namesOrExtentions);
            dialog.ShowDialog();
            return dialog.FileName;
        }

        /// <summary>
        /// 获取存储路径,并存储
        /// </summary>
        public static void SaveToSelectedPath(byte[] bytes, params string[] namesOrExtentions)
        {
            var path = SelectSavePath(namesOrExtentions);
            if (string.IsNullOrEmpty(path))
                return;
            FileUtil.Save(path, bytes);
        }

        public static void SaveToSelectedPath(Stream stream, params string[] namesOrExtentions)
        {
            var path = SelectSavePath(namesOrExtentions);
            if (string.IsNullOrEmpty(path))
                return;
            FileUtil.Save(path, stream);
        }

        /// <summary>
        /// 延迟执行
        /// </summary> 
        public static void Wait()
        {
            //Application.DoEvents();
            Dispatcher.Run();
        }

        /// <summary>
        /// 延迟执行
        /// </summary>
        public static void Wait(int second)
        {
            DateTime dtPast = DateTime.Now, dtNow = DateTime.Now;
            while ((dtNow - dtPast).TotalSeconds < second)
            {
                Wait();
                dtNow = DateTime.Now;
            }
        }

        /// <summary>
        /// 延迟执行
        /// </summary>
        /// <param name="second"></param>
        public static void Wait(int second, DateTime dtPast)
        {
            DateTime dtNow = DateTime.Now;
            while ((dtNow - dtPast).TotalSeconds < second)
            {
                Wait();
                dtNow = DateTime.Now;
            }
        }
    }
}
