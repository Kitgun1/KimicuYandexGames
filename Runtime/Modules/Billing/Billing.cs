using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Agava.YandexGames;
using Kimicu.YandexGames.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Coroutine = Kimicu.YandexGames.Utils.Coroutine;
using GetProductCatalogResponse = Agava.YandexGames.GetProductCatalogResponse;

namespace Kimicu.YandexGames
{
    public static class Billing
    {
        private const string CATALOG_FILE_NAME = "catalog";
        private const string PURCHASED_PRODUCTS_FILE_NAME = "purchased-products";
        
        private static PurchasedProduct[] _purchasedProducts;
        private static bool _purchasedProductsIsActual = false;
        
        private static readonly Coroutine PurchasedProductUpdateCoroutine = new Coroutine();

        public static CatalogProduct[] CatalogProducts { get; private set; }
        public static bool Initialized { get; private set; }

        /// <summary> Initializing billing. </summary>
        /// <exception cref="Exception"> If YandexGamesSdk is not initialized. </exception>
        public static IEnumerator Initialize()
        {
            PurchasedProductUpdateCoroutine.StartRoutine(PurchasedProductActualUpdate());
            if (!YandexGamesSdk.IsInitialized) throw new Exception("YandexGamesSdk not initialized!");

            bool catalogActual = false;
            bool purchasedProductsActual = false;
            
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.GetProductCatalog(SuccessCatalogCallback, OnGetProductCatalogError);
            Agava.YandexGames.Billing.GetPurchasedProducts(SuccessPurchasedProductCallback, OnGetPurchasedProductsError);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            // Catalog Load
            var catalog = new[] {
                new CatalogProduct {
                    id = "coins_1000_example", title = "1000 монет",
                    description = "Валюта для покупки предметов в магазине.",
                    price = "9 YAN", priceValue = "9", priceCurrencyCode = "YAN",
                    imageURI = ""
                },
                new CatalogProduct {
                    id = "coins_100_example", title = "100 монет",
                    description = "Валюта для покупки предметов в магазине.",
                    price = "1 YAN", priceValue = "1", priceCurrencyCode = "YAN",
                    imageURI = ""
                },
            };
            string catalogJson = JsonConvert.SerializeObject(catalog, Formatting.Indented);
            string actualCatalogJson = FileUtility.ReadFile(CATALOG_FILE_NAME, catalogJson, true);
            catalog = JsonConvert.DeserializeObject<CatalogProduct[]>(actualCatalogJson);
            GetProductCatalogResponse catalogResponse = new() { products = catalog };
            SuccessCatalogCallback(catalogResponse);
            
            // Purchased Products Load
            var purchasedProducts = new PurchasedProduct[] { };
            string purchasedProductsJson = JsonConvert.SerializeObject(purchasedProducts, Formatting.Indented);
            string actualPurchasedProductsJson = FileUtility.ReadFile(PURCHASED_PRODUCTS_FILE_NAME, purchasedProductsJson, true);
            purchasedProducts = JsonConvert.DeserializeObject<PurchasedProduct[]>(actualPurchasedProductsJson);
            GetPurchasedProductsResponse purchasedProductsResponse = new()
            {
                purchasedProducts = purchasedProducts,
                signature = Guid.NewGuid().ToString()
            };
            SuccessPurchasedProductCallback(purchasedProductsResponse);
#endif
            yield return new WaitUntil(() => catalogActual && purchasedProductsActual);
            Initialized = true;
            yield break;

            void SuccessCatalogCallback(GetProductCatalogResponse response)
            {
                OnGetProductCatalogSuccess(response);
                catalogActual = true;
            }
            void SuccessPurchasedProductCallback(GetPurchasedProductsResponse response)
            {
                OnGetPurchasedProductsSuccess(response);
                purchasedProductsActual = true;
            }
        }

        public static void GetPurchasedProducts(Action<GetPurchasedProductsResponse> onSuccessCallback, Action<string> onErrorCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.GetPurchasedProducts(SuccessCallback, onErrorCallback);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            string defaultJson = JsonConvert.SerializeObject(new PurchasedProduct[] { });
            string purchasedProductsJson = FileUtility.ReadFile(PURCHASED_PRODUCTS_FILE_NAME, defaultJson, true);
            var purchasedProducts = JsonConvert.DeserializeObject<PurchasedProduct[]>(purchasedProductsJson);
            SuccessCallback(new GetPurchasedProductsResponse {
                purchasedProducts =  purchasedProducts,
                signature = Guid.NewGuid().ToString()
            });
#endif

            void SuccessCallback(GetPurchasedProductsResponse response)
            {
                OnGetPurchasedProductsSuccess(response);
                onSuccessCallback?.Invoke(response);
            }
        }
        
        /// <summary> Causes the purchase of a product. </summary>
        /// <param name="productId"> id product. </param>
        /// <param name="onSuccessCallback"> Successful purchase by clicking on 'Okay' in the shopping menu. </param>
        /// <param name="onErrorCallback"> After canceling a purchase. </param>
        /// <param name="developerPayload"></param>
        /// <remarks> Don't forget to add a call to <see cref="ConsumeProduct"/> in onSuccessCallback. </remarks>
        public static void PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, 
            Action<string> onErrorCallback = null, string developerPayload = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.PurchaseProduct(productId, SuccessCallback, onErrorCallback, developerPayload);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            CatalogProduct catalogProduct = CatalogProducts.FirstOrDefault(p => p.id == productId);
            if (catalogProduct == null)
            {
                onErrorCallback?.Invoke($"Product not found in 'ProjectFolder/EditorCloud/{CATALOG_FILE_NAME}'!");
                return;
            }
            PurchasedProduct purchasedProduct = new()
            {
                productID = catalogProduct.id,
                purchaseToken = Guid.NewGuid().ToString(),
                purchaseTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                developerPayload = developerPayload
            };
            var purchasedProducts = _purchasedProducts.ToList();
            purchasedProducts.Add(purchasedProduct);
            string purchasedProductsJson = JsonConvert.SerializeObject(purchasedProducts, Formatting.Indented);
            FileUtility.EditOrCreateFile(PURCHASED_PRODUCTS_FILE_NAME, purchasedProductsJson);
            SuccessCallback(new PurchaseProductResponse()
            {
                purchaseData = purchasedProduct, 
                signature = Guid.NewGuid().ToString()
            });
#endif
            void SuccessCallback(PurchaseProductResponse response)
            {
                _purchasedProductsIsActual = false;
                onSuccessCallback?.Invoke(response);
            }
        }

        /// <summary> Confirmation of purchased purchase. </summary>
        /// <param name="productToken"> Token of the purchased item. <see cref="GetPurchasedProducts"/> </param>
        /// <param name="onSuccessCallback"> Confirmation was successful, I advise you to issue the reward here. </param>
        /// <param name="onErrorCallback"> Confirmation failed. </param>
        public static void ConsumeProduct(string productToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.ConsumeProduct(productToken, OnSuccessCallback, onErrorCallback);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            string defaultProducts = JsonConvert.SerializeObject(new PurchasedProduct[] { });
            string purchasedProductsJson = FileUtility.ReadFile(PURCHASED_PRODUCTS_FILE_NAME, defaultProducts);
            var purchasedProducts = JsonConvert.DeserializeObject<PurchasedProduct[]>(purchasedProductsJson);
            var actualPurchasedProducts = purchasedProducts.ToList();
            actualPurchasedProducts.Remove(actualPurchasedProducts.Find(p => p.purchaseToken == productToken));
            purchasedProductsJson = JsonConvert.SerializeObject(actualPurchasedProducts, Formatting.Indented);
            FileUtility.EditOrCreateFile(PURCHASED_PRODUCTS_FILE_NAME, purchasedProductsJson);
            OnSuccessCallback();
#endif
            
            void OnSuccessCallback()
            {
                _purchasedProductsIsActual = false;
                onSuccessCallback?.Invoke();
            }
        }
        
#region Callbacks

        private static void OnGetProductCatalogSuccess(GetProductCatalogResponse response) => CatalogProducts = response.products;

        private static void OnGetProductCatalogError(string error)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(Billing)}.{nameof(OnGetProductCatalogError)} invoked, {nameof(error)} = {error}");
        }

        private static void OnGetPurchasedProductsSuccess(GetPurchasedProductsResponse response)
        {
            _purchasedProducts = response.purchasedProducts;
            _purchasedProductsIsActual = true;
        }
        
        private static void OnGetPurchasedProductsError(string error)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(Billing)}.{nameof(OnGetPurchasedProductsError)} invoked, {nameof(error)} = {error}");
        }

#endregion

#region Routines

        private static IEnumerator PurchasedProductActualUpdate()
        {
            while (true)
            {
                yield return new WaitUntil(() => !_purchasedProductsIsActual);
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccess, OnGetPurchasedProductsError);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
                var purchasedProducts = new PurchasedProduct[] { };
                string purchasedProductsJson = JsonConvert.SerializeObject(purchasedProducts, Formatting.Indented);
                string actualPurchasedProductsJson = FileUtility.ReadFile(PURCHASED_PRODUCTS_FILE_NAME, purchasedProductsJson, true);
                purchasedProducts = JsonConvert.DeserializeObject<PurchasedProduct[]>(actualPurchasedProductsJson);
                GetPurchasedProductsResponse purchasedProductsResponse = new()
                {
                    purchasedProducts = purchasedProducts,
                    signature = Guid.NewGuid().ToString()
                };
                OnGetPurchasedProductsSuccess(purchasedProductsResponse);
#endif
            }
        }

#endregion
    }
}