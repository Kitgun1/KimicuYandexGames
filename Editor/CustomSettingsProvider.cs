#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Kimicu.YandexGames.Editor
{
#if UNITY_EDITOR
    public class CustomSettingsProvider : SettingsProvider
    {
        private SerializedObject m_SettingsObject;
        private bool m_IsActiveCloudSave = true;
        private bool m_IsActiveWebGL = true;
        private bool m_IsActivePurchases = true;
        private bool m_IsActiveAdvert = true;

        public CustomSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope)
        {
        }

        public override void OnGUI(string searchContext)
        {
            if (m_SettingsObject == null)
            {
                m_SettingsObject = new SerializedObject(KimicuYandexSettings.Instance);
            }

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
#endif
}