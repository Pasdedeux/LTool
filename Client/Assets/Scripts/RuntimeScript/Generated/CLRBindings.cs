using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_String_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Type_Binding.Register(app);
            LDebug_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Char_Binding.Register(app);
            System_Single_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            System_Text_StringBuilder_Binding.Register(app);
            System_Int32_Binding.Register(app);
            LitFramework_SingletonMono_1_FrameworkConfig_Binding.Register(app);
            LitFramework_FrameworkConfig_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
