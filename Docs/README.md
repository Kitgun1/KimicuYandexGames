# 1️⃣ Yandex Games Sdk
## 👉Компоненты Sdk👈
### ⚙️ [YandexGamesSdk - Главный класс работы с SDK.](YandexGamesSdk.md)
### ☁️ [Cloud - Сохранение данных в облако Yandex.](Cloud.md)
### 📢 [Advertisement - Работа с различной рекламой в игре.](Advertisement.md)
### 💲 [Billing - Внутриигровые покупки за валюту YAN.](Billing.md)
### ⛄ [Account - Аккаунт игрока на yandex games.](Account.md)
### 🌐 [WebApplication - Состояние игры в браузере.](WebApplication.md)
### 🫧 [Shortcut - Иконки на рабочий стол.](Shortcut.md)
### 📽️ [AdBlock - Отслеживание включенного AdBlock.](Shortcut.md)
### 🖥️ [Device - Отслеживание устройства.](Device.md)
### 🚩 [Flags - Удаленные конфиги.](Flags.md)

## Установка
1) PackageManager > + > Add package from git URL.. <br>
   ```http request
   https://github.com/Kitgun1/KimicuYandexGames.git
   ```
2) PackageManager > + > Add package by name... <br>
   ```http request
   com.unity.nuget.newtonsoft-json
   ```

## Перед началом работы с SDK
### Настройка проекта
* Генерируем все файлы для работы в Editor <br>
  `Kimicu > Yandex Games > Generate Editor Cloud Files` <br>
  Зайдите в корневую папку проекта > `EditorCloud` и заполните все файлы при необходимости
  * `adblock.txt` - включен ли блокировщик рекламы в Editor
  * `device.txt` - на мобильном ли телефоне сейчас идет игра в Editor
  * `environment.txt` - информация об окружении в Editor
  * `Save.txt` - сохранения в виде десятичной строки в Editor
  * `catalog.txt` - информация о всех продуктах в Editor, укажите все как в yandex console
  * `purchased-products.txt` - информация о необработанных продуктов в Editor <br>
    **ПРИМЕЧАНИЕ:** при покупке продукта в Editor, она добавиться в этот файл
* Заменяем на всех сценах `Event System` на `Web Event System` <br>
  **ПРИМЕЧАНИЕ:** Вызывайте это перед билдом, если вы создавали новую сцену <br>
  `Kimicu > Yandex Games > Replace All Event Systems`
* Настройка в `Project Settings > Player`
  * Resolution and Presentation
    * Run In Background - ✅
  * Publishing Settings
    * `Compression Format` - `Brotli`
    * `Name Files As Hashes` - ✅
    * `Data Caching` - ❎
    * `Decompression Fallback` - ✅
