/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：AdapterSTGA
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/6/3 15:22:26
* 更新时间 ：2019/6/3 15:22:26
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/6/3 15:22:26
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

#if GA
using GameAnalyticsSDK;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class AdapterSTGA : BaseStatistician
    {
        public AdapterSTGA(StatisticManager mng) : base(mng)
        {
#if GA && !UNITY_EDITOR
            GameAnalytics.Initialize();
            GameAnalytics.SettingsGA.SubmitErrors = true;
#if DEBUG
            GameAnalytics.SettingsGA.InfoLogBuild = true;
#else
             GameAnalytics.SettingsGA.InfoLogBuild = false;
#endif
#endif
        }

        public override void DOT( string key, string value = null, string tag = null )
        {
#if GA && !UNITY_EDITOR
            string[] names = key.Split('_');
            string result;
            if (value != null)
            {
                List<string> ss = names.ToList();
                ss.AddRange(value.Split('_'));
                names = ss.ToArray();
            }
            if (tag != null) SetGADOTProgress(names);
            else
            {
                result = string.Join(":", names);
                GameAnalytics.NewDesignEvent(result);
            }
#endif
        }

        private object[] _gaProgressFormat = new object[ 5 ];
        private void SetGADOTProgress( string[] names )
        {
#if GA
            _gaProgressFormat[ 0 ] = null;
            _gaProgressFormat[ 1 ] = null;
            _gaProgressFormat[ 2 ] = null;
            _gaProgressFormat[ 3 ] = null;
            _gaProgressFormat[ 4 ] = null;

            //Status
            if ( names[ 2 ].Equals( "enter" ) )
                _gaProgressFormat[ 0 ] = GAProgressionStatus.Start; //装拆箱过程，尽量避免
            else if ( names[ 2 ].Equals( "win" ) )
            {
                _gaProgressFormat[ 0 ] = GAProgressionStatus.Complete; //装拆箱过程，尽量避免
                if ( names[ 0 ].Equals( "level" ) ) _gaProgressFormat[ 4 ] = names[ 3 ] ?? null;
            }
            else if ( names[ 2 ].Equals( "fail" ) )
                _gaProgressFormat[ 0 ] = GAProgressionStatus.Fail; //装拆箱过程，尽量避免
            else
                _gaProgressFormat[ 0 ] = GAProgressionStatus.Complete; //装拆箱过程，尽量避免

            //progress01
            _gaProgressFormat[ 1 ] = names[ 1 ];
            //progress02
            _gaProgressFormat[ 2 ] = names[ 2 ];

            //以LEVEL开头
            if ( names[ 0 ].Equals( "level" ) )
            {
                //progress03
                _gaProgressFormat[ 3 ] = string.Format( "progress" );
            }
            else if ( names[ 0 ].Equals( "ui" ) )
            {
                //progress03  
                _gaProgressFormat[ 3 ] = names.Length > 3 ? names[ 3 ] : names[ 2 ];
            }

            //Debug.LogWarning("<color=yellow>====ProgressEvent====</color>");
            //for (int i = 0; i < _gaProgressFormat.Length; i++)
            //{
            //    Debug.LogWarning(_gaProgressFormat[i]);
            //}

            //if (_gaProgressFormat[4] != null)
            //{
            //    Debug.Log(">>>>>>>>>>>>>>>>>>>>>>SetGADOT  _gaProgressFormat >>>>>>" + Convert.ToInt32(_gaProgressFormat[4]));
            //}
            if ( _gaProgressFormat[ 4 ] != null )
            {
                GameAnalytics.NewProgressionEvent(
                ( GAProgressionStatus )_gaProgressFormat[ 0 ],
                ( string )_gaProgressFormat[ 1 ],
                ( string )_gaProgressFormat[ 2 ],
                ( string )_gaProgressFormat[ 3 ],
                Convert.ToInt32( _gaProgressFormat[ 4 ] )
                );
            }
            else
            {
                GameAnalytics.NewProgressionEvent(
                ( GAProgressionStatus )_gaProgressFormat[ 0 ],
                ( string )_gaProgressFormat[ 1 ],
                ( string )_gaProgressFormat[ 2 ],
                ( string )_gaProgressFormat[ 3 ]
                );
            }
#endif
        }
    }
}