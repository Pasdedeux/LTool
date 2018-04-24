using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTool;
using System.Reflection;

/// <summary>
/// 以下UI类，需配合UImanager.unitypackage使用。
/// 
/// 主要包含Cavas_Root及相关Tag等
/// 
/// </summary>
public class UIManager : Singleton<UIManager>
{
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
    private Dictionary<string , BaseUI> _dictOpenedAllUIs;
    /// <summary>
    /// 当前显示的UI窗体
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
    private Transform _transManager = null;
    /// <summary>
    /// 外部传入UI的加载方法。Resource.Load || AssetBundle.Load
    /// </summary>
    public Func<string , GameObject> GetUIResource;



    public void Install( Func<string , GameObject> loadPrefabFunction )
    {
        _allRegisterUIList = new List<string>();
        _stackCurrentUI = new Stack<BaseUI>();
        _dictOpenedAllUIs = new Dictionary<string , BaseUI>();
        _dictCurrentShowUIs = new Dictionary<string , BaseUI>();

        _transCanvas = GameObject.FindGameObjectWithTag( SysDefine.SYS_TAG_ROOTCANVAS ).transform;
        _transNormal = UnityHelper.FindTheChildNode( _transCanvas.gameObject , SysDefine.SYS_TAG_NORMALCANVAS );
        _transFixed = UnityHelper.FindTheChildNode( _transCanvas.gameObject , SysDefine.SYS_TAG_FIXEDCANVAS );
        _transPopUp = UnityHelper.FindTheChildNode( _transCanvas.gameObject , SysDefine.SYS_TAG_POPUPCANVAS );
        _transManager = UnityHelper.FindTheChildNode( _transCanvas.gameObject , SysDefine.SYS_TAG_MANAGERCANVAS );

        GetUIResource = loadPrefabFunction;
        Reflection();
    }

    private void Reflection()
    {
        System.Reflection.Assembly asb = System.Reflection.Assembly.GetExecutingAssembly();

        System.Type[] assemblyTypes = asb.GetTypes();

        for( int indexType = 0; indexType < assemblyTypes.Length; indexType++ )
        {
            if( !assemblyTypes[ indexType ].IsAbstract && assemblyTypes[ indexType ].BaseType == typeof( BaseUI ) )
            {
                //通过程序集获取到他的返回实例对象方法  并且初始化对象
                System.Reflection.MethodInfo mif = assemblyTypes[ indexType ].GetMethod( "RegistSystem" );
                if( mif != null )
                {
                    //目前只认静态方法
                    if( mif.IsStatic )
                        mif.Invoke( null , null );
                    else
                    {
                        //TODO 这一部分还没写好
                        ConstructorInfo magicConstructor = assemblyTypes[ indexType ].GetConstructor( Type.EmptyTypes );
                        object magicClassObject = magicConstructor.Invoke( new object[] { } );
                        mif.Invoke( magicClassObject , new object[] { assemblyTypes[ indexType ].Name } );
                    }
                }
            }
        }
    }

    public void Uninstall()
    {
        foreach( var item in _dictOpenedAllUIs )
        {
            var baseui = item.Value;
            baseui.DoDestroy();
        }
        _allRegisterUIList.Clear();
        _stackCurrentUI.Clear();
        _dictCurrentShowUIs.Clear();
        _dictOpenedAllUIs.Clear();

        _transFixed = null;
        _transPopUp = null;
        _transCanvas = null;
        _transNormal = null;
        _transManager = null;
        _stackCurrentUI = null;
        _allRegisterUIList = null;
        _dictOpenedAllUIs = null;
        _dictCurrentShowUIs = null;

        GetUIResource = null;
    }


    /// <summary>
    /// 反射注册UI回调
    /// </summary>
    /// <param name="_assetsName"></param>
    /// <param name="_className"></param>
    public void RegistFunctionCallFun( string uiPathName )
    {
        if( !String.IsNullOrEmpty( uiPathName ) && !_allRegisterUIList.Contains( uiPathName ) )
            _allRegisterUIList.Add( uiPathName );

        Debug.LogError( "LTOOL UI添加成功 " + uiPathName );
    }



    /// <summary>
    /// 显示（打开）UI窗口
    /// 功能：
    /// 1、根据UI窗体的名称，加载到UI窗口缓存列表
    /// 2、根据不同UI显示模式，做不同的加载处理
    /// </summary>
    /// <param name="uiName">UI窗体预制件名称</param>
    public BaseUI Show( string uiName )
    {
        if( !_allRegisterUIList.Contains( uiName ) )
            throw new Exception( "-->UI " + uiName + "并未注册" );

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
            case UIShowModeEnum.Normal:
                LoadUIToCurrentCache( uiName );
                break;
            case UIShowModeEnum.PopUp:
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
    public void Close( string uiName )
    {
        if( string.IsNullOrEmpty( uiName ) )
            return;

        BaseUI baseUI = null;

        //所有窗体如果没有记录，直接返回
        _dictOpenedAllUIs.TryGetValue( uiName , out baseUI );
        if( baseUI == null )
            return;

        //不同类型窗体执行各自关闭逻辑
        switch( baseUI.CurrentUIType.uiShowMode )
        {
            case UIShowModeEnum.Normal:
                UnLoadUIToCurrentCache( uiName );
                break;
            case UIShowModeEnum.PopUp:
                UnLoadPopUpUI( uiName );
                break;
            case UIShowModeEnum.Unique:
                UnLoadUniqueUI( uiName );
                break;
            default:
                throw new Exception( "未登记的UI类型--" + baseUI.CurrentUIType.uiShowMode );
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
                stackUI.Hide();
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
        _dictOpenedAllUIs.TryGetValue( uiName , out result );
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
            prefClone = GetUIResource != null ? GameObject.Instantiate( GetUIResource( uiName ) ) : null;
        if( prefClone == null )
            throw new Exception( "未指定UI预制件加载方法或UI预制件路径指定错误 ==>" + uiName );

        //设置父节点
        if( _transCanvas != null && prefClone != null )
        {
            baseUI = prefClone.GetComponent<BaseUI>();

            if( baseUI == null )
            { Debug.LogError( uiName + "UI 脚本加载失败" ); return null; }

            switch( baseUI.CurrentUIType.uiType )
            {
                case UITypeEnum.Normal:
                    prefClone.transform.SetParent( _transNormal , false );
                    break;
                case UITypeEnum.Fixed:
                    prefClone.transform.SetParent( _transFixed , false );
                    break;
                case UITypeEnum.PopUp:
                    prefClone.transform.SetParent( _transPopUp , false );
                    break;
                default:
                    throw new Exception( "未登记的UI类型--" + baseUI.CurrentUIType.uiShowMode );
            }

            prefClone.SetActive( false );
            //加入到所有窗体缓存中
            _dictOpenedAllUIs.Add( uiName , baseUI );
            return baseUI;
        }

        Debug.Log( "窗体加载失败！" );
        return null;
    }


    /// <summary>
    /// 加载当前窗体到当前窗体集合
    /// </summary>
    /// <param name="uiName"></param>
    private void LoadUIToCurrentCache( string uiName )
    {
        BaseUI baseUI;
        _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
        if( baseUI != null )
            return;

        //加载当前窗体到当前显示集合
        _dictOpenedAllUIs.TryGetValue( uiName , out baseUI );
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
    private void UnLoadUIToCurrentCache( string uiName )
    {
        BaseUI baseUI;
        //当前UI显示列表中没有记录则直接返回
        _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
        if( baseUI == null )
            return;

        //隐藏窗口并从列表中移除
        baseUI.Hide();
        _dictCurrentShowUIs.Remove( uiName );
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
            return;

        //正在显示的UI进行隐藏
        foreach( BaseUI baseui in _dictCurrentShowUIs.Values )
        {
            baseui.Hide();
        }
        foreach( BaseUI baseui in _stackCurrentUI )
        {
            baseui.Hide();
        }

        //把当前窗体加载到正显示的UI窗口缓存中去
        _dictOpenedAllUIs.TryGetValue( uiName , out baseUI );
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
    private void UnLoadUniqueUI( string uiName )
    {
        BaseUI baseUI;

        _dictCurrentShowUIs.TryGetValue( uiName , out baseUI );
        if( baseUI == null )
            return;

        //指定窗口隐藏
        baseUI.Hide();
        _dictCurrentShowUIs.Remove( uiName );

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
            topUI.Hide( true );
        }

        //获取当前UI，进行展示处理
        _dictOpenedAllUIs.TryGetValue( uiName , out baseUI );
        if( baseUI != null )
            baseUI.Show();
        else
            throw new Exception( "UIManager catch an error" );

        //该弹出UI入栈
        _stackCurrentUI.Push( baseUI );
    }

    /// <summary>
    /// 弹出窗口，出栈
    /// </summary>
    /// <param name="uiName"></param>
    private void UnLoadPopUpUI( string uiName )
    {
        //有两个以上弹窗出现时
        if( _stackCurrentUI.Count >= 2 )
        {
            //第一个出栈
            BaseUI topUI = _stackCurrentUI.Pop();
            topUI.Hide();

            //第二个重新显示
            BaseUI nextUI = _stackCurrentUI.Peek();
            nextUI.Show( true );
        }
        //当前只有一个弹窗
        else if( _stackCurrentUI.Count == 1 )
        {
            //出栈的窗体进行隐藏
            BaseUI topUI = _stackCurrentUI.Pop();
            topUI.Hide();
        }
    }


    [Obsolete( "这个方法暂时不要用" , true )]
    /// <summary>
    /// 加载UI根容器预制件
    /// </summary>
    private void InitRootCanvasLoading()
    {
        if( GameObject.FindGameObjectWithTag( SysDefine.SYS_TAG_ROOTCANVAS ) == null )
        {
            ResourceManager.Instance.LoadAssets( @"UI\" + SysDefine.SYS_TAG_ROOTCANVAS , false );
        }
    }

}
