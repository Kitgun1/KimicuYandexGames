# KimicuYandexGames

|      Languages      |
|:-------------------:|
| [Russian](#русский) |
| [English](#english) |

# Русский:
+ [Описание пакета](#описание-проекта)
+ [Как добавить в проект](#как-добавить-в-проект)
+ [Как начать](#как-начать)
+ [Примеры](#примеры)
  + [Реклама](#реклама)
  + [Звук](#звук)
  + [Облачные сохранения](#облачные-сохранения)
    + [Тонкости использования](#тонкости-использования-yandexdata)
  + [Покупки](#покупки)

---------------------------------------------------------------------------

## Описание проекта:
Этот пакет был разработан специально для быстрого внедрения YandexSDK в проект.

---------------------------------------------------------------------------

## Как добавить в проект:
Жмем на `+` и выбрать `Add package from git URL...` и вставить эти дополнительные ссылки:
<br>``` https://github.com/forcepusher/com.agava.yandexmetrica.git ```
<br>``` https://github.com/forcepusher/com.agava.webutility.git ```
<br>``` https://github.com/forcepusher/com.agava.yandexgames.git ```
<br>``` https://github.com/Kitgun1/KimicuYandexGames.git ```
<br> Далее добавим [KimicuUtility](https://github.com/Kitgun1/KimicuUtility)

Также не забудьте использовать TextMeshPro в вашем проекте.

## Как начать:
Создайте новую загрузочную сцену > создайте пустой объект > добавьте компонент `YandexSDKInitialize`
и добавьте в `OnInitialize` вызов метода с переходом на сцену с игрой или уровнем.<br>

## Примеры
Откройте `Package Manager` > Выберите `KimicuYandexGames` в разделе `Packages: In Project` >
Выберите вкладку `Samples` и нажмите `Import` > открыть импортированные сцены.

### Реклама

```csharp
using KiYandexSDK;

// Для показа рекламы InterstitialAd: 
// Если реклама будет вызвана и при этом она будет отключена, рекламу не покажут + будет вызван onOpen и onClose
AdvertSDK.InterstitialAd(Action onOpen = null, Action<bool> onClose = null, Action<string> onError = null, Action onOffline = null);

// Для показа рекламы с наградой: 
// Если реклама будет вызвана и при этом она будет отключена, рекламу не покажут + будет вызван onRewarded и onClose
AdvertSDK.RewardAd(Action onOpen = null, Action onRewarded = null, Action onClose = null, Action<string> onError = null)

// Для включения или отключения баннера:
AdvertSDK.StickyAdActive(bool value)

// Для отключения определенной рекламы в игре (по умолчанию InterstitialAd):
AdvertSDK.AdvertOff(advertType = AdvertType.InterstitialAd)

```
Не нужно сохранять отключение рекламы после вызова `AdvertSDK.AdvertOff()`. <br>
Сохранение и загрузка произойдет автоматически. <br>
По умолчанию id для отключения рекламы - "ADVERT_OFF" <br>
Если нужно изменить id, то можно использовать поле AdvertOffKey
```csharp
using KiYandexSDK;

AdvertSDK.InterAdvertOffKey = "ключ"; // "INTER_ADVERT_OFF";
AdvertSDK.RewardAdvertOffKey = "ключ"; // "REWARD_ADVERT_OFF";
AdvertSDK.StickyAdvertOffKey = "ключ"; // "STICKY_ADVERT_OFF";
```

### Звук
Для отключения звука при показе рекламы или при открытии покупки больше ничего делать не надо, т.к. уже все сделано. 
Просто в окне `Project Settings` > `Player` > `Resolution and Presentation` включите галочку `Run In Background`

### Облачные сохранения
Для сохранения есть всего 2 метода:
```csharp
using KiYandexSDK;

void YandexData.Save(string key, JToken value, Action onSuccess = null, Action<string> onError = null)
JToken YandexData.Load(string key, JToken defaultValue)
// JToken это почти любой тип: int, float, bool, string и тд.
```
Например, для сохранения/получения нужно писать:
```csharp
using KiYandexSDK;

// Bool:
YandexData.Save("example_key", true); // Сохранить
bool myBool = (bool)YandexData.Load("example_key", false/true); // Получить

// Float:
YandexData.Save("example_key", 1f); // Сохранить
bool myBool = (bool)YandexData.Load("example_key", 0f); // Получить
```

#### Тонкости использования YandexData
Следите за типом, который указываете в аргумент:
```csharp
using KiYandexSDK;

int valueInt = 10;
float valueFloat = 5f;
string valueString = "Привет";

int result1 = (int)YandexData.Load("key1", valueInt); 
int result2 = (float)YandexData.Load("key1", valueFloat);
int result3 = (string)YandexData.Load("key1", valueString);
```
В этом примере сохраняются 3 разных типа под одним ключом, но из-за того что тип у defaultValue всегда разный, 
в переменные result будут получены значения из разных ячеек.<br>
Например в примере выше переменные result будут равны:<br>
result1 -> 10,<br>
result2 -> 5f,<br>
result3 -> "Привет"

### Покупки
Для работы с покупками используйте статический класс Billing в пространсве имен `KiYandexSDK`.<br>
Список того что можно использовать:
```csharp
using KiYandexSDK;

// Список всех товаров.
IEnumerable<CatalogProduct> Billing.CatalogProduct; 

// Список купленных и не обработанных товаров в методе "ConsumeProduct".
IEnumerable<PurchasedProduct> PurchasedProducts; 

// Инициализирует покупки 
//(получение всех товаров и покупок, которые не обработаны в "ConsumeProduct")
// По умолчанию вызывается в "YandexSDKInitialize"
Billing.Initialize();

// Вызывает покупку.
Billing.PurchaseProduct(id, onSuccess, onError, developerPayload);

// Вызывает покупку, а также вызывает метод Billing.ConsumeProduct для подтверждения.
Billing.PurchaseProduct(id, onSuccessPurchase, onSuccessConsume, onErrorPurchase, onErrorConsume, developerPayload);

// Подтверждает покупку и убирает продукт из списка "PurchasedProducts".
Billing.ConsumeProduct();
```