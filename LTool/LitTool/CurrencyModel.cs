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
        StringBuilder valueStr = new StringBuilder();
        //是否是小数
        if ( str.Contains( '.' ) )
        {
            string[] nums = str.Split( '.' );
            int left = nums[ 0 ].Length;
            int right = nums[ 1 ].Length;
            if ( left > 3 ) //小数点左边大于3位数
            {
                int tolargenmsignIndex = left / 3 - 1;
                string sign = TOLARGENUMSIGN[ tolargenmsignIndex ];
                int ynum = left % 3;
                if ( left > 5 )
                {
                    ynum += 1;
                }

                for ( int i = 0; i < ynum; i++ )
                {
                    valueStr.Append( nums[ 0 ][ i ] );
                }
                valueStr.Append( "." );
                int lastNum = 4 - ynum;
                for ( int i = 0; i < lastNum; i++ )
                {
                    valueStr.Append( nums[ 0 ][ ynum + i ] );
                }

                valueStr.Append( sign );
            }
            else //小数点左边不足
            {

                for ( int i = 0; i < left; i++ )
                {
                    valueStr.Append( nums[ 0 ][ i ] );
                }
                int lastNum = 5 - left;
                valueStr.Append( "." );
                lastNum = right > lastNum ? lastNum : right;
                for ( int i = 0; i < lastNum; i++ )
                {
                    valueStr.Append( nums[ 1 ][ i ] );
                }
            }
        }
        else
        {
            if ( str.Length > 3 )
            {
                int tolargenmsignIndex = str.Length / 3 - 1;
                string sign = TOLARGENUMSIGN[ tolargenmsignIndex ];
                int ynum = str.Length % 3;
                if ( str.Length > 5 )
                {
                    ynum += 1;
                }

                for ( int i = 0; i < ynum; i++ )
                {
                    valueStr.Append( str[ i ] );
                }
                valueStr.Append( "." );
                int lastNum = 4 - ynum;
                for ( int i = 0; i < lastNum; i++ )
                {
                    valueStr.Append( str[ ynum + i ] );
                }
                valueStr.Append( sign );
            }
            else
            {
                valueStr.Append( str );
            }

        }

        return valueStr.ToString();
    }

    /// <summary>
    /// 大数量级数字+运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static string Plus( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[\\d\\.]", "", RegexOptions.IgnoreCase );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        var gap = oriUnitIndex - targetUnitIndex;
        //原来的值小于附加值1个单位以上--替换
        if ( gap < -1 )
        {
            ori = variable;
            return ori;
        }
        //原来的值大于附加值1个单位以上--忽略
        else if ( gap > 1 )
        {
            return ori;
        }
        //降成同单位级的计算
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );
        if ( oriUnitIndex < targetUnitIndex )
        {
            targetNum *= 1000 * ( targetUnitIndex - oriUnitIndex );
            oriNum += targetNum;
            ori = string.Format( "{0}{1}", ( oriNum * 0.001f ).ToString( targetUnit == "" ? "F0" : "G4" ), targetUnit );
        }
        else if ( oriUnitIndex > targetUnitIndex )
        {
            oriNum *= 1000 * ( oriUnitIndex - targetUnitIndex );
            oriNum += targetNum;
            ori = string.Format( "{0}{1}", ( oriNum * 0.001f ).ToString( oriUnit == "" ? "F0" : "G4" ), oriUnit );
        }
        else
        {
            oriNum += targetNum;
            if ( oriNum >= 1000 )
                ori = oriNum.ToLargeNum();
            else
                ori = string.Format( "{0}{1}", ( oriNum ).ToString( oriUnit == "" ? "F0" : "G4" ), oriUnit );
        }
        return ori;
    }
    /// <summary>
    /// 大数量级数字-运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static string Minus( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[\\d\\.]", "", RegexOptions.IgnoreCase );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        var gap = oriUnitIndex - targetUnitIndex;
        //被减数单位量级小于减数单位量级
        if ( gap < 0 )
        {
            ori = "0";
            return ori;
        }
        //原来的值大于附加值1个单位以上--忽略
        else if ( gap > 1 )
        {
            return ori;
        }

        //降成同单位级的计算
        string finalUnity = targetUnit;
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );
        //被减数单位量级大于减数单位量级
        if ( oriUnitIndex > targetUnitIndex )
        {
            oriNum *= 1000 * ( oriUnitIndex - targetUnitIndex );
            oriNum -= targetNum;
            if ( oriNum > 1000 )
            {
                finalUnity = oriUnit;
                ori = string.Format( "{0}{1}", ( oriNum * 0.001f ).ToString( finalUnity == "" ? "F0" : "G4" ), finalUnity );
            }
            else if ( oriNum > 1 ) ori = string.Format( "{0}{1}", oriNum.ToString( finalUnity == "" ? "F0" : "G4" ), finalUnity );
            //exm:1.996b-1b=0.996b=>996a
            else if ( targetUnitIndex > 0 ) ori = string.Format( "{0}{1}", ( oriNum * 1000f ).ToString( finalUnity == "" ? "F0" : "G4" ), TOLARGENUMSIGN[ targetUnitIndex - 1 ] );
            //exm:1.996a-1a=0.996=>996
            else if ( targetUnitIndex == 0 ) ori = string.Format( "{0}{1}", ( oriNum * 1000f ).ToString( "F0" ), "" );
            //exm:1.996-1=0.996=>0
            else ori = "0";
        }
        //被减数单位量级等于减数单位量级
        else
        {
            oriNum -= targetNum;
            if ( oriNum > 1 ) ori = string.Format( "{0}{1}", oriNum.ToString( finalUnity == "" ? "F0" : "G4" ), finalUnity );
            //exm:1.996b-1b=0.996b=>996a
            else if ( targetUnitIndex > 0 ) ori = string.Format( "{0}{1}", ( oriNum * 1000f ).ToString( finalUnity == "" ? "F0" : "G4" ), TOLARGENUMSIGN[ targetUnitIndex - 1 ] );
            //exm:1.996a-1a=0.996=>996
            else if ( targetUnitIndex == 0 ) ori = string.Format( "{0}{1}", ( oriNum * 1000f ).ToString( "F0" ), "" );
            //exm:1.996-1=0.996=>0
            else ori = "0";
        }
        return ori;
    }
    /// <summary>
    /// 大数量级数字X运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static string Mul( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[\\d\\.]", "", RegexOptions.IgnoreCase );

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
        if ( mulResultUnitIndex > -1 )
            ori = string.Format( "{0}{1}", mulResultNumber.ToString( "G4" ), TOLARGENUMSIGN[ mulResultUnitIndex ] );
        else
            ori = string.Format( "{0}{1}", mulResultNumber.ToString( "F0" ), "" );
        return ori;
    }
    /// <summary>
    /// 大数量级数字÷运算
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    public static string Div( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[\\d\\.]", "", RegexOptions.IgnoreCase );

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
        if ( mulResultUnitIndex > -1 )
            ori = string.Format( "{0}{1}", mulResultNumber.ToString( "G4" ), TOLARGENUMSIGN[ mulResultUnitIndex ] );
        else
            ori = string.Format( "{0}{1}", mulResultNumber.ToString( "F0" ), "" );
        return ori;
    }
    /// <summary>
    /// 大数量级数字比较大小
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    /// <returns> 《0小于   =0等于  》0大于 </returns>
    public static float CompareValue( this string ori, string variable )
    {
        var oriUnit = Regex.Replace( ori, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );

        if ( oriUnitIndex < targetUnitIndex )
        {
            return -1;
        }
        else if ( oriUnitIndex > targetUnitIndex )
        {
            return 1;
        }

        //数字部分
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );
        return oriNum - targetNum;
    }
    /// <summary>
    /// 计算1个大数据相互比例（分母）
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static float CalculateRatio( this string ori, string variable )
    {
        //降成同单位级的计算
        var oriNum = float.Parse( Regex.Replace( ori, "[a-z]", "", RegexOptions.IgnoreCase ) );
        var targetNum = float.Parse( Regex.Replace( variable, "[a-z]", "", RegexOptions.IgnoreCase ) );

        if ( targetNum == 0 ) return 0f;

        var oriUnit = Regex.Replace( ori, "[\\d\\.]", "", RegexOptions.IgnoreCase );
        var targetUnit = Regex.Replace( variable, "[\\d\\.]", "", RegexOptions.IgnoreCase );

        //原本单位级
        var oriUnitIndex = Array.IndexOf( TOLARGENUMSIGN, oriUnit );
        //附加单位级
        var targetUnitIndex = Array.IndexOf( TOLARGENUMSIGN, targetUnit );
        var gap = oriUnitIndex - targetUnitIndex;
        //原来的值小于附加值1个单位以上
        if ( gap < -1 )
        {
            return 0f;
        }
        //原来的值大于附加值1个单位以上
        else if ( gap > 1 )
        {
            return 1f;
        }

        if ( oriUnitIndex < targetUnitIndex )
        {
            targetNum *= 1000 * ( targetUnitIndex - oriUnitIndex );
        }
        else if ( oriUnitIndex > targetUnitIndex )
        {
            oriNum *= 1000 * ( oriUnitIndex - targetUnitIndex );
        }
        return oriNum / targetNum;
    }
    #endregion
}
