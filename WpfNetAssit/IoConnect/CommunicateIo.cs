using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    public interface ICommunicateIo
    {
        string NickName { get; set; }
        string LinkInfo { get; set; }
        string FullInfo { get; }

        string ErrorInfo { get; set; }

        bool IsLinkOk { get; set; }

        bool Open();
        bool Close();
        bool Read(byte[] pBuf, ref int readSize);
        bool Write(byte[] pData, ref int writeSize);
    }
}
