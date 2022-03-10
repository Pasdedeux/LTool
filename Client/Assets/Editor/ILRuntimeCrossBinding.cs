#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using ILRuntime.Runtime;
using Assets.Scripts.UI;

[System.Reflection.Obfuscation( Exclude = true )]
public class ILRuntimeCrossBinding
{
    [MenuItem( "ILRuntime/生成跨域继承适配器" )]
    static void GenerateCrossbindAdapter()
    {
        #region Common
        string clsName, realClsName;
        bool isByRef;
        List<Type> toGenCommonClass = new List<Type>()
        {
            typeof( Queue ),
            typeof( IEqualityComparer<int> ),
            typeof( ApplicationException ),
            typeof( BaseScrollElement ),
            //typeof( System.Exception ),
            //typeof( System.Collections.IEnumerable ),
            //typeof( Sirenix.OdinInspector.SerializedScriptableObject ),
        };

        for ( int i = 0; i < toGenCommonClass.Count; i++ )
        {
            var tType = toGenCommonClass[ i ];
            var name = tType.Name.Contains( "`" ) ? ( tType.Name.Replace( "`", "_" ) ) : tType.Name;
            using ( System.IO.StreamWriter sw = new System.IO.StreamWriter( Assets.Scripts.ILRScriptCall.ADAPTOR_SAVE_PATH + string.Format( "Common_{0}Adapter.cs", name ) ) )
                sw.WriteLine( ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode( tType, Assets.Scripts.ILRScriptCall.HOTFIX_NAMESPACE ) );
        }

        using ( System.IO.StreamWriter sw = new System.IO.StreamWriter( Assets.Scripts.ILRScriptCall.ADAPTOR_SAVE_PATH + "Common_AdapterRegister.cs" ) )
        {
            sw.WriteLine( "namespace " + Assets.Scripts.ILRScriptCall.HOTFIX_NAMESPACE + "{" );
            sw.WriteLine( "  public class Common_AdapterRegister {" );
            sw.WriteLine( "      public static void RegisterAdaptor( ILRuntime.Runtime.Enviorment.AppDomain domain){" );
            for ( int i = 0; i < toGenCommonClass.Count; i++ )
            {
                var tType = toGenCommonClass[ i ];
                tType.GetClassName( out clsName, out realClsName, out isByRef, true );
                sw.WriteLine( string.Format( "        domain.RegisterCrossBindingAdaptor( new {0}Adapter() );", clsName ) );
            }
            sw.WriteLine( "      }" );
            sw.WriteLine( "  }" );
            sw.WriteLine( "}" );
        }

        #endregion

        #region GamePlay
        toGenCommonClass.Clear();
        toGenCommonClass = new List<Type>()
        {
            //TODO 自定义继承绑定
            //..
        };
        for (int i = 0; i < toGenCommonClass.Count; i++)
        {
            var tType = toGenCommonClass[i];
            var name = tType.Name.Contains("`") ? (tType.Name.Replace("`", "_")) : tType.Name;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Assets.Scripts.ILRScriptCall.ADAPTOR_SAVE_PATH + string.Format("GamePlay_{0}Adapter.cs", name)))
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(tType, Assets.Scripts.ILRScriptCall.HOTFIX_NAMESPACE));
        }

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Assets.Scripts.ILRScriptCall.ADAPTOR_SAVE_PATH + "GamePlay_AdapterRegister.cs"))
        {
            sw.WriteLine("namespace " + Assets.Scripts.ILRScriptCall.HOTFIX_NAMESPACE + "{");
            sw.WriteLine("  public class GamePlay_AdapterRegister {");
            sw.WriteLine("      public static void RegisterAdaptor( ILRuntime.Runtime.Enviorment.AppDomain domain){");

            for (int i = 0; i < toGenCommonClass.Count; i++)
            {
                var tType = toGenCommonClass[i];
                tType.GetClassName(out clsName, out realClsName, out isByRef, true);

                sw.WriteLine(string.Format("        domain.RegisterCrossBindingAdaptor( new {0}Adapter() );", clsName));
            }
            sw.WriteLine("      }");
            sw.WriteLine("  }");
            sw.WriteLine("}");
        }

        #endregion

        #region FariyGUI

        //****************************** create adapter *************************************
        toGenCommonClass.Clear();
        toGenCommonClass = new List<Type>()
        {
            typeof(FairyGUI.GLoader),
            typeof(FairyGUI.GLoader3D),
            typeof(FairyGUI.Window),
            typeof(FairyGUI.GTree),
            typeof(FairyGUI.GTextInput),
            typeof(FairyGUI.GTextField),
            typeof(FairyGUI.GSlider),
            typeof(FairyGUI.GScrollBar),
            typeof(FairyGUI.GRoot),
            typeof(FairyGUI.GRichTextField),
            typeof(FairyGUI.GProgressBar),
            typeof(FairyGUI.GObject),
            typeof(FairyGUI.GMovieClip),
            typeof(FairyGUI.GList),
            typeof(FairyGUI.GLabel),
            typeof(FairyGUI.GImage),
            typeof(FairyGUI.GGraph),
            typeof(FairyGUI.GGroup),
            typeof(FairyGUI.GComboBox),
            typeof(FairyGUI.GComponent),
            typeof(FairyGUI.GButton)
        };

        for (int i = 0; i < toGenCommonClass.Count; i++)
        {
            var tType = toGenCommonClass[i];
            var name = tType.Name.Contains("`") ? (tType.Name.Replace("`", "_")) : tType.Name;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Assets.Scripts.ILRScriptCall.ADAPTOR_SAVE_PATH + string.Format("FairyGUI_{0}Adapter.cs", name)))
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(tType, Assets.Scripts.ILRScriptCall.HOTFIX_NAMESPACE));
        }

        /******************************regist adapter * ************************************/

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Assets.Scripts.ILRScriptCall.ADAPTOR_SAVE_PATH + "FairyGUI_AdapterRegister.cs"))
        {
            sw.WriteLine("namespace " + Assets.Scripts.ILRScriptCall.HOTFIX_NAMESPACE + "{");
            sw.WriteLine("  public class FairyGUI_AdapterRegister {");
            sw.WriteLine("      public static void RegisterAdaptor( ILRuntime.Runtime.Enviorment.AppDomain domain){");
            for (int i = 0; i < toGenCommonClass.Count; i++)
            {
                var tType = toGenCommonClass[i];
                tType.GetClassName(out clsName, out realClsName, out isByRef, true);
                sw.WriteLine(string.Format("        domain.RegisterCrossBindingAdaptor( new {0}Adapter() );", clsName));
            }
            sw.WriteLine("      }");
            sw.WriteLine("  }");
            sw.WriteLine("}");
        }
        #endregion

        toGenCommonClass.Clear();
        AssetDatabase.Refresh();
    }
}
#endif
