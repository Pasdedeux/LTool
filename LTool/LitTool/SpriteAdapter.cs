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
    /// 对SpriteRender纹理进行UI摄像机正交边界尺寸的调整
    /// </summary>
    public class SpriteAdapter : MonoBehaviour
    {
        private Camera _cam;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _oneVec = Vector2.one;
        private Vector2 _baseCamSize;
        private Vector2 _baseScale;

        void Awake()
        {
            _cam = Camera.main;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _baseCamSize = new Vector2( _cam.aspect, 1f );

            Resize();
        }

        private void Resize()
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

        private void LateUpdate()
        {
            Resize();
        }
    }
}
