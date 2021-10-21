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


public class GUITest : MonoBehaviour
{
    private void OnGUI()
    {
        int index = 0;

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "测试按钮1"))
        {
            //DocumentAccessor.SaveAsset2LocalFileByJson(  new TesttClass() , AssetPathManager.Instance.GetStreamAssetDataPath( "TessJson.json", false ) );
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "测试按钮1"))
        {
            //var stri = AssetPathManager.Instance.GetStreamAssetDataPath( "TessJson.json" );
            //DocumentAccessor.LoadAsset( stri, ( string e ) => 
            //{
            //    var obj = LitJson.JsonMapper.ToObject<TesttClass>( e );
            //    LDebug.Log( ">>>" + obj.pos );
            //} );
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
