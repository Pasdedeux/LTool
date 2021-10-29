using UnityEngine;
using System.Collections;
using LitFramework.LitPool;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        

        //Loop Rect创建新对象的同时，均会与新子对象建立消息侦听，避免使用SendMessage
        public virtual GameObject GetObject( LoopScrollRect scrollRect )
        {
            var go = SpawnManager.Instance.SpwanObject(prefabName);
            go.transform.localScale = Vector3.one;
            return go;
        }

        //回收时解除消息绑定，及两者间关联
        public virtual void ReturnObject( LoopScrollRect scrollRect, Transform go )
        {
            SpawnManager.Instance.DespawnObject( go );
        }
    }
}
