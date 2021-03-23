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

namespace WpfNetAssit.Communicate.EncodingTool
{
    /// <summary>
    /// EncodingSelector.xaml 的交互逻辑
    /// </summary>
    public partial class EncodingSelector : UserControl
    {
        public EncodingSelector()
        {
            InitializeComponent();
            InitEncodingComboBox();
        }



        public bool IsHeaderShow
        {
            get { return (bool)GetValue(IsHeaderShowProperty); }
            set { SetValue(IsHeaderShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeaderShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeaderShowProperty =
            DependencyProperty.Register("IsHeaderShow", typeof(bool), typeof(EncodingSelector), new PropertyMetadata(true));



        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(EncodingSelector), new PropertyMetadata("编码"));



        public string EncodingString
        {
            get { return (string)GetValue(EncodingStringProperty); }
            set { CurEncoding = Encoding.GetEncoding(value); SetValue(EncodingStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EncodingString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EncodingStringProperty =
            DependencyProperty.Register("EncodingString", typeof(string), typeof(EncodingSelector), new FrameworkPropertyMetadata("UTF-8", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public event EventHandler EncodingChanged
        {
            add { AddHandler(EncodingChangedEvent, value); }
            remove { RemoveHandler(EncodingChangedEvent, value); }
        }

        public static readonly RoutedEvent EncodingChangedEvent = EventManager.RegisterRoutedEvent(
        "EncodingChanged", RoutingStrategy.Bubble, typeof(EventHandler), typeof(EncodingSelector));




        private void InitEncodingComboBox()
        {
            EncodingComboBox.ItemsSource = new Encoding[]{
                Encoding.Default, Encoding.UTF8, Encoding.Unicode, Encoding.ASCII,
                Encoding.GetEncoding("GB2312"), Encoding.GetEncoding("Big5")
            };
        }



        public Encoding CurEncoding
        {
            get { return (Encoding)GetValue(CurEncodingProperty); }
            set { SetValue(CurEncodingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurEncoding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurEncodingProperty =
            DependencyProperty.Register("CurEncoding", typeof(Encoding), typeof(EncodingSelector), new FrameworkPropertyMetadata(Encoding.UTF8, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        private void EncodingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurEncoding = EncodingComboBox.SelectedItem as Encoding;
            EncodingString = CurEncoding.WebName;
            RaiseEvent(new RoutedEventArgs(EncodingChangedEvent, this));
        }


        /// <summary>
        /// 将数据按编码转换
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public string EncodingToString(byte[] buf)
        {
            return CurEncoding.GetString(buf);
        }
    }
}
