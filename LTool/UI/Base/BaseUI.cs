using UnityEngine;


public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// 当前窗口类型
    /// </summary>
    /// 
    private UIType _uiType = new UIType();
    public UIType CurrentUIType
    { get { return _uiType; } set { _uiType = value; } }

    /// <summary>
    /// 显示窗体
    /// </summary>
    /// <param name="replay">是否是重新显示</param>
    public virtual void Show( bool replay = false )
    {
        gameObject.SetActive( true );
        //设置模态窗体调用(弹出窗体)
        if( CurrentUIType.uiType == UITypeEnum.PopUp )
            UIMaskManager.Instance.SetMaskWindow( gameObject , CurrentUIType.uiTransparent );
    }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    /// <param name="freeze">是否暂时冻结（功能未想好）</param>
    public virtual void Hide( bool freeze = false )
    {
        if( !freeze )
        {
            gameObject.SetActive( false );
            if( CurrentUIType.uiType == UITypeEnum.PopUp )
                UIMaskManager.Instance.CancelMaskWindow();
        }
        else
        {
            //TODO 对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
            gameObject.SetActive( false );
        }
    }


    public virtual void DoDestroy()
    {
        GameObject.Destroy( gameObject );
    }

}
