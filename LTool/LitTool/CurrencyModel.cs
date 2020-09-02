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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class CurrencyModel
{
    #region 转换大数值
    public static readonly string[] TOLARGENUMSIGN = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
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

    public static string GetToLargeNum( string str )
    {
        bool isNegative = str[ 0 ] == '-';
        if ( isNegative )
            str = str.Remove( 0, 1 );
        int count = str.Length - 4;
        string sign = TOLARGENUMSIGN[ Mathf.FloorToInt( count / 3f ) ];
        int dotLeftCount = str.Contains( '.' ) ? 0 : ( count % 3 + 1 );

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
        else
        {
            valueStr.Append( str[ 0 ] );
            valueStr.Append( str[ 1 ] );
            valueStr.Append( str[ 2 ] );
            valueStr.Append( str[ 3 ] );
        }
        valueStr.Append( sign );
        if ( isNegative )
            return '-' + valueStr.ToString();
        else
            return valueStr.ToString();
    }

    /// <summary>
    /// 大数量级数字+运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static void Plus( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[0-9].", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[0-9].", "", RegexOptions.IgnoreCase );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        var gap = oriUnitIndex - targetUnitIndex;
        //原来的值小于附加值两个单位以上--替换
        if ( gap <= -2 )
        {
            ori = variable;
            return;
        }
        //原来的值大于附加值两个单位以上--忽略
        else if ( gap >= 2 )
        {
            return;
        }
        //降成同单位级的计算
        string finalUnity = targetUnit;
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );
        if ( oriUnitIndex < targetUnitIndex )
        {
            targetNum *= 1000 * ( targetUnitIndex - oriUnitIndex );
            finalUnity = targetUnit;
        }
        else if ( oriUnitIndex > targetUnitIndex )
        {
            oriNum *= 1000 * ( oriUnitIndex - targetUnitIndex );
            finalUnity = oriUnit;
        }
        oriNum += targetNum;
        ori = string.Format( "{0}{1}", ( oriNum * 0.001f ).ToString( "G4" ), finalUnity );
    }
    /// <summary>
    /// 大数量级数字-运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static void Minus( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[0-9].", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[0-9].", "", RegexOptions.IgnoreCase );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        var gap = oriUnitIndex - targetUnitIndex;
        //原来的值小于附加值单位归0
        if ( gap < 0 )
        {
            ori = "0";
            return;
        }
        //原来的值大于附加值两个单位以上--忽略
        else if ( gap >= 2 )
        {
            return;
        }

        //降成同单位级的计算
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );
        if ( oriUnitIndex > targetUnitIndex )
        {
            oriNum *= 1000 * ( oriUnitIndex - targetUnitIndex );
            oriNum -= targetNum;
            ori = string.Format( "{0}{1}", ( oriNum * 0.001f ).ToString( "G4" ), targetUnit );
        }
        else
        {
            oriNum -= targetNum;
            ori = string.Format( "{0}{1}", oriNum.ToString( "G4" ), targetUnit );
        }
    }
    /// <summary>
    /// 大数量级数字X运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static void Mul( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[0-9].", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[0-9].", "", RegexOptions.IgnoreCase );

        //数字部分
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        //对于乘法单位级+1，用于级数运算
        oriUnitIndex++;
        targetUnitIndex++;

        int mulResultUnitIndex = oriUnitIndex + targetUnitIndex;
        float mulResultNumber = oriNum * targetNum;
        if ( mulResultNumber >= 1000f )
        {
            mulResultUnitIndex++;
            mulResultNumber *= 0.001f;
        }
        mulResultUnitIndex--;
        ori = string.Format( "{0}{1}", mulResultNumber.ToString( "G4" ), mulResultUnitIndex > -1 ? TOLARGENUMSIGN[ mulResultUnitIndex ] : "" );
    }
    /// <summary>
    /// 大数量级数字÷运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static void Div( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[0-9].", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[0-9].", "", RegexOptions.IgnoreCase );

        //数字部分
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        //对于除法单位级+1，用于级数运算
        oriUnitIndex++;
        targetUnitIndex++;

        int mulResultUnitIndex = oriUnitIndex - targetUnitIndex;
        float mulResultNumber = oriNum / targetNum;
        if ( mulResultNumber < 1f && mulResultUnitIndex > 0 )
        {
            mulResultUnitIndex--;
            mulResultNumber *= 1000f;
        }
        mulResultUnitIndex--;
        ori = string.Format( "{0}{1}", mulResultNumber.ToString( "G4" ), mulResultUnitIndex > -1 ? TOLARGENUMSIGN[ mulResultUnitIndex ] : "" );
    }
    #endregion
}
