# ⚙️ Billing
## Инициализация
Требует перед этим инициализацию в классе [YandexGamesSdk](YandexGamesSdk.md)
```csharp
// ...
yield return Initialize();
// ...
```

## Fields
```csharp
/// Используйте его, чтобы проверить, инициализирован ли Billing.
Billing.IsInitialized; // bool

// ============================================================================= //

/// Используйте для полученя списка продуктов в игре
/// ПРИМЕЧАНИЕ: Для Editor можно также настроить в папке "ProjectFolder"/EditorCloud/catalog.txt
Billing.CatalogProducts; // CatalogProduct[]
```

## Methods
```csharp
/// Используйте для получения не обработанных покупок.
/// ПРИМЕЧАНИЕ: Для Editor можно также настроить в папке "ProjectFolder"/EditorCloud/purchased-products.txt
Billing.GetPurchasedProducts(Action<GetPurchasedProductsResponse> onSuccessCallback, 
    Action<string> onErrorCallback = null); // void

// ============================================================================= //

/// Используйте для покупки продукта.
/// ПРИМЕЧАНИЕ: Для Editor можно также настроить в папке "ProjectFolder"/EditorCloud/purchased-products.txt
Billing.PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, 
            Action<string> onErrorCallback = null, string developerPayload = ""); // void

// ============================================================================= //

/// Используйте для подтверждения покупки.
/// ПРИМЕЧАНИЕ: Для Editor можно также настроить в папке "ProjectFolder"/EditorCloud/purchased-products.txt
Billing.ConsumeProduct(string productToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null); // void
```

## Classes
```csharp
class GetPurchasedProductsResponse
{
    PurchasedProduct[] purchasedProducts;
    string signature;
}

class PurchaseProductResponse
{
    PurchasedProduct purchaseData;
    string signature;
}
class PurchasedProduct
{
    string developerPayload;
    string productID;
    string purchaseTime;
    string purchaseToken;
}


class GetProductCatalogResponse
{
    CatalogProduct[] products;
}

class CatalogProduct {
    string description;
    string id;
    string imageURI;
    string price;
    string priceCurrencyCode;
    string priceValue;
    string priceCurrencyPicture;
    string title;
}
```