using System.Windows;

namespace ClassLibrary
{
    public class Preparer
    {
        public byte[] GetStrDataForSending(Commands command, string str)
        {
            var serialazideData = new Converter<string>().ConvertToBytes(str);
            return GetDataForSending(command, serialazideData);
        }

        public byte[] GetPointDataForSending(Commands command, Point point)
        {
            var serialazideData = new Converter<Point>().ConvertToBytes(point);
            return GetDataForSending(command, serialazideData);
        }

        public byte[] GetPurposeDataForSending(Commands command, Purpose purpose)
        {
            var serialazideData = new Converter<Purpose>().ConvertToBytes(purpose);
            return GetDataForSending(command, serialazideData);
        }

        public byte[] GetPlayInfoDataForSending(Commands command, PlayInfo info)
        {
            var serialazideData = new Converter<PlayInfo>().ConvertToBytes(info);
            return GetDataForSending(command, serialazideData);
        }

        private byte[] GetDataForSending(Commands command, byte[] data)
        {
            var package = new PackageForSending
            {
                Command = command,
                Data = data
            };
            return new Converter<PackageForSending>().ConvertToBytes(package);
        }
    }
}
