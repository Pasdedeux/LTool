using System;
using System.Net;
using System.Net.Sockets;
using common.io;
using common.net.exception;
using common.net.socket.ioHandler;
using common.net.socket.session;
namespace common.net.socket.channel
{
	/// <summary>
    /// 连接通知代理
    /// </summary>
    public delegate void ConnectNotifyHandler(SocketError error, Session session);
    /// <summary>
    /// 从chanle中读取到数据时调用
    /// </summary>
    /// <param name="buffer"></param>
    public delegate void OnReadData(DefaultBuffer buffer);
    /// <summary>
    /// 网络异常捕获
    /// </summary>
    /// <param name="e"></param>
    public delegate void ExceptionCaught(Exception e);
    /// <summary>
    /// channel类,sockt底层连接通道
    /// </summary>
    public class Channel
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        internal const int readBufferSize = 1024;
        private byte[] readBuffer;
        private volatile bool writing = false;
        private TcpClient mTcp;
        private string ip;
        private int port;
        private byte[] wrlock = new byte[0];
        internal TcpClient ClientTcp { get { return mTcp; } }
        internal Channel(string ip,int port)
        {
            this.ip = ip;
            this.port = port;
            readBuffer = new byte[readBufferSize];
        }
        internal void BeginConnect(IoHandler ioHandler)
        {
            if (mTcp != null)
                ForceDisconnect();
            IPAddress[] address = Dns.GetHostAddresses(ip);
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                mTcp = new TcpClient(AddressFamily.InterNetworkV6);
            else
                mTcp = new TcpClient(AddressFamily.InterNetwork);
                IPAddress ipAddr = null;
            if (!IPAddress.TryParse(ip, out ipAddr))
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                if (entry.AddressList.Length > 0)
                    ipAddr = entry.AddressList[0];
            }
            if (ipAddr == null)
                throw new Exception("IP Address error.");
            Object[] args = new Object[2];
            args[0] = ioHandler;
            args[1] = mTcp;
            mTcp.BeginConnect(ipAddr, port, new AsyncCallback(ConnectCallback), args);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            Object[] args = (Object[])ar.AsyncState;
            IoHandler ioHandler = (IoHandler)args[0];
            TcpClient mTcp = (TcpClient)args[1];
            try
            {
                if (mTcp.Connected == true)
                {
                    mTcp.EndConnect(ar);
                    mTcp.Client.NoDelay = true;
                    mTcp.Client.LingerState = new LingerOption(true, 0);
                    Session session = new Session(this);
                    session.IoHandler = ioHandler;
                    ioHandler.ConnectReturn(session);
                }
                else
                    ioHandler.ConnectReturn(null);
            }
            catch (Exception e)
            {
                logReport.OnWarningReport("ConnectCallback fail:" + e.StackTrace);
                ioHandler.ConnectReturn(null);
            }
        }
        internal bool IsConnected()
        {
            return mTcp != null && mTcp.Connected;
        }
        internal bool ForceDisconnect()
        {
            if (IsConnected())
            {
                mTcp.GetStream().Close();
                mTcp.Close();
                mTcp.Client.Close();
                mTcp = null;
                return true;
            }
            else
                return true;
        }
        internal void Write(IoBuffer ioBuffer, ExceptionCaught exceptionCaught)
        {
            if (!IsConnected())
                throw new Exception("can't send data,socket connect is closed.");
            if (ioBuffer.Length == 0)
            {
                logReport.OnDebugReport("skip write to sockt,iobuffer is empty");
                return;
            }
            if (writing)
            {
                logReport.OnDebugReport("skip write to sockt in this loop");
                return;
            }
            lock(wrlock)
            {
                if (writing) 
                {
                    logReport.OnDebugReport("skip write to sockt in this loop");
                    return;
                }
                writing = true;
            }
            Socket socket = mTcp.Client;
            byte[] data = new byte[ioBuffer.Length];
            ioBuffer.Read(data, 0, data.Length);
            try
            {
                Object[] args = new Object[2];
                args[0] = socket;
                args[1] = exceptionCaught;
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), args);
            }
            finally
            {
                writing = false;
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            Object[] args = (Object[])ar.AsyncState;
            Socket socket = (Socket)args[0];
            ExceptionCaught caught = (ExceptionCaught)args[1];
            try
            {
                socket.EndSend(ar);
                logReport.OnLogReport("socket send msg to tcp stream ok");
            }
            catch(Exception e)
            {
                caught(e);
            }
            finally
            {
                writing = false;
            }
        }
        internal bool CanRead()
        {
            return mTcp != null && mTcp.Connected;
        }
        internal void Read(OnReadData callback)
        {
            if (!CanRead())
                throw new Exception("sockt can't read");
            Socket socket = mTcp.Client;
            try 
            {
                int readSize = socket.Receive(readBuffer, readBufferSize, SocketFlags.None);
                if (readSize == 0)//所有可用数据已被接收， Receive 方法将立即完成并返回零字节。
                    throw new BrokenException("channel is broken.");
                else
                    callback(new DefaultBuffer(readBuffer, readSize));
            }
            catch(Exception e)
            {
                throw new Exception("channel read fail,case by: " + e.Message);
            }
        }
    }
}