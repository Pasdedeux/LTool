using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common.io
{
    /// <summary>
    /// 接收buffer
    /// </summary>
    public class DefaultBuffer
    {
        private byte[] hb;
        public byte[] Hb { get { return hb; } }
        int len;
        public int Len { get { return len; } }
        public DefaultBuffer(byte[] hb,int len)
        {
            this.hb = hb;
            this.len = len;
        }
    }
}
