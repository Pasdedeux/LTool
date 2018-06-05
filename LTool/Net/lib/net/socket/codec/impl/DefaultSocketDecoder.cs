using System;
using System.Collections.Generic;
using common.io;
using common.core;
using common.net.socket.session;
using common.core.codec;

namespace common.net.socket.codec
{
    /// <summary>
    /// 组包完成调用接口
    /// </summary>
    /// <param name="msg"></param>
    public delegate void OnPackageRev(Msg msg);

    public class DefaultSocketDecoder:DefaultDecoder,ISocketDecoder
    {
        /// <summary>
        /// 日志接口
        /// </summary>
		private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();

        public int DeCode(Session session, OnPackageRev handler)
        {
            try
            {
                IoBuffer buffer = session.ReceiveBuffer;
                IoBufferReader reader = new IoBufferReader(buffer);
                reader.readInt();
                int cmd = reader.readInt();
                Dictionary<object, object> param = (Dictionary<object, object>)reader.Read();
                Msg msg = new Msg(cmd);
                msg.ParamMap = param;
                handler(msg);
                return OK;
            }
            catch(Exception e)
            {
                logReport.OnWarningReport("decode not ok,protocal erro,ex:"+e.StackTrace);
                return NOT_OK;
            }
           
        }
        public int Decodable(Session session)
        {
            IoBuffer buffer = session.ReceiveBuffer;
            int len =0;
            int rsCode = buffer.PeekInt(ref len);
            if(rsCode!=0)
                return NO_DATA;
            int len2 = buffer.Length;
            if (len > len2 - 4)
                return NEED_DATA;
            return OK;
        }
        public bool IsDecodable(Session session)
        {
            return Decodable(session) == OK;
        }
        public int DoDecode(Session session, OnPackageRev handler)
        {
            int rs = Decodable(session);
            if (rs == OK)
                rs = DeCode(session,handler);
            else if (rs == NOT_OK)
            {
                if (logReport != null)
                    logReport.OnWarningReport("decode not ok,protocal erro");
            }
            else if (rs == NEED_DATA)
            {
                if (logReport != null)
                    logReport.OnDebugReport("decode not all,protocal need data");
            }
            else if(rs == NO_DATA)
            {
                if (logReport != null)
                    logReport.OnDebugReport("decode not all,protocal no data");
            }
            return rs;
        }
    }
}
