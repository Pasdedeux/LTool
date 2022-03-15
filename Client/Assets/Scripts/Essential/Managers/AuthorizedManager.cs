/*======================================
* 项目名称 ：Assets.Scripts.Essential.SDK
* 项目描述 ：
* 类 名 称 ：AuthorizedManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Essential.SDK
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/9/26 11:16:38
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Android;

namespace Assets.Scripts.Essential.SDK
{
    /// <summary>
    /// 动态权限申请。用于启动及运行时权限申请功能
    /// 
    /// 集成项目插件，用于在Unity环境下进行权限申请
    /// </summary>
    public class AuthorizedManager : Singleton<AuthorizedManager>, IManager
    {
        //启动时需要申请检查的权限列表
        private string[] _permissionToRequestDict = {
              PERMISSION_INTERNET,
              PERMISSION_READ_PHONE_STATE,
              PERMISSION_WRITE_PHONE_STATE,
              PERMISSION_ACCESS_WIFI_STATE,
              PERMISSION_ACCESS_NETWORK_STATE,
              PERMISSION_GET_TASKS,
              PERMISSION_WAKE_LOCK,
              PERMISSION_READ_EXTERNAL_STORAGE,
              PERMISSION_WRITE_EXTERNAL_STORAGE,

              //特殊权限-视SDK要求而定
              //PERMISSION_SYSTEM_OVERLAY_WINDOW,
              //PERMISSION_WRITE_SETTINGS,
              //PERMISSION_BATTERY_STATS,
              //PERMISSION_MOUNT_UNMOUNT_FILESYSTEMS,
              //PERMISSION_ACCESS_COARSE_UPDATES,
              //PERMISSION_ACCESS_COARSE_LOCATION,
              //PERMISSION_ACCESS_FINE_LOCATION,
              //PERMISSION_CHANGE_WIFI_STATE,
              //PERMISSION_CHANGE_NETWORK_STATE,
              //PERMISSION_SYSTEM_ALERT_WINDOW,
              //PERMISSION_REQUEST_INSTALL_PACKAGES,
        };

        #region 可申请的权限列表

        public static string PERMISSION_INTERNET = "android.permission.INTERNET";
        public static string PERMISSION_READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
        public static string PERMISSION_WRITE_PHONE_STATE = "android.permission.WRITE_PHONE_STATE";
        public static string PERMISSION_ACCESS_WIFI_STATE = "android.permission.ACCESS_WIFI_STATE";
        public static string PERMISSION_ACCESS_NETWORK_STATE = "android.permission.ACCESS_NETWORK_STATE";
        public static string PERMISSION_GET_TASKS = "android.permission.GET_TASKS";
        public static string PERMISSION_WAKE_LOCK = "android.permission.WAKE_LOCK";
        public static string PERMISSION_SYSTEM_OVERLAY_WINDOW = "android.permission.SYSTEM_OVERLAY_WINDOW";
        public static string PERMISSION_WRITE_SETTINGS = "android.permission.WRITE_SETTINGS";
        public static string PERMISSION_BATTERY_STATS = "android.permission.BATTERY_STATS";
        public static string PERMISSION_MOUNT_UNMOUNT_FILESYSTEMS = "android.permission.MOUNT_UNMOUNT_FILESYSTEMS";
        public static string PERMISSION_ACCESS_COARSE_UPDATES = "android.permission.ACCESS_COARSE_UPDATES";
        public static string PERMISSION_ACCESS_COARSE_LOCATION = "android.permission.ACCESS_COARSE_LOCATION";
        public static string PERMISSION_ACCESS_FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
        public static string PERMISSION_CHANGE_WIFI_STATE = "android.permission.CHANGE_WIFI_STATE";
        public static string PERMISSION_CHANGE_NETWORK_STATE = "android.permission.CHANGE_NETWORK_STATE";
        public static string PERMISSION_SYSTEM_ALERT_WINDOW = "android.permission.SYSTEM_ALERT_WINDOW";
        public static string PERMISSION_READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
        public static string PERMISSION_WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
        public static string PERMISSION_REQUEST_INSTALL_PACKAGES = "android.permission.REQUEST_INSTALL_PACKAGES";

        #endregion

        public void Install()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var checkPermission = AndroidRuntimePermissions.CheckPermissions(_permissionToRequestDict);
            for (int i = 0; i < checkPermission.Length; i++)
            {
                Log.TraceInfo($">>{_permissionToRequestDict[i]}: {checkPermission[i]}" );
            }
            var toCheckListIndex = checkPermission.Select( ( a, k ) => new { value = a, index = k } ).Where( a => a.value != AndroidRuntimePermissions.Permission.Granted ).Select( k => k.index ).ToList();

            var toRequestArray = new string[ toCheckListIndex.Count ];
            for ( int i = 0; i < toCheckListIndex.Count; i++ ) toRequestArray[ i ] = _permissionToRequestDict[ toCheckListIndex[ i ] ];
            var ps = AndroidRuntimePermissions.RequestPermissions( toRequestArray );
            for (int i = 0; i < ps.Length; i++)
            {
                Log.TraceInfo($">>Re request {toRequestArray[ i ]} : {ps[i]}");
            }
#endif
        }


        public bool CheckPermission( string permission )
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidRuntimePermissions.CheckPermission( permission ) == AndroidRuntimePermissions.Permission.Granted;
#else
            return true;
#endif
        }


        public void CheckPermission( string permission, Action callBaccIFTrue )
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            switch ( AndroidRuntimePermissions.CheckPermission( permission ) )
            {
                case AndroidRuntimePermissions.Permission.Denied:
                    LDebug.LogFormat("{0} is NOT authorized! ", permission );
                    break;
                case AndroidRuntimePermissions.Permission.Granted:
                    callBaccIFTrue?.Invoke();
                    break;
                case AndroidRuntimePermissions.Permission.ShouldAsk:
                    RequestPermission( permission );
                    break;
            }
#else
            callBaccIFTrue?.Invoke();
#endif
        }


        public bool RequestPermission( string permission )
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidRuntimePermissions.RequestPermission( permission ) == AndroidRuntimePermissions.Permission.Granted;
#else
            return true;
#endif
        }

        public void Uninstall() { }


    }
}
