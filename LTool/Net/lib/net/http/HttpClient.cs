using common.io;
using System.Net;
using System.IO;
using System;
using common.core;
using common.core.codec;
using common.lib.core.collections.concurrent;
using common.net.netEvent;
namespace common.net.http
{
    public delegate void OnTimeOut(Msg msg);
    internal delegate void AsyncEventHandler(Msg msg);
	public class HttpClient
	{
		private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();

        private IDecoder decoder;
        public IDecoder Decoder
        {
            set { decoder = value; }
        }

        private IEncoder encoder;
        public IEncoder Encoder
        {
            set { encoder = value; }
        }

        private OnTimeOut timeOutProc;
        public OnTimeOut TimeOutProc
        {
            set { timeOutProc = value; }
        }
        CookieContainer myCookie = new CookieContainer();
		private string url;
	    
	    public HttpClient(string url)
	    {
	        this.url = url;
	    }
        public void Update()
        {
            Fire();
        }
        private CmdDelegate cmdDelegate = new CmdDelegate();

        /// <summary>
        /// 派遣消息队列锁
        /// </summary>
        private byte[] myDataPacketsLock = new byte[0];
        /// <summary>
        /// 消息派遣队列
        /// </summary>
        LinkedBlockingQueue<NetEvent> mDataPackets = new LinkedBlockingQueue<NetEvent>();
        public void RevMsg(Msg msg)
        {
            cmdDelegate.DispatchRealData(msg);
        }
        public void OnTimeOut(Msg msg)
        {
            timeOutProc(msg);
        }
        /// <summary>
        /// 消息事件派遣
        /// </summary>
        public void Fire()
        {
            //事件派遣
            while (mDataPackets.GetCount() > 0)
            {
                NetEvent msg = mDataPackets.Poll();
                if (msg != null)
                    msg.Fire();
            }
        }
        public void Send(Msg msg, object callback)
	    {
            if (callback != null)
			{
                DateTime now = DateTime.Now;
				msg.SendTime = now;
			}
	        //实例委托
	        AsyncEventHandler asy = new AsyncEventHandler(HttpAsyncEvent);
            cmdDelegate.AddCmdCallbackMap(msg, callback);
	        //异步调用开始，没有回调函数和AsyncState,都为null
	        asy.BeginInvoke(msg, null, asy);
	    }
	    public void HttpAsyncEvent(Msg msg)
	    {
            if (msg == null)
            {
                return;
            }
            byte[] data = encoder.encode(msg);
            HttpWebRequest myRequest = null;
			Stream stream = null;
            try
            {
                // 发送请求
                myRequest = (HttpWebRequest)WebRequest.Create(url);
				myRequest.Timeout = 30000;
				myRequest.KeepAlive = false;
                myRequest.CookieContainer = myCookie;
                myRequest.Method = "POST";
                myRequest.ContentLength = data.Length;
                stream = myRequest.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Flush();
                // 获得回复
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                Stream responseStream = myResponse.GetResponseStream();
                HttpStatusCode code = myResponse.StatusCode;
                if (code == HttpStatusCode.OK)
                {
                    byte[] temp = new byte[128];
                    int i = 0;
                    IoBuffer buffer = new IoBuffer();
                    do
                    {
                        i = responseStream.Read(temp, 0, 128);
                        if (i > 0)
                            buffer.Write(temp, 0, i);
                    } while (i > 0);
                    responseStream.Close();
                    int len = buffer.Length;
                    byte[] datas = new byte[len];
                    buffer.Read(datas, 0, len);

                    //单包模型
                    Msg revMsg = decoder.DeCode(datas);
                    if (logReport != null)
						logReport.OnLogReport("rev http rsp,cmd:" + revMsg.Cmd + " rsCode:" + revMsg.GetParam(BaseCodeMap.BaseParam.RS_CODE));
                    mDataPackets.Put(new RevEvent(this,revMsg));
                }
                else
                {
                    if(logReport != null)
                        logReport.OnWarningReport("http cmd:" + msg.Cmd + " fail,code:" + code);
                    msg.AddParam(BaseCodeMap.BaseParam.RS_CODE, BaseCodeMap.BaseRsCode.TIME_OUT);
                    mDataPackets.Put(new TimeOutEvent(this, msg));
                }

                return;
            }
            catch (Exception ex)
            {
                if (logReport != null)
                    logReport.OnWarningReport("http cmd:" + msg.Cmd + " ex:" + ex.ToString());
				mDataPackets.Put(new TimeOutEvent(this, msg));

            }
			finally 
			{
				if(stream!=null)
					stream.Close();
			}
	    }
    }
}
