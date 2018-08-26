#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.Input
* 项目描述 ：
* 类 名 称 ：TouchInput
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.Input
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/6/6 21:28:28
* 更新时间 ：2018/6/6 21:28:28
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion


namespace LitFramework.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [Flags]
    public enum TouchDirection
    {
        None = 1<<0,
        Left = 1<<1,
        Right = 1<<2,
        Up = 1<<3,
        Down = 1<<4,
        OnUI = 1<<5
    }

    public class TouchDirectionControl : SingletonMono<TouchDirectionControl>
    {
        //滑动距离限制，超过这个距离视为滑动有效
        private const float MOVE_MIX_DISTANCE = 1f;
        //触碰阶段外发函数
        public Action<Vector2> TouchBeganCallback, TouchEndCallback, TouchStationaryCallback, TouchMoveCallback;
        private Vector2 _touchBeginPos;
        private Vector2 _touchEndPos;
        private float _clockWiseDegree = 0;
        private EventSystem _currentEventSys = EventSystem.current;

        /// <summary>
        /// 设置触控方向的【顺时针】旋转角度
        /// </summary>
        /// <param name="degree"></param>
        public void SetRotateClockwise( float degree )
        {
            _clockWiseDegree = degree;
        }


        /// <summary>
        /// 获取一次滑动行为
        /// </summary>
        /// <returns></returns>
        public TouchDirection GetTouchMoveDirection( TouchDirection dirResult )
        {
            if ( Input.touchCount > 0 )
            {
                //实时检测触碰到UI
                if ( _currentEventSys.IsPointerOverGameObject( Input.GetTouch( 0 ).fingerId ) )
                    dirResult |= TouchDirection.OnUI;

                if ( Input.touches[ 0 ].phase != TouchPhase.Canceled )
                {
                    Vector2 inputPos = Input.touches[ 0 ].position;
                    switch ( Input.touches[ 0 ].phase )
                    {
                        case TouchPhase.Began:
                            _touchBeginPos = inputPos;
                            TouchBeganCallback?.Invoke( inputPos );
                            break;
                        case TouchPhase.Ended:
                            _touchEndPos = inputPos;
                            TouchEndCallback?.Invoke( inputPos );
                            if ( Vector2.Distance( _touchBeginPos, _touchEndPos ) < MOVE_MIX_DISTANCE )
                                return dirResult;
                            float offSetX = _touchEndPos.x - _touchBeginPos.x;
                            float offSetY = _touchEndPos.y - _touchBeginPos.y;
                            float angle = Mathf.Atan2( offSetY, offSetX ) * Mathf.Rad2Deg;
                            //顺时针偏移
                            angle += _clockWiseDegree;
                            if ( angle > 180 )
                                angle = 360 - angle;
                            else if ( angle < -180 )
                                angle += 360;
                            if ( angle <= 45f && angle > -45f )
                                dirResult |= TouchDirection.Right;
                            else if ( angle > 45f && angle <= 135f )
                                dirResult |= TouchDirection.Up;
                            else if ( ( angle > 135f && angle < 180f ) || ( angle > -180f && angle <= -135f ) )
                                dirResult |= TouchDirection.Left;
                            else if ( angle > -135f && angle <= -45f )
                                dirResult |= TouchDirection.Down;
                            break;
                        case TouchPhase.Moved:
                            TouchMoveCallback?.Invoke( inputPos );
                            break;
                        case TouchPhase.Stationary:
                            TouchStationaryCallback?.Invoke( inputPos );
                            break;
                    }
                }
                return dirResult;
            }
            else
                return dirResult = TouchDirection.None;
        }
    }
}