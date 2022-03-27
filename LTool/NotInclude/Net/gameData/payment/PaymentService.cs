using System;
using System.Collections.Generic;
using common.core;
using common.gameData.Item;

namespace common.gameData.payment
{
    public class PaymentCmdCallbackWraper : CmdCallbackWraper
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private PaymentService paymentService;
        private int sdkCode;
        private int productId;
        private string sdkProductId;
        private int productNum;
        private float price;
        private string currency;
        private SDKHandler sdkHandler;
        private string orderid;
        private ReqPayOrderReturn reqPayOrderReturn;
        public PaymentCmdCallbackWraper(PaymentService paymentService,int sdkCode, int productId,string sdkProductId, int productNum,float price,string currency, SDKHandler sdkHandler, ReqPayOrderReturn reqPayOrderReturn, CmdCallback cmdCallback) : base(cmdCallback)
        {
            this.paymentService = paymentService;
            this.sdkCode = sdkCode;
            this.productId = productId;
            this.sdkProductId = sdkProductId;
            this.productNum = productNum;
            this.price = price;
            this.currency = currency;
            this.sdkHandler = sdkHandler;
            this.reqPayOrderReturn = reqPayOrderReturn;
        }
        public override void DoInWraper(Msg msg)
        {
            int rsCode = (int)msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
            if (rsCode != BaseCodeMap.BaseRsCode.SUCCESS)
            {
                logReport.OnWarningReport("req pay fail:"+rsCode);
                if(reqPayOrderReturn!=null)
                    reqPayOrderReturn(false,rsCode);
                return;
            }    
            orderid = msg.GetParam(GameSrvCodeMap.Param.PAY_ORDER_ID) as string;
            logReport.OnLogReport("req pay success,orderid:"+orderid);
            if (reqPayOrderReturn != null)
                reqPayOrderReturn(true, BaseCodeMap.BaseRsCode.SUCCESS);
            paymentService.OnReqPayOrderReturn(msg,orderid, sdkCode, productId, sdkProductId, productNum,price,currency, sdkHandler);
       }

       public override bool IsCallback()
       {
            return true;
       }
    }
    public class OnNoticePaySuccessReturnWraper : CmdCallbackWraper
    {
        private int sdkCode;
        private OnNoticePaySuccessReturn onNoticePaySuccessReturn;
        public OnNoticePaySuccessReturnWraper(int sdkCode, OnNoticePaySuccessReturn onNoticePaySuccessReturn,CmdCallback cmdCallback) :base(cmdCallback)
        {
            this.sdkCode = sdkCode;
            this.onNoticePaySuccessReturn = onNoticePaySuccessReturn;
        }
        public override void DoInWraper(Msg msg)
        {
            onNoticePaySuccessReturn(sdkCode,msg);
        }

        public override bool IsCallback()
        {
            return true;
        }
    }
    public delegate void ReqPayOrderReturn(bool rs,int rsCode);
    public delegate void OnNoticePaySuccessReturn(int sdkCode, Msg msg);
    public class PaymentService
    {
        private UnityLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        public const int TIME_OUT = 10;
        private Orders orders;
        private NoticePayReturn payReturn;
        public NoticePayReturn PayReturn { set { payReturn = value; } }
        private BaseDao<Orders> baseDao = new BaseDao<Orders>(CodeMap.Filed.FIELED_PAYMENT);
        private static PaymentService instance=new PaymentService();
        public static PaymentService getInstance()
        {
            return instance;
        }
        private PaymentService(){}
        public void Start()
        {
            Load();
        }
        public void Update(DateTime runtime)
        {
            if (orders == null||!NetService.getInstance().IsLogined)
                return;
            List<Order> os = orders.GetTimeOutOrders(runtime, TIME_OUT);
            if (os == null || os.Count == 0)
                return;
            for(int i=0;i<os.Count;i++)
            {
                Order o = os[i];
                NoticePaySuccess(o.Orderid, o.SdkCode, o.Recipt,o.Currency);
            }
        }
        public void Load()
        {
            logReport.OnLogReport("start game,load orders ...");
            orders = baseDao.Load(typeof(Orders));
            if (orders == null)
            {
                orders = new Orders();
                baseDao.Save(orders);
            }
            logReport.OnLogReport("start game,load orders count:"+orders.Count());
        }
        public void ReqPay(int sdkCode,int productId,string sdkProductId,int productNum,float price,string currency, SDKHandler sdkHandler, ReqPayOrderReturn reqPayOrderReturn)
        {
            ReqPay(sdkCode, null, productId, sdkProductId, productNum, price, currency, sdkHandler, reqPayOrderReturn);
        }
        public void ReqPay(int sdkCode,string sdkUid,int productId, string sdkProductId, int productNum, float price, string currency, SDKHandler sdkHandler, ReqPayOrderReturn reqPayOrderReturn)
        {
            Msg reqOrder = new Msg(GameSrvCodeMap.Cmd.CMD_PAY_REQ);
            reqOrder.AddParam(GameSrvCodeMap.Param.PAY_PRODUCT_ID, productId);
            if(sdkUid!=null)
                reqOrder.AddParam(GameSrvCodeMap.Param.PAY_SDK_UID, sdkUid);
            reqOrder.AddParam(GameSrvCodeMap.Param.PAY_PRODUCT_NUM, productNum);
            reqOrder.AddParam(GameSrvCodeMap.Param.PAY_PRICE, price);
            reqOrder.AddParam(GameSrvCodeMap.Param.PAY_SDK_CODE, sdkCode);
            reqOrder.AddParam(GameSrvCodeMap.Param.PAY_CURRENCY, currency);
            logReport.OnLogReport("req order->sdkCode:" + sdkCode + ",productId:" + productId + ",sdkProductId:" + sdkProductId + ",productNum:" + productNum + ",currency:" + currency);
            NetService.getInstance().GameSrvClient.Send(reqOrder, new PaymentCmdCallbackWraper(this, sdkCode, productId, sdkProductId, productNum,price, currency, sdkHandler, reqPayOrderReturn, null));
        }
        public void OnReqPayOrderReturn(Msg msg,string orderid, int sdkCode, int productId,string sdkProductId, int productNum,float price,string currency, SDKHandler sdkHandler)
        {
            logReport.OnLogReport("reqPayOrderReturn,save oder,order id:"+ orderid);
            sdkHandler.DoPay(msg,orderid, sdkCode, productId, sdkProductId, productNum,price, OnSDKPayReturn);
        }
        public void OnSDKPayReturn(string orderid,int sdkCode, int productId, int productNum, string currency,PayRs payRs)
        {
            logReport.OnLogReport("OnSDKPayReturn,orderid:"+orderid+","+payRs.ToString());
            ProcPayRs(orderid, sdkCode,productId,productNum, currency, payRs);
        }
        private void AddOrder(int sdkCode,string orderid, int productid, int productNum,string currency,string recipt)
        {
            orders.AddOrder(new Order(sdkCode, orderid, productid, productNum,currency, recipt));
            baseDao.Save(orders);
        }
        public void ProcPayRs(string orderid, int sdkCode,int productId,int productNum,string currency, PayRs payRs)
        {
            if (payRs.IsSuccess)
            {
                logReport.OnLogReport("sdkPaySuccess,save order:" + orderid + " save to local");
                AddOrder(sdkCode, orderid, productId, productNum, currency, payRs.Recipt);
                logReport.OnLogReport("sdkPaySuccess,order:" + orderid + " notice to srv");
                NoticePaySuccess(orderid, sdkCode, payRs.Recipt, currency);
            }
            else
            {
                logReport.OnLogReport("sdkPayFail,delete order:"+orderid);
                DeletLocalOrderId(false, orderid);
                baseDao.Save(orders);
            }
        }
        private void NoticePaySuccess(string orderid, int sdkCode,string recipt,string currency)
        {
            Msg msg = new Msg(GameSrvCodeMap.Cmd.CMD_PAY_NOTICE_PAY_SUCCESS);
            msg.AddParam(GameSrvCodeMap.Param.PAY_ORDER_ID, orderid);
            msg.AddParam(GameSrvCodeMap.Param.PAY_SDK_CODE, sdkCode);
            msg.AddParam(GameSrvCodeMap.Param.PAY_RECEIPT, recipt);
            msg.AddParam(GameSrvCodeMap.Param.PAY_CURRENCY, currency);
            NetService.getInstance().GameSrvClient.Send(msg, new OnNoticePaySuccessReturnWraper(sdkCode,OnNoticePaySuccessReturn,null));
        }
        private void OnNoticePaySuccessReturn(int sdkCode,Msg msg)
        {
            int rsCode = (int)msg.GetParam(BaseCodeMap.BaseParam.RS_CODE);
            string orderid = msg.GetParam(GameSrvCodeMap.Param.PAY_ORDER_ID) as string;
            logReport.OnLogReport("pay rsCode:" + rsCode + ",orderid:" + orderid);
            Order order = orders.GetOrderById(orderid);
            int productid = order.Productid;
            if (rsCode == BaseCodeMap.BaseRsCode.SUCCESS||rsCode == GameSrvCodeMap.RsCode.ERRO_PAY_FINISHED)
            {
                DeletLocalOrderId(true, orderid);
                baseDao.Save(orders);
                Dictionary<int, int[]> updateItems = msg.GetParamDictionary2<int, int>(GameSrvCodeMap.Param.ITEM_UPDATES);
                Dictionary<int, ItemChangeObj> updates=null;
                string info = "";
                if(updateItems!=null&& updateItems.Count>0)
                {
                    updates = new Dictionary<int, ItemChangeObj>();
                    foreach (var e in updateItems)
                    {
                        int itemcid = e.Key;
                        int[] itemUpdates = e.Value;
                        int itemAddNum = itemUpdates[0];
                        int itemNum = itemUpdates[1];
                        updates.Add(itemcid, new ItemChangeObj(itemcid,itemAddNum, itemNum));
                        info = info + itemcid + ":" + itemAddNum + "|" + itemNum + ",";
                    }
                }
                logReport.OnLogReport("pay orderid"+ orderid + "addItem:" + info);
                if (payReturn != null)
                    payReturn.OnPaySuccess(sdkCode,orderid, productid, updates);
            }
            else 
            {
                if (rsCode != GameSrvCodeMap.RsCode.ERRO_PAY_TIME_OUT && rsCode != GameSrvCodeMap.RsCode.ERRO_PAY_ORDER_PORCESSING)
                {
                    DeletLocalOrderId(true, orderid);
                    baseDao.Save(orders);
                }  
                if (payReturn != null)
                    payReturn.NoticeFail(sdkCode,orderid, productid, rsCode);
            }
        }
        private bool DeletLocalOrderId(bool isPaySuccess,string orderid)
        {
            if (orderid == null)
                return false;
            return orders.RemoveOder(orderid);
        }
    }
    public interface NoticePayReturn
    {
        void OnPaySuccess(int sdkCode,string orderid, int productid, Dictionary<int, ItemChangeObj> updates);
        void NoticeFail(int sdkCode,string orderid, int productid, int rsCode);
    }
}
