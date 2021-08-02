using System.Numerics;

namespace qem
{
    public record Triangle
    {
        public Vector3 v1, v2, v3;

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Matrix Quadric()
        {
            var n = Normal();

            var x = v1.X;
            var y = v1.Y;
            var z = v1.Z;
            var a = n.X;
            var b = n.Y;
            var c = n.Z;
            var d = -a * x - b * y - c * z;

            return new Matrix(
                a * a, a * b, a * c, a * d,
                a * b, b * b, b * c, b * d,
                a * c, b * c, c * c, c * d,
                a * d, b * d, c * d, d * d
            );
        }

        public Vector3 Normal()
        {
            var e1 = v2 - v1;
            var e2 = v3 - v2;
            return e1.Cross(e2).Normalize();
        }
    }
}