using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common.util
{
    class ListReader<T>
    {

        public List<T> read(List<Object> input)
        {
            List<T> rs = new List<T>();
            if (input == null)
                return rs;
            foreach(var o in input)
            {
                T v = (T)o;
                rs.Add(v);
            }
            return rs;
        }

        public List<T[]> read2(List<Object> input)
        {
            List<T[]> rs = new List<T[]>();
            if (input == null)
                return rs;
            foreach (var o in input)
            {
                Object[] vs = (Object[])o;
                T[] ret = new T[vs.Length];
                for( int i = 0; i < vs.Length; i++ )
                {
                    ret[ i ] = ( T )vs[ i ];
                }
                rs.Add(ret);
            }
            return rs;
        }
    }
}
