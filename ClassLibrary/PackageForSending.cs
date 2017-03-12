using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class PackageForSending
    {
        public Commands Command { get; set; }
        public byte[] Data { get; set; }
    }
}
