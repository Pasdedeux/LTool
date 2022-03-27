using common.net.netEvent;
using common.net.socket.session;

namespace common.net.socket.ioHandler.netEvent
{
    public class ConnectSuccessEvent : NetEvent
    {
        private Session session;
        public ConnectSuccessEvent(Session session)
        {
            this.session = session;
        }
        public override void Fire()
        {
            session.IoHandler.OnConnect(session);
        }
    }
}
