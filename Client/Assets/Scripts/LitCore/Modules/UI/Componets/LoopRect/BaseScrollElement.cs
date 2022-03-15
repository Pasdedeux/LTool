﻿using LitFramework.MsgSystem;
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
        public int index;
        private Transform _linkedTrans;
        public Transform linkedTrans {
            set
            {
                _linkedTrans = value;
                if(_linkedTrans)
                {
                    FindMember();
                    OnInit();
                }
            }
            get => _linkedTrans;
        }

        protected int linkedTransID;

        //LoopScrollPrefabRect 从池中取出时自动注册归属的菜单事件
        public virtual void RegisterEvent(LoopScrollRect lsr, Transform link) { linkedTransID = lsr.GetInstanceID(); linkedTrans = link; MsgManager.Instance.Register(InternalEvent.UI_SCROLL_ELEMENT, OnUpdateInfo); }

        //LoopScrollPrefabRect 从池中取出时自动去注册归属的菜单事件
        public virtual void UnRegisterEvent() { MsgManager.Instance.UnRegister(InternalEvent.UI_SCROLL_ELEMENT, OnUpdateInfo); linkedTrans = null; }


        /// <summary>
        /// 接收列表更新事件
        /// </summary>
        /// <param name="args"></param>
        protected void OnUpdateInfo(MsgArgs args)
        {
            //Log.TraceInfo(">>>" + args.Get<int>(1) + ">>>>" + index, LogColor.red);
            if (args.Get<int>(0) != linkedTransID || args.Get<int>(1) != index) return;
            UpdateInfo(args);
        }

        public abstract void UpdateInfo(MsgArgs args);
        public virtual void OnInit() { }
        public abstract void SetElement();

        public abstract void Dispose();
        public virtual void FindMember()
        {
        }
    }
}
