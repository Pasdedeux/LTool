using System;
using System.Collections.Generic;
using common.io;

namespace common.core.codec
{
    public class DefaultEncoder:IEncoder
    {
		private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();

        public byte[] encode(Msg msg)
        {
            IoBufferWriter wr = new IoBufferWriter();
            byte[] data = toByte(msg);
            int len = data.Length;
            if (logReport != null)
                logReport.OnDebugReport("encode msg len:" + len);
            wr.Buffer.WriteInt(len);
            wr.Buffer.WriteByts(data);
            return wr.ToBytes();
        }

        public byte[] toByte(Msg msg)
        {
            Dictionary<Object,Object> param = msg.ParamMap;
            IoBufferWriter wr = new IoBufferWriter();
            wr.Buffer.WriteInt(msg.Cmd);
            wr.Write(param);
            byte[] rs = wr.ToBytes();
            return rs;
        }
    }
}
