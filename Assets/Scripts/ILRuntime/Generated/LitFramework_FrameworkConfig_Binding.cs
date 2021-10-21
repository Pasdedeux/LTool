using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class LitFramework_FrameworkConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(LitFramework.FrameworkConfig);

            field = type.GetField("scriptEnvironment", flag);
            app.RegisterCLRFieldGetter(field, get_scriptEnvironment_0);
            app.RegisterCLRFieldSetter(field, set_scriptEnvironment_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_scriptEnvironment_0, AssignFromStack_scriptEnvironment_0);


        }



        static object get_scriptEnvironment_0(ref object o)
        {
            return ((LitFramework.FrameworkConfig)o).scriptEnvironment;
        }

        static StackObject* CopyToStack_scriptEnvironment_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((LitFramework.FrameworkConfig)o).scriptEnvironment;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_scriptEnvironment_0(ref object o, object v)
        {
            ((LitFramework.FrameworkConfig)o).scriptEnvironment = (LitFramework.RunEnvironment)v;
        }

        static StackObject* AssignFromStack_scriptEnvironment_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            LitFramework.RunEnvironment @scriptEnvironment = (LitFramework.RunEnvironment)typeof(LitFramework.RunEnvironment).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((LitFramework.FrameworkConfig)o).scriptEnvironment = @scriptEnvironment;
            return ptr_of_this_method;
        }



    }
}
