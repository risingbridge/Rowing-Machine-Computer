using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowerDataCollector.Classes
{
    internal class WorkoutDataPoint
    {
        public WorkoutDataPoint(DateTime timestamp, int distance, int numberOfStrokes, int pulse, int watt, int kCal)
        {
            Timestamp = timestamp;
            Distance = distance;
            NumberOfStrokes = numberOfStrokes;
            Pulse = pulse;
            Watt = watt;
            KCal = kCal;
        }

        public DateTime Timestamp { get; set; }
        public int Distance { get; set; }
        public int NumberOfStrokes { get; set; }
        public int Pulse { get; set; }
        public int Watt { get; set; }
        public int KCal { get; set; }
    }
}
