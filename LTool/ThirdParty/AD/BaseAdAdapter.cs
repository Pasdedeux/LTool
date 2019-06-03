/*======================================
* 项目名称 ：Assets.Scripts.UI
* 项目描述 ：
* 类 名 称 ：BaseAdAdapter
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.UI
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/5/23 15:26:58
* 更新时间 ：2019/5/23 15:26:58
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/5/23 15:26:58
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;

public abstract class BaseAdAdapter : IAdPlatform
{
    public Action<bool> InterstitialShowEventHandler, InterstitialLoadEventHandler;
    public Action<bool> RewardShowEventHandler, RewardCloseEventHandler;

    public BaseAdAdapter()
    {
        Init();

        InitInitCallBack();
        InitBannerCallBack();
        InitRewardedCallBack();
    }

    public abstract void Init();

    public abstract void CreateBanner();

    public abstract void CreateInterstitial();

    public abstract void CreateRewarded();

    public virtual void Dispose()
    {
        SubDispose();

        InterstitialShowEventHandler = null;
        InterstitialLoadEventHandler = null;
        RewardShowEventHandler = null;
    }

    public abstract void SubDispose();

    public abstract void HideBanner( bool destroy );

    public abstract void HideInterstitial();

    public virtual void HideRewarded() { }

    protected abstract void InitBannerCallBack();

    protected abstract void InitInitCallBack();

    protected abstract void InitRewardedCallBack();

    public abstract void ShowBanner();

    public abstract void ShowInterstitial();

    public abstract void ShowRewarded();

    public abstract bool IsIntersititialReady();

    public abstract bool IsRewardedVideoAvailable();

    public virtual void OnApplicationPause( bool pause ) { }
}
