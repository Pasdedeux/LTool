using System;

namespace common.net.socket.exception
{
    class BrokenException : Exception
    {
        public BrokenException(String msg):base(msg)
        {
        }
    }
}
