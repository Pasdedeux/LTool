#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Assets.Scripts.Statistic
* 项目描述 ：
* 类 名 称 ：AdapterSTUMeng
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：Assets.Scripts.Statistic
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2020/9/12 16:14:32
* 更新时间 ：2020/9/12 16:14:32
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2020. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UM
using Umeng;
#endif

namespace Assets.Scripts.Statistic
{
    public class AdapterSTUMeng : BaseStatistician
    {
        private static string _appkey;

        public AdapterSTUMeng( StatisticManager mng ) : base( mng )
        {
#if UM
#if UNITY_ANDROID
            //导入app key 标识应用 (Android)  
            _appkey = "5f5c6059b4739632429e12a6";
#elif UNITY_IPHONE
         //导入app key 标识应用 (ios)  
        _appkey = "$$$$$$$$$$$$$$$$$$$$$$$";
#endif

            //设置Umeng Appkey   
            GA.StartWithAppKeyAndChannelId( _appkey, "Baidu" );

            //调试时开启日志 发布时设置为false  
            GA.SetLogEnabled( true );

            //触发统计事件 开始关卡         
            GA.StartLevel( "enter game test" );

            //var testInfoMac = GA.GetTestDeviceInfo(1);
            //Log.TraceInfo( "===>Umeng  "+ testInfoMac );
#endif
        }

        public override void DOT( string key, string value, string tag = null )
        {
            
        }
       
    }
}
