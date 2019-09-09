using System.Collections.Generic;

namespace qem
{
    public static class Extensions
    {
        public static void AppendEx<K, T>(this Dictionary<K, List<T>> dic, K key, T newEntry)
        {
            if (dic.ContainsKey(key))
            {
                List<T> value = dic[key];
                value.Add(newEntry);
            }
            else
            {
                dic.Add(key, new List<T> { newEntry });
            }
        }

        public static void AddPair(this Dictionary<Pair.Key, Pair> dic, Vertex a, Vertex b)
        {
            Pair.Key key = Pair.Key.Make(a, b);
            if (!dic.ContainsKey(key))
                dic.Add(Pair.Key.Make(a, b), new Pair(a, b));
        }
    }
}