using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.AbstractionCode
{
    public class Vector2D
    {
        private static double EPSILON = 0.005;  // Mabey we shoud use some better epsilon
        public double X { get; set; }
        public double Y { get; set; }


        public Vector2D()
        {
            X = 0;
            Y = 0;
        }

        public Vector2D( double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vector2D(Vector2D other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }

        public double Length()
        {
            double xSquare = X * X;
            double ySquare = Y * Y;

            return Math.Sqrt(xSquare + ySquare);
        }

        public Vector2D Normalize()
        {
            if(Length() == 0)
            {
                throw new DivideByZeroException($"The length of the vector {this} has the length of zero an can not be normalized");
            }

            return Scalar(1 / Length());
        }

        public Vector2D Add(Vector2D other)
        {
            return Add(this, other);
        }

        public static Vector2D Add(Vector2D first, Vector2D second)
        {
            return new Vector2D(
                first.X + second.X,
                first.Y + second.Y
                );
        }

        public Vector2D Sub(Vector2D other)
        {
            return Sub(this, other);
        }

        public static Vector2D Sub(Vector2D first, Vector2D second)
        {
            return new Vector2D(
                first.X - second.X,
                first.Y - second.Y
                );
        }

        public Vector2D Scalar(double num)
        {
            var x = this.X * num;
            var y = this.Y * num;

            return new Vector2D(x,y);
        }

        public double DotProduct(Vector2D other) 
        {
            return DotProduct(this, other);
        }

        public static double DotProduct(Vector2D first, Vector2D second)
        {
            return (first.X * second.X) + (first.Y * second.Y);
        }

        public double CrossProduct(Vector2D other)
        {
            return CrossProduct(this, other);
        }

        public static double CrossProduct(Vector2D first, Vector2D second)
        {
            return (first.X * second.Y) - (second.X * first.Y);
        }

        public bool IslinearlyIndependen(Vector2D other)
        {
            return IslinearlyIndependen(other, EPSILON);
        }

        public bool IslinearlyIndependen(Vector2D other, double epsilon)
        {
            return IslinearlyIndependen(this, other, epsilon);
        }

        public static bool IslinearlyIndependen(Vector2D first, Vector2D second)
        {
            return IslinearlyIndependen(first, second, EPSILON);
        }

        public static bool IslinearlyIndependen(Vector2D first, Vector2D second, double epsilon)
        {
            return Math.Abs(CrossProduct(first, second)) > epsilon;
        }

        public Vector2D Copy()
        {
            return new Vector2D(this);
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(obj is Vector2D vector)
            {
                var xLowerLimet = this.X - EPSILON;
                var xUpperLimet = this.X + EPSILON;
                var yLowerLimet = this.Y - EPSILON;
                var yUpperLimet = this.Y + EPSILON;

                if(vector.X < xLowerLimet || vector.X > xUpperLimet)
                {
                    return false;
                }
                if (vector.Y < yLowerLimet || vector.Y > yUpperLimet)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"(X: {this.X.ToString("0.###")}, Y: {this.Y.ToString("0.###")})";
        }
    }
}
