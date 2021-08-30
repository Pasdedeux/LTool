using common.core;
using common.net.netEvent;
using common.net.socket.ioHandler;
using common.net.socket.session;
namespace common.net.socket.acceptor.netEvent
{
    /// <summary>
    /// 网络连接断开事件
    /// </summary>
    class BrokenEvent : NetEvent
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        private Acceptor acceptor;
        private Session session;
        public BrokenEvent(Acceptor acceptor,Session session) : base()
        {
            string stackTrace = Runtime.GetStackTrace();
            logReport.OnWarningReport(stackTrace);
            logReport.OnWarningReport("fire Broken,sessionid:"+session.Id);
            this.acceptor = acceptor;
            this.session = session;
        }
        public override void Fire()
        {
            acceptor.OnBroken(acceptor, session);
            IoHandler ioHandler = session.IoHandler;
            if(ioHandler!=null)
                ioHandler.Close(session);
            logReport.OnWarningReport("connect is broken,notic to acceptor,sessionid:"+ session.Id);
        }
    }
}