using System;
using System.Collections.Generic;
using common.util;
using Assets.lib.util;

namespace common.core
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Msg
    {
        public int Cmd { get; set; }
        public DateTime SendTime { get; set; }
        public Msg(int cmd)
        {
            Cmd = cmd;
        }

        private Dictionary<object, object> paramMap = new Dictionary<object, object>();


        public Dictionary<object, object> ParamMap
        {
            get{ return paramMap; }
            set{ paramMap = value; }
        }

        public void AddParam(object key, object value)
        {
            if (paramMap.ContainsKey(key))
                paramMap.Remove(key);
            paramMap.Add(key, value);
        }

        public object GetParam(object key)
        {
            if (paramMap.ContainsKey(key))
                return paramMap[key];
            return null;
        }

        public T GetParam<T>(object key)
        {
            if (GetParam(key) != null)
            {
                return (T)GetParam(key);
            }
            return default(T);
        }

        public List<T> GetParamList<T>(object key)
        {
            List<Object> input = (List<Object>)GetParam(key);
            ListReader<T> reader = new ListReader<T>();
            return reader.read(input);
        }

        public List<T[]> GetParamList2<T>(object key)
        {
            List<Object> input = (List<Object>)GetParam(key);
            ListReader<T> reader = new ListReader<T>();
            return reader.read2(input);
        }

        public Dictionary<K,V> GetParamDictionary<K, V>(object key) 
        {
            Dictionary<Object, Object> input = (Dictionary<Object,Object>)GetParam(key);
            DictionaryReader<K, V> reader = new DictionaryReader<K, V>();
            return reader.read(input);
        }

        public Dictionary<K, V[]> GetParamDictionary2<K, V>(object key)
        {
            Dictionary<Object, Object> input = (Dictionary<Object, Object>)GetParam(key);
            DictionaryReader<K, V> reader = new DictionaryReader<K, V>();
            return reader.read2(input);
        }

        public T[] GetParamArr<T>(object key) 
        {
            Object[] input = (Object[])GetParam(key);
            ArrayReader<T> reader = new ArrayReader<T>();
            return reader.read(input);
        }

        public T[][] GetParamArr2<T>(object key)
        {
            Object[][] input = (Object[][])GetParam(key);
            ArrayReader<T> reader = new ArrayReader<T>();
            return reader.read2(input);
        }

        override
        public string ToString()
        {
            string str = "{ cmd=" + Cmd + " paramMap: ";
            if(paramMap != null)
            {
            	foreach (var key in paramMap)
            	{
                	str += " " + key.Key + "=" + key.Value + ",";
            	}
            	str += "}";
            }
            return str;
        }
    }
}
