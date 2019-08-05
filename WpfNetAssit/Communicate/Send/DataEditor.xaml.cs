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

namespace WpfNetAssit.Communicate.Send
{
    /// <summary>
    /// DataEditor.xaml 的交互逻辑
    /// </summary>
    public partial class DataEditor : UserControl
    {

        public DataEditor()
        {
            InitializeComponent();
        }




        public byte[] Data { get; private set; } = new byte[] { };

        /// <summary>
        /// 计算新的数据
        /// </summary>
        private void CalcData()
        {
            if(IsHexCheckBox.IsChecked == true)
            {
                Data = HexStringConvertor.StringToHex(DataTextBox.Text);
            }
            else
            {
                Data = EncodingSelector.CurEncoding.GetBytes(DataTextBox.Text);
            }

            ByteCountLabel.Content = Data.Length.ToString();
            DataTextBox.ToolTip = HexStringConvertor.HexToString(Data);
        }


        private void EncodingSelector_EncodingChanged(object sender, EventArgs e)
        {
            CalcData();
        }

        private void IsHexCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CalcData();
        }

        private void DataTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalcData();
        }
    }
}
