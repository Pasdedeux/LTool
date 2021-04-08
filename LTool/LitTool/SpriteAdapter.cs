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
    /// 对SpriteRender纹理进行Camera.main【正交】摄像机边界尺寸的调整
    /// </summary>
    public class SpriteAdapter : MonoBehaviour
    {
        private bool _isOrthographic = true;
        [SerializeField]
        public bool IsOrthographic
        {
            get { return _isOrthographic; }
            set
            {
                //if ( _isOrthographic != value )
                //{
                _isOrthographic = value;
                if ( _isOrthographic )
                { ResiezeEventHandler = ResizeOrt; _baseCamSize = new Vector2( _cam.aspect, 1f ); }
                else
                { ResiezeEventHandler = ResiezeProj; }
                //}
            }
        }
        private Camera _cam;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _oneVec = Vector2.one;
        private Vector2 _baseCamSize;
        //正交下缩放比例
        private Vector2 _baseScale;
        //透视视角指定距离下的四个边角世界坐标
        private Vector3[] _corners = new Vector3[ 4 ];

        //每帧Late执行回调
        private Action ResiezeEventHandler = null;

        void Awake()
        {
            _cam = Camera.main;
            _spriteRenderer = GetComponent<SpriteRenderer>();

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
            Vector2 cameraSize = _baseCamSize * _cam.orthographicSize * 2;

            if ( cameraSize.x >= cameraSize.y )
            { // Landscape (or equal)
                _baseScale = _oneVec * cameraSize.x / _spriteRenderer.sprite.bounds.size.x;
            }
            else
            { // Portrait
                _baseScale = _oneVec * cameraSize.y / _spriteRenderer.sprite.bounds.size.y;
            }

            transform.localScale = _baseScale;
        }

        /// <summary>
        /// 透视适配
        /// </summary>
        private void ResiezeProj()
        {
            ////Press the Space key to increase the size of the sprite
            //if ( Input.GetKey( KeyCode.Space ) )
            //{
            //    Vector3 min = _spriteRenderer.bounds.min;
            //    Vector3 max = _spriteRenderer.bounds.max;

            //    _spriteRenderer.bounds.SetMinMax( min - new Vector3( 0.1f, 0.1f, 0.1f ), max - new Vector3( 0.1f, 0.1f, 0.1f ) );
            //}
        }

    }
}
