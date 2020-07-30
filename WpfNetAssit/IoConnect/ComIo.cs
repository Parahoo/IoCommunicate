using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    public class ComIoParam : ICloneable
    {
        public string Name { get; set; } = "com3";
        public int BaudRate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public int DataBits { get; set; } = 8;
        public StopBits StopBit { get; set; } = StopBits.One;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return Name + "@" + BaudRate.ToString();
        }
    }

    public class ComIo : CommunicateIo
    {
        public ComIoParam Param { get; set; } = new ComIoParam();
        public string NickName { get; set; } = "Com";

        public string LinkInfo => Param.ToString();

        public string FullInfo => NickName+LinkInfo;

        public SerialPort port;

        public bool Open()
        {
            try
            {
                port = new SerialPort(Param.Name, Param.BaudRate,
                    Param.Parity, Param.DataBits, Param.StopBit);
                port.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                port.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Read(byte[] pBuf, ref int readSize)
        {
            try
            {
                readSize = port.Read(pBuf, 0, 0x100);
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
