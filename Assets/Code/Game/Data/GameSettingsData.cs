using Code.Game.Data.Vehicles;
using UnityEngine;

namespace Code.Game.Data
{
    [CreateAssetMenu(fileName = "GameSettingsData", menuName = "Data/Settings/GameSettingsData", order = 0)]
    public class GameSettingsData: ScriptableObject
    {
        [SerializeField] private VehicleData _debugDefaultVehicle;
        [SerializeField] private float _unitAimingRange = 300f;
        
        [SerializeField, Space] private LayerMask _vehicleLayerMask;
        [SerializeField] private LayerMask _pickupItemLayerMask;

        [SerializeField, Space] private int _killMoneyAwardCount = 50;
        [SerializeField] private int _winMoneyAwardCount = 250;
        [SerializeField] private int _itemPickupMoneyAwardCount = 10;
        
        
        public VehicleData DebugDefaultVehicle => _debugDefaultVehicle;
        public float UnitAimingRange => _unitAimingRange;
        
        public LayerMask VehicleLayerMask => _vehicleLayerMask;
        public LayerMask PickupItemLayerMask => _pickupItemLayerMask;
        
        public int KillMoneyAwardCount => _killMoneyAwardCount;
        public int WinMoneyAwardCount => _winMoneyAwardCount;
        public int ItemPickupMoneyAwardCount => _itemPickupMoneyAwardCount;
    }
}