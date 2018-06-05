using System;
using System.Collections;
using common;
using common.core;

namespace common.io
{
    class IoBufferWriter
    {
        private IoBuffer buffer;
        public IoBuffer Buffer
        {
            get { return this.buffer; }
        }

        public Msg LoginPackage { get; set; }

        public IoBufferWriter(IoBuffer buffer)
        {
            this.buffer = buffer;
        }
        public IoBufferWriter()
        {
            this.buffer = IoBuffer.Allocate();
        }
        public void Write(object o)
        {
            if (o == null)
                buffer.WriteByte(BufferType.NIL);
            else if (o is IDictionary)
            {
                IDictionary v = (IDictionary)o;
                int size = v.Count;
                buffer.WriteByte(BufferType.MAP);
                buffer.WriteInt(size);
                foreach(var r in v.Keys)
                {
                    Object value = v[r];
                    Write(r);
                    Write(value);
                }
            }
            else if (o is string)
                buffer.Write((String)o);
            else if (o is byte)
                buffer.Write((byte)o);
            else if (o is bool)
                buffer.Write((bool)o);
            else if (o is short)
                buffer.Write((short)o);
            else if (o is int)
                buffer.Write((int)o);
            else if (o is float)
                buffer.Write((float)o);
            else if (o is long)
                buffer.Write((long)o);
            else if (o is DateTime)
                buffer.Write((DateTime)o);
            else if (o is byte[])
                buffer.Write((byte[])o);
            else if (o is Array)
            {
                Array v = (Array)o;
                int size = v.Length;
                buffer.WriteByte(BufferType.ARR);
                buffer.WriteInt(size);
                foreach (Object r in v)
                    Write(r);
            }
            else if (o is IList)
            {
                IList v = (IList)o;
                int size = v.Count;
                buffer.WriteByte(BufferType.LIST);
                buffer.WriteInt(size);
                foreach (Object r in v)
                    Write(r);
            }
            else
                throw new IoBufferException(IoBufferException.typeUnknown);
        }
        public byte[] ToBytes()
        {
            return buffer.readAll();
        }
    }
}
