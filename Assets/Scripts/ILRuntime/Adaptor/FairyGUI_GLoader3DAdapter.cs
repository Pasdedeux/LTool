using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class GLoader3DAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mDispose_0 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingFunctionInfo<System.String> mget_icon_1 = new CrossBindingFunctionInfo<System.String>("get_icon");
        static CrossBindingMethodInfo<System.String> mset_icon_2 = new CrossBindingMethodInfo<System.String>("set_icon");
        static CrossBindingFunctionInfo<FairyGUI.IFilter> mget_filter_3 = new CrossBindingFunctionInfo<FairyGUI.IFilter>("get_filter");
        static CrossBindingMethodInfo<FairyGUI.IFilter> mset_filter_4 = new CrossBindingMethodInfo<FairyGUI.IFilter>("set_filter");
        static CrossBindingFunctionInfo<FairyGUI.BlendMode> mget_blendMode_5 = new CrossBindingFunctionInfo<FairyGUI.BlendMode>("get_blendMode");
        static CrossBindingMethodInfo<FairyGUI.BlendMode> mset_blendMode_6 = new CrossBindingMethodInfo<FairyGUI.BlendMode>("set_blendMode");
        static CrossBindingMethodInfo<System.String> mOnChange_7 = new CrossBindingMethodInfo<System.String>("OnChange");
        static CrossBindingMethodInfo mLoadExternal_8 = new CrossBindingMethodInfo("LoadExternal");
        static CrossBindingFunctionInfo<System.Single> mget_alpha_9 = new CrossBindingFunctionInfo<System.Single>("get_alpha");
        static CrossBindingMethodInfo<System.Single> mset_alpha_10 = new CrossBindingMethodInfo<System.Single>("set_alpha");
        static CrossBindingMethodInfo mFreeExternal_11 = new CrossBindingMethodInfo("FreeExternal");
        static CrossBindingMethodInfo mHandleSizeChanged_12 = new CrossBindingMethodInfo("HandleSizeChanged");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_BeforeAdd_13 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_BeforeAdd");
        static CrossBindingMethodInfo<FairyGUI.Controller> mHandleControllerChanged_14 = new CrossBindingMethodInfo<FairyGUI.Controller>("HandleControllerChanged");
        static CrossBindingFunctionInfo<System.String> mget_text_15 = new CrossBindingFunctionInfo<System.String>("get_text");
        static CrossBindingMethodInfo<System.String> mset_text_16 = new CrossBindingMethodInfo<System.String>("set_text");
        static CrossBindingMethodInfo mHandlePositionChanged_17 = new CrossBindingMethodInfo("HandlePositionChanged");
        static CrossBindingMethodInfo mHandleScaleChanged_18 = new CrossBindingMethodInfo("HandleScaleChanged");
        static CrossBindingMethodInfo mHandleGrayedChanged_19 = new CrossBindingMethodInfo("HandleGrayedChanged");
        static CrossBindingMethodInfo mHandleAlphaChanged_20 = new CrossBindingMethodInfo("HandleAlphaChanged");
        static CrossBindingMethodInfo mHandleVisibleChanged_21 = new CrossBindingMethodInfo("HandleVisibleChanged");
        static CrossBindingMethodInfo mConstructFromResource_22 = new CrossBindingMethodInfo("ConstructFromResource");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_AfterAdd_23 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_AfterAdd");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FairyGUI.GLoader3D);
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

        public class Adapter : FairyGUI.GLoader3D, CrossBindingAdaptorType
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

            public override void Dispose()
            {
                if (mDispose_0.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_0.Invoke(this.instance);
            }

            protected override void OnChange(System.String propertyName)
            {
                if (mOnChange_7.CheckShouldInvokeBase(this.instance))
                    base.OnChange(propertyName);
                else
                    mOnChange_7.Invoke(this.instance, propertyName);
            }

            protected override void LoadExternal()
            {
                if (mLoadExternal_8.CheckShouldInvokeBase(this.instance))
                    base.LoadExternal();
                else
                    mLoadExternal_8.Invoke(this.instance);
            }

            protected override void FreeExternal()
            {
                if (mFreeExternal_11.CheckShouldInvokeBase(this.instance))
                    base.FreeExternal();
                else
                    mFreeExternal_11.Invoke(this.instance);
            }

            protected override void HandleSizeChanged()
            {
                if (mHandleSizeChanged_12.CheckShouldInvokeBase(this.instance))
                    base.HandleSizeChanged();
                else
                    mHandleSizeChanged_12.Invoke(this.instance);
            }

            public override void Setup_BeforeAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_BeforeAdd_13.CheckShouldInvokeBase(this.instance))
                    base.Setup_BeforeAdd(buffer, beginPos);
                else
                    mSetup_BeforeAdd_13.Invoke(this.instance, buffer, beginPos);
            }

            public override void HandleControllerChanged(FairyGUI.Controller c)
            {
                if (mHandleControllerChanged_14.CheckShouldInvokeBase(this.instance))
                    base.HandleControllerChanged(c);
                else
                    mHandleControllerChanged_14.Invoke(this.instance, c);
            }

            protected override void HandlePositionChanged()
            {
                if (mHandlePositionChanged_17.CheckShouldInvokeBase(this.instance))
                    base.HandlePositionChanged();
                else
                    mHandlePositionChanged_17.Invoke(this.instance);
            }

            protected override void HandleScaleChanged()
            {
                if (mHandleScaleChanged_18.CheckShouldInvokeBase(this.instance))
                    base.HandleScaleChanged();
                else
                    mHandleScaleChanged_18.Invoke(this.instance);
            }

            protected override void HandleGrayedChanged()
            {
                if (mHandleGrayedChanged_19.CheckShouldInvokeBase(this.instance))
                    base.HandleGrayedChanged();
                else
                    mHandleGrayedChanged_19.Invoke(this.instance);
            }

            protected override void HandleAlphaChanged()
            {
                if (mHandleAlphaChanged_20.CheckShouldInvokeBase(this.instance))
                    base.HandleAlphaChanged();
                else
                    mHandleAlphaChanged_20.Invoke(this.instance);
            }

            public override void HandleVisibleChanged()
            {
                if (mHandleVisibleChanged_21.CheckShouldInvokeBase(this.instance))
                    base.HandleVisibleChanged();
                else
                    mHandleVisibleChanged_21.Invoke(this.instance);
            }

            public override void ConstructFromResource()
            {
                if (mConstructFromResource_22.CheckShouldInvokeBase(this.instance))
                    base.ConstructFromResource();
                else
                    mConstructFromResource_22.Invoke(this.instance);
            }

            public override void Setup_AfterAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_AfterAdd_23.CheckShouldInvokeBase(this.instance))
                    base.Setup_AfterAdd(buffer, beginPos);
                else
                    mSetup_AfterAdd_23.Invoke(this.instance, buffer, beginPos);
            }

            public override System.String icon
            {
            get
            {
                if (mget_icon_1.CheckShouldInvokeBase(this.instance))
                    return base.icon;
                else
                    return mget_icon_1.Invoke(this.instance);

            }
            set
            {
                if (mset_icon_2.CheckShouldInvokeBase(this.instance))
                    base.icon = value;
                else
                    mset_icon_2.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.IFilter filter
            {
            get
            {
                if (mget_filter_3.CheckShouldInvokeBase(this.instance))
                    return base.filter;
                else
                    return mget_filter_3.Invoke(this.instance);

            }
            set
            {
                if (mset_filter_4.CheckShouldInvokeBase(this.instance))
                    base.filter = value;
                else
                    mset_filter_4.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.BlendMode blendMode
            {
            get
            {
                if (mget_blendMode_5.CheckShouldInvokeBase(this.instance))
                    return base.blendMode;
                else
                    return mget_blendMode_5.Invoke(this.instance);

            }
            set
            {
                if (mset_blendMode_6.CheckShouldInvokeBase(this.instance))
                    base.blendMode = value;
                else
                    mset_blendMode_6.Invoke(this.instance, value);

            }
            }

            public override System.Single alpha
            {
            get
            {
                if (mget_alpha_9.CheckShouldInvokeBase(this.instance))
                    return base.alpha;
                else
                    return mget_alpha_9.Invoke(this.instance);

            }
            set
            {
                if (mset_alpha_10.CheckShouldInvokeBase(this.instance))
                    base.alpha = value;
                else
                    mset_alpha_10.Invoke(this.instance, value);

            }
            }

            public override System.String text
            {
            get
            {
                if (mget_text_15.CheckShouldInvokeBase(this.instance))
                    return base.text;
                else
                    return mget_text_15.Invoke(this.instance);

            }
            set
            {
                if (mset_text_16.CheckShouldInvokeBase(this.instance))
                    base.text = value;
                else
                    mset_text_16.Invoke(this.instance, value);

            }
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

