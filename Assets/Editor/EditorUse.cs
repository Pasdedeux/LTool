using DG.Tweening;
using LitFrameworkEditor.EditorExtended;
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
        UICreateWindow.CreateAnimationComponentEvent = ( e, f, g ) =>
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

    //// Creates a new menu item 'Examples > Create Prefab' in the main menu.
    //[MenuItem( "Tools/创建UI界面" )]
    //static void CreatePrefab()
    //{
    //    // Keep track of the currently selected GameObject(s)
    //    GameObject[] objectArray = Selection.gameObjects;

    //    // Loop through every GameObject in the array above
    //    foreach ( GameObject gameObject in objectArray )
    //    {
    //        // Set the path as within the Assets folder,
    //        // and name it as the GameObject's name with the .Prefab format
    //        string localPath = "Assets/" + gameObject.name + ".prefab";

    //        // Make sure the file name is unique, in case an existing Prefab has the same name.
    //        localPath = AssetDatabase.GenerateUniqueAssetPath( localPath );

    //        // Create the new Prefab.
    //        PrefabUtility.SaveAsPrefabAssetAndConnect( gameObject, localPath, InteractionMode.UserAction );
    //    }
    //}

    //// Disable the menu item if no selection is in place.
    //[MenuItem( "Examples/Create Prefab", true )]
    //static bool ValidateCreatePrefab()
    //{
    //    return Selection.activeGameObject != null && !EditorUtility.IsPersistent( Selection.activeGameObject );
    //}
}

