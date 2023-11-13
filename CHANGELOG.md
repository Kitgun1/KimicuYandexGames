# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.11] - 13.11.2023
### Fixed
- Теперь настройки не будут слетать после обновления пакета.

## [1.1.10] - 13.11.2023
### Change
- Добавлена возможность логировать YandexData. (См. `ProjectSettings > Kimicu > Yandex Settings`)

## [1.1.9] - 19.10.2023
### Change
- Исправлен баг с вызовом callback до добавления в  PurchaseProduct.

## [1.1.8] - 19.10.2023
### Added
- Добавлена логика `Leaderboard`

## [1.1.7] - 15.10.2023
### Change
- Именена панель настроек в `ProjectSettings`.

### Fixed
- Исправлен баг со звуком при переходе по вкладкам


## [1.1.6] - 13.10.2023
### Fixed
- Исправлен баг инита.


## [1.1.5] - 13.10.2023
### Fixed
- Исправлен баг инита.


## [1.1.3] - 13.10.2023
### Fixed
- Исправлен баг отображения настроек.


## [1.1.2] - 13.10.2023
### Fixed
- исправлены ошибки `Advert` и `Billing`.


## [1.1.1] - 13.10.2023
### Fixed
- исправлены ошибки в покупках в `Editor`.


## [1.1.0] - 13.10.2023
### Added
- Добавлены настройки для `YandexGames` в `ProjectSettings`.
- В Editor теперь есть стандартные покупки, которые можно настроить в `ProjectSettings` > `Kimicu` > `Yandex Settings`.

### Change
- Пространство имен `KiYandexSDK` изменено на `Kimicu.YandexGames`.
- Все скрипты были перемещены в `Runtime`.
- Изменено имя `AdvertSDK` на `Advert`.
- Переработан `README.md`.

### Remove
- Убран доступ к некоторым полям в `Advert`.


## [1.0.15] - 12.10.2023
### Added
- Added FAQ section to README.md.
- Added the ability to customize states for a WebGL object.

### Change
- Now there will be no error with initialization of YandexData in Editor.


## [1.0.14] - 03.10.2023
### Change
- Separated overloading of PurchaseProduct methods. <br>
Now the method that was responsible for the purchase and consume is called PurchaseProductAndConsume.


## [1.0.13] - 02.10.2023
### Added
- Now you can disable the ads of your choice.


## [1.0.12] - 02.10.2023
### Change
- Now "advert off" affects only InterstitialAd.


## [1.0.10] - 02.10.2023
### Added
- Added more documentation.
- Now you can configure the delay of the ad call yourself.

### Change
- Change documentation language "russian to english".
- Now some parameters are optional in PurchaseProduct.
- Now, when "RewardAd" is called and when advertising is turned off, the following events will be triggered: <br>
onOpen, onRewarded and onClose.
- Now, when "Rewarded" is called in the Editor, the following events will be triggered: <br>
  onOpen, onRewarded, and onClose.


## [1.0.9] - 01.10.2023
### Added
- Added a stub for the Editor in Billing.


## [1.0.8] - 01.10.2023
### Added
- Add Billing methods.

### Removed
- Clean code.