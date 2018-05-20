/**************************************************************** 
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
 * Copyright @ Derek 2018 All rights reserved 
*****************************************************************/

using LitFramework;
using UnityEngine;
using UnityEngine.UI;

public class UIMaskManager : SingletonMono<UIMaskManager> 
{
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

    private void Awake( )
    {
        //得到UI根节点对象
        _rootCanvas = GameObject.FindGameObjectWithTag( UISysDefine.SYS_TAG_ROOTCANVAS );
        _transScriptNode = UnityHelper.FindTheChildNode( _rootCanvas , UISysDefine.SYS_TAG_MANAGERCANVAS );
        //将本脚本实例作为脚本节点对象子节点
        transform.SetParent( _transScriptNode );
        UnityHelper.AddChildNodeToParentNode( _transScriptNode , transform );
        //顶层面板、遮罩面板
        _topPanel = _rootCanvas;
        _maskPanel = UnityHelper.FindTheChildNode( _rootCanvas , "Panel_Mask" ).gameObject;
        _maskImage = _maskPanel.GetComponent<Image>();
        //获得摄像机层深
        _uiCamera = GameObject.FindGameObjectWithTag( UISysDefine.SYS_TAG_UICAMERA ).GetComponent<Camera>();
        if ( _uiCamera != null ) _oriUICameraDepth = _uiCamera.depth;
    }

    /// <summary>
    /// 设置遮罩状态
    /// </summary>
    public void SetMaskWindow( GameObject displayUIForms, UITransparentEnum transparent = UITransparentEnum.NoPenetratingTotal )
    {
        //顶层窗体下移
        _topPanel.transform.SetAsLastSibling();

        switch ( transparent )
        {
            case UITransparentEnum.NoPenetratingLow:
                _maskPanel.SetActive( true );
                _maskImage.color = new Color( 50 / 255F , 50 / 255F , 50 / 255F , 200F / 255F );
                break;
                //半透明
            case UITransparentEnum.NoPenetratingMiddle:
                _maskPanel.SetActive( true );
                _maskImage.color = new Color( 220 / 255F , 220 / 255F , 220 / 255F , 50F / 255F );
                break;
                //完全透明
            case UITransparentEnum.NoPenetratingTotal:
                _maskPanel.SetActive( true );
                _maskImage.color = new Color( 255 / 255F , 255 / 255F , 255 / 255F , 0F / 255F );
                break;
                //可以穿透
            case UITransparentEnum.Penetrating:
                if ( _maskPanel.activeInHierarchy ) _maskPanel.SetActive( false );
                break;
        }

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
        if ( _maskPanel.activeInHierarchy )
            _maskPanel.SetActive( false );
        //恢复UI相机层深
        if ( _uiCamera != null )
            _uiCamera.depth = _oriUICameraDepth;
    }
}
