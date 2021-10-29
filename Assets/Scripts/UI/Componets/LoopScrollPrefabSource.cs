using UnityEngine;
using System.Collections;
using LitFramework.LitPool;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        public int poolSize = 5;

        private bool inited = false;
        public virtual GameObject GetObject()
        {
            var go = SpawnManager.Instance.SpwanObject(prefabName);
            go.transform.localScale = Vector3.one;
            return go;
        }

        public virtual void ReturnObject( Transform go )
        {
            go.SendMessage( "ScrollCellReturn", SendMessageOptions.DontRequireReceiver );
            SpawnManager.Instance.DespawnObject( go );
        }
    }
}
