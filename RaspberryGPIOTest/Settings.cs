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
        public int HallPinB { get; set; }
        public int HallPinC { get; set; }

        public override string ToString()
        {
            string outString = $"" +
                $"Number of magnets pr revolution: {Magnets}\n" +
                $"Hall Sensor Alpha Pin: \t{HallPinA}\n" +
                $"Hall Sensor Bravo Pin: \t{HallPinB}\n" +
                $"Hall Sensor Charlie Pin: \t{HallPinC}";
            return outString;
        }
    }
}
