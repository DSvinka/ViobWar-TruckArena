using UnityEngine;

namespace Code.Core.Singleton
{
    public class Singleton<T> where T : MonoBehaviour, ISingleton
    {
        private static T _instance;
        public static bool IsDestroyOnLoad { get; private set; }

        public static T Instance
        {
            get
            {
                if (_instance is null)
                    Init();
                return _instance;
            }
        }

        public static bool IsInited { get; set; }

        private Singleton()
        {
        }

        /// <param name="name"> Name of object, showing on scene </param>
        /// <param name="isToDestroyOnLoad"> True for not destroy on load </param>
        public static T Init(string name = "", bool isToDestroyOnLoad = true)
        {
            if (!Singleton<SingletonManager>.IsInited && typeof(T).FullName != typeof(SingletonManager).FullName)
            {
                Singleton<SingletonManager>.Init("Singletons");

                Singleton<SingletonManager>.Instance.Awoken.Add(Singleton<SingletonManager>.Instance.gameObject);
            }

            IsDestroyOnLoad = isToDestroyOnLoad;
            if (Singleton<SingletonManager>.IsInited)
            {
                if (IsInited==true)
                {
                    Debug.LogError($"Уже существует одиночка типа {typeof(T)}");
                    return _instance;
                }
            }

            IsInited = true;
            _instance = new GameObject(name == "" ? $"{typeof(T)}" : name).AddComponent<T>();
            
            if (typeof(T).FullName == typeof(SingletonManager).FullName) return _instance;
            
            if (isToDestroyOnLoad)
            {
                _instance.transform.SetParent(Singleton<SingletonManager>.Instance.transform);
                Singleton<SingletonManager>.Instance.Awoken.Add(_instance.gameObject);
            }
            else
            {
                Singleton<SingletonManager>.Instance.DontDestroyOnLoad.Add(_instance.gameObject);
            }

            if (!isToDestroyOnLoad)
                Object.DontDestroyOnLoad(_instance);
            
            return _instance;
        }
    }
}