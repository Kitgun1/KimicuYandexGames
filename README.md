# KimicuYandexGames

+ [Описание пакета](#описание-проекта)
+ [Связь](#связь)
+ [Поддержать проект](#поддержать-проект)
+ [Как добавить в проект](#как-добавить-в-проект)
+ [Как начать](#как-начать)
+ [Примеры](#примеры)
  + [Реклама](#реклама)
  + [Звук](#звук)
  + [Облачные сохранения](#облачные-сохранения)
    + [Тонкости использования](#тонкости-использования-yandexdata)
  + [Покупки](#покупки)
+ [FAQ](#faq)

---------------------------------------------------------------------------

## Описание проекта:
Этот пакет был разработан специально для быстрого внедрения YandexSDK в проект.

---------------------------------------------------------------------------

## Связь:
Если остались вопросы, пиишите в тг -> `@KimicuK`
или пишите [тут](https://github.com/Kitgun1/KimicuYandexGames/discussions)

---------------------------------------------------------------------------

## Поддержать проект:
Вы можете отправить донат -> 
```
https://www.donationalerts.com/r/kimicu
```
<details>

<summary> QR Code </summary>

![image](ResourcesGit~/donation_qrcode.png)

</details>

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
и добавьте слушателя в `OnInitialize` вызов метода с переходом на сцену с игрой или уровнем.<br>

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
IEnumerable<PurchasedProduct> Billing.PurchasedProducts; 

// Инициализирует покупки 
//(получение всех товаров и покупок, которые не обработаны в "ConsumeProduct")
// По умолчанию вызывается в "YandexSDKInitialize"
Billing.Initialize();

// Вызывает покупку.
Billing.PurchaseProduct(id, onSuccess, onError, developerPayload);

// Вызывает покупку, а также вызывает метод Billing.ConsumeProduct для подтверждения.
Billing.PurchaseProductAndConsume(id, onSuccessPurchase, onSuccessConsume, onErrorPurchase, onErrorConsume, developerPayload);

// Подтверждает покупку и убирает продукт из списка "PurchasedProducts".
Billing.ConsumeProduct();
```

## FAQ
1) Как внедрить в свой проект?
> 1) Создайте или перейдите на загрузочную сцену и поставьте в настройках BuildSettings
> созданную сцену с индексом 0. Эта сцена будет единой точкой входа в игру. <br><br>
> 2) Создайте пустой объект и накиньте на него уже готовый скрипт YandexSDKInitialize, 
> который инициализирует все компоненты SDK за вас. <br><br>
> 3) На скрипте есть событие OnInitialize, которое будет вызвано после инициализации. 
> Вам нужно будет подписать туда свой метод, который будет закидывать на сцену с игрой.

2) Как показать рекламу в игре?
> Для начала определитесь с видом рекламы: <br>
> 1) InterstitialAd - реклама, которую можно пропустить.<br>
> Для вызова используйте метод - `AdvertSDK.InterstitialAd();` <br><br>
> 2) RewardAd - реклама, которую нельзя пропустить, обычно используют для получения
> какой-нибудь награды. <br>
> Для вызова используйте метод - `AdvertSDK.RewardAd();`.
> Не забудьте что-то сделать при просмотре, указав что-то в параметр `onClose`!<br><br>
> 3) StickyAd - Реклама, которая отображается сбоку или снизу экрана. <br>
> Обычно она включена в самом начале игры, но вы можете управлять баннером из кода,
> вызывая метод - `AdvertSDK.StickyAdActive(bool);`

3) Как сделать консумацию? 
> Консумация - это когда игрок после покупки товара не нажимает на **Хорошо!**, а
> обновляет страницу или выходит из игры и из-за этого в коде не успевает вызваться
> событие onSuccess из метода `Billing.PurchaseProduct();`.<br>
> **Модерация ругается на то что деньги сняли, а игроку ничего не выдали.** <br><br>
> Теперь как это решать: <br>
> Сначала нужно где-то в самом начале игры после инициализации SDK,
> например при срабатывании события `OnInitialize` в скрипте `YandexSDKInitialize`,
> пройтись по списку `Billing.PurchasedProducts<PurchasedProduct>`и выдать награду 
> в зависимости от `productID`. После выдачи также нужно вызвать метод 
> `Billing.ConsumeProduct(purchaseToken)`. <br>
> `purchaseToken` - берем из PurchasedProduct.<br>
> Метод `Billing.ConsumeProduct(purchaseToken)` нужно вызвать всегда после 
> `Billing.PurchaseProduct();`.