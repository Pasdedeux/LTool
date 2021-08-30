/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：AdapterSTFacebook
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/6/3 15:35:55
* 更新时间 ：2019/6/3 15:35:55
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/6/3 15:35:55
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

#if FACEBOOK
using Facebook.Unity;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class AdapterSTFacebook : BaseStatistician
    {
        public AdapterSTFacebook( StatisticManager mng ) : base( mng )
        {
               InitFaceBook();
        }

        private void InitFaceBook()
        {
#if FACEBOOK && !UNITY_EDITOR
            FB.Init( this.OnInitComplete, this.OnHideUnity );
#endif
        }

        private void OnHideUnity( bool isUnityShown ) { }

        private void OnInitComplete()
        {
#if FACEBOOK && !UNITY_EDITOR
            FB.LimitAppEventUsage = true;
            FB.ActivateApp();

            //FB.LogInWithReadPermissions(new List<string> { "public_profile", "email" }, (result) =>
            //{

            //    FB.API("/"+ GameConfig.Instance.FaceBookID, HttpMethod.GET, e =>
            //    {
            //        Debug.Log(">>>>" + e.RawResult);
            //        var dict = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(e.RawResult);
            //        foreach (var item in e.ResultDictionary)
            //        {
            //            Debug.Log("===>" + item);
            //        }
            //    });
            //});



            //Debug.Log("AppID " + FB.AppId);
            Dictionary<string, object> tutParams = new Dictionary<string, object>();
            tutParams["parameter"] = FB.AppId;
            FB.LogAppEvent("FBAPPID", parameters: tutParams);
#endif
        }


        public override void DOT( string key, string value = null, string tag = null )
        {
#if FACEBOOK && !UNITY_EDITOR
            if (!FB.IsInitialized) return;
            if (value != null)
            {
                Dictionary<string, object> tutParams = new Dictionary<string, object>();
                tutParams["parameter"] = value;
                FB.LogAppEvent( key, parameters: tutParams);
            }
            else
                FB.LogAppEvent( key );
#endif
        }
    }
}
