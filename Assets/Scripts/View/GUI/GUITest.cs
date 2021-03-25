using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GUITest : MonoBehaviour
{

    private Action UpdateEventHandler;
#if UNITY_EDITOR
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "注册事件" ) )
        {
            MsgManager.Instance.Register( EventType1.EventAr, CallBack );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "去注册事件" ) )
        {
            MsgManager.Instance.UnRegister( EventType1.EventAr, CallBack );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "广播事件" ) )
        {
            MsgManager.Instance.Broadcast( EventType1.EventAr, new MsgArgs( "ceshi1", 1 ) );
        }
        
    }

    private void CallBack( MsgArgs obj )
    {
        LDebug.Log( "====>MsgArgs" );
    }




#endif
}
