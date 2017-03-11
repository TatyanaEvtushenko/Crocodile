using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public enum Commands { Start, Enter, Exit, Message, Point, BeginPaint, Purpose }

    public enum Purpose { Guess, Paint }

    public static class ConnectInfo
    {
        public static string ServerAddr { get; } = "127.0.0.1";
        public static string GroupAddr { get; } = "235.5.5.1";
        public static int StartPort { get; } = 10000;
        public static int MaxPortCount { get; } = 500;
    }

    public abstract class BaseConnect
    {
        protected int GetFreePort()
        {
            var usedPorts = GetUsedPorts();
            var freePort = Enumerable.Range(ConnectInfo.StartPort, ConnectInfo.MaxPortCount).Except(usedPorts).FirstOrDefault();
            if (freePort == 0)
            {
                throw new Exception("There aren't free ports");
            }
            return freePort;
        }

        protected void ReceiveStr(byte[] data)
        {
            var command = (Commands)data[0];
            var tryData = new byte[data.Length-1];
            Array.Copy(data, 1, tryData, 0, tryData.Length);
            var str = Encoding.Unicode.GetString(tryData);
            var length = str.IndexOf('\0');
            if (length >= 0)
                str = str.Substring(0, length);
            GetReceiveData(command, str);
        }

        protected void SendStr(Commands command, string str)
        {
            var data = Encoding.Unicode.GetBytes(str);
            var dataForSending = new byte[data.Length + 1];
            dataForSending[0] = (byte)command;
            Array.Copy(data, 0, dataForSending, 1, data.Length);
            Send(dataForSending);
        }

        protected abstract void Send(byte[] data);
        protected abstract void Receive();
        protected abstract void GetReceiveData(Commands command, string str);
        protected abstract IEnumerable<int> GetUsedPorts();
    }

    public abstract class TCPConnect : BaseConnect
    {
        private TcpListener listener;

        protected TCPConnect()
        {
            listener = new TcpListener(IPAddress.Parse(ConnectInfo.ServerAddr), GetFreePort());
            listener.Start();
            Task.Factory.StartNew(Receive);
        }

        ~TCPConnect()
        {
            listener?.Stop();
        }

        protected override void Send(byte[] data)
        {
            var usedPorts = GetUsedPorts();
            foreach (var port in usedPorts)
            {
                using (var client = new TcpClient())
                {
                    client.Connect(ConnectInfo.ServerAddr, port);
                    using (var stream = client.GetStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        protected override void Receive()
        {
            while (true)
            {
                using (var client = listener.AcceptTcpClient())
                {
                    using (var stream = client.GetStream())
                    {
                        var data = new byte[client.ReceiveBufferSize];
                        stream.Read(data, 0, client.ReceiveBufferSize);
                        ReceiveStr(data);
                    }
                }
            }
        }

        protected override IEnumerable<int> GetUsedPorts()
        {
            var portsForChecking = Enumerable.Range(ConnectInfo.StartPort, ConnectInfo.MaxPortCount);
            var usedPorts = from port in portsForChecking
                            join usedPort in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners() on port equals
                                usedPort.Port
                            select port;
            return usedPorts;
        }
    }

    public abstract class UDPConnect : BaseConnect
    {
        private UdpClient connect;

        protected UDPConnect()
        {
            connect = new UdpClient(GetFreePort());
            connect.JoinMulticastGroup(IPAddress.Parse(ConnectInfo.GroupAddr));
            Task.Factory.StartNew(Receive);
        }

        ~UDPConnect()
        {
            try
            {
                connect?.DropMulticastGroup(IPAddress.Parse(ConnectInfo.GroupAddr));
            }
            finally
            {
                connect?.Close();
            }
        }

        protected override void Receive()
        {
            while (true)
            {
                IPEndPoint remoteEndPoint = null;
                var data = connect.Receive(ref remoteEndPoint);
                ReceiveStr(data);
            }
        }

        protected override void Send(byte[] data)
        {
            var usedPorts = GetUsedPorts();
            foreach (var usedPort in usedPorts)
            {
                connect.Send(data, data.Length, ConnectInfo.GroupAddr, usedPort);
            }
        }

        protected override IEnumerable<int> GetUsedPorts()
        {
            var portsForChecking = Enumerable.Range(ConnectInfo.StartPort, ConnectInfo.MaxPortCount);
            var usedPorts = from port in portsForChecking
                            join usedPort in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners() on port equals
                                usedPort.Port
                            select port;
            return usedPorts;
        }
    }
}
