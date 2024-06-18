using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Agava.YandexGames;
using Kimicu.YandexGames.Extension;
using UnityEngine;
using Coroutine = Kimicu.YandexGames.Utils.Coroutine;
using GetProductCatalogResponse = Agava.YandexGames.GetProductCatalogResponse;

namespace Kimicu.YandexGames
{
    public static class Billing
    {
        private const string CATALOG_FILE_NAME = "catalog";
        private const string PURCHASED_PRODUCTS_FILE_NAME = "purchased-products";
        
        private static bool _catalogProductsIsActual = false;
        
        private static readonly Coroutine PurchasedProductUpdateCoroutine = new Coroutine();

        public static CatalogProduct[] CatalogProducts { get; private set; }
        public static bool Initialized { get; private set; }

        /// <summary> Initializing billing. </summary>
        /// <exception cref="Exception"> If YandexGamesSdk is not initialized. </exception>
        public static IEnumerator Initialize(ProductPictureSize pictureSize = ProductPictureSize.medium)
        {
            if (!YandexGamesSdk.IsInitialized) throw new Exception("YandexGamesSdk not initialized!");
            if (Initialized) throw new Exception("Billing is initialized!");

#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.GetProductCatalog(pictureSize, SuccessCatalogCallback, OnGetProductCatalogError);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            var catalog = new[] {
                new CatalogProduct {
                    id = "coins_1000_example", title = "1000 монет",
                    description = "Валюта для покупки предметов в магазине.",
                    price = "9 YAN", priceValue = "9", priceCurrencyCode = "YAN",
                    imageURI = "//yastatic.net/s3/games-static/static-data/images/payments/sdk/currency-icon-s@2x.png", 
                    priceCurrencyPicture = "//yastatic.net/s3/games-static/static-data/images/payments/sdk/currency-icon-s@2x.png"
                },
                new CatalogProduct {
                    id = "coins_100_example", title = "100 монет",
                    description = "Валюта для покупки предметов в магазине.",
                    price = "1 YAN", priceValue = "1", priceCurrencyCode = "YAN",
                    imageURI = "//yastatic.net/s3/games-static/static-data/images/payments/sdk/currency-icon-s@2x.png", 
                    priceCurrencyPicture = "//yastatic.net/s3/games-static/static-data/images/payments/sdk/currency-icon-s@2x.png"
                },
            };
            SuccessCatalogCallback(new GetProductCatalogResponse { products = FileExtensions.LoadObject(CATALOG_FILE_NAME, catalog) });
#endif
            yield return new WaitUntil(() => _catalogProductsIsActual);
            Initialized = true;
            yield break;

            void SuccessCatalogCallback(GetProductCatalogResponse response)
            {
                OnGetProductCatalogSuccess(response);
                _catalogProductsIsActual = true;
            }
        }

        /// <summary> Receive purchased and unprocessed purchases via <see cref="ConsumeProduct"/>. </summary>
        /// <param name="onSuccessCallback"> Return response with purchased products </param>
        /// <param name="onErrorCallback"></param>
        /// <returns></returns>
        public static void GetPurchasedProducts(Action<GetPurchasedProductsResponse> onSuccessCallback, Action<string> onErrorCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.GetPurchasedProducts(onSuccessCallback, onErrorCallback);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            var purchasedProducts = FileExtensions.LoadObject(PURCHASED_PRODUCTS_FILE_NAME, new PurchasedProduct[] { });
            onSuccessCallback?.Invoke(new GetPurchasedProductsResponse {
                purchasedProducts =  purchasedProducts,
                signature = Guid.NewGuid().ToString()
            });
#endif
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
            Agava.YandexGames.Billing.PurchaseProduct(productId, onSuccessCallback, onErrorCallback, developerPayload);
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
            var purchasedProducts = FileExtensions.LoadObject(PURCHASED_PRODUCTS_FILE_NAME, new PurchasedProduct[] { }).ToList();
            purchasedProducts.Add(purchasedProduct);
            FileExtensions.SaveObject(PURCHASED_PRODUCTS_FILE_NAME, purchasedProducts.ToArray());
            
            onSuccessCallback?.Invoke(new PurchaseProductResponse()
            {
                purchaseData = purchasedProduct, 
                signature = Guid.NewGuid().ToString()
            });
#endif
        }

        /// <summary> Confirmation of purchased purchase. </summary>
        /// <param name="productToken"> Token of the purchased item. <see cref="GetPurchasedProducts"/> </param>
        /// <param name="onSuccessCallback"> Confirmation was successful, I advise you to issue the reward here. </param>
        /// <param name="onErrorCallback"> Confirmation failed. </param>
        public static void ConsumeProduct(string productToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL // Yandex //
            Agava.YandexGames.Billing.ConsumeProduct(productToken, onSuccessCallback, onErrorCallback);
#endif
#if UNITY_EDITOR && UNITY_WEBGL // Editor //
            var purchasedProducts = FileExtensions.LoadObject(PURCHASED_PRODUCTS_FILE_NAME, new PurchasedProduct[] { });
            var actualPurchasedProducts = purchasedProducts.ToList();
            actualPurchasedProducts.Remove(actualPurchasedProducts.Find(p => p.purchaseToken == productToken));
            FileExtensions.SaveObject(PURCHASED_PRODUCTS_FILE_NAME ,actualPurchasedProducts.ToArray());
            onSuccessCallback?.Invoke();
#endif
        }
        
#region Callbacks

        private static void OnGetProductCatalogSuccess(GetProductCatalogResponse response) => CatalogProducts = response.products;

        private static void OnGetProductCatalogError(string error)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(Billing)}.{nameof(OnGetProductCatalogError)} invoked, {nameof(error)} = {error}");
        }

        
        private static void OnGetPurchasedProductsError(string error)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(Billing)}.{nameof(OnGetPurchasedProductsError)} invoked, {nameof(error)} = {error}");
        }

#endregion
    }
}