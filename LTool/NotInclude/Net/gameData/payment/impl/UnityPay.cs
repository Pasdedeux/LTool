#if UNITY_IOS
using IAPCustom;
#endif
namespace common.gameData.payment.impl
{
    public class UnityPay : SDKHandler
    {
        public void DoPay(core.Msg msg, string orderid, int sdkCode, int productId, string sdkProuctId, int productNum, float price, OnSDKPayReturn onSDKPayReturn)
        {

#if UNITY_IOS
            UnityPurchaser.Instance.setCurrentBuyInfo(orderid, sdkCode, productId, sdkProuctId, productNum, onSDKPayReturn);
            UnityPurchaser.Instance.BuyProductID(sdkProuctId);
#endif
            var productInfo = com.shengyan.models.CacheController.Instance.GetShopConfigByNewItemId(productId);
            TalkingDataController.OnChargeRequest(orderid, productInfo.name, productInfo.Rmb, "CNY", productInfo.Rmb * 10, "ios支付");
        }
    }
}