using LitFramework.LitTool;
using System;
using System.Collections;
using UnityEngine;

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
            LitTool.DelayPlayFunction( 1.5f, () => { LDebug.Log( "===>Update" ); });
            LitTool.DelayPlayFunction( 1.5f, () => { LDebug.Log( "===>Update Mono" ); }, useUpdate: false );
        }
    }
    
#endif



    
}
