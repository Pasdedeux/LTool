using System;

namespace common.gameData.payment.impl
{
    class TestDemoPay : SDKHandler
    {
        public void DoPay(core.Msg msg, string orderid, int sdkCode, int productId, string sdkProuctId, int productNum, float price, OnSDKPayReturn onSDKPayReturn)
        {
            onSDKPayReturn(orderid, sdkCode,productId,productNum, "CNY", new PayRs(true, "xxxxx"));
        }
    }
}
