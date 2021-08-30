using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.lib.util
{
    class ArrayReader<T>
    {
        public T[] read(Object[] input)
        {
            T[] rs;
            if (input == null)
            {
                rs = new T[0];
                return rs;
            }
            int len = input.Length;
            rs = new T[len];
            for (int i = 0; i < len; i++)
                rs[i] = (T)input[i];
            return rs;
        }

        public T[][] read2(Object[][] input)
        {
            T[][] rs;
            if (input == null)
            {
                rs = new T[0][];
                return rs;
            }
            int len1 = input.Length;
            rs = new T[len1][];
            for (int i = 0; i < len1; i++)
            {
                Object[] vs = (Object[])input[i];
                int len2 = vs.Length;
                T[] ret = new T[len2];
                for (int j = 0; j < len2; j++)
                    ret[j] = (T)vs[j];
                rs[i] = ret;
            }
            return rs;
        }
    }
}
