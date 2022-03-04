#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.GameUtility
* 项目描述 ：
* 类 名 称 ：CSVParser
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.GameUtility
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 22:51:28
* 更新时间 ：2018/5/9 22:51:28
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

namespace Litframework.ExcelTool
{
    /// <summary>
    /// 表头为3行制
    /// </summary>
    class CSVParser
    {
        enum EnMethodType
        {
            List,
            Dictionary,
        }

        private string[] _description;
        private string[] _attribute;
        private string[] _type;
        private string _className;
        private const string SPACENAME = "LitFramework";
        private const string TEMPLATE_VARS_NAME = "paras";

        private int _template_vars_index = 0;
        private bool _needToParseFlag = false;
        private string _paramName;

        List<string> CSString = new List<string>();

        public string CreateCS( string className , string csv )
        {
            StringBuilder sb = new StringBuilder();

            CSVReader reader = new CSVReader( csv );

            _template_vars_index = 0;
            _description = reader.GetRow( 0 );
            _attribute = reader.GetRow( 1 );
            _type = reader.GetRow( 2 );

            _className = className;

            AddHead();
            AddBody();
            AddTail();
            string result = GetFomatedCS();

            return result;
        }

        private void AddHead()
        {
            CSString.Add( "using UnityEngine;" );
            CSString.Add( "using System;" );
            CSString.Add( "using System.Collections.Generic;" );
            CSString.Add( "using " + SPACENAME + ";" );
            //CSString.Add( "#if UNITY_EDITOR" );
            //CSString.Add( "using LitFrameworkEditor.EditorExtended;" );
            //CSString.Add( "#endif" );
            CSString.Add( "/// <summary>" );
            CSString.Add( "/// Author : Derek Liu" );
            CSString.Add( "/// </summary>" );
            //CSString.Add( "namespace " + spaceName );
            //CSString.Add( "{" );
            CSString.Add( "public class " + _className );
            CSString.Add( "{" );
        }
        private void AddBody()
        {
            //添加属性
            for( int i = 0; i < _type.Length; i++ ) 
            {
                _type[ i ] = CheckStringType( _type[ i ] );
                CSString.Add( "/// <summary>" );
                CSString.Add( string.Format( "/// {0}" , _description[ i ] ) );
                CSString.Add( "/// </summary>" );
                string subtype = _type[ i ];
                if( subtype.Contains( "En_" ) )
                {
                    subtype = subtype.Substring( 3 );
                }
                CSString.Add( string.Format( "public {0} {1} {{ get; private set; }}" , CheckDictType( subtype ) , _attribute[ i ] ) );
            }
            //添加方法
            //AddMethod( EnMethodType.List );
            AddMethod( EnMethodType.Dictionary );
            AddParseVector3();
        }
        private void AddTail()
        {
            CSString.Add( "" );
            CSString.Add( "}" );
            //CSString.Add( "}" );
        }

        private void AddParseVector3()
        {
            CSString.Add( "" );
            CSString.Add( "/// <summary>" );
            CSString.Add( "/// 解析Vector3" );
            CSString.Add( "/// </summary>" );
            CSString.Add( "/// <param name=\"string\">配置文件数据</param>" );
            CSString.Add( "/// <returns>Vector3</returns>" );
            CSString.Add( "static Vector3 ParseVector3(string str)" );
            CSString.Add( "{" );
            CSString.Add( "str = str.Substring(1, str.Length - 2);" );
            CSString.Add( "str.Replace(\" \", \"\");" );
            CSString.Add( "string[] splits = str.Split(',');" );
            CSString.Add( "float x = float.Parse(splits[0]);" );
            CSString.Add( "float y = float.Parse(splits[1]);" );
            CSString.Add( "float z = float.Parse(splits[2]);" );
            CSString.Add( "return new Vector3(x, y, z);" );
            CSString.Add( "}" );
        }

        private string CheckDictType( string type )
        {
            return type.Replace( "|" , "," );
        }

        private string CheckEnType( string type )
        {
            if( type.Contains( "En_" ) )
            {
                return type.Substring( 3 );
            }
            return type;
        }

        private void AddMethod( EnMethodType mtype )
        {
            //if ( mtype == EnMethodType.List )
            //    CSString.Add( string.Format( "private static List<{0}> vec = new List<{0}>();", _className ) );
            //else
            //    CSString.Add( string.Format( "private static Dictionary<{0}, {1}> vec = new Dictionary<{0}, {1}>();", CheckEnType( _type[ 0 ] ), _className ) );

            CSString.Add( "" );
            CSString.Add( "/// <summary>" );
            CSString.Add( "/// 读取配置文件" );
            CSString.Add( "/// </summary>" );
            CSString.Add( "/// <param name=\"config\">配置文件数据</param>" );
            CSString.Add( "/// <returns>数据列表</returns>" );
            if( mtype == EnMethodType.List )
                CSString.Add( "public static List<" + _className + "> ReturnList(string csv)" );
            else
                CSString.Add( string.Format( "public static Dictionary<{0}, {1}> ReturnDictionary(string csv)" , CheckEnType( _type[ 0 ] ) , _className ) );
            CSString.Add( "{" );
            if( mtype == EnMethodType.List )
                CSString.Add( string.Format( "List<{0}> vec = new List<{0}>();" , _className ) );
            else
                CSString.Add( string.Format( "Dictionary<{0}, {1}> vec = new Dictionary<{0}, {1}>();" , CheckEnType( _type[ 0 ] ) , _className ) );
            CSString.Add( "CSVReader reader = new CSVReader(csv);" );
            CSString.Add( "for (int i = 3; i < reader.Row; i++)" );
            CSString.Add( "{" );
            CSString.Add( string.Format( "{0} item = new {0}();" , _className ) );
            //中心
            for ( int i = 0; i < _type.Length; i++ )
            {
                if( _type[ i ].Contains( "En_" ) )
                {
                    string subtype = _type[ i ].Substring( 3 );
                    CSString.Add( string.Format( "item.{0} = {1};" , _attribute[ i ] , 
                        ParseBaseType( subtype , string.Format( "reader.GetData({0}, i)" , i ), ref _needToParseFlag, out _paramName) ) );
                }
                else if( _type[ i ].Contains( "List<" ) )
                {
                    int stb = _type[ i ].IndexOf( '<' );
                    int ste = _type[ i ].Count() - 1;
                    string subtype = _type[ i ].Substring( stb + 1 , ste - stb - 1 );
                    string substrName = string.Format( "{0}_Array" , _attribute[ i ] );

                    CSString.Add( string.Format( "item.{0}= new List<{1}>();" , _attribute[ i ] , subtype ) );

                    CSString.Add(string.Format("if(!string.IsNullOrEmpty({0}))", string.Format("reader.GetData({0}, i)", i)));
                    CSString.Add("{");
                    CSString.Add(string.Format("string[] {0} = {1}.Split(';');", substrName, string.Format("reader.GetData({0}, i)", i)));
                    CSString.Add( string.Format( "for (int j = 0; j < {0}.Length; j++)" , substrName ) );
                    CSString.Add( "{" );
                    if( subtype.Contains( "List<" ) )
                    {
                        int ustb = subtype.IndexOf( '<' );
                        int uste = subtype.Count() - 1;
                        string usubtype = subtype.Substring( ustb + 1 , uste - ustb - 1 );//基础type
                        string usubstrName = string.Format( "{0}_Array2" , _attribute[ i ] );//一级数组
                        string usublstname = string.Format( "{0}_List" , _attribute[ i ] );//一级list名
                        CSString.Add( string.Format( "List<{0}> {1} = new List<{0}>();" , usubtype , usublstname ) );
                        CSString.Add(string.Format("if(!string.IsNullOrEmpty({0}))", string.Format("reader.GetData({0}, i)", i)));
                        CSString.Add("{");
                        CSString.Add( string.Format( "string[] {0} = {1}[j].Split('|');" , usubstrName , substrName ) );

                        CSString.Add( string.Format( "item.{0}= new List<{1}>();" , _attribute[ i ] , usubtype ) );

                        CSString.Add( string.Format( "for (int k = 0; k < {0}.Length; k++)" , usubstrName ) );
                        CSString.Add( "{" );
                        CSString.Add( string.Format( "{0}.Add({1});" , usublstname , 
                            ParseBaseType( usubtype , string.Format( "{0}[k]" , usubstrName ), ref _needToParseFlag, out _paramName) ) );
                        CSString.Add( "}" ); 
                        CSString.Add("}");
                        CSString.Add( string.Format( "item.{0}.Add({1});" , _attribute[ i ] , usublstname ) );
                    }
                    else
                    {
                        CSString.Add( string.Format( "item.{0}.Add({1});" , _attribute[ i ] , 
                            ParseBaseType( subtype , string.Format( "{0}[j]" , substrName ), ref _needToParseFlag, out _paramName) ) );

                    }
                    CSString.Add( "}" );
                    CSString.Add("}");

                }
                else if( _type[ i ].Contains( "Dictionary<" ) )
                {
                    int stb = _type[ i ].IndexOf( '<' );
                    int ste = _type[ i ].IndexOf( '>' );
                    string[] subtype = _type[ i ].Substring( stb + 1 , ste - stb - 1 ).Split( ',' );
                    CSString.Add( string.Format( "item.{0} = new Dictionary<{1}, {2}>();" , _attribute[ i ] , subtype[ 0 ] , subtype[ 1 ] ) );
                    string substrName = string.Format( "{0}_Array" , _attribute[ i ] );
                    CSString.Add(string.Format("if(!string.IsNullOrEmpty({0}))",  string.Format("reader.GetData({0}, i)", i)));
                    CSString.Add("{");
                    CSString.Add( string.Format( "string[] {0} = {1}.Split(';');" , substrName , string.Format( "reader.GetData({0}, i)" , i ) ) );
                    CSString.Add( string.Format( "for (int j = 0; j < {0}.Length; j++)" , substrName ) );
                    CSString.Add( "{" );
                    CSString.Add( string.Format( "string[] subArray = {0}[j].Split('|');" , substrName ) );
                    CSString.Add( string.Format( "item.{0}.Add({1}, {2});" , _attribute[ i ] , 
                        ParseBaseType( subtype[ 0 ] , "subArray[0]", ref _needToParseFlag, out _paramName) , 
                        ParseBaseType( subtype[ 1 ] , "subArray[1]", ref _needToParseFlag, out _paramName) ) );
                    CSString.Add( "}" );
                    CSString.Add("}");
                }
                else
                {
                    var resultParse = ParseBaseType(_type[i], string.Format("reader.GetData({0}, i)", i), ref _needToParseFlag, out _paramName);
                    if (!_needToParseFlag)
                        CSString.Add(string.Format("item.{0} = {1};", _attribute[i], resultParse));
                    else{
                        CSString.Add($"{resultParse};");
                        CSString.Add(string.Format("item.{0} = {1};", _attribute[i], _paramName));
                    }
                }
            }
            //收尾
            if (mtype == EnMethodType.List)
                CSString.Add("vec.Add(item);");
            else
            {
                CSString.Add("try");
                CSString.Add("{");
                CSString.Add(string.Format("vec.Add(item.{0}, item);", _attribute[0]));
                CSString.Add("}");
                CSString.Add("catch (Exception e)");
                CSString.Add("{");
                CSString.Add("LDebug.LogError($\"" + "{e.Message} 表: " + $"{_className} " + "行: {i}" + "列: " + $"{_attribute[0]}" + "\", LogColor.red); ");
                CSString.Add("}");
            }
            CSString.Add( "}" );
            CSString.Add( "return vec;" );
            CSString.Add( "}" );
        }

        private string CheckStringType( string type )
        {
            if( type.Length == 0 )
                return "string";
            if( type.Contains( "Dic" ) )
                return type.Replace( "Dic" , "Dictionary" );
           if( type.Contains( "\"" ) )
                return type.Replace( "\"" , "" );
            if( type.Contains( " " ) )
                return type.Replace( " " , "" );

            return type;
        }

        /// <summary>
        /// 最终整合
        /// </summary>
        /// <returns>原代码文件</returns>
        private string GetFomatedCS()
        {
            StringBuilder result = new StringBuilder();
            int tablevel = 0;
            for( int i = 0; i < CSString.Count; i++ )
            {
                string tab = "";

                for( int j = 0; j < tablevel; ++j )
                    tab += "\t";

                if( CSString[ i ].Contains( "{" ) )
                    tablevel++;
                if( CSString[ i ].Contains( "}" ) )
                {
                    tablevel--;
                    tab = "";
                    for( int j = 0; j < tablevel; ++j )
                        tab += "\t";
                }

                result.Append( tab + CSString[ i ] + "\n" );
            }
            return result.ToString();
        }

        private string ParseBaseType( string type , string attribute , ref bool needTryParse , out string paramName )
        {
            if (type.Length == 0) throw new Exception($"解析配置表 {_className} 失败！存在空数据类型! ");
            
            needTryParse = true;
            paramName = $"{TEMPLATE_VARS_NAME}{_template_vars_index}";
            string result = "";
            switch( type )
            {
                case "string":
                    result = attribute;
                    needTryParse = false;
                    return result; ;
                case "char":
                    result = attribute;
                    needTryParse = false;
                    return result; ;
                case "DateTime":
                    result = "DateTime.TryParse(" + attribute + $", out DateTime {paramName})";
                    break;
                case "short":
                    result = "short.TryParse(" + attribute + $", out short {paramName})";
                    break;
                case "int":
                    result = "int.TryParse(" + attribute + $", out int {paramName})";
                    break;
                case "long":
                    result = "long.TryParse(" + attribute + $", out long {paramName})";
                    break;
                case "float":
                    result = "float.TryParse(" + attribute + $", out float {paramName})";
                    break;
                case "double":
                    result = "double.TryParse(" + attribute + $", out double {paramName})";
                    break;
                case "bool":
                    result = "bool.TryParse(" + attribute + $", out bool {paramName})";
                    break;
                case "Vector3":
                    result = "ParseVector3(" + attribute + ")";
                    needTryParse = false;
                    break;
                case "En_":
                    result = "(" + type + ")Enum.Parse(typeof(" +type +")," + attribute + ")";
                    break;
            }
            _template_vars_index++;
            return result;
        }
    }
}

