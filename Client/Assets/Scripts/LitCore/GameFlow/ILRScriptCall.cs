/*======================================
* 项目名称 ：Assets.Scripts.ILRuntime
* 项目描述 ：
* 类 名 称 ：ILRScriptCall
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.ILRuntime
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/16 17:08:41
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using LHotfixProject;
using ILRuntime.Runtime;
using ILRuntime.Runtime.Stack;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using PathologicalGames;
using System.Reflection;
using LitFramework.UI.Base;

namespace Assets.Scripts
{
    public class ILRScriptCall : IScriptCall
    {
        //热更dll名称
        public static string DLL_NAME = "LHotfixProject.dll";
        //热更调试库名称
        public static string PDB_NAME = "LHotfixProject.pdb";
        //热更工程命名空间
        public static string HOTFIX_NAMESPACE = "LHotfixProject";
        //热更工程启动类
        public static string HOTFIX_CLASS = "HotfixEntry";
        //热更工程预启动方法
        public static string HOTFIX_PRE_FUNCNAME = "Pre_RunGame_ILRuntime";
        //热更工程启动方法
        public static string HOTFIX_FUNCNAME = "RunGame_ILRuntime";
        //跨域继承类适配器存储路径
        public static string ADAPTOR_SAVE_PATH = "Assets/Scripts/RuntimeScript/Adaptor/";
        //CLR绑定文件存储路径
        public static string CLR_SAVE_PATH = "Assets/Scripts/RuntimeScript/Generated";
        //DLL、  PDB文件存储路径
        public static string DLL_SAVE_PATH = "Assets/StreamingAssets/";

        private static ILRuntime.Runtime.Enviorment.AppDomain _appdomain;
        public static ILRuntime.Runtime.Enviorment.AppDomain AppDomain
        {
            get
            {
                return _appdomain;
            }
        }
        private bool _isPreInit = false;
        /// <summary>
        /// 预加载部分
        /// </summary>
        public void PreStartRun()
        {
            //热更模式下，对象池扩展加载需要延迟到DLL预加载执行，原本位置只针对DOTNET环境执行
            GameObject.FindObjectOfType<ExtendSpawnPool>().LoadSpawnConfig(true);

            _appdomain.Invoke(string.Format("{0}.{1}", HOTFIX_NAMESPACE, HOTFIX_CLASS), HOTFIX_PRE_FUNCNAME, null, null);
            _isPreInit = true;
        }

        //热更主逻辑启动
        public void StartRun()
        {
            if (!_isPreInit) PreStartRun();

            _appdomain.Invoke(string.Format("{0}.{1}", HOTFIX_NAMESPACE, HOTFIX_CLASS), HOTFIX_FUNCNAME, null, null);
        }

        public void Load()
        {
            _appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            MemoryStream fs = null;
            MemoryStream p = null;

            /*yield return */
            DocumentAccessor.LoadAsset(FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath(DLL_NAME) : AssetPathManager.Instance.GetStreamAssetDataPath(DLL_NAME), (UnityWebRequest e) => { fs = new MemoryStream(e.downloadHandler.data); });
            /*yield return*/

#if UNITY_EDITOR
            //The PDB file is not recongized as a Windows PDB file ? WTF?
            DocumentAccessor.LoadAsset(FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath(PDB_NAME) : AssetPathManager.Instance.GetStreamAssetDataPath(PDB_NAME), (UnityWebRequest e) => { p = new MemoryStream(e.downloadHandler.data); });

            if (FrameworkConfig.Instance.showLog)
                _appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            else
#endif
                _appdomain.LoadAssembly(fs);

            RegisterAdaptor(_appdomain);
            RegisterDelegate(_appdomain);

            OnILRuntimeInitialized();
        }

        void OnILRuntimeInitialized()
        {
            if (FrameworkConfig.Instance.showLog)
            {
                _appdomain.DebugService.StartDebugService(56000);
                //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
                _appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            }

            var allTypes = _appdomain.LoadedTypes.Values.ToList();
            var interfaceType = typeof(IBaseUI);
            for (int indexType = 0; indexType < allTypes.Count; indexType++)
            {
                var item = allTypes[indexType];
                var reflectType = item.ReflectionType;
                if (!reflectType.IsAbstract && reflectType.BaseType != null && interfaceType.IsAssignableFrom(reflectType.BaseType.BaseType))
                {
                    LDebug.Log(reflectType.Name);
                    //通过程序集获取到他的返回实例对象方法  并且初始化对象
                    System.Reflection.MethodInfo mif = reflectType.GetMethod("RegistSystem");
                    if (mif != null)
                    {
                        //目前只认静态方法
                        if (mif.IsStatic)
                            mif.Invoke(null, new object[] { reflectType.Namespace + "." + reflectType.Name });
                    }
                }
            }

            ////是否以读取数据库方式读取配置
            //if (FrameworkConfig.Instance.useSql)
            //{
            //    var mainClass = _appdomain.LoadedTypes["SQLite.SQLManager"].ReflectionType;
            //    var method = mainClass.GetMethod("Install");
            //    method.Invoke(null, null);
            //}
        }



        /// <summary>
        /// 适配器的注册
        /// </summary>
        public static void RegisterAdaptor(ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            domain.RegisterCrossBindingAdaptor(new BaseUIAdapter());
            domain.RegisterCrossBindingAdaptor(new DebugAdapter());
            domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());

            Common_AdapterRegister.RegisterAdaptor(domain);
            FairyGUI_AdapterRegister.RegisterAdaptor(domain);
            GamePlay_AdapterRegister.RegisterAdaptor(domain);
        }

        /// <summary>
        /// 方法委托重定向及CLR绑定
        /// </summary>
        private void RegisterDelegate(ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
            //===============这里做一些ILRuntime的注册===============//
            
            //这里做一些ILRuntime的注册，这里我们注册值类型Binder，注释和解注下面的代码来对比性能差别
            _appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            _appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            _appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());

            //ILRuntime内部是用Action和Func这两个系统内置的委托类型来创建实例的，所以其他的委托类型都需要写转换器
            //将Action或者Func转换成目标委托类型
            //注册仅需要注册不同的参数搭配即可
            _appdomain.DelegateManager.RegisterMethodDelegate<int>();
            //带返回值的委托的话需要用RegisterFunctionDelegate，返回类型为最后一个
            _appdomain.DelegateManager.RegisterFunctionDelegate<int, string>();
            _appdomain.DelegateManager.RegisterMethodDelegate<string>();
            _appdomain.DelegateManager.RegisterMethodDelegate<DateTime>();
            _appdomain.DelegateManager.RegisterMethodDelegate<global::MsgArgs>();
            _appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            //通用性接口，防御一下
            _appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            _appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
            _appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Object>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });
            _appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Action<UnityEngine.U2D.SpriteAtlas>>();
            _appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.U2D.SpriteAtlas>();

            _appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
            _appdomain.DelegateManager.RegisterMethodDelegate<System.Object[]>();
            _appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
            {
                return new System.Comparison<System.Int32>((x, y) =>
                {
                    return ((Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
                });
            });
            _appdomain.DelegateManager.RegisterMethodDelegate<Assets.Scripts.StatisticManager>();
            _appdomain.DelegateManager.RegisterMethodDelegate<global::ADManager>();
            _appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>((arg0, arg1) =>
                {
                    ((Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>)act)(arg0, arg1);
                });
            });
            //对象池
            _appdomain.DelegateManager.RegisterFunctionDelegate<PathologicalGames.SortSpawnPool, System.Boolean>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<PathologicalGames.SortSpawnPool>>((act) =>
            {
                return new System.Predicate<PathologicalGames.SortSpawnPool>((obj) =>
                {
                    return ((Func<PathologicalGames.SortSpawnPool, System.Boolean>)act)(obj);
                });
            });
            _appdomain.DelegateManager.RegisterFunctionDelegate<global::PrefabPool, System.Boolean>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<global::PrefabPool>>((act) =>
            {
                return new System.Predicate<global::PrefabPool>((obj) =>
                {
                    return ((Func<global::PrefabPool, System.Boolean>)act)(obj);
                });
            });



            //UnityAction<float>
            _appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) => { return new UnityEngine.Events.UnityAction(() => { ((Action)act)(); }); });
            _appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<float>>((action) => { return new UnityEngine.Events.UnityAction<float>((a) => { ((System.Action<float>)action)(a); }); });

            #region FariyGUI delegate
#if FGUI

            _appdomain.DelegateManager.RegisterMethodDelegate<FairyGUI.EventContext>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.EventCallback0>((act) => { return new FairyGUI.EventCallback0(() => { ((Action)act)(); }); });
            _appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.EventCallback1>((act) => { return new FairyGUI.EventCallback1((_context) => { ((Action<FairyGUI.EventContext>)act)(_context); }); });
            _appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.ListItemRenderer>((act) =>
            {
                return new FairyGUI.ListItemRenderer((index, item) =>
                {
                    ((Action<System.Int32, FairyGUI.GObject>)act)(index, item);
                });
            });
            _appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.PlayCompleteCallback>((act) =>
            {
                return new FairyGUI.PlayCompleteCallback(() =>
                {
                    ((Action)act)();
                });
            });
            _appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.GTweenCallback>((act) =>
            {
                return new FairyGUI.GTweenCallback(() =>
                {
                    ((Action)act)();
                });
            });

            _appdomain.DelegateManager.RegisterMethodDelegate<Vector2>();
            _appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, FairyGUI.GObject>();

#endif
#endregion


            _appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Action<UnityEngine.U2D.SpriteAtlas>>();
            _appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.U2D.SpriteAtlas>();
            _appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            _appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
            _appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });
            _appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((Action)act)();
                });
            });
            _appdomain.DelegateManager.RegisterFunctionDelegate<Assets.Scripts.UI.BaseScrollElement>();

            SetupAddGetComponent();
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(_appdomain);

            //===============始终确保最后执行===============//
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(_appdomain);
        }

#region 静态外调方法

        //================静态外调方法===============//
        public static void SetConfigInstall(string className, string methodName, string e)
        {
            var t = _appdomain.LoadedTypes[className].ReflectionType;
            var method = t.GetMethod(methodName);

            var mainClass = _appdomain.LoadedTypes["Configs"].ReflectionType;
            var props = mainClass.GetField(className + "Dict");
            var finalType = method.Invoke(null, new object[1] { e });// Convert.ChangeType( method.Invoke( null, new object[ 1 ] { e } ), props.FieldType );
            props.SetValue(props, finalType);
        }

        public static void SetConfigInstall2()
        {
            var mainClass = _appdomain.LoadedTypes["Configs"].ReflectionType;
            var method = mainClass.GetMethod("Install");
            method.Invoke(null, null);
        }


        public static void SetSpawnPool(SpawnPool sp)
        {
            var mainClass = _appdomain.LoadedTypes["FrameworkSys.SpawnPoolReflection"].ReflectionType;
            var method = mainClass.GetMethod("SpawnReflection");
            method.Invoke(null, new object[] { sp });
        }


        public static IBaseUI GetUITypeByThis(string uiAssembly)
        {
            if (_appdomain.LoadedTypes.ContainsKey(uiAssembly))
            {
                LDebug.Log("Get UI From Hotfix..." + _appdomain.LoadedTypes[uiAssembly].BaseType.BaseType + "   " + _appdomain.LoadedTypes[uiAssembly].ReflectionType + "   ");
                //需要获取的是实例类
                var ss = _appdomain.Instantiate(_appdomain.LoadedTypes[uiAssembly].ReflectionType.FullName);
                return ss.CLRInstance as IBaseUI;
            }
            else return null;
        }

#endregion

#region AddComponent/GetComponent重定向方法
        unsafe void SetupAddGetComponent()
        {
            var arr = typeof(GameObject).GetMethods();
            foreach (var i in arr)
            {
                if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
                {
                    _appdomain.RegisterCLRMethodRedirection(i, AddComponentCustom);
                }
                if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
                {
                    _appdomain.RegisterCLRMethodRedirection(i, GetComponentCustom);
                }
            }
        }

        unsafe static StackObject* AddComponentCustom(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.AddComponent(type.TypeForCLR);
                }
                else
                {
                    //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                    var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                    //var contructor = ilInstance.Type.GetConstructor( ILRuntime.CLR.Utils.Extensions.EmptyParamList );
                    //__domain.Invoke( contructor, ilInstance, null );

                    //接下来创建Adapter实例
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    //var clrInstance = instance.AddComponent( ( ilInstance.Type.FirstCLRBaseType as CrossBindingAdaptor ).AdaptorType ) as MonoBehaviourAdapter.Adaptor;
                    //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    clrInstance.ILInstance = ilInstance;
                    clrInstance.AppDomain = __domain;
                    //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;

                    res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

                    clrInstance.Awake();//因为Unity调用这个方法时还没准备好所以这里补调一次
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        unsafe static StackObject* GetComponentCustom(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.GetComponent(type.TypeForCLR);
                }
                else
                {
                    //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                    var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for (int i = 0; i < clrInstances.Length; i++)
                    {
                        var clrInstance = clrInstances[i];
                        if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
                                break;
                            }
                        }
                    }
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }
#endregion

#region 业务逻辑

        /// <summary>
        /// 用于将Resource下的ScriptableObject
        /// </summary>
        public static void SetCommonData(UnityEngine.Object content)
        {
            //例如
            ////CommonData Scriptable
            //var mainClass = content.GetType();
            //var props = mainClass.GetFields();

            ////Local Container
            //var mainClassContainer = _appdomain.LoadedTypes["CommonDataInfo"].ReflectionType;
            //var propsContainer = mainClassContainer.GetFields();

            ////查找匹配
            //bool isMatch = false;
            //for (int i = 0; i < props.Length; i++)
            //{
            //    isMatch = false;
            //    FieldInfo fieldInfo = props[i];
            //    for (int x = 0; x < propsContainer.Length; x++)
            //    {
            //        if (propsContainer[x].Name == fieldInfo.Name)
            //        {
            //            propsContainer[x].SetValue(null, fieldInfo.GetValue(content));
            //            isMatch = true;
            //            break;
            //        }
            //    }
            //    if (!isMatch)
            //        throw new Exception(string.Format("CommonData字段  {0}  在承接类CommonDataInfo中未找到对应字段", fieldInfo.Name));
            //}
        }


#endregion
    }
}
