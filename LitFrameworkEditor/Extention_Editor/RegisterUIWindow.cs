#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFrameworkEditor.Extention_Editor
* 项目描述 ：
* 类 名 称 ：UICreateWindow
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFrameworkEditor.Extention_Editor
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2021/2/24 16:10:05
* 更新时间 ：2021/2/24 16:10:05
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2021. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using LitFramework.LitTool;
using LitFrameworkEditor.EditorExtended;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RegisterUIWindow : EditorWindow
{
    [MenuItem( "Tools/Build/Build UI &u" )]
    private static void CreateUIWindow()
    {
        ExpandEdiorUseEvent?.Invoke();
        GetWindow<RegisterUIWindow>();
    }

    #region UI模板生成

    #region 窗口使用的变量
    public static Action ExpandEdiorUseEvent;
    public static Action<GameObject, string, string> CreateAnimationComponentEvent;

    public string uiScriptsName = "", uiSummary = "";

    public UINodeTypeEnum uiNodeType = UINodeTypeEnum.PopUp;
    public UIShowModeEnum uiShowMode = UIShowModeEnum.Stack;
    public UITransparentEnum uiTransparent = UITransparentEnum.NoPenetratingMiddle;

    public bool useAnimRoot = true, useOnEnable_OnDisable = true, useDefaultExitBtn = true;

    public string animStartID = "10001";
    public string animCloseID = "10002";

    public bool isDirty = false;
    public Canvas newCanvas;

    #endregion

    #region 窗口绘制

    void OnGUI()
    {
        //必选项：类名
        GUILayout.Label( "脚本类名(UI+类名) 例如：UIMain" );
        uiScriptsName = EditorGUILayout.TextField( uiScriptsName );

        //选填
        GUILayout.Label( "类说明，建议写UI界面类型，例如：主界面" );
        uiSummary = EditorGUILayout.TextField( uiSummary );

        uiNodeType = ( UINodeTypeEnum )EditorGUILayout.EnumPopup( "挂载节点", uiNodeType );
        uiShowMode = ( UIShowModeEnum )EditorGUILayout.EnumPopup( "窗体显示方式", uiShowMode );
        uiTransparent = ( UITransparentEnum )EditorGUILayout.EnumPopup( "窗体背后的遮罩透明度", uiTransparent );

        EditorGUILayout.Space();

        //勾选项
        useOnEnable_OnDisable = EditorGUILayout.Toggle( "OnEnable/OnDisable", useOnEnable_OnDisable );
        useDefaultExitBtn = EditorGUILayout.Toggle( "退出按钮", useDefaultExitBtn );
        if ( useDefaultExitBtn )
            useOnEnable_OnDisable = useDefaultExitBtn ? true : EditorGUILayout.Toggle( "启用OnEnable/OnDisable", useOnEnable_OnDisable );
        useAnimRoot = EditorGUILayout.Toggle( "动画控制器", useAnimRoot );
        if ( useAnimRoot )
        {
            animStartID = EditorGUILayout.TextField( "    弹出动画ID", animStartID );
            animCloseID = EditorGUILayout.TextField( "    关闭动画ID", animCloseID );
        }

        EditorGUILayout.Space();

        if ( GUILayout.Button( "创建脚本+UI预制件+绑定" ) )
        {
            if ( CheckClassNameValid() )
            {
                isDirty = true;

                EditorUtility.DisplayProgressBar( "生成UI模块", "", 1f );

                //CS 脚本
                UICreateParse cs = new UICreateParse();
                string csOutPath = Application.dataPath + "/Scripts/UI";
                EditorMenuExtention.CreateCSFile( csOutPath, uiScriptsName + ".cs", cs.CreateCS( this ) );
                AssetDatabase.Refresh();

                //预制件
                newCanvas = new GameObject( "Canvas_" + uiScriptsName.Substring( 2 ), typeof( Canvas ) ).GetComponent<Canvas>();
                newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var canvasScaler = newCanvas.gameObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

                var graphics = newCanvas.gameObject.AddComponent<GraphicRaycaster>();
                graphics.ignoreReversedGraphics = true;
                graphics.blockingObjects = GraphicRaycaster.BlockingObjects.None;

                GameObject animTrans = new GameObject( "Container_Anim", typeof( RectTransform ) );
                animTrans.transform.SetParent( newCanvas.transform );
                animTrans.transform.localPosition = Vector3.zero;
                animTrans.transform.localScale = Vector3.one;

                if ( useAnimRoot )
                {
                    //DOTEEN插件未集成在编辑器库中，引出到库外部使用
                    CreateAnimationComponentEvent?.Invoke( animTrans, animStartID, animCloseID );
                }

                if ( useDefaultExitBtn )
                {
                    GameObject btnExit = new GameObject( "Btn_Exit", typeof( RectTransform ) );
                    btnExit.AddComponent<CanvasRenderer>();
                    btnExit.AddComponent<Image>().maskable = false;
                    btnExit.AddComponent<Button>();
                    btnExit.transform.SetParent( animTrans.transform );
                    btnExit.transform.localPosition = Vector3.zero;
                    btnExit.transform.localScale = Vector3.one;
                }

                //预制件自注册
                RigisterUIPath( uiScriptsName );

                AssetDatabase.Refresh();

            }
            else
                EditorUtility.DisplayDialog( "类名错误", "类名应该不为空、空格，并且以UI开头", "哦" );
        }

        EditorGUILayout.Space();

        if ( GUILayout.Button( "创建脚本" ) )
        {
            if ( CheckClassNameValid() )
            {
                //CS 脚本
                UICreateParse cs = new UICreateParse();
                string csOutPath = Application.dataPath + "/Scripts/UI";
                EditorMenuExtention.CreateCSFile( csOutPath, uiScriptsName + ".cs", cs.CreateCS( this ) );
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog( "类名错误", "类名应该不为空、空格，并且以UI开头", "哦" );
            }
        }

        EditorGUILayout.Space();

        if ( GUILayout.Button( "更新UI配置" ) )
        {
            FileInfo _saveLocalFileInfo = new FileInfo( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) );

            //============JSON文件取出============//
            ResPathTemplate rpt = null;
            //如果文件存在，则读取解析为存储类，写入相关数据条后写入JSON并保存
            if ( DocumentAccessor.IsExists( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) ) )
            {
                var content = DocumentAccessor.ReadFile( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) );

                rpt = JsonMapper.ToObject<ResPathTemplate>( content );
            }
            //如果文件不存在，则新建存储类，并保存相关的数据，然后写入JSON并保存
            else
            {
                rpt = new ResPathTemplate();
            }
            //=================================//

            //每次都重新写入
            Dictionary<string, string> resPathSumList = new Dictionary<string, string>();
            foreach ( var item in rpt.UI )
            {
                var sum = item.Value.Split( '|' );
                if ( sum.Length > 1 && !string.IsNullOrEmpty( sum[ 1 ] ) )
                    resPathSumList.Add( item.Key.ToUpper(), sum[ 1 ] );
            }
            rpt.UI.Clear();

            //============存入UI配置============//
            DirectoryInfo folder = new DirectoryInfo( "Assets/Resources/" + GlobalEditorSetting.UI_PREFAB_PATH );
            foreach ( FileInfo file in folder.GetFiles() )
            {
                string ss = file.Extension.ToUpper();
                if ( ss.Contains( ".PREFAB" ) && file.FullName.Contains( "Canvas" ) )
                {
                    var result = file.Name.Split( '.' )[ 0 ];
                    var key = "UI" + result.Split( '_' )[ 1 ].ToUpper();
                    rpt.UI.Add( key, GlobalEditorSetting.UI_PREFAB_PATH + result );

                    if ( resPathSumList.ContainsKey( key ) )
                        rpt.UI[ key ] += "|" + resPathSumList[ key ];
                }
            }
            //=================================//

            //============JSON文件存入============//
            using ( StreamWriter sw = _saveLocalFileInfo.CreateText() )
            {
                var result = JsonMapper.ToJson( rpt );
                sw.Write( result );
            }
            //=================================//

            //============更新并保存CS============//
            //更新并保存CS
            ResPathParse rpp = new ResPathParse();
            EditorMenuExtention.CreateCSFile( Application.dataPath + "/Scripts", GlobalEditorSetting.OUTPUT_FILENAME, rpp.CreateCS( rpt ) );
            AssetDatabase.Refresh();
        }

        //汇总编译
        while ( isDirty && !EditorApplication.isCompiling )
        {
            isDirty = false;

            EditorUtility.ClearProgressBar();
            LDebug.Log( " 成功生成UI预制件! " );

            //反射生成脚本组件
            var asmb = System.Reflection.Assembly.Load( "Assembly-CSharp" );
            var t = asmb.GetType( "Assets.Scripts.UI." + uiScriptsName );
            if ( null != t ) newCanvas.gameObject.AddComponent( t );
            else LDebug.LogError( "UI脚本绑定失败" );

            string localPath = "Assets/Resources/" + GlobalEditorSetting.UI_PREFAB_PATH + newCanvas.gameObject.name + ".prefab";
            //预防重名
            localPath = AssetDatabase.GenerateUniqueAssetPath( localPath );
            PrefabUtility.SaveAsPrefabAssetAndConnect( newCanvas.gameObject, localPath, InteractionMode.UserAction );

            AssetDatabase.Refresh();
        }
    }

    #endregion

    #region 制作UI及注册路径
    private static FileInfo _saveLocalFileInfo = new FileInfo( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) );
    /// <summary>
    /// 这里将类名注册为地址类中的字典键值对，将预制件地址存储为值
    /// JSON将需要保存已经注册过的UI、音频文件
    /// JSON保存地址为StreamingAssets   configs.dat
    /// </summary>
    private void RigisterUIPath( string uiScriptsName )
    {
        string localPath = "Assets/Resources/" + GlobalEditorSetting.UI_PREFAB_PATH + "Canvas_" + uiScriptsName.Substring( 2 );

        //预防重名
        localPath = AssetDatabase.GenerateUniqueAssetPath( localPath );
        localPath = localPath.Substring( 17 );
        localPath = localPath.Split( '.' )[ 0 ];
        localPath = localPath + "|" + uiSummary;

        ResPathTemplate rpt = null;
        //如果文件存在，则读取解析为存储类，写入相关数据条后写入JSON并保存
        if ( DocumentAccessor.IsExists( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) ) )
        {
            var content = DocumentAccessor.ReadFile( AssetPathManager.Instance.GetStreamAssetDataPath( GlobalEditorSetting.JSON_FILE_NAME, false ) );

            rpt = JsonMapper.ToObject<ResPathTemplate>( content );
            if ( !rpt.UI.ContainsKey( uiScriptsName ) )
                rpt.UI.Add( uiScriptsName, localPath );
        }
        //如果文件不存在，则新建存储类，并保存相关的数据，然后写入JSON并保存
        else
        {
            rpt = new ResPathTemplate();
            rpt.UI.Add( uiScriptsName, localPath );
        }

        //写入已经注册的UI
        using ( StreamWriter sw = _saveLocalFileInfo.CreateText() )
        {
            var result = JsonMapper.ToJson( rpt );
            sw.Write( result );
        }

        //更新并保存CS
        ResPathParse rpp = new ResPathParse();
        EditorMenuExtention.CreateCSFile( Application.dataPath + "/Scripts", GlobalEditorSetting.OUTPUT_FILENAME, rpp.CreateCS( rpt ) );
        AssetDatabase.Refresh();
    }

    #endregion

    private bool CheckClassNameValid()
    {
        return !string.IsNullOrEmpty( uiScriptsName ) && !string.IsNullOrWhiteSpace( uiScriptsName ) && uiScriptsName.Substring( 0, 2 ).Equals( "UI" );
    }

    #endregion
}

#region UI 脚本CS生成器

namespace LitFrameworkEditor.EditorExtended
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// UI脚本生成器
    /// </summary>
    class UICreateParse
    {
        private const string SPACENAME = "LitFramework";
        private string _className;
        private StringBuilder _sb = new StringBuilder();
        private bool _useAnimRoot = true,
        _useOnEnable_OnDisable = true,
        _useDefaultExitBtn = true;

        List<string> CSString = new List<string>();
        RegisterUIWindow _uiWindowInfo;

        public string CreateCS( RegisterUIWindow uiWindowInfo )
        {
            _uiWindowInfo = uiWindowInfo;
            _className = uiWindowInfo.uiScriptsName;
            _useAnimRoot = uiWindowInfo.useAnimRoot;
            _useDefaultExitBtn = uiWindowInfo.useDefaultExitBtn;
            _useOnEnable_OnDisable = uiWindowInfo.useOnEnable_OnDisable;

            AddHead();
            AddBody();
            AddTail();
            string result = GetFomatedCS();

            return result;
        }

        private void AddBody()
        {
            if ( _useDefaultExitBtn )
                CSString.Add( "public Button btnExit;" );
            CSString.Add( "private bool _isFreeze;" );
            CSString.Add( "private Transform _root;" );
            if ( _useAnimRoot )
                CSString.Add( "private DOTweenAnimation[] _anims;" );

            #region Awake

            CSString.Add( "" );
            CSString.Add( "public override void OnAwake()" );
            CSString.Add( "{" );
            CSString.Add( string.Format( "CurrentUIType.uiNodeType = UINodeTypeEnum.{0};", _uiWindowInfo.uiNodeType ) );
            CSString.Add( string.Format( "CurrentUIType.uiShowMode = UIShowModeEnum.{0};", _uiWindowInfo.uiShowMode ) );
            CSString.Add( string.Format( "CurrentUIType.uiTransparent = UITransparentEnum.{0};", _uiWindowInfo.uiTransparent ) );
            CSString.Add( "" );
            CSString.Add( "Init();" );
            CSString.Add( "}" );

            #endregion

            #region Init 用于初始化各类UI信息

            CSString.Add( "" );
            CSString.Add( "/// <summary>" );
            CSString.Add( "/// 用于初始化各类UI信息" );
            CSString.Add( "/// </summary>" );
            CSString.Add( "private void Init()" );
            CSString.Add( "{" );
            CSString.Add( "_root = transform;" );

            if ( _uiWindowInfo.useDefaultExitBtn )
            {
                CSString.Add( "btnExit = UnityHelper.GetTheChildNodeComponetScripts<Button>( _root, \"btnExit\" );" );
            }

            CSString.Add( "" );
            CSString.Add( "//TODO 初始化该UI信息" );
            CSString.Add( "//.." );
            CSString.Add( "" );
            if ( _useAnimRoot )
                CSString.Add( " _anims = AnimationManager.GetAllAnim( _root );" );
            CSString.Add( "}" );

            #endregion

            #region OnShow

            CSString.Add( "" );
            CSString.Add( "public override void OnShow()" );
            CSString.Add( "{" );
            if ( _uiWindowInfo.useAnimRoot )
                CSString.Add( string.Format( "_anims.Restart( \"{0}\");", _uiWindowInfo.animStartID ) );
            CSString.Add( "" );
            CSString.Add( "}" );

            #endregion

            #region OnEnable/OnDisbale

            if ( _uiWindowInfo.useOnEnable_OnDisable || _uiWindowInfo.useDefaultExitBtn )
            {
                CSString.Add( "" );
                CSString.Add( "public override void OnEnabled( bool freeze )" );
                CSString.Add( "{" );

                if ( _uiWindowInfo.useDefaultExitBtn )
                {
                    CSString.Add( "btnExit.onClick.AddListener( OnClickExit );" );
                }
                CSString.Add( "//TODO 注册事件" );
                CSString.Add( "//.." );
                CSString.Add( "" );
                CSString.Add( "_isFreeze = freeze;" );
                CSString.Add( "}" );

                CSString.Add( "" );
                CSString.Add( "public override void OnDisabled( bool freeze )" );
                CSString.Add( "{" );

                if ( _uiWindowInfo.useDefaultExitBtn )
                {
                    CSString.Add( "btnExit.onClick.RemoveAllListeners();" );
                }
                CSString.Add( "//TODO 取消注册事件" );
                CSString.Add( "//.." );
                CSString.Add( "" );
                CSString.Add( "}" );
            }

            #endregion

            CSString.Add( "" );
            CSString.Add( "" );
            CSString.Add( "" );
            CSString.Add( "" );
            CSString.Add( "" );
            CSString.Add( "" );
            CSString.Add( "//==========点击回调事件==========//" );

            #region 固定退出回调

            if ( _uiWindowInfo.useDefaultExitBtn )
            {
                CSString.Add( "" );
                CSString.Add( "private void OnClickExit()" );
                CSString.Add( "{" );
                if ( _uiWindowInfo.useAnimRoot )
                {
                    CSString.Add( string.Format( "_anims.Restart( \"{0}\", () => ", _uiWindowInfo.animCloseID ) );
                    CSString.Add( "{" );
                    CSString.Add( "UIManager.Instance.Close( AssetsName );" );
                    CSString.Add( "//.." );
                    CSString.Add( "});" );
                }
                else
                {
                    CSString.Add( "UIManager.Instance.Close( AssetsName );" );
                }
                CSString.Add( "}" );
            }

            #endregion
        }

        void AddHead()
        {
            CSString.Add( "#region << 版 本 注 释 >>" );
            CSString.Add( "///*----------------------------------------------------------------" );
            CSString.Add( "// Author : Derek Liu" );
            CSString.Add( "// 创建时间:" + DateTime.Now.ToString() );
            CSString.Add( "// 该类由模板工具自动生成" );
            CSString.Add( "///----------------------------------------------------------------*/" );
            CSString.Add( "#endregion" );

            CSString.Add( "using UnityEngine;" );
            CSString.Add( "using UnityEngine.UI;" );
            CSString.Add( "using System;" );
            CSString.Add( "using LitFramework;" );
            CSString.Add( "using LitFramework.Mono;" );
            CSString.Add( "using LitFramework.LitTool;" );
            CSString.Add( "using System.Collections.Generic;" );

            if ( _useAnimRoot )
                CSString.Add( "using DG.Tweening;" );

            CSString.Add( "namespace Assets.Scripts.UI" );
            CSString.Add( "{" );
            CSString.Add( string.Format( "/// <summary>" ) );
            CSString.Add( string.Format( "/// {0}", _uiWindowInfo.uiSummary ) );
            CSString.Add( string.Format( "/// </summary>" ) );
            CSString.Add( string.Format( "public class {0} : BaseUI", _className ) );
            CSString.Add( "{" );
        }

        void AddTail()
        {
            CSString.Add( "}" );
            CSString.Add( "}" );
        }

        /// <summary>
        /// 最终整合
        /// </summary>
        /// <returns>原代码文件</returns>
        string GetFomatedCS()
        {
            StringBuilder result = new StringBuilder();
            int tablevel = 0;
            for ( int i = 0; i < CSString.Count; i++ )
            {
                string tab = "";

                for ( int j = 0; j < tablevel; ++j )
                    tab += "\t";

                if ( CSString[ i ].Contains( "{" ) )
                    tablevel++;
                if ( CSString[ i ].Contains( "}" ) )
                {
                    tablevel--;
                    tab = "";
                    for ( int j = 0; j < tablevel; ++j )
                        tab += "\t";
                }

                result.Append( tab + CSString[ i ] + "\n" );
            }
            return result.ToString();
        }
    }
    /// <summary>
    /// UI/Sound路径注册类
    /// </summary>
    class ResPathParse
    {
        List<string> CSString = new List<string>();

        public string CreateCS( ResPathTemplate rpt )
        {
            AddHead();
            AddBody( rpt );
            AddTail();
            string result = GetFomatedCS();

            return result;
        }

        private void AddHead()
        {
            CSString.Add( "#region << 版 本 注 释 >>" );
            CSString.Add( "///*----------------------------------------------------------------" );
            CSString.Add( "// Author : Derek Liu" );
            CSString.Add( "// 创建时间:" + DateTime.Now.ToString() );
            CSString.Add( "// 备注：由模板工具自动生成" );
            CSString.Add( "///----------------------------------------------------------------*/" );
            CSString.Add( "#endregion" );
            CSString.Add( "" );
            CSString.Add( "//*******************************************************************" );
            CSString.Add( "//**                  该类由工具自动生成，请勿手动修改                   **" );
            CSString.Add( "//*******************************************************************" );
            CSString.Add( "" );
            CSString.Add( "using LitFramework;" );
            CSString.Add( "public class ResPath:Singleton<ResPath>" );
            CSString.Add( "{" );
        }
        private void AddTail()
        {
            CSString.Add( "}" );
        }
        private void AddBody( ResPathTemplate rpt )
        {
            //Sound
            CSString.Add( "public class Sound" );
            CSString.Add( "{" );
            foreach ( var item in rpt.Sound )
            {
                CSString.Add( string.Format( "public const string {0} = \"{1}\";", item.Key.ToUpper(), item.Value ) );
            }
            CSString.Add( "}" );

            //UI
            CSString.Add( "public class UI" );
            CSString.Add( "{" );
            foreach ( var item in rpt.UI )
            {
                string[] nameAndComment = item.Value.Split( '|' );
                CSString.Add( "/// <summary>" );
                CSString.Add( string.Format( "/// {0}", nameAndComment.Length > 1 ? nameAndComment[ 1 ] : "" ) );
                CSString.Add( "/// </summary>" );
                CSString.Add( string.Format( "public const string {0} = \"{1}\";", item.Key.ToUpper(), nameAndComment[ 0 ] ) );
            }
            CSString.Add( "}" );
        }
        string GetFomatedCS()
        {
            StringBuilder result = new StringBuilder();
            int tablevel = 0;
            for ( int i = 0; i < CSString.Count; i++ )
            {
                string tab = "";

                for ( int j = 0; j < tablevel; ++j )
                    tab += "\t";

                if ( CSString[ i ].Contains( "{" ) )
                    tablevel++;
                if ( CSString[ i ].Contains( "}" ) )
                {
                    tablevel--;
                    tab = "";
                    for ( int j = 0; j < tablevel; ++j )
                        tab += "\t";
                }

                result.Append( tab + CSString[ i ] + "\n" );
            }
            return result.ToString();
        }
    }
}

#endregion

