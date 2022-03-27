using System;
using System.Collections.Generic;

namespace common.core
{
    /// <summary>
    /// 命令回调
    /// </summary>
    /// <param name="msg"></param>
    public delegate void CmdCallback(Msg msg);
    public delegate void TimeOutProc(Msg msg, params Object[] args);
    /// <summary>
	/// 推送处理接口
	/// </summary>
	public delegate void OnMsgPush(Msg msg);
    /// <summary>
    /// 命令回调包装类
    /// </summary>
    public abstract class CmdCallbackWraper{
		private CmdCallback cmdCallback;
		public CmdCallback CmdCallback
        {
            get { return cmdCallback; }
            set { cmdCallback = value; }
        }
		public CmdCallbackWraper(CmdCallback cmdCallback)
		{
			this.cmdCallback = cmdCallback;
		}

		/// <summary>
		/// 调用方法
		/// </summary>
		/// <param name="msg">Message.</param>
		public void proc(Msg msg)
		{
			DoInWraper (msg);
            if(CmdCallback!=null)
			    CmdCallback (msg);
		}

		/// <summary>
		/// 包装类中需要处理的逻辑
		/// </summary>
		public abstract void DoInWraper(Msg msg);
        public abstract bool IsCallback();
	}

	public class CmdDelegate
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        /// <summary>
        /// 运行时
        /// </summary>
        public DateTime Runtime { get; set; }
        private Dictionary<int, CmdCallback> cmdCallbackMap = new Dictionary<int, CmdCallback>();
		private Dictionary<int,CmdCallbackWraper> cmdCallbackWraperMap = new Dictionary<int, CmdCallbackWraper>();
        private Dictionary<int, Msg> sendDataMap = new Dictionary<int, Msg>();
		private OnMsgPush onMsgPush;
		public OnMsgPush OnMsgPush
		{
			set{ onMsgPush = value;}

		}
        /// <summary>
        /// 超时检查处理
        /// </summary>
        /// <param name="sendTimeOut"></param>
        /// <param name="callback"></param>
        internal void SendTimeOut(long sendTimeOut, TimeOutProc timeOutProc,params Object[] args) 
        {
            Queue<Msg> timeOutMsgs = null;
            foreach (var v in sendDataMap.Values)
            {
                if (v == null)
                    continue;
                
                if (Runtime > v.SendTime.AddSeconds(sendTimeOut))
                {
                    logReport.OnWarningReport("msg is timeout,cmd:"+v.Cmd+",Runtime:"+Runtime+",sendTime:"+v.SendTime+",sendTimeOut:"+ sendTimeOut);
                    if (timeOutMsgs == null)
                        timeOutMsgs = new Queue<Msg>();
                    timeOutMsgs.Enqueue(v);
                    timeOutProc(v, args);
                }
            }
            if (timeOutMsgs == null || timeOutMsgs.Count == 0)
                return;
            foreach(var v in timeOutMsgs)
            {
                int cmd = v.Cmd;
                if (sendDataMap.ContainsKey(cmd))
                    sendDataMap.Remove(cmd);
            }
        }
        internal void AddSendDataMap(Msg msg)
        {
            int cmd = msg.Cmd;
            if (sendDataMap.ContainsKey(cmd))
                sendDataMap[cmd] = msg;
            else
                sendDataMap.Add(cmd, msg);
        }
        public void AddCmdCallbackMap(Msg msg, Object callback)
        {
            if (callback == null)
                return;
			if (callback is CmdCallback)
            {
                cmdCallbackMap[msg.Cmd] = callback as CmdCallback;
                AddSendDataMap(msg);
            }
			else if (callback is CmdCallbackWraper)
            {
                CmdCallbackWraper cmdCallbackWraper = callback as CmdCallbackWraper;
                if (cmdCallbackWraper.IsCallback())
                {
                    AddSendDataMap(msg);
                    cmdCallbackWraperMap[msg.Cmd] = callback as CmdCallbackWraper;
                } 
            }	
        }
        internal void OnMsgRev(Msg msg)
        {
            int cmd = msg.Cmd;
            sendDataMap.Remove(cmd);
        }
        internal void DispatchRealData(Msg msg)
        {
            int cmd = msg.Cmd;
            if (cmdCallbackMap.ContainsKey (cmd)) 
			{
				CmdCallback callback = cmdCallbackMap [cmd];
				if (callback != null)
					callback (msg);
				cmdCallbackMap.Remove (cmd);
			} 
			else if (cmdCallbackWraperMap.ContainsKey (cmd)) 
			{
				CmdCallbackWraper cmdCallbackWraper = cmdCallbackWraperMap[cmd];
				if (cmdCallbackWraper != null) 
				{
					cmdCallbackWraper.proc (msg);
					cmdCallbackWraperMap.Remove (cmd);
				}
			}
			else
            {
                if(onMsgPush!=null)
                    onMsgPush(msg);
            }
        }
    }
    
}
