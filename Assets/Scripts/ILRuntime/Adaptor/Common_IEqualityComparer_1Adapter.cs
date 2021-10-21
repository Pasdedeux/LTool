using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class IEqualityComparer_1_Int32Adapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32, System.Int32, System.Boolean> mEquals_0 = new CrossBindingFunctionInfo<System.Int32, System.Int32, System.Boolean>("Equals");
        static CrossBindingFunctionInfo<System.Int32, System.Int32> mGetHashCode_1 = new CrossBindingFunctionInfo<System.Int32, System.Int32>("GetHashCode");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEqualityComparer<System.Int32>);
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

        public class Adapter : System.Collections.Generic.IEqualityComparer<System.Int32>, CrossBindingAdaptorType
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

            public System.Boolean Equals(System.Int32 x, System.Int32 y)
            {
                return mEquals_0.Invoke(this.instance, x, y);
            }

            public System.Int32 GetHashCode(System.Int32 obj)
            {
                return mGetHashCode_1.Invoke(this.instance, obj);
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

