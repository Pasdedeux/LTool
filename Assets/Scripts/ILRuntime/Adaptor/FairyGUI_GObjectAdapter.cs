using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class GObjectAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Single> mget_alpha_0 = new CrossBindingFunctionInfo<System.Single>("get_alpha");
        static CrossBindingMethodInfo<System.Single> mset_alpha_1 = new CrossBindingMethodInfo<System.Single>("set_alpha");
        static CrossBindingFunctionInfo<FairyGUI.IFilter> mget_filter_2 = new CrossBindingFunctionInfo<FairyGUI.IFilter>("get_filter");
        static CrossBindingMethodInfo<FairyGUI.IFilter> mset_filter_3 = new CrossBindingMethodInfo<FairyGUI.IFilter>("set_filter");
        static CrossBindingFunctionInfo<FairyGUI.BlendMode> mget_blendMode_4 = new CrossBindingFunctionInfo<FairyGUI.BlendMode>("get_blendMode");
        static CrossBindingMethodInfo<FairyGUI.BlendMode> mset_blendMode_5 = new CrossBindingMethodInfo<FairyGUI.BlendMode>("set_blendMode");
        static CrossBindingMethodInfo<FairyGUI.Controller> mHandleControllerChanged_6 = new CrossBindingMethodInfo<FairyGUI.Controller>("HandleControllerChanged");
        static CrossBindingFunctionInfo<System.String> mget_text_7 = new CrossBindingFunctionInfo<System.String>("get_text");
        static CrossBindingMethodInfo<System.String> mset_text_8 = new CrossBindingMethodInfo<System.String>("set_text");
        static CrossBindingFunctionInfo<System.String> mget_icon_9 = new CrossBindingFunctionInfo<System.String>("get_icon");
        static CrossBindingMethodInfo<System.String> mset_icon_10 = new CrossBindingMethodInfo<System.String>("set_icon");
        static CrossBindingMethodInfo mDispose_11 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo mHandlePositionChanged_12 = new CrossBindingMethodInfo("HandlePositionChanged");
        static CrossBindingMethodInfo mHandleSizeChanged_13 = new CrossBindingMethodInfo("HandleSizeChanged");
        static CrossBindingMethodInfo mHandleScaleChanged_14 = new CrossBindingMethodInfo("HandleScaleChanged");
        static CrossBindingMethodInfo mHandleGrayedChanged_15 = new CrossBindingMethodInfo("HandleGrayedChanged");
        static CrossBindingMethodInfo mHandleAlphaChanged_16 = new CrossBindingMethodInfo("HandleAlphaChanged");
        static CrossBindingMethodInfo mHandleVisibleChanged_17 = new CrossBindingMethodInfo("HandleVisibleChanged");
        static CrossBindingMethodInfo mConstructFromResource_18 = new CrossBindingMethodInfo("ConstructFromResource");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_BeforeAdd_19 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_BeforeAdd");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_AfterAdd_20 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_AfterAdd");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FairyGUI.GObject);
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

        public class Adapter : FairyGUI.GObject, CrossBindingAdaptorType
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

            public override void HandleControllerChanged(FairyGUI.Controller c)
            {
                if (mHandleControllerChanged_6.CheckShouldInvokeBase(this.instance))
                    base.HandleControllerChanged(c);
                else
                    mHandleControllerChanged_6.Invoke(this.instance, c);
            }

            public override void Dispose()
            {
                if (mDispose_11.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_11.Invoke(this.instance);
            }

            protected override void HandlePositionChanged()
            {
                if (mHandlePositionChanged_12.CheckShouldInvokeBase(this.instance))
                    base.HandlePositionChanged();
                else
                    mHandlePositionChanged_12.Invoke(this.instance);
            }

            protected override void HandleSizeChanged()
            {
                if (mHandleSizeChanged_13.CheckShouldInvokeBase(this.instance))
                    base.HandleSizeChanged();
                else
                    mHandleSizeChanged_13.Invoke(this.instance);
            }

            protected override void HandleScaleChanged()
            {
                if (mHandleScaleChanged_14.CheckShouldInvokeBase(this.instance))
                    base.HandleScaleChanged();
                else
                    mHandleScaleChanged_14.Invoke(this.instance);
            }

            protected override void HandleGrayedChanged()
            {
                if (mHandleGrayedChanged_15.CheckShouldInvokeBase(this.instance))
                    base.HandleGrayedChanged();
                else
                    mHandleGrayedChanged_15.Invoke(this.instance);
            }

            protected override void HandleAlphaChanged()
            {
                if (mHandleAlphaChanged_16.CheckShouldInvokeBase(this.instance))
                    base.HandleAlphaChanged();
                else
                    mHandleAlphaChanged_16.Invoke(this.instance);
            }

            public override void HandleVisibleChanged()
            {
                if (mHandleVisibleChanged_17.CheckShouldInvokeBase(this.instance))
                    base.HandleVisibleChanged();
                else
                    mHandleVisibleChanged_17.Invoke(this.instance);
            }

            public override void ConstructFromResource()
            {
                if (mConstructFromResource_18.CheckShouldInvokeBase(this.instance))
                    base.ConstructFromResource();
                else
                    mConstructFromResource_18.Invoke(this.instance);
            }

            public override void Setup_BeforeAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_BeforeAdd_19.CheckShouldInvokeBase(this.instance))
                    base.Setup_BeforeAdd(buffer, beginPos);
                else
                    mSetup_BeforeAdd_19.Invoke(this.instance, buffer, beginPos);
            }

            public override void Setup_AfterAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_AfterAdd_20.CheckShouldInvokeBase(this.instance))
                    base.Setup_AfterAdd(buffer, beginPos);
                else
                    mSetup_AfterAdd_20.Invoke(this.instance, buffer, beginPos);
            }

            public override System.Single alpha
            {
            get
            {
                if (mget_alpha_0.CheckShouldInvokeBase(this.instance))
                    return base.alpha;
                else
                    return mget_alpha_0.Invoke(this.instance);

            }
            set
            {
                if (mset_alpha_1.CheckShouldInvokeBase(this.instance))
                    base.alpha = value;
                else
                    mset_alpha_1.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.IFilter filter
            {
            get
            {
                if (mget_filter_2.CheckShouldInvokeBase(this.instance))
                    return base.filter;
                else
                    return mget_filter_2.Invoke(this.instance);

            }
            set
            {
                if (mset_filter_3.CheckShouldInvokeBase(this.instance))
                    base.filter = value;
                else
                    mset_filter_3.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.BlendMode blendMode
            {
            get
            {
                if (mget_blendMode_4.CheckShouldInvokeBase(this.instance))
                    return base.blendMode;
                else
                    return mget_blendMode_4.Invoke(this.instance);

            }
            set
            {
                if (mset_blendMode_5.CheckShouldInvokeBase(this.instance))
                    base.blendMode = value;
                else
                    mset_blendMode_5.Invoke(this.instance, value);

            }
            }

            public override System.String text
            {
            get
            {
                if (mget_text_7.CheckShouldInvokeBase(this.instance))
                    return base.text;
                else
                    return mget_text_7.Invoke(this.instance);

            }
            set
            {
                if (mset_text_8.CheckShouldInvokeBase(this.instance))
                    base.text = value;
                else
                    mset_text_8.Invoke(this.instance, value);

            }
            }

            public override System.String icon
            {
            get
            {
                if (mget_icon_9.CheckShouldInvokeBase(this.instance))
                    return base.icon;
                else
                    return mget_icon_9.Invoke(this.instance);

            }
            set
            {
                if (mset_icon_10.CheckShouldInvokeBase(this.instance))
                    base.icon = value;
                else
                    mset_icon_10.Invoke(this.instance, value);

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

