using System.Windows;

namespace CommonUtils.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ButtonTest.Click += ButtonTest_Click;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            ConsoleUtil.Print(KeysUtil.ChinaInnovation);
            TextBoxLog.Insert(ConsoleUtil.Print(KeysUtil.ChinaInnovation));
        }
    }
}
