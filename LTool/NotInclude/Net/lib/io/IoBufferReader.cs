using common;
using System;
using System.Collections.Generic;

namespace common.io
{
    internal class IoBufferReader
    {
        private IoBuffer buffer;
        public IoBufferReader(IoBuffer buffer)
        {
            this.buffer = buffer;
        }
        public IoBufferReader()
        {
            this.buffer = IoBuffer.Allocate();
        }
        public static IoBufferReader Wrap(byte[] src)
        {
            IoBufferReader reader = new IoBufferReader();
            reader.buffer = IoBuffer.Wrap(src);
            return reader;
        }
        public Object Read()
        {
            byte tag = buffer.Read();
            switch (tag)
            {
                case BufferType.NIL:
                    return null;
                case BufferType.MAP:
                    {
                        int len = buffer.ReadInt();
                        Dictionary<object, object> vs = new Dictionary<object, object>();
                        for (int i = 0; i < len; i++)
                            vs.Add(Read(), Read());
                        return vs;
                    }
                case BufferType.BYTE:
                    return buffer.Read();
                case BufferType.BOOLEAN:
                    byte v = buffer.Read();
                    return v == 0 ? false : true;
                case BufferType.SHORT:
                    return buffer.ReadShort();
                case BufferType.INT:
                    return buffer.ReadInt();
                case BufferType.FLOAT:
                    return buffer.ReadFloat();
                case BufferType.LONG:
                    return buffer.ReadLong();
                case BufferType.DATE:
                    return buffer.ReadDate();
                case BufferType.STRING:
                    return buffer.ReadString();
                case BufferType.BYTE_ARR:
                    return buffer.ReadByteArr();
                case BufferType.ARR:
                    {
                        int len = buffer.ReadInt();
                        object[] vs = new object[len];
                        for (int i = 0; i < len; i++)
                            vs[i] = Read();
                        return vs;
                    }
                case BufferType.LIST:
                    {
                        int len = buffer.ReadInt();
                        List<object> vs = new List<object>();
                        for (int i = 0; i < len; i++)
                            vs.Add(Read());
                        return vs;
                    }
                default:
                    UnityEngine.Debug.LogWarning("-----------------------unkowntype:"+tag);
                    throw new IoBufferException(IoBufferException.typeUnknown);
            }
        }
        public int readInt()
        {
            return buffer.ReadInt();
        }
        public short readShort()
        {
            return buffer.ReadShort();
        } 
    }

}
