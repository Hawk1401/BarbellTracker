using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.AbstractionCode
{
    public class Vector2D
    {
        public static double EPSILON = 0.005;  // Mabey we shoud use some better epsilon
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

        public double length()
        {
            double xSquare = X * X;
            double ySquare = Y * Y;

            return Math.Sqrt(xSquare + ySquare);
        }

        public void normalize()
        {
            if(length() == 0)
            {
                throw new DivideByZeroException($"The length of the vector {this} has the length of zero an can not be normalized");
            }

            scalar(1 / length());
        }



        public void Add(Vector2D other)
        {
            this.X += other.X;
            this.Y += other.Y;
        }

        public static Vector2D Add(Vector2D first, Vector2D second)
        {
            return new Vector2D(
                first.X + second.X,
                first.Y + second.Y
                );
        }



        public void Sub(Vector2D other)
        {
            this.X -= other.X;
            this.Y -= other.Y;
        }

        public static Vector2D Sub(Vector2D first, Vector2D second)
        {
            return new Vector2D(
                first.X - second.X,
                first.Y - second.Y
                );
        }

        public void scalar(double num)
        {
            this.X *= num;
            this.Y *= num;
        }


        public double dotProduct(Vector2D other) 
        {
            return dotProduct(this, other);
        }

        public static double dotProduct(Vector2D first, Vector2D second)
        {
            return (first.X * second.X) + (first.Y * second.Y);
        }



        public double crossProduct(Vector2D other)
        {
            return crossProduct(this, other);
        }

        public static double crossProduct(Vector2D first, Vector2D second)
        {
            return (first.X * second.Y) - (second.X * first.Y);
        }



        public bool islinearlyIndependen(Vector2D other)
        {
            return islinearlyIndependen(other, EPSILON);
        }

        public bool islinearlyIndependen(Vector2D other, double epsilon)
        {
            return islinearlyIndependen(this, other, epsilon);
        }


        public static bool islinearlyIndependen(Vector2D first, Vector2D second)
        {
            return islinearlyIndependen(first, second, EPSILON);
        }

        public static bool islinearlyIndependen(Vector2D first, Vector2D second, double epsilon)
        {
            return Math.Abs(crossProduct(first, second)) <= epsilon;
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
