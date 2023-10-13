# Kimicu Yandex

## Для чего нужен Kimicu Yandex

Пакет был разработан специально для облегчения работы с
[**SDK от Agava**](https://github.com/forcepusher/com.agava.yandexgames). <br>
<details>
<summary>Список функций:</summary>

1) Упрощённая работа с рекламой.
2) Упрощённая работа с состояниями игры (во вкладке, в рекламе, в покупке).
3) Упрощённая работа с покупками.
4) Упрощённая работа с облачными сохранениями.

</details>

## Как можно поддержать проект

Вы можете отправить донатик по **[ссылке](https://www.donationalerts.com/r/kimicu)**.

## Обратная связь

Вы можете написать в телеграм по нику **@KimicuK** или **[тут](https://github.com/Kitgun1/KimicuYandexGames/discussions)
**

## Как добавить в проект

Открыть `Package Manager` > `+` > `Add package from git URL...` <br>
Вставить по очереди все ссылки:
<br>``` https://github.com/forcepusher/com.agava.yandexmetrica.git ``` - метрика (не обязательно)
<br>``` https://github.com/forcepusher/com.agava.webutility.git ``` - web утилита (обязательно)
<br>``` https://github.com/forcepusher/com.agava.yandexgames.git ``` - sdk для yandex games (обязательно)
<br>``` https://github.com/Kitgun1/KimicuYandexGames.git ``` - [облегчает работу с sdk](#для-чего-нужен-kimicu-yandex) (
обязательно)
<br> Следуя инструкциям, добавляем - **[KimicuUtility](https://github.com/Kitgun1/KimicuUtility)** (обязательно)

## Гайд по использованию

### Что нужно для начала

Для начала нужно сделать инициализацию `SDK` и всех компонентов `Kimicu Yandex`. <br>
Создаем новую или переходим уже существующую `boot` сцену.
> **Она должна быть с 0 индексом в `BuildSettings`!**

Далее нужно создать `empty object` и добавить компонент `YandexSDKInitialize`.
В параметрах есть 2 поля:

* `Initialize Delay` - Время, через которое будет вызван `UnityEvent` `OnInitialize`. **Только в Editor!**
* `OnInitialize` - `UnityEvent`, который вызовет `Invoke` после инициализации SDK и компонентов `Kimicu Yandex`

Добавляем слушателя в `OnInitialize`, который будет что-нибудь делать после инициализации `SDK`, например:

1) Добавляем консумирование (об этом чуть позже)
2) Переходим на сцену с игрой

### Добавление рекламы

Для работы с внутриигровыми покупками используйте статический класс: `AdvertSDK` из пространства
имен `Kimicu.YandexGames`. <br>

Есть несколько типов рекламы на yandex games:

1) [Полноэкранные блоки](https://yandex.ru/dev/games/doc/ru/console/adv-monetization#polnoekrannye-bloki) -
   Блоки с рекламой, которые полностью закрывают фон приложения и показываются в определенные моменты
   (например, при переходе на следующий уровень игры).
2) [Rewarded-блоки (блоки с вознаграждением)](https://yandex.ru/dev/games/doc/ru/console/adv-monetization#rewarded-bloki-bloki-s-voznagrazhdeniem) -
   Блоки с рекламой, которые пользователь может просматривать по своему желанию и получать за просмотр награду
   или внутриигровую валюту.
3) [Sticky-баннеры](https://yandex.ru/dev/games/doc/ru/console/adv-monetization#sticky-banner) -
   рекламный баннер, который отображается в игре постоянно во время играния.

Для вызова рекламы используйте статический класс: `AdvertSDK`. <br>

1) Полноэкранные блоки -
   `InterstitialAd(Action onOpen, Action<bool> onClose, Action<string> onError, Action onOffline);`
    * `onOpen` - При открытии рекламы.
    * `onClose` - При закрытии рекламы. **Не используйте параметр, т.к. он криво работает на `Android`**
    * `onError` - При какой-либо ошибке. Возвращает строку с ошибкой.
    * `onOffline` - При потере соединении.

2) Rewarded-блоки (блоки с вознаграждением) -
   `RewardAd(Action onOpen, Action onRewarded, Action onClose, Action<string> onError);`
    * `onOpen` - При открытии рекламы.
    * `onRewarded` - После окончания таймера.
    * `onClose` - После закрытия рекламы.
    * `onError` - При какой-либо ошибке. Возвращает строку с ошибкой.

3) Sticky-баннеры - `AdvertSDK.StickyAdActive(bool value);`
    * `value` - Выключить(`false`) или Включить(`true`) баннер.

Также `AdvertSDK` имеет дополнительный функционал:

* `AdvertSDK.AdvertOff(AdvertType);` - при выключенной рекламе, она не будет показана, но `callback`-и будут
  вызваны.<br>
  `AdvertType` - тип рекламы, которая будет отключена. `[InterstitialAd, RewardAd, StickyAd]`
* `AdvertSDK.DelayAd` - задержка между вызовами рекламы. Перезарядка между вызовом рекламы `AdvertSDK.InterstitialAd();`
* `InterAdvertOffKey` - ключ, по которому будет сохраняться отключение `AdvertSDK.InterstitialAd();` рекламы.
* `RewardAdvertOffKey` - ключ, по которому будет сохраняться отключение `AdvertSDK.RewardAd();` рекламы.
* `StickyAdvertOffKey` - ключ, по которому будет сохраняться отключение `AdvertSDK.StickyAdActive();` рекламы.

### Добавление покупок и метод консумирования

Для работы с внутриигровыми покупками используйте статический класс: `Billing` из пространства
имен `Kimicu.YandexGames`. <br>

1) `IEnumerable<CatalogProduct> CatalogProduct` - список всех товаров.
    * `CatalogProduct` (CatalogProduct) - Информация о продукте
        * `description` (string) - Описание. Пример: `Отключение рекламы в игре...`
        * `id` (string) - Айди продукта. Пример: `remove_ads`
        * `imageURL` (string) - Ссылка на изображение продукта. Пример: `url`
        * `price` (string) - Цена. Пример: `19 YAN`
        * `priceCurrencyCode` (string) - Код валюты. Пример: `YAN`
        * `priceValue` (string) - Значение цены. Пример: `19`
        * `title` (string) - Название продукта. Пример: `Отключить рекламу`

2) `IEnumerable<PurchasedProduct> PurchasedProducts` - список необработанных покупок методом `ConsumeProduct()`.
    * `PurchasedProduct` (PurchasedProduct) - Информация о купленном продукте
        * `developerPayload` (string) - Дополнительные данные о покупке. Пример: `Отключение рекламы в игре...`
        * `productID` (string) - Айди продукта. Пример: `remove_ads`
        * `purchaseTime` (string) - Ссылка на изображение продукта. Пример: `url`
        * `purchaseToken` (string) - Цена. Пример: `19 YAN`
          <br><br>

3) `void PurchaseProduct(productID, onSuccess, onError, developerPayload)` - Произвести покупку у игрока.
    * `productID` (string) - Айди продукта. Пример: `remove_ads`
    * `onSuccess` (Action<PurchaseProductResponse>) - Успешная покупка продукта. Возвращает `PurchaseProductResponse`:
        * `purchaseData` (string) - Дата покупки. Пример: `13.10.2023`.
        * `signature` (string) - данные о покупке и подпись для проверки подлинности игрока. Пример: "
          hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk..."
    * `onError` (Action<string>) - Ошибка при покупке товара.
    * `developerPayload` (string) - дополнительные данные о покупке.
      <br><br>

4) `void PurchaseProductAndConsume(productID, onSuccessPurchase, onSuccessConsume, onErrorPurchase, onErrorConsume, developerPayload)` -
   Произвести покупку у игрока, а также ее подтвердить методом `ConsumeProduct`.
    * `productID` (string) - Айди продукта. Пример: `remove_ads`
    * `onSuccessPurchase` (Action<PurchaseProductResponse>) - Успешная покупка продукта.
      Возвращает `PurchaseProductResponse`:
        * `purchaseData` (string) - Дата покупки. Пример: `13.10.2023`.
        * `signature` (string) - данные о покупке и подпись для проверки подлинности игрока. Пример: "
          hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk..."
    * `onSuccessConsume` (Action) - Успешное подтверждение покупки.
    * `onErrorPurchase` (Action<string>) - Ошибка при покупке товара.
    * `onErrorConsume` (Action<string>) - Ошибка при подтверждении товара.
    * `developerPayload` (string) - дополнительные данные о покупке.
      <br><br>

5) `void ConsumeProduct(purchaseToken, onSuccess, onError)` -
   Произвести подтверждение покупки `ConsumeProduct`.
    * `purchaseToken` (string) - Токен для использования покупки.
    * `onSuccess` (Action) - Успешное подтверждение покупки.
    * `onError` (Action<string>) - Ошибка при покупке товара.

#### Реализация метода консумирование

Метод консумирования исправляет баг, связанный с покупками, когда игрок покупает продукт и не дожидаясь возвращения
в игру происходит обновление страницы и награда не будет начислена. Это значит, что покупка заберёт у игрока яны,
а событие onSuccess не будет вызван.<br>
Для исправления требуется:

1) Добавить метод `ConsumeProduct()` в событии `onSuccess` метода `PurchaseProduct()`.
2) Добавить в `Boot сцену` после инициализации `YandexSDKInitialize` метод, в котором нужно будет пройтись по списку
   `IEnumerable<PurchasedProduct> PurchasedProducts` и в зависимости от `productID` выдать награду игроку и вызвать
   метод `ConsumeProduct(purchaseToken)`.

### Состояния страницы

Для работы с состояниями страницы используйте статические классы:

1) `WebGL` из пространства имен `Kimicu.YandexGames` - подписаться или описаться на состояние игры.
2) `WebProperty` - Получить доступ к текущем состояниям:
    * `AdvertOpened` - Открыта ли реклама
    * `PurchaseWindowOpened` - Открыто ли окно с покупками
    * `InGameView` - Вкладка находиться в фокусе

### Облачные сохранения

Для работы с внутриигровыми покупками используйте статический класс: `YandexData` из пространства
имен `Kimicu.YandexGames`. <br>

1) `Save(key, value, saveInCloud, onSuccess, onError)` - Сохранения данных в облако.
    * `key` (string) - Ключ, по которому будет произведено сохранение. Например: `"Coins"`
    * `value` (JToken) - Тип значение, по которому будет произведено сохранение, а также что будет сохранено.
      Например: `100`, `"10 монет"`, `new[] {50, 100}`, `101.5f` и тд.
    * `saveInCloud` (bool) - Будет ли отправлено сохранения в облако или будет храниться в кэше игры.
    * `onSuccess` (Action) - Успешное сохранение в облако или в кэш.
    * `onError` (Action<string>) - Неудачное сохранение в облако.
    * Пример использования метода:
   ```cs
   YandexData.Save("Coins", 100); // В ячейку "Coins.Int" будет записано значение 100
   YandexData.Save("Money", 123.4f); // В ячейку "Money.Float" будет записано значение 123.4f
   ```

2) `Load(key, defaultValue)` - Получение данных из облака.
    * `key` (string) - Ключ, по которому будет произведен поиск. Например: `"Coins"`
    * `defaultValue` (JToken) - Тип значение, по которому будет произведено поиск, а также значение по умолчанию.
      Например: `100`, `"10 монет"`, `new[] {50, 100}`, `101.5f` и тд.
    * Пример использования метода:
   ```cs
   int coins = (int)YandexData.Load("Coins", 100); // Из ячейки "Coins.Int" будет получено значение если оно есть, иначе 100
   float money = (float)YandexData.Load("Money", 123.4f); // Из ячейки "Money.Float" будет получено значение если оно есть, иначе 123.4f
   ```

3) `SaveToClaud(onSuccess, onError)` - Сохранить несохранённые дынные в облако.
    * `onSuccess` (Action) - Успешное сохраннее в облако.
    * `onError` (Action<string>) - Неуспешное сохраннее в облако.