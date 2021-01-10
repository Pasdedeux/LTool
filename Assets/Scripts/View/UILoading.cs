/*======================================
* 项目名称 ：Assets.Scripts.UI
* 项目描述 ：
* 类 名 称 ：UILoading
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.UI
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/8/6 9:48:13
* 更新时间 ：2019/8/6 9:48:13
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/8/6 9:48:13
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections;
using System.Linq;
using System.Text;
using LitFramework;
using UnityEngine;
using LitFramework.Mono;
using UnityEngine.UI;
using LitFramework.GameFlow;

namespace Assets.Scripts.UI
{
    public class UILoading : BaseUI
    {
        public Image sliderSlider;
        //public Text progressText;

        private ushort _progress = 0;
        private Func<bool> _func = () => { return true; };
        private LoadingTaskModel _loadingModel;
        private WaitForEndOfFrame _waitEndFrame;
        private WaitForSeconds _waitSeconds;
        private AsyncOperation _asyncOperation;

        public override void OnAwake()
        {
            CurrentUIType.uiNodeType = UINodeTypeEnum.Fixed;
            CurrentUIType.uiShowMode = UIShowModeEnum.Parallel;
            CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingLow;

            _loadingModel = LoadingTaskModel.Instance;
            _waitEndFrame = new WaitForEndOfFrame();
            _waitSeconds = new WaitForSeconds( 0.1f );
        }

        public override void OnStart()
        {
            Canvas canvas = GetComponent<Canvas>();
            StartCoroutine( OneFrame( () => { canvas.overrideSorting = true; canvas.sortingOrder = 9; } ) );
        }

        private IEnumerator OneFrame( Action callback )
        {
            yield return null;
            callback();
        }

        public override void OnShow()
        {
            _progress = 0;
            sliderSlider.fillAmount = _progress / 100f;
            //progressText.text = string.Format( "{0}%", _progress );
            LitFramework.LitTool.LitTool.monoBehaviour.StartCoroutine( IStartLoading() );
        }

        IEnumerator IStartLoading()
        {
            while ( _progress < 100 )
            {
                //if( _progress == 80 ) yield return _waitSeconds;

                var func = _loadingModel.TryGetTask( _progress );
                if ( func != null ) yield return new WaitUntil( func );

                _progress++;
                if ( sliderSlider == null ) yield break;
                sliderSlider.fillAmount = _progress / 100f;
                //progressText.text = string.Format( "{0}%", _progress );
                yield return _waitEndFrame;
            }

            sliderSlider.fillAmount = _progress / 100f;
            //progressText.text = string.Format( "{0}%", _progress );

            var func1 = _loadingModel.TryGetTask( _progress );
            if ( func1 != null ) yield return new WaitUntil( func1 );
        }

        public override void OnClose()
        {
            LitFramework.LitTool.LitTool.monoBehaviour.StopCoroutine( IStartLoading() );
        }
    }
}
