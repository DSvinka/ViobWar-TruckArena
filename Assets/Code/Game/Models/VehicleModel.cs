using System;
using System.Collections.Generic;
using Code.Game.Data.Vehicles;
using Code.Game.Views;

namespace Code.Game.Models
{
    public class VehicleModel
    {
        public event Action<VehicleData, VehicleView> OnVehicleChange;
        
        
        private Dictionary<VehicleTurretRack, TurretModel> _turrets;

        private VehicleData _vehicleData;
        private VehicleView _vehicleView;
        

        public VehicleData VehicleData => _vehicleData;
        public VehicleView VehicleView => _vehicleView;
        

        public Dictionary<VehicleTurretRack, TurretModel> Turrets => _turrets;

        public float Fuel { get; set; }
        public float Health { get; set; }
        public float Speed { get; set; }


        public void SetVehicle(VehicleView vehicleView, VehicleData vehicleData)
        {
            _vehicleView = vehicleView;
            _vehicleData = vehicleData;

            Health = vehicleData.Health;
            
            _turrets = new Dictionary<VehicleTurretRack, TurretModel>();

            foreach (var turretRack in vehicleView.TurretRacks)
            {
                _turrets.Add(turretRack, null);
            }
            
            OnVehicleChange?.Invoke(_vehicleData, _vehicleView);
        }
    }
}