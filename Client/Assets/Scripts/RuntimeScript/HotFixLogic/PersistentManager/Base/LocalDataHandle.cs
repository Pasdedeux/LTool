/*======================================
* 项目名称 ：Assets.Scripts.Controller
* 项目描述 ：
* 类 名 称 ：CameraController
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Controller
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：周霞
* 创建时间 ：2022/1/8 10:53:20
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2022. All rights reserved.
*******************************************************************
======================================*/

using LitFramework.LitTool;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace LitFramework.Persistent
{
    /// <summary>
    /// LocalData类数据存取默认均在Persisit目录进行
    /// </summary>
    public static class LocalDataHandle
    {
        public static System.Action onSaveData;
        private static string m_ConfigDire = "Config/";
        private static string m_LocalDataDire = "LocalData/";
        private static string m_FileType = ".bin";

        public static void SaveData(object aValue)
        {
            Type rType = aValue.GetType();
            string direPath = AssetPathManager.Instance.GetPersistentDataPath(m_LocalDataDire , false);
            if(!Directory.Exists(direPath))
            {
                Directory.CreateDirectory(direPath);
            }
            string localTextPath = direPath + rType.ToString() + m_FileType;
            DocumentAccessor.SaveAsset2LocalFileByJson(aValue, localTextPath);
            onSaveData?.Invoke();
        }
        public static T LoadData<T>() where T : class, new()
        {
            string aName = typeof(T).ToString();
            string localTextPath = AssetPathManager.Instance.GetPersistentDataPath(m_LocalDataDire + aName + m_FileType, false);

            if (!File.Exists(localTextPath)) return new T();

            T tObject = DocumentAccessor.ToObject<T>(m_LocalDataDire + aName + m_FileType, true);
            return tObject;
        }



        public static void EditorSaveConfig(object aValue)
        {
            Type rType = aValue.GetType();
            string direPath = AssetPathManager.Instance.GetStreamAssetDataPath(m_ConfigDire, false);
            if (!Directory.Exists(direPath))
            {
                Directory.CreateDirectory(direPath);
            }
            string localTextPath = direPath + rType.ToString() + m_FileType;
            DocumentAccessor.SaveAsset2LocalFileByJson(aValue, localTextPath);
            onSaveData?.Invoke();
        }
        public static T LoadConfig<T>() where T : class
        {
            string aName = typeof(T).ToString();
            T tObject = null;
            if (Application.isPlaying)
            {
                tObject = DocumentAccessor.ToObject<T>(m_ConfigDire + aName + m_FileType, FrameworkConfig.Instance.UsePersistantPath);
            }else
            {
                tObject = DocumentAccessor.ToObject<T>(m_ConfigDire + aName + m_FileType, false);
            }
            return tObject;
        }

    }
    public class LocalDataBase
    {
        protected bool isCanSave;
        public virtual void SaveFlag()
        {
            isCanSave = true;
        }
        public virtual void SaveImmit()
        {
            if (isCanSave)
            {
                LocalDataHandle.SaveData(this);
                isCanSave = false;
            }
        }
    }
}

