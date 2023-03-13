using UnityEngine;

namespace Code.Game.Data.Vehicles
{
    [CreateAssetMenu(fileName = "VehiclesData", menuName = "Data/VehiclesData", order = 0)]
    public class VehiclesData: ScriptableObject
    {
        [SerializeField] private VehicleData[] _vehicles;


        public VehicleData[] Vehicles => _vehicles;
    }
}