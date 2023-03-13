using Code.Game.Data.Vehicles;
using Code.Game.Views;

namespace Code.Game.Models
{
    public class TurretModel
    {
        private readonly TurretData _turretData;
        private readonly TurretView _turretView;

        
        public int AmmoCount { get; set; }
        public float ShootDelayTime { get; set; } = -1;
        public float ReloadTime { get; set; } = -1;

        public TurretData TurretData => _turretData;
        public TurretView TurretView => _turretView;


        public TurretModel(TurretData turretData, TurretView turretView)
        {
            _turretData = turretData;
            _turretView = turretView;

            AmmoCount = turretData.MaxAmmoCount;
        }
    }
}