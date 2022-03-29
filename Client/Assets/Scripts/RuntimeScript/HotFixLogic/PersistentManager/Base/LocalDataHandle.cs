/**************************************************************** 
 * 作    者：周霞
 * 创建时间：
 * 当前版本：1.0.0.1 
 *  
 * 描述说明： 
 * 
 * 修改历史： 
 * 
***************************************************************** 
 * Copyright @ Derek Liu 2022 All rights reserved 
*****************************************************************/

using LitFramework;
using LitFramework.LitTool;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// 对于Data类型数据默认使用 PersistentDataPath
/// 
/// 对于Config类型，存储时使用 StreamingAsset，读取时根据配置选项走
/// </summary>
public static class LocalDataHandle
{
    public static System.Action onSaveData;
    private static string m_FileType = ".bin";

    /// <summary>
    /// 存数据类默认存可读写目录
    /// </summary>
    /// <param name="aValue"></param>
    public static void SaveData(object aValue)
    {
        Type rType = aValue.GetType();
        DocumentAccessor.ToJson(aValue, rType.ToString() + m_FileType, true);
        
        onSaveData?.Invoke();
    }
    /// <summary>
    /// 所以读也只在可读写目录
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadData<T>() where T : class, new()
    {
        string aName = typeof( T ).ToString() + m_FileType;
        string localTextPath = AssetPathManager.Instance.GetPersistentDataPath( aName, false );

        if ( !File.Exists( localTextPath ) ) return new T();

        T tObject = DocumentAccessor.ToObject<T>(aName, true);
        return tObject;
    }



    /// <summary>
    /// 编辑器存储CommonData.bin到本地目录
    /// </summary>
    /// <param name="aValue"></param>
    public static void EditorSaveConfig(object aValue)
    {
        Type rType = aValue.GetType();
        DocumentAccessor.ToJson(aValue, rType.ToString() + m_FileType, false);

        onSaveData?.Invoke();
    }
    /// <summary>
    /// 读根据环境设定做筛选
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadConfig<T>() where T : class, new()
    {
        string aName = typeof(T).ToString() + m_FileType;
        T tObject = DocumentAccessor.ToObject<T>(aName, Application.isPlaying && FrameworkConfig.Instance.UsePersistantPath);
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
        if(isCanSave)
        {
            LocalDataHandle.SaveData(this);
            isCanSave = false;
        }
    }
}

