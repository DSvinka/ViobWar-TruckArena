using Code.Game.Data.Vehicles;
using UnityEngine;

namespace Code.Game.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData", order = 0)]
    public class GameData: ScriptableObject
    {
        [SerializeField] private AiSettingsData _aiSettingsData;
        [SerializeField] private GameSettingsData _gameSettingsData;
        
        [SerializeField] private PlayerKeymapData _playerKeymapData;
        [SerializeField] private PlayerNetworkData _playerNetworkData;

        [SerializeField] private VehiclesData _vehiclesData;

        
        public AiSettingsData AISettingsData => _aiSettingsData;
        public GameSettingsData GameSettingsData => _gameSettingsData;

        public PlayerKeymapData PlayerKeymapData => _playerKeymapData;
        public PlayerNetworkData PlayerNetworkData => _playerNetworkData;

        public VehiclesData VehiclesData => _vehiclesData;
    }
}