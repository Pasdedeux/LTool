using System;
using common.core;
using common.net.exception;
using common.net.socket.channel;
using common.net.socket.codec;
using common.net.socket.ioHandler;
using common.net.socket.ioHandler.netEvent;
using common.net.socket.session;
namespace common.net.socket
{
    public delegate void OnTimeOut(Msg msg);
    /// <summary>
    /// 连接处理器
    /// </summary>
    public interface ConnectedProcessor
    {
        /// <summary>
        /// 连接事件方法
        /// </summary>
        /// <param name="isConnected"></param>
        void OnConnected(Boolean isConnected,Client client);
        /// <summary>
        /// 断开连接时调用
        /// </summary>
        void OnOffline();
    }
    public class Client:IoHandler
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public  ConnectedProcessor ConnectedProcessor { get; set; }
        public CmdCallback OnLoginReturn { get; set; }
		private string ip;
		private int port;
		public OnMsgPush OnMsgPush { get; set; }
        public Msg LoginPackage{ get; set; }
        private bool isOpenIng = false;
        private bool isKickOff = false;
        public bool IsKickOff { get { return isKickOff; } }
        public void KickOff()
        {
            isKickOff = true;
        }
        public void RestKickOffFlag()
        {
            isKickOff = false;
        }
        private OnTimeOut procTimeOut;
        public OnTimeOut ProcTimeOut { get { return procTimeOut; }set { procTimeOut = value; } }
        public new void Update(DateTime runtime)
        {
            base.Update(runtime);
        }
        public Client(SessionConfig sessionConfig, ISocketEncoder encoder, ISocketDecoder decoder, Msg heartBeatPackage) : base(sessionConfig, encoder, decoder, heartBeatPackage)
        {
        }
        public bool Send(Msg msg, Object callback)
        {
            bool rs = true;
            try 
            {
                session.Write(msg, callback);
            }
            catch(TimeOutException e)
            {
                logReport.OnWarningReport(Runtime + ":" + e.Message);
                Trigger(new TimeOutEvent(session,msg));
                rs = false;
            }
            return rs;
        }
        public void SendLoginPackage()
        {
            if(LoginPackage!=null)
                Send(LoginPackage, OnLoginReturn);
        }
        public void Connect(String ip, int port, ConnectedProcessor connectedProcessor)
        { 
            this.ip = ip;
            this.port = port;
            ConnectedProcessor = connectedProcessor;
            DoConnect();
        }
        public void DoConnect()
        {
            isOpenIng = true;
            Channel channel = new Channel(ip, port);
            try 
            {
                channel.BeginConnect(this);
            }
            catch(Exception e)
            {
                Trigger(new ConnectFailEvent(this));
                logReport.OnWarningReport(Runtime+":"+"connect to srv fail,case by" + e.Message);
            }
        }
        public void Close()
        {
            if(session != null)
            {
                Session closeSession = session;
                session = null;
                closeSession.Close();
            }
        }
        public override void OnConnect(Session session)
        {
            isOpenIng = false;
            if (ConnectedProcessor != null)
                ConnectedProcessor.OnConnected(true,this);
            logReport.OnLogReport("session id:"+session.Id+",ip:"+ip+",port:"+port+" connect success.");
            session.cmdDelegate.OnMsgPush = OnMsgPush;
            SendLoginPackage();
        }
        public override void ConnectFail()
        {
            isOpenIng = false;
            if (ConnectedProcessor != null)
                ConnectedProcessor.OnConnected(false,this);
        }
        public override void MsgRev(Session session, Msg msg)
        {
            logReport.OnLogReport("rev msg:"+msg.Cmd);
            session.cmdDelegate.DispatchRealData(msg);
        }

        public override void OnTimeOut(Session session, Msg msg)
        {
            if(procTimeOut!=null)
                procTimeOut(msg); 
        }
        public override void Closed(Session session)
        {
            logReport.OnWarningReport("notiec client close,session id:" + session.Id);
            if (ConnectedProcessor != null)
            {
                if(IsOffline())
                    ConnectedProcessor.OnOffline();
            }
                
        }
    }
}