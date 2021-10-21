using Assets.Scripts.Module.HotFix;
using LitFramework;
using LitFramework.LitTool;
using LitFramework.Mono;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class GUITest : MonoBehaviour
{
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮1" ) )
        {
            
        }

    }
}
