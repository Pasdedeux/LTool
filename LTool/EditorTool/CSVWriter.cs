#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.EditorTool
* 项目描述 ：
* 类 名 称 ：CSVWriter
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.EditorTool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 23:03:13
* 更新时间 ：2018/5/9 23:03:13
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

namespace LitFramework.EditorTool
{
    public class CSVWriter
    {
        string csv = "";
        public CSVWriter()
        {
            csv = "";
        }
        public CSVWriter( List<List<string>> data )
        {
            Write( data );
        }
        public void Write( List<List<string>> data )
        {
            //将表格转为csv
            for( int i = 0; i < data.Count; ++i )
            {
                AddRow( data[ i ].ToArray() );
            }
        }

        public void AddRow( object[] row )
        {
            //添加行
            string rowstr = "";
            for( int i = 0; i < row.Length; i++ )
            {
                string nvalue = row[ i ].ToString();

                if( nvalue.Contains( "," ) || nvalue.Contains( "\r\n" ) || nvalue.Contains( "\"" ) )
                {
                    if( nvalue.Contains( "\"" ) )
                    {
                        string[] ary = nvalue.Split( '\"' );
                        nvalue = "";
                        for( int k = 0; k < ary.Length; ++k )
                        {
                            nvalue += ary[ k ];
                            if( k != ary.Length - 1 )
                                nvalue += "\"\"";
                        }
                    }

                    nvalue = "\"" + nvalue + "\"";
                }

                rowstr += nvalue;
                if( i != row.Length - 1 )
                    rowstr += ",";
            }
            rowstr += "\r\n";
            csv += rowstr;
        }

        public override string ToString()
        {
            return csv;
        }
    }
}
