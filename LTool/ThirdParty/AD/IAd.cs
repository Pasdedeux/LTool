/*======================================
* 项目名称 ：Interface
* 项目描述 ：
* 类 名 称 ：IAd
* 类 描 述 ：
* 命名空间 ：
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/5/21 14:11:16
* 更新时间 ：2019/5/21 14:11:16
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/5/21 14:11:16
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/


public interface IAdPlatform
{
    #region Banner
    void CreateBanner();
    void ShowBanner();
    void HideBanner( bool destroy );
    //void InitBannerCallBack();
    #endregion

    #region Interstitial
    void CreateInterstitial();
    void ShowInterstitial();
    void HideInterstitial();
    //void InitInitCallBack();
    bool IsIntersititialReady();
    #endregion

    #region Rewarded
    void CreateRewarded();
    void ShowRewarded();
    void HideRewarded();
    //void InitRewardedCallBack();
    bool IsRewardedVideoAvailable();
    #endregion

    void Dispose();
}
