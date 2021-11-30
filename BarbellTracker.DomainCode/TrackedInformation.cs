using BarbellTracker.AbstractionCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.DomainCode
{
    class TrackedInformation
    {
        public string Name { get; set; }
        public Vector2D[] Positions { get; set; }
        public int FrameRate { get; private set; }

        public int PixelPerCm { get; set; }
    }
}
