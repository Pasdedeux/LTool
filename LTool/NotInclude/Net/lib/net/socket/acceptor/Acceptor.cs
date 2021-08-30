using System;
using System.Collections.Generic;
using System.Threading;
using common.net.socket.acceptor.filterchain;
using common.net.netEvent;
using common.net.socket.session;
namespace common.net.socket.acceptor
{
    /// <summary>
    /// 接收处理器，负责网络接入
    /// </summary>
    public class Acceptor:BlockingNetEventProcessor
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public Acceptor()
        {
            timeout = 1000;
            chain = BuilChain();
        }
        private DateTime runtime;
        private volatile bool isRunWorking = false;
        private volatile bool isCheckWorking = false;
        private Dictionary<long,Session> sessions = new Dictionary<long, Session>();
        private byte[] sessionsLock = new byte[0];
        private byte[] runLock = new byte[0];
        private volatile bool runSwitch = true;
        Filterchain chain;
        private volatile Queue<Session> removeQueue;
        Thread exec;
        Thread checker;
        public void Pause()
        {
            runSwitch = false;
        }
        public void Active()
        {
            runSwitch = true;
            SignalRun();
        }
        private void SignalRun()
        {
            lock(runLock)
            {
                logReport.OnLogReport("acceptor signal run");
                Monitor.PulseAll(runLock);
            }
        }
        private void RunWait()
        {
            lock (runLock)
            {
                logReport.OnLogReport("acceptor runWait");
                runSwitch = false;
                Monitor.Wait(runLock);
            }
        }
        public void Start()
        {
            exec = new Thread(Run);
            exec.Start(this);
            checker = new Thread(Check);
            checker.Start(this);
        }
        private Filterchain BuilChain()
        {
            chain = new Filterchain();
            List<Filter> filters = new List<Filter>();
            KeepALiveFilter keepAliveFilter = new KeepALiveFilter();
            SendTimeOutFilter sendTimeOutFilter = new SendTimeOutFilter();
            filters.Add(keepAliveFilter);
            filters.Add(sendTimeOutFilter);
            chain.setFilters(filters);
            chain.init();
            return chain;
        }
        public void Stop()
        {
            logReport.OnWarningReport("stop acceptor ing ...");
            foreach (var o in sessions.Values)
                o.Close();
            isRunWorking = false;
            if (exec.IsAlive)
                exec.Join();
            logReport.OnWarningReport("stop acceptor run finished ...");
            isCheckWorking = false;
            if(checker.IsAlive)
                checker.Join();
            logReport.OnWarningReport("stop acceptor finished ...");
        }
        public void Run(Object param)
        {
            isRunWorking = true;
            while (isRunWorking)
            {
                try
                {
                    if (!runSwitch)
                        RunWait();
                    Fire();
                }
                catch (Exception e)
                {
                    logReport.OnWarningReport("acceptor exec thrd loop exception:" + e.Message + "\r\n" + e.StackTrace);
                }
            } 
            logReport.OnWarningReport("acceptor run thrd will exit");
        }
        public void Check(Object param)
        {
            isCheckWorking = true;
            while (isCheckWorking)
            {
                try
                {
                    if (!runSwitch)
                        RunWait();
                    DateTime runtime = DateTime.Now;
                    removeClosedSession();
                    DoChain(runtime);
                    Thread.Sleep(1000);
                }
                catch(Exception e)
                {
                    logReport.OnWarningReport("acceptor check thrd loop exception:" + e.Message + "\r\n" + e.StackTrace);
                }
            }
            logReport.OnWarningReport("acceptor check thrd will exit");
        }
        public void replaceOldSession(long sessionid)
        {
            if (sessions.ContainsKey(sessionid))
            {
                Session session = sessions[sessionid];
                if (removeQueue == null)
                    removeQueue = new Queue<Session>();
                removeQueue.Enqueue(session);
            }
        }
        public void OnConnected(Acceptor acceptor, Session session)
        {
            runtime = DateTime.Now;
            lock (acceptor.sessionsLock)
            {
                session.State = Session.STATE_OPEN;
                session.LastReadTime = runtime;
                session.LastWriteTime = runtime;
                long id = session.Id;
                if (sessions.ContainsKey(id))
                    sessions[id] = session;
                else
                    sessions.Add(id,session);
            }
            session.Start();
        }
        public void OnBroken(Acceptor acceptor, Session session)
        {
            lock (acceptor.sessionsLock) 
            {
                long id = session.Id;
                if (acceptor.sessions.ContainsKey(id))
                {
                    if (acceptor.removeQueue == null)
                        acceptor.removeQueue = new Queue<Session>();
                    acceptor.removeQueue.Enqueue(session);
                } 
            }
            session.State = Session.STATE_CLOSED;
            session.Channel.ForceDisconnect();
        }
        internal void DoChain(DateTime runtime)
        {
            foreach (var o in sessions.Values)
            {
                if (o.Channel.IsConnected())
                    chain.doChain(runtime, o);
            }
        }
        private void removeClosedSession()
        {
            Queue<Session> removeQueue = null;
            lock (sessionsLock)
            {
                removeQueue = this.removeQueue;
                this.removeQueue = null;
            }
            if (removeQueue != null && removeQueue.Count > 0)
            {
                foreach (var o in removeQueue)
                    sessions.Remove(o.Id);
            }
        }
    }
}