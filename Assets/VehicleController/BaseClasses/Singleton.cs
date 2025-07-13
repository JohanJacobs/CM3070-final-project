using UnityEngine;

namespace vc
{
    public class Singleton<T> : MonoBehaviour where T: Component
    {
        protected static T _instance;
                
        public static T Instance
        { 
            get
            {
                // use lazy initialization for the instance 
                if (_instance==null)
                {
                    _instance = FindAnyObjectByType<T>();

                    // if the instance dont exist then create it 
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + "Generated");
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;                
            }
        }

        protected virtual void Awake()
        {
            InitSingleton();
        }

        protected virtual void InitSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _instance = this as T;

        }
    }
    
}