using System.Collections.Generic;
using Code.Game.Abstractions.Controller;
using UnityEngine;

namespace Code.Core.Singleton
{
    public class SingletonManager : MonoBehaviour, ISingleton, ICleanup
    {
        private HashSet<GameObject> _awoken;
        private HashSet<GameObject> _dontDestroyOnLoad;
        public HashSet<GameObject> Awoken => _awoken ??= new HashSet<GameObject>();
        public new HashSet<GameObject> DontDestroyOnLoad => _dontDestroyOnLoad ??= new HashSet<GameObject>();

        public void Cleanup(bool isMaster)
        {
            Singleton<SingletonManager>.IsInited = false;
           
            foreach (var single in _awoken)
            {
                Destroy(single);
            }
            
            _awoken.Clear();
        }
    }
}