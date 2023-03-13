using UnityEngine;

namespace Code.Game.Views
{
    public class MapView: MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;

        public Transform[] SpawnPoints => _spawnPoints;
    }
}