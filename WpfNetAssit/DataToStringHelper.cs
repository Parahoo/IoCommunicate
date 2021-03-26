using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.Communicate;

namespace WpfNetAssit
{
    class DataToStringHelper
    {
        public static string BeautifyToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static string HexString(byte[] data)
        {
            return HexStringConvertor.HexToString(data);
        }
    }
}
