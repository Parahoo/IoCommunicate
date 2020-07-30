using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    public interface CommunicateIo
    {
        string NickName { get; set; }
        string LinkInfo { get; }
        string FullInfo { get; }

        bool Open();
        bool Close();
        bool Read(byte[] pBuf, ref int readSize);
        bool Write(byte[] pData, ref int writeSize);
    }
}
