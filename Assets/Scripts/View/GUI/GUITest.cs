using LitFramework;
using LitFramework.LitTool;
using LitFramework.Mono;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GUITest : MonoBehaviour
{
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮1" ) )
        {
            ResourceManager.Instance.DownLoadCSVAB( FrameworkConfig.Instance.RemoteUrlConfig );
            LDebug.Log( ">>>走下一步" );
        }

    }
}
