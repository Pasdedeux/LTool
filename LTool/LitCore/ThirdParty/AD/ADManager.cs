/*======================================
* 项目名称 ：LitFramework.ThirdParty.AD
* 项目描述 ：
* 类 名 称 ：ADManager
* 类 描 述 ：
* 命名空间 ：LitFramework.ThirdParty.AD
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/2/19 15:50:22
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：ADManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/5/21 14:36:44
* 更新时间 ：2019/5/21 14:36:44
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/5/21 14:36:44
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using Assets.Scripts;
using LitFramework;
using LitFramework.Base;
using LitFramework.InputSystemSp;
using System;
using UnityEngine;


public class ADManager : Singleton<ADManager>, IManager
{
    public Action<ADManager> InstallEventHandler;
    /// <summary>
    /// 是否开启切回前台插页广告
    /// </summary>
    public bool UseBackgroundAD
    {
        get { return PlayerPrefs.GetInt( "UseBackgroundAD", 1 ) == 1; }
        set { PlayerPrefs.SetInt( "UseBackgroundAD", value ? 1 : 0 ); }
    }
    /// <summary>
    /// 是否需要播放插页广告回调
    /// </summary>
    public Func<bool> CanShowInitialEvent;
    ////插页加载失败时重试次数
    //public const int INTERSTITIAL_LOAD_TRY_TIMES = 2;
    //插页广告播放结果
    public Action<bool> InterstitialPlayEvent, InterstitialLoadEvent;
    //激励视频播放结果
    public Action<bool> RewardPlayEventHandler;
    public Action<bool> RewardShutDownEventHandler;
    /// <summary>
    /// 当前使用中的广告适配器
    /// </summary>
    public BaseAdAdapter Adapter { get; internal set; }

    /// <summary>
    /// 是否启用广告
    /// </summary>
    public bool UseAds
    {
        get { return PlayerPrefs.GetInt( "UseAds", 1 ) == 1; }
        set { PlayerPrefs.SetInt( "UseAds", value ? 1 : 0 ); }
    }
    
    public bool IsAppLeave { get; private set; }

    private bool _isPlayReward;

    //启动项
    public void Install()
    {
        _isPlayReward = false;

        InstallEventHandler?.Invoke( this );
        InstallEventHandler = null;

        Adapter = Adapter ?? new BlankAdapter( this );
        AddDelegate();
    }

    public void Uninstall()
    {
        RemoveDelegate();

        InstallEventHandler = null;
        Adapter.Dispose();
        Adapter = null;
    }

    public void RemoveAds()
    {
        UseAds = false;
        Adapter.HideBanner( true );
    }

    #region Call Back

    private void AddDelegate()
    {
        Adapter.InterstitialShowEventHandler += InterstitialShowEventHandler;
        Adapter.InterstitialLoadEventHandler += InterstitialLoadEventHandler;
        Adapter.RewardShowEventHandler += RewardShowEventHandler;
        Adapter.RewardCloseEventHandler += RewardCloseEventHandler;
    }

    private void RemoveDelegate()
    {
        Adapter.InterstitialShowEventHandler -= InterstitialShowEventHandler;
        Adapter.InterstitialLoadEventHandler -= InterstitialLoadEventHandler;
        Adapter.RewardShowEventHandler -= RewardShowEventHandler;
        Adapter.RewardCloseEventHandler -= RewardCloseEventHandler;
    }

    #region 插页回调

    private void InterstitialShowEventHandler( bool succ )
    {
        IsAppLeave = false;
        InterstitialPlayEvent?.Invoke( succ );
        InterstitialPlayEvent = null;

        LDebug.Log( "Interstitial Show Status: " + succ + "! ");
    }

    private void InterstitialLoadEventHandler( bool succ )
    {
        InterstitialLoadEvent?.Invoke( succ );
        InterstitialLoadEvent = null;

        LDebug.Log( "Interstitial Load Status: " + succ + "! " );
    }

    #endregion

    #region 激励视频回调

    private void RewardShowEventHandler( bool succ )
    {
        _isPlayReward = false;

        IsAppLeave = false;
        RewardPlayEventHandler?.Invoke( succ );
        RewardPlayEventHandler = null;

        LDebug.Log( "Reward Show Status " + succ );
    }

    private void RewardCloseEventHandler( bool succ )
    {
        _isPlayReward = false;

        IsAppLeave = false;
        RewardShutDownEventHandler?.Invoke( succ );
        RewardShutDownEventHandler = null;

        LDebug.Log( "Reward Close Status " + succ );
    }

    #endregion

    #endregion




    //=====================Banner===================//
    public void ShowBanner()
    {
        if ( UseAds )
            Adapter.ShowBanner();
    }

    public void HideBanner( bool destroy = false, bool recreate = false )
    {
        Adapter.HideBanner( destroy );
        if ( recreate && UseAds ) Adapter.CreateBanner();
    }



    //=====================Interstitial===================//
    /// <summary>
    /// 满足 CanShowInitialEvent 回调判定条件，或者直接返回播放插页
    /// </summary>
    public void ShowFullScreen()
    {
        if ( CanShowInterstitial() )
        {
            IsAppLeave = true;
            Adapter.ShowInterstitial();
        }
        else
            InterstitialPlayEvent?.Invoke( true );
    }
    
    private bool CanShowInterstitial()
    {
        return CanShowInitialEvent == null ? UseAds : CanShowInitialEvent.Invoke();
    }
    
    //=====================Rewarded===================//
    public void ShowRewardedVedio()
    {
        _isPlayReward = true;
        IsAppLeave = true;
        Adapter.ShowRewarded();
    }


    private void OnApplicationPause( bool pause )
    {
        if ( Adapter != null ) Adapter.OnApplicationPause( pause );

        //程序从后台进入前台时检测是否需要弹出插页广告
        if ( UseBackgroundAD && !pause && !_isPlayReward && CanShowInterstitial() )
            ShowFullScreen();
    }
}

