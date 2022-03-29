using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLocalData : LocalDataBase
{
    public long CreateAccountTime = 0L;
    
    [Header("最近本地化时间(毫秒)")]
    //[SerializeField]
    public long localSaveTime;
}
