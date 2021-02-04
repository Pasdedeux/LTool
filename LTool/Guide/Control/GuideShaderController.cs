using DG.Tweening;
using LitFramework;
using LitFramework.Base;
using LitFramework.Input;
using LitFramework.LitTool;
using LitFramework.Mono;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;


public enum ShaderType
{
    Circle,
    Rect
}

public partial class GuideShaderController : SingletonMono<GuideShaderController>, IManager
{
    [Header( "初始的引导形状" )]
    [SerializeField]
    private ShaderType _initType = ShaderType.Circle;
    public ShaderType SType
    {
        get { return _initType; }
        set
        {
            _initType = value;
            _parentBG.material = _initType == ShaderType.Circle ? _circleMat : _rectMat;
            _targetThreshold = _initType == ShaderType.Circle ? _guideConfig.thresholdCircle : _guideConfig.thresholdRect;
            _material = _parentBG.material;
        }
    }

    private Image _hand;
    private Image _parentBG;
    private Button _targetBtn;
    private Material _circleMat, _rectMat;

    /// <summary>
    /// 要高亮显示的目标
    /// </summary>
    private Image _target;
    private EventPenetrate _ev;
    private GuideConfig _guideConfig;
    private GuideDataModel _guideDataModel;

    private bool _isShowHand = true;
    private bool _isFocusing = false;
    private bool _isInit = false;
    private float _targetThreshold;
    private Camera _uiCam;
    private Canvas _rootCanv;
    /// <summary>
    /// 区域范围缓存
    /// </summary>
    private Vector3[] _corners = new Vector3[ 4 ];
    /// <summary>
    /// 镂空区域圆心
    /// </summary>
    private Vector4 _center;
    /// <summary>
    /// 镂空区域半径
    /// </summary>
    private float _radius;
    /// <summary>
    /// 遮罩材质
    /// </summary>
    private Material _material;
    /// <summary>
    /// 当前高亮区域的半径
    /// </summary>
    private float _currentRadius;
    /// <summary>
    /// 收缩速度
    /// </summary>
    private float _shrinkVelocity = 0f;

    public void ShowHand( Transform target )
    {
        if ( !_isShowHand )
        {
            _isShowHand = true;

            _hand.DOKill();
            _hand.enabled = true;
            _hand.transform.SetAsLastSibling();
            _hand.transform.position = target.position + _guideConfig.handImageOffset;
            _hand.CrossFadeAlpha( 0, 0.01f, true );
            _hand.CrossFadeAlpha( 1, 0.4f, true );
            _hand.transform.DOScale( 0.7f, 0.6f ).SetEase( Ease.InCirc ).SetLoops( -1, LoopType.Yoyo );
        }
    }

    public void HideHand()
    {
        if ( _isShowHand )
        {
            _isShowHand = false;

            _hand.DOKill();
            _hand.CrossFadeAlpha( 0, 0.01f, true );
            _hand.enabled = false;
        }
    }


    

    /// <summary>
    /// 聚焦到引导目标
    /// TODO 这个地方需要增加对场景3D物体的特定点击回调操作
    /// </summary>
    /// <param name="operateImage">目标点击区域Button Image</param>
    /// <param name="callBack"></param>
    public void ChangeTarget( Image operateImage, Action callBack = null )
    {
        _ev.SetTargetImage( null );

        if ( operateImage == null )
        {
            _parentBG.enabled = false;
            _hand.enabled = false;

            _isFocusing = false;
            _currentRadius = 0;
            _radius = 0f;
            ResetGuide();
            return;
        }
        else
        {
            _parentBG.enabled = true;
        }

        _isFocusing = true;
        _target = operateImage;

        UIMaskManager.Instance.SetMaskEnable( UITransparentEnum.NoPenetratingMiddle );
        InputControlManager.Instance.IsEnable = false;
       
        //获取高亮区域的四个顶点的世界坐标
        _target.rectTransform.GetWorldCorners( _corners );

        _guideDataModel.GuideDoneEvent = callBack;
        //如果目标位置是一个UI Button
        //任何按钮点击应该都是进行完成并关闭当前引导或加载下一步引导
        _targetBtn = operateImage.GetComponent<Button>();
        if ( _targetBtn != null )
            _targetBtn.onClick.AddListener( OnClickTargetButtonGuideDone );
        
        //计算高亮显示区域的圆心
        float x = _corners[ 0 ].x + ( ( _corners[ 3 ].x - _corners[ 0 ].x ) / 2f );
        float y = _corners[ 0 ].y + ( ( _corners[ 1 ].y - _corners[ 0 ].y ) / 2f );
        Vector3 centerWorld = new Vector3( x, y, 0 );
        Vector2 center = LitTool.WorldToCanvasPos( _rootCanv, centerWorld, _uiCam );

        //设置遮罩材料中的变量
        Vector4 centerMat = new Vector4( center.x, center.y, 0, 0 );
        Vector4 widthHeight = new Vector4( _target.rectTransform.rect.width, _target.rectTransform.rect.height, 0, 0 );

        _material.SetVector( "_Center", centerMat );
        _material.SetVector( "_WH", widthHeight );

        //计算当前高亮显示区域的半径
        RectTransform canRectTransform = _rootCanv.transform as RectTransform;
        if ( SType == ShaderType.Circle )
        {
            //计算最终高亮显示区域的半径
            _radius = Vector2.Distance( LitTool.WorldToCanvasPos( _rootCanv, _corners[ 0 ], _uiCam ),
                          LitTool.WorldToCanvasPos( _rootCanv, _corners[ 2 ], _uiCam ) ) / 2f;
            if ( canRectTransform != null )
            {
                //获取画布区域的四个顶点
                canRectTransform.GetWorldCorners( _corners );
                //将画布顶点距离高亮区域中心最远的距离作为当前高亮区域半径的初始值
                foreach ( Vector3 corner in _corners )
                {
                    _currentRadius = Mathf.Max( Vector3.Distance( LitTool.WorldToCanvasPos( _rootCanv, corner, _uiCam ), center ),
                        _currentRadius );
                }
            }
        }
        else if ( SType == ShaderType.Rect )
        {
            //计算最终高亮显示区域的半径
            _radius = 1f;
            _currentRadius = 10f;
        }
        _material.SetFloat( "_Slider", _currentRadius );
        
        UIManager.Instance.MaskImage.transform.SetAsLastSibling();
    }

    public void Install()
    {
        if ( _isInit ) return;
        _isInit = true;

        //加载新手引导所需各类资源
        _circleMat = GameObject.Instantiate<Material>( Resources.Load<Material>( "Shaders/Guide/Material/CircleMateria" ) );
        _rectMat = GameObject.Instantiate<Material>( Resources.Load<Material>( "Shaders/Guide/Material/RectMateria" ) );
        _hand = UnityHelper.GetTheChildNodeComponetScripts<Image>( UIManager.Instance.TransPopUp, "Image_Hand" );
        _hand.sprite = GameObject.Instantiate<Sprite>( Resources.Load<Sprite>( "Prefabs/UI/common_hand" ) );
        _parentBG = UIManager.Instance.MaskImage;

        Assert.IsNotNull( _circleMat, "GuideShaderController circleMat 未加载成功" );
        Assert.IsNotNull( _rectMat, "GuideShaderController rectMat 未加载成功" );
        Assert.IsNotNull( _parentBG, "GuideShaderController parentBG 未加载成功" );
        Assert.IsNotNull( _hand, "GuideShaderController hand 未加载成功" );

        _parentBG.enabled = true;
        _parentBG.material = _initType == ShaderType.Circle ? _circleMat : _rectMat;

        _isFocusing = false;
        _material = _parentBG.material;
        _guideConfig = GuideConfig.Instance;
        _guideDataModel = GuideDataModel.Instance;
        _ev = _parentBG.GetComponent<EventPenetrate>();
        if ( _ev == null )
            _ev = _parentBG.gameObject.AddComponent<EventPenetrate>();
        _targetThreshold = _initType == ShaderType.Circle ? _guideConfig.thresholdCircle : _guideConfig.thresholdRect;
        _uiCam = UIManager.Instance.UICam;
        _rootCanv = UIManager.Instance.TransRoot.GetComponent<Canvas>();
        _parentBG.enabled = false;

        ResetGuide();
        HideHand();
        GC.Collect();
    }

    public void Uninstall()
    {
        _circleMat = null;
        _rectMat = null;
        _parentBG = null;
        _hand = null;

        _material = null;
        _ev = null;

        _guideConfig = null;
        _guideDataModel = null;

        GC.Collect();
    }

    private void Update()
    {
        if ( !_isFocusing ) return;

        float value = Mathf.SmoothDamp( _currentRadius, _radius, ref _shrinkVelocity, _guideConfig.shrinkTime );
        if ( value - _radius >= _targetThreshold )
        {
            _currentRadius = value;
            _material.SetFloat( "_Slider", _currentRadius );
        }
        else if ( _isFocusing )
        {
            _isFocusing = false;
            _currentRadius = _radius;
            _material.SetFloat( "_Slider", _currentRadius );
            _ev.SetTargetImage( _target );

            InputControlManager.Instance.IsEnable = true;
            //聚焦到位以后，触发一个回调
            _guideDataModel.FocusTargetDoneEvent?.Invoke();
        }
    }

    /// <summary>
    /// 引导完成后UI重置
    /// </summary>
    private void CurrentGuideOver()
    {
        ChangeTarget( null );
        ResetGuide();
        HideHand();
    }

    /// <summary>
    /// 重置引导画面-去除镂空区域
    /// </summary>
    public void ResetGuide()
    {
        _isFocusing = false;
        _material.SetVector( "_Center", Vector4.zero );
        _material.SetVector( "_WH", Vector4.zero );
        _material.SetFloat( "_Slider", 0 );
    }



    /// <summary>
    /// 新手引导UI点击回调
    /// </summary>
    private void OnClickTargetButtonGuideDone()
    {
        if ( _targetBtn!=null )
            _targetBtn.onClick.RemoveListener( OnClickTargetButtonGuideDone );
        CurrentGuideOver();

        _guideDataModel.GuideDoneEvent?.Invoke();
    }

    /// <summary>
    /// 3D物体触碰回调
    /// </summary>
    public void OnTouchTargetGuideDone()
    {
        CurrentGuideOver();

        _guideDataModel.GuideDoneEvent?.Invoke();
    }
}