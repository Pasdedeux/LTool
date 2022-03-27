/*======================================
* 项目名称 ：Assets.Scripts.AD
* 项目描述 ：
* 类 名 称 ：AdapterMopub
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.AD
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/7/4 11:40:55
* 更新时间 ：2019/7/4 11:40:55
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/7/4 11:40:55
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdapterMopub : BaseAdAdapter
{
#if MOPUB
#if UNITY_IOS
    private readonly string[] _bannerAdUnits = { "09ba8385ae2944a8927a65207a196309" };
    private readonly string[] _interstitialAdUnits = { "d73ebeb5e9e443fca3ffa1ce14d99b7b", "bd441753f1e449f883a45b159a0d6c6b" };
    private readonly string[] _rewardedVideoAdUnits = { "a46bdadc548e4b62b612708a1c979f45" };
#elif UNITY_ANDROID
    private readonly string[] _bannerAdUnits = { "7c79295443a547eea0ff798eb61dd134" };
    private readonly string[] _interstitialAdUnits = { "2734d2ffe3694983ade5a0144a9da874", "8564359e36da4b7ba19ccaca646da78a" };
    private readonly string[] _rewardedVideoAdUnits = { "a6c2dd5e67d549738a51e7e9b12c245b" };
#endif
    //private readonly string[] _rewardedRichMediaAdUnits = { "a96ae2ef41d44822af45c6328c4e1eb1" };
    //private readonly string[] _bannerAdUnits = { "b195f8dd8ded45fe847ad89ed1d016da" };
    //private readonly string[] _interstitialAdUnits = { "24534e1901884e398f1253216226017e" };
    //private readonly string[] _rewardedVideoAdUnits = { "920b6145fb1546cf8b5cf2ac34638bb7" };

#endif

    private bool _bannerRequest = false;
    public AdapterMopub( ADManager mng ) : base( mng ) { }

    public override void CreateBanner()
    {
#if MOPUB
        _bannerRequest = true;
        MoPub.RequestBanner(_bannerAdUnits[0], MoPub.AdPosition.BottomCenter);
#endif
    }

    public override void CreateInterstitial()
    {
#if MOPUB
        MoPub.RequestInterstitialAd( _interstitialAdUnits[ 0 ] );
        MoPub.RequestInterstitialAd( _interstitialAdUnits[ 1 ] );
#endif
    }

    public override void CreateRewarded()
    {
#if MOPUB
        MoPub.RequestRewardedVideo(_rewardedVideoAdUnits[0]);
#endif
    }

    public override void HideBanner(bool destroy)
    {
#if MOPUB
        if (!destroy)
            MoPub.ShowBanner(_bannerAdUnits[0], false);
        else
            MoPub.DestroyBanner(_bannerAdUnits[0]);
#endif
    }

    public override void HideInterstitial() { }

    public override void Init()
    {
#if MOPUB
        // NOTE: the MoPub SDK needs to be initialized on Start() to ensure all other objects have been enabled first.
        var anyAdUnitId = _bannerAdUnits[0];
        MoPub.InitializeSdk(new MoPub.SdkConfiguration
        {
            AdUnitId = anyAdUnitId,

            // Set desired log level here to override default level of MPLogLevelNone
            LogLevel = MoPub.LogLevel.Debug,
        });

        // register for initialized callback event in the app
        MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEventHandler;
#endif
    }

    private void OnSdkInitializedEventHandler(string e)
    {
        // The SDK is initialized here. Ready to make ad requests.
        //this.CreateBanner();
        this.CreateInterstitial();
        this.CreateRewarded();
#if MOPUB
        MoPubManager.OnSdkInitializedEvent -= OnSdkInitializedEventHandler;
#endif
    }

    public override bool IsIntersititialReady()
    {
#if MOPUB
#if UNITY_ANDROID
        if ( DataModel.Instance.CurrentLevel <= 20 )
            return MoPub.IsInterstitialReady( _interstitialAdUnits[ 1 ] );
        else
            return MoPub.IsInterstitialReady( _interstitialAdUnits[ 0 ] );
#elif UNITY_IOS
        if (DataModel.Instance.CurrentLevel <= 20)
            return MoPub.IsInterstitialReady(_interstitialAdUnits[1]);
        else
            return MoPub.IsInterstitialReady(_interstitialAdUnits[0]);
#endif
#else
        return false;
#endif
    }

    public override bool IsRewardedVideoAvailable()
    {
#if MOPUB
#if !UNITY_EDITOR
        return MoPub.HasRewardedVideo(_rewardedVideoAdUnits[0]);
#else
        return false;
#endif
#else
        return false;
#endif
    }

    public override void ShowBanner()
    {
#if MOPUB
        if (!_bannerRequest)
        {
            this.CreateBanner();
        }
        MoPub.ShowBanner(_bannerAdUnits[0], true);
#endif
    }

    public override void ShowInterstitial()
    {
#if MOPUB
        LDebug.Log("IsIntersititialReady " + IsIntersititialReady());

        if (IsIntersititialReady())
        {
#if UNITY_ANDROID
            if ( DataModel.Instance.CurrentLevel <= 20 )
            {
                MoPub.ShowInterstitialAd( _interstitialAdUnits[ 1 ] );
                LDebug.Log( "===>MoPub.ShowInterstitialAd _interstitialAdUnits[1]" );
            }
            else
            {
                MoPub.ShowInterstitialAd( _interstitialAdUnits[ 0 ] );
                LDebug.Log( "===>MoPub.ShowInterstitialAd _interstitialAdUnits[0]" );
            }
#elif UNITY_IOS
            if (DataModel.Instance.CurrentLevel <= 20)
            {
                MoPub.ShowInterstitialAd(_interstitialAdUnits[1]);
                LDebug.Log("===>MoPub.ShowInterstitialAd _interstitialAdUnits[1]");
            }
            else
            {
                MoPub.ShowInterstitialAd(_interstitialAdUnits[0]);
                LDebug.Log("===>MoPub.ShowInterstitialAd _interstitialAdUnits[0]");
            }
#endif
        }
        else InterstitialShowEventHandler?.Invoke(false);
#else
        InterstitialShowEventHandler?.Invoke(false);
#endif
    }

    public override void ShowRewarded()
    {
#if MOPUB
        if (IsRewardedVideoAvailable())
            MoPub.ShowRewardedVideo(_rewardedVideoAdUnits[0]);
        else if (RewardShowEventHandler != null)
            RewardShowEventHandler(false);
#else
        RewardShowEventHandler?.Invoke(false);
#endif
    }

    public override void SubDispose()
    {
#if MOPUB
        MoPub.DestroyBanner(_bannerAdUnits[0]);
#endif
    }

    protected override void InitBannerCallBack()
    {
#if MOPUB
        //加载完毕
        MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
        //加载失败
        MoPubManager.OnAdFailedEvent += OnAdFailedEvent;
        //单击banner
        MoPubManager.OnAdClickedEvent += OnAdClickedEvent;
        //Fired when a banner ad expands to encompass a greater portion of the screen
        MoPubManager.OnAdExpandedEvent += OnAdExpandedEvent;
        // Android only. Fired when a banner ad collapses back to its initial size
        MoPubManager.OnAdCollapsedEvent += OnAdCollapsedEvent;

        MoPub.LoadBannerPluginsForAdUnits(_bannerAdUnits);
#endif
    }

    protected override void InitInterstitialCallBack()
    {
#if MOPUB
        MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        //加载失败
        MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
        //插屏过期
        MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;
        //插屏关闭
        MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
#if UNITY_ANDROID || UNITY_IOS
        //激活的时候回调
        MoPubManager.OnInterstitialShownEvent += OnInterstitialShownEvent;
        //单击的时候回调
        MoPubManager.OnInterstitialClickedEvent += OnInterstitialClickedEvent;

        MoPub.LoadInterstitialPluginsForAdUnits(_interstitialAdUnits);
#endif
#endif
    }

    protected override void InitRewardedCallBack()
    {
#if MOPUB
        MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
        //加载失败
        MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
        //申请视频过期
        MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoExpiredEvent;
        //视频激活的时候回调
        MoPubManager.OnRewardedVideoShownEvent += OnRewardedVideoShownEvent;
        //视频单击回调
        MoPubManager.OnRewardedVideoClickedEvent += OnRewardedVideoClickedEvent;
        //视频播放失败
        MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
        //视频完整播放完回调
        MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
        //视频关闭回调
        MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        //视频广告跳转其他应用
        MoPubManager.OnRewardedVideoLeavingApplicationEvent += OnRewardedVideoLeavingApplicationEvent;

        MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedVideoAdUnits);
#endif
    }


#if MOPUB
    //banner读取成功
    private void OnAdLoadedEvent(string adUnitId, float height)
    {
        LDebug.Log("mopub调试——banner读取成功");
    }

    //banner读取失败
    private void OnAdFailedEvent(string adUnitId, string error)
    {
        LDebug.Log("mopub调试——banner读取失败, id:" + adUnitId);
    }

    //单击banner
    private void OnAdClickedEvent(string adUnitId)
    {
        LDebug.Log("mopub调试——单击banner, id:" + adUnitId);
    }

    //Fired when a banner ad expands to encompass a greater portion of the screen
    private void OnAdExpandedEvent(string adUnitId)
    {
        LDebug.Log("mopub调试——OnAdExpandedEvent, id:" + adUnitId);
    }

    //Fired when a banner ad collapses back to its initial size  
    private void OnAdCollapsedEvent(string adUnitId)
    {
        LDebug.Log("mopub调试——OnAdCollapsedEvent, id:" + adUnitId);
    }






    //interstitial读取成功
    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        LDebug.Log("mopub调试——interstitial读取成功");
        if (InterstitialLoadEventHandler != null) InterstitialLoadEventHandler(true);
    }

    //interstitial读取失败
    private void OnInterstitialFailedEvent(string adUnitId, string error)
    {
        if (error != null) LDebug.Log("OnInterstitialFailedEvent " + error);
        if (InterstitialLoadEventHandler != null) InterstitialLoadEventHandler(false);
        LDebug.Log("mopub调试——interstitial读取失败, id:" + adUnitId);
        LDebug.Log("error-----" + error);
    }
    //申请插屏过期
    private void OnInterstitialExpiredEvent(string adUnitId)
    {
        //重新申请
        //MoPub.RequestInterstitialAd(_interstitialAdUnits[0]);
        LDebug.Log("mopub调试——interstitial读取失败, id:" + adUnitId);
    }

    //interstitial关闭
    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        //MoPub.RequestInterstitialAd(_interstitialAdUnits[0]);
        LDebug.Log("mopub调试——interstitial关闭");
    }

    //插屏激活回调
    private void OnInterstitialShownEvent(string adUnitId)
    {
        LDebug.Log("mopub调试——interstitial插屏激活回调,id:" + adUnitId);
        if (InterstitialShowEventHandler != null) InterstitialShowEventHandler(true);
    }
    //单击的时候回调
    private void OnInterstitialClickedEvent(string adUnitId)
    {
        LDebug.Log("mopub调试——interstitial单击的时候回调,id:" + adUnitId);
        if (InterstitialShowEventHandler != null) InterstitialShowEventHandler(true);
    }







    //加载
    private void OnRewardedVideoLoadedEvent(string adUnitId)
    {
        LDebug.Log("激励视频广告读取,id:" + adUnitId);

    }
    //加载失败
    private void OnRewardedVideoFailedEvent(string adUnitId, string error)
    {
        //MoPub.RequestRewardedVideo(_rewardedVideoAdUnits[0]);
        LDebug.Log("激励视频广告加载失败,id:" + adUnitId);
        LDebug.Log("激励视频广告加载失败,error:" + error);
        RewardShowEventHandler?.Invoke(false);
    }

    //播放失败
    private void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
    {
        LDebug.Log("激励视频广告播放失败,id:" + adUnitId);
        LDebug.Log("激励视频广告播放失败,error:" + error);
    }

    //申请视频过期
    private void OnRewardedVideoExpiredEvent(string adUnitId)
    {
        //重新申请
        MoPub.RequestRewardedVideo(_rewardedVideoAdUnits[0]);
        LDebug.Log("激励视频申请视频过期,id:" + adUnitId);
    }
    //视频激活的时候回调
    private void OnRewardedVideoShownEvent(string adUnitId)
    {
        LDebug.Log("视频激活的时候回调,id:" + adUnitId);
    }
    //视频单击回调
    private void OnRewardedVideoClickedEvent(string adUnitId)
    {
        LDebug.Log("视频单击回调,id:" + adUnitId);
    }

    //视频完整播放完回调
    private void OnRewardedVideoReceivedRewardEvent(string adUnitId, string label, float amount)
    {
        LDebug.Log("视频完整播放完回调,id:" + adUnitId);
        LDebug.Log("视频完整播放完回调,label:" + label);
        LDebug.Log("视频完整播放完回调,amount:" + amount);
        RewardShowEventHandler?.Invoke(true);
    }

    //关闭激励视频回调
    private void OnRewardedVideoClosedEvent(string adUnitId)
    {
        LDebug.Log("关闭激励视频回调,id:" + adUnitId);
        MoPub.RequestRewardedVideo(_rewardedVideoAdUnits[0]);
        if (RewardCloseEventHandler != null) RewardCloseEventHandler(false);
    }

    //激励视频跳转其他应用
    private void OnRewardedVideoLeavingApplicationEvent(string adUnitId)
    {
        LDebug.Log("激励视频跳转其他应用,id:" + adUnitId);
    }
#endif

    public override bool IsSdkInitialized()
    {
#if MOPUB
        return MoPub.IsSdkInitialized;
#else
        return false;
#endif
    }

}
