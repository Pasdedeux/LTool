using LitFramework.LitTool;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public static class LocalDataHandle
{
    public static System.Action onSaveData;

    public static void SaveData(object aValue)
    {
        Type rType = aValue.GetType();
        string localTextPath = AssetPathManager.Instance.GetPersistentDataPath( rType.ToString() + ".bin", false );
        DocumentAccessor.SaveAsset2LocalFileByJson(aValue, localTextPath);
        onSaveData?.Invoke();
    }
    public static T LoadData<T>() where T :  new()
    {
        string aName = typeof( T ).ToString();

        //  Log.TraceInfo(aName + "------------------LoadData");
        string localTextPath = AssetPathManager.Instance.GetPersistentDataPath( aName + ".bin", false );
        if ( !File.Exists( localTextPath ) )
        {
            return new T();
        }
        //Log.TraceInfo(">>> LOAD SAVE DATA");
        T tObject = LitJson.JsonMapper.ToObject<T>( DocumentAccessor.ReadFile( localTextPath ) );
        return tObject;
    }
    public static void SaveConfig(object aValue)
    {
        Type rType = aValue.GetType();
        string localTextPath = AssetPathManager.Instance.GetStreamAssetDataPath(rType.ToString() + ".bin", false);
        DocumentAccessor.SaveAsset2LocalFileByJson(aValue, localTextPath);
        onSaveData?.Invoke();
    }
    public static T LoadConfig<T>() where T : new()
    {
        string aName = typeof(T).ToString();

        //  Log.TraceInfo(aName + "------------------LoadData");
        string localTextPath;
        
        if (LitFramework.FrameworkConfig.Instance.UsePersistantPath)
        {
            localTextPath = AssetPathManager.Instance.GetPersistentDataPath(aName + ".bin", false);
        }
        else
        {
            localTextPath = AssetPathManager.Instance.GetStreamAssetDataPath(aName + ".bin", false);
        }

        if (!File.Exists(localTextPath))
        {
            return new T();
        }
        //Log.TraceInfo(">>> LOAD SAVE DATA");
        T tObject = LitJson.JsonMapper.ToObject<T>(DocumentAccessor.ReadFile(localTextPath));
        return tObject;
    }

}
//[System.Serializable]
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

