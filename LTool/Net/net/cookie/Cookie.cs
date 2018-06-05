using common.gameData;
using System;
using System.Collections.Generic;
namespace common.net.cookie
{
    public class Cookie : Po
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private const int FIELED_UID = 0;
        private const int FIELED_TOKEN = 1;
        private const int FIELED_SRV_ID = 2;
        private const int FIELED_TOKEN_TIMESTAMP = 3;
        private const int FIELED_IS_BIND = 4;
        private const int FIELED_OPEN_ID = 5;
        private const int FIELED_SDK_CODE = 6;
        private string uid;
        public string Uid { get { return uid; } }
        private string token;
        public string Token { get { return token; } }
        private int srvid;
        public int Srvid { get { return srvid; } }
        private long tokenTimestamp;
        public long TokenTimestamp { get { return tokenTimestamp; } }
        private bool isBind;
        public bool IsBind { get { return isBind; } set { isBind = value; } }
        private string openid;
        public string Openid { get { return openid; } }
        private int sdkCode;
        public int SdkCode { get { return sdkCode; } }
        public Cookie() : base() { }
        public Cookie(byte[] data) : base(data) { }
        public Cookie(string uid,string token,int srvid,long tokenTimestamp,bool isBind,string openid,int sdkCode)
        {
            FillData(uid, token, srvid, tokenTimestamp, isBind,openid,sdkCode);
        }
        public override void Init()
        {
        }
        public override void ReadData(Dictionary<object, object> input)
        {
            logReport.OnLogReport("cookie data read ...");
            if (!input.ContainsKey(FIELED_UID) || input[FIELED_UID]==null|| "".Equals(input[FIELED_UID]))
                throw new Exception("cookie uid is null");
            string uid = (string)input[FIELED_UID];
            if(!input.ContainsKey(FIELED_TOKEN) || input[FIELED_TOKEN] == null || "".Equals(input[FIELED_TOKEN]))
                throw new Exception("cookie token is null");
            string token = (string)input[FIELED_TOKEN];
            if (!input.ContainsKey(FIELED_SRV_ID) || input[FIELED_SRV_ID] == null || (int)input[FIELED_SRV_ID]==0)
                throw new Exception("cookie srvid is null");
            int srvid = (int)input[FIELED_SRV_ID];
            if (!input.ContainsKey(FIELED_TOKEN_TIMESTAMP) || input[FIELED_TOKEN_TIMESTAMP] == null || (long)input[FIELED_TOKEN_TIMESTAMP]==0L)
                throw new Exception("cookie tokenTimestamp is null");
            long tokenTimestamp = (long)input[FIELED_TOKEN_TIMESTAMP];
            if (!input.ContainsKey(FIELED_IS_BIND) || input[FIELED_IS_BIND] == null)
                throw new Exception("cookie isBind is null");
            bool isBind = (bool)input[FIELED_IS_BIND];
            if (!input.ContainsKey(FIELED_OPEN_ID) || input[FIELED_OPEN_ID] == null || "".Equals(input[FIELED_OPEN_ID]))
                throw new Exception("cookie openid is null");
            string openid = (string)input[FIELED_OPEN_ID];
            if (!input.ContainsKey(FIELED_SDK_CODE) || input[FIELED_SDK_CODE] == null || (int)input[FIELED_SDK_CODE]==0)
                throw new Exception("cookie sdkCode is null");
            int sdkCode = (int)input[FIELED_SDK_CODE];
            FillData(uid, token, srvid, tokenTimestamp, isBind,openid,sdkCode);
        }
        private void FillData(string uid, string token, int srvid, long tokenTimestamp, bool isBind,string openid,int sdkCode)
        {
            this.uid = uid;
            if (saveData.ContainsKey(FIELED_UID))
                saveData[FIELED_UID] = uid;
            else
                saveData.Add(FIELED_UID, uid);
            this.token = token;
            if (saveData.ContainsKey(FIELED_TOKEN))
                saveData[FIELED_TOKEN] = token;
            else
                saveData.Add(FIELED_TOKEN, token);
            this.srvid = srvid;
            if (saveData.ContainsKey(FIELED_SRV_ID))
                saveData[FIELED_SRV_ID] = srvid;
            else
                saveData.Add(FIELED_SRV_ID, srvid);
            this.tokenTimestamp = tokenTimestamp;
            if (saveData.ContainsKey(FIELED_TOKEN_TIMESTAMP))
                saveData[FIELED_TOKEN_TIMESTAMP] = tokenTimestamp;
            else
                saveData.Add(FIELED_TOKEN_TIMESTAMP, tokenTimestamp);
            this.isBind = isBind;
            if (saveData.ContainsKey(FIELED_IS_BIND))
                saveData[FIELED_IS_BIND] = isBind;
            else
                saveData.Add(FIELED_IS_BIND, isBind);
            this.openid = openid;
            if (saveData.ContainsKey(FIELED_OPEN_ID))
                saveData[FIELED_OPEN_ID] = openid;
            else
                saveData.Add(FIELED_OPEN_ID, openid);
            this.sdkCode = sdkCode;
            if (saveData.ContainsKey(FIELED_SDK_CODE))
                saveData[FIELED_SDK_CODE] = sdkCode;
            else
                saveData.Add(FIELED_SDK_CODE, sdkCode);
        }
    }
}