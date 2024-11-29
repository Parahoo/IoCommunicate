using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNetAssit.IoConnect;

namespace WpfNetAssit.LogicalAction
{
    public class RecvPack
    {
        public byte[] Data { get; set; } = null;
        public double UsedTime { get; set; } = -1.0;
    }

    public class IoPipe
    {
        BlockingCollection<RecvPack> recvqueue = new BlockingCollection<RecvPack>();
        private DateTime lastSendTick = DateTime.Now;

        public ICommunicateIo Io { get; set; } = null;

        public void RecvData(byte[] data)
        {
            RecvPack pack = new RecvPack();
            pack.Data = data;
            pack.UsedTime = (DateTime.Now - lastSendTick).TotalMilliseconds;
            recvqueue.Add(pack);
            if(recvqueue.Count > 10000)
            {
                // 防止内存占用过大
                recvqueue.Take();
            }
        }

        public bool Read(int timeout, System.Threading.CancellationToken cancellationToken, ref byte[] buf, ref double usedtime)
        {
            RecvPack pack;
            var ret = recvqueue.TryTake(out pack, timeout, cancellationToken);
            if(ret)
            {
                buf = pack.Data;
                usedtime = pack.UsedTime;
            }
            return ret;
        }

        public bool Write(byte[] buf, ref int writesize)
        {
            lastSendTick = DateTime.Now;
            return Io.Write(buf, ref writesize);
        }
    }
}
