using System.IO;
using System.Numerics;
using IxMilia.Stl;

namespace qem
{
    public static class STLParser
    {
        private static Triangle FromSTLTriangle(StlTriangle stlTriangle)
        {
            return new Triangle
            (
                FromSTLVertex(stlTriangle.Vertex1),
                FromSTLVertex(stlTriangle.Vertex2),
                FromSTLVertex(stlTriangle.Vertex3)
            );
        }

        private static Vector3 FromSTLVertex(StlVertex stlVertex)
        {
            return new Vector3(stlVertex.X, stlVertex.Y, stlVertex.Z);
        }

        private static StlTriangle ToSTLTriangle(Triangle triangle)
        {
            var normal = triangle.Normal();

            return new StlTriangle(
                new StlNormal((float)normal.X, (float)normal.Y, (float)normal.Z),
                ToSTLVertex(triangle.v1),
                ToSTLVertex(triangle.v2),
                ToSTLVertex(triangle.v3)
                );
        }

        private static StlVertex ToSTLVertex(Vector3 vector)
        {
            return new StlVertex((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        public static void SaveSTL(Mesh mesh, string path)
        {
            StlFile stlFile = new StlFile();
            stlFile.SolidName = "my-solid";
            for (int i = 0; i < mesh.tris.Length; i++)
            {
                Triangle item = mesh.tris[i];
                stlFile.Triangles.Add(ToSTLTriangle(item));
            }
            try
            {

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    stlFile.Save(fs, false);
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Exception during saving STL occured: \n{e.Message}");
            }
        }

        public static Mesh LoadSTL(string path)
        {
            StlFile stlFile;
            Mesh m;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                stlFile = StlFile.Load(fs);
                Triangle[] tris = new Triangle[stlFile.Triangles.Count];
                for (int i = 0; i < tris.Length; i++)
                {
                    tris[i] = FromSTLTriangle(stlFile.Triangles[i]);
                }
                m.tris = tris;
            }
            return m;
        }
    }
}