using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using AssetBundleBrowser.AssetBundleDataSource;
using LitFramework.LitTool;
using System.Collections;
using LitFramework.Crypto;
using System;
using System.Text;
using System.Linq;
using Assets.Scripts.LitCore.LitTool;
using LitFramework;

namespace AssetBundleBrowser
{
    [System.Serializable]
    internal class AssetBundleBuildTab
    {
        const string k_BuildPrefPrefix = "ABBBuild:";

        private string m_streamingPath = "Assets/StreamingAssets";

        [SerializeField]
        private bool m_AdvancedSettings;

        [SerializeField]
        private Vector2 m_ScrollPosition;


        class ToggleData
        {
            internal ToggleData(bool s,
                string title,
                string tooltip,
                List<string> onToggles,
                BuildAssetBundleOptions opt = BuildAssetBundleOptions.None)
            {
                if (onToggles.Contains(title))
                    state = true;
                else
                    state = s;
                content = new GUIContent(title, tooltip);
                option = opt;
            }
            //internal string prefsKey
            //{ get { return k_BuildPrefPrefix + content.text; } }
            internal bool state;
            internal GUIContent content;
            internal BuildAssetBundleOptions option;
        }

        private AssetBundleInspectTab m_InspectTab;

        [SerializeField]
        private BuildTabData m_UserData;

        List<ToggleData> m_ToggleData;
        ToggleData m_ForceRebuild;
        ToggleData m_CopyToStreaming;
        GUIContent m_TargetContent;
        GUIContent m_CompressionContent;
        internal enum CompressOptions
        {
            Uncompressed = 0,
            StandardCompression,
            ChunkBasedCompression,
        }
        GUIContent[] m_CompressionOptions =
        {
            new GUIContent("No Compression"),
            new GUIContent("Standard Compression (LZMA)"),
            new GUIContent("Chunk Based Compression (LZ4)")
        };
        int[] m_CompressionValues = { 0, 1, 2 };


        internal AssetBundleBuildTab()
        {
            m_AdvancedSettings = false;
            m_UserData = new BuildTabData();
            m_UserData.m_OnToggles = new List<string>();
            m_UserData.m_UseDefaultPath = true;
        }

        internal void OnDisable()
        {
            var dataPath = System.IO.Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/AssetBundleBrowserBuild.dat";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(dataPath);

            bf.Serialize(file, m_UserData);
            file.Close();

        }
        internal void OnEnable(EditorWindow parent)
        {
            m_InspectTab = (parent as AssetBundleBrowserMain).m_InspectTab;

            //LoadData...
            var dataPath = System.IO.Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/AssetBundleBrowserBuild.dat";

            if (File.Exists(dataPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(dataPath, FileMode.Open);
                var data = bf.Deserialize(file) as BuildTabData;
                if (data != null)
                    m_UserData = data;
                file.Close();
            }

            m_ToggleData = new List<ToggleData>();
            m_ToggleData.Add(new ToggleData(
                false,
                "Exclude Type Information",
                "Do not include type information within the asset bundle (don't write type tree).",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.DisableWriteTypeTree));
            m_ToggleData.Add(new ToggleData(
                false,
                "Force Rebuild",
                "Force rebuild the asset bundles",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.ForceRebuildAssetBundle));
            m_ToggleData.Add(new ToggleData(
                false,
                "Ignore Type Tree Changes",
                "Ignore the type tree changes when doing the incremental build check.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.IgnoreTypeTreeChanges));
            m_ToggleData.Add(new ToggleData(
                false,
                "Append Hash",
                "Append the hash to the assetBundle name.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.AppendHashToAssetBundleName));
            m_ToggleData.Add(new ToggleData(
                false,
                "Strict Mode",
                "Do not allow the build to succeed if any errors are reporting during it.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.StrictMode));
            m_ToggleData.Add(new ToggleData(
                false,
                "Dry Run Build",
                "Do a dry run build.",
                m_UserData.m_OnToggles,
                BuildAssetBundleOptions.DryRunBuild));


            m_ForceRebuild = new ToggleData(
                false,
                "Clear Folders",
                "Will wipe out all contents of build directory as well as StreamingAssets/AssetBundles if you are choosing to copy build there.",
                m_UserData.m_OnToggles);
            m_CopyToStreaming = new ToggleData(
                false,
                "Copy to StreamingAssets",
                "After build completes, will copy all build content to " + m_streamingPath + " for use in stand-alone player.",
                m_UserData.m_OnToggles);

            m_TargetContent = new GUIContent("Build Target", "Choose target platform to build for.");
            m_CompressionContent = new GUIContent("Compression", "Choose no compress, standard (LZMA), or chunk based (LZ4)");

            if (m_UserData.m_UseDefaultPath)
            {
                ResetPathToDefault();
            }
        }

        internal void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            bool newState = false;
            var centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(new GUIContent("Example build setup"), centeredStyle);
            //basic options
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            // build target
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildTarget))
            {
                ValidBuildTarget tgt = (ValidBuildTarget)EditorGUILayout.EnumPopup(m_TargetContent, m_UserData.m_BuildTarget);
                if (tgt != m_UserData.m_BuildTarget)
                {
                    m_UserData.m_BuildTarget = tgt;
                    if (m_UserData.m_UseDefaultPath)
                    {
                        m_UserData.m_OutputPath = "AssetBundles/";
                        m_UserData.m_OutputPath += m_UserData.m_BuildTarget.ToString();
                        //EditorUserBuildSettings.SetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath", m_OutputPath);
                    }
                }
            }


            ////output path
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildOutputDirectory))
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                var newPath = EditorGUILayout.TextField("Output Path", m_UserData.m_OutputPath);
                if (!System.String.IsNullOrEmpty(newPath) && newPath != m_UserData.m_OutputPath)
                {
                    m_UserData.m_UseDefaultPath = false;
                    m_UserData.m_OutputPath = newPath;
                    //EditorUserBuildSettings.SetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath", m_OutputPath);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                    BrowseForFolder();
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
                    ResetPathToDefault();
                //if (string.IsNullOrEmpty(m_OutputPath))
                //    m_OutputPath = EditorUserBuildSettings.GetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath");
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();

                newState = GUILayout.Toggle(
                    m_ForceRebuild.state,
                    m_ForceRebuild.content);
                if (newState != m_ForceRebuild.state)
                {
                    if (newState)
                        m_UserData.m_OnToggles.Add(m_ForceRebuild.content.text);
                    else
                        m_UserData.m_OnToggles.Remove(m_ForceRebuild.content.text);
                    m_ForceRebuild.state = newState;
                }
                newState = GUILayout.Toggle(
                    m_CopyToStreaming.state,
                    m_CopyToStreaming.content);
                if (newState != m_CopyToStreaming.state)
                {
                    if (newState)
                        m_UserData.m_OnToggles.Add(m_CopyToStreaming.content.text);
                    else
                        m_UserData.m_OnToggles.Remove(m_CopyToStreaming.content.text);
                    m_CopyToStreaming.state = newState;
                }
            }

            // advanced options
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildOptions))
            {
                EditorGUILayout.Space();
                m_AdvancedSettings = EditorGUILayout.Foldout(m_AdvancedSettings, "Advanced Settings");
                if (m_AdvancedSettings)
                {
                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 1;
                    CompressOptions cmp = (CompressOptions)EditorGUILayout.IntPopup(
                        m_CompressionContent,
                        (int)m_UserData.m_Compression,
                        m_CompressionOptions,
                        m_CompressionValues);

                    if (cmp != m_UserData.m_Compression)
                    {
                        m_UserData.m_Compression = cmp;
                    }
                    foreach (var tog in m_ToggleData)
                    {
                        newState = EditorGUILayout.ToggleLeft(
                            tog.content,
                            tog.state);
                        if (newState != tog.state)
                        {

                            if (newState)
                                m_UserData.m_OnToggles.Add(tog.content.text);
                            else
                                m_UserData.m_OnToggles.Remove(tog.content.text);
                            tog.state = newState;
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel = indent;
                }
            }

            // build.
            EditorGUILayout.Space();
            if (GUILayout.Button("Build"))
            {
                EditorApplication.delayCall += ExecuteBuild;
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void ExecuteBuild()
        {
            string outPutPath = m_UserData.m_OutputPath + "/" + FrameworkConfig.Instance.ABFolderName;
            if (AssetBundleModel.Model.DataSource.CanSpecifyBuildOutputDirectory)
            {

                if (string.IsNullOrEmpty(outPutPath))
                    BrowseForFolder();

                if (string.IsNullOrEmpty(outPutPath)) //in case they hit "cancel" on the open browser
                {
                    Debug.LogError("AssetBundle Build: No valid output path for build.");
                    return;
                }

                if (m_ForceRebuild.state)
                {
                    string message = "Do you want to delete all files in the directory " + outPutPath;
                    if (m_CopyToStreaming.state)
                        message += " and " + m_streamingPath;
                    message += "?";
                    if (EditorUtility.DisplayDialog("File delete confirmation", message, "Yes", "No"))
                    {
                        try
                        {
                            if (Directory.Exists(outPutPath))
                                Directory.Delete(outPutPath, true);

                            if (m_CopyToStreaming.state)
                                if (Directory.Exists(m_streamingPath))
                                    Directory.Delete(m_streamingPath, true);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                if (!Directory.Exists(outPutPath))
                    Directory.CreateDirectory(outPutPath);
            }

            BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;

            if (AssetBundleModel.Model.DataSource.CanSpecifyBuildOptions)
            {
                if (m_UserData.m_Compression == CompressOptions.Uncompressed)
                    opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
                else if (m_UserData.m_Compression == CompressOptions.ChunkBasedCompression)
                    opt |= BuildAssetBundleOptions.ChunkBasedCompression;
                foreach (var tog in m_ToggleData)
                {
                    if (tog.state)
                        opt |= tog.option;
                }
            }

            ABBuildInfo buildInfo = new ABBuildInfo();

            buildInfo.outputDirectory = outPutPath;
            buildInfo.options = opt;
            buildInfo.buildTarget = (BuildTarget)m_UserData.m_BuildTarget;
            buildInfo.onBuild = (assetBundleName) =>
            {
                if (m_InspectTab == null)
                    return;
                m_InspectTab.AddBundleFolder(buildInfo.outputDirectory);
                m_InspectTab.RefreshBundles();
            };

            AssetBundleModel.Model.DataSource.BuildAssetBundles(buildInfo);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            if (m_CopyToStreaming.state)
                DirectoryCopy(outPutPath, m_streamingPath);

            LitTool.MonoBehaviour.StartCoroutine(IEStartSendCSV(m_UserData.m_OutputPath));
            LitTool.MonoBehaviour.StartCoroutine(IEPathfToABSendCSV(m_UserData.m_OutputPath));
        }


        #region AB包生成

        //AB包信息文件
        private static string ABVersionName = "ABVersion.csv";
        //资源访问路径列表文件
        private static string ABPathName = "ABPath.csv";

        #region ABEditor
        public class ABVersion
        {
            public string AbName;
            public int Version;
            public string MD5;
        }

        private static bool _IsABVersionCSV;
        private static bool IsABVersionCSV() { return _IsABVersionCSV; }

        /// <summary>
        /// 写入CSV的标题栏
        /// </summary>
        static string _dataHeard = "AbName,Version,MD5";
        /// <summary>
        /// 写入CSV的值
        /// </summary>
        static string _dataHeardValue = "{0},{1},{2}";

        /// <summary>
        /// 资源所在路径
        /// </summary>
        private static string _abResPath;

        /// <summary>
        /// 生成csv文件
        /// </summary>
        public static IEnumerator IEStartSendCSV(string outpuut)
        {
            _IsABVersionCSV = false;
            _abResPath = outpuut + "/" + FrameworkConfig.Instance.ABFolderName;

            string csvPath = outpuut + "/" + ABVersionName;
            Dictionary<string, ABVersion> abVersionsDic = new Dictionary<string, ABVersion>();
            if (File.Exists(csvPath))
            {
                FileInfo fi = new FileInfo(csvPath);
                LitTool.MonoBehaviour.StartCoroutine(DocumentAccessor.ILoadAsset(fi.FullName, (string e) =>
                {
                    string[] str = e.Split('\n');
                    for (int i = 1; i < str.Length; i++)
                    {
                        string line = str[i];
                        if (line != "")
                        {
                            string[] content = line.Split(',');

                            if (File.Exists(_abResPath + "/" + content[0]))
                            {
                                string md5AquireName = content[0];
                                if (content[0].Contains(".assetbundle"))
                                {
                                    md5AquireName = md5AquireName.Split('.')[0];
                                }
                                string newMd5 = GetMD5HashFromFile(_abResPath + "/" + md5AquireName);
                                ABVersion ab;
                                content[2] = content[2].Trim();
                                if (content[2] != newMd5)
                                {
                                    if (abVersionsDic.ContainsKey(content[0]))
                                    {
                                        abVersionsDic[content[0]].Version = Convert.ToInt32(content[1]) + 1;
                                        abVersionsDic[content[0]].MD5 = newMd5;
                                    }
                                    else
                                    {
                                        ab = new ABVersion
                                        {
                                            AbName = content[0],
                                            Version = Convert.ToInt32(content[1]) + 1,
                                            MD5 = newMd5
                                        };
                                        abVersionsDic.Add(content[0], ab);
                                    }
                                }
                                else
                                {
                                    ab = new ABVersion
                                    {
                                        AbName = content[0],
                                        Version = Convert.ToInt32(content[1]),
                                        MD5 = content[2]
                                    };
                                    abVersionsDic.Add(content[0], ab);
                                }
                            }
                            else
                            {
                                abVersionsDic.Remove(content[0]);
                            }
                        }
                    }
                    MatchFiles(abVersionsDic);
                }));
            }
            else
            {
                FullfillCSVDict(abVersionsDic);
            }
            yield return new WaitUntil(IsABVersionCSV);
            List<ABVersion> abVersionsList = new List<ABVersion>();
            foreach (var item in abVersionsDic)
            {
                abVersionsList.Add(item.Value);
            }
            //导出AB压缩包
            ExportABZip(abVersionsList, outpuut);
            //导出记录版本的CSV
            ExportCSV(abVersionsList, csvPath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// 导出AB压缩包
        /// </summary>
        /// <param name="abVersionsList"></param>
        /// <param name="v"></param>
        private static void ExportABZip(List<ABVersion> abVersionsList, string output)
        {
            if (!FrameworkConfig.Instance.useZIP) return;

            List<KeyValuePair<string, string>> recordedGroup;
            var keyGrouped = abVersionsList.GroupBy(e => e.AbName.Split('.')[0]);
            foreach (var item in keyGrouped)
            {
                string fullName = null;
                try
                {
                    fullName = new FileInfo(output + "/" + FrameworkConfig.Instance.ABTotalName + "/" + item.Key + ".zip").FullName;
                    ZipManager.CompressFileWithPassword(
                    item.Select(e => new FileInfo(output + "/" + FrameworkConfig.Instance.ABTotalName + "/" + e.AbName).FullName).ToArray(), fullName, "31409");
                }
                catch (Exception)
                {
                    Log.Error($" 压缩错误：[{fullName}]", LogColor.green);
                    return;
                }
            }
        }

        /// <summary>
        /// 将ssv写入到指定目录
        /// </summary>
        /// <param name="abVersions"></param>
        /// <param name="fileName"></param>
        private static void ExportCSV(List<ABVersion> abVersions, string fileName)
        {
            if (fileName.Length > 0)
            {
                //if ( !abVersions.Exists( e => e.AbName.Equals( ABPathName ) ) )
                //    abVersions = new List<ABVersion> { new ABVersion { AbName = ABPathName, Version = 0, MD5 = string.Empty } };

                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));

                sw.WriteLine(_dataHeard);
                //写入数据
                for (int i = 0; i < abVersions.Count; i++)
                {
                    string dataStr = string.Format(_dataHeardValue, abVersions[i].AbName, abVersions[i].Version, abVersions[i].MD5);
                    sw.WriteLine(dataStr);
                }
                sw.Close();
                fs.Close();
            }
        }

        private static void FullfillCSVDict(Dictionary<string, ABVersion> abVersionsDic)
        {
            //先获取指定路径下的所有Asset，包括子文件夹下的资源
            DirectoryInfo dir = new DirectoryInfo(_abResPath);
            FileInfo[] files = dir.GetFiles();

            foreach (var file in files)
            {
                string suffix = file.FullName.Substring(file.FullName.Length - 4);
                if (suffix != "meta")
                {
                    //对AB包添加后缀
                    FileInfo fi = new FileInfo(file.FullName);
                    var fileName = file.Name;
                    string md5 = GetMD5HashFromFile(_abResPath + "/" + fi.Name);
                    ////assetbundle
                    //if ( !fileName.Contains("."))
                    //{
                    //    fileName += ".assetbundle";
                    //    File.Move( fi.FullName, fi.FullName + ".assetbundle" );
                    //}

                    ABVersion ab = new ABVersion
                    {
                        AbName = fileName,
                        Version = 1,
                        MD5 = md5
                    };
                    abVersionsDic.Add(fileName, ab);
                }
            }
            _IsABVersionCSV = true;
        }

        private static void MatchFiles(Dictionary<string, ABVersion> abVersionsDic)
        {
            string[] files = Directory.GetFiles(_abResPath);
            foreach (string file in files)
            {
                string suffix = file.Substring(file.Length - 4);
                if (suffix != "meta")
                {
                    var newFile = file.Replace("\\", "/");
                    string md5 = GetMD5HashFromFile(newFile);
                    string abName = newFile.Substring(_abResPath.Length + 1);
                    if (!abVersionsDic.ContainsKey(abName))
                    {
                        ABVersion ab = new ABVersion
                        {
                            AbName = abName,
                            Version = 1,
                            MD5 = md5
                        };
                        abVersionsDic.Add(abName, ab);
                    }
                }
            }
            _IsABVersionCSV = true;
        }

        /// <summary>
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
        /// <returns></returns>
        private static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                return LitFramework.Crypto.Crypto.md5.GetFileHash(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Get MD5 Hash From File( {0} ) Fail,error: {2} ", fileName, ex.Message));
            }
        }


        #endregion

        #region PathfToAB


        private static bool _IsPathfToABCSV;

        private static bool _IsPathfToABChange;
        private static bool IsPathfToABCSV() { return _IsPathfToABCSV; }

        /// <summary>
        /// 写入CSV的标题栏
        /// </summary>
        static string _PathfToABHeard = "Path,BundleName";
        /// <summary>
        /// 写入CSV的值
        /// </summary>
        static string _PathfToABValue = "{0},{1}";


        /// <summary>
        /// 生成csv文件
        /// </summary>
        public static IEnumerator IEPathfToABSendCSV(string outpuut)
        {
            _IsPathfToABCSV = false;

            _IsPathfToABChange = false;
            string _PathfToABResPath = outpuut + "/" + FrameworkConfig.Instance.ABFolderName;
            string csvPath = outpuut + "/" + ABPathName;

            Dictionary<string, string> abPathDic = new Dictionary<string, string>();
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
                PathfToABMatchFiles(abPathDic);
            }
            else
            {
                PathfToABMatchFiles(abPathDic);
            }
            yield return new WaitUntil(IsPathfToABCSV);
            if (_IsPathfToABChange)
            {
                PathfToABExportCSV(abPathDic, csvPath);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }

        }

        private static void PathfToABMatchFiles(Dictionary<string, string> abPathDic)
        {
            string[] AssetBundleNameArray = AssetBundleModel.Model.DataSource.GetAllAssetBundleNames();
            for (int i = 0; i < AssetBundleNameArray.Length; i++)
            {
                string assetBundleName = AssetBundleNameArray[i];
                string[] asstePath = AssetBundleModel.Model.DataSource.GetAssetPathsFromAssetBundle(assetBundleName);
                for (int j = 0; j < asstePath.Length; j++)
                {
                    string path = asstePath[j].Trim();
                    if (path.Contains("Assets/Resources/"))
                    {
                        path = path.Remove(0, 17);
                        int endDot = path.LastIndexOf('.');
                        path = path.Remove(endDot, path.Length - endDot);
                        if (abPathDic.ContainsKey(path))
                        {
                            // Log.Error(path + "----" + "重名了！！！或者打到两包里了");
                        }
                        else
                        {
                            abPathDic.Add(path, assetBundleName);
                            _IsPathfToABChange = true;
                        }
                    }

                }

            }
            _IsPathfToABCSV = true;
        }

        /// <summary>
        /// 将ssv写入到指定目录
        /// </summary>
        /// <param name="abVersions"></param>
        /// <param name="fileName"></param>
        public static void PathfToABExportCSV(Dictionary<string, string> abPathDic, string fileName)
        {
            if (fileName.Length > 0)
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));

                sw.WriteLine(_PathfToABHeard);
                foreach (KeyValuePair<string, string> keyValuePair in abPathDic)
                {
                    string dataStr = string.Format(_PathfToABValue, keyValuePair.Key, keyValuePair.Value);
                    sw.WriteLine(dataStr);
                }

                sw.Close();
                fs.Close();
            }
        }


        #endregion

        #endregion



        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            foreach (string folderPath in Directory.GetDirectories(sourceDirName, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(folderPath.Replace(sourceDirName, destDirName)))
                    Directory.CreateDirectory(folderPath.Replace(sourceDirName, destDirName));
            }

            foreach (string filePath in Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories))
            {
                var fileDirName = Path.GetDirectoryName(filePath).Replace("\\", "/");
                var fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(fileDirName.Replace(sourceDirName, destDirName), fileName);

                File.Copy(filePath, newFilePath, true);
            }
        }

        private void BrowseForFolder()
        {
            m_UserData.m_UseDefaultPath = false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", m_UserData.m_OutputPath + "/" + FrameworkConfig.Instance.ABFolderName, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = System.IO.Path.GetFullPath(".");
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                m_UserData.m_OutputPath = newPath;
                //EditorUserBuildSettings.SetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath", m_OutputPath);
            }
        }
        private void ResetPathToDefault()
        {
            m_UserData.m_UseDefaultPath = true;
            m_UserData.m_OutputPath = "AssetBundles/";
            m_UserData.m_OutputPath += m_UserData.m_BuildTarget.ToString();
            //EditorUserBuildSettings.SetPlatformSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), "AssetBundleOutputPath", m_OutputPath);
        }

        //Note: this is the provided BuildTarget enum with some entries removed as they are invalid in the dropdown
        internal enum ValidBuildTarget
        {
            //NoTarget = -2,        --doesn't make sense
            //iPhone = -1,          --deprecated
            //BB10 = -1,            --deprecated
            //MetroPlayer = -1,     --deprecated
            StandaloneOSXUniversal = 2,
            StandaloneOSXIntel = 4,
            StandaloneWindows = 5,
            WebPlayer = 6,
            WebPlayerStreamed = 7,
            iOS = 9,
            PS3 = 10,
            XBOX360 = 11,
            Android = 13,
            StandaloneLinux = 17,
            StandaloneWindows64 = 19,
            WebGL = 20,
            WSAPlayer = 21,
            StandaloneLinux64 = 24,
            StandaloneLinuxUniversal = 25,
            WP8Player = 26,
            StandaloneOSXIntel64 = 27,
            BlackBerry = 28,
            Tizen = 29,
            PSP2 = 30,
            PS4 = 31,
            PSM = 32,
            XboxOne = 33,
            SamsungTV = 34,
            N3DS = 35,
            WiiU = 36,
            tvOS = 37,
            Switch = 38
        }

        [System.Serializable]
        internal class BuildTabData
        {
            internal List<string> m_OnToggles;
            internal ValidBuildTarget m_BuildTarget = ValidBuildTarget.StandaloneWindows;
            internal CompressOptions m_Compression = CompressOptions.StandardCompression;
            internal string m_OutputPath = string.Empty;
            internal bool m_UseDefaultPath = true;
        }
    }

}