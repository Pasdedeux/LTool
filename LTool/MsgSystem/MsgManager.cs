/*======================================
* 项目名称 ：LitFramework.MsgSystem
* 项目描述 ：
* 类 名 称 ：MsgManager
* 类 描 述 ：
* 命名空间 ：LitFramework.MsgSystem
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/3/24 18:30:22
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 事件管理器
/// 
/// id内部采用ushort类型，参数采用枚举类型
/// </summary>
public class MsgManager : Singleton<MsgManager>
{
    //主要事件记录器
    private Dictionary<ushort, Action<MsgArgs>> _internalMsgDict;
    //临时事件记录器（用于局部临时回调）
    private Dictionary<string, Action<MsgArgs>> _internalMsgTempleDict;

    public MsgManager()
    {
        _internalMsgDict = new Dictionary<ushort, Action<MsgArgs>>();
        _internalMsgTempleDict = new Dictionary<string, Action<MsgArgs>>();
    }

    #region 枚举方案

    //：有些许GC
    //通用枚举的转换，采用的是IConvertible.ToUInt16。中间借用了format涉及到了拆装箱，ID转换将会固定产生40B
    /// <summary>
    /// 注册事件关注
    /// </summary>
    /// <typeparam name="T">统用枚举类。可接受多种枚举。这里会转换为ushort类型。</typeparam>
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
    /// 广播事件。统用枚举类
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

    #region string Temple事件方案
    /// <summary>
    /// 注册事件关注。推荐用于临时事件注册
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="callBack">事件回调</param>
    public void Register( string id, Action<MsgArgs> callBack )
    {
        if ( !_internalMsgTempleDict.ContainsKey( id ) )
            _internalMsgTempleDict.Add( id, null );
        _internalMsgTempleDict[ id ] += callBack;
    }

    /// <summary>
    /// 取消事件关注。推荐用于临时事件注册
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="callBack">事件回调</param>
    public void UnRegister( string id, Action<MsgArgs> callBack )
    {
        if ( _internalMsgTempleDict.ContainsKey( id ) )
        {
            _internalMsgTempleDict[ id ] -= callBack;
            if ( _internalMsgTempleDict[ id ] == null ) _internalMsgTempleDict.Remove( id );
        }
    }

    /// <summary>
    /// 广播事件。推荐用于临时事件注册
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="msg">事件参数</param>
    public void Broadcast( string id, MsgArgs msg = null )
    {
        if ( !_internalMsgTempleDict.ContainsKey( id ) ) throw new Exception( "事件未注册：" + id.ToString() );
        _internalMsgTempleDict[ id ].Invoke( msg );
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

///// <summary>
///// 消息参数体T。
///// </summary>
//public class MsgArgs<T> : IMsgArgs<T>
//{
//    //考虑导复合类型的参数传递，这里牺牲了一定了性能
//    public List<T> args;

//    public MsgArgs( params T[] rArgs )
//    {
//        args = new List<T>( rArgs );
//    }

//    public T Get( int nIndex )
//    {
//        if ( args == null ) return default;
//        if ( nIndex < 0 || nIndex >= args.Count ) return default;

//        return args[ nIndex ];
//    }

//    public T Get<T1>( int nIndex )
//    {
//        return Get( nIndex );
//    }
//}


public interface IMsgArgs<T>
{
    T Get<T>( int nIndex );
}

