namespace qem
{
    public class Face
    {
        public Vertex V1, V2, V3;
        public bool Removed;

        public Face(Vertex v1, Vertex v2, Vertex v3)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            Removed = false;
        }

        public bool Degenerate => V1 == V2 || V1 == V3 || V2 == V3;

        //public override bool Equals(object other)
        //{
        //    var t = other as Face;
        //    if (t == null)
        //    {
        //        return false;
        //    }
        //    t.no
        //}

        public override string ToString()
        {
            return $"V1: {V1.Vector} V2: {V2.Vector} V3: {V3.Vector}";
        }

        public bool Equals2(Vertex v1, Vertex v2, Vertex v3)
        {
            return Share1V(v1) && Share1V(v2) && Share1V(v3);
        }

        private bool Share1V(Vertex v)
        {
            return v == V2 || v == V2 || v == V3;
        }

        //public override bool Equals(object obj)
        //{
        //    return base.Equals(obj);
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        public Vector Normal()
        {
            Vector e1 = V2.Vector - V1.Vector;
            Vector e2 = V3.Vector - V1.Vector;
            return e1.Cross(e2).Normalize();
        }
    }
}