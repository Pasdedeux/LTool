#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.GameUtility
* 项目描述 ：
* 类 名 称 ：CSVReader
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.GameUtility
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 22:51:58
* 更新时间 ：2018/5/9 22:51:58
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CSVReader
{
    List<List<string>> csvData = new List<List<string>>();

    public CSVReader( string csv )
    {
        ParseCSV( csv );
    }
    public int Row
    {
        get { return csvData.Count; }
    }
    public int Colume
    {
        get
        {
            if ( csvData.Count == 0 )
                return 0;
            return csvData[ 0 ].Count;
        }
    }
    /// <summary>
    /// 解析csv数据
    /// </summary>
    /// <param name="csv"></param>
    private void ParseCSV( string csv )
    {
        if ( csv == null && csv == "" )
            return;

        csvData.Clear();
        //按行拆分
        string strsp = "\r\n";
        if ( !csv.Contains( "\r" ) )
        {
            strsp = "\n";
        }
        string[] rowstr = csv.Split( new string[] { strsp }, System.StringSplitOptions.RemoveEmptyEntries );
        for ( int i = 0; i < rowstr.Length; ++i )
        {
            List<string> row = new List<string>();
            string cur = "";
            string left = rowstr[ i ];
            int spilt = -1;
            //按照逗号拆分字符串
            do
            {
                spilt = left.IndexOf( "," );

                if ( spilt == -1 )
                    cur = left;
                else
                {
                    cur = left.Substring( 0, spilt );
                    left = left.Substring( spilt + 1 );

                    //第一个字符是"\""的情况
                    if ( cur != "" && cur[ 0 ] == '\"' )
                    {
                        //拼接完整句子
                        while ( cur.LastIndexOf( '"' ) != cur.Length - 1 )
                        {
                            cur += ",";
                            spilt = left.IndexOf( "," );
                            if ( spilt == -1 )
                                cur += left;
                            else
                                cur += left.Substring( 0, spilt );
                            left = left.Substring( spilt + 1 );
                        }
                        //去掉多余的"\""号
                        cur = cur.Substring( 1, cur.Length - 2 );
                        cur = cur.Replace( "\"\"", "\"" );
                    }
                }
                row.Add( cur );
            }
            while ( spilt != -1 );

            csvData.Add( row );
        }

    }
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public string GetData( int x, int y )
    {
        if ( y >= csvData.Count && x >= csvData[ y ].Count )
            return "";
        return csvData[ y ][ x ];
    }

    public string[] GetRow( int index )
    {
        return csvData[ index ].ToArray();
    }

}
