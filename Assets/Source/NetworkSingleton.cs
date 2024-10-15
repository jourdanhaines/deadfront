using Mirror;
using UnityEngine;

namespace ColonyCrisis
{
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : MonoBehaviour
    {
        private static object _lock = new();
        private static T _instance;
        [SerializeField] private bool dontDestroyOnLoad = true;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _lock = null;
            _instance = null;
        }

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindAnyObjectByType(typeof(T));
                        if (_instance == null)
                        {
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T) + " (Singleton)";

                            if (Application.isPlaying && (_instance as NetworkSingleton<T>).dontDestroyOnLoad)
                                DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }
    }
}