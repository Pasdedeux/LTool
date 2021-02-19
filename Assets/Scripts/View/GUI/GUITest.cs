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
using System.IO;
using UnityEditor;
using System.Text;
using Excel;
using System.Data;
using LitFrameworkEditor.EditorExtended;

public class GUITest : MonoBehaviour
{

    private Action UpdateEventHandler;
#if UNITY_EDITOR
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮1" ) )
        {
            
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "测试按钮2" ) ) 
        {
            
        }
    }
#endif
}