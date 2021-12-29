﻿#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFrameworkEditor.Extention_Editor
* 项目描述 ：
* 类 名 称 ：CreatUIWindow
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：
* CLR 版本 ：4.0.30319.42000
* 作    者 ：ZhouXia
* 创建时间 ：2021/12/7 16:10:05
* 更新时间 ：2021/12/7 16:10:05
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2021. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using LitFramework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using DG.Tweening;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;

public class CreateUIWindow : OdinEditorWindow
{
    [MenuItem(@"Assets/UI/Build", priority = 0)]
    public static EditorWindow OpenWindow()
    {
        ;
        if (LitFramework.FrameworkConfig.Instance.UseHotFixMode)
        {
            CreateUIWindow window = GetWindow<CreateUIWindow>();
            // Nifty little trick to quickly position the window in the middle of the editor.
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
            Object selet = Selection.activeObject;
            if (selet)
            {
                string seletPath = AssetDatabase.GetAssetPath(selet);
                if (seletPath.Contains(GlobalEditorSetting.UI_PREFAB_PATH))
                {
                    int startFolerIndex = "Assets/Resources/Prefabs/UI/".Length;
                    int classLen = 0;
                    if (seletPath.Contains("."))
                    {
                        classLen = seletPath.Length - seletPath.IndexOf(selet.name);
                    }

                    int endLength = seletPath.Length - startFolerIndex - classLen;
                    if (classLen > 0)
                    {
                        endLength = endLength - 1;
                    }
                    string folder = seletPath.Substring(startFolerIndex, endLength);
                    window.uiFolderName = folder;
                }
            }
            window.uiScriptsName = "UI";
            return window;
        }
        else
            return GetWindow<RegisterUIWindow>();
    }
    private static CSWriteTool mCSWrite = new CSWriteTool();
   [ValidateInput("CheckClassNameValid", "(UI+类名) 例如：UIMain 类名应该不为空、空格，并且以UI开头")]
    [LabelText("脚本名")]
    [HorizontalGroup(LabelWidth = 40)]
    public string uiScriptsName;
    [LabelText("UI路径")]
    [HorizontalGroup("Class")]
    public string uiFolderName;
    [HorizontalGroup("Class")]
    [LabelText("类说明")]
    public string uiSummary;
    [Space(10, order = 0)]
    [LabelText("退出按钮")]
    [HorizontalGroup("Prefab", LabelWidth = 40)]
    public bool useDefaultExitBtn = true;
    [LabelText("打开退出是否有动画")]
    [Space(10, order = 0)]
    [HorizontalGroup("Prefab",LabelWidth =80)]
    public bool useAnimRoot = true;
    [LabelText("关闭所有界面时是否过滤掉")]
    [Space(10, order = 0)]
    [HorizontalGroup("Prefab", LabelWidth = 150)]
    public bool IsFloor=false;
    [LabelText("是否使用低帧率")]
    [Space(10, order = 0)]
    [HorizontalGroup("Prefab", LabelWidth = 150)]
    public bool UseLowFrame = false;
    [BoxGroup("UI类型设置")]
    [ShowInInspector]
    public UIType uiType = new UIType();
   

    private FileInfo _saveLocalFileInfo;

    private GameObject _Prefab;

    [Button("创建脚本+UI预制件+注册绑定", ButtonSizes.Medium, ButtonHeight = 40), GUIColor(0f, 0.8f, 0f)]
    private void CreatCSPrefabPath()
    {
        _saveLocalFileInfo = new FileInfo(Application.dataPath + "/Editor/" + GlobalEditorSetting.JSON_FILE_NAME);

        if (CheckClassNameValid(uiScriptsName))
        {

            EditorUtility.DisplayProgressBar("UI生成", "设置UI预制", 1f/4f);
            //设置预制
            SetPrefab();

            EditorUtility.DisplayProgressBar("UI生成", "设置自定义脚本", 2f / 4f);
            //CS 自定义脚本
            SetCustomCs(uiFolderName, uiScriptsName, mCSWrite, useDefaultExitBtn, uiSummary);

            EditorUtility.DisplayProgressBar("UI生成", "设置基础脚本", 2f / 4f);
            //设置基础cs
            SetBaseCs(_Prefab, mCSWrite, uiFolderName+"/", uiType, IsFloor, useLowFrame: UseLowFrame );

            EditorUtility.DisplayProgressBar("UI生成", "设置路径文本", 2f / 4f);

            //路径更新
            UpdataPath(mCSWrite);

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

        }
        else
            EditorUtility.DisplayDialog("类名错误", "类名应该不为空、空格，并且以UI开头", "哦");
    }
    private static void SetCustomCs(string aFolderName,string aScriptsName,CSWriteTool aCSWrite,bool haveClose,string uiSummary)
    {
        string csOutPath = Application.dataPath + "/Scripts/UI/" + aFolderName;
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/UI/" + aFolderName;
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UI/" + aFolderName;
        if (File.Exists(csOutPath + aScriptsName + ".cs"))
        {
            return;
        }
        aCSWrite.Reset();

        aCSWrite.WriteLine("using System.Collections;");
        aCSWrite.WriteLine("using System.Collections.Generic;");
        aCSWrite.WriteLine("using UnityEngine;");
        aCSWrite.WriteLine("using UnityEngine.UI;");
        aCSWrite.WriteLine("using LitFramework.HotFix;");
        aCSWrite.WriteLine("namespace Assets.Scripts.UI");
        //开始namespace
        aCSWrite.StartBracket();

        aCSWrite.WriteLine("/// <summary>");
        aCSWrite.WriteLine("///" + uiSummary);
        aCSWrite.WriteLine("/// </summary>");
        aCSWrite.WriteLine("public partial class {0} : BaseUI", aScriptsName);
        //开始Class
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public override void OnAwake()");
        //开始初始化
        aCSWrite.StartBracket();
        if(haveClose)
        {
            aCSWrite.WriteLine("bttn_ReturnBtn.onClick.AddListener(OnClickExit);");
        }
        //结束初始化
        aCSWrite.EndBracket();
        aCSWrite.WriteLine("public override void OnShow(params object[] args)");
        //开始show
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        aCSWrite.EndBracket();
        aCSWrite.WriteLine("public override void OnClose()");
        //开始Close
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        //结束Close
        aCSWrite.EndBracket();



        aCSWrite.WriteLine("#region 点击回调事件");
        if (haveClose)
        {
            aCSWrite.WriteLine("private void OnClickExit()");
            aCSWrite.StartBracket();
            aCSWrite.WriteLine("UIManager.Instance.Close(AssetsName);");
            aCSWrite.EndBracket();
        }
            
        aCSWrite.WriteLine("#endregion");
        //结束Class
        aCSWrite.EndBracket();
        //结束namespace
        aCSWrite.EndBracket();
        aCSWrite.Save(csOutPath, aScriptsName + ".cs");
        Debug.Log("自定义文件更新完成");
    }
    private static void SetCustomElementCs(string aFolderName, string aScriptsName, CSWriteTool aCSWrite)
    {
        string csOutPath = Application.dataPath + "/Scripts/UI/" + aFolderName;
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/UI/" + aFolderName;
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UI/" + aFolderName;
        if (File.Exists(csOutPath + aScriptsName + ".cs"))
        {
            return;
        }

        aCSWrite.Reset();
        aCSWrite.WriteLine("using System.Collections;");
        aCSWrite.WriteLine("using System.Collections.Generic;");
        aCSWrite.WriteLine("using UnityEngine;");
        aCSWrite.WriteLine("using UnityEngine.UI;");
        aCSWrite.WriteLine("using LitFramework.HotFix;");
        aCSWrite.WriteLine("namespace Assets.Scripts.UI");
        //开始namespace
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public partial class {0}:BaseScrollElement ", aScriptsName);
        //开始Class
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public override void OnInit()");
        //开始初始化
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        //结束初始化
        aCSWrite.EndBracket();
        aCSWrite.WriteLine("public override void SetElement()");
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        aCSWrite.EndBracket();

        aCSWrite.WriteLine("public override void UpdateInfo(MsgArgs args)");
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        aCSWrite.EndBracket();
        aCSWrite.WriteLine("public override void Dispose()");
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        aCSWrite.EndBracket();


        aCSWrite.WriteLine("#region 点击回调事件");
        aCSWrite.WriteLine("");
        aCSWrite.WriteLine("");
        aCSWrite.WriteLine("#endregion");
        //结束Class
        aCSWrite.EndBracket();
        //结束namespace
        aCSWrite.EndBracket();
        aCSWrite.Save(csOutPath, aScriptsName + ".cs");
        Debug.Log("自定义Element文件更新完成");
    }
    private static void SetCustomExComCs(string aFolderName, string aScriptsName, CSWriteTool aCSWrite)
    {
        string csOutPath = Application.dataPath + "/Scripts/UI/" + aFolderName;
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/UI/" + aFolderName;
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UI/" + aFolderName;
        if (File.Exists(csOutPath + aScriptsName + ".cs"))
        {
            return;
        }
        aCSWrite.Reset();
        aCSWrite.WriteLine("using System.Collections;");
        aCSWrite.WriteLine("using System.Collections.Generic;");
        aCSWrite.WriteLine("using UnityEngine;");
        aCSWrite.WriteLine("using UnityEngine.UI;");
        aCSWrite.WriteLine("using LitFramework.HotFix;");
        aCSWrite.WriteLine("namespace Assets.Scripts.UI");
        //开始namespace
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public partial class {0} ", aScriptsName);
        //开始Class
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public void OnInite()");
        //开始初始化
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("");
        //结束初始化
        aCSWrite.EndBracket();
        aCSWrite.WriteLine("private int _id;");
        aCSWrite.WriteLine("private System.Action _callBack;");

        aCSWrite.WriteLine("/// <summary>");
        aCSWrite.WriteLine("///设置组件");
        aCSWrite.WriteLine("/// </summary>");
        aCSWrite.WriteLine("public void SetExCom(int aID,System.Action aCallBack=null)");
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("_id=aID;");
        aCSWrite.WriteLine("_callBack = aCallBack;");
        aCSWrite.EndBracket();

        aCSWrite.WriteLine("/// <summary>");
        aCSWrite.WriteLine("///关闭组件");
        aCSWrite.WriteLine("/// </summary>");
        aCSWrite.WriteLine("public void CloseExCom()");
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("_id = -1;");
        aCSWrite.WriteLine("_callBack = null;");
        aCSWrite.EndBracket();

        aCSWrite.WriteLine("#region 点击回调事件");
        aCSWrite.WriteLine("");
        aCSWrite.WriteLine("");
        aCSWrite.WriteLine("#endregion");
        //结束Class
        aCSWrite.EndBracket();
        //结束namespace
        aCSWrite.EndBracket();
        aCSWrite.Save(csOutPath, aScriptsName + ".cs");
        Debug.Log("自定义Element文件更新完成");
    }
    #region BaseCS
    public static void SetBaseCs(GameObject aPrefab, CSWriteTool aCSWrite, string folderName, UIType uiType=null,bool isFloor=false,bool useLowFrame = false, string uiSummary="")
    {
        if (!aPrefab)
        {
            return;
        }
        Transform m_Container = aPrefab.transform.Find("Container_Anim");
        if (!m_Container)
        {
            return;
        }
        string className = "UI" + aPrefab.name.Substring(7);
        string csOutPath = Application.dataPath + "/Scripts/UIExport/" + folderName;
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/UIExport/" + folderName;
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport/" + folderName;


        tempArray.Clear();
        aCSWrite.Reset();
        if (File.Exists(csOutPath + className + ".cs"))
        {
            string[] oldText = File.ReadAllLines(csOutPath + className + ".cs");
            for (int i =0;i< oldText.Length;i++)
            {
                string line = oldText[i].TrimStart().TrimEnd();
                if("{".Equals(line))
                {
                    aCSWrite.StartBracket();
                }else if ("}".Equals(line))
                {
                    aCSWrite.EndBracket();
                    break;
                }else
                {
                    aCSWrite.WriteLine(line);
                }

            }
          //  File.Delete(csOutPath + className + ".cs");
        }
        else
        {

            aCSWrite.WriteLine("using System.Collections;");
            aCSWrite.WriteLine("using System.Collections.Generic;");
            aCSWrite.WriteLine("using UnityEngine;");
            aCSWrite.WriteLine("using UnityEngine.UI;");
            aCSWrite.WriteLine("using LitFramework.HotFix;");
            aCSWrite.WriteLine("namespace Assets.Scripts.UI");
            //开始namespace
            aCSWrite.StartBracket();

            aCSWrite.WriteLine("/// <summary>");
            aCSWrite.WriteLine("///" + uiSummary);
            aCSWrite.WriteLine("/// </summary>");
            aCSWrite.WriteLine("public partial class {0} : BaseUI", className);
            //开始Class
            aCSWrite.StartBracket();
            if (uiType==null)
            {
                uiType = new UIType();
            }
            aCSWrite.WriteLine("private void SetUIType()");
            aCSWrite.StartBracket();
            aCSWrite.WriteLine("CurrentUIType.uiNodeType = UINodeTypeEnum.{0};", uiType.uiNodeType.ToString());
            aCSWrite.WriteLine("CurrentUIType.uiShowMode = UIShowModeEnum.{0};", uiType.uiShowMode.ToString());
            aCSWrite.WriteLine("CurrentUIType.uiTransparent = UITransparentEnum.{0};", uiType.uiTransparent.ToString());
            aCSWrite.WriteLine("Flag = UIFlag.{0};", isFloor? "Fix" : "Normal");
            aCSWrite.WriteLine("UseLowFrame = {0};", useLowFrame ? "true" : "false");
            aCSWrite.EndBracket();
        }

        aCSWrite.WriteLine("public static int RegistSystem(string className)");
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("UIManager.Instance.RegistFunctionCallFun(ResPath.UI.{0}, className);", className.ToUpper());
        aCSWrite.WriteLine("return 1;");
        aCSWrite.EndBracket();

        SetCsAttribute(m_Container,"", aCSWrite, "private");
        foreach(KeyValuePair<string,Transform> keyValue in tempArray)
        {
            SetCsAttrbuteArray(keyValue.Value, aCSWrite, "private");
        }
        aCSWrite.WriteLine("protected override void FindMember()");
        //开始初始化
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("SetUIType();");

        SetCsFindPath(m_Container, "", "m_AniTrans", aCSWrite);
        foreach (KeyValuePair<string, Transform> keyValue in tempArray)
        {
            CsFindPathArray(keyValue.Value, keyValue.Key, "m_AniTrans", aCSWrite);
        }
        aCSWrite.EndBracket();

        //结束class
        aCSWrite.EndBracket();
        //结束namespace
        aCSWrite.EndBracket();
        aCSWrite.Save(csOutPath, className + ".cs");

        tempArray.Clear();
        Debug.Log("基础路径文件更新完成");
    }
    public static void SetBaseElementCs(GameObject aPrefab, CSWriteTool aCSWrite, string folderName, UIType uiType = null, bool isFloor = false)
    {
        if (!aPrefab)
        {
            return;
        }
       
        string className = aPrefab.name.Replace("Element_", "Element");
        string csOutPath = Application.dataPath + "/Scripts/UIExport/" + folderName;
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/UIExport/" + folderName;
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport/" + folderName;


        tempArray.Clear();
        aCSWrite.Reset();
        aCSWrite.WriteLine("using System.Collections;");
        aCSWrite.WriteLine("using System.Collections.Generic;");
        aCSWrite.WriteLine("using UnityEngine;");
        aCSWrite.WriteLine("using UnityEngine.UI;");
        aCSWrite.WriteLine("using LitFramework.LitPool; ");
        aCSWrite.WriteLine("namespace Assets.Scripts.UI");
        //开始namespace
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public partial class {0} : BaseScrollElement", className);
        //开始Class
        aCSWrite.StartBracket();


        aCSWrite.WriteLine("public static string AssetsName=ResPath.UI.{0};", className.ToUpper());
        aCSWrite.WriteLine("public {0} ()", className);
        aCSWrite.StartBracket();
        aCSWrite.EndBracket();
        aCSWrite.WriteLine("public {0} (Transform tans)", className);
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("linkedTrans = tans;");
        aCSWrite.EndBracket();

        aCSWrite.WriteLine("public static {0} CreateInstance()", className);
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("GameObject obj = SpawnManager.Instance.SpwanObject(AssetsName);");
        aCSWrite.WriteLine("{0} element = new {1}(obj.transform);", className,className);
        aCSWrite.WriteLine("return element;");
        aCSWrite.EndBracket();



        SetCsAttrbuteCom(aPrefab.transform, aCSWrite, null, "internal");
        SetCsAttribute(aPrefab.transform, "", aCSWrite, "internal");
        foreach (KeyValuePair<string, Transform> keyValue in tempArray)
        {
            SetCsAttrbuteArray(keyValue.Value, aCSWrite, "internal");
        }

        aCSWrite.WriteLine("public override void FindMenber()");
        aCSWrite.StartBracket();

        CsFindPathCom(aPrefab.transform, "","", "linkedTrans", aCSWrite);
        SetCsFindPath(aPrefab.transform, "", "linkedTrans", aCSWrite);
        foreach (KeyValuePair<string, Transform> keyValue in tempArray)
        {
            CsFindPathArray(keyValue.Value, keyValue.Key, "linkedTrans", aCSWrite);
        }
        aCSWrite.EndBracket();

        aCSWrite.EndBracket(); 
        aCSWrite.EndBracket();

        aCSWrite.Save(csOutPath, className + ".cs");

        tempArray.Clear();
        Debug.Log("ExCom路径文件更新完成");
    }
    public static void SetBaseExComCs(GameObject aPrefab, CSWriteTool aCSWrite, string folderName, UIType uiType = null, bool isFloor = false)
    {
        if (!aPrefab)
        {
            return;
        }
        string className = aPrefab.name.Replace("ExCom_", "ExCom");
        string csOutPath = Application.dataPath + "/Scripts/UIExport/" + folderName;
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/UIExport/" + folderName;
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport/" + folderName;


        tempArray.Clear();
        aCSWrite.Reset();
        aCSWrite.WriteLine("using System.Collections;");
        aCSWrite.WriteLine("using System.Collections.Generic;");
        aCSWrite.WriteLine("using UnityEngine;");
        aCSWrite.WriteLine("using UnityEngine.UI;");
        aCSWrite.WriteLine("using LitFramework.LitPool; ");
        aCSWrite.WriteLine("namespace Assets.Scripts.UI");
        //开始namespace
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("public partial class {0}", className);
        //开始Class
        aCSWrite.StartBracket();


        aCSWrite.WriteLine("public static string AssetsName=ResPath.UI.{0};", className.ToUpper());

        aCSWrite.WriteLine("public Transform transform;");
        aCSWrite.WriteLine("public {0} (Transform tans)", className);
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("transform = tans;");
        aCSWrite.WriteLine("FindMenber();");

        aCSWrite.WriteLine("OnInite();");
        aCSWrite.EndBracket();

        aCSWrite.WriteLine("public static {0} CreateInstance()", className);
        aCSWrite.StartBracket();
        aCSWrite.WriteLine("GameObject obj= GameObject.Instantiate<GameObject>(RsLoadManager.Instance.Load<GameObject>(AssetsName));");
        aCSWrite.WriteLine("{0} excom = new {1}(obj.transform);", className, className);
        aCSWrite.WriteLine("return excom;");
        aCSWrite.EndBracket();



        SetCsAttrbuteCom(aPrefab.transform, aCSWrite, null, "private");
        SetCsAttribute(aPrefab.transform, "", aCSWrite, "private");
        foreach (KeyValuePair<string, Transform> keyValue in tempArray)
        {
            SetCsAttrbuteArray(keyValue.Value, aCSWrite,"private");
        }
        aCSWrite.WriteLine("public void FindMenber()");
        aCSWrite.StartBracket();

        CsFindPathCom(aPrefab.transform, null,"", "transform", aCSWrite);
        SetCsFindPath(aPrefab.transform, "", "transform", aCSWrite);
        foreach (KeyValuePair<string, Transform> keyValue in tempArray)
        {
            CsFindPathArray(keyValue.Value, keyValue.Key, "transform", aCSWrite);
        }
        aCSWrite.EndBracket();

        aCSWrite.EndBracket();
        aCSWrite.EndBracket();
        aCSWrite.Save(csOutPath, className + ".cs");
        tempArray.Clear();
        Debug.Log("ExCom路径文件更新完成");
    }

    private static Dictionary<string, Transform> tempArray=new Dictionary<string, Transform>();
    public static void SetCsAttribute(Transform _root, string aPath, CSWriteTool cSWrite,string aScope)
    {
        if (_root.childCount <= 0)
        {
            return;
        }
        if (!string.IsNullOrEmpty(aPath))
        {
            aPath = aPath + "/";
        }
        foreach (Transform trans in _root)
        {
            if (string.IsNullOrEmpty(trans.name))
            {
                continue;
            }

            string _Name = trans.gameObject.name.Trim();
            _Name = _Name.Replace(" ", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(".", "");
            if (_Name.EndsWith(")"))
            {
                int indexStart = _Name.IndexOf('(')+1;
                if (indexStart>1)
                {
                    string numstr = _Name.Substring(indexStart, _Name.Length - indexStart-1);
                    string _className= _Name.Substring(0, indexStart-1);
                    int curIndex = 0;

                    string key = aPath + _className;
                    if (int.TryParse(numstr,out curIndex))
                    {
                        if (tempArray.ContainsKey(key))
                        {
                            string oldName= tempArray[key].name.Replace(" ", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(".", "");
                            string numstrOld = oldName.Replace(_className,"").Replace("(", "").Replace(")", "");
                            int idexOld = int.Parse(numstrOld);
                            if(curIndex>idexOld)
                            {
                                tempArray[key] = trans;
                            }
                        }else
                        {
                            tempArray[key] = trans;
                        }
                        continue;
                    }
                }
            }

            string _path = aPath + trans.name;
            _Name = _Name.Replace("(", "").Replace(")", "");
            if (!'_'.Equals(_Name[0]))
            {
                UnityEngine.Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(trans.gameObject);
                if(prefab!=null)
                {
                    if(prefab.name.StartsWith("ExCom_"))
                    {
                        string prefabName = prefab.name;
                        string type = prefabName.Replace("_", "");
                        string attrname = _Name.Replace("ExCom_", "").Replace("exCom_", "").Replace("ExCom", "").Replace("exCom", "");

                        SetCsAttrbuteCom(trans, cSWrite, "_" + attrname, aScope);
                        attrname = "exCom_" + attrname;
                        cSWrite.WriteLine("private {0} {1};", type, attrname);
                    }else if(prefab.name.StartsWith("Element_"))
                    {
                        string prefabName = prefab.name;
                        string type = prefabName.Replace("_", "");
                        string attrname = trans.name.Replace("Element_", "").Replace("element_", "").Replace("Element", "").Replace("element", "");
                        SetCsAttrbuteCom(trans, cSWrite, "_" + attrname, aScope);
                        attrname = "element_" + attrname;
                        cSWrite.WriteLine("private {0} {1};", type, attrname);
                    }else
                    {
                        SetCsAttribute(trans, _path, cSWrite, aScope);
                    }

                }else
                {
                    SetCsAttribute(trans, _path, cSWrite, aScope);
                }

                continue;
            }
            SetCsAttrbuteCom(trans, cSWrite, _Name, aScope);
            SetCsAttribute(trans, _path, cSWrite, aScope);
        }

    }
    private static void SetCsAttrbuteCom(Transform trans, CSWriteTool cSWrite,string _Name,string aScope)
    {
        MonoBehaviour[] comArray = trans.gameObject.GetComponents<MonoBehaviour>();
        if(!string.IsNullOrEmpty(_Name))
        {
            cSWrite.WriteLine("private Transform {0};", "trans" + _Name);
        }
        else
        {
            _Name = "_self";
        }
        foreach (MonoBehaviour com in comArray)
        {
            if (com is UnityEngine.EventSystems.UIBehaviour)
            {
                UnityEngine.EventSystems.UIBehaviour uiCom = com as UnityEngine.EventSystems.UIBehaviour;
                if (uiCom.IsExport)
                {
                    string _type = com.GetType().ToString();
                    string[] _typePath = _type.Split('.');
                    _type = _typePath[_typePath.Length - 1];
                    string endStr = _type.ToLower();
                    string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                    endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                    string _NameCom = endStr + _Name;
                    cSWrite.WriteLine("{0} {1} {2};", aScope, _type, _NameCom);
                }
            }
            else
            {
                string _type = com.GetType().ToString();
                string[] _typePath = _type.Split('.');
                _type = _typePath[_typePath.Length - 1];
                if ("DOTweenAnimation".Equals(_type))
                {
                    continue;
                }
                string endStr = _type.ToLower();
                string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                string _NameCom = endStr +_Name;
                cSWrite.WriteLine("{0} {1} {2};", aScope, _type, _NameCom);
            }
        }
    }
    private static void SetCsAttrbuteArray(Transform trans, CSWriteTool cSWrite, string aScope)
    {
        string _Name = trans.gameObject.name.Trim();
        _Name = _Name.Replace(" ", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(".", "");


        int startIndex = _Name.IndexOf('(');
        string _className = _Name.Substring(0, startIndex);
        string numstr = _Name.Substring(startIndex+1, _Name.Length - startIndex - 2);
        int idex = int.Parse(numstr);
        int aLen = idex + 1;


        _Name = _className;
        _Name = _Name.Replace("(", "").Replace(")", "");
        if (!'_'.Equals(_Name[0]))
        {
            UnityEngine.GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(trans.gameObject);
            if (prefab != null)
            {

                if (prefab.name.StartsWith("ExCom_"))
                {
                    string prefabName = prefab.name;
                    string type = prefabName.Replace("_", "");
                    string attrname = _Name.Replace("ExCom_", "").Replace("exCom_", "").Replace("ExCom", "").Replace("exCom", "");
                    SetCsAttrbuteComArray(trans, cSWrite, "_" + attrname, aLen, aScope);
                    attrname = "exCom_" + attrname;
                    attrname = attrname + "Array";

                    cSWrite.WriteLine("{0} {1}[] {2}=new {3}[{4}];", aScope, type, attrname, type,aLen);
                }
                else if (prefab.name.StartsWith("Element_"))
                {
                    string prefabName = prefab.name;
                    string type = prefabName.Replace("_", "");
                    string attrname = trans.name.Replace("Element_", "").Replace("element_", "").Replace("Element", "").Replace("element", "");

                    SetCsAttrbuteComArray(trans, cSWrite, "_" + attrname, aLen, aScope);

                    attrname = "element_" + attrname;
                    attrname = attrname + "Array";
                    cSWrite.WriteLine("{0} {1}[] {2}=new {3}[{4}];", aScope,type, attrname, type, aLen);
                }

            }

            return;
        }
        SetCsAttrbuteComArray(trans, cSWrite, _Name, aLen, aScope);
    }
    private static void SetCsAttrbuteComArray(Transform trans, CSWriteTool cSWrite, string _Name,int aLen,string aScope)
    {
        MonoBehaviour[] comArray = trans.gameObject.GetComponents<MonoBehaviour>();

        _Name = _Name + "Array";
        cSWrite.WriteLine("{0} Transform[] {1}=new Transform[{2}];", aScope,"trans" + _Name , aLen);
        foreach (MonoBehaviour com in comArray)
        {
            if (com is UnityEngine.EventSystems.UIBehaviour)
            {
                UnityEngine.EventSystems.UIBehaviour uiCom = com as UnityEngine.EventSystems.UIBehaviour;
                if (uiCom.IsExport)
                {
                    string _type = com.GetType().ToString();
                    string[] _typePath = _type.Split('.');
                    _type = _typePath[_typePath.Length - 1]; 
                    string endStr = _type.ToLower();
                    string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                    endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                    string _NameCom = endStr + _Name;
                    cSWrite.WriteLine("{0} {1}[] {2}=new {3}[{4}];", aScope, _type, _NameCom, _type, aLen);
                }
            }
            else
            {
                string _type = com.GetType().ToString();
                string[] _typePath = _type.Split('.');
                _type = _typePath[_typePath.Length - 1]; 
                if("DOTweenAnimation".Equals(_type))
                {
                    continue;
                }
                string endStr = _type.ToLower();
                string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                string _NameCom = endStr + _Name;
                cSWrite.WriteLine("{0} {1}[] {2}=new {3}[{4}];", aScope,_type, _NameCom, _type, aLen);
            }
        }
    }
    public static void SetCsFindPath(Transform _root, string aPath, string _rootName, CSWriteTool cSWrite)
    {
        if (_root.childCount <= 0)
        {
            return;
        }
        if (!string.IsNullOrEmpty(aPath))
        {
            aPath = aPath + "/";
        }

        foreach (Transform trans in _root)
        {
            if (string.IsNullOrEmpty(trans.name))
            {
                continue;
            }
            string _Name = trans.gameObject.name.Trim();
            _Name = _Name.Replace(" ", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(".", "");

            string _path = aPath + trans.name;
            if (_Name.EndsWith(")"))
            {
                int indexStart = _Name.IndexOf('(');
                if (indexStart > 0)
                {
                    string _className = _Name.Substring(0, indexStart);
                    string numstr = _Name.Substring(indexStart + 1, _Name.Length - indexStart - 2);
                    int curIndex = 0;
                    if (int.TryParse(numstr, out curIndex))
                    {
                        continue;
                    }
                }
            }

            _Name = _Name.Replace("(", "").Replace(")", "");
            if (!'_'.Equals(_Name[0]))
            {
                UnityEngine.Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(trans.gameObject);
                if (prefab != null)
                {

                    if (prefab.name.StartsWith("ExCom_"))
                    {
                        string prefabName = prefab.name;
                        string type = prefabName.Replace("_", "");
                        string attrname = _Name.Replace("ExCom_", "").Replace("exCom_", "").Replace("ExCom", "").Replace("exCom", "");

                        CsFindPathCom(trans, "_" + attrname, _path, _rootName, cSWrite);
                        cSWrite.WriteLine(" {0} = new {1}({2});", "exCom_" + attrname, type, "trans_" + attrname);
                    }
                    else if (prefab.name.StartsWith("Element_"))
                    {
                        string prefabName = prefab.name;
                        string type = prefabName.Replace("_", "");
                        string attrname = trans.name.Replace("Element_", "").Replace("element_", "").Replace("Element", "").Replace("element", "");

                        CsFindPathCom(trans, "_" + attrname, _path, _rootName, cSWrite);
                        cSWrite.WriteLine(" {0} = new {1}({2});", "element_" + attrname, type, "trans_" + attrname);
                    }
                    else
                    {
                        SetCsFindPath(trans, _path, _rootName, cSWrite);
                    }

                }
                else
                {
                    SetCsFindPath(trans, _path, _rootName,  cSWrite);
                }

                continue;
            }
            CsFindPathCom(trans,_Name,_path, _rootName, cSWrite);
            SetCsFindPath(trans, _path, _rootName, cSWrite);
        }

    }
    private static void CsFindPathCom(Transform trans, string _Name,string _path, string _rootName, CSWriteTool cSWrite)
    {
        MonoBehaviour[] comArray = trans.gameObject.GetComponents<MonoBehaviour>();
        string transName;
        if(!string.IsNullOrEmpty(_Name))
        {
            cSWrite.WriteLine("{0} = {1}.Find(\"{2}\");", "trans" + _Name, _rootName, _path);
            transName = "trans" + _Name;
        }
        else
        {
            _Name = "_self";
            transName = _rootName;
        }

        foreach (MonoBehaviour com in comArray)
        {
            if (com is UnityEngine.EventSystems.UIBehaviour)
            {
                UnityEngine.EventSystems.UIBehaviour uiCom = com as UnityEngine.EventSystems.UIBehaviour;
                if (uiCom.IsExport)
                {
                    string _type = com.GetType().ToString();
                    string[] _typePath = _type.Split('.');
                    _type = _typePath[_typePath.Length - 1];
                    string endStr = _type.ToLower();
                    string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                    endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                    string _NameCom = endStr + _Name;
                    cSWrite.WriteLine("{0} = {1}.GetComponent<{2}>();", _NameCom, transName, _type);
                }
            }
            else
            {
                string _type = com.GetType().ToString();
                string[] _typePath = _type.Split('.');
                _type = _typePath[_typePath.Length - 1];
                if ("DOTweenAnimation".Equals(_type))
                {
                    continue;
                }
                string endStr = _type.ToLower();
                string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                string _NameCom = endStr + _Name;
                cSWrite.WriteLine("{0} = {1}.GetComponent<{2}>();", _NameCom, transName, _type);
            }
        }
    }

    private static void CsFindPathArray(Transform trans, string aPath, string _rootName, CSWriteTool cSWrite)
    {
        string _Name = trans.gameObject.name.Trim();
        _Name = _Name.Replace(" ", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(".", "");

        int startIndex = _Name.IndexOf('(');
        string _className = _Name.Substring(0, startIndex);
        string numstr = _Name.Substring(startIndex + 1, _Name.Length - startIndex - 2);
        int idex = int.Parse(numstr);
        int aLen = idex + 1;


        _Name = _className;
        _Name = _Name.Replace("(", "").Replace(")", "");

        if (!'_'.Equals(_Name[0]))
        {
            UnityEngine.GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(trans.gameObject);
            if (prefab != null)
            {

                if (prefab.name.StartsWith("ExCom_"))
                {
                    string prefabName = prefab.name;
                    string type = prefabName.Replace("_", "");
                    string attrname = _Name.Replace("ExCom_", "").Replace("exCom_", "").Replace("ExCom", "").Replace("exCom", "");

                    CsFindPathComArray(trans, "_"+attrname, aPath, _rootName, cSWrite, aLen);
                    cSWrite.WriteLine("for(int i=0;i<{0};i++)", aLen);
                    cSWrite.StartBracket();
                    cSWrite.WriteLine(" {0}[i] = new {1}({2}[i]);", "exCom_" + attrname+"Array", type, "trans_" + attrname + "Array");
                    cSWrite.EndBracket();
                }
                else if (prefab.name.StartsWith("Element_"))
                {
                    string prefabName = prefab.name;
                    string type = prefabName.Replace("_", "");
                    string attrname = trans.name.Replace("Element_", "").Replace("element_", "").Replace("Element", "").Replace("element", "");

                    CsFindPathComArray(trans, "_" + attrname, aPath, _rootName, cSWrite, aLen);

                    cSWrite.WriteLine("for(int i=0;i<{0};i++)", aLen);
                    cSWrite.StartBracket();
                    cSWrite.WriteLine(" {0}[i] = new {1}({2}[i]);", "element_" + attrname + "Array", type, "trans" + attrname + "Array");
                    cSWrite.EndBracket();
                }

            }
            return;
        }
        CsFindPathComArray(trans, _Name, aPath, _rootName, cSWrite,aLen);
    }

    private static void CsFindPathComArray(Transform trans, string _Name, string aPath,string _rootName, CSWriteTool cSWrite,int aLen)
    {
        MonoBehaviour[] comArray = trans.gameObject.GetComponents<MonoBehaviour>();
        _Name = _Name + "Array";
        cSWrite.WriteLine("for(int i=0;i<{0};i++)", aLen);
        cSWrite.StartBracket();
        cSWrite.WriteLine("{0}[i] = {1}.Find(\"{2} (\"+i+\")\");", "trans" + _Name, _rootName, aPath);
        cSWrite.EndBracket();

        foreach (MonoBehaviour com in comArray)
        {
            if (com is UnityEngine.EventSystems.UIBehaviour)
            {
                UnityEngine.EventSystems.UIBehaviour uiCom = com as UnityEngine.EventSystems.UIBehaviour;
                if (uiCom.IsExport)
                {
                    string _type = com.GetType().ToString();
                    string[] _typePath = _type.Split('.');
                    _type = _typePath[_typePath.Length - 1];
                    string endStr = _type.ToLower();
                    string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                    endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                    string _NameCom = endStr + _Name;
                    cSWrite.WriteLine("for(int i=0;i<{0};i++)", aLen);
                    cSWrite.StartBracket();
                    cSWrite.WriteLine("{0}[i] ={1}[i].GetComponent<{2}>();", _NameCom, "trans" + _Name, _type);
                    cSWrite.EndBracket();
                }
            }
            else
            {
                string _type = com.GetType().ToString();
                string[] _typePath = _type.Split('.');
                _type = _typePath[_typePath.Length - 1];
                if ("DOTweenAnimation".Equals(_type))
                {
                    continue;
                }
                string endStr = _type.ToLower();
                string tstr = endStr.Replace("a", "").Replace("e", "").Replace("o", "").Replace("u", "");
                endStr = tstr.Length > 4 ? endStr.Substring(0, 4) : tstr;
                string _NameCom = endStr + _Name;
                cSWrite.WriteLine("for(int i=0;i<{0};i++)", aLen);
                cSWrite.StartBracket();
                cSWrite.WriteLine("{0}[i] ={1}[i].GetComponent<{2}>();", _NameCom, "trans" + _Name, _type);
                cSWrite.EndBracket();
            }
        }
    }
    #endregion
    private void SetPrefab()
    {
        string path1 = "Assets/Resources/" + GlobalEditorSetting.UI_PREFAB_PATH + uiFolderName;
        string path2 = Application.dataPath+"/Resources/" + GlobalEditorSetting.UI_PREFAB_PATH + uiFolderName;
        string path3 = Application.dataPath + "/Resources/" + GlobalEditorSetting.UI_PREFAB_PATH + uiFolderName + "/Canvas_" + uiScriptsName.Substring(2) + ".prefab";
        string path4 = path1 + "/Canvas_" + uiScriptsName.Substring(2) + ".prefab";
        Debug.Log(path2);
        if (!Directory.Exists(path2))
        {
            Directory.CreateDirectory(path2);
        }
        if (!File.Exists(path3))
        {
            _Prefab = CreatPrafab(path4);
        }
        _Prefab = AssetDatabase.LoadAssetAtPath(path4, typeof(GameObject)) as GameObject;
    }
    private GameObject CreatPrafab(string localPath)
    {
        Canvas newCanvas = new GameObject("Canvas_" + uiScriptsName.Substring(2), typeof(Canvas)).GetComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var canvasScaler = newCanvas.gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        //提升性能，需要时增加到子对象上
        //var graphics = newCanvas.gameObject.AddComponent<GraphicRaycaster>();
        //graphics.ignoreReversedGraphics = true;
        //graphics.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        GameObject animTrans = new GameObject("Container_Anim", typeof(RectTransform));
        animTrans.transform.SetParent(newCanvas.transform);
        var recTrans = animTrans.GetComponent<RectTransform>();
        recTrans.sizeDelta = Vector2.zero;
        recTrans.anchorMin = Vector2.zero;
        recTrans.anchorMax = Vector2.one;
        recTrans.anchoredPosition = Vector2.zero;
        animTrans.transform.localPosition = Vector3.zero;
        animTrans.transform.localScale = Vector3.one;

        if (useAnimRoot)
        {
            SetOpenCloseAni(animTrans);
        }

        if (useDefaultExitBtn)
        {
            GameObject btnExit = new GameObject("_ReturnBtn", typeof(RectTransform));
            btnExit.AddComponent<CanvasRenderer>();
            btnExit.AddComponent<Image>().maskable = false;
            btnExit.AddComponent<Button>();
            btnExit.transform.SetParent(animTrans.transform);
            btnExit.transform.localPosition = Vector3.zero;
            btnExit.transform.localScale = Vector3.one;
        }

        //Layer设定
        ChangeUILayer(newCanvas.transform, "UI");
        //if (FrameworkConfig.Instance.UIMode == UseUIType.UGUIMono)
        //{
        //    //反射生成脚本组件
        //    var asmb = System.Reflection.Assembly.Load("Assembly-CSharp");
        //    var t = asmb.GetType("Assets.Scripts.UI." + uiScriptsName);
        //    if (null != t) newCanvas.gameObject.AddComponent(t);
        //    else LDebug.LogError("UI脚本绑定失败");
        //}
        GameObject prefab=  PrefabUtility.SaveAsPrefabAsset(newCanvas.gameObject,localPath);
        DestroyImmediate(newCanvas.gameObject);
        Debug.Log("创建预制完成");
        return prefab;
    }

    private void ChangeUILayer(Transform trans, string targetLayer)
    {
        if (LayerMask.NameToLayer(targetLayer) == -1)
        {
            Debug.Log("Layer中不存在,请手动添加LayerName");

            return;
        }

        //遍历更改所有子物体layer
        trans.gameObject.layer = LayerMask.NameToLayer(targetLayer);
        foreach (Transform child in trans)
        {
            ChangeUILayer(child, targetLayer);
            Debug.Log(child.name + "子对象Layer更改成功！");
        }
    }
    private bool CheckClassNameValid(string aValue)
    {
        if(string.IsNullOrEmpty(aValue))
        {
            return false;
        }
        if(aValue.Length<3)
        {
            return false;
        }
        return  !string.IsNullOrWhiteSpace(aValue) && aValue.Substring(0, 2).Equals("UI");
    }
    private void SetOpenCloseAni(GameObject root)
    {
        DOTweenAnimation animTarget;
        //进场UI动画
        animTarget = root.AddComponent<DOTweenAnimation>();
        animTarget.animationType = DOTweenAnimation.AnimationType.Scale;
        animTarget.easeType = Ease.OutBack;
        animTarget.duration = 0.4f;
        animTarget.id = FrameworkConfig.Instance.OPENID;
        animTarget.isFrom = true;
        animTarget.endValueFloat = 0f;
        animTarget.optionalBool0 = true;

        //出场UI动画
        animTarget = root.AddComponent<DOTweenAnimation>();
        animTarget.animationType = DOTweenAnimation.AnimationType.Scale;
        animTarget.easeType = Ease.InBack;
        animTarget.duration = 0.4f;
        animTarget.id = FrameworkConfig.Instance.CLOSEID;
        animTarget.isFrom = false;
        animTarget.endValueFloat = 0f;
        animTarget.optionalBool0 = true;
    }
    private static void UpdataPath(CSWriteTool cSWrite)
    {
        string floder;
        if (FrameworkConfig.Instance.UseHotFixMode)
        {
            floder = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport";
        }else
        {
            floder = Application.dataPath + "/Scripts/UIExport";
        }
        
        string prefabPath = Application.dataPath + "/Resources/Prefabs/UI";
        DirectoryInfo direction = new DirectoryInfo(prefabPath);
        FileInfo[] files = direction.GetFiles("*.prefab", SearchOption.AllDirectories);
        cSWrite.Reset();
        cSWrite.WriteLine("using Assets.Scripts.UI;");
        cSWrite.WriteLine("using LitFramework.HotFix; ");
        cSWrite.WriteLine("internal partial class ResPath");
        cSWrite.StartBracket();
        cSWrite.WriteLine("internal partial class UI");
        cSWrite.StartBracket();
        foreach (FileInfo fileInfo in files)
        {
            if("Canvas_Loading.prefab".Equals(fileInfo.Name))
            {
                continue;
            }
            string _Name = fileInfo.Name.Replace(".prefab", "");
            if (fileInfo.Name.StartsWith("Canvas_"))
            {
                string className = _Name.Replace("Canvas_", "UI");
                int startIndex = Application.dataPath.Length + 11;
                int lengthPath = fileInfo.FullName.Length - startIndex - 7;
                string path = fileInfo.FullName.Substring(startIndex, lengthPath);
                path = path.Replace("\\", "/");
                cSWrite.WriteLine("public const string {0} = \"{1}\"; ", className.ToUpper(), path);
            }
            else if (fileInfo.Name.StartsWith("ExCom_") )
            {
                string className = _Name.Replace("ExCom_", "ExCom");
                int startIndex = Application.dataPath.Length + 11;
                int lengthPath = fileInfo.FullName.Length - startIndex - 7;
                string path = fileInfo.FullName.Substring(startIndex, lengthPath);
                path = path.Replace("\\", "/");
                cSWrite.WriteLine("public const string {0} = \"{1}\"; ", className.ToUpper(), path);
            }
            else if (fileInfo.Name.StartsWith("Element_"))
            {
                string className = _Name.Replace("Element_", "Element");
                int startIndex = Application.dataPath.Length + 11;
                int lengthPath = fileInfo.FullName.Length - startIndex - 7;
                string path = fileInfo.FullName.Substring(startIndex, lengthPath);
                path = path.Replace("\\", "/");
                cSWrite.WriteLine("public const string {0} = \"{1}\"; ", className.ToUpper(), path);
            }
        }
        cSWrite.EndBracket();
        //cSWrite.WriteLine("public static partial class UIPanel");
        //cSWrite.StartBracket();
        //foreach (FileInfo fileInfo in files)
        //{
        //    if (fileInfo.Name.StartsWith("Canvas_") && !"Canvas_Loading.prefab".Equals(fileInfo.Name))
        //    {
        //        string className = "UI" + fileInfo.Name.Substring(7, fileInfo.Name.Length - 14);

        //        string csstring= "public static "+ className+" "+ className+ " { get => ("+ className+ ")UIManager.Instance.LoadUIToAndFromAllList(UIPanelPath."+ className.ToUpper()+ ");}";
        //        cSWrite.WriteLine(csstring);
        //    }
        //}
        //cSWrite.EndBracket();
        cSWrite.EndBracket();
        cSWrite.Save(floder, "ResPath.cs");
        Debug.Log("预制路径更新完成");
    }

    [MenuItem(@"Assets/UI/DeleteUI", priority = 1)]
    private static void DeleteUI()
    {

        Object[] objs = Selection.GetFiltered<Object>(SelectionMode.Unfiltered);
        if(objs.Length==1)
        {
            Object select = objs[0];
            string path = AssetDatabase.GetAssetPath(select);
            if (path.EndsWith(".cs") && (select.name.StartsWith("UI") || select.name.StartsWith("ExCom") || select.name.StartsWith("Element")))
            {
                int startIndex = 0;
                if (path.Contains("UIExport"))
                {
                    startIndex = path.IndexOf("UIExport") + 9;
                }
                else if (path.Contains("UI"))
                {
                    startIndex = path.IndexOf("UI") + 3;
                }
                int endLen = path.Length - startIndex - (select.name.Length + 3);
                string folder = path.Substring(startIndex, endLen);
                if (select.name.StartsWith("UI"))
                {
                    string _name = select.name.Replace("Canvas_", "");
                    DeleteOne(_name, folder, "Canvas_", "UI");
                }
                else if (select.name.StartsWith("ExCom"))
                {
                    string _name = select.name.Replace("ExCom_", "");
                    DeleteOne(_name, folder, "ExCom_", "ExCom");
                }
                else if (select.name.StartsWith("Element"))
                {
                    string _name = select.name.Replace("Element_", "");
                    DeleteOne(_name, folder, "Element_", "Element");
                }
            }
            else if (path.EndsWith(".prefab")&&(select.name.StartsWith("Canvas_") || select.name.StartsWith("ExCom_") || select.name.StartsWith("Element_")))
            {
                int startFolerIndex = "Assets/Resources/Prefabs/UI/".Length;
                int endLength = path.Length - startFolerIndex - (select.name.Length + 7);
                string folder = path.Substring(startFolerIndex, endLength);
                if (select.name.StartsWith("Canvas_") )
                {
                    string _name = select.name.Replace("Canvas_","");
                    DeleteOne(_name, folder, "Canvas_", "UI");
                }else if ( select.name.StartsWith("ExCom_") )
                {
                    string _name = select.name.Replace("ExCom_", "");
                    DeleteOne(_name, folder, "ExCom_", "ExCom");
                }
                else if ( select.name.StartsWith("Element_"))
                {
                    string _name = select.name.Replace("Element_", "");
                    DeleteOne(_name, folder, "Element_", "Element");
                }
            }
            else if (!path.Contains(".")&& path.Contains("UI"))
            {
                AllDelete("Canvas","UI");
                AllDelete("Element", "Element");
                AllDelete("ExCom", "ExCom");
            }
        }else
        {
            AllDelete("Canvas", "UI");
            AllDelete("Element", "Element");
            AllDelete("ExCom", "ExCom");
        }
        EditorUtility.DisplayProgressBar("路径更新", "", 0);
        //路径更新
        UpdataPath(mCSWrite);
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log("删除成功");
    }

   static Dictionary<string, int> classFloderNameList = new Dictionary<string, int>();
    /// <summary>
    /// 删除导出cs和预制没有对应的
    /// </summary>
    /// <param name="prefabF">预制前缀</param>
    /// <param name="csF">cs导出文件前缀</param>
    private static void AllDelete(string prefabF,string csF)
    {
        classFloderNameList.Clear();
        List<string> deleteList = new List<string>();
        string prefabUIPath = Application.dataPath + "/Resources/Prefabs/UI/";
        string[] paths = Directory.GetFiles(prefabUIPath,prefabF+ "_*.prefab", SearchOption.AllDirectories);
        float curPro = 0;
        int allPro = paths.Length;
        foreach (string path in paths)
        {
            int startIndex = prefabUIPath.Length;
            string floder = path.Substring(startIndex);
            floder = floder.Replace("Canvas_", "");
            string classFloderName = floder.Replace(".prefab", "");
            classFloderName.Replace("\\", "/");
            classFloderNameList.Add(classFloderName, 1);
            curPro = curPro + 1;
            EditorUtility.DisplayProgressBar("检测"+prefabF+"预制删除", classFloderName, curPro / allPro);
        }
        //string custonPath;
        //if (!FrameworkConfig.Instance.UseHotFixMode)
        //{
        //    custonPath = Application.dataPath + "/Scripts/UI/";
        //}
        //else
        //{
        //    custonPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UI/";
        //}
        //paths = Directory.GetFiles(custonPath, "UI*.cs", SearchOption.AllDirectories);

        // curPro = 0;
        // allPro = paths.Length;
        //foreach (string path in paths)
        //{
        //    int startIndex = custonPath.Length;
        //    string floder = path.Substring(startIndex);
        //    floder = floder.Replace(csF, "");
        //    string classFloderName = floder.Replace(".cs", "");
        //    classFloderName.Replace("\\", "/");
        //    if(!classFloderNameList.ContainsKey(classFloderName))
        //    {
        //        classFloderNameList.Add(classFloderName, 1);
        //    }
        //    else
        //    {
        //        classFloderNameList[classFloderName] = classFloderNameList[classFloderName] + 1;
        //    }

        //    EditorUtility.DisplayProgressBar("自定义脚本检测", classFloderName, curPro / allPro);
        //}
        string exportCS;
        if (!FrameworkConfig.Instance.UseHotFixMode)
        {
            exportCS = Application.dataPath + "/Scripts/UIExport/";
        }
        else
        {
            exportCS = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport/";
        }
        paths = Directory.GetFiles(exportCS, csF+"*.cs", SearchOption.AllDirectories);

        curPro = 0;
        allPro = paths.Length;
        foreach (string path in paths)
        {
            int startIndex = exportCS.Length;
            string floder = path.Substring(startIndex);
            floder = floder.Replace(csF, "");
            string classFloderName = floder.Replace(".cs", "");
            classFloderName.Replace("\\", "/");
            if (!classFloderNameList.ContainsKey(classFloderName))
            {
                classFloderNameList.Add(classFloderName, 1);
            }
            else
            {
                classFloderNameList[classFloderName] = classFloderNameList[classFloderName] + 1;
            }

            EditorUtility.DisplayProgressBar("检测"+csF+"导出代码删除", classFloderName, curPro / allPro);
        }
        classFloderNameList.Remove("Loading");
        curPro = 0;
        allPro = classFloderNameList.Count;
        foreach (KeyValuePair<string, int> keyValue in classFloderNameList)
        {
            if (keyValue.Value < 2)
            {
                string classFloderName = keyValue.Key;
                string[] sp = classFloderName.Split('/');
                string _Name = sp[sp.Length - 1];
                string floder = classFloderName.Substring(0, classFloderName.Length - _Name.Length);
                DeleteOne(_Name, floder, prefabF,csF);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    private static void DeleteOne(string aName,string aFloder,string aPrefabF,string aCsF)
    {
        string prefabPath = Application.dataPath+ "/Resources/Prefabs/UI/" + aFloder + aPrefabF + aName + ".prefab";
        if(File.Exists(prefabPath))
        {
            File.Delete(prefabPath);
        }
        string custonPath;
        if (!FrameworkConfig.Instance.UseHotFixMode)
        {
            custonPath = Application.dataPath + "/Scripts/UI/" + aFloder + aCsF + aName + ".cs";
        }
        else
        {
            custonPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UI/" + aFloder + aCsF + aName + ".cs";
        }
        if (File.Exists(custonPath))
        {
            File.Delete(custonPath);
        }
        string exportCS;
        if (!FrameworkConfig.Instance.UseHotFixMode)
        {
            exportCS = Application.dataPath + "/Scripts/UIExport/" + aFloder + aCsF + aName + ".cs";
        }
        else
        {
            exportCS = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/UIExport/" + aFloder + aCsF + aName + ".cs";
        }
        if (File.Exists(exportCS))
        {
            File.Delete(exportCS);
        }
    }
    [MenuItem(@"Assets/UI/UpdateUI", priority =1)]
    private static void UpdateUI(MenuCommand menuCommand)
    {
        string pathf = AssetDatabase.GetAssetPath(Selection.activeObject);
        CSWriteTool _WriteTool = mCSWrite;
        if (pathf.Contains("Assets/Resources/Prefabs/UI/"))
        {
            Object[] objs  = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            int len = objs.Length;
            for (int i = 0; i < len; i++)
            {
                Object obj = objs[i];

                UpdateOne(obj, _WriteTool);
                EditorUtility.DisplayProgressBar("更新预制", obj.name, (float)i / len);
            }
        }else
        {
            string prefabUIPath = Application.dataPath + "/Resources/Prefabs/UI/";
            string[] paths = Directory.GetFiles(prefabUIPath, "*.prefab", SearchOption.AllDirectories);
            int len = paths.Length;
            for (int i = 0; i < len; i++)
            {
                string pth= paths[i];
                pth = "Assets/"+ pth.Substring(Application.dataPath.Length);
                Object obj = AssetDatabase.LoadAssetAtPath(pth, typeof(Object));

                UpdateOne(obj, _WriteTool);
                EditorUtility.DisplayProgressBar("更新预制", obj.name, (float)i / len);
            }
        }
        

        //路径更新
        UpdataPath(_WriteTool);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    private static void UpdateOne(Object obj ,CSWriteTool _WriteTool)
    {
        if(!obj)
        {
            return;
        }
        if (obj.name == "Canvas_Loading")
        {
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(obj);
        if (!path.EndsWith(".prefab"))
        {
            return;
        }

        int startFolerIndex = "Assets/Resources/Prefabs/UI/".Length;
        if ( obj.name.StartsWith("Canvas_"))
        {
            GameObject _Prefab = obj as GameObject;
            Transform aniRoot = _Prefab.transform.Find("Container_Anim");
            if (!aniRoot)
            {
                GameObject newPrefab = GameObject.Instantiate(_Prefab) as GameObject;

                newPrefab.name = _Prefab.name;
                GameObject animTrans = new GameObject("Container_Anim", typeof(RectTransform));
                RectTransform recTrans = animTrans.GetComponent<RectTransform>();
                int childCount = newPrefab.transform.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    Transform tran = newPrefab.transform.GetChild(0);
                    tran.SetParent(recTrans);
                }
                animTrans.transform.SetParent(newPrefab.transform);
                recTrans.sizeDelta = Vector2.zero;
                recTrans.anchorMin = Vector2.zero;
                recTrans.anchorMax = Vector2.one;
                recTrans.anchoredPosition = Vector2.zero;
                animTrans.transform.localPosition = Vector3.zero;
                animTrans.transform.localScale = Vector3.one;
                bool isSuccess = false;
                PrefabUtility.SaveAsPrefabAsset(newPrefab, path, out isSuccess);
                if (!isSuccess)
                {
                    GameObject.DestroyImmediate(newPrefab);
                    return;
                }

                GameObject.DestroyImmediate(newPrefab);

            }
            string className = _Prefab.name.Substring(7);
            int endLength = path.Length - startFolerIndex - (_Prefab.name.Length + 7);
            string folder = path.Substring(startFolerIndex, endLength);
            Transform returnBtnTrans = aniRoot.Find("ReturnBtn");
            Button returnBtn = null;
            if (returnBtnTrans)
            {
                returnBtn = returnBtnTrans.GetComponent<Button>();
            }
            SetCustomCs(folder, "UI" + className, _WriteTool, returnBtn != null, "");

            //设置基础cs
            SetBaseCs(_Prefab, _WriteTool, folder, null, returnBtn != null, uiSummary: "");
        }
        else if (obj.name.StartsWith("ExCom_"))
        {
            string className = obj.name.Substring(6);
            int endLength = path.Length - startFolerIndex - (obj.name.Length + 7);
            string folder = path.Substring(startFolerIndex, endLength);
            SetCustomExComCs(folder, "ExCom" + className, _WriteTool);
            SetBaseExComCs(obj as GameObject, _WriteTool, folder);
        }
        else if (obj.name.StartsWith("Element_"))
        {
            string className = obj.name.Substring(8);
            int endLength = path.Length - startFolerIndex - (obj.name.Length + 7);
            string folder = path.Substring(startFolerIndex, endLength);
            SetCustomElementCs(folder, "Element" + className, _WriteTool);
            SetBaseElementCs(obj as GameObject, _WriteTool, folder);
        }
    }

    [MenuItem(@"Tools/更新ugui", priority = 1)]
    private static void UpdateUGUI()
    {
        //F:\UnityPackge\BodyLife\UnityProject
        // LDebug.Log(  System.Environment.CurrentDirectory,LogColor.orange);
        // LDebug.Log("MyDocuments:"+System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), LogColor.orange);
        // LDebug.Log("CommonProgramFiles:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonProgramFiles), LogColor.orange);
        // LDebug.Log("ProgramFiles:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), LogColor.orange);
        // LDebug.Log("CommonStartup:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartup), LogColor.orange);
        // LDebug.Log("DesktopDirectory:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory), LogColor.orange);
        //LDebug.Log("InternetCache:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache), LogColor.orange);
        //LDebug.Log("LocalizedResources:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalizedResources), LogColor.orange);
        //LDebug.Log("ProgramFiles:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), LogColor.orange);
        //LDebug.Log("CDBurning:" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.CDBurning), LogColor.orange);

       // Directory directory = new Directory(System.Environment.CurrentDirectory);


        string path = "";
        for (int i=0;i<60;i++)
        {
            path = path + System.Environment.GetFolderPath((System.Environment.SpecialFolder)i) + "\n";
        }
        LDebug.Log(path, LogColor.orange);


    }
}
public class CSWriteTool
{
    private string csText="";

    private int tableCount = 0;
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        csText = "";
        tableCount = 0;
    }
    /// <summary>
    /// 开始花括号
    /// </summary>
    public void StartBracket()
    {
        csText = csText+"\n" + CurTable() + "{";
        tableCount = tableCount + 1;
    }
    /// <summary>
    /// 写入行代码
    /// </summary>
    /// <param name="aLineCs">代码</param>
    public void WriteLine(string aLineCs)
    {
        csText = csText + "\n" + CurTable() + aLineCs;
    }
    /// <summary>
    /// 写入行代码Format参数替换
    /// </summary>
    /// <param name="aLineCs">代码</param>
    /// <param name="arg">参数</param>
    public void WriteLine(string aLineCs,params object[] arg)
    {
        string line = string.Format(@aLineCs, arg);
        csText = csText + "\n" + CurTable() + line;
    }
    /// <summary>
    /// 结束花括号
    /// </summary>
    public void EndBracket()
    {
        tableCount = tableCount - 1;
        csText = csText +"\n"+ CurTable()+ "}";
    }
    /// <summary>
    /// 换行
    /// </summary>
    private string CurTable()
    {
       string csTextT = "";
        for(int i=0;i< tableCount;i++)
        {
            csTextT = csTextT + "\t";
        }
        return csTextT;
    }
    /// <summary>
    /// 获取代码
    /// </summary>
    public string ReadCS()
    {
        return csText;
    }
    /// <summary>
    /// 保存到文件
    /// </summary>
    /// <param name="aPath"></param>
    public void Save(string aPath)
    {
        FileStream fs = new FileStream(aPath, FileMode.OpenOrCreate);
        fs.SetLength(0);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        csText= csText.Trim();
        sw.Write(csText);
        fs.Flush();
        sw.Close();
        sw.Dispose();
        fs.Dispose();
        fs.Close();
    }
    /// <summary>
    /// 保存到文件
    /// </summary>
    /// <param name="aPath"></param>
    /// <param name="aClassName"></param>
    public void Save(string aPath, string aClassName)
    {
        if (!Directory.Exists(aPath))
        {
            //该路径不存在
            Directory.CreateDirectory(aPath);
        }
        aPath += "/";
        aPath += aClassName;

        FileStream fs = new FileStream(aPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        fs.SetLength(0);
        StreamWriter sw = new StreamWriter(fs,System.Text.Encoding.UTF8);
        csText=csText.Trim();
        sw.Write(csText);
        fs.Flush();
        sw.Close();
        sw.Dispose();
        fs.Dispose();
        fs.Close();
    }
}

[InitializeOnLoad]
public class CheckUIManager 
{
    // 层级窗口项回调
    private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
    /// <summary>
    /// 静态构造
    /// </summary>
    static CheckUIManager()
    {
      //  hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(DrawHierarchyIcon);
       // EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)System.Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, hiearchyItemCallback);
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
        EditorApplication.hierarchyChanged += Check;
        PrefabStage.prefabStageOpened += OpenPrefab;
        PrefabStage.prefabStageClosing += ClosePrefab;
        
        DuplicateTrans = new List<Transform>();
    }
    static bool isCanChcek = false;
    static List<Transform> DuplicateTrans;
    private static Transform CurPrefab;
    static void OpenPrefab(PrefabStage prefabStage)
    {
        GameObject obj=  prefabStage.prefabContentsRoot;
        if(obj.name.StartsWith("Canvas_"))
        {
            CurPrefab = obj.transform;
            DuplicateTrans.Clear();
            CheckName(CurPrefab);
            isCanChcek = true;
        }
     
    }
    static void ClosePrefab(PrefabStage prefabStage)
    {
        isCanChcek = false;
        DuplicateTrans.Clear();
    }
    private static void Check()
    {
        if(!isCanChcek)
        {
            return;
        }
        DuplicateTrans.Clear();
        CheckName(CurPrefab);
    }
    private static void CheckName(Transform aCurCheck)
    {
        if(aCurCheck.childCount==0)
        {
            return;
        }else if (aCurCheck.childCount == 1)
        {
            CheckName(aCurCheck.GetChild(0));
        }
        foreach (Transform trans in aCurCheck)
        {
            if(trans.name.StartsWith("_"))
            {
                Transform first = aCurCheck.Find(trans.name);
                if (first != trans)
                {
                    DuplicateTrans.Add(trans);
                    if (!DuplicateTrans.Contains(first))
                    {
                        DuplicateTrans.Add(first);
                    }
                }
            }
            CheckName(trans);
        }
    }


    // 绘制icon方法
    private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
    {
        if (!isCanChcek)
        {
            return;
        }
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if(!obj)
        {
            return;
        }
        if(DuplicateTrans.Contains(obj.transform))
        {
            GUI.contentColor = Color.red;
            var index = 0;

            Rect rectCheck = new Rect(selectionRect);
            rectCheck.x += rectCheck.width - 20;
            rectCheck.width = 18;

            DrawRectIcon(selectionRect,EditorIcons.UnityErrorIcon ,ref index);
            rectCheck.x += rectCheck.width - 20;
            rectCheck.width = 58;
            GUI.Label(rectCheck,"重名");

            GUI.contentColor = Color.white;
        }
    }


    private static Rect GetRect(Rect selectionRect, int index)
    {
        Rect rect = new Rect(selectionRect);
        rect.x += rect.width - 20 - (20 * index);
        rect.width = 18;
        return rect;
    }

    private static void DrawRectIcon(Rect selectionRect, Texture2D texture, ref int order)
    {
        order += 1;
        var rect = GetRect(selectionRect, order);

        GUI.Label(rect, texture);
    }
}
//[CreateAssetMenu]
//public class UGUIPrefabTool : ScriptableObject
//{
//    //private static UGUIPrefabTool _instance;
//    //public static UGUIPrefabTool Instance
//    //{
//    //    get
//    //    {
//    //        if (_instance == null)
//    //        {
//    //            _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<UGUIPrefabTool>("Assets/Editor/UIGUIPrefabData.asset");
//    //        }
//    //        return _instance;
//    //    }
//    //}
//    public FontData DefueltFont = new FontData
//    {
//        fontSize = 14,
//        lineSpacing = 1f,
//        fontStyle = FontStyle.Normal,
//        bestFit = false,
//        minSize = 10,
//        maxSize = 40,
//        alignment = TextAnchor.UpperLeft,
//        horizontalOverflow = HorizontalWrapMode.Wrap,
//        verticalOverflow = VerticalWrapMode.Truncate,
//        richText = true,
//        alignByGeometry = false
//    };
//    public Color[] DefueltColor;
//}
//[CreateAssetMenu]//可以直接在Project右键创建
//public class MySciptObj : ScriptableObject
//{
//    public List<int> myObjs;

//    public void Print()
//    {
       
//    }

//    public void Save(string name, int age)
//    {
//        myObjs.Add(1);
//    }
//}