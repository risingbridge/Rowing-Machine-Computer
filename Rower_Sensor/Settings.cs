using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rower_Sensor
{
    internal class Settings
    {
        public Settings(int sensorAlpha, int sensorBravo, int sensorCharlie, int magnetsPrRev, float meterPrRev, int sleepTime)
        {
            SensorAlpha = sensorAlpha;
            SensorBravo = sensorBravo;
            SensorCharlie = sensorCharlie;
            MagnetsPrRev = magnetsPrRev;
            MeterPrRev = meterPrRev;
            SleepTime = sleepTime;
        }

        public int SensorAlpha { get; set; }
        public int SensorBravo { get; set; }
        public int SensorCharlie { get; set; }
        public int MagnetsPrRev { get; set; }
        public float MeterPrRev { get; set; }
        public int SleepTime { get; set; } //Time in minutes

        public override string ToString()
        {
            string returnString = $"" +
                $"Sensor Alpha: {SensorAlpha}\n" +
                $"Sensor Bravo: {SensorBravo}\n" +
                $"Sensor Charlie: {SensorCharlie}\n" +
                $"Magnets pr revolution: {MagnetsPrRev}\n" +
                $"Meters pr rev: {MeterPrRev}\n" +
                $"Sleep Time: {SleepTime}";

            return returnString;
        }
    }
}
