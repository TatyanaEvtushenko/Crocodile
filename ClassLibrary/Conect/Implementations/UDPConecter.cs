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
            connect.JoinMulticastGroup(IPAddress.Parse(ConectInfo.GroupAddr));
            Task.Factory.StartNew(Receive);
        }

        ~UDPConecter()
        {
            try
            {
                connect?.DropMulticastGroup(IPAddress.Parse(ConectInfo.GroupAddr));
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
                var package = new Converter<PackageForSending>().ConvertToObject(data);
                GetReceiveData(package);
            }
        }

        protected override void Send(byte[] data)
        {
            var usedPorts = GetUsedPorts();
            foreach (var usedPort in usedPorts)
            {
                connect.Send(data, data.Length, ConectInfo.GroupAddr, usedPort);
            }
        }

        protected override void SendToRemote(byte[] data, IPEndPoint ipEndPoint)
        {
            connect.Send(data, data.Length, ipEndPoint);
        }

        protected override IEnumerable<int> GetUsedPorts()
        {
            var portsForChecking = Enumerable.Range(ConectInfo.StartPort, ConectInfo.MaxPortCount);
            var usedPorts = from port in portsForChecking
                            join usedPort in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners() on port equals
                                usedPort.Port
                            select port;
            return usedPorts;
        }
    }
}
