using System.Linq;

namespace qem
{
    public struct Mesh
    {
        public Triangle[] tris;

        public int trisCount => tris.Length;
        public int vertexCount => tris.SelectMany(x => new[] { x.v1, x.v2, x.v3 }).Distinct().Count();
    }
}