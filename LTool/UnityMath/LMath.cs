/****************************************************************
 * 作    者：Derek Liu
 * CLR 版本：4.0.30319.42000
 * 创建时间：2018/2/2 22:08:14
 * 当前版本：1.0.0.1
 * 
 * 描述说明：
 *
 * 修改历史：
 *
*****************************************************************
 * Copyright @ Derek 2018 All rights reserved
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LTool.UnityMath
{
    public static class LMath
    {
        /// <summary>
        /// 返回 -180°~180°/ 0°~180° 向量夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fromZero">是否最小值为0°。默认为 False</param>
        /// <returns></returns>
        public static float VectorAngle( Vector2 from , Vector2 to , bool fromZero = false )
        {
            float angle;
            if ( !fromZero )
            {
                Vector3 cross = Vector3.Cross( from , to );
                angle = Vector2.Angle( from , to );
                return cross.z > 0 ? -angle : angle;
            }
            else
            {
                angle = Vector2.Angle( from , to );
                return angle;
            }
        }

    }
}
