using Mirror;
using UnityEngine;

namespace Deadfront
{
    // TODO: this class can be later refactored to stop being a Singleton and be similar to a factory, holding
    // references to many instances that can be delivered based on the scene or some other network criteria.
    // This is to achieve having multiple games running on the same server.
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
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

                            if (Application.isPlaying && (_instance as Singleton<T>).dontDestroyOnLoad)
                                DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }
    }
}