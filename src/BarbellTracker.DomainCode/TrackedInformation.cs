using BarbellTracker.AbstractionCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.DomainCode
{
    public class TrackedInformation
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public Vector2D[] Positions { get; init; }
        public int FrameRate { get; init; }
        public int PixelPerCm { get; init; }


        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(obj is TrackedInformation other)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
