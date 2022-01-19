using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RowerClassLibrary
{
    public class WorkoutLog
    {
        public string User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float TotalDistance { get; set; }
        public List<WorkoutPacket> WorkoutList { get; set; }
    }
}
