using System;
using System.Collections.Generic;
using System.Linq;

namespace qem
{
    public class Pair
    {
        public Vertex A, B;
        public double CachedError;
        public bool Removed;

        public Pair(Vertex a, Vertex b)
        {
            if (a.Vector < b.Vector)
            {
                (a, b) = (b, a);
            }
            A = a;
            B = b;
            CachedError = -1;
            Removed = false;
        }

        public Vector Vector()
        {
            var q = Quadric();

            if (Math.Abs(q.Determinant()) > 1e-3)
            {
                var v = q.QuadricVector();
                if (!double.IsNaN(v.X) && !double.IsNaN(v.Y) && !double.IsNaN(v.Z))
                    return v;
            }

            //cannot compute best vector with matrix 
            // look for vest along edge
            int n = 32;
            var a = A.Vector;
            var b = B.Vector;
            var bestE = -1d;
            var bestV = new Vector();

            for (int i = 0; i < n; i++)
            {
                int frac = i * (1 / n);
                var v = qem.Vector.Lerp(a, b, frac);
                var e = A.Quadric.QuadricError(v);
                if (bestE < 0 || e < bestE)
                {
                    bestE = e;
                    bestV = v;
                }
            }
            return bestV;
        }

        public double Error()
        {
            if (CachedError < 0)
            {
                CachedError = Quadric().QuadricError(Vector());
            }
            return CachedError;
        }

        public Matrix Quadric()
        {
            return A.Quadric.Add(B.Quadric);
        }

        public struct Key
        {
            public Vector A, B;
            public Key(Vector a, Vector b)
            {
                A = a;
                B = b;
            }

            public static Key Make(Vertex a, Vertex b)
            {
                if (a.Vector < b.Vector)
                {
                    return new Key(a.Vector, b.Vector);
                }
                return new Key(b.Vector, a.Vector);
            }
        }
    }
}