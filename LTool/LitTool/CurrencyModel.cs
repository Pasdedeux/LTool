/*======================================
* 项目名称 ：Assets.Scripts.Model
* 项目描述 ：
* 类 名 称 ：CurrencyModel
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Model
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2020/6/30 16:07:14
* 更新时间 ：2020/6/30 16:07:14
* 版 本 号 ：v1.0.0.0
*******************************************************************
======================================*/

using LitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class CurrencyModel
{
    #region 转换大数值
    private static readonly string[] TOLARGENUMSIGN = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
            , "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az"
            , "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz"
        };

    public static string ToLargeNum( this int value )
    {
        if ( value < 1000 && value > -1000 )
            return value.ToString();
        string str = value.ToString( "#" );
        return GetToLargeNum( str );
    }
    public static string ToLargeNum( this long value )
    {
        if ( value < 1000 && value > -1000 )
            return value.ToString();
        string str = value.ToString( "#" );
        return GetToLargeNum( str );
    }
    public static string ToLargeNum( this ulong value )
    {
        if ( value < 1000 )
            return value.ToString();
        string str = value.ToString( "#" );
        return GetToLargeNum( str );
    }

    public static string ToLargeNum( this float value )
    {
        if ( value < 1000 && value > -1000 )
            return value.ToString();
        string str = value.ToString( "#" );
        return GetToLargeNum( str );
    }
    public static string ToLargeNum( this double value )
    {
        if ( value < 1000 && value > -1000 )
            return value.ToString();
        string str = value.ToString( "#" );
        return GetToLargeNum( str );
    }

    private static string GetToLargeNum( string str )
    {
        bool isNegative = str[ 0 ] == '-';
        if ( isNegative )
            str = str.Remove( 0, 1 );
        int count = str.Length - 4;
        string sign = TOLARGENUMSIGN[ Mathf.FloorToInt( count / 3f ) ];
        int dotLeftCount = count % 3 + 1;

        StringBuilder valueStr = new StringBuilder();
        if ( dotLeftCount == 1 )
        {
            valueStr.Append( str[ 0 ] );
            valueStr.Append( '.' );
            valueStr.Append( str[ 1 ] );
            valueStr.Append( str[ 2 ] );
        }
        else if ( dotLeftCount == 2 )
        {
            valueStr.Append( str[ 0 ] );
            valueStr.Append( str[ 1 ] );
            valueStr.Append( '.' );
            valueStr.Append( str[ 2 ] );
            valueStr.Append( str[ 3 ] );
        }
        else if ( dotLeftCount == 3 )
        {
            valueStr.Append( str[ 0 ] );
            valueStr.Append( str[ 1 ] );
            valueStr.Append( str[ 2 ] );
            valueStr.Append( '.' );
            valueStr.Append( str[ 3 ] );
            valueStr.Append( str[ 4 ] );
        }
        valueStr.Append( sign );
        if ( isNegative )
            return '-' + valueStr.ToString();
        else
            return valueStr.ToString();
    }
    #endregion
}
