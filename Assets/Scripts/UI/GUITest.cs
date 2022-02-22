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
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Resource"))
        {
            _resourceItem = RsLoadManager.Instance.Get("Prefabs/Test/Image_1").transform;
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Spawn"))
        {
            _spwanItem = RsLoadManager.Instance.Get("Image_2");
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Spawn"))
        {
            SpawnManager.Instance.CreatePool("Prefabs/Test/Image_1", "UI");
        }

        index = 0; vindex++;
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Res Destroy"))
        {
            _resourceItem.gameObject.Destroy();
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "Spawn Destroy"))
        {
            _spwanItem.Destroy();
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "RemoveByTag"))
        {
            SpawnManager.Instance.RemovePool("UI");
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "RemoveByTransform"))
        {
            SpawnManager.Instance.RemovePool(_resourceItem);
        }
        if (GUI.Button(new Rect(10 + 110 * index++, 100 + 110 * vindex, 100, 100), "RemoveByGameObject"))
        {
            SpawnManager.Instance.RemovePool(_spwanItem);
        }
    }

}

public class TestInfo
{
    public Color ss = new Color(200f / 255, 200 / 255f, 155 / 255f);
    public Vector3 v3 = new Vector3(20, 15, 19f);
    public Quaternion qq = new Quaternion(1, 2.2f, 4.55666f, 7.8888f);
    public Cust Test = new Cust { ss = 12, sss = new Vector3(25f, 12f, 1), ssss = Color.red };
}

public struct Cust
{
    public int ss;
    public Vector3 sss;
    public Color ssss;
}