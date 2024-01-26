using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Kimicu.YandexGames.Utils
{
    public class Coroutines : MonoBehaviour
    {
        private static Coroutines _instance;

        private static Coroutines Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new("[COROUTINES]");
                    _instance = go.AddComponent<Coroutines>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        public static UnityEngine.Coroutine StartRoutine(IEnumerator enumerator) => Instance.StartCoroutine(enumerator);
        public static void StopRoutine([NotNull] UnityEngine.Coroutine routine) => Instance.StopCoroutine(routine);
    }
}