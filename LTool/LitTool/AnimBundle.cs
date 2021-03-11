/*======================================
* 项目名称 ：LitFramework.LitTool
* 项目描述 ：
* 类 名 称 ：AnimBundle
* 类 描 述 ：
* 命名空间 ：LitFramework.LitTool
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/3/11 14:26:21
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using DG.Tweening;
using LitFramework.LitPool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LitFramework.LitTool
{
    /// <summary>
    /// UI/场景动画包
    /// </summary>
    public class AnimBundle : SingletonMono<AnimBundle>
    {
        /// <summary>
        /// 在目标位置炸开多个UI元素，等待固定时间后，飞向指定区域
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="num">数量</param>
        /// <param name="range">范围</param>
        /// <param name="explodeTime">炸开时间</param>
        /// <param name="flyTime">飞到终点所需时间</param>
        /// <param name="spwanObjName">预制名称</param>
        /// <param name="parnet">设置的父节点</param>
        /// <param name="oneNumFun">每一颗飞到终点时执行的事情</param>
        /// <param name="secondsTime">炸开后等多久开始飞</param>
        /// <returns></returns>
        private IEnumerator IGenerateTargetUI
            ( Vector3 from, Vector3 to, int num, float range, float explodeTime, float flyTime, string spwanObjName, Transform parnet, Action oneNumFun, float secondsTime = 0.75f )
        {
            var explodeYield = new WaitForEndOfFrame();

            List<Transform> generatedCashList = new List<Transform>( num );
            for ( int i = 0; i < num; i++ )
            {
                var cash = SpawnManager.Instance.SpwanObject( spwanObjName );
                cash.transform.localScale = Vector3.one;
                cash.transform.parent = parnet;
                cash.transform.position = from;

                var exPos = new Vector2( from.x, from.y ) + UnityEngine.Random.insideUnitCircle * range;
                cash.transform.DOMove( exPos, explodeTime ).SetUpdate( true ).SetEase( Ease.OutBack );

                generatedCashList.Add( cash.transform );
                yield return explodeYield;
            }

            yield return new WaitForSeconds( secondsTime );

            for ( int i = generatedCashList.Count - 1; i > -1; i-- )
            {
                int index = i;
                generatedCashList[ i ].DOMove( to, flyTime ).SetUpdate( true ).SetEase( Ease.InExpo ).OnComplete( () =>
                {
                    var target = generatedCashList[ index ];
                    SpawnManager.Instance.DespawnObject( target );
                    generatedCashList.Remove( target );
                    oneNumFun?.Invoke();
                } );
                yield return explodeYield;
            }
        }
    }
}
