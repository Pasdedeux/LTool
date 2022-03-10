#if FAIRYGUI_TOLUA
using System;
using LuaInterface;
#endif

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class EventBridge
    {
        public EventDispatcher owner;

        EventCallback0 _callback0Forward;
        EventCallback1 _callback1Forward;
        EventCallback0 _callback0;
        EventCallback1 _callback1;
        EventCallback1 _captureCallback;

        EventCallback0 _callback0Last;
        EventCallback1 _callback1Last;
        internal bool _dispatching;

        public EventBridge(EventDispatcher owner)
        {
            this.owner = owner;
        }

        public void AddCapture(EventCallback1 callback)
        {
            _captureCallback -= callback;
            _captureCallback += callback;
        }

        public void RemoveCapture(EventCallback1 callback)
        {
            _captureCallback -= callback;
        }

        public void Add(EventCallback1 callback)
        {
            _callback1 -= callback;
            _callback1 += callback;
        }

        public void Remove(EventCallback1 callback)
        {
            _callback1 -= callback;
        }

        public void Add(EventCallback0 callback)
        {
            _callback0 -= callback;
            _callback0 += callback;
        }

        public void Remove(EventCallback0 callback)
        {
            _callback0 -= callback;
        }

        public void AddLast(EventCallback1 callback)
        {
            _callback1Last -= callback;
            _callback1Last += callback;
        }

        public void RemoveLast(EventCallback1 callback)
        {
            _callback1Last -= callback;
        }
       
        public void AddLast(EventCallback0 callback)
        {
            _callback0Last -= callback;
            _callback0Last += callback;
        }

        public void RemoveLast(EventCallback0 callback)
        {
            _callback0Last-= callback;
        }
        public void AddForward(EventCallback1 callback)
        {
            _callback1Forward -= callback;
            _callback1Forward += callback;
        }

        public void RemoveForward(EventCallback1 callback)
        {
            _callback1Forward -= callback;
        }
        public void AddForward(EventCallback0 callback)
        {
            _callback0Forward -= callback;
            _callback0Forward += callback;
        }

        public void RemoveForward(EventCallback0 callback)
        {
            _callback0Forward -= callback;
        }


#if FAIRYGUI_TOLUA
        public void Add(LuaFunction func, LuaTable self)
        {
            EventCallback1 callback;
            if(self != null)
                callback = (EventCallback1)DelegateTraits<EventCallback1>.Create(func, self);
            else
                callback = (EventCallback1)DelegateTraits<EventCallback1>.Create(func);
            _callback1 -= callback;
            _callback1 += callback;
        }

        public void Add(LuaFunction func, GComponent self)
        {
            if (self._peerTable == null)
                throw new Exception("self is not connected to lua.");

            Add(func, self._peerTable);
        }

        public void Remove(LuaFunction func, LuaTable self)
        {
            LuaState state = func.GetLuaState();
            LuaDelegate target;
            if (self != null)
                target = state.GetLuaDelegate(func, self);
            else
                target = state.GetLuaDelegate(func);

            Delegate[] ds = _callback1.GetInvocationList();

            for (int i = 0; i < ds.Length; i++)
            {
                LuaDelegate ld = ds[i].Target as LuaDelegate;
                if (ld != null && ld.Equals(target))
                {
                    _callback1 = (EventCallback1)Delegate.Remove(_callback1, ds[i]);
                    //DelayDispose will cause problem
                    //state.DelayDispose(ld.func);
                    //if (ld.self != null)
                    //	state.DelayDispose(ld.self);
                    break;
                }
            }
        }

        public void Remove(LuaFunction func, GComponent self)
        {
            if (self._peerTable == null)
                throw new Exception("self is not connected to lua.");

            Remove(func, self._peerTable);
        }
#endif

        public bool isEmpty
        {
            get { return _callback1 == null && _callback0 == null && _captureCallback == null; }
        }

        public void Clear()
        {
#if FAIRYGUI_TOLUA
            //if (_callback1 != null)
            //{
            //	Delegate[] ds = _callback1.GetInvocationList();
            //	for (int i = 0; i < ds.Length; i++)
            //	{
            //		LuaDelegate ld = ds[i].Target as LuaDelegate;
            //		if (ld != null)
            //		{
            //			LuaState state = ld.func.GetLuaState();
            //			state.DelayDispose(ld.func);
            //			if (ld.self != null)
            //				state.DelayDispose(ld.self);
            //		}
            //	}
            //}
#endif
            _callback1Last = null;
            _callback0Last = null;
            _callback0Forward = null;
            _callback1Forward = null;
            _callback1 = null;
            _callback0 = null;
            _captureCallback = null;
        }

        public void CallInternal(EventContext context)
        {
            _dispatching = true;
            context.sender = owner;
            try
            {
                if (_callback0Forward != null)
                    _callback0Forward();
                if (_callback1Forward != null)
                    _callback1Forward(context);
                if (_callback1 != null)
                    _callback1(context);
                if (_callback0 != null)
                    _callback0();
                if (_callback1Last != null)
                    _callback1Last(context);
                if (_callback0Last != null)
                    _callback0Last();
                
            }
            finally
            {
                _dispatching = false;
            }
        }

        public void CallCaptureInternal(EventContext context)
        {
            if (_captureCallback == null)
                return;

            _dispatching = true;
            context.sender = owner;
            try
            {
                _captureCallback(context);
            }
            finally
            {
                _dispatching = false;
            }
        }
    }
}
