using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class CommonData
{
    public static CommonData Instance;
    /// <summary>
    /// 大数显示小数保留位数
    /// </summary>
    [Header("大数显示小数保留位数")]
    public int LargeNumberKeepBit = 1;
  
}
