using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace vc
{
    public class CoroutineHelper
    {
        static GameObject _gameObject = null;
        static crHelper _coroutineHelper = null;
        static string _gameObjectName = "CoroutineHelper";

        public static Coroutine ExecuteAfterDelay(float delay, Action action)
        {
            // lazy initialize a new game object to attach a mono behaviour too 
            CheckOrCreateActiveMonobehaviour();
            // use the monobehaviour  and attach a co-routine
            return _coroutineHelper.StartCoroutine(RunActionAfterWait(delay, action));
        }

        static IEnumerator RunActionAfterWait(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }

        // check if we have a monobehaviour  that we can run co-routines against
        // if not, create it.
        private static void CheckOrCreateActiveMonobehaviour()
        {
            if (_gameObject == null)
            {
                _gameObject = new GameObject(_gameObjectName);
                _coroutineHelper = _gameObject.AddComponent<crHelper>();
                MonoBehaviour.DontDestroyOnLoad(_gameObject);
            }
        }

        public class crHelper : MonoBehaviour
        {

            // stop all co-routines when this game object is destroyed
            // https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.StopAllCoroutines.html
            private void OnDestroy()
            {
                StopAllCoroutines();
            }
        }
    }
}