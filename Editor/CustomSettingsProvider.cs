using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Kimicu.YandexGames.Editor
{
    public class CustomSettingsProvider : SettingsProvider
    {
        private SerializedObject m_SettingsObject;
        private bool m_IsActiveCloudSave = true;
        private bool m_IsActiveWebGL = true;
        private bool m_IsActivePurchases = true;
        private bool m_IsActiveAdvert = true;
        private bool m_IsActiveLeaderboard = true;

        public CustomSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope) { }

        public override void OnGUI(string searchContext)
        {
            if (m_SettingsObject == null)
            {
                m_SettingsObject = new SerializedObject(KimicuYandexSettings.Instance);
            }
            
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("YandexDataDebugEnabled"));

            m_IsActiveCloudSave = EditorGUILayout.Foldout(m_IsActiveCloudSave, "Настройки для облачного сохранения:");
            if (m_IsActiveCloudSave)
            {
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("Postfix"));
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("Separator"));
            }

            EditorGUILayout.Separator();
            m_IsActiveWebGL = EditorGUILayout.Foldout(m_IsActiveWebGL, "Настройки окна браузера:");
            if (m_IsActiveWebGL)
            {
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("SoundChange"));
                EditorGUILayout.HelpBox("Звук будет отключаться/включатся при выходе из вкладки или при показе рекламы",
                    MessageType.Info);
            }

            EditorGUILayout.Separator();
            m_IsActivePurchases = EditorGUILayout.Foldout(m_IsActivePurchases, "Настройки покупок:");
            if (m_IsActivePurchases)
            {
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("CatalogProductInEditor"));
                EditorGUILayout.HelpBox("Каталог товаров, доступные в Editor(в браузер не используются)",
                    MessageType.Info);
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("PurchasedProductInEditor"));
                EditorGUILayout.HelpBox(
                    "Купленные и необработанные товары, доступные в Editor(в браузер не испольуются)",
                    MessageType.Info);
            }

            EditorGUILayout.Separator();
            m_IsActiveAdvert = EditorGUILayout.Foldout(m_IsActiveAdvert, "Настройки рекламы:");
            if (m_IsActiveAdvert)
            {
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("DelayAdvert"));
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("InterAdvertOffKey"));
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("RewardAdvertOffKey"));
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("StickyAdvertOffKey"));
            }

            EditorGUILayout.Separator();
            m_IsActiveLeaderboard = EditorGUILayout.Foldout(m_IsActiveLeaderboard, "Настройки лидеров:");
            if (m_IsActiveLeaderboard)
            {
                EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("LeaderboardActive"));
                if (m_SettingsObject.FindProperty("LeaderboardActive").boolValue)
                {
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("LeaderboardName"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("TopPlayersCount"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("PlayerRankInEditor"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("DelayUpdateLeaderboardInfo"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("CompetingPlayersCount"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("IncludeSelf"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("WaitInitializePicture"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("InvertSortOrder"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("LeaderboardValueType"));
                    EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("PictureSize"));
                }
            }

            if (GUI.changed)
            {
                m_SettingsObject.ApplyModifiedProperties();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
        {
            var provider = new CustomSettingsProvider("Project/Kimicu/Yandex Settings", SettingsScope.Project);

            return provider;
        }
    }
}
#endif