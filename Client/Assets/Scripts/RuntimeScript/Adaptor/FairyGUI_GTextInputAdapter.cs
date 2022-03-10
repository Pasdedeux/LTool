using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class GTextInputAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mSetTextFieldText_0 = new CrossBindingMethodInfo("SetTextFieldText");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_BeforeAdd_1 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_BeforeAdd");
        static CrossBindingFunctionInfo<System.String> mget_text_2 = new CrossBindingFunctionInfo<System.String>("get_text");
        static CrossBindingMethodInfo<System.String> mset_text_3 = new CrossBindingMethodInfo<System.String>("set_text");
        static CrossBindingMethodInfo mHandleSizeChanged_4 = new CrossBindingMethodInfo("HandleSizeChanged");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_AfterAdd_5 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_AfterAdd");
        static CrossBindingFunctionInfo<System.Single> mget_alpha_6 = new CrossBindingFunctionInfo<System.Single>("get_alpha");
        static CrossBindingMethodInfo<System.Single> mset_alpha_7 = new CrossBindingMethodInfo<System.Single>("set_alpha");
        static CrossBindingFunctionInfo<FairyGUI.IFilter> mget_filter_8 = new CrossBindingFunctionInfo<FairyGUI.IFilter>("get_filter");
        static CrossBindingMethodInfo<FairyGUI.IFilter> mset_filter_9 = new CrossBindingMethodInfo<FairyGUI.IFilter>("set_filter");
        static CrossBindingFunctionInfo<FairyGUI.BlendMode> mget_blendMode_10 = new CrossBindingFunctionInfo<FairyGUI.BlendMode>("get_blendMode");
        static CrossBindingMethodInfo<FairyGUI.BlendMode> mset_blendMode_11 = new CrossBindingMethodInfo<FairyGUI.BlendMode>("set_blendMode");
        static CrossBindingMethodInfo<FairyGUI.Controller> mHandleControllerChanged_12 = new CrossBindingMethodInfo<FairyGUI.Controller>("HandleControllerChanged");
        static CrossBindingFunctionInfo<System.String> mget_icon_13 = new CrossBindingFunctionInfo<System.String>("get_icon");
        static CrossBindingMethodInfo<System.String> mset_icon_14 = new CrossBindingMethodInfo<System.String>("set_icon");
        static CrossBindingMethodInfo mDispose_15 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo mHandlePositionChanged_16 = new CrossBindingMethodInfo("HandlePositionChanged");
        static CrossBindingMethodInfo mHandleScaleChanged_17 = new CrossBindingMethodInfo("HandleScaleChanged");
        static CrossBindingMethodInfo mHandleGrayedChanged_18 = new CrossBindingMethodInfo("HandleGrayedChanged");
        static CrossBindingMethodInfo mHandleAlphaChanged_19 = new CrossBindingMethodInfo("HandleAlphaChanged");
        static CrossBindingMethodInfo mHandleVisibleChanged_20 = new CrossBindingMethodInfo("HandleVisibleChanged");
        static CrossBindingMethodInfo mConstructFromResource_21 = new CrossBindingMethodInfo("ConstructFromResource");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FairyGUI.GTextInput);
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

        public class Adapter : FairyGUI.GTextInput, CrossBindingAdaptorType
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

            protected override void SetTextFieldText()
            {
                if (mSetTextFieldText_0.CheckShouldInvokeBase(this.instance))
                    base.SetTextFieldText();
                else
                    mSetTextFieldText_0.Invoke(this.instance);
            }

            public override void Setup_BeforeAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_BeforeAdd_1.CheckShouldInvokeBase(this.instance))
                    base.Setup_BeforeAdd(buffer, beginPos);
                else
                    mSetup_BeforeAdd_1.Invoke(this.instance, buffer, beginPos);
            }

            protected override void HandleSizeChanged()
            {
                if (mHandleSizeChanged_4.CheckShouldInvokeBase(this.instance))
                    base.HandleSizeChanged();
                else
                    mHandleSizeChanged_4.Invoke(this.instance);
            }

            public override void Setup_AfterAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_AfterAdd_5.CheckShouldInvokeBase(this.instance))
                    base.Setup_AfterAdd(buffer, beginPos);
                else
                    mSetup_AfterAdd_5.Invoke(this.instance, buffer, beginPos);
            }

            public override void HandleControllerChanged(FairyGUI.Controller c)
            {
                if (mHandleControllerChanged_12.CheckShouldInvokeBase(this.instance))
                    base.HandleControllerChanged(c);
                else
                    mHandleControllerChanged_12.Invoke(this.instance, c);
            }

            public override void Dispose()
            {
                if (mDispose_15.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_15.Invoke(this.instance);
            }

            protected override void HandlePositionChanged()
            {
                if (mHandlePositionChanged_16.CheckShouldInvokeBase(this.instance))
                    base.HandlePositionChanged();
                else
                    mHandlePositionChanged_16.Invoke(this.instance);
            }

            protected override void HandleScaleChanged()
            {
                if (mHandleScaleChanged_17.CheckShouldInvokeBase(this.instance))
                    base.HandleScaleChanged();
                else
                    mHandleScaleChanged_17.Invoke(this.instance);
            }

            protected override void HandleGrayedChanged()
            {
                if (mHandleGrayedChanged_18.CheckShouldInvokeBase(this.instance))
                    base.HandleGrayedChanged();
                else
                    mHandleGrayedChanged_18.Invoke(this.instance);
            }

            protected override void HandleAlphaChanged()
            {
                if (mHandleAlphaChanged_19.CheckShouldInvokeBase(this.instance))
                    base.HandleAlphaChanged();
                else
                    mHandleAlphaChanged_19.Invoke(this.instance);
            }

            public override void HandleVisibleChanged()
            {
                if (mHandleVisibleChanged_20.CheckShouldInvokeBase(this.instance))
                    base.HandleVisibleChanged();
                else
                    mHandleVisibleChanged_20.Invoke(this.instance);
            }

            public override void ConstructFromResource()
            {
                if (mConstructFromResource_21.CheckShouldInvokeBase(this.instance))
                    base.ConstructFromResource();
                else
                    mConstructFromResource_21.Invoke(this.instance);
            }

            public override System.String text
            {
            get
            {
                if (mget_text_2.CheckShouldInvokeBase(this.instance))
                    return base.text;
                else
                    return mget_text_2.Invoke(this.instance);

            }
            set
            {
                if (mset_text_3.CheckShouldInvokeBase(this.instance))
                    base.text = value;
                else
                    mset_text_3.Invoke(this.instance, value);

            }
            }

            public override System.Single alpha
            {
            get
            {
                if (mget_alpha_6.CheckShouldInvokeBase(this.instance))
                    return base.alpha;
                else
                    return mget_alpha_6.Invoke(this.instance);

            }
            set
            {
                if (mset_alpha_7.CheckShouldInvokeBase(this.instance))
                    base.alpha = value;
                else
                    mset_alpha_7.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.IFilter filter
            {
            get
            {
                if (mget_filter_8.CheckShouldInvokeBase(this.instance))
                    return base.filter;
                else
                    return mget_filter_8.Invoke(this.instance);

            }
            set
            {
                if (mset_filter_9.CheckShouldInvokeBase(this.instance))
                    base.filter = value;
                else
                    mset_filter_9.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.BlendMode blendMode
            {
            get
            {
                if (mget_blendMode_10.CheckShouldInvokeBase(this.instance))
                    return base.blendMode;
                else
                    return mget_blendMode_10.Invoke(this.instance);

            }
            set
            {
                if (mset_blendMode_11.CheckShouldInvokeBase(this.instance))
                    base.blendMode = value;
                else
                    mset_blendMode_11.Invoke(this.instance, value);

            }
            }

            public override System.String icon
            {
            get
            {
                if (mget_icon_13.CheckShouldInvokeBase(this.instance))
                    return base.icon;
                else
                    return mget_icon_13.Invoke(this.instance);

            }
            set
            {
                if (mset_icon_14.CheckShouldInvokeBase(this.instance))
                    base.icon = value;
                else
                    mset_icon_14.Invoke(this.instance, value);

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

