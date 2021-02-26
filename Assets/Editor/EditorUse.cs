using DG.Tweening;
using LitFramework.LitTool;
using LitFrameworkEditor.EditorExtended;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoadAttribute]
[ExecuteInEditMode]
public class EditorUse
{
    //初始化类时,注册事件处理函数
    static EditorUse()
    {
        //UI创建窗口初始化
        RegisterUIWindow.CreateAnimationComponentEvent = ( e, f, g ) =>
        {
            DOTweenAnimation animTarget;
            //进场UI动画
            animTarget = e.AddComponent<DOTweenAnimation>();
            animTarget.animationType = DOTweenAnimation.AnimationType.Scale;
            animTarget.easeType = Ease.OutBack;
            animTarget.duration = 0.4f;
            animTarget.id = f;
            animTarget.isFrom = true;
            animTarget.endValueFloat = 0f;
            animTarget.optionalBool0 = true;

            //出场UI动画
            animTarget = e.AddComponent<DOTweenAnimation>();
            animTarget.animationType = DOTweenAnimation.AnimationType.Scale;
            animTarget.easeType = Ease.InBack;
            animTarget.duration = 0.4f;
            animTarget.id = g;
            animTarget.isFrom = false;
            animTarget.endValueFloat = 0f;
            animTarget.optionalBool0 = true;
        };
        
        EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;
    }

    private static void OnPlayerModeStateChanged( PlayModeStateChange playModeState )
    {
        LDebug.LogWarning( string.Format( "state:{0} will:{1} isPlaying:{2}", playModeState, EditorApplication.isPlayingOrWillChangePlaymode, EditorApplication.isPlaying ) );
    }

    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
    [MenuItem( "Tools/原型测试按钮" )]
    static void CreatePrefab()
    {
        
    }

    //// Disable the menu item if no selection is in place.
    //[MenuItem( "Examples/Create Prefab", true )]
    //static bool ValidateCreatePrefab()
    //{
    //    return Selection.activeGameObject != null && !EditorUtility.IsPersistent( Selection.activeGameObject );
    //}
}

public class AssetsInEditorManager : AssetPostprocessor
{
    /// <summary>
    /// 在完成任意数量的资源导入后（当资源进度条到达末尾时）调用此函数。
    /// </summary>
    /// <param name="importedAssets"></param>
    /// <param name="deletedAssets"></param>
    /// <param name="movedAssets"></param>
    /// <param name="movedFromAssetPaths"></param>
    public static void OnPostprocessAllAssets( 
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths )
    {
        LDebug.Log( "====>importedAssets<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入所有资源之前获取通知。
    /// </summary>
    public void OnPreprocessAsset()
    {
        LDebug.Log( "====>OnPreprocessAsset<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入模型（.fbx、.mb 文件等）之前获取通知。
    /// </summary>
    public void OnPreprocessModel( GameObject go )
    {
        LDebug.Log( "====>OnPreprocessModel<====" );
        //ModelImporter fbx = assetImporter as ModelImporter;
        //fbx.importAnimation = false;
        //fbx.importCameras = false;
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在模型完成导入时获取通知。
    /// </summary>
    public void OnPostprocessModel()
    {
        LDebug.Log( "====>OnPostprocessModel<====" );
    }
    
    /// <summary>
    /// 将此函数添加到一个子类中，以在导入模型（.fbx、.mb 文件等）中的动画之前获取通知。
    /// </summary>
    public void OnPreprocessAnimation()
    {
        LDebug.Log( "====>OnPreprocessAnimation<====" );
    }

    /// <summary>
    /// 当 AnimationClip 已完成导入时调用此函数。
    /// </summary>
    public void OnPostprocessAnimation()
    {
        LDebug.Log( "====>OnPostprocessAnimation<====" );
    }

    /// <summary>
    /// 将资源分配给其他资源捆绑包时调用的处理程序。
    /// </summary>
    public void OnPostprocessAssetbundleNameChanged()
    {
        LDebug.Log( "====>OnPostprocessAssetbundleNameChanged<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在立方体贴图纹理完成导入之前获取通知。
    /// </summary>
    public void OnPostprocessCubemap()
    {
        LDebug.Log( "====>OnPostprocessCubemap<====" );
    }

    /// <summary>
    /// 当自定义属性的动画曲线已完成导入时调用此函数。
    /// </summary>
    public void OnPostprocessGameObjectWithAnimatedUserProperties()
    {
        LDebug.Log( "====>OnPostprocessGameObjectWithAnimatedUserProperties<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在材质资源完成导入时获取通知。
    /// </summary>
    public void OnPostprocessMaterial()
    {
        LDebug.Log( "====>OnPostprocessMaterial<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在精灵的纹理完成导入时获取通知。
    /// </summary>
    public void OnPostprocessSprites()
    {
        LDebug.Log( "====>OnPostprocessSprites<====" );
    }
    
    /// <summary>
    /// 将此函数添加到一个子类中，以在纹理导入器运行之前获取通知。
    /// </summary>
    public void OnPreprocessTexture()
    {
        LDebug.Log( "====>OnPreprocessTexture<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在纹理刚完成导入之前获取通知。
    /// </summary>
    public void OnPostprocessTexture()
    {
        LDebug.Log( "====>OnPostprocessTexture<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入音频剪辑之前获取通知。
    /// </summary>
    public void OnPreprocessAudio()
    {
        LDebug.Log( "====>OnPreprocessAudio<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在音频剪辑完成导入时获取通知。
    /// </summary>
    public void OnPostprocessAudio()
    {
        LDebug.Log( "====>OnPostprocessAudio<====" );
    }
    
}
