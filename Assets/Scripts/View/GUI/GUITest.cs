using LitFramework;
using LitFramework.LitTool;
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

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "注册事件" ) )
        {
            VibrateManager.Instance.Install();
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "强烈震动" ) )
        {
            VibrateManager.Instance.Shake( VibrateState.Acute );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "中等震动" ) )
        {
            VibrateManager.Instance.Shake( VibrateState.Interval );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "弱震动" ) )
        {
            VibrateManager.Instance.Shake( VibrateState.Softly );
        }

    }
}
