using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agava.YandexGames;
using Newtonsoft.Json;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_WEBGL
using UnityEngine;
#endif

namespace Kimicu.YandexGames
{
    public static partial class Billing
    {
        private static CatalogProduct[] _catalogProducts;
        private static GetPurchasedProductsResponse _getPurchasedProductsResponse;

        private static bool _productCatalogSuccesses;
        private static bool _purchasedProductsSuccesses;

        private static bool _relevancePurchaseProductData = false;

        private static IEnumerable<PurchasedProduct> PurchasedProducts =>
            _getPurchasedProductsResponse.purchasedProducts;

        public static bool Initialized { get; private set; } = false;

        public static IEnumerable<CatalogProduct> CatalogProducts => _catalogProducts;
        public static string PurchasedSignature => _getPurchasedProductsResponse.signature;
        
        public static IEnumerator Initialize(Action onSuccessCallback = null)
        {
            if (!YandexGamesSdk.IsInitialized) throw new Exception("YandexGamesSdk not initialized!");
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.GetProductCatalog(OnGetProductCatalogSuccessCallback, OnErrorCallback);
            Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
#else
            OnGetProductCatalogSuccessCallback(LoadProductCatalog());
            OnGetPurchasedProductsSuccessCallback(LoadPurchasedProductsInEditor());
#endif
            yield return new WaitUntil(() => _purchasedProductsSuccesses && _productCatalogSuccesses);
            Initialized = true;
            onSuccessCallback?.Invoke();
        }

        public static void GetPurchasedProducts(Action<PurchasedProduct[]> onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!Initialized) throw new Exception("Billing not initialized!");
            if (_relevancePurchaseProductData)
            {
                onSuccessCallback?.Invoke(PurchasedProducts.ToArray());
                return;
            }

#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.GetPurchasedProducts((response) =>
            {
                OnGetPurchasedProductsSuccessCallback(response);
                onSuccessCallback?.Invoke(response.purchasedProducts.ToArray());
            }, (error) =>
            {
                OnErrorCallback(error);
                onErrorCallback?.Invoke(error);
            });
#else
            OnGetPurchasedProductsSuccessCallback(LoadPurchasedProductsInEditor());
            onSuccessCallback?.Invoke(LoadPurchasedProductsInEditor().purchasedProducts.ToArray());
#endif
        }

        public static void PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, Action<string> onErrorCallback = null, string developerPayload = "")
        {
            if (!Initialized) throw new Exception("Billing not initialized!");
#if !UNITY_EDITOR && UNITY_WEBGL
            _purchasedProductsSuccesses = false;
            _relevancePurchaseProductData = false;
            WebApplication.InPurchaseWindow = true;
            Agava.YandexGames.Billing.PurchaseProduct(productId, (response) =>
            {
                WebApplication.InPurchaseWindow = false;
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
                onSuccessCallback?.Invoke(response);
            }, (error) =>
            {
                WebApplication.InPurchaseWindow = false;
                onErrorCallback?.Invoke(error);
            }, developerPayload);
#else
            _purchasedProductsSuccesses = false;
            _relevancePurchaseProductData = false;
            AddPurchasedProductInEditor(productId);
            OnGetPurchasedProductsSuccessCallback(LoadPurchasedProductsInEditor());
            CatalogProduct catalogProduct = LoadProductCatalog().products.First(i => i.id == productId);
            PurchaseProductResponse purchaseProductResponse = new()
            {
                signature = Guid.NewGuid().ToString(),
                purchaseData = new PurchasedProduct
                {
                    developerPayload = "",
                    purchaseTime = DateTime.Now.ToString("g"),
                    productID = catalogProduct.id,
                    purchaseToken = Guid.NewGuid().ToString()
                }
            };
            onSuccessCallback?.Invoke(purchaseProductResponse);
#endif
        }

        public static void ConsumeProduct(string purchasedProductToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!Initialized) throw new Exception("Billing not initialized!");
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.ConsumeProduct(purchasedProductToken, () =>
            {
                onSuccessCallback?.Invoke();
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
            }, (error) =>
            {
                onErrorCallback?.Invoke(error);
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
            });
#else
            ConsumePurchasedProductInEditor(purchasedProductToken);
            OnGetPurchasedProductsSuccessCallback(LoadPurchasedProductsInEditor());
            onSuccessCallback?.Invoke();
#endif
        }

        private static GetProductCatalogResponse LoadProductCatalog()
        {
            return JsonConvert.DeserializeObject<GetProductCatalogResponse>(PlayerPrefs.GetString("catalog_products",
                JsonConvert.SerializeObject(new GetProductCatalogResponse { products = new CatalogProduct[] { } })));
        }

        private static GetPurchasedProductsResponse LoadPurchasedProductsInEditor()
        {
            return JsonConvert.DeserializeObject<GetPurchasedProductsResponse>(PlayerPrefs.GetString(
                "purchased_products",
                JsonConvert.SerializeObject(new GetPurchasedProductsResponse
                {
                    signature = Guid.NewGuid().ToString(),
                    purchasedProducts = new PurchasedProduct[] { }
                })));
        }

        private static void AddPurchasedProductInEditor(string productId)
        {
            CatalogProduct catalogProduct = LoadProductCatalog().products.First(i => i.id == productId);
            var products = LoadPurchasedProductsInEditor().purchasedProducts.ToList();
            var token = Guid.NewGuid().ToString();
            products.Add(new PurchasedProduct
            {
                developerPayload = "",
                purchaseTime = DateTime.Now.ToString("g"),
                productID = catalogProduct.id,
                purchaseToken = token
            });
            
            PlayerPrefs.SetString("purchased_products", JsonConvert.SerializeObject(new GetPurchasedProductsResponse
            {
                signature = Guid.NewGuid().ToString(), 
                purchasedProducts = products.ToArray()
            }));
            OnGetPurchasedProductsSuccessCallback(LoadPurchasedProductsInEditor());
        }

        private static void ConsumePurchasedProductInEditor(string purchasedProductToken)
        {
            var purchasedProduct = LoadPurchasedProductsInEditor().purchasedProducts.First(i => i.purchaseToken == purchasedProductToken);
            var products = LoadPurchasedProductsInEditor().purchasedProducts.ToList();
            products.RemoveAll(p => p.purchaseToken == purchasedProduct.purchaseToken);

            GetPurchasedProductsResponse saveValue = new()
            {
                signature = Guid.NewGuid().ToString(),
                purchasedProducts = products.ToArray()
            };
            PlayerPrefs.SetString("purchased_products", JsonConvert.SerializeObject(saveValue));
            OnGetPurchasedProductsSuccessCallback(saveValue);
        }
    }
}