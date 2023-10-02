using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Agava.YandexGames;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KiYandexSDK
{
    public static class Billing
    {
        private static List<CatalogProduct> _catalogProduct;
        private static List<PurchasedProduct> _purchasedProducts;

        private static bool _catalogInitialized;
        private static bool _purchasedProductsInitialized;
        private static bool _errored;

        private static Action<PurchaseProductResponse> OnSuccessPurchaseProduct;
        private static Action<string> OnErrorPurchaseProduct;

        private static Action OnSuccessConsumeProduct;
        private static Action<string> OnErrorConsumeProduct;

        /// <summary> Catalog of all products in yandex console. </summary>
        public static IEnumerable<CatalogProduct> CatalogProduct
        {
            get => _catalogProduct;
            private set => _catalogProduct = value.ToList();
        }

        /// <summary> List of purchased goods that are not processed in "ConsumeProduct" </summary>
        /// <remarks> Upon confirmation of purchase, the item will be removed from the list. </remarks>
        public static IEnumerable<PurchasedProduct> PurchasedProducts
        {
            get => _purchasedProducts;
            private set => _purchasedProducts = value.ToList();
        }

        /// <summary> Getting a catalog of products and unprocessed purchases. </summary>
        public static IEnumerator Initialize()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.GetProductCatalog(OnGetProductCatalogSuccess, OnErrorCallback);
            Agava.YandexGames.Billing.GetPurchasedProducts(OnGetPurchasedProductsSuccess, OnErrorCallback);
#else
            OnGetProductCatalogSuccess(null);
            OnGetPurchasedProductsSuccess(null);
#endif
            yield return new WaitUntil(() => (_catalogInitialized && _purchasedProductsInitialized) || _errored);
        }

        private static void OnGetProductCatalogSuccess(GetProductCatalogResponse response)
        {
            if (response != null) CatalogProduct = response.products;
            _catalogInitialized = true;
        }

        private static void OnGetPurchasedProductsSuccess(GetPurchasedProductsResponse response)
        {
            if (response != null) PurchasedProducts = response.purchasedProducts;
            _purchasedProductsInitialized = true;
        }

        private static void OnErrorCallback(string error)
        {
            Debug.LogWarning(error);
            _errored = true;
        }

        /// <summary> Activation of the purchase process. </summary>
        /// <param name="id"> The product ID that is set in the developer console. </param>
        /// <param name="onSuccess"> Successful purchase of the product. </param>
        /// <param name="onError"> Error when buying a product. </param>
        /// <param name="developerPayload">
        /// Optional parameter. Additional information about the purchase that you want to transmit to your server
        /// (will be transmitted in the signature parameter).
        /// </param>
        public static void PurchaseProduct(string id, Action<PurchaseProductResponse> onSuccess,
            Action<string> onError = null, string developerPayload = default)
        {
            OnSuccessPurchaseProduct = response =>
            {
                onSuccess?.Invoke(response);
                _purchasedProducts ??= new List<PurchasedProduct>();
                _purchasedProducts.Add(response.purchaseData);
            };
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.PurchaseProduct(id, OnSuccessPurchaseProduct, onError, developerPayload);
#else
            OnSuccessPurchaseProduct?.Invoke(new PurchaseProductResponse
            {
                purchaseData = new PurchasedProduct
                {
                    developerPayload = "",
                    purchaseToken = id,
                    productID = id,
                    purchaseTime = Time.time.ToString(CultureInfo.InvariantCulture)
                },
                signature = id
            });
#endif
        }

        /// <summary> Activation of the purchase process, as well as its confirmation in the "ConsumeProduct". </summary>
        /// <param name="id"> The product ID that is set in the developer console. </param>
        /// <param name="onSuccessPurchase"> Successful purchase of the product. </param>
        /// <param name="onSuccessConsume"> Successful confirmation of purchase. </param>
        /// <param name="onErrorPurchase"> Error when confirming the product. </param>
        /// <param name="onErrorConsume"> Failed purchase confirmation. </param>
        /// <param name="developerPayload">
        /// Optional parameter. Additional information about the purchase that you want to transmit to your server
        /// (will be transmitted in the signature parameter).
        /// </param>
        /// <remarks> Upon confirmation of purchase, the item will be removed from the list of "Purchased items". </remarks>
        public static void PurchaseProduct(string id, Action<PurchaseProductResponse> onSuccessPurchase = null,
            Action onSuccessConsume = null, Action<string> onErrorPurchase = null, Action<string> onErrorConsume = null,
            string developerPayload = default)
        {
            OnSuccessPurchaseProduct = response =>
            {
                onSuccessPurchase?.Invoke(response);
                _purchasedProducts ??= new List<PurchasedProduct>();
                _purchasedProducts.Add(response.purchaseData);
                ConsumeProduct(response.purchaseData.purchaseToken, onSuccessConsume, onErrorConsume);
            };
            OnErrorPurchaseProduct = onErrorPurchase;

            OnSuccessConsumeProduct = () => { onSuccessConsume?.Invoke(); };
            OnErrorConsumeProduct = onErrorConsume;

#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.PurchaseProduct(id, OnSuccessPurchaseProduct, OnErrorPurchaseProduct,
                developerPayload);
#else
            OnSuccessPurchaseProduct?.Invoke(new PurchaseProductResponse
            {
                purchaseData = new PurchasedProduct
                {
                    developerPayload = "",
                    purchaseToken = id,
                    productID = id,
                    purchaseTime = Time.time.ToString(CultureInfo.InvariantCulture)
                },
                signature = id
            });
#endif
        }

        /// <summary> Confirmation of purchase. </summary>
        /// <param name="purchaseToken"> A token for using the purchase. </param>
        /// <param name="onSuccess"> Successful confirmation of purchase. </param>
        /// <param name="onError"> Failed purchase confirmation. </param>
        /// <remarks> Upon confirmation of purchase, the item will be removed from the list of "Purchased items". </remarks>
        public static void ConsumeProduct(string purchaseToken, Action onSuccess = null, Action<string> onError = null)
        {
            OnSuccessConsumeProduct = () =>
            {
                _purchasedProducts ??= new List<PurchasedProduct>();
                var purchasedProducts = _purchasedProducts
                    .Where(product => product.purchaseToken == purchaseToken)
                    .ToList();
                _purchasedProducts = purchasedProducts;
                onSuccess?.Invoke();
            };
            OnErrorConsumeProduct = onError;
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.ConsumeProduct(purchaseToken, OnSuccessConsumeProduct, OnErrorConsumeProduct);
#else
            OnSuccessConsumeProduct?.Invoke();
#endif
        }
    }
}