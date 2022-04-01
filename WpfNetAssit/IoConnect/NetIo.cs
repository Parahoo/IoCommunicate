using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfNetAssit.IoConnect
{
    public class NetIoParam : ObservableObject, ICloneable
    {

        public bool BRefLocalIp { get; set; } = false;
        public bool BRefLocalPort { get; set; } = false;
        public string LocalIp { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; } = 0;
        public string RemoteIp { get; set; } = "192.168.1.100";
        public int RemotePort { get; set; } = 60000;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        override public string ToString()
        {
            return "["+ LocalIp + ":" + LocalPort +"] <-> [" + RemoteIp + ":" + RemotePort + "]";
        }

        public string RemoteEndpointToString()
        {
            return RemoteIp + ":" + RemotePort;
        }
    }

    public class UdpIo : ICommunicateIo
    {
        public NetIoParam Param { get; set; }= new NetIoParam();

        public string NickName { get; set; } = "Udp";

        public string LinkInfo { get; set; } = "";

        public string FullInfo => NickName + "\r\n"+ LinkInfo;

        public string ErrorInfo { get; set; } = "";
        public bool IsLinkOk { get; set; } = false;

        private Socket socket;
        private string lastRemoteIp = "0.0.0.0";
        private int lastRemoteport = 0;

        private bool OpenSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            int port = 0;
            if (Param.BRefLocalPort)
                port = Param.LocalPort;
            try
            {
                IPEndPoint endPoint;
                if (Param.BRefLocalIp)
                    endPoint = new IPEndPoint(IPAddress.Parse(Param.LocalIp), port);
                else
                    endPoint = new IPEndPoint(IPAddress.Any, port);
                socket.Bind(endPoint);
                LinkInfo = socket.LocalEndPoint.ToString() + "->" + Param.RemoteEndpointToString();
                ErrorInfo = "";
                IsLinkOk = true;
                return true;
            }
            catch (Exception e)
            {
                ErrorInfo = e.Message;
                IsLinkOk = false;
                return false;
            }
        }

        private bool CloseSocket()
        {
            socket.Close();
            IsLinkOk = false;
            return true;
        }

        public bool Close()
        {
            return CloseSocket();
        }

        public bool Open()
        {
            return OpenSocket();
        }

        public bool Read(byte[] pBuf, ref int readSize)
        {
            try
            {
                EndPoint point = new IPEndPoint(IPAddress.Parse(Param.RemoteIp), Param.RemotePort);
                readSize = socket.ReceiveFrom(pBuf, ref point);

                if(Param.RemotePort == 0 || Param.RemoteIp == "0.0.0.0")
                {
                    if (Param.RemoteIp == "0.0.0.0")
                        lastRemoteIp = (point as IPEndPoint).Address.ToString();
                    if (Param.RemotePort == 0)
                        lastRemoteport = (point as IPEndPoint).Port;
                    var info = socket.LocalEndPoint.ToString() + "->" + point.ToString();
                    if (info != LinkInfo)
                        LinkInfo = info;
                }
            }
            catch (Exception)
            {
                IsLinkOk = false;
                return false;
            }
            return true;
        }

        public bool Write(byte[] pData, ref int writeSize)
        {
            int Start = 0;
            while (writeSize > 0)
            {
                int NeedSendSize = writeSize;
                const int udpmaxsize = 65507;
                if (NeedSendSize > udpmaxsize)
                    NeedSendSize = udpmaxsize;

                try
                {
                    string ip = (Param.RemoteIp == "0.0.0.0") ? lastRemoteIp : Param.RemoteIp;
                    int port = (Param.RemotePort == 0) ? lastRemoteport : Param.RemotePort;
                    EndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
                    int sendSize = socket.SendTo(pData, Start, NeedSendSize, SocketFlags.None, point);
                    if (sendSize != NeedSendSize)
                        return false;
                    writeSize -= sendSize;
                    Start += sendSize;
                }
                catch (Exception)
                {
                    IsLinkOk = false;
                    return false;
                }

            }
            return true;
        }
    }

    public class TcpIo : ICommunicateIo
    {
        public NetIoParam Param { get; set; } = new NetIoParam();
        public string NickName { get; set; } = "Tcp";

        public string LinkInfo { get; set; } = "";

        public string FullInfo => NickName + "\r\n" + LinkInfo;

        public string ErrorInfo { get; set; } = "";

        public bool IsLinkOk { get; set; } = false;

        private Socket socket;


        private bool OpenSocket()
        {
            //创建监听用的Socket 
            /*
             *   AddressFamily.InterNetWork：使用 IP4地址。
                SocketType.Stream：支持可靠、双向、基于连接的字节流，而不重复数据。此类型的 Socket 与单个对方主机进行通信，并且在通信开始之前需要远程主机连接。Stream 使用传输控制协议 (Tcp) ProtocolType 和 InterNetworkAddressFamily。
                ProtocolType.Tcp：使用传输控制协议。
             */
            //使用IPv4地址，流式Socket方式，tcp协议传递数据
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int port = 0;
            port = Param.RemotePort;
            IPEndPoint endPoint;
            if (Param.RemotePort > 0 && !string.IsNullOrEmpty(Param.RemoteIp))
                endPoint = new IPEndPoint(IPAddress.Parse(Param.RemoteIp), port);
            else
                endPoint = new IPEndPoint(IPAddress.Any, port);
            return ConnectTimeOut(endPoint);
        }

        private bool IsConnectSuccessful = false;
        private ManualResetEvent TimeOutObject = new ManualResetEvent(false);
        private bool ConnectTimeOut(IPEndPoint endPoint)
        {
            TimeOutObject.Reset();
            socket.BeginConnect(endPoint, new AsyncCallback(callBackConnect), socket);
            if (TimeOutObject.WaitOne(2000, false))
            {
                if (IsConnectSuccessful)
                {
                    ErrorInfo = "";
                    IsLinkOk = true;
                    LinkInfo = socket.LocalEndPoint.ToString() + "->" + endPoint.ToString();
                    return true;
                }
                else
                {
                    IsLinkOk = false;
                    return false;
                }
            }
            else
            {
                ErrorInfo = "无法连接TcpServer";
                socket.Close();
                IsLinkOk = false;
                return false;
            }

        }

        private void callBackConnect(IAsyncResult ar)
        {
            try
            {
                IsConnectSuccessful = false;
                Socket tcpClient = (Socket)ar.AsyncState;

                if (tcpClient != null)
                {
                    tcpClient.EndConnect(ar);
                    IsConnectSuccessful = true;
                }
            }
            catch (Exception e)
            {
                ErrorInfo = e.Message;
                IsConnectSuccessful = false;
            }
            finally
            {
                TimeOutObject.Set();
            }

        }

        private bool CloseSocket()
        {
            socket.Close();
            IsLinkOk = false;
            return true;
        }

        public bool Close()
        {
            return CloseSocket();
        }

        public bool Open()
        {
            return OpenSocket();
        }

        public bool Read(byte[] pBuf, ref int readSize)
        {
            try
            {
                readSize = socket.Receive(pBuf);
            }
            catch (Exception)
            {
                IsLinkOk = false;
                return false;
            }
            return true;
        }

        public bool Write(byte[] pData, ref int writeSize)
        {
            try
            {
                writeSize = socket.Send(pData);
                Console.WriteLine("Send :{0}", pData);
                return true;
            }
            catch (Exception)
            {
                IsLinkOk = false;
                return false;
            }
        }
    }
}
