/*======================================
* 项目名称 ：Assets.Scripts.SDKs
* 项目描述 ：
* 类 名 称 ：PurchaserDataModel
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.SDKs
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/5/5 15:48:08
* 更新时间 ：2019/5/5 15:48:08
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/5/5 15:48:08
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using LitFramework;
using LitFramework.Mono;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaserDataModel : Singleton<PurchaserDataModel>, IDisposable
{
    public const string NOADS_ITEM_ID ="";

    public Dictionary<string, StoreItem> ProductsDict;
    private Dictionary<string, string> _dicountItem = new Dictionary<string, string>()
    {
        //{ DISCOUNT_ID,"com.aha.mahjonganimaltour11" }
    };


    public PurchaserDataModel()
    {
#if IAP
        Purchaser.Instance.InitializedEventHandler += Initialized;
        Purchaser.Instance.ProcessPurchaseFailEventHandler += BuyFailEventHandler;
        Purchaser.Instance.ProcessPurchaseEventHandler += ProcessPurchase;
        Purchaser.Instance.ProcessPurchaseReceiptEventHandler += ProcessPurchaseReceipt;
#endif
    }

    private void BuyFailEventHandler( ushort obj )
    {
        switch ( obj )
        {
            //未初始化
            case 0:
                DataModel.Instance.buyOKContents = LanguageModel.Instance.GetString( "Purchase not initialized" );
                break;
            //商品未找到或未初始化
            case 1:
                DataModel.Instance.buyOKContents = LanguageModel.Instance.GetString( "Not purchasing product, not found or not available" );
                break;
            default:
                DataModel.Instance.buyOKContents = LanguageModel.Instance.GetString( "Not purchasing product, not found or not available" );
                break;
        }
        UIManager.Instance.Show( DataModel.UI.UI_BUYOK );
    }

    public void LoadShop()
    {
        ProductsDict = new Dictionary<string, StoreItem>();
        var products = Purchaser.Instance.products;
        var fakePrices = Purchaser.Instance.fakePrice;
        var fakeNames = Purchaser.Instance.fakeName;
        var fakeDesc = Purchaser.Instance.fakeDesc;
        var rewards = Purchaser.Instance.Rewards;

        for ( int i = 0; i < products.Count; i++ )
        {
            StoreItem si = new StoreItem( products[ i ] );
            si.AddStoreItemEventHandler += AddShopItem;
            si.Description = fakeDesc[ i ];
            si.Name = fakeNames[ i ];
            si.Price = fakePrices[ i ];
            if ( Purchaser.Instance.Icons.Count > i )
                si.Icon = Purchaser.Instance.Icons[ i ];

            var reward = rewards[ i ];
            if ( !string.IsNullOrEmpty( reward ) )
                reward.Split( '|' ).ToList().ForEach( e => { var result = e.Split( '-' ); si.Rewards.Add( short.Parse( result[ 0 ] ), ushort.Parse( result[ 1 ] ) ); } );

            if ( !ProductsDict.ContainsKey( products[ i ] ) )
                ProductsDict.Add( products[ i ], si );
            else
                ProductsDict[ products[ i ] ] = si;

            //指定的打折商品处理
            if ( _dicountItem.ContainsKey( products[ i ] ) )
            {
                ProductsDict[ _dicountItem[ products[ i ] ] ].AlternativeBuyID = products[ i ];
                ProductsDict[ _dicountItem[ products[ i ] ] ].AlternativePrice = fakePrices[ i ];
                continue;
            }
        }
    }

    public void Dispose()
    {
#if IAP
        Purchaser.Instance.InitializedEventHandler -= Initialized;
        Purchaser.Instance.ProcessPurchaseEventHandler -= ProcessPurchase;
        Purchaser.Instance.ProcessPurchaseFailEventHandler -= BuyFailEventHandler;
        Purchaser.Instance.ProcessPurchaseReceiptEventHandler -= ProcessPurchaseReceipt;
#endif
    }

#if IAP
    private void Initialized( ProductCollection productCollection )
    {
        LDebug.Log( "IAP total count ==>" + productCollection.all.Length );

        for ( int i = 0; i < productCollection.all.Length; i++ )
        {
            var product = productCollection.all[ i ];

            LDebug.Log( "IAP product storeSpecificId ==>" + product.definition.storeSpecificId );
            LDebug.Log( "IAP availableToPurchase ==>" + product.availableToPurchase );

            //指定的打折商品处理，打折商品肯定要放到列表最后
            if ( _dicountItem.ContainsKey( product.definition.storeSpecificId ) )
            {
                ProductsDict[ _dicountItem[ product.definition.storeSpecificId ] ].AlternativeBuyID = product.definition.storeSpecificId;
                ProductsDict[ _dicountItem[ product.definition.storeSpecificId ] ].AlternativePrice = product.metadata.localizedPriceString;
                continue;
            }

            if ( ProductsDict.ContainsKey( product.definition.storeSpecificId ) )
            {
                if ( !product.definition.storeSpecificId.StartsWith( "buy" ) )
                {
                    ProductsDict[ product.definition.storeSpecificId ].Name = product.metadata.localizedTitle;
                    ProductsDict[ product.definition.storeSpecificId ].Price = product.metadata.localizedPriceString;
                    ProductsDict[ product.definition.storeSpecificId ].Description = product.metadata.localizedDescription;
                    ProductsDict[ product.definition.storeSpecificId ].ProductType = product.definition.type;
                }
            }
            else
            {
                ProductsDict.Add( product.definition.storeSpecificId, new StoreItem( product.definition.storeSpecificId )
                {
                    Name = product.metadata.localizedTitle,
                    Price = product.metadata.localizedPriceString,
                    Description = product.metadata.localizedDescription,
                    ProductType = product.definition.type,
                } );
            }
            LDebug.Log( "IAP localizedTitle ==>" + product.metadata.localizedTitle );
            LDebug.Log( "IAP storeSpecificId ==>" + product.definition.storeSpecificId );
        }
    }
#endif

    private void ProcessPurchase( string productID )
    {
        //商品购买成功逻辑
        if ( ProductsDict.ContainsKey( productID ) )
            ProductsDict[ productID ].IsBought = true;
        else if ( _dicountItem.ContainsKey( productID ) )
            ProductsDict[ _dicountItem[ productID ] ].IsBought = true;

        AddShopItem( productID );

        LitFramework.Input.InputControlManager.Instance.IsEnable = true;
    }

#if IAP
    private void ProcessPurchaseReceipt( string currency, string productID, int amount, Receipt receiptClass )
    {
#if UNITY_ANDROID
        PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>( receiptClass.Payload );
#if GA
        GameAnalyticsSDK.GameAnalytics.NewBusinessEventGooglePlay(currency, amount, "my_item_type", productID, "my_cart_type", receiptPayload.json, receiptPayload.signature);
#endif
#endif
#if UNITY_IOS
            string receiptPayload = receiptClass.Payload;
#if GA
            GameAnalyticsSDK.GameAnalytics.NewBusinessEventIOS( currency, amount, "my_item_type", productID, "my_cart_type", receiptPayload );
#endif
#endif
    }
#endif

    public event Action DelUpdateShopStatus;
    public List<int> boughtIndex = new List<int>();

    public void AddShopItem( string productID, bool isConfig = false )//, ProductMetadata meta = null)
    {
        LDebug.Log( "========>购买了商品 " + productID );
        //StatisticManager.Instance.DOT( "shop_buy_" + Purchaser.Instance.products.IndexOf( productID ) );

        //移除广告商品
        if ( productID == NOADS_ITEM_ID ) AdManager.Instance.RemoveAds();
       //TODO 这里添加根据不同商品增加特别处理规则
        else
        {
            StoreItem result = null;
            if ( ProductsDict.ContainsKey( productID ) )
                result = ProductsDict[ productID ];
            else if ( _dicountItem.ContainsKey( productID ) )
                result = ProductsDict[ _dicountItem[ productID ] ];

            if ( result != null )
            {
                DataModel.Instance.ResolveRewards( result.Rewards );
                //打折商品处理
                CheckDiscount( result, productID );
            }

            //TODO 这里添加根据不同商品增加特别处理规则
            //if ( productID == GOLD_PACK )
            //{
            //    AdManager.Instance.RemoveAds();
            //    DataModel2.Instance.UseGoldenEye = true;
            //}
        }
        UpdateShopStatus();

        AudioManager.Instance.PlaySE( DataModel.Sound.Sound_ShopSucc );
        DataModel.Instance.buyOKContents = DataModel.BUYOKCONTENTS;
        UIManager.Instance.Show( DataModel.UI.UI_BUYOK );
    }

    private void CheckDiscount( StoreItem result, string productID )
    {
        if ( result.BuyID != productID && result.AlternativeBuyID == productID )
        {
            //TODO 这里添加根据不同商品增加特别处理规则
            //switch ( productID )
            //{
            //    case DISCOUNT_ID:
            //        //DataModel2.Instance.UseDiscountSilverPack = true;
            //        break;
            //    default:
            //        break;
            //}
        }
    }


    public void UpdateShopStatus( bool needScale = true )
    {
        DelUpdateShopStatus?.Invoke();
    }

    /// <summary>
    /// 获取去广告价格
    /// 保持在商品列表中得最后一个
    /// </summary>
    /// <returns></returns>
    public string GetRemoveAdsPrice()
    {
        return ProductsDict[ NOADS_ITEM_ID ].Price;
    }

    public string GetShopPrice( string id )
    {
        return LanguageModel.Instance.GetString( ProductsDict[ id ].Price );
    }

    public string GetShopName( string id )
    {
        return LanguageModel.Instance.GetString( ProductsDict[ id ].Name );
    }

    public string GetShopDes( string id )
    {
        return LanguageModel.Instance.GetString( ProductsDict[ id ].Description );
    }
}

