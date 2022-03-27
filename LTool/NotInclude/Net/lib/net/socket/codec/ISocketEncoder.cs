using common.core;
using common.core.codec;
using common.net.socket.session;

namespace common.net.socket.codec
{
    public interface ISocketEncoder :IEncoder
    {
        void encode(Session session, Msg msg);
    }
}
