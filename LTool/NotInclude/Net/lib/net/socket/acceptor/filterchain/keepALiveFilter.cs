using System;
using common.core;
using common.net.socket.session;

namespace common.net.socket.acceptor.filterchain
{
    class KeepALiveFilter : Filter
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
        public KeepALiveFilter(){}
        public override void doFilter(DateTime runtime,Session session)
        {
            try
            {
                long keepAliveTimeLen = session.SessionConfig.KeepAliveTimeLen;
                if (runtime < session.LastWriteTime.AddSeconds(keepAliveTimeLen))
                    return;
                session.Write(session.HeartBeatPackage, null);
                logReport.OnLogReport("keepAlive->runtime:" + runtime + ",session.LastWriteTime:" + session.LastWriteTime + ",keepAliveTimeLen:" + keepAliveTimeLen);
            }
            catch(Exception e)
            {
                logReport.OnWarningReport("runtime:"+ runtime +"send heartBeatPkg fail,case by:" + e.Message);
            }
        }
    }
}