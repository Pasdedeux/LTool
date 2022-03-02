using LitFramework.LitTool;
using LitJson;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CreatDataManagerWindow : OdinEditorWindow
{
    public enum CreateType
    {
        [LabelText("创建本地数据和对应管理类")]
        CreatLocalAndManager,
        [LabelText("创建配置数据类")]
        CreatConfigData,
        [LabelText("创建数据管理类")]
        CreatManager,
        [LabelText("创建本地数据类")]
        CreatLocalData,

    }
    private static string m_HeadAllPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/PersistentManager/";
    private static string m_HeadAssetsPath = "Assets/Scripts/RuntimeScript/HotFixLogic/PersistentManager/";
    CSWriteTool csWrite=new CSWriteTool();
    [MenuItem(@"Assets/本地数据/Build", priority = 0)]
    public static CreatDataManagerWindow OpenWindow()
    {
        var window = GetWindow<CreatDataManagerWindow>();
        window.position = Sirenix.Utilities.Editor.GUIHelper.GetEditorWindowRect().AlignCenter(350f, 200f);
        return window;
    }
    private bool CheckClassNameValid(string aValue)
    {
        if (string.IsNullOrEmpty(aValue))
        {
            return false;
        }
       
        return !string.IsNullOrWhiteSpace(aValue);
    }
    [ValidateInput("CheckClassNameValid", "类名不可以为空、空格")]
    [LabelText("类名")]
    public string ClassName;
    private void CheckCreateTypeValid()
    {
        CreatConfigData = false;
        CreatManager = false;
        CreatLocalData = false;
        if (createType== CreateType.CreatLocalAndManager)
        {
            CreatManager = true;
            CreatLocalData = true;
        }else if(createType == CreateType.CreatConfigData)
        {
            CreatConfigData = true;
        }
        else if (createType == CreateType.CreatManager)
        {
            CreatManager = true;
        }
        else if (createType == CreateType.CreatLocalData)
        {
            CreatLocalData = true;
        }
    }

    [LabelText("创建类型")]
    [OnValueChanged("CheckCreateTypeValid")]
    public CreateType createType = CreateType.CreatLocalAndManager;
    private bool CreatConfigData=false;
    private bool CreatManager=true;
    private bool CreatLocalData=true;


    [Button("创建脚本", ButtonSizes.Medium, ButtonHeight = 40), GUIColor(0f, 0.8f, 0f)]
    private void CreatCS()
    {
        if (!CheckClassNameValid(ClassName))
        {
            Debug.LogError("类名不可以为空");
            return;
        }
       
        if(CreatConfigData)
        {
            CreateConfigData(csWrite, ClassName);
            SetConfigEditorCS(csWrite);
        }else
        {
            if (CreatLocalData)
            {
                CreateLocalData(csWrite, ClassName);
            }
            if (CreatManager)
            {
                CreateManager(csWrite, ClassName);
            }
            SetEditorCS(csWrite);
        }
        UpdataLacalDataManager(csWrite);
        AssetDatabase.Refresh();
        this.Close();
    }

    private static void CreateLocalData()
    {
        string aName = "New";
        string locaFoderPath = m_HeadAllPath+"LocalData/";
        if(!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + "LocalData.cs";
        CreateLocalData(new CSWriteTool(), aName);
        AssetDatabase.Refresh();

        UnityEngine.Object obj= AssetDatabase.LoadMainAssetAtPath(m_HeadAssetsPath+"LocalData/" + aName + "LocalData.cs");
        Debug.Log(obj.GetType().ToString());
        Selection.activeObject = obj;
    }
    private static void CreateLocalData(CSWriteTool csWrite, string aName)
    {
        string managerFoderPath = m_HeadAllPath+"Manager/";
        if (!Directory.Exists(managerFoderPath))
        {
            Directory.CreateDirectory(managerFoderPath);
        }
        string managerFilePath = managerFoderPath + aName + "Manager.cs";
        string locaFoderPath = m_HeadAllPath+"LocalData/";
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + "LocalData.cs";
        csWrite.Reset();
        csWrite.WriteLine("using System.Collections;");
        csWrite.WriteLine("using Sirenix.OdinInspector;");
        csWrite.WriteLine("using System.Collections.Generic;");
        csWrite.WriteLine("using UnityEngine;");
        csWrite.WriteLine("/// <summary>");
        csWrite.WriteLine("/// 代码自动创建");
        csWrite.WriteLine("/// </summary>");
        if (string.IsNullOrEmpty(aName))
        {
            aName = "New";
            localFilePath = locaFoderPath + aName + "LocalData.cs";
        }
        if (File.Exists(localFilePath))
        {
            Debug.LogError(aName + "LocalData.cs已经存在");
            return;
        }


        csWrite.WriteLine("public class {0}:LocalDataBase", aName+ "LocalData");
        csWrite.StartBracket();
        csWrite.WriteLine("");
        //if (File.Exists(managerFilePath))
        //{
        //    string managerStr=File.ReadAllText(managerFilePath);
        //    if(!managerStr.Contains(aName+ "LocalData"))
        //    {
        //        int startIndex = managerStr.IndexOf('{');
        //        managerStr.IndexOf("\n\tpublic " + aName + "LocalData" + " LocalData;", startIndex);
        //        File.WriteAllText(managerFilePath, managerStr, System.Text.Encoding.UTF8);
        //    }

        //}
        csWrite.EndBracket();
        csWrite.Save(localFilePath);
        LDebug.Log("本地数据LocalData创建完成");
    }
    private static void CreateConfigData()
    {
        string aName = "New";
        string locaFoderPath = m_HeadAllPath+"ConfigData/";
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + "Data.cs";
        CreateConfigData(new CSWriteTool(), aName);
        AssetDatabase.Refresh();
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(m_HeadAssetsPath+"ConfigData/" + aName + "Data.cs");
        Debug.Log(obj.GetType().ToString());
        Selection.activeObject = obj;
    }
    private static void CreateConfigData(CSWriteTool csWrite, string aName)
    {
       
       
        string locaFoderPath = m_HeadAllPath+"ConfigData/";
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + "Data.cs";
        csWrite.Reset();
        csWrite.WriteLine("using System.Collections;");
        csWrite.WriteLine("using Sirenix.OdinInspector;");
        csWrite.WriteLine("using System.Collections.Generic;");
        csWrite.WriteLine("using UnityEngine;");
        csWrite.WriteLine("/// <summary>");
        csWrite.WriteLine("/// 代码自动创建");
        csWrite.WriteLine("/// </summary>");
        if (string.IsNullOrEmpty(aName))
        {
            aName = "New";
            localFilePath = locaFoderPath + aName + "Data.cs";
        }
        if (File.Exists(localFilePath))
        {
            Debug.LogError(aName + "Data.cs已经存在");
            return;
        }


        csWrite.WriteLine("public class {0}", aName + "Data");
        csWrite.StartBracket();
        csWrite.WriteLine("");
        csWrite.WriteLine(" public static {0}Data Instance;", aName);
        //if (File.Exists(managerFilePath))
        //{
        //    string managerStr=File.ReadAllText(managerFilePath);
        //    if(!managerStr.Contains(aName+ "LocalData"))
        //    {
        //        int startIndex = managerStr.IndexOf('{');
        //        managerStr.IndexOf("\n\tpublic " + aName + "LocalData" + " LocalData;", startIndex);
        //        File.WriteAllText(managerFilePath, managerStr, System.Text.Encoding.UTF8);
        //    }

        //}
        csWrite.EndBracket();
        csWrite.Save(localFilePath);
        LDebug.Log("本地数据ConfigData创建完成");
    }
    private static void CreateManager()
    {
        CreateManager(new CSWriteTool(), null);
        AssetDatabase.Refresh();
    }
    private static void CreateManager(CSWriteTool csWrite,string aName)
    {
        string managerFoderPath = m_HeadAllPath+"Manager/";
        if (!Directory.Exists(managerFoderPath))
        {
            Directory.CreateDirectory(managerFoderPath);
        }
        string managerFilePath= managerFoderPath + aName + "Manager.cs";
        string locaFoderPath = m_HeadAllPath+"LocalData/";
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + "LocalData.cs";
        csWrite.Reset();
        csWrite.WriteLine("using LitFramework;");
        csWrite.WriteLine("using LitFramework.Base;");
        csWrite.WriteLine("using System.Collections;");
        csWrite.WriteLine("using System.Collections.Generic;");
        csWrite.WriteLine("using UnityEngine;");
        csWrite.WriteLine("/// <summary>");
        csWrite.WriteLine("/// 代码自动创建");
        csWrite.WriteLine("/// </summary>");
        if (string.IsNullOrEmpty(aName))
        {
            aName = "New"; 
            managerFilePath = managerFoderPath + aName + "Manager.cs";
            localFilePath = locaFoderPath + aName + "LocalData.cs";
        }
        if(File.Exists(managerFilePath))
        {
            Debug.LogError(aName + "Manager.cs已经存在");
            return;
        }
        csWrite.WriteLine("public class {0} : BaseLocalConfigManager<{0}>", aName+ "Manager", aName+ "Manager");
        csWrite.StartBracket();
        if(File.Exists(localFilePath))
        {
            csWrite.WriteLine("public {0} LocalData;", aName+"LocalData");
        }
        csWrite.WriteLine("");
        csWrite.WriteLine("");
        csWrite.WriteLine("public override void Install()");
        csWrite.StartBracket();
        csWrite.WriteLine("");
        csWrite.EndBracket();
        csWrite.WriteLine("public override void FirstIniteData()");
        csWrite.StartBracket();
        csWrite.WriteLine("");
        csWrite.EndBracket();

        csWrite.EndBracket();
        csWrite.Save(managerFilePath);
        LDebug.Log("本地数据Manager创建完成");
    }

    [MenuItem(@"Assets/本地数据/更新", priority = 0)]
    private static void UpdataLacalDataManager()
    {
        CSWriteTool cSWrite = new CSWriteTool();
        UpdataLacalDataManager(cSWrite);
        SetEditorCS(cSWrite);
        SetConfigEditorCS(cSWrite);
        AssetDatabase.Refresh();
    }
    [MenuItem(@"Assets/本地数据/删除", priority = 0)]
    private static void DeleteDate()
    {
        string path=AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (path.Contains(m_HeadAssetsPath+"ConfigData/"))
        {
            AssetDatabase.DeleteAsset(path);
            UpdataLacalDataManager();
        }
        else if(path.Contains(m_HeadAssetsPath + "LocalData/"))
        {
            AssetDatabase.DeleteAsset(path);
            UpdataLacalDataManager();
        }
        else if (path.Contains(m_HeadAssetsPath + "Manager/"))
        {
            AssetDatabase.DeleteAsset(path);
            UpdataLacalDataManager();
        }
    }
    private static void UpdataLacalDataManager(CSWriteTool csWrite)
    {
        string className = "DataManager";
        string[] ManagerFileNames = null;
        string managerFoderPath = m_HeadAllPath+"Manager/";
        if (!Directory.Exists(managerFoderPath))
        {
            Directory.CreateDirectory(managerFoderPath);
        }
        else
        {
             ManagerFileNames = Directory.GetFiles(managerFoderPath, "*.cs");

            for (int i = 0; i < ManagerFileNames.Length; i++)
            {
                ManagerFileNames[i] = ManagerFileNames[i].Replace(managerFoderPath, "").Replace("Manager.cs", "");
            }
        }
        string configFoderPath = m_HeadAllPath+"ConfigData/";
        string[] ConfigFileNames = null;
        if (!Directory.Exists(configFoderPath))
        {
            Directory.CreateDirectory(configFoderPath);
        }else
        {
            ConfigFileNames = Directory.GetFiles(configFoderPath, "*.cs");
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                ConfigFileNames[i] = ConfigFileNames[i].Replace(configFoderPath, "").Replace("Data.cs", "");
            }
        }
       

        string locaFoderPath = m_HeadAllPath+"LocalData/";
        string[] LocalDataFileNames = null;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        else
        {
            LocalDataFileNames = Directory.GetFiles(locaFoderPath, "*.cs");
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                LocalDataFileNames[i] = LocalDataFileNames[i].Replace(locaFoderPath, "").Replace("LocalData.cs", "");
            }
        }
        string filePath = m_HeadAllPath+"Base/" + className + ".cs";
        
        List<string> managerFileNameList = new List<string>(ManagerFileNames);

        csWrite.Reset();
        csWrite.WriteLine("using LitFramework;");
        csWrite.WriteLine("using LitFramework.Base;");
        csWrite.WriteLine("using LitFramework.LitTool;");
        csWrite.WriteLine("/// 代码自动创建、更新");
        csWrite.WriteLine("/// 更新时间:" + System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day + "   " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second);
        csWrite.WriteLine("/// </summary>");
        csWrite.WriteLine("public class {0} : Singleton<{0}>,IManager", className, className);
        csWrite.StartBracket();
        csWrite.WriteLine("public System.Action DataInstallEnd;");
        csWrite.WriteLine("public void Install()");
        csWrite.StartBracket();
        csWrite.WriteLine("LoadData();");
        csWrite.WriteLine("SetPlayerData();");
        csWrite.WriteLine("CheckFristLogin();");
        csWrite.WriteLine("InstallManagers();");
        csWrite.WriteLine("GameDriver.Instance.UpdateEventHandler += SaveData;");
        csWrite.WriteLine("DataInstallEnd?.Invoke();");
        csWrite.EndBracket();
        //属性
        csWrite.WriteLine("public AccountLocalData AccountLocal;");
        csWrite.WriteLine("public FuncRecordLocalData FuncRecordLocal;");
        if(LocalDataFileNames!=null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("public {0}LocalData {1}Local;", fileName, fileName);
            }
        }
        
        //加载Data
        csWrite.WriteLine("private void LoadData()");
        csWrite.StartBracket();
       
        csWrite.WriteLine("AccountLocal = LocalDataHandle.LoadData<AccountLocalData>();");
        csWrite.WriteLine("FuncRecordLocal = LocalDataHandle.LoadData<FuncRecordLocalData>();");


        csWrite.WriteLine("");
        if (ConfigFileNames != null)
        {
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                string fileName = ConfigFileNames[i];
                csWrite.WriteLine("{0}Data.Instance= LocalDataHandle.LoadConfig<{0}Data>();", fileName, fileName);
            }
        }
        csWrite.WriteLine("");

        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("{0}Local = LocalDataHandle.LoadData<{0}LocalData>();", fileName, fileName);
            }
        }
        csWrite.EndBracket();
        //Manager的LocalData赋值
        csWrite.WriteLine("private void SetPlayerData()");
        csWrite.StartBracket();
        csWrite.WriteLine("AccountManager.Instance.LocalData = AccountLocal;");
        csWrite.WriteLine("FuncRecordManager.Instance.LocalData = FuncRecordLocal;");
        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                if (managerFileNameList.Contains(fileName))
                {
                    csWrite.WriteLine("{0}Manager.Instance.LocalData = {0}Local;", fileName, fileName);
                }
            }
        }
       
        csWrite.EndBracket();
        //Manager初始化
        csWrite.WriteLine("private void InstallManagers()");
        csWrite.StartBracket();
        csWrite.WriteLine("AccountManager.Instance.Install();");
        csWrite.WriteLine("FuncRecordManager.Instance.Install();");
        if (ManagerFileNames != null)
        {
            for (int i = 0; i < ManagerFileNames.Length; i++)
            {
                string fileName = ManagerFileNames[i];
                csWrite.WriteLine("{0}Manager.Instance.Install();", fileName);
            }
        }
        csWrite.EndBracket();
        //首次登录检测
        csWrite.WriteLine("private void CheckFristLogin()");
        csWrite.StartBracket();
        csWrite.WriteLine("if (0L !=AccountLocal.CreateAccountTime)");
        csWrite.StartBracket();
        csWrite.WriteLine("return;");
        csWrite.EndBracket();
        csWrite.WriteLine("AccountManager.Instance.FirstIniteData();");
        csWrite.WriteLine("FuncRecordManager.Instance.FirstIniteData();");
        if (ManagerFileNames != null)
        {
            for (int i = 0; i < ManagerFileNames.Length; i++)
            {
                string fileName = ManagerFileNames[i];
                csWrite.WriteLine("{0}Manager.Instance.FirstIniteData();", fileName);
            }
        }
        csWrite.WriteLine("AccountLocal.CreateAccountTime = LitTool.GetTimeStamp();");
        csWrite.WriteLine("SaveAllFlag();");
        csWrite.WriteLine("SaveData();");
        csWrite.EndBracket();
       
        //保存标记
        csWrite.WriteLine("public void SaveAllFlag()");
        csWrite.StartBracket();
        csWrite.WriteLine("AccountLocal.SaveFlag();");
        csWrite.WriteLine("FuncRecordLocal.SaveFlag();");
        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("{0}Local.SaveFlag();", fileName);
            }
        }
        csWrite.EndBracket();
        //马上保存
        csWrite.WriteLine("public void SaveData()");
        csWrite.StartBracket();
        csWrite.WriteLine("AccountLocal.SavaImmit();");
        csWrite.WriteLine("FuncRecordLocal.SavaImmit();");
        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("{0}Local.SavaImmit();", fileName);
            }
        }
        csWrite.EndBracket();
        csWrite.WriteLine("public System.Action DestroyPayerData;");
        //卸载
        csWrite.WriteLine("public void Uninstall()");
        csWrite.StartBracket();
        csWrite.WriteLine("DestroyPayerData?.Invoke();");
        csWrite.WriteLine("DestroyPayerData = null;");
        csWrite.WriteLine("SaveAllFlag();");
        csWrite.WriteLine("SaveData();");

        csWrite.WriteLine("AccountLocal = null;");
        csWrite.WriteLine("FuncRecordLocal= null;");
        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("{0}Local= null;", fileName);
            }
        }
        csWrite.EndBracket();
        csWrite.EndBracket();
        csWrite.Save(filePath);
        LDebug.Log("本地数据DataManager跟新完成");
    }
    private static void SetEditorCS(CSWriteTool csWrite)
    {
        string className = "LocalDataToolWindow";
        string editorFoderPath = m_HeadAllPath+"Editor/";
        if (!Directory.Exists(editorFoderPath))
        {
            Directory.CreateDirectory(editorFoderPath);
        }
        string locaFoderPath = m_HeadAllPath+"LocalData/";
        string[] LocalDataFileNames= null;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }else
        {
            LocalDataFileNames = Directory.GetFiles(locaFoderPath, "*.cs");
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                LocalDataFileNames[i] = LocalDataFileNames[i].Replace(locaFoderPath, "").Replace("LocalData.cs", "");
            }

        }



        string configFoderPath = m_HeadAllPath+"ConfigData/";
        string[] ConfigFileNames = null;
        if (!Directory.Exists(configFoderPath))
        {
            Directory.CreateDirectory(configFoderPath);
        }
        else
        {
            ConfigFileNames = Directory.GetFiles(configFoderPath, "*.cs");
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                ConfigFileNames[i] = ConfigFileNames[i].Replace(configFoderPath, "").Replace("Data.cs", "");
            }
        }
        Color c0 = new Color(0.1f, 0.4f, 0.8f);
        Color c1 = new Color(0f, 0.8f, 0.6f);
        csWrite.Reset();
        csWrite.WriteLine("using Sirenix.OdinInspector;");
        csWrite.WriteLine("using Sirenix.OdinInspector.Editor;");
        csWrite.WriteLine("using Sirenix.Utilities;");
        csWrite.WriteLine("using Sirenix.Utilities.Editor;");
        csWrite.WriteLine("using System.Collections;");
        csWrite.WriteLine("using System.Collections.Generic;");
        csWrite.WriteLine("using UnityEditor; ");
        csWrite.WriteLine("using UnityEngine;");
        csWrite.WriteLine("/// 代码自动创建、更新");
        csWrite.WriteLine("/// 更新时间:" + System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day + "   " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second);
        csWrite.WriteLine("/// </summary>");
        csWrite.WriteLine("public class {0} : OdinEditorWindow", className);
        csWrite.StartBracket();
        csWrite.WriteLine("[MenuItem(\"本地数据 / 修改本地数据\")]");
        csWrite.WriteLine(" public static {0} OpenWindow()", className);
        csWrite.StartBracket();
        csWrite.WriteLine(" {0} window = OdinEditorWindow.GetWindow<{1}>();", className, className);
        csWrite.WriteLine("window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);");
        csWrite.WriteLine(" return window;");
        csWrite.EndBracket();
        csWrite.WriteLine("[Button(\"FuncRecordLocalData\", buttonSize: ButtonSizes.Large), GUIColor("+ c0 .r+ "f, "+ c0.g+ "f,"+c0.b+"f)]");
        csWrite.WriteLine("public void SetFuncRecordLocalData()");
        csWrite.StartBracket();
        csWrite.WriteLine("FuncRecordLocalData LocalData = LocalDataHandle.LoadData<FuncRecordLocalData>();");
        csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
        csWrite.WriteLine("window.OnClose += LocalData.SaveFlag;");
        csWrite.WriteLine("window.OnClose += LocalData.SavaImmit;");
        csWrite.EndBracket();
        csWrite.WriteLine("[Button(\"AccountLocalData\", buttonSize: ButtonSizes.Large),GUIColor(" + c1.r + "f, " + c1.g + "f," + c1.b + "f)]");
        csWrite.WriteLine("public void SetAccountLocalData()");
        csWrite.StartBracket();
        csWrite.WriteLine("AccountLocalData LocalData = LocalDataHandle.LoadData<AccountLocalData>();");
        csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
        csWrite.WriteLine("window.OnClose += LocalData.SaveFlag;");
        csWrite.WriteLine("window.OnClose += LocalData.SavaImmit;");
        csWrite.EndBracket();
        if(LocalDataFileNames!=null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string localName = LocalDataFileNames[i];
                Color cTemp = i % 2 == 0 ? c0 : c1;
                csWrite.WriteLine("[Button(\"" + localName + "LocalData\", buttonSize: ButtonSizes.Large),GUIColor(" + cTemp.r + "f, " + cTemp.g + "f," + cTemp.b + "f)]");
                csWrite.WriteLine("public void Set{0}()", localName);
                csWrite.StartBracket();
                csWrite.WriteLine("{0}LocalData LocalData = LocalDataHandle.LoadData<{1}LocalData>();", localName, localName);
                csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
                csWrite.WriteLine("window.OnClose += LocalData.SaveFlag;");
                csWrite.WriteLine("window.OnClose += LocalData.SavaImmit;");
                csWrite.EndBracket();
            }
        }
        

        csWrite.EndBracket();
        csWrite.Save(editorFoderPath + className + ".cs");
        LDebug.Log("本地数据LocalDataEditor跟新完成");
    }

    private static void SetConfigEditorCS(CSWriteTool csWrite)
    {
        string className = "ConfigToolWindow";
        string editorFoderPath = m_HeadAllPath+"Editor/";
        if (!Directory.Exists(editorFoderPath))
        {
            Directory.CreateDirectory(editorFoderPath);
        }
        

        string configFoderPath = m_HeadAllPath+"ConfigData/";
        string[] ConfigFileNames = null;
        if (!Directory.Exists(configFoderPath))
        {
            Directory.CreateDirectory(configFoderPath);
        }
        else
        {
            ConfigFileNames = Directory.GetFiles(configFoderPath, "*.cs");
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                ConfigFileNames[i] = ConfigFileNames[i].Replace(configFoderPath, "").Replace("Data.cs", "");
            }
        }
        Color c0 = new Color(0.8f, 0.1f, 0.8f);
        Color c1 = new Color(0f, 0.8f, 0.6f);
        csWrite.Reset();
        csWrite.WriteLine("using Sirenix.OdinInspector;");
        csWrite.WriteLine("using Sirenix.OdinInspector.Editor;");
        csWrite.WriteLine("using Sirenix.Utilities;");
        csWrite.WriteLine("using Sirenix.Utilities.Editor;");
        csWrite.WriteLine("using System.Collections;");
        csWrite.WriteLine("using System.Collections.Generic;");
        csWrite.WriteLine("using UnityEditor; ");
        csWrite.WriteLine("using UnityEngine;");
        csWrite.WriteLine("/// 代码自动创建、更新");
        csWrite.WriteLine("/// 更新时间:" + System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day + "   " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second);
        csWrite.WriteLine("/// </summary>");
        csWrite.WriteLine("public class {0} : OdinEditorWindow", className);
        csWrite.StartBracket();
       
        if (ConfigFileNames != null)
        {
            if(ConfigFileNames.Length>1)
            {
                csWrite.WriteLine("[MenuItem(\"本地数据 / 本地配置数据\")]");
                csWrite.WriteLine(" public static {0} OpenWindow()", className);
                csWrite.StartBracket();
                csWrite.WriteLine(" {0} window = OdinEditorWindow.GetWindow<{1}>();", className, className);
                csWrite.WriteLine("window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);");
                csWrite.WriteLine(" return window;");
                csWrite.EndBracket();
                for (int i = 0; i < ConfigFileNames.Length; i++)
                {
                    string localName = ConfigFileNames[i];
                    Color cTemp = i % 2 == 0 ? c0 : c1;
                    csWrite.WriteLine("[Button(\"" + localName + "Data\", buttonSize: ButtonSizes.Large),GUIColor(" + cTemp.r + "f, " + cTemp.g + "f," + cTemp.b + "f)]");
                    csWrite.WriteLine("public void Set{0}()", localName);
                    csWrite.StartBracket();
                    csWrite.WriteLine("{0}Data LocalData = LocalDataHandle.LoadData<{1}Data>();", localName, localName);
                    csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
                    csWrite.WriteLine("window.OnClose +=()=>{ LocalDataHandle.SaveConfig(LocalData);};");
                    csWrite.EndBracket();
                }
            }else
            {
                int i = 0;
                string localName = ConfigFileNames[i];
                csWrite.WriteLine("[MenuItem(\"本地数据 / 本地配置数据\")]");
                csWrite.WriteLine("public static void Set{0}()", localName);
                csWrite.StartBracket();
                csWrite.WriteLine("{0}Data LocalData = LocalDataHandle.LoadConfig<{1}Data>();", localName, localName);
                csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
                csWrite.WriteLine("window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);");
                csWrite.WriteLine("window.OnClose +=()=>{ LocalDataHandle.SaveConfig(LocalData);};");
                csWrite.EndBracket();
            }
           
        }


        csWrite.EndBracket();
        csWrite.Save(editorFoderPath + className + ".cs");
        LDebug.Log("本地数据ConfigEditor跟新完成");
    }
}
