using UnityEngine;
using System.Collections;
using LitFramework.LitPool;
using System.Collections.Generic;
using Assets.Scripts.UI;
using System;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public string prefabName;

        public Func<BaseScrollElement> ExecuteTypeScript { get; set; }

        //全部经由LoopScrollPrefab实现的子元素查找字典
        public static Dictionary<int, Dictionary<Transform, BaseScrollElement>> ScrollElementDict = new Dictionary<int, Dictionary<Transform, BaseScrollElement>>();

        //Loop Rect创建新对象的同时，均会与新子对象建立消息侦听，避免使用SendMessage
        public virtual GameObject GetObject(LoopScrollRect scrollRect, int idx)
        {
            var go = SpawnManager.Instance.SpwanObject(prefabName);
            BindElementScript(go.transform, scrollRect, idx);

            return go;
        }

        public void BindElementScript(Transform trans, LoopScrollRect scrollRect, int idx)
        {
            var insID = scrollRect.GetInstanceID();

            trans.localScale = Vector3.one;
            if (!ScrollElementDict.ContainsKey(insID)) ScrollElementDict.Add(insID, new Dictionary<Transform, BaseScrollElement>());
            if (ScrollElementDict[insID].ContainsKey(trans))
            {
                ScrollElementDict[insID][trans].UnRegisterEvent();
                ScrollElementDict[insID][trans].Dispose();
                ScrollElementDict[insID][trans] = null;
                ScrollElementDict[insID].Remove(trans);
            }
            ScrollElementDict[insID][trans] = ExecuteTypeScript();
            ScrollElementDict[insID][trans].RegisterEvent(scrollRect, trans);
            //Log.TraceInfo("赋值ID " + idx);
            ScrollElementDict[insID][trans].index = idx;
            ScrollElementDict[insID][trans].SetElement();
        }

        //回收时解除消息绑定，及两者间关联
        public virtual void ReturnObject(LoopScrollRect scrollRect, Transform trans)
        {
            var insID = scrollRect.GetInstanceID();
            try
            {
                ScrollElementDict[insID][trans].UnRegisterEvent();
                ScrollElementDict[insID][trans].Dispose();
                ScrollElementDict[insID][trans] = null;
                ScrollElementDict[insID].Remove(trans);
            }
            catch (System.Exception)
            {
                throw new System.Exception($"{scrollRect.name}不存在于事件列表: {trans.name}");
            }
            SpawnManager.Instance.DespawnObject(trans);
        }
    }
}
