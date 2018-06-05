using System;
using System.Collections.Generic;
namespace common.gameData.payment
{
    class Order
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private const int FIELD_SDK_CODE = 0;
        private const int FIELD_ORDERID = 1;
        private const int FIELD_PRODUCT_ID = 2;
        private const int FIELD_PRODUCT_NUM = 3;
        private const int FIELD_RECIPT = 4;
        private const int FIELD_CURRENCY = 5;
        private const int FIELD_UPDATE_TIME = 6;
        private int sdkCode;
        public int SdkCode { get { return sdkCode; } }
        private string orderid;
        public string Orderid { get { return orderid; } }
        private int productid;
        public int Productid { get { return productid; } }
        private int productnum;
        public int Productnum { get { return productnum; } }
        private string recipt;
        public string Recipt { get { return recipt; } }
        private string currency;
        public string Currency { get { return currency; } }
        private int state;
        public int State { get { return state; } }
        private DateTime updateTime;
        public DateTime UpdateTime { get { return updateTime; } }
        private Dictionary<object, object> saveData = new Dictionary<object, object>();
        public Dictionary<object, object> SaveData { get { return saveData; } }
        public Order(int sdkCode,string orderid,int productid,int productnum,string currency, string recipt)
        {
            this.sdkCode = sdkCode;
            SaveData.Add(FIELD_SDK_CODE, sdkCode);
            this.orderid = orderid;
            SaveData.Add(FIELD_ORDERID, orderid);
            this.productid = productid;
            SaveData.Add(FIELD_PRODUCT_ID, productid);
            this.productnum = productnum;
            SaveData.Add(FIELD_PRODUCT_NUM, productnum);
            this.currency = currency;
            SaveData.Add(FIELD_CURRENCY, currency);
            updateTime = DateTime.Now;
            SaveData.Add(FIELD_UPDATE_TIME, updateTime);
            this.recipt = recipt;
            saveData[FIELD_RECIPT] = recipt;
            logReport.OnLogReport("crt order,orderid:"+ orderid + ",crttime:"+updateTime);
        }
        public Order(Dictionary<object,object> input)
        {
            sdkCode = (int)input[FIELD_SDK_CODE];
            saveData.Add(FIELD_SDK_CODE, sdkCode);
            orderid = (string)input[FIELD_ORDERID];
            saveData.Add(FIELD_ORDERID, orderid);
            productid = (int)input[FIELD_PRODUCT_ID];
            saveData.Add(FIELD_PRODUCT_ID, productid);
            productnum = (int)input[FIELD_PRODUCT_NUM];
            saveData.Add(FIELD_PRODUCT_NUM, productnum);
            currency = (string)input[FIELD_CURRENCY];
            saveData.Add(FIELD_CURRENCY, currency);
            recipt = (string)input[FIELD_RECIPT];
            saveData.Add(FIELD_RECIPT, recipt);
            updateTime = (DateTime)input[FIELD_UPDATE_TIME];
            saveData.Add(FIELD_UPDATE_TIME, updateTime);
            logReport.OnLogReport("read order,orderid:"+ orderid +",crttime:"+ updateTime);
        }
        public bool IsPaySuccessTimeOut(DateTime runtime ,int timeOut)
        {
            bool rs = false;
            if (updateTime.AddSeconds(timeOut) < runtime &&(lastCheckTime==null||( lastCheckTime!=null&&lastCheckTime.AddSeconds(timeOut)<runtime)))
            {
                logReport.OnLogReport("updateTime:" + updateTime + "/timeOut:" + timeOut + "/lastCheckTime:" + lastCheckTime + "/runtime:" + runtime);
                rs = true;
                lastCheckTime = runtime;
            }  
            return rs;
        }
        private DateTime lastCheckTime;
    }
}