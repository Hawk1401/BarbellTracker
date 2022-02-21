using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Model
{
    public class CSVVelocityModel
    {
        public string Time { get; set; }
        public int Length { get; set; }
        public string Vector { get; set; }

        public CSVVelocityModel(string time, int length, string vector)
        {
            this.Time = time;
            this.Vector = vector;
            this.Length = length;
        }

        public static string GetHeader()
        {
            return "Time;Length;Vector";
        }

        public override string ToString()
        {
            return $"{Time};{Length};{Vector}";
        }
    }
}
