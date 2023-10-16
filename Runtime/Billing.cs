using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Agava.YandexGames;
using UnityEngine;

namespace Kimicu.YandexGames
{
    public static class Billing
    {
        #region Fields

        private static List<CatalogProduct> _catalogProduct;
        private static List<PurchasedProduct> _purchasedProducts;

        private static bool _catalogInitialized;
        private static bool _purchasedProductsInitialized;
        private static bool _errored;

        private static Action<PurchaseProductResponse> _onSuccessPurchaseProduct;
        private static Action<string> _onErrorPurchaseProduct;

        private static Action _onSuccessConsumeProduct;
        private static Action<string> _onErrorConsumeProduct;

        #endregion

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
            InitializeEditorCatalog();
            InitializeEditorPurchasedProduct();
#endif
            yield return new WaitUntil(() => (_catalogInitialized && _purchasedProductsInitialized) || _errored);
        }

        #region Editor

        private static void InitializeEditorCatalog()
        {
            OnGetProductCatalogSuccess(new GetProductCatalogResponse
            {
                products = KimicuYandexSettings.Instance.CatalogProductInEditor
            });
        }

        private static void InitializeEditorPurchasedProduct()
        {
            OnGetPurchasedProductsSuccess(new GetPurchasedProductsResponse()
            {
                signature =
                    "hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk=.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZEF0IjoxNTcxMjMzMzcxLCJyZXF1ZXN0UGF5bG9hZCI6InF3ZSIsImRhdGEiOnsidG9rZW4iOiJkODVhZTBiMS05MTY2LTRmYmItYmIzOC02ZDJhNGNhNDQxNmQiLCJzdGF0dXMiOiJ3YWl0aW5nIiwiZXJyb3JDb2RlIjoiIiwiZXJyb3JEZXNjcmlwdGlvbiI6IiIsInVybCI6Imh0dHBzOi8veWFuZGV4LnJ1L2dhbWVzL3Nkay9wYXltZW50cy90cnVzdC1mYWtlLmh0bWwiLCJwcm9kdWN0Ijp7ImlkIjoibm9hZHMiLCJ0aXRsZSI6ItCR0LXQtyDRgNC10LrQu9Cw0LzRiyIsImRlc2NyaXB0aW9uIjoi0J7RgtC60LvRjtGH0LjRgtGMINGA0LXQutC70LDQvNGDINCyINC40LPRgNC1IiwicHJpY2UiOnsiY29kZSI6IlJVUiIsInZhbHVlIjoiNDkifSwiaW1hZ2VQcmVmaXgiOiJodHRwczovL2F2YXRhcnMubWRzLnlhbmRleC5uZXQvZ2V0LWdhbWVzLzE4OTI5OTUvMmEwMDAwMDE2ZDFjMTcxN2JkN2EwMTQ5Y2NhZGM4NjA3OGExLyJ9fX0=",
                purchasedProducts = KimicuYandexSettings.Instance.PurchasedProductInEditor
            });
        }

        private static PurchaseProductResponse GetNewPurchaseProductResponse(string productId) => new()
        {
            signature =
                "hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk=.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZEF0IjoxNTcxMjMzMzcxLCJyZXF1ZXN0UGF5bG9hZCI6InF3ZSIsImRhdGEiOnsidG9rZW4iOiJkODVhZTBiMS05MTY2LTRmYmItYmIzOC02ZDJhNGNhNDQxNmQiLCJzdGF0dXMiOiJ3YWl0aW5nIiwiZXJyb3JDb2RlIjoiIiwiZXJyb3JEZXNjcmlwdGlvbiI6IiIsInVybCI6Imh0dHBzOi8veWFuZGV4LnJ1L2dhbWVzL3Nkay9wYXltZW50cy90cnVzdC1mYWtlLmh0bWwiLCJwcm9kdWN0Ijp7ImlkIjoibm9hZHMiLCJ0aXRsZSI6ItCR0LXQtyDRgNC10LrQu9Cw0LzRiyIsImRlc2NyaXB0aW9uIjoi0J7RgtC60LvRjtGH0LjRgtGMINGA0LXQutC70LDQvNGDINCyINC40LPRgNC1IiwicHJpY2UiOnsiY29kZSI6IlJVUiIsInZhbHVlIjoiNDkifSwiaW1hZ2VQcmVmaXgiOiJodHRwczovL2F2YXRhcnMubWRzLnlhbmRleC5uZXQvZ2V0LWdhbWVzLzE4OTI5OTUvMmEwMDAwMDE2ZDFjMTcxN2JkN2EwMTQ5Y2NhZGM4NjA3OGExLyJ9fX0=",
            purchaseData = new PurchasedProduct
            {
                developerPayload = "IN EDITOR",
                purchaseTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                productID = productId,
                purchaseToken = Guid.NewGuid().ToString()
            }
        };

        #endregion

        #region Callbacks

        private static void OnGetProductCatalogSuccess(GetProductCatalogResponse response)
        {
            CatalogProduct = response?.products;
            _catalogInitialized = true;
        }

        private static void OnGetPurchasedProductsSuccess(GetPurchasedProductsResponse response)
        {
            PurchasedProducts = response?.purchasedProducts;
            _purchasedProductsInitialized = true;
        }

        private static void OnErrorCallback(string error)
        {
            Debug.LogWarning(error);
            _errored = true;
        }

        #endregion

        /// <summary> Activation of the purchase process. </summary>
        /// <param name="productID"> The product ID that is set in the developer console. </param>
        /// <param name="onSuccess"> Successful purchase of the product. </param>
        /// <param name="onError"> Error when buying a product. </param>
        /// <param name="developerPayload">
        /// Optional parameter. Additional information about the purchase that you want to transmit to your server
        /// (will be transmitted in the signature parameter).
        /// </param>
        public static void PurchaseProduct(string productID,
            Action<PurchaseProductResponse> onSuccess = null,
            Action<string> onError = null,
            string developerPayload = default)
        {
            WebProperty.PurchaseWindowOpened = true;
            _onSuccessPurchaseProduct = response =>
            {
                onSuccess?.Invoke(response);
                _purchasedProducts ??= new List<PurchasedProduct>();
                _purchasedProducts.Add(response.purchaseData);
                WebProperty.PurchaseWindowOpened = false;
            };
            _onErrorPurchaseProduct = error =>
            {
                WebProperty.PurchaseWindowOpened = false;
                onError?.Invoke(error);
            };
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.PurchaseProduct(productID, _onSuccessPurchaseProduct, _onErrorPurchaseProduct, developerPayload);
#else
            _onSuccessPurchaseProduct?.Invoke(GetNewPurchaseProductResponse(productID));
#endif
        }

        /// <summary> Activation of the purchase process, as well as its confirmation in the "ConsumeProduct". </summary>
        /// <param name="productID"> The product ID that is set in the developer console. </param>
        /// <param name="onSuccessPurchase"> Successful purchase of the product. </param>
        /// <param name="onSuccessConsume"> Successful confirmation of purchase. </param>
        /// <param name="onErrorPurchase"> Error when confirming the product. </param>
        /// <param name="onErrorConsume"> Failed purchase confirmation. </param>
        /// <param name="developerPayload">
        /// Optional parameter. Additional information about the purchase that you want to transmit to your server
        /// (will be transmitted in the signature parameter).
        /// </param>
        /// <remarks> Upon confirmation of purchase, the item will be removed from the list of "Purchased items". </remarks>
        public static void PurchaseProductAndConsume(string productID,
            Action<PurchaseProductResponse> onSuccessPurchase = null,
            Action onSuccessConsume = null,
            Action<string> onErrorPurchase = null,
            Action<string> onErrorConsume = null,
            string developerPayload = default)
        {
            WebProperty.PurchaseWindowOpened = true;
            _onSuccessPurchaseProduct = response =>
            {
                onSuccessPurchase?.Invoke(response);
                WebProperty.PurchaseWindowOpened = false;
                _purchasedProducts ??= new List<PurchasedProduct>();
                _purchasedProducts.Add(response.purchaseData);
                ConsumeProduct(response.purchaseData.purchaseToken, onSuccessConsume, onErrorConsume);
            };

            _onErrorPurchaseProduct = error =>
            {
                WebProperty.PurchaseWindowOpened = false;
                onErrorPurchase?.Invoke(error);
            };

            _onSuccessConsumeProduct = () => { onSuccessConsume?.Invoke(); };
            _onErrorConsumeProduct = onErrorConsume;

#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.PurchaseProduct(productID, _onSuccessPurchaseProduct, _onErrorPurchaseProduct,
                developerPayload);
#else
            _onSuccessPurchaseProduct?.Invoke(GetNewPurchaseProductResponse(productID));
#endif
        }

        /// <summary> Confirmation of purchase. </summary>
        /// <param name="purchaseToken"> A token for using the purchase. </param>
        /// <param name="onSuccess"> Successful confirmation of purchase. </param>
        /// <param name="onError"> Failed purchase confirmation. </param>
        /// <remarks> Upon confirmation of purchase, the item will be removed from the list of "Purchased items". </remarks>
        public static void ConsumeProduct(string purchaseToken, Action onSuccess = null, Action<string> onError = null)
        {
            _onSuccessConsumeProduct = () =>
            {
                _purchasedProducts ??= new List<PurchasedProduct>();
                _purchasedProducts.Remove(_purchasedProducts
                    .First(product => product.purchaseToken == purchaseToken));
                onSuccess?.Invoke();
            };
            _onErrorConsumeProduct = onError;
#if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.Billing.ConsumeProduct(purchaseToken, OnSuccessConsumeProduct, OnErrorConsumeProduct);
#else
            _onSuccessConsumeProduct?.Invoke();
#endif
        }
    }
}