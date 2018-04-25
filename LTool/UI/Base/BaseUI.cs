using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// 该窗口是否开启中
    /// </summary>
    public bool IsShowing { get; set; }
    /// <summary>
    /// 当前窗口类型
    /// </summary>
    private UIType _uiType = new UIType();
    public UIType CurrentUIType
    { get { return _uiType; } set { _uiType = value; } }

    /// <summary>
    /// 显示窗体
    /// </summary>
    /// <param name="replay">是否是重新显示</param>
    public virtual void Show( bool replay = false )
    {
        IsShowing = true;
        this.enabled = IsShowing;
        gameObject.SetActive( IsShowing );
        //设置模态窗体调用(弹出窗体)
        if( CurrentUIType.uiType == UITypeEnum.PopUp )
            UIMaskManager.Instance.SetMaskWindow( gameObject , CurrentUIType.uiTransparent );
        Refresh();
    }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    /// <param name="freeze">是否暂时冻结（功能未想好）</param>
    public virtual void Close( bool freeze = false )
    {
        if( !freeze )
        {
            gameObject.SetActive( false );

            if( CurrentUIType.uiType == UITypeEnum.PopUp && IsShowing )
                UIMaskManager.Instance.CancelMaskWindow();
        }
        else
        {
            //TODO 对于处于冻结的UI，可能需要断开该窗口的网络通信或者操作、刷新响应等操作
            gameObject.SetActive( false );
        }

        IsShowing = false;
    }

    /// <summary>
    /// 刷新窗体
    /// </summary>
    public virtual void Refresh() { }

    public virtual void OnAdapter() { }

    public virtual void DoDestroy()
    {
        OnDestroy();
    }

    public virtual void OnDispose() { }

    #region Alternative Function

    public abstract void OnAwake();

    public abstract void OnEnabled();

    public abstract void OnDisabled();

    public virtual void OnStart() { }



    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void OnEnable()
    {
        OnEnabled();
    }

    private void OnDisable()
    {
        OnDisabled();
    }

    private void OnDestroy()
    {
        Close();
        OnDispose();
        GameObject.Destroy( gameObject );
    }
    #endregion
}
