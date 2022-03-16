using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class BaseScrollElementAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo<UnityEngine.UI.LoopScrollRect, UnityEngine.Transform> mRegisterEvent_0 = new CrossBindingMethodInfo<UnityEngine.UI.LoopScrollRect, UnityEngine.Transform>("RegisterEvent");
        static CrossBindingMethodInfo mUnRegisterEvent_1 = new CrossBindingMethodInfo("UnRegisterEvent");
        static CrossBindingMethodInfo<global::MsgArgs> mUpdateInfo_2 = new CrossBindingMethodInfo<global::MsgArgs>("UpdateInfo");
        static CrossBindingMethodInfo mOnInit_3 = new CrossBindingMethodInfo("OnInit");
        static CrossBindingMethodInfo mSetElement_4 = new CrossBindingMethodInfo("SetElement");
        static CrossBindingMethodInfo mDispose_5 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo mFindMenber_6 = new CrossBindingMethodInfo("FindMember");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Assets.Scripts.UI.BaseScrollElement);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Assets.Scripts.UI.BaseScrollElement, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void RegisterEvent(UnityEngine.UI.LoopScrollRect lsr, UnityEngine.Transform link)
            {
                if (mRegisterEvent_0.CheckShouldInvokeBase(this.instance))
                    base.RegisterEvent(lsr, link);
                else
                    mRegisterEvent_0.Invoke(this.instance, lsr, link);
            }

            public override void UnRegisterEvent()
            {
                if (mUnRegisterEvent_1.CheckShouldInvokeBase(this.instance))
                    base.UnRegisterEvent();
                else
                    mUnRegisterEvent_1.Invoke(this.instance);
            }

            public override void UpdateInfo(global::MsgArgs args)
            {
                mUpdateInfo_2.Invoke(this.instance, args);
            }

            public override void OnInit()
            {
                if (mOnInit_3.CheckShouldInvokeBase(this.instance))
                    base.OnInit();
                else
                    mOnInit_3.Invoke(this.instance);
            }

            public override void SetElement()
            {
                mSetElement_4.Invoke(this.instance);
            }

            public override void Dispose()
            {
                mDispose_5.Invoke(this.instance);
            }

            public override void FindMember()
            {
                if (mFindMenber_6.CheckShouldInvokeBase(this.instance))
                    base.FindMember();
                else
                    mFindMenber_6.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}
