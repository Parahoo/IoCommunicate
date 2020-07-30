using MahApps.Metro.Controls;
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
using WpfNetAssit.Communicate;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            UserSetting.Default = UserSetting.Load();
            ioSelectPage.DataContext = UserSetting.Default.IosSetting;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserSetting.Default.Save();
        }

        void AddNewCommunicate(CommunicateIo io)
        {
            var communicatePage = new CommunicatePage();

            var page = new TabItem();
            page.Header = io.FullInfo;
            page.Content = communicatePage;
            page.SetValue(MahApps.Metro.Controls.TabControlHelper.CloseButtonEnabledProperty, true);
            FunctionTab.Items.Add(page);

            communicatePage.StartCommunicate(io);
        }


        private void IoSelectPage_IoOpened(object sender, EventArgs e)
        {
            FunctionTab.SelectedIndex = 1;
            AddNewCommunicate(ioSelectPage.CurIo);
        }

        private void IoSelectPage_IoClosed(object sender, EventArgs e)
        {
        }
    }
}
