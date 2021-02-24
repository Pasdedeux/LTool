/*======================================
* 项目名称 ：Assets.Scripts.Manager
* 项目描述 ：
* 类 名 称 ：AnimationManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Manager
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/6/5 18:15:46
* 更新时间 ：2019/6/5 18:15:46
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/6/5 18:15:46
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using DG.Tweening;
using System;
using UnityEngine;

public static class AnimationManager
{
    public static DOTweenAnimation[] GetAllAnim( Transform root )
    {
        DOTweenAnimation[] result;
        result = root.GetComponentsInChildren<DOTweenAnimation>( true );
        for ( int i = 0; i < result.Length; i++ )
        {
            result[ i ].autoKill = false;
            result[ i ].autoPlay = false;
        }
        return result;
    }

    public static void Restart( this DOTweenAnimation[] animArray, string id, float delayTime, Action callBack = null )
    {
        Restart( animArray, id );

        if ( callBack != null )
            LitFramework.LitTool.LitTool.DelayPlayFunction( delayTime, () => { callBack.Invoke(); } );
    }

    public static void Restart( this DOTweenAnimation[] animArray , string id )
    {
        for ( int i = 0; i < animArray.Length; i++ )
            animArray[ i ].DORestartById( id );
    }

    public static void Restart( this DOTweenAnimation[] animArray, string id, Action callBack = null )
    {
        for ( int i = 0; i < animArray.Length; i++ )
            animArray[ i ].DORestartById( id );

        if ( callBack != null )
            LitFramework.LitTool.LitTool.DelayPlayFunction( 0.4f, () => { callBack.Invoke(); } );
    }

    public static void Pause( this DOTweenAnimation[] animArray, string id , Action callBack = null )
    {
        Pause( animArray, id );
        callBack?.Invoke();
    }

    public static void Pause( this DOTweenAnimation[] animArray, string id )
    {
        for ( int i = 0; i < animArray.Length; i++ )
            animArray[ i ].DOPauseAllById( id );
    }
}

