#if UNITY_EDITOR
using System.Linq;
using Agava.WebUtility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace KimicuYandexGames.Editors
{
    public class WebEventSystemReplace : Editor
    {
        [MenuItem("Kimicu/Yandex Games/Replace All Event Systems")]
        private static void ReplaceEventSystems()
        {
            var scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToList();

            Debug.Log($"scenes: {scenes.Count}");
            foreach (var scene in scenes)
            {
                Debug.Log($"scene: {scene}");
                EditorSceneManager.OpenScene(scene);

                var allGameObjects = Resources.FindObjectsOfTypeAll<EventSystem>();
                foreach (EventSystem eventSystem in allGameObjects)
                {
                    var obj = eventSystem.gameObject;
                    if (eventSystem != null)
                    {
                        DestroyImmediate(obj.GetComponent<StandaloneInputModule>());
                        DestroyImmediate(eventSystem);
                        
                        obj.AddComponent<WebEventSystem>();
                        obj.AddComponent<StandaloneInputModule>();
                        EditorUtility.SetDirty(obj);
                    }
                }

                EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(scene));
                EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(scene), false);
            }
        }
    }
}
#endif