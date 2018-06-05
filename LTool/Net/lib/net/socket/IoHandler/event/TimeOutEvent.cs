using System;
using common.core;
using common.net.netEvent;
using common.net.socket.session;

namespace common.net.socket.ioHandler.netEvent
{
    public class TimeOutEvent : NetEvent
    {
        private Session session;
        private Msg msg;
        public TimeOutEvent(Session session,Msg msg)
        {
            this.session = session;
            this.msg = msg;
        }
        public override void Fire()
        {
            IoHandler ioHandler = session.IoHandler;
            if (ioHandler != null)
                ioHandler.OnTimeOut(session,msg);
        }
    }
}
