using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowerClassLibrary
{
    public class DisplayPacket
    {
        public TimeSpan ElapsedTime { get; set; }
        public int TotalDistance { get; set; }
        public int StrokePrMin { get; set; }
        public TimeSpan Split { get; set; }
        public int Pulse { get; set; } = 0;
        public int Effect { get; set; } = 0;
        public int Calories { get; set; } = 0;
    }
}
