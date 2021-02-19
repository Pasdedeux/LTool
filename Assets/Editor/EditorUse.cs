using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorUse
{

#if UNITY_EDITOR
    [MenuItem( "Tools/配置文件1", priority = 20 )]
#endif
    public static void XlsxToCSV()
    {
        FolderCopy.CopyTo( Application.dataPath + "/Resources", Application.dataPath + "/Resources1" );
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

#if UNITY_EDITOR
    [MenuItem( "Tools/配置文件2", priority = 20 )]
#endif
    public static void XlsxToCSV2()
    {
        FolderCopy.MoveTo( Application.dataPath + "/Resources1", Application.dataPath + "/Resources2" );
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

}

