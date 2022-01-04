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
using UnityEditor;
using UnityEngine;

namespace LitFrameworkEditor.EditorExtended
{
    /// <summary>
    /// 表头为3行制
    /// </summary>
    public class SQLParser
    {
        private string[] _description;
        private string[] _attribute;
        private string[] _type;
        private string _className, _classNameWithoutFlag;
        private const string SPACENAME = "LitFramework";

        List<string> CSString = new List<string>();

        public string CreateCS(string className, string csv)
        {
            StringBuilder sb = new StringBuilder();

            CSVReader reader = new CSVReader(csv);

            _description = reader.GetRow(0);
            _attribute = reader.GetRow(1);
            _type = reader.GetRow(2);

            _className = className;

            AddUsingNameSpace();

            AddHead_Table();
            AddBody_Table();

            CSString.Add("");
            CSString.Add("#region Raw");

            AddHead();
            AddBody();
            AddTail();

            CSString.Add("#endregion");

            AddTail_Table();
            string result = GetFomatedCS();

            return result;
        }
        private void AddUsingNameSpace()
        {
            CSString.Add("using UnityEngine;");
            CSString.Add("using System;");
            CSString.Add("using System.Text;");
            CSString.Add("using System.Collections.Generic;");
            CSString.Add("using SQLite;");
            CSString.Add("using " + SPACENAME + ";");
            //CSString.Add( "#if UNITY_EDITOR" );
            //CSString.Add( "using LitFrameworkEditor.EditorExtended;" );
            //CSString.Add( "#endif" );
            CSString.Add("/// <summary>");
            CSString.Add("/// Author : Derek Liu");
            CSString.Add("/// 创建时间:" + DateTime.Now.ToString());
            CSString.Add("/// </summary>");
        }
        private void AddHead()
        {
            CSString.Add("public class " + _className);
            CSString.Add("{");
        }
        private void AddBody()
        {
            //添加属性
            for (int i = 0; i < _type.Length; i++)
            {
                _type[i] = CheckStringType(_type[i]);
                string subtype = _type[i];
                if (subtype.Contains("En_"))
                {
                    subtype = subtype.Substring(3);
                }
                var tAttrib = _attribute[i];
                var tPropTP = GetPropertyTP(subtype);
                subtype = CheckDictType(subtype);

                switch (tPropTP)
                {
                    case PropertyTP._string:
                        {
                            string sPriAtrri = "s" + _attribute[i];
                            CSString.Add(string.Format("private {0} {1};", subtype, sPriAtrri));
                            CSString.Add("/// <summary>");
                            CSString.Add(string.Format("/// {0}", _description[i]));
                            CSString.Add("/// </summary>");

                            CSString.Add(string.Format("public {0} {1} {{ ", subtype, tAttrib));
                            CSString.Add("get {");
                            CSString.Add(string.Format("if(string.IsNullOrEmpty({0})){{", sPriAtrri));
                            CSString.Add(string.Format("{0} = SQLManager.Instance.ExcurteScalar<string>(string.Format(SQLReader.SINGLE,\"{1}\",TName,ID));", sPriAtrri, tAttrib));
                            CSString.Add("}");
                            CSString.Add(string.Format("return {0};", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add("}");
                        }
                        break;
                    case PropertyTP._list:
                        {
                            string sPriAtrri = "s" + _attribute[i];

                            CSString.Add(string.Format("private {0} {1};", subtype, sPriAtrri));
                            CSString.Add("/// <summary>");
                            CSString.Add(string.Format("/// {0}", _description[i]));
                            CSString.Add("/// </summary>");

                            CSString.Add(string.Format("public {0} {1} {{ ", subtype, tAttrib));
                            CSString.Add("get {");
                            CSString.Add(string.Format("if({0} == null){{", sPriAtrri));
                            CSString.Add(string.Format("{0} = new {1}();", sPriAtrri, subtype));
                            CSString.Add("}");
                            CSString.Add(string.Format("var tData = SQLManager.Instance.ExcurteScalar<string>(string.Format(SQLReader.SINGLE,\"{0}\",TName,ID));", tAttrib));
                            CSString.Add("if(string.IsNullOrEmpty(tData)){");
                            CSString.Add(string.Format("return {0};", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add("string[] tSplits = tData.Split(';');");
                            CSString.Add("for (int j = 0; j < tSplits.Length; j++) {");
                            int stb = subtype.IndexOf('<');
                            int ste = subtype.Count() - 1;
                            string subtype_list_in = _type[i].Substring(stb + 1, ste - stb - 1);
                            CSString.Add(string.Format("{0} tValue;", subtype_list_in));
                            CSString.Add(string.Format("if(!{0}.TryParse(tSplits[j],out tValue)){{", subtype_list_in));
                            CSString.Add("throw new ArgumentException(\"Parse Error\");");
                            CSString.Add("}");
                            CSString.Add(string.Format("{0}.Add(tValue);", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add(string.Format("return {0};", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add("}");
                        }
                        break;
                    case PropertyTP._dic:
                        {
                            string sPriAtrri = "s" + _attribute[i];

                            int stb = subtype.IndexOf('<');
                            int ste = subtype.IndexOf('>');
                            string[] subtype_dic_in = subtype.Substring(stb + 1, ste - stb - 1).Split(',');
                            string subtype_dic_in1 = subtype_dic_in[0];
                            string subtype_dic_in2 = subtype_dic_in[1];

                            CSString.Add(string.Format("private {0} {1};", CheckDictType(subtype), sPriAtrri));

                            CSString.Add("/// <summary>");
                            CSString.Add(string.Format("/// {0}", _description[i]));
                            CSString.Add("/// </summary>");
                            CSString.Add(string.Format("public {0} {1} {{ ", CheckDictType(subtype), tAttrib));
                            CSString.Add("get {");
                            CSString.Add(string.Format("if({0} == null){{", sPriAtrri));
                            CSString.Add(string.Format("{0} = new {1}();", sPriAtrri, subtype));
                            CSString.Add("}");

                            CSString.Add(string.Format("var tData = SQLManager.Instance.ExcurteScalar<string>(string.Format(SQLReader.SINGLE,\"{0}\",TName,ID));", tAttrib));
                            CSString.Add("if(string.IsNullOrEmpty(tData)){");
                            CSString.Add(string.Format("return {0};", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add("var tSplits = tData.Split(';');");
                            CSString.Add("for (int j = 0; j < tSplits.Length; j++) {");
                            CSString.Add("var tSubSplit = tSplits[j].Split('|');");
                            CSString.Add(string.Format("{0} tKey;", subtype_dic_in1));
                            CSString.Add(string.Format("{0} tValue;", subtype_dic_in2));

                            CSString.Add(string.Format("if(!{0}.TryParse(tSubSplit[0],out tKey)){{", subtype_dic_in1));
                            CSString.Add("throw new ArgumentException(\"Parse Error\");");
                            CSString.Add("}");

                            CSString.Add(string.Format("if(!{0}.TryParse(tSubSplit[1],out tValue)){{", subtype_dic_in2));
                            CSString.Add("throw new ArgumentException(\"Parse Error\");");
                            CSString.Add("}");

                            CSString.Add(string.Format("{0}.Add(tKey,tValue);", sPriAtrri));

                            CSString.Add("}");
                            CSString.Add(string.Format("return {0};", sPriAtrri));

                            CSString.Add("}");
                            CSString.Add("}");
                        }
                        break;
                    case PropertyTP._vc3:
                        {
                            string sPriAtrri = "s" + _attribute[i];
                            CSString.Add(string.Format("private bool  {0}_v3;", sPriAtrri));
                            CSString.Add(string.Format("private {0} {1};", subtype, sPriAtrri));
                            CSString.Add("/// <summary>");
                            CSString.Add(string.Format("/// {0}", _description[i]));
                            CSString.Add("/// </summary>");

                            CSString.Add(string.Format("public {0} {1} {{ ", subtype, tAttrib));
                            CSString.Add("get{");
                            CSString.Add(string.Format("if(!{0}_v3) {{", sPriAtrri));
                            CSString.Add(string.Format("var aStr = SQLManager.Instance.ExcurteScalar<string>(string.Format(SQLReader.SINGLE,\"{0}\",TName,ID));", tAttrib));
                            CSString.Add("if(string.IsNullOrEmpty(aStr)){");
                            CSString.Add(string.Format("{0}_v3 = true;", sPriAtrri));
                            CSString.Add(string.Format("return {0};", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add(string.Format("{0} = ParseVector3(aStr);", sPriAtrri));
                            CSString.Add(string.Format("{0}_v3 = true;", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add(string.Format("return {0};", sPriAtrri));
                            CSString.Add("}");
                            CSString.Add("}");
                        }
                        break;
                    default:
                        {
                            CSString.Add("/// <summary>");
                            CSString.Add(string.Format("/// {0}", _description[i]));
                            CSString.Add("/// </summary>");
                            CSString.Add(string.Format("public {0} {1} {{get;set;}}", CheckDictType(subtype), tAttrib));
                        }
                        break;
                }

            }
            AddParseVector3();
            AddDebug();
        }
        private void AddTail()
        {
            CSString.Add("");
            CSString.Add("}");
        }

        private void AddParseVector3()
        {
            CSString.Add("");
            CSString.Add("/// <summary>");
            CSString.Add("/// 解析Vector3");
            CSString.Add("/// </summary>");
            CSString.Add("/// <param name=\"string\">配置文件数据</param>");
            CSString.Add("/// <returns>Vector3</returns>");
            CSString.Add("static Vector3 ParseVector3(string str)");
            CSString.Add("{");
            CSString.Add("str = str.Substring(1, str.Length - 2);");
            CSString.Add("str.Replace(\" \", \"\");");
            CSString.Add("string[] splits = str.Split(',');");
            CSString.Add("float x = float.Parse(splits[0]);");
            CSString.Add("float y = float.Parse(splits[1]);");
            CSString.Add("float z = float.Parse(splits[2]);");
            CSString.Add("return new Vector3(x, y, z);");
            CSString.Add("}");
        }

        private void AddDebug()
        {
            //Debug.
            CSString.Add("public override string ToString(){");
            CSString.Add("StringBuilder sb = new StringBuilder();");
            //添加属性
            for (int i = 0; i < _type.Length; i++)
            {
                _type[i] = CheckStringType(_type[i]);
                string subtype = _type[i];
                if (subtype.Contains("En_"))
                {
                    subtype = subtype.Substring(3);
                }
                var tAttrib = _attribute[i];
                var tPropTP = GetPropertyTP(subtype);
                subtype = CheckDictType(subtype);

                switch (tPropTP)
                {
                    case PropertyTP._string:
                        {
                            CSString.Add(string.Format("sb.AppendLine(string.Format(\"{0}={{0}}\" ,{0}.ToString()));", tAttrib));
                        }
                        break;
                    case PropertyTP._list:
                        {
                            CSString.Add(string.Format("sb.AppendLine(string.Format(\"{0}.Count={{0}}\" ,{0}.Count));", tAttrib));
                        }
                        break;
                    case PropertyTP._dic:
                        {
                            CSString.Add(string.Format("sb.AppendLine(string.Format(\"{0}.Count={{0}}\" ,{0}.Count));", tAttrib));
                        }
                        break;
                    case PropertyTP._vc3:
                        {
                            CSString.Add(string.Format("sb.AppendLine(string.Format(\"{0}={{0}}\" ,{0}.ToString()));", tAttrib));
                        }
                        break;
                    default:
                        {
                            CSString.Add(string.Format("sb.AppendLine(string.Format(\"{0}={{0}}\" ,{0}.ToString()));", tAttrib));
                        }
                        break;
                }
            }
            CSString.Add("return sb.ToString();");
            CSString.Add("}");
        }
        private void AddHead_Table()
        {
            CSString.Add("public class " + _className );
            CSString.Add("{");
            _classNameWithoutFlag = _className;
            //增加Raw后缀
            _className += "Raw";
        }
        private void AddBody_Table()
        {
            CSString.Add(string.Format("public const string TName = \"{0}\";", _classNameWithoutFlag));

            CSString.Add("private int sCount = -1;");
            CSString.Add("public  int  Count {");
            CSString.Add("get {");
            CSString.Add("if(sCount == -1) {");
            CSString.Add("sCount = SQLManager.Instance.ExcurteScalar<int>(string.Format(SQLReader.COUNT,TName));");
            CSString.Add("}");
            CSString.Add("return sCount;");
            CSString.Add("}");
            CSString.Add("}");




            CSString.Add("private List<int> sIDs;");
            CSString.Add("public  List<int> IDs {");
            CSString.Add("get {");
            CSString.Add("if(sIDs == null) {");
            CSString.Add("sIDs = new List<int>();");
            CSString.Add("var tList = SQLManager.Instance.QueryGeneric(string.Format(SQLReader.SELECT_CLOWS_ORDERBY,\"ID\",TName)).rows;");
            CSString.Add("for(int i = 0; i < tList.Count; i++){");
            CSString.Add("sIDs.Add((int)tList[i][0]);");
            CSString.Add("}");
            CSString.Add("}");
            CSString.Add("return sIDs;");
            CSString.Add("}");
            CSString.Add("}");



            CSString.Add(string.Format("private Dictionary<int,{0}> sTable;",_className));
            CSString.Add(string.Format("public {0} this[int index] {{", _className));
            CSString.Add("get {");

            CSString.Add("if(sTable == null){");
            CSString.Add(string.Format("sTable = new Dictionary<int,{0}>();", _className));
            CSString.Add("}");

            CSString.Add(string.Format("{0} tObj;",_className));
            CSString.Add("if(!sTable.TryGetValue(index,out tObj)){");
            CSString.Add("bool tOK = false;");
            CSString.Add(string.Format("tObj = SQLManager.Instance.QueryFirstRecord<{0}>(out tOK,string.Format(SQLReader.SINGLE_ROW,TName,index));",_className));
            CSString.Add("if(!tOK){");
            CSString.Add("return null;");
            CSString.Add("}");
            CSString.Add("sTable[index] = tObj;");
            CSString.Add("}");
            CSString.Add("return tObj;");
            CSString.Add("}");
            CSString.Add("}");

        }
        private void AddTail_Table()
        {
            CSString.Add("");
            CSString.Add("}");
        }

        private string CheckDictType( string type )
        {
            return type.Replace( "|" , "," );
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
        static PropertyTP GetPropertyTP(string aProperTP)
        {
            var tProperTP = aProperTP.ToLower().Split('<')[0];
            PropertyTP tProper = PropertyTP._nono;
            switch (tProperTP)
            {
                case "bool":
                    tProper = PropertyTP._bool;
                    break;
                case "byte":
                    tProper = PropertyTP._byte;
                    break;
                case "char":
                    tProper = PropertyTP._char;
                    break;
                case "string":
                    tProper = PropertyTP._string;
                    break;
                case "short":
                    tProper = PropertyTP._short;
                    break;
                case "int":
                    tProper = PropertyTP._int;
                    break;
                case "uint":
                    tProper = PropertyTP._uint;
                    break;
                case "long":
                    tProper = PropertyTP._long;
                    break;
                case "ulong":
                    tProper = PropertyTP._ulong;
                    break;
                case "float":
                    tProper = PropertyTP._float;
                    break;
                case "double":
                    tProper = PropertyTP._double;
                    break;
                case "list":
                    {
                        tProper = PropertyTP._list;
                    }
                    break;
                case "dic":
                case "dictionary":
                    {
                        tProper = PropertyTP._dic;
                    }
                    break;
                case "vector3":
                    {
                        tProper = PropertyTP._vc3;
                    }
                    break;
            }

            return tProper;
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

    }

    public enum PropertyTP
    {
        _nono,
        _bool,
        _byte,
        _char,

        _short,
        _string,
        _int,
        _uint,
        _long,
        _ulong,

        _float,
        _double,

        _dic,
        _list,

        _combo,

        _vc3,
    }
}

