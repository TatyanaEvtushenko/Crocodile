using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;

namespace ClassLibrary
{
    public abstract class BaseConecter
    {
        protected IPEndPoint remoteEndPoint = null;
        protected Preparer preparer = new Preparer();

        protected int GetFreePort()
        {
            var usedPorts = GetUsedPorts();
            var freePort = Enumerable.Range(ConectInfo.StartPort, ConectInfo.MaxPortCount).Except(usedPorts).FirstOrDefault();
            if (freePort == 0)
            {
                throw new Exception("There aren't free ports");
            }
            return freePort;
        }

        protected abstract void Send(byte[] data);
        protected abstract void SendToRemote(byte[] data, IPEndPoint remoteEndPoint);
        protected abstract void GetReceiveData(PackageForSending package);
        protected abstract void Receive();
        protected abstract IEnumerable<int> GetUsedPorts();
    }
}
