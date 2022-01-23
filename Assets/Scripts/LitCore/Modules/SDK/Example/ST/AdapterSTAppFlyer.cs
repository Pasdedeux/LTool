/*======================================
* 项目名称 ：Assets.Scripts.Statistic
* 项目描述 ：
* 类 名 称 ：AdapterSTAppFlyer
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Statistic
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2020/9/11 16:21:53
* 更新时间 ：2020/9/11 16:21:53
* 版 本 号 ：v1.0.0.0
*******************************************************************
======================================*/

#if APPFLY
using AppsFlyerSDK;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Statistic
{
    public class AdapterSTAppFlyer : BaseStatistician
#if APPFLY
        , IAppsFlyerConversionData
#endif
    {
        public AdapterSTAppFlyer(StatisticManager mng) : base(mng)
        {
            InitAppFlyer();
        }

        private void InitAppFlyer()
        {
#if APPFLY
            LDebug.Log( ">>>AppsFlyer initSDK " );
            AppsFlyer.initSDK( "devkey", "appID" );
            LDebug.Log( ">>>AppsFlyer UNITY_ANDROID initSDK " );
            AppsFlyer.startSDK();
#endif
        }

        public override void DOT(string key, string value, string tag = null) { }

#if APPFLY
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFail", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
#endif
    }
}
