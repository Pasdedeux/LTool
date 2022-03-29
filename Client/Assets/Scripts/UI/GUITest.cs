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
using LitFramework.InputSystem;

public class GUITest : MonoBehaviour
{
    private Transform _resourceItem;
    private GameObject _spwanItem;

    private void OnGUI()
    {
        //第一行
        int index = 0, vindex = 0 ;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Func1"))
        {
            VibrateManager.Instance.Shake(VibrateState.Interval);
        }

        index = 0; vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Func2"))
        {
            VibrateManager.Instance.Shake(VibrateState.Acute);
        }

        index = 0; vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Func3"))
        {
            VibrateManager.Instance.Shake(VibrateState.Softly);
        }
    }

}