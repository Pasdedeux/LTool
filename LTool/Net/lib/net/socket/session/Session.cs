using common.io;
using System;
using common.core;
using common.net.socket.codec;
using System.Collections.Generic;
using common.net.socket.channel;
using common.net.socket.ioHandler;
using common.net.socket.ioHandler.netEvent;
using System.Threading;
namespace common.net.socket.session
{
    public class Session
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public const int STATE_CLOSED = 0;
        public const int STATE_CLOSING = 1;
        public const int STATE_OPEN = 2;
        private int state = STATE_OPEN;
        public int State { get { return state; } set { state = value; } }
        private static long seq = 0;
        private static byte[] seqLock = new byte[0];
        private static long getSeq()
        {
            long rs = 0;
            lock (seqLock)
            {
                seq++;
                rs = seq;
            }
            return rs;
        }
        private long id;
        public long Id { get { return id; } }
        private ISocketDecoder decoder;
        public ISocketDecoder Decoder { set { this.decoder = value; } }
        private ISocketEncoder encoder;
        public ISocketEncoder Encoder { set { this.encoder = value; } }
        public SessionConfig SessionConfig { get; set; }
        public Msg HeartBeatPackage { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastReadTime { get; set; }
        private IoBuffer mReceiveBuffer = new IoBuffer();
        public IoBuffer ReceiveBuffer { get { return mReceiveBuffer; } }
        private IoBuffer mSendBuffer = new IoBuffer();
        public IoBuffer SendBuffer { get { return mSendBuffer; } }
        private Channel channel;
        public Channel Channel { get { return channel; } }
        public IoHandler IoHandler { get; set; }
        public Queue<Msg> msgs = new Queue<Msg>();
        private byte[] msgsLock = new byte[0];
        internal CmdDelegate cmdDelegate = new CmdDelegate();
        private Thread exec;
        private byte[] runLock = new byte[0];
        private volatile bool runSwitch = true;
        public Session(Channel channel)
        {
            id = getSeq();
            decoder = new DefaultSocketDecoder();
            encoder = new DefaultSocketEncoder();
            this.channel = channel;
            exec = new Thread(Run);
        }
        public void Run(object param)
        {
            while (true)
            {
                try
                {
                    channel.Read(OnReadData);
                }
                catch(Exception e)
                {
                    logReport.OnWarningReport("session id:"+id+ " thrd loop exception: ex:" + e.Message + "\r\n" + e.StackTrace);
                    ExceptionCaught(e);
                    state = STATE_CLOSED;
                    break;
                }
            }
            logReport.OnWarningReport("session id:" + id + " thrd will exit.");
        }
        public void Start()
        {
            exec.Start(this);
        }
        public void Close()
        {
            logReport.OnWarningReport("stop session id:" + id + " thread ing ...");
            DisConnect();
            exec.Join();
            logReport.OnWarningReport("stop session id:" + id + " finised.");
        }
        public void OnReadData(DefaultBuffer buffer)
        {
            Read(buffer);
        }
        public void Read(DefaultBuffer buffer)
        {
            mReceiveBuffer.Write(buffer.Hb, 0, buffer.Len);
            do
            {
                decoder.DoDecode(this, OnPackageRev);
            }
            while (!mReceiveBuffer.IsEmpty() && decoder.IsDecodable(this));    
        }
        public void Write(Msg msg,Object callback)
        {
            LastWriteTime = DateTime.Now;
            msg.SendTime = LastWriteTime;
            if (msg == null)
            {
                logReport.OnWarningReport("msg is null");
                return;
            }
            if (IsOpen())
            {
                cmdDelegate.AddCmdCallbackMap(msg, callback);
                IoHandler.Write(this, msg);
                logReport.OnLogReport("sessionid"+id+"->send:" + msg.ToString() + ",time:" + LastWriteTime);
            }
            else
            {
                logReport.OnWarningReport("sessionid" + id + "->msg" + msg.ToString() + " send time out,net is broken");
                cmdDelegate.OnMsgRev(msg);
                if(IoHandler!=null)
                    IoHandler.Trigger(new TimeOutEvent(this, msg));
            }
        }
        public void flushToNet(Msg msg)
        {
            encoder.encode(this, msg);
            channel.Write(mSendBuffer, ExceptionCaught);
        }
        public void ExceptionCaught(Exception e)
        {
            if (IoHandler != null)
                IoHandler.ExceptionCaught(this,e);
        }
        private void OnPackageRev(Msg msg)
        {
            LastReadTime = DateTime.Now;
            logReport.OnWarningReport("sessionid:" + id + "->rev:msg" + msg.ToString() +",time:" + LastReadTime);
            cmdDelegate.OnMsgRev(msg);
            if (IoHandler != null)
                IoHandler.Trigger(new RevEvent(this,msg));
        }
        public void DisConnect()
        {
            state = STATE_CLOSING;
            channel.ForceDisconnect();

        }
        public bool IsOpen()
        {
            return state == STATE_OPEN ? true:false;
        }
    }
}