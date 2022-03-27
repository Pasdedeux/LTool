using common.core;
using common.net.netEvent;

namespace common.net.socket.ioHandler.netEvent
{
    public class ConnectFailEvent : NetEvent
    {
        private IoHandler ioHandler;
        public ConnectFailEvent(IoHandler ioHandler) :base()
        {
            this.ioHandler = ioHandler;
        }
        public override void Fire()
        {
            ioHandler.ConnectFail();
        }
    }
}
