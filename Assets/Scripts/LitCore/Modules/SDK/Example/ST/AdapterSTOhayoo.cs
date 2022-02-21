/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：AdapterSTGA
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2022/1/3 15:22:26
* 更新时间 ：2022/1/3 15:22:26
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2022. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2022/1/3 15:22:26
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/


using Assets.Scripts.Controller;
#if OHAYOO
using ByteDance.Union;
using LitFramework.LitTool;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class AdapterSTOhayoo : BaseStatistician
    {
        public AdapterSTOhayoo(StatisticManager mng) : base(mng)
        {
#if OHAYOO
            LGSDKCore.Init(lgInitSuccessInfo =>
            {
                Log.D("deviceID = " + lgInitSuccessInfo.DeviceID + "--installID = " + lgInitSuccessInfo.InstallID + "--ssID = " + lgInitSuccessInfo.SsID + "--userUniqueID=" + lgInitSuccessInfo.UserUniqueID);

                //===========开启账号系统===========//
                //LGAbsRealName.GlobalRealNameFailCallback = delegate (int Errno, string ErrMsg)
                //{
                //    // 认证失败 返回错误信息
                //    var resultErrMsg = "RealNameVerified OnFail:" + Errno + ",errMsg:" + ErrMsg;
                //    LDebug.Log(resultErrMsg);

                //};
                //LGAbsRealName.GlobalRealNameSuccessCallback = delegate (bool isAdult)
                //{
                //    // 认证成功 返回用户是否成年
                //    var msg = "RealNameVerified onSuc isAdult:" + isAdult;
                //    LDebug.Log(msg);
                //    FrameworkController.Instance.StartLoadingLogo();
                //};
                //LGSDKDevKit.RealNameService.RealNameAuth();


                //===========不开启账号系统或者开启允许匿名登陆===========//
                LGSDKDevKit.RealNameService.CheckDeviceRealName(
                delegate (bool isRealNameVerified, bool isAdult) // 认证成功 返回用户是否成年
                {
                    var msg = "get device Verified onSuc isRealNameVerified:" + isRealNameVerified + "---isAdult:" +
                              isAdult;
                    LDebug.Log(msg);
                    FrameworkController.Instance.StartLoadingLogo();
                },
                delegate (int Errno, string ErrMsg) // 查询失败 返回错误信息
                {
                    var msg = "get device Verified info OnFail:" + Errno + ",errMsg:" + ErrMsg;
                    LDebug.Log(msg);
                }
                );
                //ToastManager.Instance.ShowToast("InitSuccess");

                // 全局防沉迷回调
                LGAbsRealName.GlobalAntiAddictionCallback = delegate (LGAntiAddictionResult result)
                {
                    var msg = $"TriggerAntiAddiction 是否已经自动弹窗：{result.AutoPopup}" +
                                   $"-- CanPlayTime : {result.CanPlayTime}";
                    Log.D(msg);
                    LDebug.Log(msg);
                    //ToastManager.Instance.ShowToast(msg);

                    // 若 autoPopup 为true, 说明应用启动的时候触发了防沉迷策略，已经自动弹窗提示，需要接入方关闭APP
                    //  若为false:说明在游戏的过程中触发了防沉迷策略，未自动弹窗提示，若用户正在游戏中，可等待该局游戏结束后进行关闭应用
                    if (result.AutoPopup)
                    {
                        LDebug.Log("防沉迷弹窗已自动弹出，游戏可关闭应用");
                        Application.Quit();
                    }
                };



            }, (errorCode, errorMessage) =>
            {
                var toastMsg = $"Init Fail errCode = {errorCode}, errMessage = {errorMessage}";
                Log.D(toastMsg);
                //ToastManager.Instance.ShowToast(toastMsg);
            });
#endif
        }

        public override void DOT(string key, string value = null, string tag = null)
        {

        }

    }
}