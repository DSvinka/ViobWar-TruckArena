using Code.Game.Data.Vehicles;
using Photon.Pun;
using UnityEngine;

namespace Code.Game.Views
{
    public class TurretBoxView: MonoBehaviourPun
    {
        [SerializeField] private TurretData _turret;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<VehicleView>(out var vehicleView)) 
                return;

            vehicleView.SetTurretToRandomRack(_turret);
        }
    }
}