using Agava.YandexGames;
using UnityEngine;

namespace Kimicu.YandexGames
{
    public static partial class Billing
    {
        
        private static void OnGetProductCatalogSuccessCallback(GetProductCatalogResponse response)
        {
            _catalogProducts = response.products;
            _productCatalogSuccesses = true;
        }

        private static void OnGetPurchasedProductsSuccessCallback(GetPurchasedProductsResponse response)
        {
            _getPurchasedProductsResponse = response;
            _purchasedProductsSuccesses = true;
            _relevancePurchaseProductData = true;
        }

        private static void OnErrorCallback(string error) => Debug.LogError($"Billing error: {error}");
    }
}