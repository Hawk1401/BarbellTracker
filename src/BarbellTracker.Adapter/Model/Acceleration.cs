using BarbellTracker.AbstractionCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Adapter.Model
{
    public class Acceleration
    {
        public Vector2D[] Vectors { get; set; }
        public int FPS { get; set; }
    }
}
