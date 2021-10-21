using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class SerializedScriptableObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mOnAfterDeserialize_0 = new CrossBindingMethodInfo("OnAfterDeserialize");
        static CrossBindingMethodInfo mOnBeforeSerialize_1 = new CrossBindingMethodInfo("OnBeforeSerialize");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Sirenix.OdinInspector.SerializedScriptableObject);
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

        public class Adapter : Sirenix.OdinInspector.SerializedScriptableObject, CrossBindingAdaptorType
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

            protected override void OnAfterDeserialize()
            {
                if (mOnAfterDeserialize_0.CheckShouldInvokeBase(this.instance))
                    base.OnAfterDeserialize();
                else
                    mOnAfterDeserialize_0.Invoke(this.instance);
            }

            protected override void OnBeforeSerialize()
            {
                if (mOnBeforeSerialize_1.CheckShouldInvokeBase(this.instance))
                    base.OnBeforeSerialize();
                else
                    mOnBeforeSerialize_1.Invoke(this.instance);
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

