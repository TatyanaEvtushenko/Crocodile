using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ClassLibrary
{
    public abstract class BaseConecter
    {
        protected IPEndPoint remoteEndPoint = null;

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

        protected void ReceiveStr(byte[] data, IPEndPoint remoteEndPoint)
        {
            var command = (Commands)data[0];
            var tryData = new byte[data.Length - 1];
            Array.Copy(data, 1, tryData, 0, tryData.Length);
            var str = Encoding.Unicode.GetString(tryData);
            var length = str.IndexOf('\0');
            if (length >= 0)
                str = str.Substring(0, length);
            GetReceiveData(command, str, remoteEndPoint);
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
        protected abstract void SendToRemote(byte[] data, IPEndPoint remoteEndPoint);
        protected abstract void Receive();
        protected abstract void GetReceiveData(Commands command, string str, IPEndPoint remoteEndPoint);
        protected abstract IEnumerable<int> GetUsedPorts();
    }
}
