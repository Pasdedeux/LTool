/*======================================
* 项目名称 ：Assets.Scripts.Module.HotFix
* 项目描述 ：
* 类 名 称 ：HotFixAB
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Module.HotFix
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/2 15:22:29
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using Assets.Scripts.LitCore.LitTool;
using LitFramework;
using LitFramework.LitTool;
using LitFramework.MsgSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Assets.Scripts.Module.HotFix
{
    public class HotFixAB : IHotFix
    {
        //AB配置档
        private const string CONFIG_NAME = "ABVersion.csv";
        //AB路径配置档
        private const string CONFIG_AB_PATH = "ABPath.csv";


        private Dictionary<string, ABVersion> _toZipList;
        public void MoveExecute()
        {
            //AB档总表
            if (FrameworkConfig.Instance.ForceUpdatePersistant || !DocumentAccessor.IsExists(AssetPathManager.Instance.GetPersistentDataPath(CONFIG_NAME, false)))
            {
                DocumentAccessor.LoadAsset(AssetPathManager.Instance.GetStreamAssetDataPath(CONFIG_NAME), (UnityWebRequest e) =>
             {
                 _toZipList = ResolveABContent(e.downloadHandler.text);
                 DocumentAccessor.SaveAsset2LocalFile(AssetPathManager.Instance.GetPersistentDataPath(CONFIG_NAME, false), e.downloadHandler.data);
             });

                string[] csvKeys = null;
                string localPath = AssetPathManager.Instance.GetPersistentDataPath(CONFIG_NAME);
                DocumentAccessor.LoadAsset(localPath, (string e) =>
               csvKeys = e.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

                int index = 0;
                foreach (var csvItem in csvKeys)
                {
                    if (index == 0) { index++; continue; }
                    string item = FrameworkConfig.Instance.ABFolderName + "/" + csvItem.Split(',')[0];
                    if (FrameworkConfig.Instance.useZIP)
                    {
                        if (item.Contains(".manifest")) continue;
                        item += ".zip";
                    }
                    if (!DocumentAccessor.IsExists(AssetPathManager.Instance.GetPersistentDataPath(item, false)))
                    {
                        DocumentAccessor.LoadAsset(AssetPathManager.Instance.GetStreamAssetDataPath(item), (UnityWebRequest e) =>
                     {
                         DocumentAccessor.SaveAsset2LocalFile(AssetPathManager.Instance.GetPersistentDataPath(item, false), e.downloadHandler.data);
                     });
                    }
                }
            }
            
            //ABPath路径配置表
            if (FrameworkConfig.Instance.ForceUpdatePersistant || !DocumentAccessor.IsExists(AssetPathManager.Instance.GetPersistentDataPath(CONFIG_AB_PATH, false)))
            {
                DocumentAccessor.LoadAsset(AssetPathManager.Instance.GetStreamAssetDataPath(CONFIG_AB_PATH), (UnityWebRequest e) => DocumentAccessor.SaveAsset2LocalFile(AssetPathManager.Instance.GetPersistentDataPath(CONFIG_AB_PATH, false), e.downloadHandler.data));
            }
        }

        public IEnumerator HotFixExecute()
        {
            LDebug.Log("开始检测更新：" + CONFIG_NAME);
            //1、下载最新的资源配置信息
            bool canGoFurther = true;
            string localContent = null;
            string wrongFileName = string.Empty;
            string remoteFilePath = CONFIG_NAME;

            //发送下载XX文件事件
            MsgManager.Instance.Broadcast(InternalEvent.HANDLING_REMOTE_RES, new MsgArgs(remoteFilePath, InternalEvent.RemoteStatus.Download));

            string localFilePath = AssetPathManager.Instance.GetPersistentDataPath(remoteFilePath, false);
            //2、根据本地是否存在资源配置信息，如果不存在，则视为远程更新流程不执行
            if (DocumentAccessor.IsExists(localFilePath))
            {
                string remoteContent = null;
                byte[] contentByteArr = null;

                //远程主配置文件获取
                LDebug.Log("Remote update..." + FrameworkConfig.Instance.RemoteUrlConfig + "/" + remoteFilePath + " 开始读取", LogColor.yellow);
                yield return DocumentAccessor.ILoadAsset(FrameworkConfig.Instance.RemoteUrlConfig + remoteFilePath, callBack: (UnityWebRequest e) =>
               {
                   LDebug.Log("Remote update..." + remoteFilePath + "读取完成", LogColor.yellow);
                   remoteContent = e.downloadHandler.text;
                   contentByteArr = e.downloadHandler.data;
               },
                errorCallBack: (UnityWebRequest e) =>
                {
                    LDebug.LogError("Remote Error..." + e + ": " + remoteFilePath);
                    if (!string.IsNullOrEmpty(e.error)) { canGoFurther = false; return; }
                });

                //因为加载出问题导致无法继续时，目前先使用中断后续步骤，并弹窗提醒的方式搞
                if (!canGoFurther)
                {
                    MsgManager.Instance.Broadcast(InternalEvent.REMOTE_UPDATE_ERROR, new MsgArgs(remoteContent, remoteFilePath));
                    yield break;
                }

                //本地主配置文件获取
                DocumentAccessor.LoadAsset(localFilePath, (string e) => { localContent = e; });

                //AB资源默认增量更新
                Dictionary<string, ABVersion> remoteABVersionsDic = ResolveABContent(remoteContent);
                Dictionary<string, ABVersion> localABVersionsDic = ResolveABContent(localContent);

                //需要删除的对象
                var toDelete = localABVersionsDic.Where(a => !remoteABVersionsDic.ContainsKey(a.Key));
                foreach (var item in toDelete)
                {
                    string opFile = $"{FrameworkConfig.Instance.ABFolderName}/{item.Key}";
                    if (FrameworkConfig.Instance.useZIP)
                    {
                        if (opFile.Contains(".manifest")) continue;
                        opFile += ".zip";
                    }
                    FileInfo fileInfo = new FileInfo(AssetPathManager.Instance.GetPersistentDataPath(opFile, false));
                    if (fileInfo.Exists) fileInfo.Delete();
                    LDebug.Log(">>>Delete " + item.Key, LogColor.red);
                    LDebug.Log(">>>Delete Result " + DocumentAccessor.IsExists(AssetPathManager.Instance.GetPersistentDataPath(opFile, false)), LogColor.red);
                }

                //需要更新的对象：可以根据需求拓展对version的使用规则。
                //默认是更新版本号更高或者新增加的对象。
                var toUpdate = remoteABVersionsDic.Where(a => !localABVersionsDic.ContainsKey(a.Key) || a.Value.Version > localABVersionsDic[a.Key].Version);
                foreach (var item in toUpdate)
                {
                    string opFile = $"{FrameworkConfig.Instance.ABFolderName}/{item.Key}";
                    if (FrameworkConfig.Instance.useZIP)
                    {
                        if (opFile.Contains(".manifest")) continue;
                        opFile += ".zip";
                    }
                    LDebug.Log("Remote update..." + FrameworkConfig.Instance.RemoteUrlConfig + opFile + " 开始访问", LogColor.yellow);
                    yield return DocumentAccessor.ILoadAsset(FrameworkConfig.Instance.RemoteUrlConfig + opFile, (UnityWebRequest e) =>
                   {
                       LDebug.Log("Remote update..." + FrameworkConfig.Instance.RemoteUrlConfig + "/" + item.Key + "访问完成", LogColor.yellow);
                       DocumentAccessor.SaveAsset2LocalFile(AssetPathManager.Instance.GetPersistentDataPath(opFile, false), e.downloadHandler.data);
                   }, (e) =>
                   {
                       LDebug.LogError("Remote Error..." + e + ": " + item.Key);
                       if (!string.IsNullOrEmpty(e.error)) { canGoFurther = false; wrongFileName = item.Key; return; }
                   });

                    // 因为加载出问题导致无法继续时，目前先使用中断后续步骤，并弹窗提醒的方式搞
                    if (!canGoFurther) break;
                }

                // 因为加载出问题导致无法继续时，目前先使用中断后续步骤，并弹窗提醒的方式搞
                if (!canGoFurther)
                {
                    LDebug.LogError("Remote Update Abort..." + wrongFileName + " : " + remoteFilePath);
                    MsgManager.Instance.Broadcast(InternalEvent.REMOTE_UPDATE_ERROR, new MsgArgs(remoteContent, remoteFilePath, wrongFileName));
                    yield break;
                }

                _toZipList = remoteABVersionsDic;

                //更新文档
                DocumentAccessor.SaveAsset2LocalFile(localFilePath, contentByteArr);
                LDebug.Log("检测更新完成：" + remoteFilePath);
            }
            else LDebug.Log("文件不存在，不执行远程下载：" + remoteFilePath);

            //=========================ABPATH=======================//
            LDebug.Log("开始检测更新：" + CONFIG_AB_PATH);
            //1、下载最新的资源配置信息
            canGoFurther = true;
            localContent = null;
            wrongFileName = string.Empty;
            remoteFilePath = CONFIG_AB_PATH;

            //发送下载XX文件事件
            MsgManager.Instance.Broadcast(InternalEvent.HANDLING_REMOTE_RES, new MsgArgs(remoteFilePath, InternalEvent.RemoteStatus.Download));

            localFilePath = AssetPathManager.Instance.GetPersistentDataPath(remoteFilePath, false);
            //2、根据本地是否存在资源配置信息，如果不存在，则视为远程更新流程不执行
            if (DocumentAccessor.IsExists(localFilePath))
            {
                string remoteContent = null;
                byte[] contentByteArr = null;

                //远程主配置文件获取
                LDebug.Log("Remote update..." + FrameworkConfig.Instance.RemoteUrlConfig + "/" + remoteFilePath + " 开始读取", LogColor.yellow);
                yield return DocumentAccessor.ILoadAsset(FrameworkConfig.Instance.RemoteUrlConfig + remoteFilePath, callBack: (UnityWebRequest e) =>
                {
                    LDebug.Log("Remote update..." + remoteFilePath + "读取完成", LogColor.yellow);
                    remoteContent = e.downloadHandler.text;
                    contentByteArr = e.downloadHandler.data;
                },
                errorCallBack: (UnityWebRequest e) =>
                {
                    LDebug.LogError("Remote Error..." + e + ": " + remoteFilePath);
                    if (!string.IsNullOrEmpty(e.error)) { canGoFurther = false; return; }
                });

                //因为加载出问题导致无法继续时，目前先使用中断后续步骤，并弹窗提醒的方式搞
                if (!canGoFurther)
                {
                    MsgManager.Instance.Broadcast(InternalEvent.REMOTE_UPDATE_ERROR, new MsgArgs(remoteContent, remoteFilePath));
                    yield break;
                }
                LDebug.Log("检测更新完成：" + remoteFilePath);
            }
            else LDebug.Log("文件不存在，不执行远程下载：" + remoteFilePath);
        }

        //解压AB压缩包
        private bool ZipAB(Dictionary<string, ABVersion> remoteABVersionsDic)
        {
            if (!FrameworkConfig.Instance.useZIP) return true;

            bool zipSuccess = true;
            try
            {
                foreach (var item in remoteABVersionsDic.Keys)
                {
                    //解压完成后，删除原压缩包
                    string fileName = $"{FrameworkConfig.Instance.ABFolderName}/{item}.zip";
                    FileInfo fileInfo = new FileInfo(AssetPathManager.Instance.GetPersistentDataPath(fileName, false));
                    if (fileInfo.Exists)
                    {
                        ZipManager.ExtractZipContent(AssetPathManager.Instance.GetPersistentDataPath(fileName, false), AssetPathManager.Instance.GetPersistentDataPath(FrameworkConfig.Instance.ABFolderName + "/", false));
                        fileInfo.Delete();
                    }
                }
            }
            catch (Exception e) { zipSuccess = false; }

            return zipSuccess;
        }

        //解析ABVersion配置表
        private Dictionary<string, ABVersion> ResolveABContent(string contentResolve)
        {
            Dictionary<string, ABVersion> resultDict = new Dictionary<string, ABVersion>();

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
                    resultDict.Add(content[0], ab);
                }
            }

            return resultDict;
        }

        public void FinalExecute()
        {
            ZipAB(_toZipList);
        }
    }
}
