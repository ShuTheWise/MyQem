using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qem
{
    public static class QEMAlgorithm
    {
        private static void AddVertex(Vector v, Dictionary<Vector, Vertex> dic)
        {
            if (!dic.ContainsKey(v))
                dic.Add(v, new Vertex(v));
        }

        private static int CompFloats(double f, double f2)
        {
            return (f > f2) ? 1 : -1;
        }

        public static Mesh Simplify(this Mesh originalMesh, int targetCount)
        {
            // gather distinct vertices
            Dictionary<Vector, Vertex> vectorVertex = new Dictionary<Vector, Vertex>();

            foreach (Triangle t in originalMesh.tris)
            {
                AddVertex(t.v1, vectorVertex);
                AddVertex(t.v2, vectorVertex);
                AddVertex(t.v3, vectorVertex);
            }

            // accumulate quadric matrices for each vertex based on its faces
            // assign initial quadric
            foreach (Triangle t in originalMesh.tris)
            {
                Matrix q = t.Quadric();
                Vertex v1 = vectorVertex[t.v1];
                Vertex v2 = vectorVertex[t.v2];
                Vertex v3 = vectorVertex[t.v3];

                v1.Quadric = v1.Quadric.Add(q);
                v2.Quadric = v2.Quadric.Add(q);
                v3.Quadric = v3.Quadric.Add(q);
            }

            //vertex -> face map
            Dictionary<Vertex, List<Face>> vertexFaces = new Dictionary<Vertex, List<Face>>();
            foreach (Triangle t in originalMesh.tris)
            {
                Vertex v1 = vectorVertex[t.v1];
                Vertex v2 = vectorVertex[t.v2];
                Vertex v3 = vectorVertex[t.v3];

                Face f = new Face(v1, v2, v3);

                vertexFaces.AppendEx(v1, f);
                vertexFaces.AppendEx(v2, f);
                vertexFaces.AppendEx(v3, f);
            }

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            //gather distinct pairs
            Dictionary<Pair.Key, Pair> pairs = new Dictionary<Pair.Key, Pair>();
            foreach (Triangle t in originalMesh.tris)
            {
                Vertex v1 = vectorVertex[t.v1];
                Vertex v2 = vectorVertex[t.v2];
                Vertex v3 = vectorVertex[t.v3];

                var one = sw.ElapsedMilliseconds;

                pairs.AddPair(v1, v2);
                pairs.AddPair(v2, v3);
                pairs.AddPair(v1, v3);

            }
            Console.WriteLine($"total: {sw.ElapsedMilliseconds}");

            Dictionary<Vertex, List<Pair>> vertexPairs = new Dictionary<Vertex, List<Pair>>();

            foreach (KeyValuePair<Pair.Key, Pair> p in pairs)
            {
                vertexPairs.AppendEx(p.Value.A, p.Value);
                vertexPairs.AppendEx(p.Value.B, p.Value);
            }

            var priorityQueue = new SimplePriorityQueue<Pair, double>(CompFloats);
            foreach (KeyValuePair<Pair.Key, Pair> item in pairs)
            {
                item.Value.Error();
                priorityQueue.Enqueue(item.Value, item.Value.CachedError);
            }

            //take best pair
            int currentFaceCount = originalMesh.tris.Length;
            int targetFaceCount = targetCount;
            while (currentFaceCount > targetFaceCount && priorityQueue.Count > 0)
            {
                //best pair
                Pair p = priorityQueue.Dequeue();

                if (p.Removed)
                    continue;

                p.Removed = true;

                //get distinct faces 
                var distinctFaces = new HashSet<Face>();
                if (vertexFaces.ContainsKey(p.A))
                    foreach (var f in vertexFaces[p.A])
                    {
                        if (!f.Removed)
                        {
                            if (!distinctFaces.Contains(f))
                                distinctFaces.Add(f);
                        }
                    }

                if (vertexFaces.ContainsKey(p.B))
                    foreach (var f in vertexFaces[p.B])
                    {
                        if (!f.Removed)
                        {
                            if (!distinctFaces.Contains(f))
                                distinctFaces.Add(f);
                        }
                    }

                //get related pairs
                var distintPairs = new HashSet<Pair>();
                if (vertexPairs.ContainsKey(p.A))
                    foreach (var q in vertexPairs[p.A])
                    {
                        if (!q.Removed)
                        {
                            if (!distintPairs.Contains(q))
                                distintPairs.Add(q);
                        }
                    }

                if (vertexPairs.ContainsKey(p.B))
                    foreach (var q in vertexPairs[p.B])
                    {
                        if (!q.Removed)
                        {
                            if (!distintPairs.Contains(q))
                                distintPairs.Add(q);
                        }
                    }

                //create new vertex
                Vertex v = new Vertex(p.Vector(), p.Quadric());

                //updateFaces
                var newFaces = new List<Face>();
                bool valid = true;
                foreach (var f in distinctFaces)
                {
                    var (v1, v2, v3) = (f.V1, f.V2, f.V3);

                    if (v1 == p.A || v1 == p.B)
                        v1 = v;

                    if (v2 == p.A || v2 == p.B)
                        v2 = v;

                    if (v3 == p.A || v3 == p.B)
                        v3 = v;

                    var face = new Face(v1, v2, v3);

                    if (face.Degenerate)
                        continue;

                    if (face.Normal().Dot(f.Normal()) < 1e-3)
                    {
                        valid = false;
                        break;
                    }

                    newFaces.Add(face);
                }

                if (!valid)
                    continue;

                if (vertexFaces.ContainsKey(p.A))
                    vertexFaces.Remove(p.A);

                if (vertexFaces.ContainsKey(p.B))
                    vertexFaces.Remove(p.B);

                foreach (var f in distinctFaces)
                {
                    f.Removed = true;
                    currentFaceCount--;
                }

                foreach (var f in newFaces)
                {
                    currentFaceCount++;
                    vertexFaces.AppendEx(f.V1, f);
                    vertexFaces.AppendEx(f.V2, f);
                    vertexFaces.AppendEx(f.V3, f);
                }

                if (vertexPairs.ContainsKey(p.A))
                    vertexPairs.Remove(p.A);

                if (vertexPairs.ContainsKey(p.B))
                    vertexPairs.Remove(p.B);

                var seen = new Dictionary<Vector, bool>();

                foreach (var q in distintPairs)
                {
                    q.Removed = true;
                    priorityQueue.Remove(q);
                    var (a, b) = (q.A, q.B);

                    if (a == p.A || a == p.B)
                    {
                        a = v;
                    }
                    if (b == p.A || b == p.B)
                    {
                        b = v;
                    }
                    if (b == v)
                    {
                        (a, b) = (b, a);
                        // a = v
                    }
                    if (seen.ContainsKey(b.Vector) && seen[b.Vector])
                    {
                        //ignore duplicates
                        continue;
                    }
                    if (!seen.ContainsKey(b.Vector))
                        seen.Add(b.Vector, true);
                    else
                        seen[b.Vector] = true;

                    var np = new Pair(a, b);
                    np.Error();
                    priorityQueue.Enqueue(np, np.CachedError);

                    vertexPairs.AppendEx(a, np);
                    vertexPairs.AppendEx(b, np);
                }
            }

            //gather distinct faces
            var finalDistinctFaces = new HashSet<Face>();
            foreach (var faces in vertexFaces)
            {
                foreach (var face in faces.Value)
                {
                    if (!face.Removed)
                    {
                        if (!finalDistinctFaces.Contains(face))
                            finalDistinctFaces.Add(face);
                    }
                }
            }

            //create final mesh
            Mesh newMesh = new Mesh
            {
                tris = finalDistinctFaces.Select(x => new Triangle(x.V1.Vector, x.V2.Vector, x.V3.Vector)).ToArray()
            };

            return newMesh;
        }
    }
}
