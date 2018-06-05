using common.core;

namespace common.gameData.payment
{
    public delegate void OnSDKPayReturn(string orderid, int sdkCode, int productId, int productNum, string currency, PayRs payRs);
    public interface SDKHandler
    {
        void DoPay(Msg msg, string orderid, int sdkCode, int productId, string sdkProuctId, int productNum, float price, OnSDKPayReturn onSDKPayReturn);
    }
}