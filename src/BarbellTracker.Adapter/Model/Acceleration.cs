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

        public override bool Equals(object obj)
        {
            if (obj is Acceleration other)
            {
                if (this.FPS != other.FPS)
                {
                    return false;
                }

                if (this.Vectors.Length != other.Vectors.Length)
                {
                    return false;
                }

                for (int i = 0; i < Vectors.Length; i++)
                {
                    if (!this.Vectors[i].Equals(other.Vectors[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
    }
}
