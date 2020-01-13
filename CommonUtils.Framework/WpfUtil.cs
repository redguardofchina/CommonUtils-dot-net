using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonUtils
{
    public static class WpfUtil
    {
        /// <summary>
        /// 启用
        /// </summary>
        public static void Enable(this UIElement uiElement, string msg = null)
        {
            uiElement.IsEnabled = true;
            WindowsUtil.RefreshUi(uiElement.Dispatcher);
            if (!msg.IsNullOrEmpty())
                MessageBox.Show(msg);
        }
        /// <summary>
        /// 启用
        /// </summary>
        public static void Enable(object uiElement, string msg = null)
        => uiElement.As<UIElement>().Enable(msg);

        /// <summary>
        /// 禁用
        /// </summary>
        public static void Disable(this UIElement uiElement)
        {
            uiElement.IsEnabled = false;
            WindowsUtil.RefreshUi(uiElement.Dispatcher);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public static void Disable(object uiElement)
        => uiElement.As<UIElement>().Disable();

        /// <summary>
        /// 居上,放置在InitializeComponent之后,Loaded之前
        /// </summary>
        public static void AlignTop(this Window window, int top = 0)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = top;
        }

        /// <summary>
        /// 垂直居中,放置在InitializeComponent之后,Loaded之前
        /// </summary>
        public static void AlignMiddle(this Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = (SystemUtil.ScreenHeight - window.Height) / 2;
        }

        /// <summary>
        /// 居下,放置在InitializeComponent之后,Loaded之前
        /// </summary>
        public static void AlignBottom(this Window window, int bottom = 0)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = SystemUtil.ScreenHeight - window.Height - bottom;
        }

        /// <summary>
        /// 居左,放置在InitializeComponent之后,Loaded之前
        /// </summary>
        public static void AlignLeft(this Window window, int left = 0)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = left;
        }

        /// <summary>
        /// 水平居中,放置在InitializeComponent之后,Loaded之前
        /// </summary>
        public static void AlignCenter(this Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = (SystemUtil.DesktopWidth - window.Width) / 2;
        }

        /// <summary>
        /// 居右,放置在InitializeComponent之后,Loaded之前
        /// </summary>
        public static void AlignRight(this Window window, int right = 0)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = SystemUtil.DesktopWidth - window.Width - right;
        }

        /// <summary>
        /// 水平垂直居中,放置在InitializeComponent之前
        /// </summary>
        public static void AlignCenterMiddle(this Window window)
        => window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        /// <summary>
        /// 禁止拉伸,放置在InitializeComponent之前
        /// </summary>
        public static void NoResize(this Window window)
        => window.ResizeMode = ResizeMode.CanMinimize;

        /// <summary>
        /// 禁止最小化,放置在InitializeComponent之前
        /// </summary>
        public static void NoMinimize(this Window window)
        => window.ResizeMode = ResizeMode.NoResize;

        /// <summary>
        /// 关闭App
        /// </summary>
        public static void Close()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        public static void SelectFile(this TextBox textBox, bool save = true, params string[] namesOrExtentions)
        {
            string path = WindowsUtil.SelectFilePath(namesOrExtentions);
            if (!string.IsNullOrWhiteSpace(path))
                textBox.Text = path;
            if (save)
                textBox.Save();
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        public static void SelectFile(this Button button, TextBox textBox, bool save = true, params string[] namesOrExtentions)
        => button.Click += delegate { textBox.SelectFile(save, namesOrExtentions); };

        /// <summary>
        /// 赋值
        /// </summary>
        public static void SetText(this TextBox textBox, string text)
        => textBox.Text = text;

        /// <summary>
        /// 插值
        /// </summary>
        public static void Insert(this TextBox textBox, string text)
        => textBox.Text = text + textBox.Text;

        /// <summary>
        /// 附值
        /// </summary>
        public static void Append(this TextBox textBox, string text)
        => textBox.AppendText(text);

        /// <summary>
        /// 太长了就清空
        /// </summary>
        public static void CheckLength(this TextBox textBox)
        {
            if (textBox.Text.Length > 6553500)//7000k
                textBox.Text = "";
        }

        /// <summary>
        /// 以log形式输出
        /// </summary>
        public static void LogInsert(this TextBox textBox, object text, params object[] args)
        {
            textBox.CheckLength();
            textBox.Insert(LogUtil.GetFormatedString(text, args));
        }

        /// <summary>
        /// 以log形式输出
        /// </summary>
        public static void LogAppend(this TextBox textBox, object text, params object[] args)
        {
            textBox.CheckLength();
            textBox.Append(LogUtil.GetFormatedString(text, args));
        }

        /// <summary>
        /// 以log形式输出
        /// </summary>
        public static void Log(this TextBox textBox, object text, params object[] args)
        => textBox.LogInsert(text, args);

        /// <summary>
        /// 赋值
        /// </summary>
        public static void SetText(this Label label, string text)
        => label.Content = text;

        /// <summary>
        /// 保存配置
        /// </summary>
        public static void Save(this TextBox textBox)
        {
            //这里不加空判断，有时候要的就是空值
            CacheUtil.SaveWithFile(textBox.Name, textBox.Text);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public static void Save(this CheckBox checkBox)
        => CacheUtil.SaveWithFile(checkBox.Name, checkBox.IsChecked.Value);

        /// <summary>
        /// 保存配置
        /// </summary>
        public static void SaveChange(this TextBox textBox)
        {
            textBox.TextChanged += delegate
            {
                ConsoleUtil.Print("TextBox({0}).Save({1})", textBox.Name, textBox.Text.AddQuotation());
                textBox.Save();
            };
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public static void SaveChange(this CheckBox checkBox)
        => checkBox.Click += delegate { checkBox.Save(); };

        /// <summary>
        /// 载入配置/或文件
        /// </summary>
        public static void Load(this TextBox textBox, string path = null)
        {
            ConsoleUtil.Print("TextBox({0}).Load()", textBox.Name);

            string text;

            if (string.IsNullOrEmpty(path))
                text = CacheUtil.GetFromFile<string>(textBox.Name);
            else
                text = FileUtil.GetText(path);

            if (!string.IsNullOrEmpty(text))
                /*
                 * 此处赋值textBox.Text = text
                 * 如果textBox.Text == text 不会触发TextChanged
                 * 如果textBox.Text != text 会触发TextChanged
                 * 但是textBox进行前端输入(包括粘贴,输入相同字符) 无论==、!=都会触发TextChanged
                 */
                textBox.Text = text;
        }

        /// <summary>
        /// 载入配置
        /// </summary>
        public static void Load(this CheckBox checkBox)
        => checkBox.IsChecked = CacheUtil.GetFromFile<bool>(checkBox.Name);

        /// <summary>
        /// 空判断
        /// </summary>
        public static bool IsNullOrEmpty(this TextBox textBox)
        => string.IsNullOrEmpty(textBox.Text);

        /// <summary>
        /// 空判断
        /// </summary>
        public static bool IsNullOrWhiteSpace(this TextBox textBox)
        => string.IsNullOrWhiteSpace(textBox.Text);

        /// <summary>
        /// 是否为有效文件路径
        /// </summary>
        public static bool IsFilePath(this TextBox textBox)
        => FileUtil.Exists(textBox.Text);

        /// <summary>
        /// 获取文件夹路径,末尾不带斜杠"\\"
        /// </summary>
        public static void SelectFloder(this TextBox textBox, bool save = false)
        {
            string path = WindowsUtil.SelectFloderPath();
            if (!string.IsNullOrWhiteSpace(path))
                textBox.Text = path;
            if (save)
                textBox.Save();
        }

        /// <summary>
        /// 获取文件夹路径,末尾不带斜杠"\\"
        /// </summary>
        public static void SelectFloder(this Button button, TextBox textBox, bool save = false)
        => button.Click += delegate { textBox.SelectFloder(save); };

        /// <summary>
        /// 打开路径
        /// </summary>
        public static void Open(this Button button, TextBox textBox)
        {
            //这里不能继承 Open(this Button button, string path)
            //因为string是值类型，textbox是引用，肯定要打开引用值
            button.Click += delegate { ProcessUtil.Run(textBox.Text); };
        }

        /// <summary>
        /// 打开路径
        /// </summary>
        public static void Open(this Button button, string path)
        => button.Click += delegate { ProcessUtil.Run(path); };

        #region 添加感谢作者窗口
        /// <summary>
        /// 空窗口
        /// </summary>
        private class WindowEmpty : Window
        {
            /// <summary>
            /// 添加控件
            /// </summary>
            public void Add(object control)
            => AddChild(control);
        }

        /// <summary>
        /// 添加感谢作者窗口 放在init之后
        /// </summary>
        public static void AddThanks(this Grid grid, ImageSource imageSource, string mail = null, double width = 0, double height = 0)
        {
            //窗口大小
            if (width == 0)
                width = imageSource.Width;
            if (height == 0)
                height = imageSource.Height;

            //按钮
            var buttonThanks = new Button();
            buttonThanks.Content = "感谢作者?";
            buttonThanks.Foreground = WindowsImage.BlueBrush;
            buttonThanks.BorderBrush = WindowsImage.TransparentBrush;
            buttonThanks.Background = WindowsImage.TransparentBrush;

            buttonThanks.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            buttonThanks.VerticalAlignment = VerticalAlignment.Top;
            buttonThanks.Margin = new Thickness(0, 6, 12, 0);

            buttonThanks.Click += delegate (object sender, RoutedEventArgs e)
            {
                //新窗口
                var win = new WindowEmpty();
                win.AlignCenterMiddle();
                win.NoResize();
                win.Title = "感谢作者";
                win.Width = width + 16;
                win.Height = height + 39;

                //布局
                Grid winGrid = new Grid();
                win.Add(winGrid);

                //图片
                Image image = new Image();
                image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Margin = new Thickness(0, 2, 0, 0);
                image.Width = width;
                image.Height = height;
                image.Source = imageSource;
                winGrid.Children.Add(image);

                //显示返回邮箱
                if (!mail.IsNullOrEmpty())
                {
                    win.Height += 32;
                    var buttonFeedback = new Button();
                    buttonFeedback.Content = "建议反馈：mailto:" + mail;
                    buttonFeedback.Foreground = WindowsImage.BlueBrush;
                    buttonFeedback.BorderBrush = WindowsImage.TransparentBrush;
                    buttonFeedback.Background = WindowsImage.TransparentBrush;

                    buttonFeedback.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    buttonFeedback.VerticalAlignment = VerticalAlignment.Bottom;
                    buttonFeedback.Margin = new Thickness(0, 0, 0, 8);

                    buttonFeedback.Click += delegate (object sender1, RoutedEventArgs e1)
                    {
                        ProcessUtil.Run("mailto:" + mail);
                        if (SystemUtil.SetClipboard(mail))
                            System.Windows.MessageBox.Show(mail + " 已复制到剪切板");
                    };
                    winGrid.Children.Add(buttonFeedback);
                }

                win.Show();
            };
            grid.Children.Add(buttonThanks);
        }

        /// <summary>
        /// 添加感谢作者窗口 放在init之后
        /// </summary>
        public static void AddThanks(this Grid grid, Image image, string mail = null, double width = 0, double height = 0)
        => grid.AddThanks(image.Source, mail, width, height);

        /// <summary>
        /// 添加感谢作者窗口 放在init之后
        /// </summary>
        public static void AddThanks(this Grid grid, string url, string mail = null, double width = 0, double height = 0)
        => grid.AddThanks(WindowsImage.GetSourceFromUrlOrPath(url), mail, width, height);
        #endregion
    }
}
