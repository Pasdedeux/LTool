using LitFramework.MsgSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// 列表窗口的元素项基类。用于规定通用列表组件的元素通用行为
    /// </summary>
    public abstract class BaseScrollElement
    {
        protected Transform linkedTrans;
        protected int linkedTransID;

        //LoopScrollPrefabRect 从池中取出时自动注册归属的菜单事件
        public virtual void RegisterEvent(LoopScrollRect lsr, Transform link ) { linkedTransID = lsr.GetInstanceID(); MsgManager.Instance.Register(InternalEvent.UI_SCROLL_ELEMENT, UpdateInfo); linkedTrans = link; }

        //LoopScrollPrefabRect 从池中取出时自动去注册归属的菜单事件
        public virtual void UnRegisterEvent() { MsgManager.Instance.UnRegister(InternalEvent.UI_SCROLL_ELEMENT, UpdateInfo); linkedTrans = null; }

        public virtual void Dispose() { }

        /// <summary>
        /// 接收列表更新事件
        /// </summary>
        /// <param name="args"></param>
        public abstract void UpdateInfo( MsgArgs args );

        public abstract void Init();
    }
}
