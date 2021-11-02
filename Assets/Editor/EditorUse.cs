﻿using Assets.Scripts.Controller;
using DG.Tweening;
using LitFramework;
using LitFramework.LitTool;
using LitFrameworkEditor.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// 编辑器扩展，以及框架库的外展方法(Editor类型)覆写
/// </summary>
[InitializeOnLoadAttribute]
[ExecuteInEditMode]
public class EditorUse
{
    #region 编辑器及框架扩展功能

    //初始化类时,注册事件处理函数
    static EditorUse()
    {
        #region Build/Build UI功能
        //放到这里是因为Dotween效果无法包含在库中

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
            animTarget.autoKill = false;
            animTarget.autoPlay = false;

            //出场UI动画
            animTarget = e.AddComponent<DOTweenAnimation>();
            animTarget.animationType = DOTweenAnimation.AnimationType.Scale;
            animTarget.easeType = Ease.InBack;
            animTarget.duration = 0.4f;
            animTarget.id = g;
            animTarget.isFrom = false;
            animTarget.endValueFloat = 0f;
            animTarget.optionalBool0 = true;
            animTarget.autoKill = false;
            animTarget.autoPlay = false;
        };

        //组件创建回调
        UGUIOptimizeStrategy.camModeFunc = e => 
        {
            if ( FrameworkConfig.Instance.renderingType == LitRenderingType.URP )
                e.gameObject.AddComponent<URPCam>();
        };
        #endregion

        EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;
    }
    private static void OnPlayerModeStateChanged( PlayModeStateChange playModeState )
    {
        LDebug.LogWarning( string.Format( "state:{0} will:{1} isPlaying:{2}", playModeState, EditorApplication.isPlayingOrWillChangePlaymode, EditorApplication.isPlaying ) );
    }

    #endregion

    #region UGUI 自定义菜单

    [ExecuteInEditMode]
    public static class UGUICustom
    {
        [MenuItem( "GameObject/UI/Text" )]
        private static void CreatText( MenuCommand menuCommand )
        {
            UGUIOptimizeCommand.CreatText( menuCommand );
        }

        [MenuItem( "GameObject/UI/Image" )]
        private static void CreatImage( MenuCommand menuCommand )
        {
            UGUIOptimizeCommand.CreatImage( menuCommand );
        }

        //[MenuItem( "GameObject/URP/Camera" )]
        //private static void CreatCamera( MenuCommand menuCommand )
        //{
        //    UGUIOptimizeCommand.CreateCamera( menuCommand );
        //}
    }

    #endregion

}

#region 导入文件自动化处理

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
        //LDebug.Log( "====>importedAssets<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入所有资源之前获取通知。
    /// </summary>
    public void OnPreprocessAsset()
    {
        //LDebug.Log( "====>OnPreprocessAsset<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入模型（.fbx、.mb 文件等）之前获取通知。
    /// </summary>
    public void OnPreprocessModel()
    {
        //ModelImporter fbx = assetImporter as ModelImporter;
        //fbx.importCameras = false;
        //fbx.importLights = false;
        //fbx.isReadable = false;

        //LDebug.Log( "====>OnPreprocessModel<====" + fbx.name );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在模型完成导入时获取通知。
    /// </summary>
    public void OnPostprocessModel( GameObject go )
    {
        //LDebug.Log( "====>OnPostprocessModel<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入模型（.fbx、.mb 文件等）中的动画之前获取通知。
    /// </summary>
    public void OnPreprocessAnimation()
    {
        //LDebug.Log( "====>OnPreprocessAnimation<====" );
    }

    /// <summary>
    /// 当 AnimationClip 已完成导入时调用此函数。
    /// </summary>
    public void OnPostprocessAnimation( GameObject go, AnimationClip ac )
    {
        //LDebug.Log( "====>OnPostprocessAnimation<====" );
    }

    /// <summary>
    /// 将资源分配给其他资源捆绑包时调用的处理程序。
    /// </summary>
    public void OnPostprocessAssetbundleNameChanged( string s1, string s2, string s3 )
    {
        //LDebug.Log( "====>OnPostprocessAssetbundleNameChanged<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在立方体贴图纹理完成导入之前获取通知。
    /// </summary>
    public void OnPostprocessCubemap( Cubemap cm )
    {
        //LDebug.Log( "====>OnPostprocessCubemap<====" );
    }

    /// <summary>
    /// 当自定义属性的动画曲线已完成导入时调用此函数。
    /// </summary>
    public void OnPostprocessGameObjectWithAnimatedUserProperties( GameObject go, EditorCurveBinding[] bindings )
    {
        //// add a particle emitter to every game object that has a custom property called "particleAmount"
        //// then map the animation to the emission rate
        //for ( int i = 0; i < bindings.Length; i++ )
        //{
        //    if ( bindings[ i ].propertyName == "particlesAmount" )
        //    {
        //        ParticleSystem emitter = go.AddComponent<ParticleSystem>();
        //        var emission = emitter.emission;
        //        emission.rateOverTimeMultiplier = 0;

        //        bindings[ i ].propertyName = "EmissionModule.rateOverTime.scalar";
        //        bindings[ i ].path = AnimationUtility.CalculateTransformPath( go.transform, go.transform.root );
        //        bindings[ i ].type = typeof( ParticleSystem );
        //    }
        //}
        //LDebug.Log( "====>OnPostprocessGameObjectWithAnimatedUserProperties<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在材质资源完成导入时获取通知。
    /// </summary>
    public void OnPostprocessMaterial( Material material )
    {
        //LDebug.Log( "====>OnPostprocessMaterial<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在精灵的纹理完成导入时获取通知。
    /// </summary>
    void OnPostprocessSprites( Texture2D texture, Sprite[] sprites )
    {
        //LDebug.Log( "====>Sprites: " + sprites.Length + "<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在纹理导入器运行之前获取通知。
    /// </summary>
    public void OnPreprocessTexture()
    {
        TextureImporter textureImporter = ( TextureImporter )assetImporter;
        Assert.IsNotNull( textureImporter, "未检测到有效Texture资源" );

        switch ( textureImporter.textureType )
        {
            case TextureImporterType.Default:
                //textureImporter.mipmapEnabled = false;
                break;
            case TextureImporterType.NormalMap:
                break;
            case TextureImporterType.GUI:
                break;
            case TextureImporterType.Sprite:
                //textureImporter.SetTextureSettings( new TextureImporterSettings() { spriteGenerateFallbackPhysicsShape = false } );
                break;
            case TextureImporterType.Cursor:
                break;
            case TextureImporterType.Cookie:
                break;
            case TextureImporterType.Lightmap:
                break;
            case TextureImporterType.SingleChannel:
                break;
            default:
                break;
        }

        LDebug.Log( "====>OnPreprocessTexture<====" + textureImporter.name );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在纹理刚完成导入之前获取通知。
    /// </summary>
    public void OnPostprocessTexture( Texture2D texture )
    {
        //LDebug.Log( "====>OnPostprocessTexture<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在导入音频剪辑之前获取通知。
    /// </summary>
    public void OnPreprocessAudio()
    {
        //LDebug.Log( "====>OnPreprocessAudio<====" );
    }

    /// <summary>
    /// 将此函数添加到一个子类中，以在音频剪辑完成导入时获取通知。
    /// </summary>
    public void OnPostprocessAudio( AudioClip audio )
    {
        //LDebug.Log( "====>OnPostprocessAudio<====" );
    }

}

#region 待完善，针对UGUI IMAGE TEXT AddComponent组件检测执行执行

[ExecuteInEditMode]
public class ComponentExpand : MonoBehaviour
{
    Type window = GetType( "AddComponentWindow" );

    private void Awake()
    {
        Debug.Log( "EditorAwake" );
    }

    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type GetType( string typeName )
    {
        Type type = null;
        Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
        int assemblyArrayLength = assemblyArray.Length;
        for ( int i = 0; i < assemblyArrayLength; ++i )
        {
            type = assemblyArray[ i ].GetType( typeName );
            if ( type != null )
            {
                return type;
            }
        }

        for ( int i = 0; ( i < assemblyArrayLength ); ++i )
        {
            Type[] typeArray = assemblyArray[ i ].GetTypes();
            int typeArrayLength = typeArray.Length;
            for ( int j = 0; j < typeArrayLength; ++j )
            {
                if ( typeArray[ j ].Name.Equals( typeName ) )
                {
                    return typeArray[ j ];
                }
            }
        }
        return type;
    }
}

#endregion

#endregion