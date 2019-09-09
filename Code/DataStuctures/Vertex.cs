namespace qem
{
    public class Vertex
    {
        public Vector Vector;
        public Matrix Quadric;

        public Vertex(Vector v)
        {
            Vector = v;
            Quadric = new Matrix();
        }

        public Vertex(Vector v, Matrix q)
        {
            Vector = v;
            Quadric = q;
        }
    }
}