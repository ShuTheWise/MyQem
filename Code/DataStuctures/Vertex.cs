using System.Numerics;

namespace qem
{
    public class Vertex
    {
        public Vector3 Vector3;
        public Matrix Quadric;

        public Vertex(Vector3 v)
        {
            Vector3 = v;
            Quadric = new Matrix();
        }

        public Vertex(Vector3 v, Matrix q)
        {
            Vector3 = v;
            Quadric = q;
        }
    }
}