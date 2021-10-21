using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class WindowAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mLayoutModalWaitPane_0 = new CrossBindingMethodInfo("LayoutModalWaitPane");
        static CrossBindingMethodInfo mOnInit_1 = new CrossBindingMethodInfo("OnInit");
        static CrossBindingMethodInfo mOnShown_2 = new CrossBindingMethodInfo("OnShown");
        static CrossBindingMethodInfo mOnHide_3 = new CrossBindingMethodInfo("OnHide");
        static CrossBindingMethodInfo mDoShowAnimation_4 = new CrossBindingMethodInfo("DoShowAnimation");
        static CrossBindingMethodInfo mDoHideAnimation_5 = new CrossBindingMethodInfo("DoHideAnimation");
        static CrossBindingMethodInfo mDispose_6 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo<FairyGUI.EventContext> mcloseEventHandler_7 = new CrossBindingMethodInfo<FairyGUI.EventContext>("closeEventHandler");
        static CrossBindingFunctionInfo<FairyGUI.GObject, System.Int32, FairyGUI.GObject> mAddChildAt_8 = new CrossBindingFunctionInfo<FairyGUI.GObject, System.Int32, FairyGUI.GObject>("AddChildAt");
        static CrossBindingFunctionInfo<System.Int32, System.Boolean, FairyGUI.GObject> mRemoveChildAt_9 = new CrossBindingFunctionInfo<System.Int32, System.Boolean, FairyGUI.GObject>("RemoveChildAt");
        static CrossBindingFunctionInfo<System.Int32> mGetFirstChildInView_10 = new CrossBindingFunctionInfo<System.Int32>("GetFirstChildInView");
        static CrossBindingMethodInfo mHandleSizeChanged_11 = new CrossBindingMethodInfo("HandleSizeChanged");
        static CrossBindingMethodInfo mHandleGrayedChanged_12 = new CrossBindingMethodInfo("HandleGrayedChanged");
        static CrossBindingMethodInfo<FairyGUI.Controller> mHandleControllerChanged_13 = new CrossBindingMethodInfo<FairyGUI.Controller>("HandleControllerChanged");
        static CrossBindingMethodInfo mUpdateBounds_14 = new CrossBindingMethodInfo("UpdateBounds");
        class GetSnappingPositionWithDir_15Info : CrossBindingMethodInfo
        {
            static Type[] pTypes = new Type[] {typeof(System.Single).MakeByRefType(), typeof(System.Single).MakeByRefType(), typeof(System.Single), typeof(System.Single)};

            public GetSnappingPositionWithDir_15Info()
                : base("GetSnappingPositionWithDir")
            {

            }

            protected override Type ReturnType { get { return null; } }

            protected override Type[] Parameters { get { return pTypes; } }
            public void Invoke(ILTypeInstance instance, ref System.Single xValue, ref System.Single yValue, System.Single xDir, System.Single yDir)
            {
                EnsureMethod(instance);

                if (method != null)
                {
                    invoking = true;
                    try
                    {
                        using (var ctx = domain.BeginInvoke(method))
                        {
                            ctx.PushFloat(xValue);
                            ctx.PushFloat(yValue);
                            ctx.PushObject(instance);
                            ctx.PushReference(0);
                            ctx.PushReference(1);
                            ctx.PushFloat(xDir);
                            ctx.PushFloat(yDir);
                            ctx.Invoke();
                            xValue = ctx.ReadFloat(0);
                            yValue = ctx.ReadFloat(1);
                        }
                    }
                    finally
                    {
                        invoking = false;
                    }
                }
            }

            public override void Invoke(ILTypeInstance instance)
            {
                throw new NotSupportedException();
            }
        }
        static GetSnappingPositionWithDir_15Info mGetSnappingPositionWithDir_15 = new GetSnappingPositionWithDir_15Info();
        static CrossBindingMethodInfo mOnUpdate_16 = new CrossBindingMethodInfo("OnUpdate");
        static CrossBindingMethodInfo mConstructFromResource_17 = new CrossBindingMethodInfo("ConstructFromResource");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer> mConstructExtension_18 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer>("ConstructExtension");
        static CrossBindingMethodInfo<FairyGUI.Utils.XML> mConstructFromXML_19 = new CrossBindingMethodInfo<FairyGUI.Utils.XML>("ConstructFromXML");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_AfterAdd_20 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_AfterAdd");
        static CrossBindingFunctionInfo<System.Single> mget_alpha_21 = new CrossBindingFunctionInfo<System.Single>("get_alpha");
        static CrossBindingMethodInfo<System.Single> mset_alpha_22 = new CrossBindingMethodInfo<System.Single>("set_alpha");
        static CrossBindingFunctionInfo<FairyGUI.IFilter> mget_filter_23 = new CrossBindingFunctionInfo<FairyGUI.IFilter>("get_filter");
        static CrossBindingMethodInfo<FairyGUI.IFilter> mset_filter_24 = new CrossBindingMethodInfo<FairyGUI.IFilter>("set_filter");
        static CrossBindingFunctionInfo<FairyGUI.BlendMode> mget_blendMode_25 = new CrossBindingFunctionInfo<FairyGUI.BlendMode>("get_blendMode");
        static CrossBindingMethodInfo<FairyGUI.BlendMode> mset_blendMode_26 = new CrossBindingMethodInfo<FairyGUI.BlendMode>("set_blendMode");
        static CrossBindingFunctionInfo<System.String> mget_text_27 = new CrossBindingFunctionInfo<System.String>("get_text");
        static CrossBindingMethodInfo<System.String> mset_text_28 = new CrossBindingMethodInfo<System.String>("set_text");
        static CrossBindingFunctionInfo<System.String> mget_icon_29 = new CrossBindingFunctionInfo<System.String>("get_icon");
        static CrossBindingMethodInfo<System.String> mset_icon_30 = new CrossBindingMethodInfo<System.String>("set_icon");
        static CrossBindingMethodInfo mHandlePositionChanged_31 = new CrossBindingMethodInfo("HandlePositionChanged");
        static CrossBindingMethodInfo mHandleScaleChanged_32 = new CrossBindingMethodInfo("HandleScaleChanged");
        static CrossBindingMethodInfo mHandleAlphaChanged_33 = new CrossBindingMethodInfo("HandleAlphaChanged");
        static CrossBindingMethodInfo mHandleVisibleChanged_34 = new CrossBindingMethodInfo("HandleVisibleChanged");
        static CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32> mSetup_BeforeAdd_35 = new CrossBindingMethodInfo<FairyGUI.Utils.ByteBuffer, System.Int32>("Setup_BeforeAdd");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(FairyGUI.Window);
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

        public class Adapter : FairyGUI.Window, CrossBindingAdaptorType
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

            protected override void LayoutModalWaitPane()
            {
                if (mLayoutModalWaitPane_0.CheckShouldInvokeBase(this.instance))
                    base.LayoutModalWaitPane();
                else
                    mLayoutModalWaitPane_0.Invoke(this.instance);
            }

            protected override void OnInit()
            {
                if (mOnInit_1.CheckShouldInvokeBase(this.instance))
                    base.OnInit();
                else
                    mOnInit_1.Invoke(this.instance);
            }

            protected override void OnShown()
            {
                if (mOnShown_2.CheckShouldInvokeBase(this.instance))
                    base.OnShown();
                else
                    mOnShown_2.Invoke(this.instance);
            }

            protected override void OnHide()
            {
                if (mOnHide_3.CheckShouldInvokeBase(this.instance))
                    base.OnHide();
                else
                    mOnHide_3.Invoke(this.instance);
            }

            protected override void DoShowAnimation()
            {
                if (mDoShowAnimation_4.CheckShouldInvokeBase(this.instance))
                    base.DoShowAnimation();
                else
                    mDoShowAnimation_4.Invoke(this.instance);
            }

            protected override void DoHideAnimation()
            {
                if (mDoHideAnimation_5.CheckShouldInvokeBase(this.instance))
                    base.DoHideAnimation();
                else
                    mDoHideAnimation_5.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDispose_6.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_6.Invoke(this.instance);
            }

            protected override void closeEventHandler(FairyGUI.EventContext context)
            {
                if (mcloseEventHandler_7.CheckShouldInvokeBase(this.instance))
                    base.closeEventHandler(context);
                else
                    mcloseEventHandler_7.Invoke(this.instance, context);
            }

            public override FairyGUI.GObject AddChildAt(FairyGUI.GObject child, System.Int32 index)
            {
                if (mAddChildAt_8.CheckShouldInvokeBase(this.instance))
                    return base.AddChildAt(child, index);
                else
                    return mAddChildAt_8.Invoke(this.instance, child, index);
            }

            public override FairyGUI.GObject RemoveChildAt(System.Int32 index, System.Boolean dispose)
            {
                if (mRemoveChildAt_9.CheckShouldInvokeBase(this.instance))
                    return base.RemoveChildAt(index, dispose);
                else
                    return mRemoveChildAt_9.Invoke(this.instance, index, dispose);
            }

            public override System.Int32 GetFirstChildInView()
            {
                if (mGetFirstChildInView_10.CheckShouldInvokeBase(this.instance))
                    return base.GetFirstChildInView();
                else
                    return mGetFirstChildInView_10.Invoke(this.instance);
            }

            protected override void HandleSizeChanged()
            {
                if (mHandleSizeChanged_11.CheckShouldInvokeBase(this.instance))
                    base.HandleSizeChanged();
                else
                    mHandleSizeChanged_11.Invoke(this.instance);
            }

            protected override void HandleGrayedChanged()
            {
                if (mHandleGrayedChanged_12.CheckShouldInvokeBase(this.instance))
                    base.HandleGrayedChanged();
                else
                    mHandleGrayedChanged_12.Invoke(this.instance);
            }

            public override void HandleControllerChanged(FairyGUI.Controller c)
            {
                if (mHandleControllerChanged_13.CheckShouldInvokeBase(this.instance))
                    base.HandleControllerChanged(c);
                else
                    mHandleControllerChanged_13.Invoke(this.instance, c);
            }

            protected override void UpdateBounds()
            {
                if (mUpdateBounds_14.CheckShouldInvokeBase(this.instance))
                    base.UpdateBounds();
                else
                    mUpdateBounds_14.Invoke(this.instance);
            }

            public override void GetSnappingPositionWithDir(ref System.Single xValue, ref System.Single yValue, System.Single xDir, System.Single yDir)
            {
                if (mGetSnappingPositionWithDir_15.CheckShouldInvokeBase(this.instance))
                    base.GetSnappingPositionWithDir(ref xValue, ref yValue, xDir, yDir);
                else
                    mGetSnappingPositionWithDir_15.Invoke(this.instance, ref xValue, ref yValue, xDir, yDir);
            }

            protected override void OnUpdate()
            {
                if (mOnUpdate_16.CheckShouldInvokeBase(this.instance))
                    base.OnUpdate();
                else
                    mOnUpdate_16.Invoke(this.instance);
            }

            public override void ConstructFromResource()
            {
                if (mConstructFromResource_17.CheckShouldInvokeBase(this.instance))
                    base.ConstructFromResource();
                else
                    mConstructFromResource_17.Invoke(this.instance);
            }

            protected override void ConstructExtension(FairyGUI.Utils.ByteBuffer buffer)
            {
                if (mConstructExtension_18.CheckShouldInvokeBase(this.instance))
                    base.ConstructExtension(buffer);
                else
                    mConstructExtension_18.Invoke(this.instance, buffer);
            }

            public override void ConstructFromXML(FairyGUI.Utils.XML xml)
            {
                if (mConstructFromXML_19.CheckShouldInvokeBase(this.instance))
                    base.ConstructFromXML(xml);
                else
                    mConstructFromXML_19.Invoke(this.instance, xml);
            }

            public override void Setup_AfterAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_AfterAdd_20.CheckShouldInvokeBase(this.instance))
                    base.Setup_AfterAdd(buffer, beginPos);
                else
                    mSetup_AfterAdd_20.Invoke(this.instance, buffer, beginPos);
            }

            protected override void HandlePositionChanged()
            {
                if (mHandlePositionChanged_31.CheckShouldInvokeBase(this.instance))
                    base.HandlePositionChanged();
                else
                    mHandlePositionChanged_31.Invoke(this.instance);
            }

            protected override void HandleScaleChanged()
            {
                if (mHandleScaleChanged_32.CheckShouldInvokeBase(this.instance))
                    base.HandleScaleChanged();
                else
                    mHandleScaleChanged_32.Invoke(this.instance);
            }

            protected override void HandleAlphaChanged()
            {
                if (mHandleAlphaChanged_33.CheckShouldInvokeBase(this.instance))
                    base.HandleAlphaChanged();
                else
                    mHandleAlphaChanged_33.Invoke(this.instance);
            }

            public override void HandleVisibleChanged()
            {
                if (mHandleVisibleChanged_34.CheckShouldInvokeBase(this.instance))
                    base.HandleVisibleChanged();
                else
                    mHandleVisibleChanged_34.Invoke(this.instance);
            }

            public override void Setup_BeforeAdd(FairyGUI.Utils.ByteBuffer buffer, System.Int32 beginPos)
            {
                if (mSetup_BeforeAdd_35.CheckShouldInvokeBase(this.instance))
                    base.Setup_BeforeAdd(buffer, beginPos);
                else
                    mSetup_BeforeAdd_35.Invoke(this.instance, buffer, beginPos);
            }

            public override System.Single alpha
            {
            get
            {
                if (mget_alpha_21.CheckShouldInvokeBase(this.instance))
                    return base.alpha;
                else
                    return mget_alpha_21.Invoke(this.instance);

            }
            set
            {
                if (mset_alpha_22.CheckShouldInvokeBase(this.instance))
                    base.alpha = value;
                else
                    mset_alpha_22.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.IFilter filter
            {
            get
            {
                if (mget_filter_23.CheckShouldInvokeBase(this.instance))
                    return base.filter;
                else
                    return mget_filter_23.Invoke(this.instance);

            }
            set
            {
                if (mset_filter_24.CheckShouldInvokeBase(this.instance))
                    base.filter = value;
                else
                    mset_filter_24.Invoke(this.instance, value);

            }
            }

            public override FairyGUI.BlendMode blendMode
            {
            get
            {
                if (mget_blendMode_25.CheckShouldInvokeBase(this.instance))
                    return base.blendMode;
                else
                    return mget_blendMode_25.Invoke(this.instance);

            }
            set
            {
                if (mset_blendMode_26.CheckShouldInvokeBase(this.instance))
                    base.blendMode = value;
                else
                    mset_blendMode_26.Invoke(this.instance, value);

            }
            }

            public override System.String text
            {
            get
            {
                if (mget_text_27.CheckShouldInvokeBase(this.instance))
                    return base.text;
                else
                    return mget_text_27.Invoke(this.instance);

            }
            set
            {
                if (mset_text_28.CheckShouldInvokeBase(this.instance))
                    base.text = value;
                else
                    mset_text_28.Invoke(this.instance, value);

            }
            }

            public override System.String icon
            {
            get
            {
                if (mget_icon_29.CheckShouldInvokeBase(this.instance))
                    return base.icon;
                else
                    return mget_icon_29.Invoke(this.instance);

            }
            set
            {
                if (mset_icon_30.CheckShouldInvokeBase(this.instance))
                    base.icon = value;
                else
                    mset_icon_30.Invoke(this.instance, value);

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

