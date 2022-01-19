using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowerClassLibrary
{
    public class WorkoutPacket
    {
        public StrokePacket Stroke { get; set; }
        public TimeSpan TotalTime { get; set; }
        public float TotalDistance { get; set; }
        public int StrokesPrMin { get; set; }
        public TimeSpan Split { get; set; }
        public int Pulse { get; set; }
        public int Effect { get; set; }
        public int Calories { get; set; }

    }
}
