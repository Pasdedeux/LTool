﻿using System;
using System.Collections.Generic;
using UnityEngine;
using LitFramework;
using System.Diagnostics;

#if IAP
using UnityEngine.Purchasing;
#endif

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : SingletonMono<Purchaser>
#if IAP
    , IStoreListener
#endif
{
    public List<string> products;
    public List<string> fakePrice;
    public List<string> fakeDesc;
    public List<string> fakeName;
    public List<string> Rewards;
    public List<Sprite> Icons;
#if IAP
    public event Action<ushort> ProcessPurchaseFailEventHandler;
    public event Action<string> ProcessPurchaseEventHandler;
    public event Action<ProductCollection> InitializedEventHandler;
    public event Action<string, string, int, Receipt> ProcessPurchaseReceiptEventHandler;

    public string product_removeAds = "";
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public static string kProductIDConsumable = "consumable";
    public static string kProductIDNonConsumable = "nonconsumable";
    public static string kProductIDSubscription = "subscription";

    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "";//"com.unity3d.subscription.new";
    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "";//"com.unity3d.subscription.original";

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if ( m_StoreController == null )
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        LDebug.Log( "====> If we have already connected to Purchasing ..." );
        if ( IsInitialized() )
        {
            // ... we are done here.
            LDebug.Log( "====> ....we are done here." );
            return;
        }


        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance() );

        if ( Application.platform == RuntimePlatform.IPhonePlayer ||
           Application.platform == RuntimePlatform.OSXPlayer )
        {
            for ( int i = 0; i < products.Count; i++ )
            {
                builder.AddProduct( products[ i ], ProductType.Consumable, new IDs
                {
                    {products[i], AppleAppStore.Name }
                } );
            }
        }
        //Android && PC 
        else
        {
            for ( int i = 0; i < products.Count; i++ )
            {
                builder.AddProduct( products[ i ], ProductType.Consumable, new IDs
                {
                    {products[i], GooglePlay.Name }
                } );
            }
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize( this, builder );
    }
    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    //public void BuyConsumable()
    //{
    //    // Buy the consumable product using its general identifier. Expect a response either 
    //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //    BuyProductID( kProductIDConsumable );
    //}
    //public void BuyNonConsumable()
    //{
    //    // Buy the non-consumable product using its general identifier. Expect a response either 
    //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //    BuyProductID( kProductIDNonConsumable );
    //}
    //public void BuySubscription()
    //{
    //    // Buy the subscription product using its the general identifier. Expect a response either 
    //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //    // Notice how we use the general product identifier in spite of this ID being mapped to
    //    // custom store-specific identifiers above.
    //    BuyProductID( kProductIDSubscription );
    //}


    public void BuyProductID( string productId )
    {
        // If Purchasing has been initialized ...
        if ( IsInitialized() )
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID( productId );

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if ( product != null && product.availableToPurchase )
            {
                LDebug.Log( string.Format( "Purchasing product asychronously: '{0}'", product.definition.storeSpecificId ) );
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase( product );
            }
            // Otherwise ...
            else
            {
                ProcessPurchaseFailEventHandler?.Invoke( 1 );
                // ... report the product look-up failure situation  
                LDebug.Log( "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase" );
            }
        }
        // Otherwise ...
        else
        {
            ProcessPurchaseFailEventHandler?.Invoke( 0 );
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            LDebug.Log( "BuyProductID FAIL. Not initialized." );
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if ( !IsInitialized() )
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            LDebug.Log( "RestorePurchases FAIL. Not initialized." );
            return;
        }

        // If we are running on an Apple device ... 
        if ( Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer )
        {
            // ... begin restoring purchases
            LDebug.Log( "RestorePurchases started ..." );

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions( ( result ) =>
             {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                LDebug.Log( "RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore." );
             } );
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            LDebug.Log( "RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform );
        }
    }




    #region 回调
    //  
    // --- IStoreListener
    //
    public void OnInitialized( IStoreController controller, IExtensionProvider extensions )
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        LDebug.Log( "OnInitialized: PASS" );

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        if ( InitializedEventHandler != null ) InitializedEventHandler( m_StoreController.products );
    }


    public void OnInitializeFailed( InitializationFailureReason error )
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        LDebug.Log( "OnInitializeFailed InitializationFailureReason:" + error );
    }

    //购买不同商品结束后的处理方法 对应定义的商品
    public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs args )
    {
        for ( int i = 0; i < products.Count; i++ )
        {
            // A consumable product has been purchased by this user.
            if ( String.Equals( args.purchasedProduct.definition.id, products[ i ], StringComparison.Ordinal ) )
            {
                LDebug.Log( string.Format( "ProcessPurchase: Succeed : '{0}'", args.purchasedProduct.definition.id ) );

                if ( ProcessPurchaseEventHandler != null ) ProcessPurchaseEventHandler( products[ i ] );

                var product = m_StoreController.products.WithID( products[ i ] );
                string receipt = product.receipt;
                string currency = product.metadata.isoCurrencyCode;
                int amount = decimal.ToInt32( product.metadata.localizedPrice * 100 );
                Receipt receiptClass = JsonUtility.FromJson<Receipt>( receipt );
                if ( ProcessPurchaseReceiptEventHandler != null ) ProcessPurchaseReceiptEventHandler( currency, products[ i ], amount, receiptClass );

                // Return a flag indicating whether this product has completely been received, or if the application needs 
                // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
                // saving purchased products to the cloud, and when that save is delayed. 
                return PurchaseProcessingResult.Complete;
            }
        }

        LDebug.Log( string.Format( "ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id ) );

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Pending;
    }

    public void OnPurchaseFailed( Product product, PurchaseFailureReason failureReason )
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        LDebug.Log( string.Format( "OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason ) );
    }

    #endregion
#endif
}

//..
public class Receipt
{

    public string Store;
    public string TransactionID;
    public string Payload;

    public Receipt()
    {
        Store = TransactionID = Payload = "";
    }

    public Receipt( string store, string transactionID, string payload )
    {
        Store = store;
        TransactionID = transactionID;
        Payload = payload;
    }
}

public class PayloadAndroid
{
    public string json;
    public string signature;

    public PayloadAndroid()
    {
        json = signature = "";
    }

    public PayloadAndroid( string _json, string _signature )
    {
        json = _json;
        signature = _signature;
    }

}
