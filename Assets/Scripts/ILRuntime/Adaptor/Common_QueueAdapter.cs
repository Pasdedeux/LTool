using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace LHotfixProject
{   
    public class QueueAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32> mget_Count_0 = new CrossBindingFunctionInfo<System.Int32>("get_Count");
        static CrossBindingFunctionInfo<System.Object> mClone_1 = new CrossBindingFunctionInfo<System.Object>("Clone");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsSynchronized_2 = new CrossBindingFunctionInfo<System.Boolean>("get_IsSynchronized");
        static CrossBindingFunctionInfo<System.Object> mget_SyncRoot_3 = new CrossBindingFunctionInfo<System.Object>("get_SyncRoot");
        static CrossBindingMethodInfo mClear_4 = new CrossBindingMethodInfo("Clear");
        static CrossBindingMethodInfo<System.Array, System.Int32> mCopyTo_5 = new CrossBindingMethodInfo<System.Array, System.Int32>("CopyTo");
        static CrossBindingMethodInfo<System.Object> mEnqueue_6 = new CrossBindingMethodInfo<System.Object>("Enqueue");
        static CrossBindingFunctionInfo<System.Collections.IEnumerator> mGetEnumerator_7 = new CrossBindingFunctionInfo<System.Collections.IEnumerator>("GetEnumerator");
        static CrossBindingFunctionInfo<System.Object> mDequeue_8 = new CrossBindingFunctionInfo<System.Object>("Dequeue");
        static CrossBindingFunctionInfo<System.Object> mPeek_9 = new CrossBindingFunctionInfo<System.Object>("Peek");
        static CrossBindingFunctionInfo<System.Object, System.Boolean> mContains_10 = new CrossBindingFunctionInfo<System.Object, System.Boolean>("Contains");
        static CrossBindingFunctionInfo<System.Object[]> mToArray_11 = new CrossBindingFunctionInfo<System.Object[]>("ToArray");
        static CrossBindingMethodInfo mTrimToSize_12 = new CrossBindingMethodInfo("TrimToSize");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Queue);
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

        public class Adapter : System.Collections.Queue, CrossBindingAdaptorType
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

            public override System.Object Clone()
            {
                if (mClone_1.CheckShouldInvokeBase(this.instance))
                    return base.Clone();
                else
                    return mClone_1.Invoke(this.instance);
            }

            public override void Clear()
            {
                if (mClear_4.CheckShouldInvokeBase(this.instance))
                    base.Clear();
                else
                    mClear_4.Invoke(this.instance);
            }

            public override void CopyTo(System.Array array, System.Int32 index)
            {
                if (mCopyTo_5.CheckShouldInvokeBase(this.instance))
                    base.CopyTo(array, index);
                else
                    mCopyTo_5.Invoke(this.instance, array, index);
            }

            public override void Enqueue(System.Object obj)
            {
                if (mEnqueue_6.CheckShouldInvokeBase(this.instance))
                    base.Enqueue(obj);
                else
                    mEnqueue_6.Invoke(this.instance, obj);
            }

            public override System.Collections.IEnumerator GetEnumerator()
            {
                if (mGetEnumerator_7.CheckShouldInvokeBase(this.instance))
                    return base.GetEnumerator();
                else
                    return mGetEnumerator_7.Invoke(this.instance);
            }

            public override System.Object Dequeue()
            {
                if (mDequeue_8.CheckShouldInvokeBase(this.instance))
                    return base.Dequeue();
                else
                    return mDequeue_8.Invoke(this.instance);
            }

            public override System.Object Peek()
            {
                if (mPeek_9.CheckShouldInvokeBase(this.instance))
                    return base.Peek();
                else
                    return mPeek_9.Invoke(this.instance);
            }

            public override System.Boolean Contains(System.Object obj)
            {
                if (mContains_10.CheckShouldInvokeBase(this.instance))
                    return base.Contains(obj);
                else
                    return mContains_10.Invoke(this.instance, obj);
            }

            public override System.Object[] ToArray()
            {
                if (mToArray_11.CheckShouldInvokeBase(this.instance))
                    return base.ToArray();
                else
                    return mToArray_11.Invoke(this.instance);
            }

            public override void TrimToSize()
            {
                if (mTrimToSize_12.CheckShouldInvokeBase(this.instance))
                    base.TrimToSize();
                else
                    mTrimToSize_12.Invoke(this.instance);
            }

            public override System.Int32 Count
            {
            get
            {
                if (mget_Count_0.CheckShouldInvokeBase(this.instance))
                    return base.Count;
                else
                    return mget_Count_0.Invoke(this.instance);

            }
            }

            public override System.Boolean IsSynchronized
            {
            get
            {
                if (mget_IsSynchronized_2.CheckShouldInvokeBase(this.instance))
                    return base.IsSynchronized;
                else
                    return mget_IsSynchronized_2.Invoke(this.instance);

            }
            }

            public override System.Object SyncRoot
            {
            get
            {
                if (mget_SyncRoot_3.CheckShouldInvokeBase(this.instance))
                    return base.SyncRoot;
                else
                    return mget_SyncRoot_3.Invoke(this.instance);

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

