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
    unsafe class UIType_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIType);

            field = type.GetField("uiNodeType", flag);
            app.RegisterCLRFieldGetter(field, get_uiNodeType_0);
            app.RegisterCLRFieldSetter(field, set_uiNodeType_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_uiNodeType_0, AssignFromStack_uiNodeType_0);
            field = type.GetField("uiShowMode", flag);
            app.RegisterCLRFieldGetter(field, get_uiShowMode_1);
            app.RegisterCLRFieldSetter(field, set_uiShowMode_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_uiShowMode_1, AssignFromStack_uiShowMode_1);
            field = type.GetField("uiTransparent", flag);
            app.RegisterCLRFieldGetter(field, get_uiTransparent_2);
            app.RegisterCLRFieldSetter(field, set_uiTransparent_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_uiTransparent_2, AssignFromStack_uiTransparent_2);


        }



        static object get_uiNodeType_0(ref object o)
        {
            return ((global::UIType)o).uiNodeType;
        }

        static StackObject* CopyToStack_uiNodeType_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIType)o).uiNodeType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_uiNodeType_0(ref object o, object v)
        {
            ((global::UIType)o).uiNodeType = (global::UINodeTypeEnum)v;
        }

        static StackObject* AssignFromStack_uiNodeType_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UINodeTypeEnum @uiNodeType = (global::UINodeTypeEnum)typeof(global::UINodeTypeEnum).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((global::UIType)o).uiNodeType = @uiNodeType;
            return ptr_of_this_method;
        }

        static object get_uiShowMode_1(ref object o)
        {
            return ((global::UIType)o).uiShowMode;
        }

        static StackObject* CopyToStack_uiShowMode_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIType)o).uiShowMode;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_uiShowMode_1(ref object o, object v)
        {
            ((global::UIType)o).uiShowMode = (global::UIShowModeEnum)v;
        }

        static StackObject* AssignFromStack_uiShowMode_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIShowModeEnum @uiShowMode = (global::UIShowModeEnum)typeof(global::UIShowModeEnum).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((global::UIType)o).uiShowMode = @uiShowMode;
            return ptr_of_this_method;
        }

        static object get_uiTransparent_2(ref object o)
        {
            return ((global::UIType)o).uiTransparent;
        }

        static StackObject* CopyToStack_uiTransparent_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIType)o).uiTransparent;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_uiTransparent_2(ref object o, object v)
        {
            ((global::UIType)o).uiTransparent = (global::UITransparentEnum)v;
        }

        static StackObject* AssignFromStack_uiTransparent_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UITransparentEnum @uiTransparent = (global::UITransparentEnum)typeof(global::UITransparentEnum).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((global::UIType)o).uiTransparent = @uiTransparent;
            return ptr_of_this_method;
        }



    }
}
