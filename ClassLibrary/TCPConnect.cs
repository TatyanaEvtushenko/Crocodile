using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClassLibrary
{
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
}
