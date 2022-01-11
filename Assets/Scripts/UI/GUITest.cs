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
            UIManager.Instance.Show(ResPath.UI.UISTACK1);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "stack2"))
        {
            UIManager.Instance.Show(ResPath.UI.UISTACK2);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "stack3"))
        {
            UIManager.Instance.Show(ResPath.UI.UISTACK3);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close stack1"))
        {
            UIManager.Instance.Close(ResPath.UI.UISTACK1);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close stack2"))
        {
            UIManager.Instance.Close(ResPath.UI.UISTACK2);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close stack3"))
        {
            UIManager.Instance.Close(ResPath.UI.UISTACK3);
        }
        //第二行
        index = 0;vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "prallel1"))
        {
            UIManager.Instance.Show(ResPath.UI.UIPARRALLEL1);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "prallel2"))
        {
            UIManager.Instance.Show(ResPath.UI.UIPARRALLEL2);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "prallel3"))
        {
            UIManager.Instance.Show(ResPath.UI.UIPARRALLEL3);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close prallel1"))
        {
            UIManager.Instance.Close(ResPath.UI.UIPARRALLEL1);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close prallel2"))
        {
            UIManager.Instance.Close(ResPath.UI.UIPARRALLEL2);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close prallel3"))
        {
            UIManager.Instance.Close(ResPath.UI.UIPARRALLEL3);
        }
        //第三行
        index = 0; vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "unique1"))
        {
            UIManager.Instance.Show(ResPath.UI.UIUNIQUE1);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "unique2"))
        {
            UIManager.Instance.Show(ResPath.UI.UIUNIQUE2);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "unique3"))
        {
            UIManager.Instance.Show(ResPath.UI.UIUNIQUE3);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close unique1"))
        {
            UIManager.Instance.Close(ResPath.UI.UIUNIQUE1);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close unique2"))
        {
            UIManager.Instance.Close(ResPath.UI.UIUNIQUE2);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "close unique3"))
        {
            UIManager.Instance.Close(ResPath.UI.UIUNIQUE3);
        }

        index = 0; vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "CloseAll"))
        {
            UIManager.Instance.CloseAll(true,useAnim:true);
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
