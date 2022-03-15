using UnityEngine;
using System.Collections;
using LitFramework.MsgSystem;

namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
        public abstract void ProvideData(LoopScrollRect sr, int idx);
    }

    public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
        public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

        LoopScrollSendIndexSource() { }

        public override void ProvideData(LoopScrollRect sr, int idx)
        {
            MsgManager.Instance.Broadcast(InternalEvent.UI_SCROLL_ELEMENT, new MsgArgs(sr.GetInstanceID(), idx));
        }
    }

    public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

        public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(LoopScrollRect sr, int idx)
        {
            Log.TraceInfo($"  UI元件： {sr.name}以下子对象Index {idx}更新");
        }
    }
}