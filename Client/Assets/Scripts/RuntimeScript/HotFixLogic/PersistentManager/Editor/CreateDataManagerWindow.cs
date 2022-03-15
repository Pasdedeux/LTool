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

public class CreateDataManagerWindow : OdinEditorWindow
{
    [MenuItem("本地数据/删除所有本地数据")]
    private static void DeleteData()
    {
        string fullPath = Application.persistentDataPath;
        DirectoryInfo direction = new DirectoryInfo(fullPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".bin"))
            {
                string FilePath = fullPath + "/" + files[i].Name;
                File.Delete(FilePath);
            }

        }
        AssetDatabase.Refresh();
    }

    public enum CreateType
    {
        [LabelText("创建本地数据和对应管理类")]
        CreateLocalAndManager,
        [LabelText("创建配置数据类")]
        CreateConfigData,
        [LabelText("创建数据管理类")]
        CreateManager,
        [LabelText("创建本地数据类")]
        CreateLocalData,

    }
    /// <summary>
    ///持久化目录全路径
    /// </summary>
    private static string m_HeadAllPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/PersistentManager/";
    /// <summary>
    /// 持久化目录Asstes半路径
    /// </summary>
    private static string m_HeadAssetsPath = "Assets/Scripts/RuntimeScript/HotFixLogic/PersistentManager/";
    /// <summary>
    /// 基础文件存放目录
    /// </summary>
    private static string m_BaseDire = "Base/";
    /// <summary>
    /// 本地数据cs目录
    /// </summary>
    private static string m_LocalDataDire = "LocalData/";
    /// <summary>
    /// 数据功能管理类cs目录
    /// </summary>
    private static string m_ManagerDire= "Manager/";
    /// <summary>
    /// 配置类cs目录
    /// </summary>
    private static string m_ConfigDataDire = "ConfigData/";
    /// <summary>
    /// 工具类cs目录
    /// </summary>
    private static string m_EditorDire = "Editor/";
    /// <summary>
    /// 本地数据类名后缀
    /// </summary>
    private static string m_LocalDataSuffix = "LocalData";
    /// <summary>
    /// 数据功能管理类类名后缀
    /// </summary>
    private static string m_ManagerSuffix = "Manager";
    /// <summary>
    /// 配置类类名后缀
    /// </summary>
    private static string m_ConfigDataSuffix = "Data";
    /// <summary>
    /// 代码文件类型
    /// </summary>
    private static string m_FileType = ".cs";
    /// <summary>
    /// 新文件默认名字
    /// </summary>
    private static string m_NewFileName = "New";
    /// <summary>
    /// 本地数据综合管理类类名
    /// </summary>
    private static string m_DataManagerName = "DataManager";
    /// <summary>
    /// 本地数据基类名字
    /// </summary>
    private static string m_LocalDataBaseName = "LocalDataBase";
    /// <summary>
    /// 数据功能管理类基类名字
    /// </summary>
    private static string m_BaseLocalConfigManagerName = "BaseLocalConfigManager";
    /// <summary>
    /// 玩家账号数据类类名
    /// </summary>
    private static string m_AccountName = "Account";
    /// <summary>
    /// 公共数据存放类类名
    /// </summary>
    private static string m_FuncRecorName = "FuncRecord";
    /// <summary>
    /// 本地数据工具类类名
    /// </summary>
    private static string m_LocalDataToolWindowName = "LocalDataToolWindow";
    /// <summary>
    /// 配置数据工具类类名
    /// </summary>
    private static string m_ConfigToolWindowName = "ConfigToolWindow";

    CSWriteTool csWrite=new CSWriteTool();
    [MenuItem(@"Assets/本地数据/Build", priority = 0)]
    public static CreateDataManagerWindow OpenWindow()
    {
        var window = GetWindow<CreateDataManagerWindow>();
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
        IsCreateConfigData = false;
        IsCreateManager = false;
        IsCreateLocalData = false;
        if (createType== CreateType.CreateLocalAndManager)
        {
            IsCreateManager = true;
            IsCreateLocalData = true;
        }else if(createType == CreateType.CreateConfigData)
        {
            IsCreateConfigData = true;
        }
        else if (createType == CreateType.CreateManager)
        {
            IsCreateManager = true;
        }
        else if (createType == CreateType.CreateLocalData)
        {
            IsCreateLocalData = true;
        }
    }

    [LabelText("创建类型")]
    [OnValueChanged("CheckCreateTypeValid")]
    public CreateType createType = CreateType.CreateLocalAndManager;
    private bool IsCreateConfigData=false;
    private bool IsCreateManager=true;
    private bool IsCreateLocalData=true;


    [Button("创建脚本", ButtonSizes.Medium, ButtonHeight = 40), GUIColor(0f, 0.8f, 0f)]
    private void CreateCS()
    {
        if (!CheckClassNameValid(ClassName))
        {
            Debug.LogError("类名不可以为空");
            return;
        }
       
        if(IsCreateConfigData)
        {
            CreateConfigData(csWrite, ClassName);
            SetConfigEditorCS(csWrite);
        }else
        {
            if (IsCreateLocalData)
            {
                CreateLocalData(csWrite, ClassName);
            }
            if (IsCreateManager)
            {
                CreateManager(csWrite, ClassName);
            }
            SetEditorCS(csWrite);
        }
        UpdataLacalDataManager(csWrite);
        AssetDatabase.Refresh();
        this.Close();
    }

   
    private static void CreateLocalData(CSWriteTool csWrite, string aName)
    {
        string managerFoderPath = m_HeadAllPath+m_ManagerDire;
        if (!Directory.Exists(managerFoderPath))
        {
            Directory.CreateDirectory(managerFoderPath);
        }
        string managerFilePath = managerFoderPath + aName + m_ManagerSuffix+m_FileType;
        string locaFoderPath = m_HeadAllPath+m_LocalDataDire;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + m_LocalDataSuffix+m_FileType;
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
            aName = m_NewFileName;
            localFilePath = locaFoderPath + aName + m_LocalDataSuffix+m_FileType;
        }
        if (File.Exists(localFilePath))
        {
            LDebug.LogError(aName + m_LocalDataSuffix+m_FileType+ "已经存在");
            return;
        }


        csWrite.WriteLine("public class {0}:"+m_LocalDataBaseName, aName+ m_LocalDataSuffix);
        csWrite.StartBracket();
        csWrite.WriteLine("");
        //if (File.Exists(managerFilePath))
        //{
        //    string managerStr=File.ReadAllText(managerFilePath);
        //    if(!managerStr.Contains(aName+ m_LocalDataSuffix))
        //    {
        //        int startIndex = managerStr.IndexOf('{');
        //        managerStr.IndexOf("\n\tpublic " + aName + m_LocalDataSuffix + " LocalData;", startIndex);
        //        File.WriteAllText(managerFilePath, managerStr, System.Text.Encoding.UTF8);
        //    }

        //}
        csWrite.EndBracket();
        csWrite.Save(localFilePath);
        Log.TraceInfo("本地数据"+ aName + m_LocalDataSuffix + "创建完成");
    }
   
    private static void CreateConfigData(CSWriteTool csWrite, string aName)
    {
       
       
        string locaFoderPath = m_HeadAllPath+m_ConfigDataDire;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + m_ConfigDataSuffix+m_FileType;
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
            aName = m_NewFileName;
            localFilePath = locaFoderPath + aName + m_ConfigDataSuffix+m_FileType;
        }
        if (File.Exists(localFilePath))
        {
            Debug.LogError(aName + m_ConfigDataSuffix + m_FileType+"已经存在");
            return;
        }


        csWrite.WriteLine("public class {0}", aName + m_ConfigDataSuffix);
        csWrite.StartBracket();
        csWrite.WriteLine("");
        csWrite.WriteLine(" public static {0} Instance;", aName+ m_ConfigDataSuffix);
        csWrite.EndBracket();
        csWrite.Save(localFilePath);
        Log.TraceInfo("配置数据"+ aName + m_ConfigDataSuffix+"创建完成");
    }
    private static void CreateManager()
    {
        CreateManager(new CSWriteTool(), null);
        AssetDatabase.Refresh();
    }
    private static void CreateManager(CSWriteTool csWrite,string aName)
    {
        string managerFoderPath = m_HeadAllPath+m_ManagerDire;
        if (!Directory.Exists(managerFoderPath))
        {
            Directory.CreateDirectory(managerFoderPath);
        }
        string managerFilePath= managerFoderPath + aName + m_ManagerSuffix+m_FileType;
        string locaFoderPath = m_HeadAllPath+m_LocalDataDire;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        string localFilePath = locaFoderPath + aName + m_LocalDataSuffix+m_FileType;
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
            aName = m_NewFileName; 
            managerFilePath = managerFoderPath + aName + m_ManagerSuffix+m_FileType;
            localFilePath = locaFoderPath + aName + m_LocalDataSuffix+m_FileType;
        }
        if(File.Exists(managerFilePath))
        {
            Debug.LogError(aName + m_LocalDataSuffix + m_FileType+ "已经存在");
            return;
        }
        csWrite.WriteLine("public class {0} : {1}<{2}>", aName+ m_ManagerSuffix, m_BaseLocalConfigManagerName, aName + m_ManagerSuffix);
        csWrite.StartBracket();
        if(File.Exists(localFilePath))
        {
            csWrite.WriteLine("public {0} LocalData;", aName+m_LocalDataSuffix);
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
        Log.TraceInfo("管理" + aName + m_ManagerSuffix+"创建完成");
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
        if (path.Contains(m_HeadAssetsPath+m_ConfigDataDire))
        {
            AssetDatabase.DeleteAsset(path);
            UpdataLacalDataManager();
        }
        else if(path.Contains(m_HeadAssetsPath + m_LocalDataDire))
        {
            AssetDatabase.DeleteAsset(path);
            UpdataLacalDataManager();
        }
        else if (path.Contains(m_HeadAssetsPath + m_ManagerDire))
        {
            AssetDatabase.DeleteAsset(path);
            UpdataLacalDataManager();
        }
    }
    private static void UpdataLacalDataManager(CSWriteTool csWrite)
    {
        string className = m_DataManagerName;
        string[] ManagerFileNames = null;
        string managerFoderPath = m_HeadAllPath+m_ManagerDire;
        if (!Directory.Exists(managerFoderPath))
        {
            Directory.CreateDirectory(managerFoderPath);
        }
        else
        {
             ManagerFileNames = Directory.GetFiles(managerFoderPath, "*"+m_FileType);

            for (int i = 0; i < ManagerFileNames.Length; i++)
            {
                ManagerFileNames[i] = ManagerFileNames[i].Replace(managerFoderPath, "").Replace(m_ManagerSuffix+m_FileType, "");
            }
        }
        string configFoderPath = m_HeadAllPath+m_ConfigDataDire;
        string[] ConfigFileNames = null;
        if (!Directory.Exists(configFoderPath))
        {
            Directory.CreateDirectory(configFoderPath);
        }else
        {
            ConfigFileNames = Directory.GetFiles(configFoderPath, "*"+m_FileType);
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                ConfigFileNames[i] = ConfigFileNames[i].Replace(configFoderPath, "").Replace(m_ConfigDataSuffix+m_FileType, "");
            }
        }
       

        string locaFoderPath = m_HeadAllPath+m_LocalDataDire;
        string[] LocalDataFileNames = null;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }
        else
        {
            LocalDataFileNames = Directory.GetFiles(locaFoderPath, "*"+m_FileType);
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                LocalDataFileNames[i] = LocalDataFileNames[i].Replace(locaFoderPath, "").Replace(m_LocalDataSuffix+m_FileType, "");
            }
        }
        string filePath = m_HeadAllPath+m_BaseDire + className + m_FileType;
        
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
        csWrite.WriteLine("public "+m_AccountName+m_LocalDataSuffix+" "+m_AccountName+"Local;");
        csWrite.WriteLine("public "+m_FuncRecorName+m_LocalDataSuffix+" "+m_FuncRecorName+"Local;");
        if(LocalDataFileNames!=null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("public {0} {1}Local;", fileName+m_LocalDataSuffix, fileName);
            }
        }
        
        //加载Data
        csWrite.WriteLine("private void LoadData()");
        csWrite.StartBracket();
       
        csWrite.WriteLine(m_AccountName+"Local = LocalDataHandle.LoadData<"+m_AccountName+m_LocalDataSuffix+">();");
        csWrite.WriteLine(m_FuncRecorName+"Local = LocalDataHandle.LoadData<"+m_FuncRecorName+m_LocalDataSuffix+">();");


        csWrite.WriteLine("");
        if (ConfigFileNames != null)
        {
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                string fileName = ConfigFileNames[i];
                csWrite.WriteLine("{0}.Instance= LocalDataHandle.LoadConfig<{1}>();", fileName + m_ConfigDataSuffix, fileName+m_ConfigDataSuffix);
            }
        }
        csWrite.WriteLine("");

        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("{0}Local = LocalDataHandle.LoadData<{1}>();", fileName, fileName+m_LocalDataSuffix);
            }
        }
        csWrite.EndBracket();
        //Manager的LocalData赋值
        csWrite.WriteLine("private void SetPlayerData()");
        csWrite.StartBracket();
        csWrite.WriteLine(m_AccountName+m_ManagerSuffix+".Instance.LocalData = "+m_AccountName+"Local;");
        csWrite.WriteLine(m_FuncRecorName+m_ManagerSuffix+".Instance.LocalData = "+m_FuncRecorName+"Local;");
        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                if (managerFileNameList.Contains(fileName))
                {
                    csWrite.WriteLine(fileName+m_ManagerSuffix+".Instance.LocalData = {0}Local;", fileName, fileName);
                }
            }
        }
       
        csWrite.EndBracket();
        //Manager初始化
        csWrite.WriteLine("private void InstallManagers()");
        csWrite.StartBracket();
        csWrite.WriteLine(m_AccountName+m_ManagerSuffix+".Instance.Install();");
        csWrite.WriteLine(m_FuncRecorName+m_ManagerSuffix+".Instance.Install();");
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
        csWrite.WriteLine("if (0L !="+m_AccountName+"Local.CreateAccountTime)");
        csWrite.StartBracket();
        csWrite.WriteLine("return;");
        csWrite.EndBracket();
        csWrite.WriteLine(m_AccountName+m_ManagerSuffix+".Instance.FirstIniteData();");
        csWrite.WriteLine(m_FuncRecorName+m_ManagerSuffix+".Instance.FirstIniteData();");
        if (ManagerFileNames != null)
        {
            for (int i = 0; i < ManagerFileNames.Length; i++)
            {
                string fileName = ManagerFileNames[i];
                csWrite.WriteLine("{0}Manager.Instance.FirstIniteData();", fileName);
            }
        }
        csWrite.WriteLine(m_AccountName+"Local.CreateAccountTime = LitTool.GetTimeStamp();");
        csWrite.WriteLine("SaveAllFlag();");
        csWrite.WriteLine("SaveData();");
        csWrite.EndBracket();
       
        //保存标记
        csWrite.WriteLine("public void SaveAllFlag()");
        csWrite.StartBracket();
        csWrite.WriteLine(m_AccountName+"Local.SaveFlag();");
        csWrite.WriteLine(m_FuncRecorName+"Local.SaveFlag();");
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
        csWrite.WriteLine(m_AccountName+"Local.SaveImmit();");
        csWrite.WriteLine(m_FuncRecorName+"Local.SaveImmit();");
        if (LocalDataFileNames != null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string fileName = LocalDataFileNames[i];
                csWrite.WriteLine("{0}Local.SaveImmit();", fileName);
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

        csWrite.WriteLine(m_AccountName+"Local = null;");
        csWrite.WriteLine(m_FuncRecorName+"Local= null;");
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
        Log.TraceInfo("本地数据"+m_DataManagerName+"跟新完成");
    }
    private static void SetEditorCS(CSWriteTool csWrite)
    {
        string className = m_LocalDataToolWindowName;
        string editorFoderPath = m_HeadAllPath+ m_EditorDire;
        if (!Directory.Exists(editorFoderPath))
        {
            Directory.CreateDirectory(editorFoderPath);
        }
        string locaFoderPath = m_HeadAllPath+m_LocalDataDire;
        string[] LocalDataFileNames= null;
        if (!Directory.Exists(locaFoderPath))
        {
            Directory.CreateDirectory(locaFoderPath);
        }else
        {
            LocalDataFileNames = Directory.GetFiles(locaFoderPath, "*"+m_FileType);
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                LocalDataFileNames[i] = LocalDataFileNames[i].Replace(locaFoderPath, "").Replace(m_LocalDataSuffix+m_FileType, "");
            }

        }



        string configFoderPath = m_HeadAllPath+m_ConfigDataDire;
        string[] ConfigFileNames = null;
        if (!Directory.Exists(configFoderPath))
        {
            Directory.CreateDirectory(configFoderPath);
        }
        else
        {
            ConfigFileNames = Directory.GetFiles(configFoderPath, "*"+m_FileType);
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                ConfigFileNames[i] = ConfigFileNames[i].Replace(configFoderPath, "").Replace(m_ConfigDataSuffix+m_FileType, "");
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
        csWrite.WriteLine("[Button(\""+m_FuncRecorName+m_LocalDataSuffix+"\", buttonSize: ButtonSizes.Large), GUIColor("+ c0 .r+ "f, "+ c0.g+ "f,"+c0.b+"f)]");
        csWrite.WriteLine("public void Set"+m_FuncRecorName+m_LocalDataSuffix+"()");
        csWrite.StartBracket();
        csWrite.WriteLine(m_FuncRecorName+m_LocalDataSuffix+" LocalData = LocalDataHandle.LoadData<"+m_FuncRecorName+m_LocalDataSuffix+">();");
        csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
        csWrite.WriteLine("window.OnClose += LocalData.SaveFlag;");
        csWrite.WriteLine("window.OnClose += LocalData.SaveImmit;");
        csWrite.EndBracket();
        csWrite.WriteLine("[Button(\""+m_AccountName+m_LocalDataSuffix+"\", buttonSize: ButtonSizes.Large),GUIColor(" + c1.r + "f, " + c1.g + "f," + c1.b + "f)]");
        csWrite.WriteLine("public void Set"+m_AccountName+m_LocalDataSuffix+"()");
        csWrite.StartBracket();
        csWrite.WriteLine(m_AccountName+m_LocalDataSuffix+" LocalData = LocalDataHandle.LoadData<"+m_AccountName+m_LocalDataSuffix+">();");
        csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
        csWrite.WriteLine("window.OnClose += LocalData.SaveFlag;");
        csWrite.WriteLine("window.OnClose += LocalData.SaveImmit;");
        csWrite.EndBracket();
        if(LocalDataFileNames!=null)
        {
            for (int i = 0; i < LocalDataFileNames.Length; i++)
            {
                string localName = LocalDataFileNames[i];
                Color cTemp = i % 2 == 0 ? c0 : c1;
                csWrite.WriteLine("[Button(\"" + localName +m_LocalDataSuffix+ "\", buttonSize: ButtonSizes.Large),GUIColor(" + cTemp.r + "f, " + cTemp.g + "f," + cTemp.b + "f)]");
                csWrite.WriteLine("public void Set{0}()", localName);
                csWrite.StartBracket();
                csWrite.WriteLine("{0} LocalData = LocalDataHandle.LoadData<{1}>();", localName+m_LocalDataSuffix, localName + m_LocalDataSuffix);
                csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
                csWrite.WriteLine("window.OnClose += LocalData.SaveFlag;");
                csWrite.WriteLine("window.OnClose += LocalData.SaveImmit;");
                csWrite.EndBracket();
            }
        }
        

        csWrite.EndBracket();
        csWrite.Save(editorFoderPath + className + m_FileType);
        Log.TraceInfo("本地数据工具类"+ m_LocalDataToolWindowName+"跟新完成");
    }

    private static void SetConfigEditorCS(CSWriteTool csWrite)
    {
        string className = m_ConfigToolWindowName;
        string editorFoderPath = m_HeadAllPath+ m_EditorDire;
        if (!Directory.Exists(editorFoderPath))
        {
            Directory.CreateDirectory(editorFoderPath);
        }
        

        string configFoderPath = m_HeadAllPath+m_ConfigDataDire;
        string[] ConfigFileNames = null;
        if (!Directory.Exists(configFoderPath))
        {
            Directory.CreateDirectory(configFoderPath);
        }
        else
        {
            ConfigFileNames = Directory.GetFiles(configFoderPath, "*"+m_FileType);
            for (int i = 0; i < ConfigFileNames.Length; i++)
            {
                ConfigFileNames[i] = ConfigFileNames[i].Replace(configFoderPath, "").Replace(m_ConfigDataSuffix+m_FileType, "");
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
                    csWrite.WriteLine("[Button(\"" + localName + m_ConfigDataSuffix + "\", buttonSize: ButtonSizes.Large),GUIColor(" + cTemp.r + "f, " + cTemp.g + "f," + cTemp.b + "f)]");
                    csWrite.WriteLine("public void Set{0}()", localName);
                    csWrite.StartBracket();
                    csWrite.WriteLine("{0} LocalData = LocalDataHandle.LoadData<{1}>();", localName+m_ConfigDataSuffix, localName + m_ConfigDataSuffix);
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
                csWrite.WriteLine("{0} LocalData = LocalDataHandle.LoadConfig<{1}>();", localName+m_ConfigDataSuffix, localName + m_ConfigDataSuffix);
                csWrite.WriteLine("var window = OdinEditorWindow.InspectObject(LocalData);");
                csWrite.WriteLine("window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);");
                csWrite.WriteLine("window.OnClose +=()=>{ LocalDataHandle.SaveConfig(LocalData);};");
                csWrite.EndBracket();
            }
           
        }


        csWrite.EndBracket();
        csWrite.Save(editorFoderPath + className + m_FileType);
        Log.TraceInfo("配置数据工具"+ m_ConfigToolWindowName + "跟新完成");
    }
}
