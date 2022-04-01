using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    public class ComIoParam : ObservableObject, ICloneable
    {
        public string Name { get; set; } = "com3";
        public int BaudRate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public int DataBits { get; set; } = 8;
        public StopBits StopBit { get; set; } = StopBits.One;

        public Handshake HandShake { get; set; } = 0;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return Name + "@" + BaudRate.ToString();
        }
    }

    public class ComIo : ICommunicateIo
    {
        public ComIoParam Param { get; set; } = new ComIoParam();
        public string NickName { get; set; } = "Com";

        public string LinkInfo { get; set; } = "";

        public string FullInfo => NickName+LinkInfo;

        public string ErrorInfo { get; set; } = "";
        public bool IsLinkOk { get; set; } = false;

        public SerialPort port;

        public bool Open()
        {
            try
            {
                port = new SerialPort(Param.Name, Param.BaudRate,
                    Param.Parity, Param.DataBits, Param.StopBit);
                port.Handshake = (Param.HandShake);
                port.ReadTimeout = 50;
                port.Open();
                LinkInfo = Param.Name;
                ErrorInfo = "";
                IsLinkOk = true;
                return true;
            }
            catch (Exception)
            {
                ErrorInfo = "无法打开 "+ Param.Name;
                IsLinkOk = false;
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                port.Close();
                IsLinkOk = false;
                return true;
            }
            catch (Exception)
            {
                IsLinkOk = false;
                return false;
            }
        }

        public bool Read(byte[] pBuf, ref int readSize)
        {
            try
            {
                readSize = 0;

                int size = port.BytesToRead;
                while(readSize < 0x100)
                {
                    int cursize = port.Read(pBuf, readSize, 0x100);
                    readSize += cursize;

                }
                return true;
            }
            catch(TimeoutException )
            {
                return true;
            }
            catch (Exception)
            {
                readSize = 0;
                return false;
            }
        }

        public bool Write(byte[] pData, ref int writeSize)
        {
            try
            {
                writeSize = pData.Length;
                port.Write(pData, 0, pData.Length);
                return true;
            }
            catch (Exception)
            {
                writeSize = 0;
                return false;
            }
        }
    }
}
