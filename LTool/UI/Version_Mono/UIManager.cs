/*======================================
* 项目名称 ：LitFramework.UI.Manager
* 项目描述 ：
* 类 名 称 ：UIManager
* 类 描 述 ：非反射版本，不做热更新
*                   
* 命名空间 ：LitFramework.UI.Manager
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/17 11:50:24
* 更新时间 ：2018/5/17 11:50:24
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DerekLiu 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/17 11:50:24
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using System.Reflection;
using System.Linq;
using LitFramework.Base;
using UnityEngine.UI;
using LitFramework.LitTool;
using LitFramework.UI.Base;

namespace LitFramework.Mono
{

    /// <summary>
    /// 以下UI类，需配合UImanager.unitypackage使用。
    /// 
    /// 主要包含Cavas_Root及相关Tag等
    /// 
    /// </summary>
    public class UIManager : Singleton<UIManager>,IManager,IUIManager
    {
        private bool _useFading = true;
        public bool UseFading
        {
            get { return _useFading; }
            set
            {
                if ( _fadeImage != null ) _fadeImage.gameObject.SetActive( value );
                _useFading = value;
            }
        }
        /// <summary>
        /// 所有的预制件名称列表
        /// </summary>
        private List<string> _allRegisterUIList;
        /// <summary>
        /// //定义“栈”集合,存储显示当前所有弹出窗口的窗体类型
        /// </summary>
        private Stack<BaseUI> _stackCurrentUI;
        /// <summary>
        /// 缓存已经开启过的所有窗体
        /// </summary>
        private Dictionary<string , BaseUI> _dictLoadedAllUIs;
        /// <summary>
        /// 当前显示的非弹出类UI窗体
        /// </summary>
        private Dictionary<string , BaseUI> _dictCurrentShowUIs;
        /// <summary>
        /// UI根节点
        /// </summary>
        private Transform _transCanvas = null;
        /// <summary>
        /// 普通窗口节点
        /// </summary>
        private Transform _transNormal = null;
        /// <summary>
        /// 固定UI节点
        /// </summary>
        private Transform _transFixed = null;
        /// <summary>
        /// 弹出窗口节点
        /// </summary>
        private Transform _transPopUp = null;
        /// <summary>
        /// 管理器节点
        /// </summary>
        private Transform _transGlobal = null;
        /// <summary>
        /// 外部传入UI的加载方法。Resource.Load || AssetBundle.Load
        /// </summary>
        public Func<string , GameObject> LoadResourceFunc;
        /// <summary>
        /// 全局渐变遮罩
        /// </summary>
        private Image _fadeImage;
        /// <summary>
        ///  遮罩结束时回调
        /// </summary>
        /// <returns></returns>
        private Action DelHideCallBack = null;

        public void Install()
        {
            _allRegisterUIList = new List<string>();
            _stackCurrentUI = new Stack<BaseUI>();
            _dictLoadedAllUIs = new Dictionary<string, BaseUI>();
            _dictCurrentShowUIs = new Dictionary<string, BaseUI>();

            _transCanvas = GameObject.FindGameObjectWithTag( UISysDefine.SYS_TAG_ROOTCANVAS ).transform;
            _transNormal = UnityHelper.FindTheChildNode( _transCanvas, UISysDefine.SYS_TAG_NORMALCANVAS );
            _transFixed = UnityHelper.FindTheChildNode( _transCanvas, UISysDefine.SYS_TAG_FIXEDCANVAS );
            _transPopUp = UnityHelper.FindTheChildNode( _transCanvas, UISysDefine.SYS_TAG_POPUPCANVAS );
            _transGlobal = UnityHelper.FindTheChildNode( _transCanvas, UISysDefine.SYS_TAG_GLOBALCANVAS );
            _fadeImage = UnityHelper.FindTheChildNode( _transGlobal, "Image_fadeBG" ).GetComponent<Image>();

            if ( _fadeImage == null )
                Debug.LogWarning( "Image_fadeBG 未定义" );
            else if ( !_fadeImage.gameObject.activeInHierarchy )
                Debug.LogWarning( "Image_fadeBG 未启用" );

            GameObject.DontDestroyOnLoad( _transCanvas.gameObject );
        }

        public void Uninstall()
        {
            while( _stackCurrentUI.Count > 0 )
            {
                BaseUI ui = _stackCurrentUI.Pop();
                ui.Close( true );
            }

            if( _dictLoadedAllUIs != null )
            {
                var list = _dictLoadedAllUIs.ToList();
                for( int i = list.Count - 1; i >= 0; i-- )
                    Close( list[ i ].Key , true );
                list = null;
            }
            _allRegisterUIList.Clear();
            _stackCurrentUI.Clear();
            _dictCurrentShowUIs.Clear();

            _transFixed = null;
            _transPopUp = null;
            _transCanvas = null;
            _transNormal = null;
            _transGlobal = null;
            _stackCurrentUI = null;
            _allRegisterUIList = null;
            _dictLoadedAllUIs = null;
            _dictCurrentShowUIs = null;

            LoadResourceFunc = null;

            Resources.UnloadUnusedAssets();
        }


        /// <summary>
        /// 隐退开始
        /// </summary>
        /// <param name="time"></param>
        /// <param name="callBack"></param>
        public void ShowFade( float time, Action callBack = null )
        {
            if ( !UseFading || _fadeImage == null || !_fadeImage.gameObject.activeInHierarchy )
            {
                if ( callBack != null )
                    callBack.Invoke();
                return;
            }
            
            _fadeImage.raycastTarget = true;
            _fadeImage.CrossFadeAlpha( 1, time, false );
            if ( callBack != null )
                LitTool.LitTool.DelayPlayFunction( time, callBack );
        }


        /// <summary>
        /// 隐退结束
        /// </summary>
        /// <param name="time"></param>
        /// <param name="callBack"></param>
        public void HideFade( float time, Action callBack = null )
        {
            if ( !UseFading || _fadeImage == null || !_fadeImage.gameObject.activeInHierarchy )
            {
                if ( callBack != null )
                    callBack.Invoke();
                return;
            }

            _fadeImage.CrossFadeAlpha( 0, time, false );
            
            if ( callBack != null ) DelHideCallBack += callBack;
            DelHideCallBack += () => { _fadeImage.raycastTarget = false; };
            DelHideCallBack += () => { DelHideCallBack = null; };
            LitTool.LitTool.DelayPlayFunction( time, DelHideCallBack );
        }


        /// <summary>
        /// 显示（打开）UI窗口
        /// 功能：
        /// 1、根据UI窗体的名称，加载到UI窗口缓存列表
        /// 2、根据不同UI显示模式，做不同的加载处理
        /// </summary>
        /// <param name="uiName">UI窗体预制件名称</param>
        public IBaseUI Show( string uiName )
        {
            BaseUI baseUI = null;

            if( string.IsNullOrEmpty( uiName ) )
                throw new Exception( "UI--uiName 为 Null" );

            //根据名称加载窗体到UI窗体缓存中
            baseUI = LoadUIToAndFromAllList( uiName );
            if( baseUI == null )
                throw new Exception( "UI--baseUI 加载失败" );

            //判断是否清空“栈”结构体集合
            if( baseUI.CurrentUIType.isClearPopUp )
                ClearPopUpStackArray();

            switch( baseUI.CurrentUIType.uiShowMode )
            {
                case UIShowModeEnum.Parallel:
                    LoadNormalUI( uiName );
                    break;
                case UIShowModeEnum.Stack:
                    LoadPopUpUI( uiName );
                    break;
                case UIShowModeEnum.Unique:
                    LoadUniqueUI( uiName );
                    break;
                default:
                    throw new Exception( "未登记的UI类型--" + baseUI.CurrentUIType.uiShowMode );
            }
            return baseUI;
        }

        /// <summary>
        /// 关闭指定UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="isDestroy">是否直接释放所有资源，销毁</param>
        public void Close( string uiName , bool isDestroy = false )
        {
            if( string.IsNullOrEmpty( uiName ) )
                return;

            //所有窗体如果没有记录，直接返回
            _dictLoadedAllUIs.TryGetValue( uiName, out BaseUI baseUI );
            if ( baseUI == null )
            {
                _dictLoadedAllUIs.Remove( uiName );
                return;
            }

            //不同类型窗体执行各自关闭逻辑
            switch( baseUI.CurrentUIType.uiShowMode )
            {
                case UIShowModeEnum.Parallel:
                    UnLoadNormalUI( uiName , isDestroy );
                    break;
                case UIShowModeEnum.Stack:
                    UnLoadPopUpUI( uiName , isDestroy );
                    break;
                case UIShowModeEnum.Unique:
                    UnLoadUniqueUI( uiName , isDestroy );
                    break;
                default:
                    throw new Exception( "未登记的UI类型--" + baseUI.CurrentUIType.uiShowMode );
            }

            //销毁
            if( isDestroy )
            {
                BaseUI ui = null;
                _dictLoadedAllUIs.TryGetValue( uiName , out ui );
                if( ui != null )
                    ui.Close( true );
                _dictLoadedAllUIs.Remove( uiName );
            }
        }



        /// <summary>
        /// 清空弹出窗口栈
        /// </summary>
        public bool ClearPopUpStackArray()
        {
            if( _stackCurrentUI != null && _stackCurrentUI.Count > 0 )
            {
                foreach( var stackUI in _stackCurrentUI )
                {
                    stackUI.Close();
                }
                _stackCurrentUI.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据UI预制件名称加载到UI缓存列表（按需），同时获取实例
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        private BaseUI LoadUIToAndFromAllList( string uiName )
        {
            BaseUI result = null;
            _dictLoadedAllUIs.TryGetValue( uiName , out result );
            if( result == null )
                result = LoadUI( uiName );
            return result;
        }

        /// <summary>
        /// 加载指定名称UI
        /// 功能：
        /// 1、根据名称加载预制体
        /// 2、根据UI类型加载到不同节点下
        /// 3、隐藏创建的UI克隆体
        /// 4、把克隆体加入到所有窗体列表
        /// </summary>
        /// <param name="uiName">窗体名称</param>
        /// <returns></returns>
        private BaseUI LoadUI( string uiName )
        {
            //加载的UI预制体
            BaseUI baseUI = null;
            GameObject prefClone = null;

            //加载预制体
            if( !string.IsNullOrEmpty( uiName ) )
                prefClone = LoadResourceFunc != null ? LoadResourceFunc( uiName ) : null;
            if( prefClone == null )
                throw new Exception( "未指定UI预制件加载方法或UI预制件路径指定错误 ==>" + uiName );
            prefClone = GameObject.Instantiate( prefClone );

            //设置父节点
            if( _transCanvas != null && prefClone != null )
            {
                baseUI = prefClone.GetComponent<BaseUI>();
                if ( prefClone == null )
                    throw new Exception( string.Format( "UI预制件 {0} 未挂载方法 {1} ", prefClone.name, uiName ) );
                baseUI.AssetsName = uiName;

                if ( baseUI == null )
                { Debug.LogError( uiName + "UI 脚本加载失败" ); return null; }

                switch( baseUI.CurrentUIType.uiNodeType )
                {
                    case UINodeTypeEnum.Normal:
                        prefClone.transform.SetParent( _transNormal , false );
                        break;
                    case UINodeTypeEnum.Fixed:
                        prefClone.transform.SetParent( _transFixed , false );
                        break;
                    case UINodeTypeEnum.PopUp:
                        prefClone.transform.SetParent( _transPopUp , false );
                        break;
                    default:
                        throw new Exception( "未登记的UI类型--" + baseUI.CurrentUIType.uiShowMode );
                }

                //加入到所有窗体缓存中
                _dictLoadedAllUIs.Add( uiName , baseUI );
                return baseUI;
            }
            
            Debug.Log( "窗体加载失败！" );
            return null;
        }

        /// <summary>
        /// 加载当前窗体到当前窗体集合
        /// </summary>
        private void LoadNormalUI( string uiName )
        {
            BaseUI baseUI;
            _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
            if( baseUI != null )
            {
                if( baseUI.IsShowing )
                    baseUI.OnShow();
                else
                    baseUI.Show();
                return;
            }

            //加载当前窗体到当前显示集合
            _dictLoadedAllUIs.TryGetValue( uiName , out baseUI );
            if( baseUI != null )
            {
                _dictCurrentShowUIs.Add( uiName , baseUI );
                baseUI.Show();
            }
        }

        /// <summary>
        /// 从当前UI列表缓存中卸载UI窗体
        /// </summary>
        /// <param name="uiName"></param>
        private void UnLoadNormalUI( string uiName , bool isDestroy = false )
        {
            BaseUI baseUI;

            //当前UI显示列表中没有记录或者总表中没有记录则直接返回
            _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
            if( baseUI == null )
            {
                if( !isDestroy )
                    return;
                else
                    _dictLoadedAllUIs.TryGetValue( uiName , out baseUI );

                if( baseUI == null )
                    return;
            }
            else
                //隐藏窗口并从列表中移除
                _dictCurrentShowUIs.Remove( uiName );

            baseUI.Close( isDestroy: isDestroy );
        }

        /// <summary>
        /// 加载独占UI窗体
        /// </summary>
        /// <param name="uiName"></param>
        private void LoadUniqueUI( string uiName )
        {
            BaseUI baseUI;
            //当前UI显示列表中没有记录则直接返回
            _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
            if( baseUI != null )
            {
                if( baseUI.IsShowing )
                    baseUI.OnShow();
                else
                    baseUI.Show();
                return;
            }

            //正在显示的UI进行隐藏
            foreach( BaseUI baseui in _dictCurrentShowUIs.Values )
            {
                baseui.Close();
            }
            foreach( BaseUI baseui in _stackCurrentUI )
            {
                baseui.Close();
            }

            //把当前窗体加载到正显示的UI窗口缓存中去
            _dictLoadedAllUIs.TryGetValue( uiName , out baseUI );
            if( baseUI != null )
            {
                _dictCurrentShowUIs.Add( uiName , baseUI );
                baseUI.Show();
            }

        }

        /// <summary>
        /// 卸载当前UI，并将原先被隐藏的UI重新显示
        /// </summary>
        /// <param name="uiName"></param>
        private void UnLoadUniqueUI( string uiName , bool isDestroy = false )
        {
            BaseUI baseUI;

            _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
            if( baseUI == null )
            {
                if( !isDestroy )
                    return;
                else
                    _dictLoadedAllUIs.TryGetValue( uiName , out baseUI );

                if( baseUI == null )
                    return;
            }
            else
                //隐藏窗口并从列表中移除
                _dictCurrentShowUIs.Remove( uiName );

            baseUI.Close( isDestroy: isDestroy );

            //如果需要清空已有 popup 窗口
            if( baseUI.CurrentUIType.isClearPopUp )
                ClearPopUpStackArray();

            //正在显示的窗口和栈缓存的窗口再次进行显示处理
            foreach( BaseUI baseui in _dictCurrentShowUIs.Values )
            {
                baseui.Show( true );
            }
            foreach( BaseUI baseui in _stackCurrentUI )
            {
                baseui.Show( true );
            }
        }

        /// <summary>
        /// 弹出窗口，入栈
        /// 先冻结栈中窗口，再将此窗口入栈
        /// </summary>
        /// <param name="uiName"></param>
        private void LoadPopUpUI( string uiName )
        {
            BaseUI baseUI;

            //判断栈里是否有窗口，有则冻结响应
            if( _stackCurrentUI.Count > 0 )
            {
                BaseUI topUI = _stackCurrentUI.Peek();
                topUI.Close( freeze: true );
            }

            //获取当前UI，进行展示处理
            _dictLoadedAllUIs.TryGetValue( uiName , out baseUI );
            if( baseUI != null )
            {
                if( baseUI.IsShowing )
                    baseUI.OnShow();
                else
                    baseUI.Show();

                if( !_stackCurrentUI.Contains( baseUI ) )
                    //该弹出UI入栈
                    _stackCurrentUI.Push( baseUI );
            }
            else
                throw new Exception( "UIManager catch an error" );
        }

        /// <summary>
        /// 弹出窗口，出栈
        /// </summary>
        /// <param name="uiName"></param>
        private void UnLoadPopUpUI( string uiName , bool isDestroy = false )
        {
            //有两个以上弹窗出现时
            if( _stackCurrentUI.Count >= 2 )
            {
                //第一个出栈
                BaseUI topUI = _stackCurrentUI.Pop();
                topUI.Close( isDestroy: isDestroy );

                //第二个重新显示
                BaseUI nextUI = _stackCurrentUI.Peek();
                nextUI.Show( true );
            }
            //当前只有一个弹窗
            else if( _stackCurrentUI.Count == 1 )
            {
                //出栈的窗体进行隐藏
                BaseUI topUI = _stackCurrentUI.Pop();
                topUI.Close( isDestroy: isDestroy );
            }
        }

        /// <summary>
        /// 从现有缓存中查找目标UI，未加载则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseUI GetUIByName( string name )
        {
            BaseUI baseUI;
            _dictLoadedAllUIs.TryGetValue( name , out baseUI );
            return baseUI;
        }


        /// <summary>
        /// 返回键执行关闭窗口操作
        /// </summary>
        public void OnEscapeCallback()
        {
            if (_stackCurrentUI.Count > 0)
            {
                var baseUi = _stackCurrentUI.Pop();
                baseUi.Close();
                return;
            }
        }
    }
}