using System.Collections.Generic;
using common.io;
namespace common.gameData
{
    public abstract class Po
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        protected Dictionary<object, object> saveData;
        public byte[] GetBinData()
        {
            IoBufferWriter wr = new IoBufferWriter();
            wr.Write(saveData);
            return wr.ToBytes();
        }
        public Po()
        {
            saveData = new Dictionary<object, object>();
            Init();
        }
        public Po(byte[] data) 
        {
            this.saveData = new Dictionary<object, object>();
            Init();
            if (data == null)
                return;
            IoBuffer buffer = new IoBuffer();
            buffer.Write(data, 0, data.Length);
            IoBufferReader rd = new IoBufferReader(buffer);
            Dictionary<object, object> saveData = (Dictionary<object, object>)rd.Read();
            if (saveData == null)
                return;
            ReadData(saveData);
        }
        public abstract void ReadData(Dictionary<object, object> saveData);
        public abstract void Init();
    }
}