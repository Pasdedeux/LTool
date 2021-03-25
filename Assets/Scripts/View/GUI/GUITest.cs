using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GUITest : MonoBehaviour
{

    private Action UpdateEventHandler;
#if UNITY_EDITOR
    private void OnGUI()
    {
        int index = 0;

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "注册事件" ) )
        {
            MsgManager.Instance.Register( EventType1.EventAr, CallBack );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "去注册事件" ) )
        {
            MsgManager.Instance.UnRegister( EventType1.EventAr, CallBack );
        }

        if ( GUI.Button( new Rect( 10 + 110 * index++, 100, 100, 100 ), "广播事件" ) )
        {
            MsgManager.Instance.Broadcast( EventType1.EventAr, new MsgArgs( "ceshi1", 1 ) );
        }
        
    }

    private void CallBack( MsgArgs obj )
    {
        LDebug.Log( "====>MsgArgs" );
    }




#endif
}


/// <summary>
/// 事件管理器
/// 
/// id内部采用ushort类型，参数采用枚举类型
/// </summary>
public class MsgManager : Singleton<MsgManager>
{
    private Dictionary<ushort, Action<MsgArgs>> _internalMsgDict;

    public MsgManager()
    {
        _internalMsgDict = new Dictionary<ushort, Action<MsgArgs>>();
    }

    #region 枚举方案

    //：有些许GC
    /// <summary>
    /// 注册事件关注
    /// </summary>
    /// <typeparam name="T">统用枚举类。可接受多种枚举。这里会转换为ushort类型。<em>通用枚举的转换，采用的是IConvertible.ToUInt16。中间借用了format涉及到了拆装箱，ID转换将会固定产生40B</em> </typeparam>
    /// <param name="id">Event ID</param>
    /// <param name="callBack">事件回调</param>
    public void Register<T>( T id, Action<MsgArgs> callBack ) where T : Enum, IComparable, IConvertible, IFormattable
    {
        ushort idCode = id.ToUInt16( null );
        if ( !_internalMsgDict.ContainsKey( idCode ) )
            _internalMsgDict.Add( idCode, null );
        _internalMsgDict[ idCode ] += callBack;
    }

    /// <summary>
    /// 取消事件关注
    /// </summary>
    /// <typeparam name="T">统用枚举类。可接受多种枚举。这里会转换为ushort类型。<em>通用枚举的转换，采用的是IConvertible.ToUInt16。中间借用了format涉及到了拆装箱，ID转换将会固定产生40B</em></typeparam>
    /// <param name="id">Event ID</param>
    /// <param name="callBack">事件回调</param>
    public void UnRegister<T>( T id, Action<MsgArgs> callBack ) where T : IComparable, IConvertible, IFormattable
    {
        ushort idCode = id.ToUInt16( null );
        if ( _internalMsgDict.ContainsKey( idCode ) )
        {
            _internalMsgDict[ idCode ] -= callBack;
            if ( _internalMsgDict[ idCode ] == null ) _internalMsgDict.Remove( idCode );
        }
    }

    /// <summary>
    /// 广播事件
    /// </summary>
    /// <typeparam name="T">统用枚举类。可接受多种枚举。这里会转换为ushort类型。<em>通用枚举的转换，采用的是IConvertible.ToUInt16。中间借用了format涉及到了拆装箱，ID转换将会固定产生40B</em></typeparam>
    /// <param name="id">Event ID</param>
    /// <param name="msg">事件参数</param>
    public void Broadcast<T>( T id, MsgArgs msg = null ) where T : IComparable, IConvertible, IFormattable
    {
        ushort idCode = id.ToUInt16( null );
        if ( !_internalMsgDict.ContainsKey( idCode ) ) throw new Exception( "事件未注册：" + id.ToString() );
        _internalMsgDict[ idCode ].Invoke( msg );
    }

    #endregion

    #region ID num方案
    /// <summary>
    /// 注册事件关注
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="callBack">事件回调</param>
    public void Register( ushort id, Action<MsgArgs> callBack )
    {
        if ( !_internalMsgDict.ContainsKey( id ) )
            _internalMsgDict.Add( id, null );
        _internalMsgDict[ id ] += callBack;
    }

    /// <summary>
    /// 取消事件关注
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="callBack">事件回调</param>
    public void UnRegister( ushort id, Action<MsgArgs> callBack )
    {
        if ( _internalMsgDict.ContainsKey( id ) )
        {
            _internalMsgDict[ id ] -= callBack;
            if ( _internalMsgDict[ id ] == null ) _internalMsgDict.Remove( id );
        }
    }

    /// <summary>
    /// 广播事件
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="msg">事件参数</param>
    public void Broadcast( ushort id, MsgArgs msg = null )
    {
        if ( !_internalMsgDict.ContainsKey( id ) ) throw new Exception( "事件未注册：" + id.ToString() );
        _internalMsgDict[ id ].Invoke( msg );
    }
    #endregion
}

/// <summary>
/// 消息参数体。
/// </summary>
public class MsgArgs
{
    //考虑导复合类型的参数传递，这里牺牲了一定了性能
    public List<object> args;

    public MsgArgs( params object[] rArgs )
    {
        args = new List<object>( rArgs );
    }

    public T Get<T>( int nIndex )
    {
        if ( args == null ) return default;
        if ( nIndex < 0 || nIndex >= this.args.Count ) return default;

        return ( T )args[ nIndex ];
    }
}

public interface IEvent { }

/// <summary>
/// 一个人的枚举
/// </summary>
public enum EventType1
{
    EventAr,
    End,
}
/// <summary>
/// 另一个人的枚举
/// </summary>
public enum EventType2
{
    UIEvent = EventType1.End + 1,
    End,
}
