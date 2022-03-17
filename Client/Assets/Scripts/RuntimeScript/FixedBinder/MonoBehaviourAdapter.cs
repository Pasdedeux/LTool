using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;


public class MonoBehaviourAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof( MonoBehaviour );
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof( Adaptor );
        }
    }

    public override object CreateCLRInstance( ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance )
    {
        return new Adaptor( appdomain, instance );
    }
    //Ϊ������ʵ��MonoBehaviour���������ԣ����Adapter������չ������ֻ��ש����ֻʵ������õ�Awake, Start��Update
    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor( ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance )
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

        IMethod mAwakeMethod;
        bool mAwakeMethodGot;
        public void Awake()
        {
            //Unity����ILRuntime׼�������ʵ��ǰ����Awake������������ʱ�Ȳ�����
            if ( instance != null )
            {
                if ( !mAwakeMethodGot )
                {
                    mAwakeMethod = instance.Type.GetMethod( "Awake", 0 );
                    mAwakeMethodGot = true;

                    //MonoBehavior�Զ���ʼ��
                    var ss = instance.Type.GetConstructor( ILRuntime.CLR.Utils.Extensions.EmptyParamList );
                    appdomain.Invoke( ss, instance, null );
                }

                if ( mAwakeMethod != null )
                {
                    appdomain.Invoke( mAwakeMethod, instance, null );
                }
            }
        }

        IMethod mStartMethod;
        bool mStartMethodGot;
        void Start()
        {
            if ( !mStartMethodGot )
            {
                mStartMethod = instance.Type.GetMethod( "Start", 0 );
                mStartMethodGot = true;
            }

            if ( mStartMethod != null )
            {
                appdomain.Invoke( mStartMethod, instance, null );
            }
        }

        IMethod mUpdateMethod;
        bool mUpdateMethodGot;
        void Update()
        {
            if ( !mUpdateMethodGot )
            {
                mUpdateMethod = instance.Type.GetMethod( "Update", 0 );
                mUpdateMethodGot = true;
            }

            if ( mUpdateMethod != null )
            {
                appdomain.Invoke( mUpdateMethod, instance, null );
            }
        }

        IMethod mLateUpdateMethod;
        bool mLateUpdateMethodGot;
        void LateUpdate()
        {
            if (!mLateUpdateMethodGot)
            {
                mLateUpdateMethod = instance.Type.GetMethod("LateUpdate", 0);
                mLateUpdateMethodGot = true;
            }

            if (mLateUpdateMethod != null)
            {
                appdomain.Invoke(mLateUpdateMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod( "ToString", 0 );
            m = instance.Type.GetVirtualMethod( m );
            if ( m == null || m is ILMethod )
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }


}

