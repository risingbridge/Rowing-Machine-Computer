using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowerClassLibrary
{
    public class StrokePacket
    {
        public StrokePacket(int revolutions, float distance, DateTime strokeStartTimestamp, DateTime strokeEndTimestamp, TimeSpan timeSinceStart)
        {
            Revolutions = revolutions;
            Distance = distance;
            StrokeStartTimestamp = strokeStartTimestamp;
            StrokeEndTimestamp = strokeEndTimestamp;
            TimeSinceStart = timeSinceStart;
        }

        public int Revolutions { get; set; }
        public float Distance { get; set; }
        public DateTime StrokeStartTimestamp { get; set; }
        public DateTime StrokeEndTimestamp { get; set; }
        public TimeSpan TimeSinceStart { get; set; }
        public TimeSpan StrokeDuration { get { 
            return StrokeEndTimestamp - StrokeStartTimestamp; 
        } }

    }
}
