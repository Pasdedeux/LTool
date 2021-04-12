/**************************************************************** 
 * 作    者：Derek Liu 
 * CLR 版本：4.0.30319.42000 
 * 创建时间：2018/1/31 17:42:22 
 * 当前版本：1.0.0.1 
 *  
 * 描述说明： 
 * 
 * 修改历史： 
 * 
***************************************************************** 
 * Copyright @ Derek Liu 2018 All rights reserved 
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace LitFramework.LitTool
{
    /// <summary>
    /// 对SpriteRender纹理进行Camera.main摄像机边界尺寸的调整
    /// </summary>
    public class SpriteAdapter : MonoBehaviour
    {
        private Transform _selfTrans;

        private bool _isOrthographic = true;
        public bool IsOrthographic
        {
            get { return _isOrthographic; }
            set
            {
                _isOrthographic = value;
                if ( _isOrthographic )
                { ResiezeEventHandler = ResizeOrt; _baseCamSize = new Vector2( _cam.aspect, 1f ); }
                else
                { ResiezeEventHandler = ResiezeProj; }
            }
        }
        private Camera _cam;
        private SpriteRenderer _spriteRenderer;
        //当前物体转换后的屏幕坐标
        private Vector3 _localScreenPos;

        //当前物体距离摄像机距离
        private float _distance;
        //当前对象自身缩放比例
        private Vector3 _localScale;
        private Vector2 _baseCamSize;
        private Vector2 _oneVec = Vector2.one;
        //正交下缩放比例
        private Vector2 _baseScale;
        private Vector2 _cameraSize;
        //透视视角指定距离下的四个边角世界坐标
        private Vector3[] _corners = new Vector3[ 4 ];

        //每帧Late执行回调
        private Action ResiezeEventHandler = null;

        void Awake()
        {
            _cam = Camera.main;
            _selfTrans = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _distance = Vector3.Distance( _selfTrans.position, _cam.transform.position );
            _localScale = _selfTrans.localScale;
            _localScreenPos = _cam.WorldToScreenPoint( _selfTrans.position );

            IsOrthographic = _cam.orthographic;
        }

        
        private void LateUpdate()
        {
            ResiezeEventHandler?.Invoke();
        }

        /// <summary>
        /// 正交适配
        /// </summary>
        private void ResizeOrt()
        {
            _cameraSize = _baseCamSize * _cam.orthographicSize * 2;

            if ( _cameraSize.x >= _cameraSize.y )
            { // Landscape (or equal)
                _baseScale = _oneVec * _cameraSize.x / _spriteRenderer.sprite.bounds.size.x;
            }
            else
            { // Portrait
                _baseScale = _oneVec * _cameraSize.y / _spriteRenderer.sprite.bounds.size.y;
            }

            _selfTrans.localScale = _baseScale;
        }

        /// <summary>
        /// 透视适配
        /// </summary>
        private void ResiezeProj()
        {
            //保持图片自身的初始位置不会因为FOV变化而产生位移
            _selfTrans.localPosition = _cam.ScreenToWorldPoint( _localScreenPos );
            //在给定距离的视锥体高度（两者的单位都为世界单位）
            float frustumHeight = 2.0f * _distance * Mathf.Tan( _cam.fieldOfView * 0.5f * Mathf.Deg2Rad );
            _selfTrans.localScale = _localScale * frustumHeight;
        }

    }
}
