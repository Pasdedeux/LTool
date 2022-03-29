using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRBaseModel;
using UnityEngine.U2D;
using System;
using LitFramework.Singleton;

public class SpriteAtlasHelper : Singleton<SpriteAtlasHelper>
{
    public  Sprite Default;
    public  Dictionary<string, SpriteAtlas> AtlasDic = new Dictionary<string, SpriteAtlas>();
    private Dictionary<string, Sprite> sSpritesDic = new Dictionary<string, Sprite>();
    public  Sprite[] Flowers;
    public  Sprite GetFlowerSprite(int aID)
    {
        return Flowers[aID - 1];
    }
    public void DoStart()
    {
        SpriteAtlasManager.atlasRequested += (string tag, System.Action<SpriteAtlas> action) =>
        {
            LDebug.Log(">>>>>>>Request sprite atlas: " + tag); 
            SpriteAtlas tAtlas = null;
            if(!AtlasDic.TryGetValue(tag ,out tAtlas))
            {
                tAtlas = LoadAtlas(tag);
                AtlasDic[tag] = tAtlas;
            }
            action(tAtlas);
        };

        SpriteAtlasManager.atlasRegistered += (SpriteAtlas spriteAtlas) =>
        {
            LDebug.Log(">>>>>>>AtlasRegistered: " + spriteAtlas.name);
        };
        Texture2D defaultTex = Texture2D.blackTexture;
        Default = Sprite.Create(defaultTex, new Rect(0,0, defaultTex.width, defaultTex.height),new Vector2(0.5f,0.5f));
    }

    private SpriteAtlas LoadAtlas(string aName)
    {
      return RsLoadManager.Instance.Load<SpriteAtlas>("Atlas/" + aName);
    }
    /// <summary>
    /// 获取Sprite
    /// </summary>
    /// <param name="aPath">图集名字不带Atlas/图片名或者图片名字</param>
    /// <returns></returns>
    public Sprite GetSprite(string aPath)
    {
        if(string.IsNullOrEmpty(aPath))
        {
            return Default;
        }
        string[] split = aPath.Split('/');
        Sprite tSprite = null;
        if (sSpritesDic.TryGetValue(aPath, out tSprite))
        {
            return tSprite;
        }
        if (split.Length==2)
        {
            tSprite = GetSprite(split[1],split[0] + "Atlas");
            if(Default==tSprite)
            {

              Sprite sprite = RsLoadManager.Instance.Load<Sprite>(aPath);
              if(sprite)
              {
                  //tSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                  sSpritesDic.Add(aPath, sprite);
                  return sprite;
              }
               
            }
            return tSprite;
        }

        if (split.Length == 1)
        {
            
            foreach (SpriteAtlas spriteAtlas in AtlasDic.Values)
            {
                
                string _path = spriteAtlas.name + "/" + aPath;
                if (sSpritesDic.TryGetValue(_path, out tSprite))
                {
                    return tSprite;
                }
                try
                {
                    tSprite = spriteAtlas.GetSprite(aPath);
                }
                catch (Exception ex)
                {

                }

                if (tSprite != null)
                {
                    sSpritesDic[_path] = tSprite;
                    return tSprite;
                }
            }
        }
         tSprite = RsLoadManager.Instance.Load<Sprite>(aPath);
        if (tSprite)
        {
            //tSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            sSpritesDic.Add(aPath, tSprite);
            return tSprite;
        }
        return Default;
    }
    public Sprite GetSprite(string aName,string aAtlasName = "UI")
    {
        string path = aAtlasName + "/" + aName;
        SpriteAtlas tAtlas = null;
        AtlasDic.TryGetValue(aAtlasName, out tAtlas);
        if (tAtlas == null)
        {
            tAtlas = LoadAtlas(aAtlasName);
            if(tAtlas!=null)
            {
                AtlasDic[aAtlasName] = tAtlas;
            }else
            {
                return Default;
            }
        }

        Sprite tTmper = null;
        if(sSpritesDic.TryGetValue(path, out tTmper))
        {
            return tTmper;
        }

        Sprite tSprite = null;
        try
        {
            tSprite = tAtlas.GetSprite(aName);
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        if(tSprite == null)
        {
            return Default;
        }

        sSpritesDic[path] = tSprite;

        return tSprite;
    }

    public void SetSprite(SpriteRenderer aSprite, string aName, string aAtlasName = "UI")
    {
        aSprite.sprite = GetSprite(aName, aAtlasName);
    }

}
