using common.core.codec;
using common.net.socket.session;

namespace common.net.socket.codec
{
    public interface ISocketDecoder:IDecoder
    {
        int Decodable(Session session);
        bool IsDecodable(Session session);
        int DoDecode(Session session, OnPackageRev handler);
    }
}
