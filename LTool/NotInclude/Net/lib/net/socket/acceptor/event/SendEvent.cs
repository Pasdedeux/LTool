using System;
using common.core;
using common.net.netEvent;
using common.net.socket.session;
namespace common.net.socket.acceptor.netEvent
{
    class SendEvent : NetEvent
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        private Session session;
        private Msg msg;
        public SendEvent(Session session,Msg msg) : base()
        {
            this.session = session;
            this.msg = msg;
        }
        public override void Fire()
        {
            session.flushToNet(msg);
        }
    }
}
