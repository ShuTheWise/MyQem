using System;
namespace qem
{
    public struct Vector
    {
        public double X, Y, Z;

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length => Convert.ToDouble(Math.Sqrt(X * X + Y * Y + Z * Z));
        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector operator *(Vector a, double f) => new Vector(a.X * f, a.Y * f, a.Z * f);
        public static Vector operator /(Vector a, double f) => new Vector(a.X / f, a.Y / f, a.Z / f);


        public static bool operator ==(Vector a, Vector b) => Equals(a, b);
        public static bool operator !=(Vector a, Vector b) => !Equals(a, b);
        public static bool operator >(Vector a, Vector b) => a.Less(b);
        public static bool operator <(Vector a, Vector b) => !a.Less(b);

        public static Vector Lerp(Vector a, Vector b, double frac)
        {
            var x = Lerp(a.X, b.X, frac);
            var y = Lerp(a.Y, b.Y, frac);
            var z = Lerp(a.Z, b.Z, frac);
            return new Vector(x, y, z);
        }

        public static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public override bool Equals(object obj)
        {
            var other = (Vector)obj;
            return nearlyEqual(other.X, X) && nearlyEqual(other.Y, Y) && nearlyEqual(other.Z, Z);
        }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}", X, Y, Z).GetHashCode();
        }

        private bool Less(Vector b)
        {
            if (X != b.X)
            {
                return X < b.X;
            }
            if (Y != b.Y)
            {
                return Y < b.Y;
            }
            return Z < b.Z;
        }

        public static bool nearlyEqual(double a, double b)
        {
            var epsilon = double.Epsilon;
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < double.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * double.MaxValue);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public override string ToString()
        {
            return $"({X};{Y};{Z})";
        }

        public Vector Normalize() => this * (1d / Length);
    }

    public static class VectorEx
    {
        public static Vector Cross(this Vector a, Vector b)
        {
            var x = a.Y * b.Z - a.Z * b.Y;
            var y = a.Z * b.X - a.X * b.Z;
            var z = a.X * b.Y - a.Y * b.X;

            return new Vector(x, y, z);
        }

        public static double Dot(this Vector a, Vector b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
}