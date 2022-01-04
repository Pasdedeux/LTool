using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

public class PlayerDataExample 
{
    [PrimaryKey]
    public int ID { get; set; }
    public int Level { get; set; }
    public string Name { get; set; }
    public string Items {
        get {
            if(_Items != null)
            {
               return string.Join(";", _Items.ToArray());
            }
            return "";
        }
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                return;
            }
            if(_Items == null)
            {
                _Items = new List<int>();
            }

            string[] splites = value.Split(';');
            for (int i = 0; i < splites.Length; i++)
            {
                _Items.Add(int.Parse(splites[i]));
            }
        } 
    }
    [ListAttr]
    public List<int> _Items;
}


public class PlayerData
{
    public const string TName = "PlayerData";
}
