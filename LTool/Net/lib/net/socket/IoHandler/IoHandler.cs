using System;
using System.Net.Sockets;
using common.core;
using common.net.netEvent;
using common.net.socket.acceptor;
using common.net.socket.acceptor.netEvent;
using common.net.socket.codec;
using common.net.socket.ioHandler.netEvent;
using common.net.socket.session;
namespace common.net.socket.ioHandler
{
    public abstract class IoHandler:NetEventProcessor
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public Acceptor Acceptor { get; set; }
        private SessionConfig sessionConfig;
        private ISocketEncoder encoder;
        private ISocketDecoder decoder;
        private DateTime runtime;
        private Msg heartBeatPackage;
        protected Session session;
        public Session Session { get { return session; } }
        public IoHandler(SessionConfig sessionConfig, ISocketEncoder encoder, ISocketDecoder decoder,Msg heartBeatPackage)
        {
            this.sessionConfig = sessionConfig;
            this.encoder = encoder;
            this.decoder = decoder;
            this.heartBeatPackage = heartBeatPackage;
        }
        
        public DateTime Runtime { get { return runtime; } }
        public abstract void OnConnect(Session session);
        public abstract void ConnectFail();
        public abstract void MsgRev(Session session,Msg msg);
        public abstract void OnTimeOut(Session session,Msg msg);
        public abstract void Closed(Session session);
        public void ExceptionCaught(Session session,Exception e)
        {
            if(e is ArgumentNullException)
                logReport.OnWarningReport("ArgumentNullException,case by" + e.Message);
            else if (e is ArgumentException)
                logReport.OnWarningReport("ArgumentException," + e.Message);
            else if (e is ObjectDisposedException)
                logReport.OnWarningReport("socket closed,ObjectDisposedException," + e.Message);
            else if (e is SocketException)
                logReport.OnWarningReport("SocketException,errcode:" + ((SocketException)e).ErrorCode);
            else
                logReport.OnWarningReport("Exception,case by" + e.Message);
            Acceptor.Trigger(new BrokenEvent(Acceptor,session));
        }
        public void Update(DateTime runtime)
        {
            this.runtime = runtime;
            Fire();
        }
        public void ConnectReturn(Session session)
        {
            Session oldSession = this.session;
            if (oldSession != null) 
            {
                Acceptor.replaceOldSession(oldSession.Id);
                oldSession.IoHandler = null;
            }
            if (session != null)
            {
                this.session = session;
                session.SessionConfig = sessionConfig;
                session.Encoder = encoder;
                session.Decoder = decoder;
                session.HeartBeatPackage = heartBeatPackage;
                Acceptor.OnConnected(Acceptor, session);
                Trigger(new ConnectSuccessEvent(session));
            }
            else
            {
                Trigger(new ConnectFailEvent(this));
            }
        }
        public void Close(Session session)
        {
            Trigger(new CloseEvent(session));
        }
        public void DoClose(Session session)
        {
            Acceptor.Trigger(new BrokenEvent(Acceptor,session));
        }
        public void Write(Session session,Msg msg)
        {
            Acceptor.Trigger(new SendEvent(session,msg));
        }
        public bool IsOpen()
        {
            return session !=null && session.State != Session.STATE_CLOSED;
        }
        public bool IsOffline()
        {
            return session != null && session.State == Session.STATE_CLOSED;
        }
    }
}