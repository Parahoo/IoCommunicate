using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.Communicate.Send.NormalSend
{
    public class DataEditorModel : ViewModelBase
    {
        private string inputString  = "";
        public string InputString {
            get {return inputString; }
            set {Set("InputString", ref inputString, value);CalcData(); } 
        }

        private string encodingString = "UTF-8";

        public string EncodingString
        {   
            get { return encodingString; }
            set { Set("EncodingString", ref encodingString, value); CurEncoding = Encoding.GetEncoding(EncodingString); CalcData(); }
        }
        [System.Xml.Serialization.XmlIgnore]
        public Encoding CurEncoding { get; set; } = Encoding.UTF8;

        private bool isHex;

        public bool IsHex
        {
            get { return isHex; }
            set { Set("IsHex", ref isHex, value); CalcData(); }
        }

        [System.Xml.Serialization.XmlIgnore]
        public int DataSize { get; private set; } = 0;
        [System.Xml.Serialization.XmlIgnore]
        public byte[] Data { get; private set; } = new byte[] { };
        [System.Xml.Serialization.XmlIgnore]
        public string ToolTip { get; private set; } = "";

        /// <summary>
        /// 计算新的数据
        /// </summary>
        private void CalcData()
        {
            if (IsHex == true)
            {
                Data = HexStringConvertor.StringToHex(InputString);
            }
            else
            {
                Data = CurEncoding.GetBytes(InputString);
            }

            DataSize = Data.Length;
            ToolTip = "Hex: "+HexStringConvertor.HexToString(Data);
        }

        public DataEditorModel()
        {

        }
    }
}
