using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class BaseUIAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mOnClose_0 = new CrossBindingMethodInfo("OnClose");
        static CrossBindingMethodInfo<System.Object[]> mOnShow_1 = new CrossBindingMethodInfo<System.Object[]>("OnShow");
        static CrossBindingMethodInfo mDispose_2 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo mOnAdapter_3 = new CrossBindingMethodInfo("OnAdapter");
        static CrossBindingMethodInfo mOnBackPushed_4 = new CrossBindingMethodInfo("OnBackPushed");
        static CrossBindingMethodInfo mOnAwake_5 = new CrossBindingMethodInfo("OnAwake");
        static CrossBindingMethodInfo<System.Boolean> mOnEnabled_6 = new CrossBindingMethodInfo<System.Boolean>("OnEnabled");
        static CrossBindingMethodInfo<System.Boolean> mOnDisabled_7 = new CrossBindingMethodInfo<System.Boolean>("OnDisabled");
        static CrossBindingMethodInfo mOnStart_8 = new CrossBindingMethodInfo("OnStart");
        static CrossBindingMethodInfo mOnUpdate_9 = new CrossBindingMethodInfo("OnUpdate");
        static CrossBindingMethodInfo mFindMember_10 = new CrossBindingMethodInfo("FindMember");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(LitFramework.HotFix.BaseUI);
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

        public class Adapter : LitFramework.HotFix.BaseUI, CrossBindingAdaptorType
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

            public override void OnClose()
            {
                if (mOnClose_0.CheckShouldInvokeBase(this.instance))
                    base.OnClose();
                else
                    mOnClose_0.Invoke(this.instance);
            }

            protected override void FindMember()
            {
                mFindMember_10.Invoke(this.instance);
            }

            public override void OnShow(params object[] args)
            {
                mOnShow_1.Invoke(this.instance, args);
            }

            public override void Dispose()
            {
                if (mDispose_2.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_2.Invoke(this.instance);
            }

            public override void OnAdapter()
            {
                if (mOnAdapter_3.CheckShouldInvokeBase(this.instance))
                    base.OnAdapter();
                else
                    mOnAdapter_3.Invoke(this.instance);
            }

            public override void OnBackPushed()
            {
                if (mOnBackPushed_4.CheckShouldInvokeBase(this.instance))
                    base.OnBackPushed();
                else
                    mOnBackPushed_4.Invoke(this.instance);
            }

            public override void OnAwake()
            {
                mOnAwake_5.Invoke(this.instance);
            }

            public override void OnEnabled(System.Boolean replay)
            {
                if (mOnEnabled_6.CheckShouldInvokeBase(this.instance))
                    base.OnEnabled(replay);
                else
                    mOnEnabled_6.Invoke(this.instance, replay);
            }

            public override void OnDisabled(System.Boolean freeze)
            {
                if (mOnDisabled_7.CheckShouldInvokeBase(this.instance))
                    base.OnDisabled(freeze);
                else
                    mOnDisabled_7.Invoke(this.instance, freeze);
            }

            public override void OnStart()
            {
                if (mOnStart_8.CheckShouldInvokeBase(this.instance))
                    base.OnStart();
                else
                    mOnStart_8.Invoke(this.instance);
            }

            public override void OnUpdate()
            {
                if (mOnUpdate_9.CheckShouldInvokeBase(this.instance))
                    base.OnUpdate();
                else
                    mOnUpdate_9.Invoke(this.instance);
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

