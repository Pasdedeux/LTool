using System;
using System.Collections.Generic;
using SQLite;
public class SQLReader 
{
    public const string SINGLE               =  "select {0} from {1} where {3} = {2}";
    public const string SINGLE_ROW           =  "select * from {0} where {2} = {1}";
    public const string COUNT                =  "select count(*) from {0}";
    public const string SELECT_CLOWS         =  "select {0} from {1}";
    public const string SELECT_CLOWS_ORDERBY =  "select {0} from {1} order by {0}";
    public const string SELECT_ID_WHERE = "select {0} from {1} where {2}";
}
public class CSVAttribute : IgnoreAttribute
{

}
/// <summary>
/// igron dictionary when deserilised this property
/// </summary>
public class DictionaryAttr : CSVAttribute
{

}
/// <summary>
/// igron list when deserilised this property
/// </summary>
public class ListAttr : CSVAttribute
{

}
