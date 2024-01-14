using System.Collections;

namespace KimicuYandexGames.Utils
{
    public class Coroutine
    {
        private UnityEngine.Coroutine _routine;

        /// <summary> Start new coroutine. </summary>
        public void StartRoutine(IEnumerator enumerator)
        {
            StopRoutine();
            _routine = Coroutines.StartRoutine(enumerator);
        }

        /// <summary> Try start new coroutine. </summary>
        public bool TryStartRoutine(IEnumerator enumerator)
        {
            if (_routine != null) return false;
            _routine = Coroutines.StartRoutine(enumerator);
            return true;
        }

        /// <summary> Stop current coroutine. </summary>
        public void StopRoutine()
        {
            if (_routine == null) return;
            Coroutines.StopRoutine(_routine);
            _routine = null;
        }
    }
}