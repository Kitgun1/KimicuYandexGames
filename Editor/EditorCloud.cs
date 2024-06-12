#if UNITY_EDITOR
using Agava.YandexGames;
using Kimicu.YandexGames.Extension;
using UnityEditor;

namespace KimicuYandexGames.Editors
{
    public class EditorCloud : Editor
    {
        private const string ENVIRONMENT_FILE_NAME = "environment";
        private const string CATALOG_FILE_NAME = "catalog";
        private const string PURCHASED_PRODUCTS_FILE_NAME = "purchased-products";
        
        
        [MenuItem("Kimicu/Yandex Games/Generate Editor Cloud Files")]
        private static void LoadEditorCloudFiles()
        {
            FileExtensions.LoadObject(ENVIRONMENT_FILE_NAME, new YandexGamesEnvironment() {
                app = new YandexGamesEnvironment.App { id = "editor" },
                browser = new YandexGamesEnvironment.Browser { lang = "ru" },
                payload = "editor",
                i18n = new YandexGamesEnvironment.Internationalization { lang = "ru", tld = "https://yandex.ru/games" }
            });
            
            var catalog = new[] {
                new CatalogProduct {
                    id = "coins_1000_example", title = "1000 монет",
                    description = "Валюта для покупки предметов в магазине.",
                    price = "9 YAN", priceValue = "9", priceCurrencyCode = "YAN",
                    imageURI = ""
                },
                new CatalogProduct {
                    id = "coins_100_example", title = "100 монет",
                    description = "Валюта для покупки предметов в магазине.",
                    price = "1 YAN", priceValue = "1", priceCurrencyCode = "YAN",
                    imageURI = ""
                },
            };
            FileExtensions.LoadObject(CATALOG_FILE_NAME, catalog);
            
            FileExtensions.LoadObject(PURCHASED_PRODUCTS_FILE_NAME, new PurchasedProduct[] { });
            
            FileExtensions.LoadObject("adblock", false);
            
            FileExtensions.LoadObject("Save", "{}");
            
            FileExtensions.LoadObject("device", true);
        }
    }
}
#endif
