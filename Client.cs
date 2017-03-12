using System;
using System.Net;
using System.Text;
using System.Windows;
using ClassLibrary;

namespace Crocodile
{
    class Client : TCPConecter
    {
        public string Username { get; set; }
        private readonly MainWindow window;
        private readonly char separator = ' ';

        public Client(MainWindow wind, string username)
        {
            window = wind;
            Username = username;
        }

        public void SendEnter()
        {
            SendStr(Commands.Enter, Username + " has joined");
        }

        public void SendLogOff()
        {
            SendStr(Commands.Exit, Username + " has left a game");
        }

        public void SendMessage(string message)
        {
            SendStr(Commands.Message, Username + ": " + message);
        }

        public void SendPoint(double x, double y)
        {
            SendStr(Commands.Point, x.ToString() + separator + y);
        }

        public void SendBeginPaint(double x, double y)
        {
            SendStr(Commands.BeginPaint, x.ToString() + separator + y);
        }

        protected override void GetReceiveData(Commands command, string str, IPEndPoint remoteEndPoint)
        {
            switch (command)
            {
                case Commands.Enter:
                case Commands.Exit:
                    window.PrintStr("\t" + str);
                    break;
                case Commands.Message:
                    window.PrintStr(DateTime.Now.ToShortTimeString() + ", " + str);
                    break;
                case Commands.Point:
                case Commands.BeginPaint:
                    var indexSeparator = str.IndexOf(separator);
                    var x = Convert.ToDouble(str.Substring(0, indexSeparator));
                    var y = Convert.ToDouble(str.Substring(indexSeparator + 1, str.Length - indexSeparator - 1));
                    if (command == Commands.Point)
                        window.PrintPoint(x, y);
                    else
                    {
                        window.LastPoint = new Point(x, y);
                    }
                    break;
            }
        }
    }
}
