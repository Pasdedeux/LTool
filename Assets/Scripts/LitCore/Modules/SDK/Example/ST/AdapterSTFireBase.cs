/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：AdapterSTFireBase
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/6/3 15:41:04
* 更新时间 ：2019/6/3 15:41:04
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/6/3 15:41:04
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdapterSTFireBase : BaseStatistician
{
    private bool _isInit = false;
    public AdapterSTFireBase(StatisticManager mng) : base(mng)
    {
#if FIREBASE
            InitFireBase();
#endif
    }

    public override void DOT(string key, string value = null, string tag = null)
    {
#if FIREBASE && !UNITY_EDITOR
             if ( _isInit )
            {
                if( key.Contains( "_WIN_TIME" ) )
                {
                    Firebase.Analytics.Parameter[] param = new Firebase.Analytics.Parameter[] { new Firebase.Analytics.Parameter( "time", value ) };
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( key, param );
                }
                else
                {
                    string result = key;
                    if ( value != null )
                    {
                        string[] names = key.Split( '_' );
                        List<string> ss = names.ToList();
                        ss.AddRange( value.Split( '_' ) );
                        names = ss.ToArray();
                        result = string.Join( "_", names );
                    }
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( result );
                }
            }
#endif
    }

    #region FireBase
#if FIREBASE
        private Firebase.FirebaseApp _fireBase;
#endif
    private void InitFireBase()
    {
        _isInit = false;
#if FIREBASE && !UNITY_EDITOR
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith( task =>
            {
                var dependencyStatus = task.Result;
                if ( dependencyStatus == Firebase.DependencyStatus.Available )
                {
                    // Create and hold a reference to your FirebaseApp, i.e.
                    //   app = Firebase.FirebaseApp.DefaultInstance;
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _fireBase = Firebase.FirebaseApp.DefaultInstance;

                    Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled( true );
                    // Set a flag here indicating that Firebase is ready to use by your
                    // application.
                    _isInit = true;
                }
                else
                {
                    UnityEngine.Debug.LogError( System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus ) );
                    // Firebase Unity SDK is not safe to use here.
                }
            } );
#endif
    }


    #endregion
}
