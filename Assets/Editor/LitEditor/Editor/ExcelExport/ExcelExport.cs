#region <<版本信息>>
/*======================================
* 项目名称 ：Litframework.ExcelTool.ExcelExport
* 项目描述 ：
* 类 名 称 ：ExcelExport
* 类 描 述 ：
* 命名空间 ：Litframework.ExcelTool.ExcelExport
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2022/3/1 14:41:52
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/
#endregion

using Excel;
using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litframework.ExcelTool
{
    internal partial class ExcelExport
    {
        /// <summary>
        /// Application.DataPath
        /// </summary>
        public static string ProjectPath = string.Empty;

        #region Props

        /// <summary>
        /// 源配置文件地址
        /// </summary>
        private static string XLSX_ORI_DIR;
        /// <summary>
        /// 目标工程StreammingAsset地址
        /// </summary>
        private static string STREAM_OUT_DIR;
        /// <summary>
        /// CSV文件导出地址
        /// </summary>
        private static string CSV_OUTPUT_DIR;
        /// <summary>
        /// 【非热更】配置文件导出位置
        /// </summary>
        private static string CS_OUTPUT_DIR;
        /// <summary>
        /// 【热更】配置文件导出位置
        /// </summary>
        private static string CS_HOTFIX_OUTPUT_DIR;
        /// <summary>
        /// 【非热更】代码文件导出位置
        /// </summary>
        private static string CONFIG_CS_OUTPUT_DIR;
        /// <summary>
        /// 【热更】代码文件导出位置
        /// </summary>
        private static string CONFIG_CS_HOTFIX_OUTPUT_DIR;

        #endregion

        #region Const Prop

        /// <summary>
        /// 配置文件总目录
        /// </summary>
        private const string CS_CONFIGS = "Configs.cs";
        /// <summary>
        /// 写入CSV标题栏
        /// </summary>
        private const string _csvListTitle = "CsvName,Version,MD5";
        /// <summary>
        /// 写入CSV的值
        /// </summary>
        private const string _csvContentValue = "{0},{1},{2}";
        /// <summary>
        /// 需要存档的配置文件
        /// </summary>
        private static List<ABVersion> _csvListToBeRestored = new List<ABVersion>();
        /// <summary>
        /// 生成的CSV记录文档名
        /// </summary>
        private const string _recordTxtFileName = "csvList.txt";
        /// <summary>
        /// 备注文本
        /// </summary>
        private const string _commentTag = "备注";
        /// <summary>
        /// 额外需要登记添加进入csvList的文件
        /// </summary>
        private const string _defaultExtralFileType = "json|dat|assetbundle";

        #endregion

        private static Action<string> _exportFunc1 = null;
        private static Action<string, FileInfo, string, string, ConfigsNamesTemplate> _exportFunc2 = null;
        private static Action<string> _exportFunc3 = null;
        private static Action<string> _exportFunc4 = null;
        private static Action<bool, ConfigsNamesTemplate> _exportFunc5 = null;

        //刷新路径节点，包含硬编配置
        internal static void CombinePath()
        {
            // 源配置文件地址
            XLSX_ORI_DIR = ProjectPath + "/XLSX";
            // 目标工程StreammingAsset地址
            STREAM_OUT_DIR = ProjectPath + "/StreamingAssets";
            // CSV文件导出地址
            CSV_OUTPUT_DIR = ProjectPath + "/StreamingAssets/csv";
            // 【非热更】配置文件导出位置
            CS_OUTPUT_DIR = ProjectPath + "/Scripts/CSV";
            // 【热更】配置文件导出位置
            CS_HOTFIX_OUTPUT_DIR = ProjectPath + "/Scripts/RuntimeScript/HotFixLogic/CSV";
            // 【非热更】代码文件导出位置
            CONFIG_CS_OUTPUT_DIR = ProjectPath + "/Scripts/Model/Const/";
            // 【热更】代码文件导出位置
            CONFIG_CS_HOTFIX_OUTPUT_DIR = ProjectPath + "/Scripts/RuntimeScript/HotFixLogic/Model/Const/";
        }




        /// <summary>
        /// 只导出CSV
        /// </summary>
        /// <param name="extralFileStr"></param>
        public static void Xlsx_2_CSV(string extralFileStr = _defaultExtralFileType)
        {
            _exportFunc2 = (csvpath, NextFile, csvfile, csOutPath, cnt) => 
            {
                WriteLocalFile(csvpath + "/" + NextFile.Name.Split('.')[0] + ".csv", csvfile);
                string str = "csv/" + NextFile.Name.Split('.')[0] + ".csv";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + NextFile.Name.Split('.')[0] + ".csv"), Version = 1 });
            };

            Template_xlsx_2_csv(false,extralFileStr);
        }

        /// <summary>
        /// 导出CS及CSV文件
        /// </summary>
        /// <param name="useHotFix"></param>
        /// <param name="extralFileStr"></param>
        public static void Xlsx_2_CsvCs(bool useHotFix, string extralFileStr = _defaultExtralFileType)
        {
            _exportFunc2 = (csvpath, NextFile, csvfile, csOutPath, cnt) => 
            {
                CSVParser cp = new CSVParser();
                CreateCSFile(csOutPath, NextFile.Name.Split('.')[0] + ".cs", cp.CreateCS(NextFile.Name.Split('.')[0], csvfile));
                WriteLocalFile(csvpath + "/" + NextFile.Name.Split('.')[0] + ".csv", csvfile);

                //这里固定取配置表第三行配置作为类型读取，如果需要修改配置表适配服务器（增加第四行），需要同步修改
                CSVReader reader = new CSVReader(csvfile);
                cnt.configsNameList.Add(NextFile.Name.Split('.')[0], reader.GetData(0, 2));

                string str = "csv/" + NextFile.Name.Split('.')[0] + ".csv";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + NextFile.Name.Split('.')[0] + ".csv"), Version = 1 });
            };
            _exportFunc5 = (useHotFix, cnt) => 
            {
                //============更新并保存CS============//
                IConfigsParse rpp = new ConfigsParse();

                if (!useHotFix)
                    CreateCSFile(CONFIG_CS_OUTPUT_DIR, CS_CONFIGS, rpp.CreateCS(cnt));
                else
                    CreateCSFile(CONFIG_CS_HOTFIX_OUTPUT_DIR, CS_CONFIGS, rpp.CreateCS(cnt));
            };

            Template_xlsx_2_csv(useHotFix,extralFileStr);
        }





        /// <summary>
        /// 加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
        /// </summary>
        /// <param name="csvListToBeRestored"></param>
        private static void MatchCSVTotalFile(List<ABVersion> csvListToBeRestored)
        {
            string listpath = STREAM_OUT_DIR + "/" + _recordTxtFileName;
            FileStream fs = null;
            StreamWriter listwriter = null;
            
            if (DocumentAccessor.IsExists(listpath))
            {
                //本地主配置文件获取
                string localContent = null;

#if UNITY_EDITOR
                DocumentAccessor.LoadAsset(listpath, (e) => { localContent = e; });
#else
                LoadAsset(listpath, (e) => { localContent = e; });
#endif
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

            listwriter?.Close();
            listwriter?.Dispose();
            fs?.Dispose();
        }

        /// <summary>
        /// Bat加载方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callBack"></param>
        private static void LoadAsset(string targetPath, Action<string> callBack)
        {
            FileInfo fileInfo = new FileInfo(targetPath);
            if (!fileInfo.Exists)
            {
                throw new Exception("文件不存在:" + targetPath);
            }

            //使用流的形式读取
            StreamReader sr = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                // 由于在iOS平台下, 读取StreamingAssets 路径的文本文件，必须使用不含 file:// 前缀的路径
                // 所以在该函数中, 只能在内部使用条件编译选项, 并且直接使用 Application.streamingAssetsPath 来组合路径
                //var fn = string.Format( "{0}/{1}" , Application.persistentDataPath , name );

                var fn = targetPath;
                sr = File.OpenText(fn);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //一行一行的读取
                    //将每一行的内容存入数组链表容器中
                    sb.AppendLine(line);
                }
            }
            catch { }
            finally
            {
                if (sr != null)
                {
                    //关闭流
                    sr.Close();
                    //销毁流
                    sr.Dispose();
                }
            }
            //将数组链表容器返回
           
            var fileContent = sb.ToString();
            callBack?.Invoke(fileContent);
        }

        /// <summary>
        /// 生成对应ABVersion类格式数据
        /// </summary>
        /// <param name="contentResolve"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 创建CS文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="className"></param>
        /// <param name="cs"></param>
        public static void CreateCSFile(string path, string className, string cs)
        {
            if (!Directory.Exists(path))
            {
                //该路径不存在
                Directory.CreateDirectory(path);
            }

            path += "/";
            path += className;

            WriteLocalFile(path, cs);
        }

        /// <summary>
        /// IO写入 CS/CSV 本地文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private static void WriteLocalFile(string path, string data)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(true));
            sw.Write(data);
            sw.Close();
            sw.Dispose();
            fs.Dispose();
        }

        /// <summary>
        /// XML写入CSVWriter
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string XLSXTOCSV(FileStream stream)
        {
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
                    if (result.Tables[0].Rows[i].ItemArray[0].ToString() == string.Empty)
                        break;

                    List<object> rowlist = new List<object>();

                    for (int j = 0; j < rowlen; j++)
                    {
                        if (result.Tables[0].Rows[0].ItemArray[j].ToString() != _commentTag)
                        {
                            rowlist.Add(result.Tables[0].Rows[i].ItemArray[j]);
                        }

                    }
                    writer.AddRow(rowlist.ToArray());

                }
                return writer.ToString();
            }
        }

        /// <summary>
        /// 查找指定文件夹下指定后缀名的文件
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="pattern">后缀名</param>
        /// <returns>文件路径</returns>
        private static void GetFiles(DirectoryInfo directory, string pattern, StreamWriter listwriter)
        {
            if (directory.Exists || pattern.Trim() != string.Empty)
            {
                try
                {
                    foreach (FileInfo info in directory.GetFiles("*." + pattern))
                    {
                        //TODO 未能提供检查是否重名
                        listwriter.WriteLine(info.Name);
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

        private static void Template_xlsx_2_csv(bool useHotFix, string extralFileStr)
        {
            CombinePath();

            string xlsxpath = XLSX_ORI_DIR;
            string streampath = STREAM_OUT_DIR;
            string csvpath = CSV_OUTPUT_DIR;
            string csOutPath;

            if (!useHotFix)
                csOutPath = CS_OUTPUT_DIR;
            else
                csOutPath = CS_HOTFIX_OUTPUT_DIR;

            _csvListToBeRestored.Clear();
            //文件列表
            DirectoryInfo TheFolder = new DirectoryInfo(xlsxpath);

            if (!Directory.Exists(csvpath))
            {
                Directory.CreateDirectory(csvpath);
            }
            if (!Directory.Exists(csOutPath))
            {
                Directory.CreateDirectory(csOutPath);
            }

            //============================
            _exportFunc1?.Invoke(csvpath);

            try
            {
                ConfigsNamesTemplate cnt = new ConfigsNamesTemplate();
                //对文件进行遍历
                foreach (var NextFile in TheFolder.GetFiles())
                {
                    if (Path.GetExtension(NextFile.Name) == ".xlsx" && !NextFile.Name.StartsWith("~$"))
                    {
                        string csvfile = XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                        //======================
                        _exportFunc2?.Invoke(csvpath, NextFile, csvfile, csOutPath, cnt);

                    }
                    else if (Path.GetExtension(NextFile.Name) == ".txt")
                    {
                        FileInfo fi = new FileInfo(csvpath + "/" + NextFile.Name);
                        if (fi.Exists)
                            fi.Delete();
                        NextFile.CopyTo(csvpath + "/" + NextFile.Name);

                        _csvListToBeRestored.Add(new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 });
                    }
                }

                //======================
                _exportFunc3?.Invoke(csvpath);

                //遍历框架配置的额外后缀文件
                string[] extralFile = extralFileStr.Split('|');
                foreach (var item in extralFile)
                {
                    if (item.Equals("csv")) continue;

                    GetFiles(new DirectoryInfo(streampath), item, _csvListToBeRestored);
                }

                _exportFunc5?.Invoke(useHotFix, cnt);
            }
            catch (Exception e)
            {
                //======================
                _exportFunc4?.Invoke(null);
                throw new Exception(e.Message);
            }
            finally
            {
                //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
                MatchCSVTotalFile(_csvListToBeRestored);
                ReleaseActionHandler();
            }
        }

        private static void ReleaseActionHandler()
        {
            _exportFunc1 = null;
            _exportFunc2 = null;
            _exportFunc3 = null;
            _exportFunc4 = null;
            _exportFunc5 = null;
        }
    }

    ///// <summary>
    ///// csv资源文件内容
    ///// </summary>
    //public class ABVersion
    //{
    //    public string AbName;
    //    public int Version;
    //    public string MD5;
    //}

    /// <summary>
    /// 配置表访问文件配置文件
    /// </summary>
    public class ConfigsNamesTemplate
    {
        public Dictionary<string, string> configsNameList = new Dictionary<string, string>();
    }

    /// <summary>
    /// CSV代码模板
    /// </summary>
    class ConfigsParse: IConfigsParse
    {
        private List<string> CSString = new List<string>();

        public string CreateCS(ConfigsNamesTemplate rpt)
        {
            AddHead();
            AddBody(rpt);
            AddTail();
            string result = GetFomatedCS();

            return result;
        }

        public void AddHead()
        {
            CSString.Add("#region << 版 本 注 释 >>");
            CSString.Add("///*----------------------------------------------------------------");
            CSString.Add("// Author : Derek Liu");
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
        public void AddTail()
        {
            CSString.Add("}");
        }
        public void AddBody(ConfigsNamesTemplate rpt)
        {
            foreach (var item in rpt.configsNameList)
            {
                CSString.Add(string.Format("public static Dictionary<{2}, {1}> {0};", item.Key + "Dict", item.Key, item.Value));
                CSString.Add(string.Format("public static List<{1}> {0};", item.Key + "List", item.Key));
            }

            CSString.Add(string.Format("public static void Install()"));
            CSString.Add("{");
            foreach (var item in rpt.configsNameList)
            {
                CSString.Add(string.Format("{0} = {1}.Values.ToList();", item.Key + "List", item.Key + "Dict"));
            }
            CSString.Add("}");
        }
        public string GetFomatedCS()
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


    interface IConfigsParse
    {
        string CreateCS(ConfigsNamesTemplate rpt);
        void AddHead();
        void AddTail();
        void AddBody(ConfigsNamesTemplate rpt);
        string GetFomatedCS();
    }
}
