using Agava.YandexGames;
using UnityEngine;

namespace Kimicu.YandexGames
{
    [CreateAssetMenu(fileName = "KimicuYandexSettings", menuName = "Kimicu/KimicuYandexSettings", order = 0)]
    public class KimicuYandexSettings : ScriptableObject
    {
        #region YandexCloudSave

        [Header("Yandex Cloud Save"), SerializeField]
        public string Postfix = "";

        [SerializeField] public string Separator = "_";

        #endregion

        #region WebGL

        [SerializeField, Header("WebGL")] public bool SoundChange = true;

        #endregion

        #region Purchase

        [field: SerializeField, Header("Purchase")]
        public CatalogProduct[] CatalogProductInEditor { get; private set; } =
        {
            new()
            {
                id = "test1",
                description = "test",
                price = "1 YAN",
                title = "test tittle",
                priceValue = "1",
                priceCurrencyCode = "YAN",
                imageURI = "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png"
            },
            new()
            {
                id = "test2",
                description = "test 2",
                price = "2 YAN",
                title = "test tittle 2",
                priceValue = "2",
                priceCurrencyCode = "YAN",
                imageURI = "https://static.donationalerts.ru/uploads/qr/3414545/qr_bfc32bd513ca60d7023358650860e2d7.png"
            },
            new()
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

        [field: SerializeField]
        public PurchasedProduct[] PurchasedProductInEditor { get; private set; } =
        {
            new()
            {
                productID = "test1",
                purchaseToken = "d85ae0b1-9166-4fbb-bb38-6d2a4ca4416d",
                purchaseTime = "13.10.2023",
                developerPayload = "TEST DEVELOPER PAYLOAD"
            },
            new()
            {
                productID = "test3",
                purchaseToken = "d85ae0b1-9166-4fbb-bb38-6d2a4ca4416d",
                purchaseTime = "10.06.2022",
                developerPayload = "TEST DEVELOPER PAYLOAD"
            },
        };

        #endregion

        #region Advert

        [SerializeField, Header("Advert"), Min(5f)]
        public float DelayAdvert = 30.1f;

        [SerializeField, Header("Advert")] public string InterAdvertOffKey = "INTER_ADVERT_OFF";
        [SerializeField] public string RewardAdvertOffKey = "REWARD_ADVERT_OFF";
        [SerializeField] public string StickyAdvertOffKey = "STICKY_ADVERT_OFF";

        #endregion

        private static KimicuYandexSettings _instance;

        public static KimicuYandexSettings Instance
        {
            get
            {
                _instance = Resources.Load<KimicuYandexSettings>("KimicuYandexSettings");
                return _instance;
            }
        }
    }
}