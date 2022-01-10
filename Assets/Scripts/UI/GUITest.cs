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
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110* vindex, 100, 100), "测试1"))
        {

        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试2"))
        {
           
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试3"))
        {

        }
        ////第二行
        //index = 0;vindex++;
        //if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试4"))
        //{

        //}
        //if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试5"))
        //{

        //}
        //if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试6"))
        //{

        //}
        ////第三行
        //index = 0; vindex++;
        //if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试7"))
        //{

        //}
        //if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试8"))
        //{

        //}
        //if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "测试9"))
        //{

        //}
    }

}


public class TesttClass
{
    public int sssvalue = 10;
    public string ssname = "name";
    public float sss = 13f;

    public Vector3 pos = new Vector3(10f, 10f, 10f);
    public Vector2 pos2d = new Vector2(10f, 10f);
}
