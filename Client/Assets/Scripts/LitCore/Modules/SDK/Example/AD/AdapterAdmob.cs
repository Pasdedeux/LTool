/*======================================
* 项目名称 ：Assets.Scripts.LitCore.Modules.SDK.Example.AD
* 项目描述 ：
* 类 名 称 ：AdapterAdmob
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.LitCore.Modules.SDK.Example.AD
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2020/1/23 13:25:10
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2020. All rights reserved.
*******************************************************************
======================================*/
#if ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AdapterAdmob
    #if ADMOB
    : BaseAdAdapter
#endif
{

    public AdapterAdmob(ADManager mng)
        #if ADMOB
       : base(mng) 
#endif
        { }

#region Android Test

//开屏广告 ca-app-pub-3940256099942544/3419835294
//横幅广告 ca-app-pub-3940256099942544/6300978111
//插页式广告 ca-app-pub-3940256099942544/1033173712
//插页式视频广告 ca-app-pub-3940256099942544/8691691433
//激励广告 ca-app-pub-3940256099942544/5224354917
//插页式激励广告 ca-app-pub-3940256099942544/5354046379
//原生高级广告 ca-app-pub-3940256099942544/2247696110
//原生高级视频广告 ca-app-pub-3940256099942544/1044960115

#endregion

#region IOS Test

//广告格式 演示广告单元 ID
//开屏广告    ca-app-pub-3940256099942544/5662855259
//横幅广告 ca-app-pub-3940256099942544/2934735716
//插页式广告 ca-app-pub-3940256099942544/4411468910
//插页式视频广告 ca-app-pub-3940256099942544/5135589807
//激励广告 ca-app-pub-3940256099942544/1712485313
//插页式激励广告 ca-app-pub-3940256099942544/6978759866
//原生高级广告 ca-app-pub-3940256099942544/3986624511
//原生高级视频广告 ca-app-pub-3940256099942544/2521693316

#endregion
#if ADMOB
    private bool isInited = false;
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    public override void CreateBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Clean up banner before reusing
        HideBanner(true);

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public override void CreateInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial before using it
        HideInterstitial();

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleInsOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleInsOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleInsOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleInsOnAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public override void CreateRewarded()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif
        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public override void HideBanner(bool destroy)
    {
        this.bannerView?.Hide();
        if (destroy) this.bannerView?.Destroy();
    }

    public override void HideInterstitial()
    {
        this.interstitial?.Destroy();
    }

    public override void Init()
    {
        List<string> deviceIds = new List<string>();
        //登记测试设备，ID运行时从输出窗口获得 
        //I / Ads: Use
        //   RequestConfiguration.Builder.setTestDeviceIds(Arrays.asList("33BE2250B43518CCDA7DE426D04EE231"))
        //to get test ads on this device.
        deviceIds.Add("2077ef9a63d2b398840261c8221a0c9b");
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetSameAppKeyEnabled(true)
             .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus =>
        {
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // main thread.
            // we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Log.TraceInfo("Initialization complete");
                isInited = true;

                CreateBanner();
                CreateInterstitial();
                CreateRewarded();
            });
        });
    }

    public override bool IsIntersititialReady()
    {
        return this.interstitial.IsLoaded();
    }

    public override bool IsRewardedVideoAvailable()
    {
        return this.rewardedAd.IsLoaded();
    }

    public override bool IsSdkInitialized()
    {
        return isInited;
    }

    public override void ShowBanner()
    {
        this.bannerView.Show();
    }

    public override void ShowInterstitial()
    {
        if (IsIntersititialReady())
            this.interstitial.Show();
    }

    public override void ShowRewarded()
    {
        if (IsRewardedVideoAvailable())
            this.rewardedAd.Show();
    }

    public override void SubDispose()
    {
        this.bannerView?.Destroy();
        this.interstitial?.Destroy();
        this.rewardedAd?.Destroy();
    }

    protected override void InitBannerCallBack()
    {

    }

    protected override void InitInterstitialCallBack()
    {

    }

    protected override void InitRewardedCallBack()
    {
    }


#region Banner/Ins Callback
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Log.TraceInfo("HandleFailedToReceiveAd event received with message: "
                            + args.LoadAdError.GetMessage());


        LoadAdError loadAdError = args.LoadAdError;

        // Gets the domain from which the error came.
        string domain = loadAdError.GetDomain();

        // Gets the error code. See
        // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
        // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
        // for a list of possible codes.
        int code = loadAdError.GetCode();

        // Gets an error message.
        // For example "Account not approved yet". See
        // https://support.google.com/admob/answer/9905175 for explanations of
        // common errors.
        string message = loadAdError.GetMessage();

        // Gets the cause of the error, if available.
        AdError underlyingError = loadAdError.GetCause();

        // All of this information is available via the error's toString() method.
        Log.TraceInfo("Load error string: " + loadAdError.ToString());

        // Get response information, which may include results of mediation requests.
        ResponseInfo responseInfo = loadAdError.GetResponseInfo();
        Log.TraceInfo("Response info: " + responseInfo.ToString());
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleAdClosed event received");
    }

#endregion

#region Ins Callback
    public void HandleInsOnAdLoaded(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleAdLoaded event received");
    }

    public void HandleInsOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Log.TraceInfo("HandleFailedToReceiveAd event received with message: "
                            + args.LoadAdError.GetMessage());


        LoadAdError loadAdError = args.LoadAdError;

        // Gets the domain from which the error came.
        string domain = loadAdError.GetDomain();

        // Gets the error code. See
        // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
        // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
        // for a list of possible codes.
        int code = loadAdError.GetCode();

        // Gets an error message.
        // For example "Account not approved yet". See
        // https://support.google.com/admob/answer/9905175 for explanations of
        // common errors.
        string message = loadAdError.GetMessage();

        // Gets the cause of the error, if available.
        AdError underlyingError = loadAdError.GetCause();

        // All of this information is available via the error's toString() method.
        Log.TraceInfo("Load error string: " + loadAdError.ToString());

        // Get response information, which may include results of mediation requests.
        ResponseInfo responseInfo = loadAdError.GetResponseInfo();
        Log.TraceInfo("Response info: " + responseInfo.ToString());
    }

    public void HandleInsOnAdOpened(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleAdOpened event received");
    }

    public void HandleInsOnAdClosed(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleAdClosed event received");
        InterstitialShowEventHandler?.Invoke(true);
        CreateInterstitial();
    }

#endregion

#region Rewarded Callback

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Log.TraceInfo(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.LoadAdError.GetMessage());

        LoadAdError loadAdError = args.LoadAdError;

        // Gets the domain from which the error came.
        string domain = loadAdError.GetDomain();

        // Gets the error code. See
        // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
        // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
        // for a list of possible codes.
        int code = loadAdError.GetCode();

        // Gets an error message.
        // For example "Account not approved yet". See
        // https://support.google.com/admob/answer/9905175 for explanations of
        // common errors.
        string message = loadAdError.GetMessage();

        // Gets the cause of the error, if available.
        AdError underlyingError = loadAdError.GetCause();

        // All of this information is available via the error's toString() method.
        Log.TraceInfo("Load error string: " + loadAdError.ToString());

        // Get response information, which may include results of mediation requests.
        ResponseInfo responseInfo = loadAdError.GetResponseInfo();
        Log.TraceInfo("Response info: " + responseInfo.ToString());
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Log.TraceInfo(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.AdError.GetMessage());
        InterstitialShowEventHandler?.Invoke(false);
        //实际项目中测试下失败的情况是否适用于重新加载的逻辑
        CreateRewarded();
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Log.TraceInfo("HandleRewardedAdClosed event received");
        RewardShowEventHandler?.Invoke(true);
        CreateRewarded();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Log.TraceInfo(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }

#endregion
#endif
}