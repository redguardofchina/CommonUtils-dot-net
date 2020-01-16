using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonUtils.Test.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextBox2.TextChanged += TextBoxTest_TextChanged;
            TextBox2.SaveChange();
            TextBox2.Load();

            ButtonSetValue.Click += ButtonSetValue_Click;
        }

        private void TextBoxTest_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            TextBoxLog.Log("{0}检测到值变化为 {1}", textBox.Name, textBox.Text);
        }

        private void ButtonSetValue_Click(object sender, RoutedEventArgs e)
        {
            TextBox2.Text = TextBox1.Text;
        }
    }
}
