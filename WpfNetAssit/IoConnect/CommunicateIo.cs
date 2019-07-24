using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    public interface CommunicateIo
    {
        bool Open();
        bool Close();
        bool Read(byte[] pBuf, ref int readSize);
        bool Write(byte[] pData, ref int writeSize);
    }
}
