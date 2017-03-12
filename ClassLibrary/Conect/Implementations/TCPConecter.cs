using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public abstract class TCPConecter : BaseConecter
    {
        private TcpListener listener;

        protected TCPConecter()
        {
            listener = new TcpListener(IPAddress.Parse(ConectInfo.ServerAddr), GetFreePort());
            listener.Start();
            Task.Factory.StartNew(Receive);
        }

        ~TCPConecter()
        {
            listener?.Stop();
        }

        protected override void Receive()
        {
            byte[] data;
            while (true)
            {
                using (var client = listener.AcceptTcpClient())
                {
                    using (var stream = client.GetStream())
                    {
                        data = new byte[client.ReceiveBufferSize];
                        stream.Read(data, 0, client.ReceiveBufferSize);
                        remoteEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
                    }
                }
                var package = new Converter<PackageForSending>().ConvertToObject(data);
                GetReceiveData(package);
            }
        }

        protected override void Send(byte[] data)
        {
            var usedPorts = GetUsedPorts();
            foreach (var port in usedPorts)
            {
                using (var client = new TcpClient())
                {
                    client.Connect(ConectInfo.ServerAddr, port);
                    using (var stream = client.GetStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        protected override void SendToRemote(byte[] data, IPEndPoint remoteEndPoint)
        {
            using (var client = new TcpClient())
            {
                client.Connect(remoteEndPoint);
                using (var stream = client.GetStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        protected override IEnumerable<int> GetUsedPorts()
        {
            var portsForChecking = Enumerable.Range(ConectInfo.StartPort, ConectInfo.MaxPortCount);
            var usedPorts = from port in portsForChecking
                            join usedPort in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners() on port equals
                                usedPort.Port
                            select port;
            return usedPorts;
        }
    }
}
