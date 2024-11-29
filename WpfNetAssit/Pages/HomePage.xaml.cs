using System;
using System.Collections.Generic;
using System.IO;
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
using WpfNetAssit.Properties;

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

        public string TitleString
        {
            get { return (string)GetValue(TitleStringProperty); }
            set { SetValue(TitleStringProperty, value); }
        }

        public static readonly DependencyProperty TitleStringProperty =
            DependencyProperty.Register("TitleString", typeof(string), typeof(HomePage), new PropertyMetadata("ENVOVE"));


        public HomePage()
        {
            InitializeComponent();
            VersionString = GetEdition();
            TitleString = Settings.Default.Title;
            LoadBackground();
        }

        /// <summary>
        /// 获取当前系统的版本号
        /// </summary>
        public static string GetEdition()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void LoadBackground()
        {
            string backgroundfilename = "";
            string[] filenames = { "background.png", "background.tif", "background.jpg" };
            foreach (string filename in filenames)
            {
                if (File.Exists(filename))
                {
                    backgroundfilename = filename;
                    break;
                }
            }

            if (backgroundfilename != "")
                backgroundImage.ImageSource = new BitmapImage(new Uri(backgroundfilename, UriKind.Relative));
        }
    }
}
