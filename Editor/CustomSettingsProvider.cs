using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using Agava.YandexGames;
using UnityEditor;
using KimicuUtility;

namespace Kimicu.YandexGames.Editor
{
    public class CustomSettingsProvider : SettingsProvider
    {
        private KimicuYandexSettings _settings;

        private const int Indent = 20;
        private const int LabelWidth = 205;
        private const int FieldHeight = 20;
        private const float Spacing = 5;

        private bool _cloudSaveEnabled;
        private bool _webGLEnabled;
        private bool _purchaseEnabled;
        private bool _catalogProductShow;
        private readonly List<bool> _catalogProductFoldouts = new();
        private bool _purchaseProductShow;
        private readonly List<bool> _purchaseProductFoldouts = new();
        private bool _advertEnabled;
        private bool _debugEnabled;

        private readonly GUIStyle _headerNormalStyle;
        private readonly GUIStyle _headerDebugStyle;

        public CustomSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
            _settings = KimicuYandexSettings.Instance;
            _cloudSaveEnabled = EditorPrefs.GetBool(nameof(_cloudSaveEnabled), true);
            _webGLEnabled = EditorPrefs.GetBool(nameof(_webGLEnabled), true);
            _purchaseEnabled = EditorPrefs.GetBool(nameof(_purchaseEnabled), true);
            _catalogProductShow = EditorPrefs.GetBool(nameof(_catalogProductShow), true);
            _purchaseProductShow = EditorPrefs.GetBool(nameof(_purchaseProductShow), true);
            _advertEnabled = EditorPrefs.GetBool(nameof(_advertEnabled), true);
            _debugEnabled = EditorPrefs.GetBool(nameof(_debugEnabled), true);

            int catalogProductFoldoutsAmount = 0;
            if (_settings != null) catalogProductFoldoutsAmount = _settings.CatalogProductInEditor.Count;
            for (int i = 0; i < catalogProductFoldoutsAmount; i++)
                _catalogProductFoldouts.Add(EditorPrefs.GetBool(nameof(_catalogProductFoldouts) + i));

            int purchaseProductFoldoutsAmount = 0;
            if (_settings != null) purchaseProductFoldoutsAmount = _settings.CatalogProductInEditor.Count;
            for (int i = 0; i < purchaseProductFoldoutsAmount; i++)
                _purchaseProductFoldouts.Add(EditorPrefs.GetBool(nameof(_purchaseProductFoldouts) + i));

            _headerNormalStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                normal = { textColor = new Color(0.36f, 0.53f, 1f) },
                onNormal = { textColor = new Color(0.36f, 0.53f, 1f) }
            };

            _headerDebugStyle = new GUIStyle(_headerNormalStyle)
            {
                normal = { textColor = new Color(0.54f, 1f, 0.42f) },
                onNormal = { textColor = new Color(0.54f, 1f, 0.42f) }
            };
        }

        [Obsolete("Obsolete")]
        public override void OnGUI(string searchContext)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, 0, LabelWidth - Indent, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Spacing - Indent;
            Rect valueRect = new(labelRect.xMax, 0, valueWidth, FieldHeight);
            valueRect.width -= 150 + Spacing * 2;
            Rect createButtonRect = new(valueRect.xMax + Spacing, 0, 60, FieldHeight);
            Rect maxPriorityButtonRect = new(createButtonRect.xMax + Spacing, 0, 90, FieldHeight);
            EditorGUI.LabelField(labelRect, "Yandex Game Settings");
            _settings = (KimicuYandexSettings)EditorGUI.ObjectField(valueRect, _settings,
                typeof(KimicuYandexSettings), false);

            if (GUI.Button(createButtonRect, "Create"))
            {
                _settings = ScriptableObject.CreateInstance<KimicuYandexSettings>();
                _settings.Priority = 1;

                string path = "Assets/Resources";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                string[] files = Directory.GetFiles(path);
                int fileCount = files.Length;
                string pathAsset = fileCount > 0
                    ? $"Assets/Resources/KimicuYandexSettings {fileCount / 2}.asset"
                    : $"Assets/Resources/KimicuYandexSettings.asset";
                AssetDatabase.CreateAsset(_settings, pathAsset);
                EditorPrefs.SetString("YandexSettingsAssetPath", pathAsset);
            }

            if (GUI.Button(maxPriorityButtonRect, "Max Priority"))
            {
                _settings = KimicuYandexSettings.Instance;
            }

            if (_settings != null)
            {
                DrawPrioritySettings();
                EditorGUILayout.Separator();
                DrawCloudSaveSettings();
                EditorGUILayout.Separator();
                DrawWebGLSettings();
                EditorGUILayout.Separator();
                DrawPurchaseSettings();
                EditorGUILayout.Separator();
                DrawAdvertSettings();
                EditorGUILayout.Separator();
                DrawDebugSettings();
            }

            Repaint();
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
        {
            var provider = new CustomSettingsProvider("Project/Kimicu/Yandex Settings");

            return provider;
        }

        private void DrawPrioritySettings()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, controlRect.y, LabelWidth - Indent, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Indent;
            Rect valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth - Spacing, FieldHeight);
            EditorGUI.LabelField(labelRect, "Priority");
            _settings.Priority = EditorGUI.IntField(valueRect, _settings.Priority);
        }

        private void DrawCloudSaveSettings()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, controlRect.y, LabelWidth - Indent, FieldHeight);
            _cloudSaveEnabled = EditorGUI.Foldout(labelRect, _cloudSaveEnabled, "Cloud Save Show", _headerNormalStyle);
            EditorPrefs.SetBool(nameof(_cloudSaveEnabled), _cloudSaveEnabled);
            if (!_cloudSaveEnabled) return;

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            Rect valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Postfix");
            _settings.Postfix = EditorGUI.TextArea(valueRect, _settings.Postfix);

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Separator");
            _settings.Separator = EditorGUI.TextArea(valueRect, _settings.Separator);
        }

        private void DrawWebGLSettings()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, controlRect.y, LabelWidth - Indent, FieldHeight);
            _webGLEnabled = EditorGUI.Foldout(labelRect, _webGLEnabled, "WebGL Show", _headerNormalStyle);
            EditorPrefs.SetBool(nameof(_webGLEnabled), _webGLEnabled);
            if (!_webGLEnabled) return;

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            Rect valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "SoundChange");
            _settings.SoundChange = EditorGUI.Toggle(valueRect, _settings.SoundChange);
        }

        private void DrawPurchaseSettings()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, controlRect.y, LabelWidth - Indent, FieldHeight);
            _purchaseEnabled = EditorGUI.Foldout(labelRect, _purchaseEnabled, "Purchase Show", _headerNormalStyle);
            EditorPrefs.SetBool(nameof(_purchaseEnabled), _purchaseEnabled);
            if (!_purchaseEnabled) return;
            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            Rect valueRect = new Rect(controlRect.xMax - 50 - Spacing, controlRect.y, 50, FieldHeight);
            _catalogProductShow = EditorGUI.Foldout(labelRect, _catalogProductShow, "CatalogProductInEditor");
            EditorPrefs.SetBool(nameof(_catalogProductShow), _catalogProductShow);
            int count = EditorGUI.IntField(valueRect, _settings.CatalogProductInEditor.Count);
            if (_catalogProductShow)
            {
                for (int i = 0; i < count; i++)
                {
                    _settings.CatalogProductInEditor.Clamp(count, count, new CatalogProduct());
                    CatalogProduct product = _settings.CatalogProductInEditor[i];
                    var fields = product.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
                    labelRect = new Rect(controlRect.x + Indent * 3, controlRect.y, LabelWidth, FieldHeight);
                    Rect clearButton = new(controlRect.xMax - 200 - Spacing * 3, controlRect.y, 50, FieldHeight);
                    Rect removeButton = new(clearButton.xMax + Spacing, controlRect.y, 75, FieldHeight);
                    Rect duplicateButton = new(removeButton.xMax + Spacing, controlRect.y, 75, FieldHeight);

                    _catalogProductFoldouts.Clamp(i + 1, true);
                    _catalogProductFoldouts[i] = EditorGUI.Foldout(labelRect, _catalogProductFoldouts[i],
                        (string)fields.Last().GetValue(product));

                    if (GUI.Button(clearButton, "Clear"))
                    {
                        foreach (var field in fields)
                        {
                            field.SetValue(product, default);
                        }
                    }

                    if (GUI.Button(removeButton, "Remove"))
                    {
                        _settings.CatalogProductInEditor.RemoveAt(i);
                        break;
                    }

                    if (GUI.Button(duplicateButton, "Duplicate"))
                    {
                        _settings.CatalogProductInEditor.Add(new CatalogProduct
                        {
                            description = product.description,
                            id = product.id,
                            price = product.price,
                            title = product.title,
                            priceValue = product.priceValue,
                            priceCurrencyCode = product.priceCurrencyCode,
                            imageURI = product.imageURI
                        });
                        break;
                    }

                    EditorPrefs.SetBool(nameof(_catalogProductFoldouts) + i, _catalogProductFoldouts[i]);
                    if (_catalogProductFoldouts[i])
                    {
                        foreach (FieldInfo field in fields)
                        {
                            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
                            labelRect = new Rect(controlRect.x + Indent * 4, controlRect.y, LabelWidth,
                                FieldHeight);
                            valueWidth = controlRect.width - labelRect.width - Spacing;
                            valueRect = new Rect(labelRect.width + Spacing, controlRect.y, valueWidth, FieldHeight);
                            EditorGUI.LabelField(labelRect, field.Name);
                            field.SetValue(product,
                                EditorGUI.TextField(valueRect, (string)field.GetValue(product)));
                        }
                    }
                }
            }

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            valueRect = new Rect(controlRect.xMax - 50 - Spacing, controlRect.y, 50, FieldHeight);
            _purchaseProductShow = EditorGUI.Foldout(labelRect, _purchaseProductShow, "PurchasedProductInEditor");
            EditorPrefs.SetBool(nameof(_purchaseProductShow), _purchaseProductShow);
            count = EditorGUI.IntField(valueRect, _settings.PurchasedProductInEditor.Count);
            if (_purchaseProductShow)
            {
                for (int i = 0; i < count; i++)
                {
                    _settings.PurchasedProductInEditor.Clamp(count, count, new PurchasedProduct());
                    PurchasedProduct product = _settings.PurchasedProductInEditor[i];
                    var fields = product.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
                    labelRect = new Rect(controlRect.x + Indent * 3, controlRect.y, LabelWidth, FieldHeight);
                    Rect clearButton = new(controlRect.xMax - 200 - Spacing * 3, controlRect.y, 50, FieldHeight);
                    Rect removeButton = new(clearButton.xMax + Spacing, controlRect.y, 75, FieldHeight);
                    Rect duplicateButton = new(removeButton.xMax + Spacing, controlRect.y, 75, FieldHeight);

                    _purchaseProductFoldouts.Clamp(i + 1, true);
                    _purchaseProductFoldouts[i] = EditorGUI.Foldout(labelRect, _purchaseProductFoldouts[i],
                        (string)fields[1].GetValue(product));
                    EditorPrefs.SetBool(nameof(_purchaseProductFoldouts) + i, _purchaseProductFoldouts[i]);

                    if (GUI.Button(clearButton, "Clear"))
                    {
                        foreach (var field in fields)
                        {
                            field.SetValue(product, default);
                        }
                    }

                    if (GUI.Button(removeButton, "Remove"))
                    {
                        _settings.PurchasedProductInEditor.RemoveAt(i);
                        break;
                    }

                    if (GUI.Button(duplicateButton, "Duplicate"))
                    {
                        _settings.PurchasedProductInEditor.Add(new PurchasedProduct
                        {
                            productID = product.productID,
                            developerPayload = product.developerPayload,
                            purchaseTime = product.purchaseTime,
                            purchaseToken = product.purchaseToken,
                        });
                        break;
                    }

                    if (_purchaseProductFoldouts[i])
                    {
                        foreach (FieldInfo field in fields)
                        {
                            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
                            labelRect = new Rect(controlRect.x + Indent * 4, controlRect.y, LabelWidth,
                                FieldHeight);
                            valueWidth = controlRect.width - labelRect.width - Spacing;
                            valueRect = new Rect(labelRect.width + Spacing, controlRect.y, valueWidth, FieldHeight);
                            EditorGUI.LabelField(labelRect, field.Name);
                            field.SetValue(product,
                                EditorGUI.TextField(valueRect, (string)field.GetValue(product)));
                        }
                    }
                }
            }
        }

        private void DrawAdvertSettings()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, controlRect.y, LabelWidth - Indent, FieldHeight);
            _advertEnabled = EditorGUI.Foldout(labelRect, _advertEnabled, "Advert Show", _headerNormalStyle);
            EditorPrefs.SetBool(nameof(_advertEnabled), _advertEnabled);
            if (!_advertEnabled) return;

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            Rect valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Delay Advert");
            _settings.DelayAdvert = EditorGUI.FloatField(valueRect, _settings.DelayAdvert);

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Inter Advert Off Key");
            _settings.InterAdvertOffKey = EditorGUI.TextArea(valueRect, _settings.InterAdvertOffKey);

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Reward Advert Off Key");
            _settings.RewardAdvertOffKey = EditorGUI.TextArea(valueRect, _settings.RewardAdvertOffKey);

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Sticky Advert Off Key");
            _settings.StickyAdvertOffKey = EditorGUI.TextArea(valueRect, _settings.StickyAdvertOffKey);
        }

        private void DrawDebugSettings()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            Rect labelRect = new(Indent, controlRect.y, LabelWidth - Indent, FieldHeight);
            _debugEnabled = EditorGUI.Foldout(labelRect, _debugEnabled, "Advert Show", _headerDebugStyle);
            EditorPrefs.SetBool(nameof(_debugEnabled), _debugEnabled);
            if (!_debugEnabled) return;

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
            float valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
            Rect valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "YandexData Debug Enabled");
            _settings.YandexDataDebugEnabled = EditorGUI.Toggle(valueRect, _settings.YandexDataDebugEnabled);

            controlRect = EditorGUILayout.GetControlRect(true, FieldHeight);
            labelRect = new Rect(Indent * 2, controlRect.y, LabelWidth - Indent * 2, FieldHeight);
             valueWidth = controlRect.width - labelRect.width - Spacing - Indent * 2;
             valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, FieldHeight);
            EditorGUI.LabelField(labelRect, "Advert Debug Enabled");
            _settings.AdvertDebugEnabled = EditorGUI.Toggle(valueRect, _settings.AdvertDebugEnabled);
        }
    }
}
#endif