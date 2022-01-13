using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryGPIOTest
{
    internal class Settings
    {
        public int Magnets { get; set; }
        public int HallPinA { get; set; }

        public override string ToString()
        {
            string outString = $"" +
                $"Number of magnets pr revolution: {Magnets}\n" +
                $"Hall Sensor Alpha Pin: {HallPinA}";
            return outString;
        }
    }
}
