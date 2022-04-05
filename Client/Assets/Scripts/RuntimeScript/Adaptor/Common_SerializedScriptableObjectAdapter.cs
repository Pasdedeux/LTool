using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{
    public class SerializedScriptableObjectAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType => throw new NotImplementedException();

        public override Type AdaptorType => throw new NotImplementedException();

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return null;
        }
    }
}

