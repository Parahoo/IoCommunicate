using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNetAssit.Pages;

namespace WpfNetAssit.Communicate.Recive
{
    [Serializable]
    public class ReciveShowPageModel : ViewModelBase
    {
        public bool IsShowText { get; set; } = true;
        public bool IsHex { get; set; } = false;
        private string encodingString = "UTF-8";
        public string EncodingString { 
            get{ return encodingString; }
            set{Set("EncodingString", ref encodingString, value); encoding=Encoding.GetEncoding(encodingString); } 
        }

        [System.Xml.Serialization.XmlIgnore]
        public int TotalCount { get; set; } = 0;

        [System.Xml.Serialization.XmlIgnore]
        public int CurCount { get; set; } = 0;

        private Encoding encoding = Encoding.UTF8;

        [System.Xml.Serialization.XmlIgnore]
        private TextMonitorModel textMonitorModel = new TextMonitorModel();
        [System.Xml.Serialization.XmlIgnore]
        public TextMonitorModel TextMonitorModel
        {
            get { return textMonitorModel; }
            set { Set("TextMonitorModel", ref textMonitorModel, value); }
        }


        [System.Xml.Serialization.XmlIgnore]
        public ICommand ClearMonitorCommand { get; }

        [System.Xml.Serialization.XmlIgnore]
        public ICommand ClearCountCommand { get; }

        public ReciveShowPageModel()
        {
            ClearMonitorCommand = new RelayCommand(ClearMonitor);
            ClearCountCommand = new RelayCommand(ClearCount);
        }

        private void CountAdd()
        {
            CurCount++;
            TotalCount++;
        }

        private void ClearMonitor()
        {
            TextMonitorModel.ClearMonitor();

        }

        private void ClearCount()
        {
            CurCount = 0;
        }


        private string EncodingToString(byte[] buf)
        {
            return encoding.GetString(buf);
        }


        /// <summary>
        /// 将数据转化为显示文本
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private string TransToString(byte[] buf)
        {
            if (IsHex)
                return HexStringConvertor.HexToString(buf);
            else
                return EncodingToString(buf);
        }

        public void RecvData(byte[] data)
        {
            CountAdd();
            if (IsShowText)
                MonitorData(data);
        }
        /// <summary>
        /// 显示接收到的数据
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        private void MonitorData(byte[] data)
        {
            string t = TransToString(data);
            TextMonitorModel.MonitorData(t);
        }

    }
}
