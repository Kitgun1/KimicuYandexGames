# 1️⃣ Info
### 👉Модули👈
⚙️ **YandexGamesSdk** - Главный класс работы с SDK. <br>
☁️ **Cloud** - Сохранение данных в облако Yandex. <br>
📢 **Advertisement** - Работа с различной рекламой в игре. <br>
💲 **Billing** - Внутриигровые покупки за валюту YAN. <br>
⛄ **Account** - Аккаунт игрока на yandex games. <br>
🌐 **WebApplication** - Состояние игры в браузере. <br>
🫧 **Shortcut** - Иконки на рабочий стол. <br>
📽️ **AdBlock** - Отслеживание включенного AdBlock. <br>
🖥️ **Device** - Отслеживание устройства. <br>

-----
# 🟡 Как добавить в проект:
1) PackageManager > + > Add package from git URL.. <br>
```
https://github.com/Kitgun1/KimicuYandexGames.git
```
3) PackageManager > + > Add package by name... <br>
```
com.unity.nuget.newtonsoft-json
```

-----
# 🟢 Доступ в коде:

using Kimicu.YandexGames;           // Общие модули <br>
using Kimicu.YandexGames.Utils;     // Утилиты <br>
using Kimicu.YandexGames.Extension; // Расширения <br>

-----
# ❓FAQ
По всем вопросам писать в тг - KimcuK

-----
# ⚙️   YandexGamesSdk
## ИНИЦИАЛИЗАЦИЯ
```csharp
IEnumerator YandexGamesSdk.Initialize(onSuccessCallback);
```
## ФУНКЦИИ
Отображает момент, когда игра загрузила все ресурсы и готова к взаимодействию с пользователем:
```cs
void YandexGamesSdk.GameReady();
```
## ЧТО ВАЖНО ЗНАТЬ:
> Вызывать GameReady нужно при запуске игры, но не в самом запуске, а когда будет загружен уровень и игрок сможет уже что-то сделать

-----
# ☁️ Cloud
## ИНИЦИАЛИЗАЦИЯ
```cs
IEnumerator Cloud.Initialize(onSuccessCallback);
```
## ФУНКЦИИ
### Cloud.SetValue();
```cs
Cloud.SetValue(string key, object value, bool saveToCloud, Action onSuccessCallback, Action<string> onErrorCallback);
```
> key - ключ сохранения <br>

> value - значение для сохранения <br>

> saveToCloud - загрузить ли все локальные сохранения в облако <br>

> onSuccessCallback - при удачном сохранении <br>

> onErrorCallback - при возникновении ошибки и при "saveToCloud = true" <br>

## ПРИМЕРЫ
```cs
Cloud.SetValue("money", 15f);
Cloud.SetValue("date", ("September", "12", "2024"));
Cloud.SetValue("data", new Data(), false, () => Debug.Log("Удачно сохранили"));

public class Data
{
  public int level = 15;
  public List<string> buyedSkins = new() { "base", "pro", "VIP" };
}
```
## ПРИМЕЧАНИЕ:
Можно сохранять абсолютно любой объект. <br>
Пример: list, dictionary, int, string, class, struct и любые другие

### Cloud.GetValue();
```cs
<T> Cloud.GetValue(string key, T defaultValue = default);
```
> key - ключ сохранения

> value - значение по умолчанию

## ПРИМЕРЫ
```cs
public class Data
{
  public int level = 15;
  public List<string> buyedSkins = new() { "base", "pro", "VIP" };
  // Любые другие поля
}

float money = Cloud.GetValue("money", 15f);
(string, string, string) date = Cloud.GetValue<(string, string, string)>("date");
Data data = Cloud.GetValue("data", new Data());
```

### Cloud.SaveInCloud();
Используйте этот метод, если хотите отправить локальные изменения на облако
```cs
Cloud.SaveInCloud(Action onSuccessCallback, Action<string> onErrorCallback);
```
> onSuccessCallback - удачно сохранено

> onErrorCallback - ошибка (например слишком часто вызывается)

### ClearCloudData
Используйте этот метод, если хотите очистить все изменения на облаке
```cs
Cloud.ClearCloudData(Action onSuccessCallback, Action<string> onErrorCallback);
```
> onSuccessCallback - удачно удалили все данные

> onErrorCallback - ошибка (например слишком часто вызывается SatValue или ClearCloudData)

## ЧТО ВАЖНО ЗНАТЬ:
> Старайтесь не вызывать SetValue() с saveInCloud = true слишком часто за раз. Например:
```cs
// НЕ ДЕЛАЙТЕ ТАК:
Cloud.SetValue("key1", 15, true);
Cloud.SetValue("key2", 23, true);
Cloud.SetValue("key3", 32, true);
Cloud.SetValue("key4", 43, true);
Cloud.SetValue("key5", 12, true);

// ДЕЛАЙТЕ ТАК:
Cloud.SetValue("key1", 15); // По умолчанию false
Cloud.SetValue("key2", 23); // По умолчанию false
Cloud.SetValue("key3", 32); // По умолчанию false
Cloud.SetValue("key4", 43); // По умолчанию false
Cloud.SetValue("key5", 12); // По умолчанию false
Cloud.SaveInCloud();

// А ЕЩЕ ЛУЧШЕ ТАК:
Cloud.SetValue("key5", _playerData, true); 
_playerData = Cloud.GetValue("key5", new PlayerData());
public class PlayerData
{
  public int Money = 0;
  public List<bool> LevelsAvailable = new();
  public Dictionary<string, bool> Skins = new();
}
```

-----
# 📢 Advertisement
## Инициализация
```cs
Advertisement.Initialize(onSuccessCallback);
```
## Функции
### Показ межстраничной рекламы
```cs
Advertisement.ShowInterstitialAd(Action onOpenCallback = null, Action onCloseCallback = null, Action<string> onErrorCallback = null, Action onOfflineCallback = null);
```
> onOpenCallback - в момент открытия межстраничной рекламы

> onCloseCallback - в момент нажатия на крестик межстраничной рекламы

> onErrorCallback - при возникновении ошибки в межстраничной рекламы 

> onOfflineCallback - вызывается при потере сетевого соединения (переходе в офлайн-режим)

## Примеры
```cs
Advertisement.ShowInterstitialAd();
```
## ПРИМЕЧАНИЕ:
> Рекламу можно вызывать каждую секунду, но покажут ее только через каждые 70 сек. 

### Показ видеорекламы с вознаграждением
```cs
Advertisement.ShowVideoAd(Action onOpenCallback = null, Action onRewardedCallback = null, Action onCloseCallback = null, Action<string> onErrorCallback = null);
```
> onOpenCallback - вызывается при отображении видеорекламы на экране

> onRewardedCallback - вызывается, когда засчитывается просмотр видеорекламы. Укажите в данной функции, какую награду пользователь получит после просмотра.

> onCloseCallback - вызывается при закрытии видеорекламы 

> onErrorCallback - вызывается при возникновении ошибки

## Примеры
```cs
Advertisement.ShowVideoAd(onRewardedCallback: GetReward);

private void GetReward(); // TODO: выдаем любую нужную награду
```
### Показ/Скрытие стики-баннеров
```cs
Advertisement.StickySetActive(bool value);
```
> value - выкл/вкл cтики-баннеры

## ЧТО ВАЖНО ЗНАТЬ:
> Иногда реклама не сразу появляется после ее вызова, поэтому постарайтесь у кнопок, которые вызывают рекламу, выключать возможность кликать на нее и врубать после закрытия onCloseCallback или onErrorCallback

-----
# 💲 Billing 
## Инициализация
```cs
IEnumerator Billing.Initialize(onSuccessCallback);
```
## Поля

### CatalogProducts 
получение списка купленных товаров
### PurchasedSignature 
данные о покупке и подпись для проверки подлинности игрока.

## Функции

### Получение купленных продуктов
```cs
Billing.GetPurchasedProducts(Action<PurchasedProduct[]> onSuccessCallback = null, Action<string> onErrorCallback = null);
```
> PurchasedProduct - хранит в себе данные о купленных товарах

> onSuccessCallback - вызывается при успешной покупки

> onErrorCallback  - вызывается при ошибки

### Пример
```cs
Billing.GetPurchasedProducts(ProcessingPurchasedProducts);

// TODO: что-то делаем с покупками, которые мы еще не обработали
private void ProcessingPurchasedProducts(PurchasedProduct[] products); 
```

### Показ окна покупки продукта
```cs
Billing.PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, Action<string> onErrorCallback = null, string developerPayload = "");
```
> PurchaseProductResponse - данные о купленном продукте

> productId - id товара на странице игры в категории покупки

> onSuccessCallback - вызывается при успешной покупки с информацией продукте

> onErrorCallback  - вызывается при отмене оплаты или другой любой ошибки

> developerPayload - дополнительные данные о покупке

## Пример
```cs
Billing.PurchaseProduct("skin_boss", ProcessingProduct);

// TODO: выдаем награду и консумируем товар
private void ProcessingProduct(PurchaseProductResponse product); 
```
### Консумирование продукта:
```cs
Billing.ConsumeProduct(string purchasedProductToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null);
```
> purchasedProductToken - tokenтовара. Его можно найти в PurchasedProduct, который можно взять при вызове метода PurchaseProduct или GetPurchasedProducts

> onSuccessCallback - вызывается при успешном консумировании продукта

> onErrorCallback  - вызывается при ошибке

## Пример
```cs
Billing.PurchaseProduct("skin_boss", ProcessingProduct);

// TODO: выдаем награду и консумируем товар
private void ProcessingProduct(PurchaseProductResponse product); 
```

## ЧТО ВАЖНО ЗНАТЬ
> При добавлении внутриигровых покупок в игру требуется реализовывать метод консумирования:

> Консумирование - это подтверждения о том что покупка была выдана игроку.

> Для добавления этого метода в вашу игру следует сделать:
> 1) После вызова метода Billing.PurchaseProduct(); Следует в callback onSuccessCallback добавить, после выдачи награды игроку, метод Billing.ConsumeProduct(); 
> 2) При запуске игры в boot сцене требуется после инициализации Billing вызвать метод Billing.GetPurchasedProducts(onSuccessCallback);. После чего в onSuccessCallback нужно пройтись по списку и выдать награду в зависимости от productId. Пример:

```cs
Billing.GetPurchasedProducts(purchaseProducts => {
  int countProducts = purchaseProducts.Length;
  for (int i = 0; i < countProducts; i++) {
    var product = purchaseProducts[i];
    if(product == "noAds") {
      // Вырубаем рекламу в игре
    }
    else if (product == "money5000") {
      // Выдаем игроку 5000 монет
    }
    Billing.ConsumeProduct(product.purchaseToken);
  }
});
```

-----
# ⛄ Account - Аккаунт игрока на yandex games.
Скоро напишу

-----
# 🌐 WebApplication - Состояние игры в браузере.
Скоро напишу

-----
# 🫧 Shortcut - Иконки на рабочий стол.
> Скоро напишу

-----
# 📽️ AdBlock - Отслеживание включенного AdBlock.
> Скоро напишу

-----
# 🖥️ Device - Отслеживание устройства.
> Скоро напишу
