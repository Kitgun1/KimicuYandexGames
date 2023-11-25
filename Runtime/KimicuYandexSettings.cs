using System.Collections.Generic;
using Agava.YandexGames;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kimicu.YandexGames
{
    [CreateAssetMenu(fileName = "KimicuYandexSettings", menuName = "Kimicu/KimicuYandexSettings", order = 0)]
    public class KimicuYandexSettings : ScriptableObject
    {
        public int Priority = -999;

        #region YandexCloudSave

        public string Postfix = "";
        public string Separator = "_";

        #endregion

        #region WebGL

        public bool SoundChange = true;

        #endregion

        #region Purchase

        public List<CatalogProduct> CatalogProductInEditor = new()
        {
            new CatalogProduct
            {
                id = "test1",
                description = "test",
                price = "1 YAN",
                title = "test tittle",
                priceValue = "1",
                priceCurrencyCode = "YAN",
                imageURI = "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png"
            },
            new CatalogProduct
            {
                id = "test2",
                description = "test 2",
                price = "2 YAN",
                title = "test tittle 2",
                priceValue = "2",
                priceCurrencyCode = "YAN",
                imageURI = "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png"
            },
            new CatalogProduct
            {
                id = "test3",
                description = "test 3",
                price = "3 YAN",
                title = "test tittle 3",
                priceValue = "3",
                priceCurrencyCode = "YAN",
                imageURI = "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png"
            }
        };

        public List<PurchasedProduct> PurchasedProductInEditor = new()
        {
            new PurchasedProduct
            {
                productID = "test1",
                purchaseToken = "d85ae0b1-9166-4fbb-bb38-6d2a4ca4416d",
                purchaseTime = "13.10.2023",
                developerPayload = "TEST DEVELOPER PAYLOAD"
            },
            new PurchasedProduct
            {
                productID = "test3",
                purchaseToken = "d85ae0b1-9166-4fbb-bb38-6d2a4ca4416d",
                purchaseTime = "10.06.2022",
                developerPayload = "TEST DEVELOPER PAYLOAD"
            },
        };

        #endregion

        #region Advert

        [Min(5f)] public float DelayAdvert = 60.1f;
        public string InterAdvertOffKey = "INTER_ADVERT_OFF";
        public string RewardAdvertOffKey = "REWARD_ADVERT_OFF";
        public string StickyAdvertOffKey = "STICKY_ADVERT_OFF";

        #endregion

        #region Leaderboard

        public bool LeaderboardActive = false;
        public string LeaderboardName = "write name!";
        public int TopPlayersCount = 5;
        public int PlayerRankInEditor = 1;
        public int DelayUpdateLeaderboardInfo = 300;
        public int CompetingPlayersCount = 5;
        public bool IncludeSelf = true;
        public bool WaitInitializePicture = true;
        public bool InvertSortOrder = false;
        public LeaderboardType LeaderboardValueType = LeaderboardType.numeric;
        public ProfilePictureSize PictureSize = ProfilePictureSize.medium;

        #endregion

        #region Debuging

        public bool YandexDataDebugEnabled = false;
        public bool AdvertDebugEnabled = false;

        #endregion

        private static KimicuYandexSettings _instance;

        public static KimicuYandexSettings Instance
        {
            get
            {
                if (_instance != null) return _instance;
                KimicuYandexSettings settings = Resources.Load<KimicuYandexSettings>("KimicuYandexSettings");
                if (settings == null)
                {
                    KimicuYandexSettings yandexSettings = CreateInstance<KimicuYandexSettings>();
                    yandexSettings.Priority = 1;
                    #if UNITY_EDITOR
                    AssetDatabase.CreateAsset(yandexSettings, "Assets/Resources/KimicuYandexSettings.asset");
                    #endif
                    _instance = yandexSettings;
                }
                else
                {
                    #if UNITY_EDITOR
                    KimicuYandexSettings yandexSettings = CreateInstance<KimicuYandexSettings>();
                    yandexSettings.Priority = 1;
                    AssetDatabase.CreateAsset(yandexSettings, "Assets/Resources/KimicuYandexSettings.asset");
                    _instance = yandexSettings;
                    #endif
                }

                return _instance;
            }
        }
    }

    public enum LeaderboardType
    {
        numeric,
        time
    }
}