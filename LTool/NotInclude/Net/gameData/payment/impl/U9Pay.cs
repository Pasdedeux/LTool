//using IAPCustom;

using com.shengyan.models;

namespace common.gameData.payment.impl
{
    public class U9Pay : SDKHandler
    {
        public void DoPay(core.Msg msg, string orderid, int sdkCode, int productId, string sdkProuctId, int productNum, float price, OnSDKPayReturn onSDKPayReturn)
        {
            var productInfo = CacheController.Instance.GetShopConfigByNewItemId(productId);
            U9PayParams prm = new U9PayParams();
            prm.resultStatus = "SUCCESS";
            var orderID_extension = msg.GetParam<string>(GameSrvCodeMap.Param.PAY_SDK_ORDER_ID);
            UnityEngine.Debug.Log("GameSrvCodeMap.Param.PAY_SDK_ORDER_ID:" + orderID_extension);
            if (orderID_extension == null)
            {
                UnityEngine.Debug.LogError("orderID_extension == null!");
                return;
            }
            var res = new string[2];
            try
            {
                res = orderID_extension.Split(new char[] { '|' });
            }
            catch(System.Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return;
            }

            prm.orderId = res[0];
            prm.extension = res[1];
            UnityEngine.Debug.Log("prm.orderId:" + prm.orderId + "-------prm.extension:" + prm.extension + "-------orderID:" + orderid);
            prm.productID = productId.ToString();
            prm.productNum = productNum.ToString();
            prm.productPrice = ((int)price * 100).ToString();
            var cookie = common.net.cookie.CookieData.GetInstance().Load();
            prm.userID = cookie.Openid;
            prm.userName = cookie.Openid;
            prm.productName = productInfo.name;
            prm.produceDes = productInfo.description;
            U9SDKInterface.Instance.Pay(prm);

            var returnData = new OnSDKPayReturnData();
            returnData.prm = prm;
            returnData.SDKCode = sdkCode;
            returnData.Payrtn = onSDKPayReturn;
            returnData.orderID = orderid;

            var callBack = U9SDKCallback.InitCallback();
            callBack.payprm = returnData;
            /*UnityPurchaser.Instance.setCurrentBuyInfo( orderid, sdkCode, productId, sdkProuctId, productNum, onSDKPayReturn);
            UnityPurchaser.Instance.BuyProductID(sdkProuctId);*/

            TalkingDataController.OnChargeRequest(prm.orderId, productInfo.name, productInfo.Rmb, "CNY", productInfo.Rmb * 10, "Andorid渠道ID:" + TalkingDataController.GetChannelID());
        }
    }
}
public class OnSDKPayReturnData
{
    public U9PayParams prm;
    public int SDKCode;
    public string orderID;
    public common.gameData.payment.OnSDKPayReturn Payrtn;
}
