using System.Collections.Generic;
using common.io;

namespace common.core.codec
{
    /// <summary>
    /// 组包完成调用接口
    /// </summary>
    /// <param name="msg"></param>
    public delegate void OnPackageRev(Msg msg);

    public class DefaultDecoder:IDecoder
    {
        public const int OK = 0;
        public const int NEED_DATA = 1;
        public const int NOT_OK = 2;
        public const int NO_DATA = 3;

        /// <summary>
        /// 日志接口
        /// </summary>
		private AbstractLogReport logReport = LoggerFactory.getInst().getLogger();
		public Msg DeCode(byte[] data)
		{
            IoBuffer buffer = new IoBuffer();
            buffer.Write(data, 0, data.Length);
            IoBufferReader reader = new IoBufferReader(buffer);
			reader.readInt();
			int cmd = reader.readInt();
			Dictionary<object, object> param = (Dictionary<object, object>)reader.Read();
			Msg msg = new Msg(cmd);
			msg.ParamMap = param;
			return msg;
		}
    }
}
