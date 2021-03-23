using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.Communicate.Send.NormalSend
{
    [Serializable]
    public class NormalSendPageModel : ViewModelBase
    {
        public bool IsRealtimeSend { get; set; } = false;

        public bool IsAppendEnter { get; set; } = true;

        private DataEditorModel dataEditorModel = new DataEditorModel();
        public DataEditorModel DataEditorModel
        {
            get { return dataEditorModel; }
            set { Set("DataEditorModel", ref dataEditorModel, value); }
        }


        private ICommunicateIo Io  = null;
        public void SetIo(ICommunicateIo io) { Io = io; }

        [System.Xml.Serialization.XmlIgnore]
        public ICommand SendCommand { get; }

        [System.Xml.Serialization.XmlIgnore]
        public ICommand PreviewTextInputCommand { get; }

        public NormalSendPageModel()
        {
            SendCommand = new RelayCommand(Send);
            PreviewTextInputCommand = new RelayCommand<TextCompositionEventArgs>(InputTextBox_PreviewTextInput);
        }

        private void Send()
        {
            byte[] src = dataEditorModel.Data;
            byte[] data = src;
            if (IsAppendEnter == true)
            {
                data = new byte[src.Length + 1];
                src.CopyTo(data, 0);
                data[src.Length] = 13;
            }
            SendData(data);
        }

        private void InputTextBox_PreviewTextInput(TextCompositionEventArgs e)
        {
            if (IsRealtimeSend == true)
            {
                SendText(e.Text);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 根据编码发送文本
        /// </summary>
        /// <param name="text"></param>
        private void SendText(string text)
        {
            byte[] data = dataEditorModel.CurEncoding.GetBytes(text);
            SendData(data);
        }

        private void SendData(byte[] data)
        {
            int writesize = data.Length;
            Io?.Write(data, ref writesize);
        }
    }
}
