﻿/**************************************************************** 
 * 作    者：Derek Liu 
 * CLR 版本：4.0.30319.42000 
 * 创建时间：2018/1/31 15:48:18 
 * 当前版本：1.0.0.1 
 *  
 * 描述说明： 
 * 
 * 修改历史： 
 * 
***************************************************************** 
 * Copyright @ Derek Liu 2018 All rights reserved 
*****************************************************************/

using LitFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 遮罩控制器。用于控制Image_Mask显示行为
/// </summary>
public class UIMaskManager : SingletonMono<UIMaskManager>
{
    /// <summary>
    /// 当前Image_Maskm蒙版的Image对象
    /// </summary>
    public Image Mask { get { return _maskImage; } }
    /// <summary>
    /// 背景遮罩启用/关闭事件。接收参数bool表明是否启用。
    /// 目前发生时机，是开关PopUp弹窗时，各会触发一次
    /// </summary>
    public Action<bool> MaskEnableEventHandler;
    //UI脚本节点对象
    private Transform _transScriptNode = null;
    //UI根节点对象
    private GameObject _rootCanvas = null;
    //顶层面板
    private GameObject _topPanel;
    //遮罩面板
    private GameObject _maskPanel;
    //遮罩颜色
    private Image _maskImage;
    //UI相机
    private Camera _uiCamera;
    //UI相机原始层深度
    private float _oriUICameraDepth;
    //全局按钮
    private Button _maskBtn;
    private static Color _color = new Color( 0 / 255F, 0 / 255F, 0 / 255F, 0 / 255F );

    private void Awake()
    {
        //得到UI根节点对象
        _rootCanvas = GameObject.FindGameObjectWithTag( UISysDefine.SYS_TAG_ROOTCANVAS );
        _transScriptNode = UnityHelper.FindTheChildNode( _rootCanvas.transform, UISysDefine.SYS_TAG_GLOBALCANVAS );
        //将本脚本实例作为脚本节点对象子节点
        transform.SetParent( _transScriptNode );
        UnityHelper.AddChildNodeToParentNode( _transScriptNode, transform );
        //顶层面板、遮罩面板
        _topPanel = _rootCanvas;
        _maskPanel = UnityHelper.FindTheChildNode( _rootCanvas.transform, "Panel_Mask" ).gameObject;
        _maskPanel.SetActive( true );
        _maskImage = _maskPanel.GetComponent<Image>();
        _maskImage.enabled = false;
        //_maskBtn = _maskPanel.GetComponent<Button>();
        //_maskBtn.enabled = false;
        //获得摄像机层深
        _uiCamera = GameObject.FindGameObjectWithTag( UISysDefine.SYS_TAG_UICAMERA ).GetComponent<Camera>();
        if ( _uiCamera != null ) _oriUICameraDepth = _uiCamera.depth;
    }

    /// <summary>
    /// 根据传入枚举类型，激活蒙版并完成对应显示状态
    /// </summary>
    /// <param name="transparent">透明度级别</param>
    public void SetMaskEnable( UITransparentEnum transparent = UITransparentEnum.NoPenetratingTotal )
    {
        switch ( transparent )
        {
            case UITransparentEnum.NoPenetratingLow:
                //_maskPanel.SetActive( true );
                _maskImage.enabled = true;
                _color.a = 255F / 255F;
                _maskImage.color = _color;
                _maskImage.raycastTarget = true;
                break;
            //半透明
            case UITransparentEnum.NoPenetratingMiddle:
                //_maskPanel.SetActive( true );
                _maskImage.enabled = true;
                _color.a = 175F / 255F;
                _maskImage.color = _color;
                _maskImage.raycastTarget = true;
                break;
            //完全透明
            case UITransparentEnum.NoPenetratingTotal:
                //_maskPanel.SetActive( true );
                _maskImage.enabled = true;
                _color.a = 0F / 255F;
                _maskImage.color = _color;
                _maskImage.raycastTarget = true;
                break;
            //可以穿透
            case UITransparentEnum.Penetrating:
                if ( _maskImage.enabled/*_maskPanel.activeInHierarchy*/ ) _maskImage.enabled = false;/* _maskPanel.SetActive( false )*/;
                _maskImage.raycastTarget = false;
                break;
        }
    }

    /// <summary>
    /// 设置指定UI的遮罩状态
    /// </summary>
    /// <param name="displayUIForms">需要为其服务的UIGameObject，比如UIMain所挂载的对象</param>
    /// <param name="transparent">透明度级别</param>
    public void SetMaskWindow( GameObject displayUIForms, UITransparentEnum transparent = UITransparentEnum.NoPenetratingTotal )
    {
        //顶层窗体下移
        _topPanel.transform.SetAsLastSibling();
        //开启并设定遮罩级别
        SetMaskEnable( transparent );
        //推送消息
        MaskEnableEventHandler?.Invoke( _maskImage.enabled );

        //遮罩窗体下移
        _maskPanel.transform.SetAsLastSibling();
        //显示窗体的下移
        displayUIForms.transform.SetAsLastSibling();
        //增加当前UI摄像机的层深，保证当前摄像机为最前显示
        if ( _uiCamera != null )
            _uiCamera.depth += 100;
    }

    /// <summary>
    /// 取消遮罩
    /// </summary>
    public void CancelMaskWindow()
    {
        //顶层窗体上移
        _topPanel.transform.SetAsFirstSibling();
        //禁用遮罩窗体
        //if ( _maskPanel.activeInHierarchy )
        //    _maskPanel.SetActive( false );
        if ( _maskImage.enabled )
            _maskImage.enabled = false;
        _maskImage.raycastTarget = false;
        MaskEnableEventHandler?.Invoke( _maskImage.enabled );
        //恢复UI相机层深
        if ( _uiCamera != null )
            _uiCamera.depth = _oriUICameraDepth;
    }

    /// <summary>
    /// 设置Mask Image蒙版的颜色。不会影响Alpha值
    /// </summary>
    /// <param name="color">对应颜色。可通过ColorUtility.TryParseHtmlString转码获得 </param>
    public void SetMaskColor( Color color )
    {
        _color.r = color.r;
        _color.g = color.g;
        _color.b = color.b;
    }

}
