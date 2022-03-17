/*======================================
* 项目名称 ：Assets.Scripts
* 项目描述 ：
* 类 名 称 ：GuideConfig
* 类 描 述 ：
* 命名空间 ：Assets.Scripts
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2021/1/10 12:06:53
* 更新时间 ：2021/1/10 12:06:53
* 版 本 号 ：v1.0.0.0
*******************************************************************
======================================*/

using LitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LitFramework.Singleton;

/// <summary>
/// 引导动画配置
/// </summary>
public partial class GuideConfig:SingletonMono<GuideConfig>
{
    [Header( "高亮区域缩放的动画时间" )]
    public float shrinkTime = 0.2f;
    
    [Header( "手形图片对目标按钮的位置偏移" )]
    public Vector3 handImageOffset = new Vector3( 0.5f, 0f, 0 );

    [Header( "聚焦过程中当前值和目标值最小差值（圆）" )]
    public float thresholdCircle = 0.3f;

    [Header( "聚焦过程中当前值和目标值最小差值（方）" )]
    public float thresholdRect = 0.01f;
}
