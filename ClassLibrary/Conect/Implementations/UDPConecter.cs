using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public abstract class UDPConecter : BaseConecter
    {
        private UdpClient connect;

        protected UDPConecter()
        {
            connect = new UdpClient(GetFreePort());
            connect.JoinMulticastGroup(IPAddress.Parse(ConnectInfo.GroupAddr));
            Task.Factory.StartNew(Receive);
        }

        ~UDPConecter()
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
                var data = connect.Receive(ref remoteEndPoint);
                ReceiveStr(data, remoteEndPoint);
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

        protected override void SendToRemote(byte[] data, IPEndPoint ipEndPoint)
        {
            connect.Send(data, data.Length, ipEndPoint);
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
