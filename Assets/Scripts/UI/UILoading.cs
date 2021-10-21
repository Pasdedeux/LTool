#region << 版 本 注 释 >>
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
using LitFramework.HotFix;
using UnityEngine.UI;
using LitFramework.GameFlow;
using LitFramework.LitTool;

namespace Assets.Scripts.UI
{
    public class UILoading : BaseUI
    {
        public static bool LOADING_CONTINUE = true;

        private Image _sliderSlider;
        private Text _progressText;
        private Text _versionText;

        private ushort _progress = 0;
        private static Func<bool> _func = () => { return LOADING_CONTINUE; };
        private LoadingTaskModel _loadingModel;
        private WaitForEndOfFrame _waitEndFrame;
        private WaitForSeconds _waitSeconds;
        private WaitUntil _waitCondition = new WaitUntil( _func );
        private AsyncOperation _asyncOperation;
        private Coroutine _coroutine;

        public static int RegistSystem( string className )
        {
            UIManager.Instance.RegistFunctionCallFun( ResPath.UI.UILOADING, className );
            return 1;
        }

        public override void OnAwake()
        {
            CurrentUIType.uiNodeType = UINodeTypeEnum.Fixed;
            CurrentUIType.uiShowMode = UIShowModeEnum.Stack;
            CurrentUIType.uiTransparent = UITransparentEnum.NoPenetratingMiddle;

            _loadingModel = LoadingTaskModel.Instance;
            _waitEndFrame = new WaitForEndOfFrame();
            _waitSeconds = new WaitForSeconds( 0.1f );

            _sliderSlider = UnityHelper.GetTheChildNodeComponetScripts<Image>( this.GameObjectInstance.transform, "Image_Progress" );
            _progressText = UnityHelper.GetTheChildNodeComponetScripts<Text>( this.GameObjectInstance.transform, "Text_Loading" );
            _versionText = UnityHelper.GetTheChildNodeComponetScripts<Text>( this.GameObjectInstance.transform, "Text_Version" );
        }

        public override void OnStart()
        {
            _versionText.text = "游戏版本：v" + Application.version;
            Canvas canvas = this.GameObjectInstance.transform.GetComponent<Canvas>();
            LitTool.MonoBehaviour.StartCoroutine( OneFrame( () => { canvas.overrideSorting = true; } ) );
        }

        private IEnumerator OneFrame( Action callback )
        {
            yield return null;
            callback?.Invoke();
        }

        public override void OnShow()
        {
            _progress = 0;
            _coroutine = LitFramework.LitTool.LitTool.MonoBehaviour.StartCoroutine( IStartLoading() );

            _sliderSlider.fillAmount = _progress / 100f;
            _progressText.text = string.Format( "{0}%", _progress );
        }

        IEnumerator IStartLoading()
        {
            while ( _progress < 100 )
            {
                yield return _waitCondition;

                 var func = _loadingModel.TryGetTask( _progress );
                if ( func != null ) yield return new WaitUntil( func );

                _progress++;
                //if ( sliderSlider == null ) yield break;
                _sliderSlider.fillAmount = _progress / 100f;
                _progressText.text = string.Format( "{0}%", _progress );
                yield return _waitEndFrame;
            }

            _sliderSlider.fillAmount = _progress / 100f;
            _progressText.text = string.Format( "{0}%", _progress );

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
