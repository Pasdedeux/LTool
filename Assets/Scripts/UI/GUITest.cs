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
        int index = 0;

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "测试按钮1"))
        {
            //var ccc = Configs.sSpawnConfig.IDs;
            LDebug.Log( Configs.SpawnConfigDict[1].resPath);
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "测试按钮1"))
        {
            
        }



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
