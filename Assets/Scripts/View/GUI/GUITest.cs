using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GUITest : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "注册事件" ) )
        {
            LDebug.Log( Camera.main.aspect );
            LDebug.Log( Camera.main.pixelWidth );
            LDebug.Log( Camera.main.pixelHeight );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "去注册事件" ) )
        {
            
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "广播事件" ) )
        {
            
        }
        
    }
#endif
}
