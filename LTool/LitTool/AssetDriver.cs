using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;



/// <summary>
/// csv资源文件内容
/// </summary>
public class ABVersion
{
    public string AbName;
    public int Version;
    public string MD5;
}

public class AssetDriver : Singleton<AssetDriver>
{

    private string _ArchiveBundlesDirectory = "";
    private AssetBundleManifest _MainManifest = null;
    private Dictionary<string, AssetBundle> _LoadedBundles = new Dictionary<string, AssetBundle>();

    private bool _isLoadAB;
    public bool IsLoadABEnd;

    int _loadAbNum = -1;

    public void Install()
    {
        _isLoadAB = false;
        string url = Application.streamingAssetsPath;
        string streamingAssetsPath = string.Format("{0}/{1}", url, FrameworkConfig.Instance.ABFolderName);
        string targetPaht = string.Format( "{0}/{1}", Application.streamingAssetsPath, FrameworkConfig.Instance.ABFolderName );
        string filePath = string.Format("{0}/AbSourcesInfo.txt", url);
        if (FrameworkConfig.Instance.UseRemotePersistantPath)
        {
            DownLoadVersion();
        }
        else
        {
            //SetAssetBundleConfig(targetPaht, FrameworkConfig.Instance.ABFolderName);
            //CopyDirToLocal(streamingAssetsPath, targetPaht, filePath);
        }
    }

    private void DownLoadVersion()
    {
        //启动游戏检测
        //下载最新的资源配置信息
        //根据本地是否存在资源配置信息
        //有就根据版本更新,没有就去下载完整ab资源
        //更新完成或者下载完成,才开始进入游戏

        //本地资源配置信息文件路径
        string ABVersionPath = Application.persistentDataPath + "/ABVersion.csv";
        //记录最新的资源配置信息,用于保存最新的csv文件
        List<ABVersion> abVersions = new List<ABVersion>();
        ABVersion ab;
        //本地存在,更新
        if (File.Exists(ABVersionPath))
        {
            //临时保存的资源配置路径,
            string TemCSVPath = Application.persistentDataPath + "/TemCSV";

            if (!Directory.Exists(TemCSVPath))
                Directory.CreateDirectory(TemCSVPath);

            //下载服务器csv配置信息到临时目录,同步
            DownLoadSources("ABVersion.csv", TemCSVPath + "/ABVersion.csv");

            //解析下载的资源配置信息
            LitTool.MonoBehaviour.StartCoroutine(DocumentAccessor.ILoadAsset(ABVersionPath, (string e) =>
            {
                //将资源配置信息保存
                Dictionary<string, ABVersion> abVersionsDic = new Dictionary<string, ABVersion>();

                string[] str = e.Split('\n');
                //取出本地的配置信息,对比获取哪些版本不同,不同的要更新
                for (int i = 1; i < str.Length; i++)
                {
                    string line = str[i];
                    if (line != "")
                    {
                        string[] content = line.Split(',');
                        ab = new ABVersion
                        {
                            AbName = content[0],
                            Version = int.Parse(content[1]),
                            MD5 = content[2].Trim()
                        };
                        abVersionsDic.Add(content[0], ab);
                    }
                }

                //更具版本更新资源
                LitTool.MonoBehaviour.StartCoroutine(DocumentAccessor.ILoadAsset(TemCSVPath + "/ABVersion.csv", (string temE) =>
                {
                    str = temE.Split('\n');
                    //需要更新的ab包
                    List<string> allAbName = new List<string>();
                    for (int i = 1; i < str.Length; i++)
                    {
                        string line = str[i];
                        if (line != "")
                        {
                            string[] content = line.Split(',');
                            string abName = content[0];
                            int version = int.Parse(content[1]);
                            if (abVersionsDic.ContainsKey(abName))
                            {
                                ab = abVersionsDic[abName];
                                //版本不同的才要更新
                                if (ab.Version != version)
                                {
                                    allAbName.Add(ab.AbName);
                                }
                            }
                            //更新csv文件内容
                            ab = new ABVersion
                            {
                                AbName = abName,
                                Version = version,
                                MD5 = content[2].Trim()
                            };
                            abVersions.Add(ab);
                        }
                    }
                    //需要更新的文件数量
                    _loadAbNum = allAbName.Count;
                    //下载更新的文件
                    for (int i = 0; i < allAbName.Count; i++)
                    {
                        LitTool.MonoBehaviour.StartCoroutine(DownLoadABIE(allAbName[i]));
                    }

                    //有更新,需要将最新的资源配置信息保存
                    if (_loadAbNum > 0)
                        ResponseExportCSV(abVersions, ABVersionPath);
                }));
            }));
        }
        else
        {
            //第一次将服务器上的的配置信息直接保存
            DownLoadSources("ABVersion.csv", ABVersionPath);

            //下载资源
            LitTool.MonoBehaviour.StartCoroutine(DocumentAccessor.ILoadAsset(ABVersionPath, (string e) =>
            {
                string[] str = e.Split('\n');
                List<string> allAbName = new List<string>();
                for (int i = 1; i < str.Length; i++)
                {
                    string line = str[i];
                    if (line != "")
                    {
                        string[] content = line.Split(',');
                        allAbName.Add(content[0]);
                        ab = new ABVersion
                        {
                            AbName = content[0],
                            Version = int.Parse(content[1]),
                            MD5 = content[2].Trim()
                        };
                        abVersions.Add(ab);
                    }
                }
                //下载的ab包资源数量
                _loadAbNum = allAbName.Count;
                //下载
                for (int i = 0; i < allAbName.Count; i++)
                {
                    LitTool.MonoBehaviour.StartCoroutine(DownLoadABIE(allAbName[i]));
                }
                ResponseExportCSV(abVersions, ABVersionPath);
            }));
        }

        //SetAssetBundleConfig( Application.streamingAssetsPath + "/" + FrameworkConfig.Instance.ABFolderName, FrameworkConfig.Instance.ABTotalName );
    }
    /// <summary>
    /// 当所有资源下载完成或者更新完成,开始游戏
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    IEnumerator DownLoadABIE(string abName)
    {
        yield return null;
        DownLoadAB(abName);
        _loadAbNum--;
    }

    /// <summary>
    /// 将ssv写入到指定目录
    /// </summary>
    /// <param name="abVersions"></param>
    /// <param name="fileName"></param>
    void ResponseExportCSV(List<ABVersion> abVersions, string fileName)
    {
        if (fileName.Length > 0)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string dataHeard = string.Empty;
            //这个地方是写入CSV的标题栏 注意最后个没有分隔符
            dataHeard = "AbName,Version,MD5";
            sw.WriteLine(dataHeard);
            //写入数据
            for (int i = 0; i < abVersions.Count; i++)
            {
                string dataStr = string.Format("{0},{1},{2}", abVersions[i].AbName, abVersions[i].Version, abVersions[i].MD5);
                sw.WriteLine(dataStr);
            }
            sw.Close();
            fs.Close();
        }
    }


    private void CopyDirToLocal(string streamingAssetsPath, string targetPath, string filePath)
    {
        //      if (!Directory.Exists(targetPath))
        //      {
        //          DirectoryInfo dirInfo = Directory.CreateDirectory(targetPath);
        //	LitTool.MonoBehaviour.StartCoroutine(ILoadAndDownAbPackageFile(streamingAssetsPath, targetPath, filePath, dirInfo));
        //}
        //      else
        //      {
        //          return;
        //      }
        LitTool.MonoBehaviour.StartCoroutine(ILoadAndDownAbPackageFile(streamingAssetsPath, targetPath, filePath));
    }

    IEnumerator ILoadAndDownAbPackageFile(string abPath, string downLoadPath, string filePath)
    {
        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();
        string str = request.downloadHandler.text;
        string[] abSources = str.Split(',');
        int allNum = abSources.Length;
        int indexNum = 0;
        string downLoadTruePath;
        for (int i = 0; i < allNum; i++)
        {
            string loadPath = string.Format("{0}/{1}", abPath, abSources[i]);
            downLoadTruePath = string.Format("{0}/{1}", downLoadPath, abSources[i]);
            request = UnityWebRequest.Get(loadPath);
            yield return request.SendWebRequest();
            downloadAndSave(request, downLoadTruePath);
            if (++indexNum == allNum)
            {
                _isLoadAB = true;
            }
        }

        yield return new WaitUntil(() => _isLoadAB);
        LDebug.Log("ab包下载完成");
    }

    //string s = Application.streamingAssetsPath + "/s/ss.txt";
    //string filePath = Application.persistentDataPath + "/s";
    //       if (!File.Exists(filePath+ "/ss.txt"))
    //       {
    //           using (WWW www = new WWW(s))
    //           {
    //               yield return www;
    //               if (!string.IsNullOrEmpty(www.error))
    //               {
    //                   Debug.Log(www.error);
    //               }
    //               else
    //               {
    //                   if (!Directory.Exists(filePath))
    //                   {
    //                       //创建文件夹
    //                       Directory.CreateDirectory(filePath);
    //                   }
    //                   File.WriteAllBytes(filePath+ "/ss.txt", www.bytes);
    //               }
    //           }
    //       }

    //   /// <summary>
    //   /// 初始化ab包配置信息AssetBundleManifest
    //   /// </summary>
    //   /// <param name="archiveDirectory"></param>
    //   /// <param name="mainBundleName"></param>
    //   public void SetAssetBundleConfig(string archiveDirectory, string mainBundleName)
    //{
    //	if (_ArchiveBundlesDirectory != "")
    //	{
    //		return;
    //	}

    //	_ArchiveBundlesDirectory = archiveDirectory;
    //	LDebug.Log("加载主包=>"+ archiveDirectory+"/"+ mainBundleName);
    //	AssetBundle main = AssetBundle.LoadFromFile(string.Format("{0}/{1}", archiveDirectory, mainBundleName));
    //	_MainManifest = main.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    //	LDebug.Log("加载主包成功!");
    //	IsLoadABEnd = true;
    //}

    /// <summary>
    /// 获取包依赖信息
    /// </summary>
    /// <param name="fileNames"></param>
    /// <returns></returns>
    //private List<string> _GetAllFileNames(string[] fileNames)
    //{
    //	List<string> allNames = new List<string>();

    //	if (_ArchiveBundlesDirectory == "")
    //	{
    //		return allNames;
    //	}

    //	for (int i = 0; i < fileNames.Length; i++)
    //	{
    //		string[] dpds = _MainManifest.GetAllDependencies(fileNames[i]);
    //		for (int j = 0; j < dpds.Length; j++)
    //		{
    //			if (!allNames.Contains(dpds[j]))
    //			{
    //				allNames.Add(dpds[j]);
    //			}
    //		}

    //		if (!allNames.Contains(fileNames[i]))
    //		{
    //			allNames.Add(fileNames[i]);
    //		}
    //	}

    //	for (int i = allNames.Count - 1; i >= 0; i--)
    //	{
    //		if (_LoadedBundles.ContainsKey(allNames[i]))
    //		{
    //			allNames.RemoveAt(i);
    //			continue;
    //		}

    //		string filePath = string.Format("{0}/{1}", _ArchiveBundlesDirectory, allNames[i]);
    //		if (!File.Exists(filePath))
    //		{
    //			LDebug.Log("[Asset] AssetBundle文件" + filePath + "不存在");
    //			allNames.RemoveAt(i);
    //		}
    //	}

    //	return allNames;
    //}

    ///// <summary>
    ///// 加载ab子包
    ///// </summary>
    ///// <param name="fileName"></param>
    //public void LoadFromFile(string fileName)
    //{
    //	List<string> allNames = _GetAllFileNames(new string[1] { fileName });
    //       Debug.LogError(fileName);
    //       for (int i = 0; i < allNames.Count; i++)
    //	{
    //           Debug.LogError(allNames[i]+"       "+ _ArchiveBundlesDirectory);
    //		_LoadedBundles.Add(allNames[i], AssetBundle.LoadFromFile(string.Format("{0}/{1}", _ArchiveBundlesDirectory, allNames[i])));
    //	}
    //}

    public void DownLoadAB(string AbSources)
    {
        //本地有没有,有的话更新,没有就读取streamingAssetsPath
        //有就根据版本更新,没有就去streamingAssets下载完整ab资源
        //更新完成或者下载完成,才开始进入游戏
        string loadPath = Application.persistentDataPath + "/"+ FrameworkConfig.Instance.ABFolderName;
        if (!Directory.Exists(loadPath))
            Directory.CreateDirectory(loadPath);

        string userName = "";
        string password = "";
        //for (int i = 0; i < loadAB.Length; i++)
        //{
        //	string abName = loadAB[i];
        //	downloadWithFTP("ftp://192.168.1.12/" + abName, "F:/FTP_Down/" + abName, userName: userName, password: password);
        //}
        downloadWithFTP("ftp://192.168.1.12/" + AbSources, loadPath + "/" + AbSources, userName: userName, password: password);
    }

    public void DownLoadSources(string sources, string loadPath)
    {
        string userName = "";
        string password = "";

        downloadWithFTP("ftp://192.168.1.12/" + sources, loadPath, userName: userName, password: password);
    }

    private byte[] downloadWithFTP(string ftpUrl, string savePath = "", string userName = "", string password = "")
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpUrl));
        //request.Proxy = null;
        request.UsePassive = true;
        request.UseBinary = true;
        request.KeepAlive = true;

        //If username or password is NOT null then use Credential
        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
            request.Credentials = new NetworkCredential(userName, password);
        }

        request.Method = WebRequestMethods.Ftp.DownloadFile;

        //If savePath is NOT null, we want to save the file to path
        //If path is null, we just want to return the file as array
        if (!string.IsNullOrEmpty(savePath))
        {
            downloadAndSave(request.GetResponse(), savePath);
            return null;
        }
        else
        {
            return downloadAsbyteArray(request.GetResponse());
        }
    }

    byte[] downloadAsbyteArray(WebResponse request)
    {
        using (Stream input = request.GetResponseStream())
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while (input.CanRead && (read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    void downloadAndSave(WebResponse request, string savePath)
    {
        Stream reader = request.GetResponseStream();
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        }

        FileStream fileStream = new FileStream(savePath, FileMode.Create);

        int bytesRead = 0;
        byte[] buffer = new byte[2048];

        while (true)
        {
            bytesRead = reader.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
                break;

            fileStream.Write(buffer, 0, bytesRead);
        }
        fileStream.Close();
        //fileStream = null;
    }

    void downloadAndSave(UnityWebRequest request, string savePath)
    {
        LDebug.Log("创建文件" + savePath);
        savePath = savePath.Trim();
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        }
        FileStream fileStream = new FileStream(savePath, FileMode.Create);
        fileStream.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
        fileStream.Close();
        //fileStream = null;
    }

    /// <summary>
    ///// 加载多个ab子包
    ///// </summary>
    ///// <param name="fileNames"></param>
    //public void LoadFromFile(string[] fileNames)
    //{
    //	List<string> allNames = _GetAllFileNames(fileNames);
    //	for (int i = 0; i < allNames.Count; i++)
    //	{
    //		_LoadedBundles[allNames[i]] = AssetBundle.LoadFromFile(_ArchiveBundlesDirectory + "/" + allNames[i]);
    //	}
    //}

    ///// <summary>
    ///// 加载资源
    ///// </summary>
    ///// <param name="rule"></param>
    ///// <returns></returns>
    //public UnityEngine.Object LoadAsset(string rule)
    //{
    //	string[] names = rule.Split('#');
    //	string abName = string.Format("{0}.assetbundle", names[0]);
    //	//是否加载,如果没加载就去加载
    //	if (!_LoadedBundles.ContainsKey(abName))
    //	{
    //		LoadFromFile(abName);
    //	}
    //	UnityEngine.Object obj = _LoadedBundles[abName].LoadAsset(names[1]);
    //	return obj;
    //}



    //   public Image LoaAssetImg(string rule)
    //{
    //	string[] names = rule.Split('#');
    //	string abName = string.Format("{0}.assetbundle",names[0]);
    //	//是否加载,如果没加载就去加载
    //	if (!_LoadedBundles.ContainsKey(abName))
    //	{
    //		LoadFromFile(abName);
    //	}
    //	return _LoadedBundles[abName].LoadAsset<Image>(names[1]);
    //}

    //public AudioClip LoadAssetAudio(string rule)
    //{
    //	string[] names = rule.Split('#');
    //	string abName = string.Format("{0}.assetbundle",names[0]);
    //	//是否加载,如果没加载就去加载
    //	if (!_LoadedBundles.ContainsKey(abName))
    //	{
    //		LoadFromFile(abName);
    //	}
    //	return _LoadedBundles[abName].LoadAsset<AudioClip>(names[1]);
    //}

    //public void Unload()
    //{
    //	AssetBundle.UnloadAllAssetBundles(false);
    //}

    #region AB包加载资源
    private Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle>();

    private AssetBundle mainAB = null;
    private AssetBundleManifest mainManifest = null;
    /// <summary>
    /// AB包路径
    /// </summary>
    private string ABFilePath
    {
        get
        {
            return FrameworkConfig.Instance.UsePersistantPath ? Application.persistentDataPath + string.Format("/{0}/", FrameworkConfig.Instance.ABFolderName ) : Application.streamingAssetsPath + string.Format( "/{0}/", FrameworkConfig.Instance.ABFolderName );
        }
    }

    /// <summary>
    /// 主包名字
    /// </summary>
    private string MainABName
    {
        get
        {
#if UNITY_IOS
            
#elif UNITY_ANDROID

#else

#endif
            return FrameworkConfig.Instance.ABTotalName;
        }
    }

    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private AssetBundle LoadAB(string name)
    {
        if (mainAB == null)
        {
            Debug.Log(ABFilePath + MainABName);
            mainAB = AssetBundle.LoadFromFile(ABFilePath + MainABName);
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        string[] str = mainManifest.GetAllDependencies(name);

        for (int i = 0; i < str.Length; i++)
        {
            if (!bundleDic.ContainsKey(str[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(ABFilePath + str[i]);
                bundleDic.Add(str[i], ab);
            }
        }
        if (!bundleDic.ContainsKey(name))
        {
            AssetBundle ab = AssetBundle.LoadFromFile(ABFilePath + name);
            bundleDic.Add(name, ab);
        }
        return bundleDic[name];
    }

    /// <summary>
    /// 从AB包中加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadFromAB<T>(string rule) where T : UnityEngine.Object
    {
        string[] names = rule.Split('#');
        string abName = names[0];
        string resName = names[1];
        AssetBundle ab = LoadAB(abName);
        return ab.LoadAsset<T>(resName);
    }

    /// <summary>
    /// 卸载没有被使用的资源
    /// </summary>
    public void RemoveMenory()
    {
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 卸载单个AB包
    /// </summary>
    /// <param name="name"></param>
    public void ClearOneAB(string name)
    {
        if (bundleDic.ContainsKey(name))
        {
            bundleDic[name].Unload(false);
            bundleDic.Remove(name);
        }
    }

    /// <summary>
    /// 卸载所有AB包
    /// </summary>
    public void ClearAll()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        bundleDic.Clear();
        mainAB = null;
        mainManifest = null;
    }
    #endregion
}