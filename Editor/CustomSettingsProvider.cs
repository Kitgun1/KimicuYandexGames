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

            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("Postfix"));
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("Separator"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("SoundChange"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("CatalogProductInEditor"));
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("PurchasedProductInEditor"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("DelayAdvert"));
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("InterAdvertOffKey"));
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("RewardAdvertOffKey"));
            EditorGUILayout.PropertyField(m_SettingsObject.FindProperty("StickyAdvertOffKey"));


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