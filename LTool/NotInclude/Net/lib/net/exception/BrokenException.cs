using System;
namespace common.net.exception
{
    class BrokenException : Exception
    {
        public BrokenException(String msg):base(msg)
        {
        }
    }
}
