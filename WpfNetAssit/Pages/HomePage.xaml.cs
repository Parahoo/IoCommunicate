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

namespace WpfNetAssit.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : UserControl
    {
        public string VersionString
        {
            get { return (string)GetValue(VersionStringProperty); }
            set { SetValue(VersionStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionStringProperty =
            DependencyProperty.Register("VersionString", typeof(string), typeof(HomePage), new PropertyMetadata(""));


        public HomePage()
        {
            InitializeComponent();
            VersionString = GetEdition();
        }

        /// <summary>
        /// 获取当前系统的版本号
        /// </summary>
        public static string GetEdition()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
