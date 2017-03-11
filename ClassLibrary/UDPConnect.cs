using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClassLibrary
{
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
