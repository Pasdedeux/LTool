using System;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using LitFramework.Singleton;
using System.Diagnostics;

public class PurchaserConfig : SingletonMono<PurchaserConfig>
{
    public List<PurchaserStoreItem> products;

}

