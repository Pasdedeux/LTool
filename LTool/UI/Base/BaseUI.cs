using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{

    private UIType _currentUIType = new UIType();
    /// <summary>
    /// 当前窗口类型
    /// </summary>
    public UIType CurrentUIType
    {
        get { return _currentUIType; }
        set { _currentUIType = value; }
    }

    /// <summary>
    /// 显示窗体
    /// </summary>
    /// <param name="replay">是否是重新显示</param>
    public virtual void Show( bool replay = false )
    {
        gameObject.SetActive( true );
        //设置模态窗体调用(弹出窗体)
        if ( _currentUIType.uiType == UITypeEnum.PopUp )
            UIMaskManager.Instance.SetMaskWindow( gameObject , _currentUIType.uiTransparent );
        Update();
    }


    /// <summary>
    /// 刷新窗体
    /// </summary>
    /// <param name="replay">是否是重新显示</param>
    public virtual void Update() { }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    /// <param name="freeze">是否暂时冻结（仍在栈中）</param>
    public virtual void Hide( bool freeze = false )
    {
        if ( !freeze )
        {
            gameObject.SetActive( false );
            if ( _currentUIType.uiType == UITypeEnum.PopUp )
                UIMaskManager.Instance.CancelMaskWindow();
        }
        else
        {
            //对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
            gameObject.SetActive( true );
        }
    }

}
