using System;
namespace common.net.exception
{
    class TimeOutException:Exception
    {
        public TimeOutException(String msg):base(msg)
        {
        }
    }
}
