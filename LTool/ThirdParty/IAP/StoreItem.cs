/*======================================
* 项目名称 ：LitFramework.ThirdParty.IAP
* 项目描述 ：
* 类 名 称 ：StoreItem
* 类 描 述 ：
* 命名空间 ：LitFramework.ThirdParty.IAP
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/7/8 16:33:40
* 更新时间 ：2019/7/8 16:33:40
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/7/8 16:33:40
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework
{

    public class StoreItem : IDisposable
    {
        public Sprite Icon;
        public string Name;
        public string BuyID;
        public string Price;
        public string Description;
        public string PriceDiscount;

        public event Action<string, bool> AddStoreItemEventHandler;
        public bool IsBought
        {
            get { return PlayerPrefs.GetInt( "ShopItem_" + BuyID, 0 ) == 1; }
            set { PlayerPrefs.SetInt( "ShopItem_" + BuyID, ( value ? 1 : 0 ) ); }
        }

        public StoreItem( string buyID )
        {
            BuyID = buyID;
            if ( IsBought ) AddStoreItemEventHandler?.Invoke( buyID, true );
        }

        public void Dispose()
        {
            Icon = null;
            Price = null;
            Name = null;
            BuyID = null;
            Description = null;
            PriceDiscount = null;
            AddStoreItemEventHandler = null;
        }
    }

}