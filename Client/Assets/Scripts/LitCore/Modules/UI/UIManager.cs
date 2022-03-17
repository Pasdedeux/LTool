﻿/*======================================
* 项目名称 ：LitFramework.UI.Manager
* 项目描述 ：
* 类 名 称 ：UIManager
* 类 描 述 ：
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
using System.Diagnostics;

namespace LitFramework
{

    /// <summary>
    /// 以下UI类，需配合UImanager.unitypackage使用。
    /// 
    /// 主要包含Cavas_Root及相关Tag等
    /// 
    /// </summary>
    public class UIManager : Singleton<UIManager>, IManager, IUIManager
    {
        public Action<float, Action> FadeStartAction = null, FadeHideAction = null;

        /// <summary>
        /// UI根节点
        /// </summary>
        public Transform TransRoot { get; private set; }
        /// <summary>
        /// 普通窗口节点
        /// </summary>
        public Transform TransNormal { get; private set; }
        /// <summary>
        /// 固定UI节点
        /// </summary>
        public Transform TransFixed { get; private set; }
        /// <summary>
        /// 弹出窗口节点
        /// </summary>
        public Transform TransPopUp { get; private set; }
        /// <summary>
        /// 顶级窗口节点
        /// </summary>
        public Transform TransGlobal { get; private set; }

        /// <summary>
        /// UI根节点
        /// </summary>
        public RectTransform RectransRoot { get; private set; }
        /// <summary>
        /// 普通窗口节点
        /// </summary>
        public RectTransform RectransNormal { get; private set; }
        /// <summary>
        /// 固定UI节点
        /// </summary>
        public RectTransform RectransFixed { get; private set; }
        /// <summary>
        /// 弹出窗口节点
        /// </summary>
        public RectTransform RectransPopUp { get; private set; }
        /// <summary>
        /// 弹出窗口节点
        /// </summary>
        public RectTransform RectransGlobal { get; private set; }

        /// <summary>
        /// 外部传入UI的加载方法。Resource.Load || AssetBundle.Load
        /// </summary>
        public Func<string, GameObject> LoadResourceFunc;
        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICam { get; set; }
        /// <summary>
        /// 全局渐变遮罩
        /// </summary>
        private Image _fadeImage;
        /// <summary>
        ///  遮罩结束时回调
        /// </summary>
        /// <returns></returns>
        public event Action DelHideCallBack = null;
        /// <summary>
        /// Image_fadeBG控制类，一般用于全局黑屏转场。
        /// </summary>
        public Image FadeImage { get { return _fadeImage; } }
        /// <summary>
        /// PopUp 类弹窗所使用的背景蒙版，默认颜色为黑色
        /// </summary>
        public Image MaskImage { get { return UIMaskManager.Instance.Mask; } }
        /// <summary>
        /// UI适配策略
        /// </summary>
        public CanvasScaler CanvasScaler { get; set; }

        /// <summary>
        /// 所有的预制件名称列表
        /// </summary>
        private Dictionary<string, string> _allRegisterUIDict;
        /// <summary>
        /// //定义“栈”集合,存储显示当前所有弹出窗口的窗体类型
        /// </summary>
        private Stack<IBaseUI> _stackCurrentUI;
        /// <summary>
        /// 缓存已经开启过的所有窗体
        /// </summary>
        private Dictionary<string, IBaseUI> _dictLoadedAllUIs;
        /// <summary>
        /// 当前显示的弹出类UI窗体。按操作排序
        /// 记录顺序开启的所有弹窗，原则上按照顺序记录顺次关闭。stack弹窗使用单独队列管理顺序，并共享此列表
        /// </summary>
        private List<IBaseUI> _listCurrentPopupShowUIs;

        public void Install()
        {
            _allRegisterUIDict = new Dictionary<string, string>();
            _stackCurrentUI = new Stack<IBaseUI>();
            _listCurrentPopupShowUIs = new List<IBaseUI>(16);
            _dictLoadedAllUIs = new Dictionary<string, IBaseUI>();

            TransRoot = GameObject.FindGameObjectWithTag(UISysDefine.SYS_TAG_ROOTCANVAS).transform;
            TransNormal = UnityHelper.FindTheChildNode(TransRoot, UISysDefine.SYS_TAG_NORMALCANVAS);
            TransFixed = UnityHelper.FindTheChildNode(TransRoot, UISysDefine.SYS_TAG_FIXEDCANVAS);
            TransPopUp = UnityHelper.FindTheChildNode(TransRoot, UISysDefine.SYS_TAG_POPUPCANVAS);
            TransGlobal = UnityHelper.FindTheChildNode(TransRoot, UISysDefine.SYS_TAG_GLOBALCANVAS);

            RectransRoot = TransRoot.GetComponent<RectTransform>();
            RectransNormal = TransNormal.GetComponent<RectTransform>();
            RectransFixed = TransFixed.GetComponent<RectTransform>();
            RectransPopUp = TransPopUp.GetComponent<RectTransform>();
            RectransGlobal = TransGlobal.GetComponent<RectTransform>();

            _fadeImage = UnityHelper.FindTheChildNode(TransGlobal, "Image_fadeBG").GetComponent<Image>();

            try
            {
                if (_fadeImage == null)
                    LDebug.LogWarning("Image_fadeBG 未定义");
                else if (!_fadeImage.gameObject.activeInHierarchy)
                    LDebug.LogWarning("Image_fadeBG 未启用");

                _fadeImage.raycastTarget = false;
                _fadeImage.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                LDebug.LogError("Image_fadeBG 错误");
            }

            //Mask蒙版初始化
            var ss = UIMaskManager.Instance;

            UICam = UnityHelper.FindTheChildNode(TransRoot, "UICamera").GetComponent<Camera>();
            CanvasScaler = TransRoot.GetComponent<CanvasScaler>();
            GameObject.DontDestroyOnLoad(TransRoot.gameObject);

            AssemblyReflection();
        }

        public void Uninstall()
        {
            while (_stackCurrentUI.Count > 0)
            {
                IBaseUI ui = _stackCurrentUI.Pop();
                Close(ui.AssetsName);
            }

            if (_dictLoadedAllUIs != null)
            {
                var list = _dictLoadedAllUIs.ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                    Close(list[i].Key, true);
                list = null;
            }
            _allRegisterUIDict.Clear();
            _stackCurrentUI.Clear();
            _listCurrentPopupShowUIs.Clear();

            TransFixed = null;
            TransPopUp = null;
            TransRoot = null;
            TransNormal = null;
            TransGlobal = null;

            RectransGlobal = null;
            RectransFixed = null;
            RectransNormal = null;
            RectransPopUp = null;
            RectransRoot = null;

            _stackCurrentUI = null;
            _allRegisterUIDict = null;
            _dictLoadedAllUIs = null;
            _listCurrentPopupShowUIs = null;

            DelHideCallBack = null;
            LoadResourceFunc = null;

            Resources.UnloadUnusedAssets();
        }


        /// <summary>
        /// 隐退开始
        /// </summary>
        /// <param name="time"></param>
        /// <param name="callBack"></param>
        public virtual void ShowFade(Action callBack = null, float time = 0.4f)
        {
            if (_fadeImage == null || !_fadeImage.gameObject.activeInHierarchy)
            {
                if (callBack != null)
                    callBack.Invoke();
                return;
            }
            _fadeImage.raycastTarget = true;

            if (FadeStartAction != null) { FadeStartAction(time, callBack); return; }

            _fadeImage.CrossFadeAlpha(1, time, true);
            if (callBack != null)
                LitTool.LitTool.DelayPlayFunction(time, callBack);
        }


        /// <summary>
        /// 隐退结束
        /// </summary>
        /// <param name="time"></param>
        /// <param name="callBack"></param>
        public virtual void HideFade(Action callBack = null, float time = 0.4f)
        {
            if (_fadeImage == null || !_fadeImage.gameObject.activeInHierarchy)
            {
                if (callBack != null)
                    callBack.Invoke();
                return;
            }

            if (FadeHideAction == null)
            {
                _fadeImage.CrossFadeAlpha(0, time, true);
                if (callBack != null)
                    DelHideCallBack += callBack;
            }
            else
            {
                DelHideCallBack += () => FadeHideAction?.Invoke(time, callBack);
            }
            DelHideCallBack += () => _fadeImage.raycastTarget = false;
            DelHideCallBack += () => DelHideCallBack = null;

            LitTool.LitTool.DelayPlayFunction(time, DelHideCallBack);
        }


        /// <summary>
        /// 显示（打开）UI窗口
        /// 功能：
        /// 1、根据UI窗体的名称，加载到UI窗口缓存列表
        /// 2、根据不同UI显示模式，做不同的加载处理
        /// </summary>
        /// <param name="uiName">UI窗体预制件名称</param>
        public IBaseUI Show(string uiName, params object[] args)
        {
            IBaseUI baseUI = null;

            if (string.IsNullOrEmpty(uiName))
                throw new Exception("UI--uiName 为 Null");

            //根据名称加载窗体到UI窗体缓存中
            baseUI = LoadUIToAndFromAllList(uiName);
            if (baseUI == null)
                throw new Exception("UI--baseUI 加载失败");

            var modelType = UIModelBehavior.Instance.GetBehavior(uiName);
            UIType targetUIType = modelType != null ? modelType : baseUI.CurrentUIType;

            //判断是否清空“栈”结构体集合
            if (targetUIType.isClearPopUp)
                ClearPopUpStackArray();

            //只针对pop up 类型窗口适用 uiShowMode 功能
            if (targetUIType.uiNodeType == UINodeTypeEnum.PopUp)
            {
                switch (targetUIType.uiShowMode)
                {
                    case UIShowModeEnum.Parallel:
                        LoadParallelUI(uiName, args);
                        break;
                    case UIShowModeEnum.Stack:
                        LoadStackUI(uiName, args);
                        break;
                    case UIShowModeEnum.Unique:
                        LoadUniqueUI(uiName, args);
                        break;
                    default:
                        throw new Exception("未登记的UI类型--" + targetUIType.uiShowMode);
                }
            }
            else
            {
                //获取当前UI，进行展示处理
                _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);
                if (baseUI != null)
                    baseUI.Show(args: args);
            }

            //动画播放前界面刷新已完成，动画独立
            AnimationManager.Restart(baseUI.DotAnims, FrameworkConfig.Instance.OPENID, () => { if (baseUI.UseLowFrame) Application.targetFrameRate = FrameworkConfig.Instance.UI_LOW_FRAMERATE; });
            return baseUI;
        }

        /// <summary>
        /// 关闭指定UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="isDestroy">是否直接释放所有资源，销毁</param>
        /// <param name="useAnim">是否需要播放Dotween动画</param>
        public void Close(string uiName, bool isDestroy = false, bool useAnim = true, bool force = false)
        {
            if (string.IsNullOrEmpty(uiName))
                return;
            IBaseUI baseUI;
            //所有窗体如果没有记录，直接返回
            _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);
            if (baseUI == null)
            {
                _dictLoadedAllUIs.Remove(uiName);
                return;
            }

            if (!force && !baseUI.IsShowing) return;

            var modelType = UIModelBehavior.Instance.GetBehavior(uiName);
            UIType targetUIType = modelType != null ? modelType : baseUI.CurrentUIType;

            //参数目前没有用，可能会在未来版本移除
            baseUI.OnDisabled(false);

            Action innerFunc = () =>
            {
                //只针对pop up 类型窗口适用 uiShowMode 功能
                if (targetUIType.uiNodeType == UINodeTypeEnum.PopUp)
                {
                    //不同类型窗体执行各自关闭逻辑
                    switch (baseUI.CurrentUIType.uiShowMode)
                    {
                        case UIShowModeEnum.Parallel:
                            UnLoadParallelUI(uiName, isDestroy);
                            break;
                        case UIShowModeEnum.Stack:
                            UnLoadStackUI(uiName, isDestroy);
                            break;
                        case UIShowModeEnum.Unique:
                            UnLoadUniqueUI(uiName, isDestroy);
                            break;
                        default:
                            throw new Exception("未登记的UI类型--" + baseUI.CurrentUIType);
                    }
                }
                else
                {
                    baseUI.Close(isDestroy);
                }

                //销毁
                if (isDestroy)
                {
                    _dictLoadedAllUIs.Remove(uiName);
                }
            };

            if (useAnim) AnimationManager.Restart(baseUI.DotAnims, FrameworkConfig.Instance.CLOSEID, innerFunc);
            else innerFunc();

            Application.targetFrameRate = FrameworkConfig.Instance.TargetFrameRate;

            Log.TraceInfo($"   UIClose: {baseUI.AssetsName}");
        }



        /// <summary>
        /// 清空弹出窗口栈
        /// </summary>
        public bool ClearPopUpStackArray()
        {
            if (_stackCurrentUI != null && _stackCurrentUI.Count > 0)
            {
                while (_stackCurrentUI.Count > 0)
                {
                    var stackUI = _stackCurrentUI.Pop();
                    Close(stackUI.AssetsName, useAnim: false);
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
        private IBaseUI LoadUIToAndFromAllList(string uiName)
        {
            IBaseUI result = null;
            _dictLoadedAllUIs.TryGetValue(uiName, out result);
            if (result == null)
                result = (IBaseUI)LoadUI(uiName);
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
        private IBaseUI LoadUI(string uiName)
        {
            //加载的UI预制体
            IBaseUI baseUI = null;
            GameObject prefClone = null;

            //加载预制体
            if (!string.IsNullOrEmpty(uiName))
                prefClone = LoadResourceFunc != null ? LoadResourceFunc(uiName) : null;
            if (prefClone == null)
                throw new Exception("未指定UI预制件加载方法或UI预制件路径指定错误 ==>" + uiName);
            prefClone = GameObject.Instantiate(prefClone);

            var monoType = typeof(MonoBehaviour);
            //设置父节点
            if (TransRoot != null && prefClone != null)
            {
                if (_allRegisterUIDict.ContainsKey(uiName))
                {
                    object baseUIOri = null;
                    //获取Unity编辑器程序集
                    var assembly = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
                    if (FrameworkConfig.Instance.scriptEnvironment == RunEnvironment.DotNet || uiName.IndexOf("Canvas_Loading") != -1)
                    {
                        Log.TraceInfo(">>>>>UI Load Search Assembly " + assembly.FullName + "  :  " + _allRegisterUIDict[uiName]);
                        baseUIOri = assembly.CreateInstance(_allRegisterUIDict[uiName], true);
                    }
                    else if (FrameworkConfig.Instance.scriptEnvironment == RunEnvironment.ILRuntime)
                    {
                        //获取热更工程程序集
                        //借由反射现成方法，调取生成ILR内部实例，并返回结果
                        Log.TraceInfo(">>>> RunEnvironment.ILRuntime " + assembly.FullName);
                        var ssstype = assembly.GetType("Assets.Scripts.ILRScriptCall");
                        baseUIOri = ssstype.GetMethod("GetUITypeByThis").Invoke(null, new object[1] { _allRegisterUIDict[uiName] });
                        if (baseUIOri == null)
                        {
                            Log.TraceInfo(">>>>>UI Load Search Assembly " + assembly.FullName + "  :  " + _allRegisterUIDict[uiName]);
                            baseUIOri = assembly.CreateInstance(_allRegisterUIDict[uiName], true);
                        }
                    }
                    //========================//
                    if (baseUIOri.GetType().IsSubclassOf(monoType))
                        baseUI = prefClone.GetComponent<IBaseUI>();
                    else
                        baseUI = (IBaseUI)baseUIOri;

                    if (baseUI == null)
                    { LDebug.LogError(uiName + "UI 脚本加载失败"); return null; }

                    baseUI.CallCtor();
                    baseUI.GameObjectInstance = prefClone;
                    baseUI.AssetsName = uiName;
                    baseUI.IsInitOver = true;
                    //========================//
                }

                if (baseUI == null)
                { LDebug.LogError(uiName + "UI 脚本加载失败"); return null; }

                baseUI.Initialize();
                baseUI.OnAwake();

                switch (baseUI.CurrentUIType.uiNodeType)
                {
                    case UINodeTypeEnum.Normal:
                        prefClone.transform.SetParent(TransNormal, false);
                        break;
                    case UINodeTypeEnum.Fixed:
                        prefClone.transform.SetParent(TransFixed, false);
                        break;
                    case UINodeTypeEnum.PopUp:
                        prefClone.transform.SetParent(TransPopUp, false);
                        break;
                    case UINodeTypeEnum.Global:
                        prefClone.transform.SetParent(TransGlobal, false);
                        break;
                    default:
                        throw new Exception("未登记的UI类型--" + baseUI.CurrentUIType.uiShowMode);
                }

                baseUI.OnAdapter();

                //加入到所有窗体缓存中
                _dictLoadedAllUIs.Add(uiName, (IBaseUI)baseUI);
                return baseUI;
            }
            return null;
        }

        /// <summary>
        /// 加载当前窗体到当前窗体集合
        /// </summary>
        /// <param name="uiName"></param>
        private void LoadParallelUI(string uiName, params object[] args)
        {
            IBaseUI baseUI;

            //判断栈里是否有窗口，有则冻结响应
            if (_stackCurrentUI.Count > 0)
            {
                IBaseUI topUI = _stackCurrentUI.Peek();
                if (!topUI.AssetsName.Equals(uiName))
                    topUI.OnDisabled(true);
            }

            if (_listCurrentPopupShowUIs.Exists(e => e.AssetsName.Equals(uiName)))
            {
                baseUI = _listCurrentPopupShowUIs.Where(e => e.AssetsName.Equals(uiName)).First();
                baseUI.Show(args: args);
                baseUI.CheckMask();

                _listCurrentPopupShowUIs.Remove(baseUI);
                _listCurrentPopupShowUIs.Add(baseUI);
                return;
            }

            //加载当前窗体到当前显示集合
            _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);
            if (baseUI != null)
            {
                _listCurrentPopupShowUIs.Add(baseUI);
                baseUI.Show(args: args);
            }
        }

        /// <summary>
        /// 从当前UI列表缓存中卸载UI窗体
        /// </summary>
        /// <param name="uiName"></param>
        private void UnLoadParallelUI(string uiName, bool isDestroy = false)
        {
            IBaseUI baseUI;

            //当前UI显示列表中没有记录或者总表中没有记录则直接返回
            if (!_listCurrentPopupShowUIs.Exists(e => e.AssetsName.Equals(uiName)))
            {
                _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);
                if (baseUI == null)
                    return;
            }
            else
            {
                baseUI = _listCurrentPopupShowUIs.Where(e => e.AssetsName.Equals(uiName)).First();
                //隐藏窗口并从列表中移除
                _listCurrentPopupShowUIs.Remove(baseUI);
            }

            baseUI.Close(isDestroy: isDestroy);

            //如果需要清空已有 popup 窗口
            if (baseUI.CurrentUIType.isClearPopUp)
                ClearPopUpStackArray();

            //if (baseUI.CurrentUIType.uiNodeType == UINodeTypeEnum.PopUp)
                CheckCurrentUIMask();
        }

        /// <summary>
        /// 加载独占UI窗体
        /// </summary>
        /// <param name="uiName"></param>
        private void LoadUniqueUI(string uiName, params object[] args)
        {
            IBaseUI baseUI;
            //当前UI显示列表中没有记录则直接返回
            //.TryGetValue(uiName, out baseUI);
            if (_listCurrentPopupShowUIs.Exists(e => e.AssetsName.Equals(uiName)))
            {
                baseUI = _listCurrentPopupShowUIs.Where(e => e.AssetsName.Equals(uiName)).First();
                baseUI.Show(args: args);
                baseUI.CheckMask();

                _listCurrentPopupShowUIs.Remove(baseUI);
                _listCurrentPopupShowUIs.Add(baseUI);
                return;
            }

            //正在显示的UI进行隐藏
            var toCloseList = _listCurrentPopupShowUIs;
            for (int i = toCloseList.Count - 1; i > -1; i--)
                Close(toCloseList[i].AssetsName, useAnim: false);
            
            //把当前窗体加载到正显示的UI窗口缓存中去
            _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);
            if (baseUI != null)
            {
                _listCurrentPopupShowUIs.Add(baseUI);
                baseUI.Show(args: args);
            }

        }

        /// <summary>
        /// 卸载当前UI，并将原先被隐藏的UI重新显示
        /// </summary>
        /// <param name="uiName"></param>
        private void UnLoadUniqueUI(string uiName, bool isDestroy = false)
        {
            IBaseUI baseUI;

            //_dictCurrentShowUIs.TryGetValue(uiName, out baseUI);
            if (!_listCurrentPopupShowUIs.Exists(e => e.AssetsName.Equals(uiName)))
            {
                if (!isDestroy)
                    return;
                else
                    _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);

                if (baseUI == null)
                    return;
            }
            else
            {
                baseUI = _listCurrentPopupShowUIs.Where(e => e.AssetsName.Equals(uiName)).First();
                //隐藏窗口并从列表中移除
                _listCurrentPopupShowUIs.Remove(baseUI);
            }

            baseUI.Close(isDestroy: isDestroy);

            //如果需要清空已有 popup 窗口
            if (baseUI.CurrentUIType.isClearPopUp)
                ClearPopUpStackArray();

            CheckCurrentUIMask();
        }

        /// <summary>
        /// 弹出窗口，入栈
        /// 先冻结栈中窗口，再将此窗口入栈
        /// </summary>
        /// <param name="uiName"></param>
        private void LoadStackUI(string uiName, params object[] args)
        {
            IBaseUI baseUI;

            //判断栈里是否有窗口，有则冻结响应
            if (_stackCurrentUI.Count > 0)
            {
                IBaseUI topUI = _stackCurrentUI.Peek();
                if (!topUI.AssetsName.Equals(uiName))
                {
                    //直接通过Close调用会引起stack重复关闭
                    topUI.OnDisabled(true);
                    topUI.Close(freeze: true);
                }
            }

            //获取当前UI，进行展示处理
            _dictLoadedAllUIs.TryGetValue(uiName, out baseUI);
            if (baseUI != null)
            {
                baseUI.Show(args: args);

                if (!_stackCurrentUI.Contains(baseUI)) //该弹出UI入栈
                {
                    _stackCurrentUI.Push(baseUI);
                    _listCurrentPopupShowUIs.Add(baseUI);
                }
                else //对于栈内存在，则将其提升至栈顶
                {
                    while (_stackCurrentUI.Count > 0)
                    {
                        var ui = _stackCurrentUI.Pop();
                        if (ui != baseUI) _backStack.Push(ui);
                    }
                    while (_backStack.Count > 0)
                    {
                        _stackCurrentUI.Push(_backStack.Pop());
                    }
                    _stackCurrentUI.Push(baseUI);
                    //操作记录：移动到最顶
                    _listCurrentPopupShowUIs.Remove(baseUI);
                    _listCurrentPopupShowUIs.Add(baseUI);
                }
                baseUI.CheckMask();
            }
            else
                throw new Exception($"UIManager LoadStackUI {uiName} 不存在 ");
        }
        private Stack<IBaseUI> _backStack = new Stack<IBaseUI>();

        /// <summary>
        /// 弹出窗口，出栈
        /// </summary>
        /// <param name="uiName"></param>
        private void UnLoadStackUI(string uiName, bool isDestroy = false)
        {
            IBaseUI topUI = null;
            //有两个以上弹窗出现时
            if (_stackCurrentUI.Count >= 2)
            {
                //第一个出栈
                topUI = _stackCurrentUI.Pop();
                topUI.Close(isDestroy: isDestroy);

                //第二个重新显示
                IBaseUI nextUI = _stackCurrentUI.Peek();
                nextUI.Show(true);
                AnimationManager.Restart(nextUI.DotAnims, FrameworkConfig.Instance.OPENID, () => { if (nextUI.UseLowFrame) Application.targetFrameRate = FrameworkConfig.Instance.UI_LOW_FRAMERATE; });
            }
            //当前只有一个弹窗
            else if (_stackCurrentUI.Count == 1)
            {
                //出栈的窗体进行隐藏
                topUI = _stackCurrentUI.Pop();
                topUI.Close(isDestroy: isDestroy);
            }
            _listCurrentPopupShowUIs.Remove(topUI);

            CheckCurrentUIMask();
        }

        /// <summary>
        /// 从现有缓存中查找目标UI，未加载则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IBaseUI GetUIByName(string name)
        {
            IBaseUI baseUI;
            _dictLoadedAllUIs.TryGetValue(name, out baseUI);
            return baseUI;
        }

        /// <summary>
        /// 关闭所有UI
        /// </summary>
        /// <param name="force">是否关闭带有FixedFlag的UI</param>
        public void CloseAll(bool force = false, bool isDestroy = false, bool useAnim = true)
        {
            var toCloseUI = _dictLoadedAllUIs.Where(e => !force ? e.Value.Flag != UIFlag.Fix : (e.Value is IBaseUI)).Select(e => e.Value).ToList();
            foreach (var item in toCloseUI)
                Close(item.AssetsName, isDestroy, useAnim, force);
        }


        /// <summary>
        /// 返回键执行关闭窗口操作
        /// </summary>
        public void OnEscapeCallback()
        {
            if (_stackCurrentUI.Count > 0)
            {
                var baseUi = _stackCurrentUI.Peek();
                Close(baseUi.AssetsName);
                return;
            }
        }

        private void CheckCurrentUIMask()
        {
            for (int i = 0; i < _listCurrentPopupShowUIs.Count; i++)
            {
                if (_listCurrentPopupShowUIs[i].IsShowing) _listCurrentPopupShowUIs[i].CheckMask();
            }
        }

        #region 反射方法，用于热更时UI绑定

        /// <summary>
        /// 反射注册UI回调
        /// </summary>
        /// <param name="_assetsName"></param>
        /// <param name="_className"></param>
        public void RegistFunctionCallFun(string uiPathName, string className)
        {
            if (!String.IsNullOrEmpty(uiPathName) && !_allRegisterUIDict.ContainsKey(uiPathName))
                _allRegisterUIDict.Add(uiPathName, className);

            Log.TraceInfo("LitFramework UI添加成功 " + uiPathName);
        }

        private void AssemblyReflection()
        {
            //System.Reflection.Assembly asb = System.Reflection.Assembly.GetExecutingAssembly();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                var asb = assemblies[i];
                System.Type[] assemblyTypes = asb.GetTypes();
                System.Type interfaceType = typeof(IBaseUI);
                bool isDotEnv = FrameworkConfig.Instance.scriptEnvironment == RunEnvironment.DotNet;

                for (int indexType = 0; indexType < assemblyTypes.Length; indexType++)
                {
                    if (!assemblyTypes[indexType].IsAbstract && interfaceType.IsAssignableFrom(assemblyTypes[indexType]))
                    {
                        //if (isDotEnv || (!isDotEnv && assemblyTypes[indexType].Name.Equals("UILoading")))
                        {
                            //通过程序集获取到他的返回实例对象方法  并且初始化对象
                            System.Reflection.MethodInfo mif = assemblyTypes[indexType].GetMethod("RegistSystem");
                            if (mif != null)
                            {
                                //目前只认静态方法
                                if (mif.IsStatic)
                                    mif.Invoke(null, new object[] { assemblyTypes[indexType].Namespace + "." + assemblyTypes[indexType].Name });
                            }
                        }
                    }
                }
            }


        }
        #endregion

    }
}