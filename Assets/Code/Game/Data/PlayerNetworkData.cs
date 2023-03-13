using UnityEngine;

namespace Code.Game.Data
{
    [CreateAssetMenu(fileName = "PlayerNetworkData", menuName = "Data/Player/PlayerNetworkData", order = 0)]
    public class PlayerNetworkData : ScriptableObject
    {
        [SerializeField] private string _playerPrefabPath;

        public string PlayerPrefabPath => _playerPrefabPath;
    }
}