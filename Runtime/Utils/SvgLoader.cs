using System;
using System.Collections;
using System.IO;
using System.Text;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace Kimicu.YandexGames.Utils
{
    public static class SvgLoader
    {
        private static Material _defaultMaterial;
        
        private const string SHADER_VECTOR_GRADIENT = "Unlit/VectorGradient";
        
        private const string SHADER_VECTOR_DEMULTIPLY = "Hidden/VectorDemultiply";
        private const string SHADER_VECTOR_EXPAND_EDGES = "Hidden/VectorExpandEdges";
        private const string SHADER_VECTOR_BLEND_MAX = "Hidden/VectorBlendMax";

        private static Material DefaultMaterial
        {
            get
            {
                if (_defaultMaterial == null)
                {
                    _defaultMaterial = new Material(Shader.Find(SHADER_VECTOR_GRADIENT));
                }
                return _defaultMaterial;
            }
        }

        public static IEnumerator GetSvgTexture(string url, int width, int height, Action<Texture2D> onSuccess, Action<string> onError = null)
        {
            if (url.Contains("svg") == false)
            {
                onError?.Invoke($"URL {url} is not SVG");
                yield break;
            }
            
            using var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(www.error);
                yield break;
            }

            var svgText = Encoding.UTF8.GetString(www.downloadHandler.data);
            
            const string fillCurrentColor = "fill=\"currentColor\">";
            const string fillBlack = "fill=\"black\">";
            
            if (svgText.Contains(fillCurrentColor))
            {
                svgText = svgText.Replace(fillCurrentColor, fillBlack);
            }
            
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svgText));

            var tessOptions = new VectorUtils.TessellationOptions
            {
                StepDistance = 1f,
                MaxCordDeviation = 0.25f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);

            Rect bounds = VectorUtils.SceneNodeBounds(sceneInfo.Scene.Root);
            if (bounds.width <= 0f || bounds.height <= 0f)
                bounds = VectorUtils.ApproximateSceneNodeBounds(sceneInfo.Scene.Root);

            var svgPixelsPerUnit = bounds is { width: > 0f, height: > 0f }
                ? Mathf.Min(width / bounds.width, height / bounds.height)
                : 100f;

            if (float.IsInfinity(svgPixelsPerUnit) || svgPixelsPerUnit <= 0f)
                svgPixelsPerUnit = 100f;
            

            var sprite = VectorUtils.BuildSprite(geoms, svgPixelsPerUnit, VectorUtils.Alignment.Center, Vector2.zero, 256, true);

            var tex = VectorUtils.RenderSpriteToTexture2D(sprite, width, height, DefaultMaterial, 4, true);
            onSuccess?.Invoke(tex);
        }

        public static IEnumerator GetSvgSprite(string url, Vector2Int size, Action<Sprite> onSuccess, Action<string> onError = null)
        {
            yield return GetSvgTexture(url, size.x, size.y, texture =>
            {
                var rect = new Rect(0, 0, texture.width, texture.height);
                var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
                onSuccess?.Invoke(sprite);
            }, onError);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Kimicu/SVG/Add SVG Shader to Always Included Shaders")]
        private static void AddShadersToAlwaysIncluded()
        {
            var shaders = new[]
            {
                SHADER_VECTOR_GRADIENT, 
                SHADER_VECTOR_DEMULTIPLY, 
                SHADER_VECTOR_EXPAND_EDGES, 
                SHADER_VECTOR_BLEND_MAX
            };
            
            foreach (var shader in shaders)
            {
                AddShaderToAlwaysIncluded(shader);
            }
        }
        
        private static void AddShaderToAlwaysIncluded(string shaderName)
        {
            var shader = Shader.Find(shaderName);

            if (shader == null)
            {
                Debug.LogError($"Shader '{shaderName}' not found. Make sure it exists and is correctly named.");
                return;
            }

            var graphicsSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");

            if (graphicsSettings == null)
            {
                Debug.LogError("GraphicsSettings.asset not found.");
                return;
            }

            var serializedObject = new UnityEditor.SerializedObject(graphicsSettings);
            var alwaysIncludedShadersProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");

            for (var i = 0; i < alwaysIncludedShadersProp.arraySize; i++)
            {
                if (alwaysIncludedShadersProp.GetArrayElementAtIndex(i).objectReferenceValue == shader)
                {
                    Debug.Log($"Shader '{shaderName}' is already in the Always Included Shaders list.");
                    return;
                }
            }

            var newIndex = alwaysIncludedShadersProp.arraySize;
            alwaysIncludedShadersProp.InsertArrayElementAtIndex(newIndex);
            alwaysIncludedShadersProp.GetArrayElementAtIndex(newIndex).objectReferenceValue = shader;

            serializedObject.ApplyModifiedProperties();
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log($"Shader '{shaderName}' successfully added to Always Included Shaders.");
        }
#endif
    }
}