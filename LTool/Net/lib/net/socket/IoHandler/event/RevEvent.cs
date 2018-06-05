using System;
using common.core;
using common.net.netEvent;
using common.net.socket.session;

namespace common.net.socket.ioHandler.netEvent
{
    public class RevEvent : NetEvent
    {
        private Session session;
        private Msg msg;
        public RevEvent(Session session,Msg msg)
        {
            this.session = session;
            this.msg = msg;
        }
        public override void Fire()
        {
            session.IoHandler.MsgRev(session,msg);
        }
    }
}
