using System;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using System.Diagnostics;
using LitFramework.Singleton;

public class PurchaserConfig : SingletonMono<PurchaserConfig>
{
    public List<PurchaserStoreItem> products;

}

