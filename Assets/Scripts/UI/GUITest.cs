using Assets.Scripts.Module.HotFix;
using LitFramework;
using LitFramework.LitTool;
using LitFramework.HotFix;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using Assets.Scripts.Module;
using System.Reflection;
using UnityEngine.Networking;
using LitFramework.LitPool;

public class GUITest : MonoBehaviour
{

    private void Funnn()
    { 
    }
    private void OnGUI()
    {
        //第一行
        int index = 0, vindex = 0 ;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110* vindex, 100, 100), "stack1"))
        {
            UIManager.Instance.Show(ResPath.UI.UITESTVERTICAL);
        }
        index = 0; vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "CloseAll(false)"))
        {
            UIManager.Instance.CloseAll(isDestroy: false,useAnim:true);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "CloseAll(true)"))
        {
            UIManager.Instance.CloseAll(isDestroy: true, useAnim: true, force: true);
        }
    }

}