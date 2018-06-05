using System;
using common.core;
using common.net.socket.ioHandler;
using common.net.socket.ioHandler.netEvent;
using common.net.socket.session;
namespace common.net.socket.acceptor.filterchain
{
    class SendTimeOutFilter : Filter
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public SendTimeOutFilter(){}
        public override void doFilter(DateTime runtime,Session session)
        {
            long sendTimeOut = session.SessionConfig.SendTimeOut;
            session.cmdDelegate.Runtime = runtime;
            session.cmdDelegate.SendTimeOut(sendTimeOut, TimeOutProc, session);
            
        }
        private void TimeOutProc(Msg msg,params Object[] args)
        {
            Session session = args[0] as Session;
            IoHandler ioHandler = session.IoHandler;
            if (ioHandler != null)
                ioHandler.Trigger(new TimeOutEvent(session, msg));
            logReport.OnWarningReport("msg:" + msg.Cmd + " is timeout,runtime:" + session.cmdDelegate.Runtime);
        }
    }
}