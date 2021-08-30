using common.net.netEvent;
using common.net.socket.session;
namespace common.net.socket.ioHandler.netEvent
{
    public class CloseEvent : NetEvent
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        private Session session;
        public CloseEvent(Session session) :base()
        {
            this.session = session;
        }
        public override void Fire()
        {
            IoHandler ioHandler = session.IoHandler;
            if (ioHandler!=null)
                ioHandler.Closed(session);
        }
    }
}
