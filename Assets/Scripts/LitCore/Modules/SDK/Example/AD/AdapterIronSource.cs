/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：AdapterIronSource
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/5/21 14:59:02
* 更新时间 ：2019/5/21 14:59:02
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/5/21 14:59:02
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AdapterIronSource : BaseAdAdapter
{
    public AdapterIronSource( ADManager mng ) : base( mng ) { }

    private readonly string appKey = "d5cd57f5";

    public override void Init()
    {
#if IRONSOURCE
        IronSource.Agent.init( appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER );
        IronSource.Agent.shouldTrackNetworkState(true);
        IronSource.Agent.setConsent(true);
#if DEBUG
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setAdaptersDebug( true );
        LDebug.Log( IronSource.pluginVersion() );
        LDebug.Log( IronSource.unityVersion() );
#endif
#endif

        LitFramework.LitTool.LitTool.DelayPlayFunction(0.1f, () =>
       {
           //if (AdManager.Instance.UseAds)
           //    CreateBanner();
           CreateInterstitial();
           CreateRewarded();
       });
    }

    public override void SubDispose()
    {
#if IRONSOURCE
        IronSource.Agent.destroyBanner();
#endif
    }

    #region Banner

    public override void CreateBanner()
    {
#if IRONSOURCE
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        HideBanner(false);
        LDebug.Log(">>>>CreateBanner");
#endif
    }

    public override void ShowBanner()
    {
#if IRONSOURCE
        LDebug.Log(">>>>ShowBanner");
        IronSource.Agent.displayBanner();
#endif
    }

    public override void HideBanner(bool destroy)
    {
#if IRONSOURCE
        LDebug.Log(">>>>HideBanner");
        if (destroy) IronSource.Agent.destroyBanner();
        else IronSource.Agent.hideBanner();
#endif
    }

    protected override void InitBannerCallBack()
    {
#if IRONSOURCE
        //IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
#endif
    }


    #region BannerCallBack
#if IRONSOURCE
    //Invoked once the banner has loaded
    void BannerAdLoadedEvent()
    {
        LDebug.Log("banner OK");
        ShowBanner();
    }
    //Invoked when the banner loading process has failed.
    //@param description - string - contains information about the failure.
    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        LDebug.Log("banner Fail");
        CreateBanner();
    }
    // Invoked when end user clicks on the banner ad
    void BannerAdClickedEvent()
    {
        CreateBanner();
    }
    //Notifies the presentation of a full screen content following user click
    void BannerAdScreenPresentedEvent()
    {
        CreateBanner();
    }
    //Notifies the presented screen has been dismissed
    void BannerAdScreenDismissedEvent()
    {
        CreateBanner();
        IronSource.Agent.displayBanner();
    }
    //Invoked when the user leaves the app
    void BannerAdLeftApplicationEvent()
    {
    }
#endif
    #endregion

    #endregion

    #region Interstitial

    public override void CreateInterstitial()
    {
#if IRONSOURCE
        IronSource.Agent.loadInterstitial();
#endif
    }

    public override void ShowInterstitial()
    {
#if IRONSOURCE
        if (IsIntersititialReady())
            IronSource.Agent.showInterstitial();
        else
        {
            InterstitialShowEventHandler?.Invoke(false);
            CreateInterstitial();
        }
#endif
    }

    public override void HideInterstitial() { }

    protected override void InitInterstitialCallBack()
    {
#if IRONSOURCE
        //IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        //IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        //IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        //IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
#endif
    }

    public override bool IsIntersititialReady()
    {
#if IRONSOURCE
        return IronSource.Agent.isInterstitialReady();
#else
        return false;
#endif
    }


    #region InterstitialCallBack
#if IRONSOURCE
    //Invoked when the initialization process has failed.
    //@param description - string - contains information about the failure.
    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        LDebug.Log("==InterstitialAdLoadFailedEvent");
        if (error != null) LDebug.Log(error.getDescription());
        if (InterstitialLoadEventHandler != null) InterstitialLoadEventHandler(false);
    }
    //Invoked right before the Interstitial screen is about to open.
    void InterstitialAdShowSucceededEvent() { }

    //Invoked when the ad fails to show.
    //@param description - string - contains information about the failure.
    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        LDebug.Log("==InterstitialAdShowFailedEvent");
        if (InterstitialShowEventHandler != null) InterstitialShowEventHandler(false);
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent()
    {
    }
    //Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent()
    {
        LDebug.Log("==InterstitialAdClosedEvent");
        if (InterstitialShowEventHandler != null) InterstitialShowEventHandler(true);
        CreateInterstitial();
    }
    //Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent()
    {
    }
    //Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent()
    {
    }
#endif
    #endregion

    #endregion

    #region Reward

    public override void CreateRewarded() { }

    public override void ShowRewarded()
    {
#if IRONSOURCE
        LDebug.Log("==ShowRewardedVedio");

        if (IsRewardedVideoAvailable())
            IronSource.Agent.showRewardedVideo();
        else if (RewardShowEventHandler != null)
            RewardShowEventHandler(false);
#endif
    }

    protected override void InitRewardedCallBack()
    {
#if IRONSOURCE
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;

        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
#endif
    }

    #region RewardCallBack
#if IRONSOURCE
    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent()
    {
        LDebug.Log("RewardedVideoAdOpenedEvent");
    }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent()
    {
        LDebug.Log("RewardedVideoAdClosedEvent");
        if (RewardCloseEventHandler != null) RewardCloseEventHandler(false);
    }
    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        LDebug.Log("RewardedVideoAvailabilityChangedEvent");
        //Change the in-app 'Traffic Driver' state according to availability.
        //bool rewardedVideoAvailability = available;
    }
    //  Note: the events below are not available for all supported rewarded video 
    //   ad networks. Check which events are available per ad network you choose 
    //   to include in your build.
    //   We recommend only using events which register to ALL ad networks you 
    //   include in your build.
    //Invoked when the video ad starts playing.
    void RewardedVideoAdStartedEvent()
    {
        LDebug.Log("RewardedVideoAdStartedEvent");
    }
    //Invoked when the video ad finishes playing.
    void RewardedVideoAdEndedEvent()
    {
        UnityEngine.Debug.Log("RewardedVideoAdEndedEvent");
        //if ( RewardShowEventHandler != null ) RewardShowEventHandler( true );
    }
    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for the callback from the  ironSource server.
    //
    //@param - placement - placement object which contains the reward data
    //
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {
        UnityEngine.Debug.Log("RewardedVideoAdRewardedEvent");
        if (RewardShowEventHandler != null) RewardShowEventHandler(true);
    }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        LDebug.Log("RewardedVideoAdShowFailedEvent" + "  " + (error != null ? error.getDescription() : "error is null"));
        if (RewardShowEventHandler != null) RewardShowEventHandler(false);
    }

#endif

    public override bool IsRewardedVideoAvailable()
    {
#if IRONSOURCE
        LDebug.Log( ">>>IsRewardedVideoAvailable  " + IronSource.Agent.isRewardedVideoAvailable() );
        return IronSource.Agent.isRewardedVideoAvailable();
#else
        return false;
#endif

    }
    #endregion

    #endregion


    public override void OnApplicationPause(bool pause)
    {
        base.OnApplicationPause(pause);
#if IRONSOURCE
        IronSource.Agent.onApplicationPause(pause);
#endif
    }

    public override bool IsSdkInitialized()
    {
        return true;
    }
}
