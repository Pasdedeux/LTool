using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common.util
{
    class DictionaryReader<K, V>
    {
        public Dictionary<K, V> read(Dictionary<Object, Object> input)
        {
            Dictionary<K, V> rs = new Dictionary<K, V>();
            if (input == null)
                return rs;
            foreach (var o in input)
            {
                K key = (K)o.Key;
                V value = (V)o.Value;
                rs[key] = value;
            }
            return rs;
        }

        public Dictionary<K, V[]> read2(Dictionary<Object, Object> input)
        {
            Dictionary<K, V[]> rs = new Dictionary<K, V[]>();
            if (input == null)
                return rs;
            foreach (var o in input)
            {
                K key = (K)o.Key;
                Object[] vs = (Object[])o.Value;
                int len = vs.Length;
                V[] ret = new V[len];
                for (int i = 0; i < len; i++)
                    ret[i] = (V)vs[i];
                rs.Add(key, ret);
            }
            return rs;
        }
    }
}
