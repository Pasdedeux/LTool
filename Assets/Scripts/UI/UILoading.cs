﻿#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2021/4/13 11:23:33
// 该类由模板工具自动生成
///----------------------------------------------------------------*/
#endregion
using System;
using System.Collections;
using System.Linq;
using System.Text;
using LitFramework;
using UnityEngine;
using LitFramework.Mono;
using UnityEngine.UI;
using LitFramework.GameFlow;
using LitFramework.LitTool;

namespace Assets.Scripts.UI
{
    public class UILoading : BaseUI
    {
        public Image sliderSlider;
        public Text progressText;

        private ushort _progress = 0;
        private Func<bool> _func = () => { return true; };
        private LoadingTaskModel _loadingModel;
        private WaitForEndOfFrame _waitEndFrame;
        private Coroutine _coroutine;

        public override void OnAwake()
        {
            CurrentUIType.uiNodeType = UINodeTypeEnum.Fixed;

            _loadingModel = LoadingTaskModel.Instance;
            _waitEndFrame = new WaitForEndOfFrame();
        }

        public override void OnStart()
        {
            Canvas canvas = GetComponent<Canvas>();
            LitTool.MonoBehaviour.StartCoroutine( OneFrame( () => { canvas.overrideSorting = true; } ) );
        }

        private IEnumerator OneFrame( Action callback )
        {
            yield return null;
            callback();
        }

        public override void OnShow()
        {
            _progress = 0;
            _coroutine = LitFramework.LitTool.LitTool.MonoBehaviour.StartCoroutine( IStartLoading() );

            sliderSlider.fillAmount = _progress / 100f;
            progressText.text = string.Format( "{0}%", _progress );
        }

        IEnumerator IStartLoading()
        {
            while ( _progress < 100 )
            {
                var func = _loadingModel.TryGetTask( _progress );
                if ( func != null ) yield return new WaitUntil( func );

                _progress++;
                if ( sliderSlider == null ) yield break;
                sliderSlider.fillAmount = _progress / 100f;
                progressText.text = string.Format( "{0}%", _progress );
                yield return _waitEndFrame;
            }

            sliderSlider.fillAmount = _progress / 100f;
            progressText.text = string.Format( "{0}%", _progress );

            var func1 = _loadingModel.TryGetTask( _progress );
            if ( func1 != null ) yield return new WaitUntil( func1 );
        }

        public override void OnClose()
        {
            LitFramework.LitTool.LitTool.MonoBehaviour.StopCoroutine( _coroutine );
            _coroutine = null;
        }
    }
}
