namespace common.net.socket.session
{
    public class SessionConfig
    {
        public long KeepAliveTimeLen { get; set; }
        public long SendTimeOut { get; set; }
        public SessionConfig(long keepAliveTimeLen, long sendTimeOut)
        {
            KeepAliveTimeLen = keepAliveTimeLen;
            if (KeepAliveTimeLen < SendTimeOut)
                KeepAliveTimeLen = sendTimeOut;
            SendTimeOut = sendTimeOut;
        }
    }
}
