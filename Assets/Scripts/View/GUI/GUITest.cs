using LitFramework.Base;
using LitFramework.LitTool;
using LitFramework.Mono;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitFramework.LitTool;
using LitFramework.Input;

public class GUITest : MonoBehaviour
{

    private Action UpdateEventHandler;
#if UNITY_EDITOR
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮1" ) )
        {
            UIManager.Instance.LoadResourceFunc = ( e ) => { return Resources.Load( e ) as GameObject; };
            UIManager.Instance.Install();

            InputControlManager.Instance.Install();
            InputControlManager.Instance.TouchEndCallback += OnTouchEnd;
            UpdateEventHandler += InputControlManager.Instance.InputUpdateHandler;

            GuideShaderController.Instance.Install();
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮2" ) ) 
        {
            GuideShaderController.Instance.ChangeTarget( GameObject.Find( "TestImg" ).GetComponent<Image>() );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮3" ) )
        {
            GuideShaderController.Instance.SType = GuideShaderController.Instance.SType == ShaderType.Circle ? ShaderType.Rect : ShaderType.Circle;
            GuideShaderController.Instance.ChangeTarget( GameObject.Find( "TestImg" ).GetComponent<Image>() );
        }
    }

    private void OnTouchEnd( Vector2 obj )
    {
        LDebug.Log( "OnTouchEnd" );
    }

    private void Update()
    {
        UpdateEventHandler?.Invoke();
    }
#endif
}