using common.io;
using common.core;
using common.core.codec;
using common.net.socket.session;

namespace common.net.socket.codec
{
    public class DefaultSocketEncoder : DefaultEncoder,ISocketEncoder
    {
		private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public void encode(Session session,Msg msg)
        {
            IoBuffer buffer = session.SendBuffer;
            byte[] data = toByte(msg);
            int len = data.Length;
            logReport.OnLogReport("encode msg len:" + len);
            buffer.WriteInt(len);
            buffer.WriteByts(data);
        }
    }
}
