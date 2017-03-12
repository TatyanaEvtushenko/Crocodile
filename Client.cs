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

        public Client(MainWindow wind, string username)
        {
            window = wind;
            Username = username;
        }

        public void SendEnter()
        {
            var data = preparer.GetStrDataForSending(Commands.Enter, Username + " has joined");
            Send(data);
        }

        public void SendLogOff()
        {
            var data = preparer.GetStrDataForSending(Commands.Exit, Username + " has left a game");
            Send(data);
        }

        public void SendMessage(string message)
        {
            var data = preparer.GetStrDataForSending(Commands.Message, Username + ": " + message);
            Send(data);
        }

        public void SendPoint(Point point)
        {
            var data = preparer.GetPointDataForSending(Commands.Point, point);
            Send(data);
        }

        public void SendBeginPaint(Point point)
        {
            var data = preparer.GetPointDataForSending(Commands.BeginPaint, point);
            Send(data);
        }

        protected override void GetReceiveData(PackageForSending package)
        {
            switch (package.Command)
            {
                case Commands.Enter:
                case Commands.Exit:
                case Commands.Message:
                    var str = new Converter<string>().ConvertToObject(package.Data);
                    if (package.Command == Commands.Message)
                        window.PrintStr(DateTime.Now.ToShortTimeString() + ", " + str);
                    else
                        window.PrintStr("   " + str);
                    break;
                case Commands.Point:
                case Commands.BeginPaint:
                    var point = new Converter<Point>().ConvertToObject(package.Data);
                    if (package.Command == Commands.Point)
                        window.PrintPoint(point);
                    else
                        window.LastPoint = point;
                    break;
            }
        }
    }
}
