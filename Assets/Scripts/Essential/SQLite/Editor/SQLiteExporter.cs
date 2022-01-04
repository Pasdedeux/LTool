using ExcelDataReader;
using LitFramework;
using LitFramework.LitTool;
using LitFrameworkEditor.EditorExtended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 配置表路径注册类
/// </summary>
class SQLConfigsParse
{
    List<string> CSString = new List<string>();

    public string CreateCS(ConfigsNamesTemplate rpt)
    {
        AddHead();
        AddBody(rpt);
        AddTail();
        string result = GetFomatedCS();

        return result;
    }

    private void AddHead()
    {
        CSString.Add("#region << 版 本 注 释 >>");
        CSString.Add("///*----------------------------------------------------------------");
        CSString.Add("// Author : Derek Liu");
        CSString.Add("// 创建时间:" + DateTime.Now.ToString());
        CSString.Add("// 备注：由模板工具自动生成");
        CSString.Add("///----------------------------------------------------------------*/");
        CSString.Add("#endregion");
        CSString.Add("");
        CSString.Add("//*******************************************************************");
        CSString.Add("//**                  该类由工具自动生成，请勿手动修改                   **");
        CSString.Add("//*******************************************************************");
        CSString.Add("");
        CSString.Add("using LitFramework;");
        CSString.Add("using System.Linq;");
        CSString.Add("using System.Collections.Generic;");
        CSString.Add("public static partial class Configs");
        CSString.Add("{");
    }
    private void AddTail()
    {
        CSString.Add("}");
    }
    private void AddBody(ConfigsNamesTemplate rpt)
    {
        foreach (var item in rpt.configsNameList)
        {
            CSString.Add(string.Format("public static {0} {0}Dict;", item.Key));
        }

        CSString.Add(string.Format("public static void Install()"));
        CSString.Add("{");
        foreach (var item in rpt.configsNameList)
        {
            CSString.Add(string.Format("{0}Dict = new {0}();", item.Key));
        }
        CSString.Add("}");
    }
    string GetFomatedCS()
    {
        StringBuilder result = new StringBuilder();
        int tablevel = 0;
        for (int i = 0; i < CSString.Count; i++)
        {
            string tab = "";

            for (int j = 0; j < tablevel; ++j)
                tab += "\t";

            if (CSString[i].Contains("{"))
                tablevel++;
            if (CSString[i].Contains("}"))
            {
                tablevel--;
                tab = "";
                for (int j = 0; j < tablevel; ++j)
                    tab += "\t";
            }

            result.Append(tab + CSString[i] + "\n");
        }
        return result.ToString();
    }
}

public class SQLiteExporter 
{
    /// <summary>
    /// 写入CSV标题栏
    /// </summary>
    private static string _csvListTitle = "CsvName,Version,MD5";
    /// <summary>
    /// 写入CSV的值
    /// </summary>
    private static string _csvContentValue = "{0},{1},{2}";
    /// <summary>
    /// 需要存档的配置文件
    /// </summary>
    private static List<ABVersion> _csvListToBeRestored = new List<ABVersion>();


#if UNITY_EDITOR
    [MenuItem("Tools/配置文件->SQLite", priority = 22)]
#endif
    public static void XlsxToSQLite_ClickHandler()
    {
        EditorApplication.delayCall += XlsxToSQLite;
    }
    public static void XlsxToSQLite()
    {
        string xlsxpath = Application.dataPath + "/XLSX";
        string streampath = Application.dataPath + "/StreamingAssets";
        string csvpath = Application.dataPath + "/StreamingAssets/csv";
        _csvListToBeRestored.Clear();
        //文件列表
        //string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
        //FileStream fs = new FileStream( listpath, FileMode.Create );
        //StreamWriter listwriter = new StreamWriter( fs, new UTF8Encoding(false) );
        DirectoryInfo TheFolder = new DirectoryInfo(xlsxpath);

        if (!Directory.Exists(csvpath))
        {
            Directory.CreateDirectory(csvpath);
        }
        SQLiteWriter sqliteWriter = new SQLiteWriter(csvpath);
        sqliteWriter.Opendb();
        try
        {
            //对文件进行遍历
            foreach (var NextFile in TheFolder.GetFiles())
            {
                if (Path.GetExtension(NextFile.Name) == ".xlsx" && !NextFile.Name.StartsWith("~$"))
                {
                    string csvfile = XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                    string tTableName = NextFile.Name.Split('.')[0];
                    Debug.Log(tTableName + "  文件生成成功！");
                    CSVReader reader = new CSVReader(csvfile);
                    sqliteWriter.Write(tTableName, reader);
                }
                else if (Path.GetExtension(NextFile.Name) == ".txt")
                {
                    FileInfo fi = new FileInfo(csvpath + "/" + NextFile.Name);
                    if (fi.Exists)
                        fi.Delete();
                    NextFile.CopyTo(csvpath + "/" + NextFile.Name);
                    //listwriter.WriteLine( NextFile.Name );
                    _csvListToBeRestored.Add(new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 });
                }
            }
            sqliteWriter.Closedb();
            string str = "csv/csvconfigs.bytes";
            _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + "csvconfigs.bytes"), Version = 1 });
            //遍历框架配置的额外后缀文件
            string[] extralFile = FrameworkConfig.Instance.configs_suffix.Split('|');
            foreach (var item in extralFile)
            {
                if (item.Equals("csv")) continue;

                GetFiles(new DirectoryInfo(streampath), item, _csvListToBeRestored);
            }
        }
        catch (Exception e) {
            sqliteWriter.Closedb();
            Debug.LogError(e.Message); 
        }
        finally
        {
            //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
            MatchCSVTotalFile(_csvListToBeRestored);

            //listwriter.Close();
            //listwriter.Dispose();
            //fs.Dispose();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
#if UNITY_EDITOR
    [MenuItem("Tools/配置文件->SQLite+代码", priority = 23)]
#endif
    public static void CsvToSqliteAndCs_ClickHandler()
    {
        EditorApplication.delayCall += CsvToSqliteAndCs;
    }
    public static void CsvToSqliteAndCs()
    {
        Debug.Log("配置文件转化为代码  开始!");
        _csvListToBeRestored.Clear();
        string xlsxpath = Application.dataPath + "/XLSX";
        string streampath = Application.dataPath + "/StreamingAssets";
        string csvOutPath = Application.dataPath + "/StreamingAssets/csv";
        string csOutPath = Application.dataPath + "/Scripts/CSV";
        if (!FrameworkConfig.Instance.UseHotFixMode)
            csOutPath = Application.dataPath + "/Scripts/CSV";
        else
            csOutPath = Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/CSV";
        DirectoryInfo theXMLFolder = new DirectoryInfo(xlsxpath);
        if (!Directory.Exists(csvOutPath))
        {
            Directory.CreateDirectory(csvOutPath);
        }
        if (!Directory.Exists(csOutPath))
        {
            Directory.CreateDirectory(csOutPath);
        }

        SQLiteWriter sqliteWriter = new SQLiteWriter(csvOutPath);
        sqliteWriter.Opendb();

        try
        {
            ConfigsNamesTemplate cnt = new ConfigsNamesTemplate();
            //对文件进行遍历
            foreach (var NextFile in theXMLFolder.GetFiles())
            {
                if (Path.GetExtension(NextFile.Name) == ".xlsx" && !NextFile.Name.StartsWith("~$"))
                {
                    LDebug.Log(" >表处理 : " + NextFile.Name);
                    string tTableName = NextFile.Name.Split('.')[0];
                    FileStream stream = NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    string csvfile = XLSXTOCSV(stream);
                    SQLParser cp = new SQLParser();
                    CreateCSFile(csOutPath, tTableName + ".cs", cp.CreateCS(tTableName, csvfile));

                    //将excel写入csvconfigs.bytes
                    CSVReader reader = new CSVReader(csvfile);
                    sqliteWriter.Write(tTableName, reader);

                    cnt.configsNameList.Add(tTableName, reader.GetData(0, 2));
                }
                else if (Path.GetExtension(NextFile.Name) == ".txt")
                {
                    FileInfo fi = new FileInfo(csvOutPath + "/" + NextFile.Name);
                    if (fi.Exists)
                        fi.Delete();
                    NextFile.CopyTo(csvOutPath + "/" + NextFile.Name);
                    //listwriter.WriteLine( NextFile.Name );
                    _csvListToBeRestored.Add(new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 });
                }
            }
            sqliteWriter.Closedb();

            string str = "csv/csvconfigs.bytes";
            _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvOutPath + "/" + "csvconfigs.bytes"), Version = 1 });

            //遍历框架配置的额外后缀文件
            string[] extralFile = FrameworkConfig.Instance.configs_suffix.Split('|');
            foreach (var item in extralFile)
            {
                if (item.Equals("csv")) continue;

                GetFiles(new DirectoryInfo(streampath), item, _csvListToBeRestored);
            }

            //============更新并保存CS============//
            SQLConfigsParse rpp = new SQLConfigsParse();

            if (!FrameworkConfig.Instance.UseHotFixMode)
                CreateCSFile(Application.dataPath + "/Scripts/Model/Const/", "Configs.cs", rpp.CreateCS(cnt));
            else
                CreateCSFile(Application.dataPath + "/Scripts/RuntimeScript/HotFixLogic/Model/Const/", "Configs.cs", rpp.CreateCS(cnt));
        }
        catch (Exception e) {
            sqliteWriter.Closedb();
            LDebug.LogError(e.Message); 
        }
        finally
        {
            //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
            MatchCSVTotalFile(_csvListToBeRestored);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
    /// <summary>
    /// 创建CS文件
    /// </summary>
    public static void CreateCSFile(string path, string className, string cs)
    {
        if (!Directory.Exists(path))
        {
            //该路径不存在
            Directory.CreateDirectory(path);
        }
        path += "/";
        path += className;

        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));
        sw.Write(cs);
        sw.Close();
        sw.Dispose();
        fs.Dispose();
    }

    static string XLSXTOCSV(FileStream stream)
    {
#if UNITY_EDITOR
        using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
        {
            DataSet result = excelReader.AsDataSet();
            CSVWriter writer = new CSVWriter();
            int rows = result.Tables[0].Rows.Count;

            //通常情况下，获取第一行的列数即可
            int rowlen = 0;
            for (int j = 0; j < result.Tables[0].Rows[0].ItemArray.Length; j++)
            {
                if (result.Tables[0].Rows[0].ItemArray[j].ToString() == "")
                {
                    break;
                }
                rowlen++;
            }

            for (int i = 0; i < rows; i++)
            {
                if (result.Tables[0].Rows[i].ItemArray[0].ToString() == "")
                    break;

                List<object> rowlist = new List<object>();

                for (int j = 0; j < rowlen; j++)
                {
                    if (result.Tables[0].Rows[0].ItemArray[j].ToString() != "备注")
                    {
                        rowlist.Add(result.Tables[0].Rows[i].ItemArray[j]);
                    }

                }
                writer.AddRow(rowlist.ToArray());

            }
            return writer.ToString();
        }
#else
            return "";
#endif
    }

    /// <summary>
    /// 查找指定文件夹下指定后缀名的文件
    /// </summary>
    /// <param name="directory">文件夹</param>
    /// <param name="pattern">后缀名</param>
    /// <returns>文件路径</returns>
    private static void GetFiles(DirectoryInfo directory, string pattern, List<ABVersion> listwriter)
    {
        if (directory.Exists || pattern.Trim() != string.Empty)
        {
            try
            {
                foreach (FileInfo info in directory.GetFiles("*." + pattern))
                {
                    string realName = info.FullName.Replace("\\", "/");
                    realName = realName.Substring(realName.LastIndexOf("StreamingAssets") + "StreamingAssets".Length + 1);
                    if (listwriter.Any(e => e.AbName == realName))
                    {
                        LDebug.LogWarning($">>配置档 {realName} 重复录入。已忽略 ");
                        continue;
                    }
                    listwriter.Add(new ABVersion { AbName = realName, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(info.FullName), Version = 0 });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            foreach (DirectoryInfo info in directory.GetDirectories())//获取文件夹下的子文件夹
            {
                GetFiles(info, pattern, listwriter);//递归调用该函数，获取子文件夹下的文件
            }
        }
    }

    private static void MatchCSVTotalFile(List<ABVersion> csvListToBeRestored)
    {
        string listpath = Application.dataPath + "/StreamingAssets/csvList.txt";
        FileStream fs;
        StreamWriter listwriter;

        if (DocumentAccessor.IsExists(AssetPathManager.Instance.GetStreamAssetDataPath("csvList.txt", false)))
        {
            //本地主配置文件获取
            string localContent = null;
            string localFilePath = AssetPathManager.Instance.GetStreamAssetDataPath("csvList.txt");
            DocumentAccessor.LoadAsset(localFilePath, (e) => { localContent = e; });
            List<ABVersion> localABVersionsDic = ResolveABContent(localContent);

            fs = new FileStream(listpath, FileMode.Create);
            listwriter = new StreamWriter(fs, new UTF8Encoding(false));
            listwriter.WriteLine(_csvListTitle);

            for (int i = 0; i < csvListToBeRestored.Count; i++)
            {
                ABVersion toSave = csvListToBeRestored[i];
                for (int k = 0; k < localABVersionsDic.Count; k++)
                {
                    ABVersion local = localABVersionsDic[k];
                    if (local.AbName == toSave.AbName)
                    {
                        toSave.Version = local.MD5 != toSave.MD5 && local.Version != 0 ? local.Version + 1 : local.Version;
                    }
                }
                listwriter.WriteLine(string.Format(_csvContentValue, toSave.AbName, toSave.Version, toSave.MD5));
            }
        }
        else
        {
            fs = new FileStream(listpath, FileMode.Create);
            listwriter = new StreamWriter(fs, new UTF8Encoding(false));
            listwriter.WriteLine(_csvListTitle);

            for (int i = 0; i < csvListToBeRestored.Count; i++)
            {
                listwriter.WriteLine(string.Format(_csvContentValue, csvListToBeRestored[i].AbName, csvListToBeRestored[i].Version, csvListToBeRestored[i].MD5));
            }
        }
        listwriter.Close();
        listwriter.Dispose();
        fs.Dispose();
    }
    //解析ABVersion配置表
    private static List<ABVersion> ResolveABContent(string contentResolve)
    {
        List<ABVersion> resultDict = new List<ABVersion>();

        string[] str = contentResolve.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int toLoadNum = str.Length;

        for (int k = 1; k < str.Length; k++)
        {
            string line = str[k];
            if (line != "")
            {
                string[] content = line.Split(',');
                ABVersion ab = new ABVersion
                {
                    AbName = content[0],
                    Version = int.Parse(content[1]),
                    MD5 = content[2].Trim()
                };
                resultDict.Add(ab);
            }
        }

        return resultDict;
    }
}
