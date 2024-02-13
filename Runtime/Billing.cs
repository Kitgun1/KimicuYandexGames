using System;
using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
#if !UNITY_EDITOR && UNITY_WEBGL
using UnityEngine;
#endif

namespace Kimicu.YandexGames
{
    public static class Billing
    {
        private static CatalogProduct[] _catalogProducts;
        private static GetPurchasedProductsResponse _getPurchasedProductsResponse;

        #if !UNITY_EDITOR && UNITY_WEBGL
        private static bool _productCatalogSuccesses;
        private static bool _purchasedProductsSuccesses;
        #endif

        public static bool Initialized { get; private set; } = false;
        
        public static IEnumerable<CatalogProduct> CatalogProducts => _catalogProducts;
        public static IEnumerable<PurchasedProduct> PurchasedProducts => _getPurchasedProductsResponse.purchasedProducts;

        public static string PurchasedSignature => _getPurchasedProductsResponse.signature;

        public static IEnumerator Initialize(Action onSuccessCallback = null)
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.GetProductCatalog(OnGetProductCatalogSuccessCallback, OnErrorCallback);
            Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);

            yield return new WaitUntil(() => _purchasedProductsSuccesses && _productCatalogSuccesses);
            #else
            // TODO: Добавить тестовые покупки для UNITY_EDITOR
            yield return null;
            #endif
            Initialized = true;
            onSuccessCallback?.Invoke();
        }

        #if !UNITY_EDITOR && UNITY_WEBGL
        private static void OnGetProductCatalogSuccessCallback(GetProductCatalogResponse response)
        {
            _catalogProducts = response.products;
            _productCatalogSuccesses = true;
        }

        private static void OnGetPurchasedProductsSuccessCallback(GetPurchasedProductsResponse response)
        {
            _getPurchasedProductsResponse = response;
            _purchasedProductsSuccesses = true;
        }

        private static void OnErrorCallback(string error) => Debug.LogError($"Billing error: {error}");
        #endif

        public static void PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, Action<string> onErrorCallback = null, string developerPayload = "")
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            WebApplication.InPurchaseWindow = true;
            Agava.YandexGames.Billing.PurchaseProduct(productId,(response) =>
            {
                onSuccessCallback?.Invoke(response);
                WebApplication.InPurchaseWindow = false;
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
            }, (error)=>
            {
                onErrorCallback?.Invoke(error);
                WebApplication.InPurchaseWindow = false;
            }, developerPayload);
            #else
            // TODO: Добавить тестовые подтверждения покупок для UNITY_EDITOR
            #endif
        }

        public static void ConsumeProduct(string purchasedProductToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.ConsumeProduct(purchasedProductToken, ()=>
            {
                onSuccessCallback?.Invoke();
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
            }, (error) =>
            {
                onErrorCallback?.Invoke(error);
                Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccessCallback, OnErrorCallback);
            });
            #else
            // TODO: Добавить тестовые покупки для UNITY_EDITOR
            #endif
        }
    }
}